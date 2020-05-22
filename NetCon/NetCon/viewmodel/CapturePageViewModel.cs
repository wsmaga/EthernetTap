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
    class CapturePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mainWindowSharedViewModel;

        public CapturePageViewModel(MainWindowViewModel sharedViewModel)
        {
            mainWindowSharedViewModel = sharedViewModel;
        }

        private ICommand _startButtonCommand;
        private ICommand _stopButtonCommand;

        private bool isCapturing = false;


        public ICommand StartButtonCommand
        {
            get
            {
                return _startButtonCommand ?? (_startButtonCommand = new CommandHandler(
                    () => {
                        mainWindowSharedViewModel.setBottomInfoBar("Rozpoczęto przechwytywanie ramek", MainWindow.ACTION_COLOR);
                        startCapture();
                    },
                    () => { return !isCapturing; })) ;
            }
        }

        public ICommand StopButtonCommand
        {
            get
            {
                return _stopButtonCommand ?? (_stopButtonCommand = new CommandHandler(
                    () => {
                        mainWindowSharedViewModel.setBottomInfoBar("Zakończono przechwytywanie ramek", MainWindow.INFO_COLOR);
                        stopCapture();
                    },
                    () => { return isCapturing; }));
            }
        }



        private void startCapture()
        {

            isCapturing = true;
            Task.Run(async () =>
            {
                while (isCapturing)
                {
                    FramesCounter++;
                    await Task.Delay(100);
                }
            });
        }

        private void stopCapture()
        {
            isCapturing = false;
        }

        public string HelloText { get; set; } = "Hello Capture!";

        public int FramesCounter { get; set; } = 0;

        public int BufferSize { get; set; } = 0;

    }
}
