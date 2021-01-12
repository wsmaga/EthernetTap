using NetCon.hardwareFilters;
using NetCon.model;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace NetCon.viewmodel
{
    class HardwareFilterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mainWindowViewModel;

        private HardwareFilterSenderService hardwareFilterSenderService = HardwareFilterSenderService.instance;

        private string portText = "";
        private string minFrameLength = "";
        private string filter1 = "";
        private string filter2 = "";
        private string filter3 = "";
        private string filter4 = "";

        private ICommand _saveChangesCommand;

        private ICommand _loadFileCommand;

        private ICommand _saveFileCommand;

        public string PortText 
        { 
            get => portText;

            set 
            {
                portText = value;
                validateForm(); 
            } 
        }
        public string MinFrameLength 
        {
            get => minFrameLength;

            set 
            {
                minFrameLength = value;
                validateForm();
            }
        }
        public string Filter1 
        { 
            get => filter1;
            set
            {
                filter1 = value;
                validateForm();
            }
        }

        public string Filter2
        {
            get => filter2;
            set
            {
                filter2 = value;
                validateForm();
            }
        }

        public string Filter3
        {
            get => filter3;
           
            set
            {
                filter3 = value;
                validateForm();
            }
        }

        public string Filter4
        {
            get => filter4;
            set
            {
                filter4 = value;
                validateForm();
            }
        }
        public bool isFormValid { get; set; }

        public ICommand SaveChangesCommand
        {
            get
            {
                return _saveChangesCommand ?? (_saveChangesCommand = new CommandHandler(
                    () => {
                        List<string> args = createArguments();
                        sendFilters(args);
                    },
                    () => {
                        return isFormValid;
                    }));
            }
        }

        public ICommand LoadFileCommand
        {
            get
            {
                return _loadFileCommand ?? (_loadFileCommand = new CommandHandler(
                    () => loadFromFile(),
                    () => true
                    ));
            }
        }

        public ICommand SaveFileCommand
        {
            get
            {
                return _saveFileCommand ?? (_saveFileCommand = new CommandHandler(
                    () => saveToFile(),
                    () => isFormValid
                    ));
            }
        }

        public HardwareFilterViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        private void validateForm()
        {
            isFormValid = isValidNumber(portText) &&
                isValidNumber(minFrameLength) &&
                (filter1.Length > 0 ||
                 (filter1.Length > 0 && filter2.Length > 0) ||
                 (filter1.Length > 0 && filter2.Length > 0 && filter3.Length > 0) ||
                 (filter1.Length > 0 && filter2.Length > 0 && filter3.Length > 0 && filter4.Length > 0));
        }

        private bool isValidNumber(string value)
        {
            int res = 0;
            return value.Length > 0 && Int32.TryParse(value, out res) && res > 0;
        }

        private List<string> createArguments()
        {
            List<string> args = new List<string>();

            args.Add(portText);


            if(filter1.Length > 0)
            {
                args.Add(minFrameLength);
                args.Add($"\"{filter1}\"");
            }

            if (filter2.Length > 0)
            {
                args.Add(minFrameLength);
                args.Add($"\"{filter1}\"");
            }

            if (filter3.Length > 0)
            {
                args.Add(minFrameLength);
                args.Add($"\"{filter1}\"");
            }

            if (filter4.Length > 0)
            {
                args.Add(minFrameLength);
                args.Add($"\"{filter1}\"");
            }

            return args;
        }

        private void sendFilters(List<string> args)
        {
            hardwareFilterSenderService.sendFilters(args);
            mainWindowViewModel.logInfo($"Wysłano filtry {args}");
        }

        private void loadFromFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "pliki XML (*.xml)|*.xml";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    HardwareFiltersConfigFileHandler handler = new HardwareFiltersConfigFileHandler(dialog.FileName);
                    HardwareFiltersDefinition definition = handler.ReadSettings();
                    fillInputsFromDefinition(definition);
                }
                catch ( Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                
            }
        }

        private void saveToFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "pliki XML (*.xml)|*.xml";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                HardwareFiltersConfigFileHandler handler = new HardwareFiltersConfigFileHandler(dialog.FileName);
                handler.WriteSettings(getHardwareFiltersDefinitionFromInputs());
            }
        }

        private HardwareFiltersDefinition getHardwareFiltersDefinitionFromInputs()
        {
            HardwareFiltersDefinition definition = new HardwareFiltersDefinition();
            definition.Port = Int32.Parse(portText);
            definition.MinFrameLength = Int32.Parse(minFrameLength);
            definition.Filter1 = filter1;
            definition.Filter2 = filter2;
            definition.Filter3 = filter3;
            definition.Filter4 = filter4;
            return definition;
        }

        private void fillInputsFromDefinition(HardwareFiltersDefinition definition)
        {
            PortText = definition.Port.ToString();
            MinFrameLength = definition.MinFrameLength.ToString();
            Filter1 = definition.Filter1;
            Filter2 = definition.Filter2;
            Filter3 = definition.Filter3;
            Filter4 = definition.Filter4;
        }
    }
}
