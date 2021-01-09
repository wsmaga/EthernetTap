using NetCon.hardwareFilters;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string PortText 
        { 
            get 
            { 
                return portText; 
            }

            set 
            {
                portText = value;
                validateForm(); 
            } 
        }
        public string MinFrameLength 
        {
            get 
            { 
                return minFrameLength; 
            }

            set 
            {
                minFrameLength = value;
                validateForm();
            }
        }
        public string Filter1 
        { 
            get
            {
                return filter1;
            }
            set
            {
                filter1 = value;
                validateForm();
            }
        }

        public string Filter2
        {
            get
            {
                return filter2;
            }
            set
            {
                filter2 = value;
                validateForm();
            }
        }

        public string Filter3
        {
            get
            {
                return filter3;
            }
            set
            {
                filter3 = value;
                validateForm();
            }
        }

        public string Filter4
        {
            get
            {
                return filter4;
            }
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
            args.Add(minFrameLength);

            string argument = "\"";

            if(filter1.Length > 0)
            {
                argument += $"{filter1};";
            }

            if (filter2.Length > 0)
            {
                argument += $" {filter2};";
            }

            if (filter3.Length > 0)
            {
                argument += $" {filter3};";
            }

            if (filter4.Length > 0)
            {
                argument += $" {filter4};";
            }

            argument += "\"";

            args.Add(argument);

            return args;
        }

        private void sendFilters(List<string> args)
        {
            hardwareFilterSenderService.sendFilters(args);
            mainWindowViewModel.logInfo($"Wysłano filtry {args}");
        }
    }
}
