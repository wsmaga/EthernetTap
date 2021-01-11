using NetCon.deviceConfig;
using NetCon.inter;
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
    class EthernetConfigurationViewModel: INotifyPropertyChanged
    {
        private MainWindowViewModel sharedViewModel;

        private DeviceConfigService deviceConfigService;

        public event PropertyChangedEventHandler PropertyChanged;


        private ICommand _resetClickCommand;
        private ICommand _saveClickCommand;

        private String port1SpeedValue;
        private String port2SpeedValue;
        private String port3SpeedValue;
        private String port4SpeedValue;

        private bool eeeEnabled;


        public String Port1SpeedValue { get; set; }
        public String Port2SpeedValue { get; set; }
        public String Port3SpeedValue { get; set; }
        public String Port4SpeedValue { get; set; }

        public bool EeeEnabled { get; set; }

        public EthernetConfigurationViewModel(MainWindowViewModel _sharedViewModel)
        {
            this.sharedViewModel = _sharedViewModel;

            this.deviceConfigService = new DeviceConfigService(new NetConImpl());
        }


        public ICommand ResetClickCommand
        {
            get
            {
                return _resetClickCommand ?? (_resetClickCommand = new CommandHandler(
                    () => { resetClick(); },
                    () => { return true; }));
            }
        }


        public ICommand SaveClickCommand
        {
            get
            {
                return _saveClickCommand ?? (_saveClickCommand = new CommandHandler(
                    () => { saveClick(); },
                    () => { return true; }));
            }
        }


        private void resetClick()
        {
            deviceConfigService.ResetDevice();
        }

        private void saveClick()
        {
            deviceConfigService.SetEthernetParametersForPort(1, speedComboValueToEthernetSpeed(Port1SpeedValue));
            deviceConfigService.SetEthernetParametersForPort(2, speedComboValueToEthernetSpeed(Port2SpeedValue));
            deviceConfigService.SetEthernetParametersForPort(3, speedComboValueToEthernetSpeed(Port3SpeedValue));
            deviceConfigService.SetEthernetParametersForPort(4, speedComboValueToEthernetSpeed(Port4SpeedValue));

            if (EeeEnabled)
            {
                deviceConfigService.EnableEEE();
            }
            else
            {
                deviceConfigService.DisableEEE();
            }
        }

        private EthernetSpeed speedComboValueToEthernetSpeed(string value)
        {
            switch (value)
            {
                case "10": return EthernetSpeed.SPEED_10;
                case "100": return EthernetSpeed.SPEED_100;
                case "Auto": return EthernetSpeed.AUTONEGOTIATION;
                default: return EthernetSpeed.AUTONEGOTIATION;
            }
        }
    }
}
