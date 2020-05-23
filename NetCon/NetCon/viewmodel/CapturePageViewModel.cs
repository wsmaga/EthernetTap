using NetCon.model;
using NetCon.repo;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace NetCon.viewmodel
{
    class CapturePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mMainWindowSharedViewModel;

        private IFrameRepository<Frame> mFramesRepository = FrameRepositoryImpl.instance;
        public CapturePageViewModel(MainWindowViewModel sharedViewModel)
        {
            mMainWindowSharedViewModel = sharedViewModel;
        }

        private int port = 0;

        public String PortText
        {
            get
            {
                return port.ToString();
            }

            set
            {
                try
                {
                    port = Int32.Parse(value);
                }catch(Exception e) {
                    //TODO obsługa błędów
                }
                
            }
        }


        private bool isCapturing = false;

        private ICommand _startButtonCommand;
        public ICommand StartButtonCommand
        {
            get
            {
                return _startButtonCommand ?? (_startButtonCommand = new CommandHandler(
                    () => {
                        mMainWindowSharedViewModel.logAction("Rozpoczęto przechwytywanie ramek");
                        startCapture();
                    },
                    () => { return !isCapturing; })) ;
            }
        }

        private ICommand _stopButtonCommand;
        public ICommand StopButtonCommand
        {
            get
            {
                return _stopButtonCommand ?? (_stopButtonCommand = new CommandHandler(
                    () => {
                        mMainWindowSharedViewModel.logInfo("Zakończono przechwytywanie ramek");
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

            //TODO przeciążyć start capture o port i rozmiar bufora

            Task.Run(()=>mFramesRepository.startCapture());

            mFramesRepository.stopCapture();
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
