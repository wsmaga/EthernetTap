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

            Task.Run(() => mFramesRepository.startCapture());

            new SubjectObserver<Frame>(frame =>
            {
                FramesCounter++;
            }).Subscribe(mFramesRepository.FrameSubject);

            new SubjectObserver<CaptureState>(state =>
            {
                if (state is CaptureState.CaptureOn)
                {
                  //  isCapturing = true;
                    mMainWindowSharedViewModel.logAction("Rozpoczęto przechwytywanie ramek");
                }
                else if (state is CaptureState.CaptureOff)
                {
                  //  isCapturing = false;
                    mMainWindowSharedViewModel.logInfo("Zakończono przechwytywanie ramek");
                }
                else if (state is CaptureState.CaptureError)
                {
                    mMainWindowSharedViewModel.logAction(((CaptureState.CaptureError)state).Error.Message);
                  //  isCapturing = false;
                }
            }).Subscribe(mFramesRepository.CaptureState);
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
                }
                catch (Exception e)
                {
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
                    () =>
                    {
                        startCapture();
                    },
                    () => { return !isCapturing; }));
            }
        }

        private ICommand _stopButtonCommand;
        public ICommand StopButtonCommand
        {
            get
            {
                return _stopButtonCommand ?? (_stopButtonCommand = new CommandHandler(
                    () =>
                    {
                        stopCapture();
                    },
                    () => { return !isCapturing; }));
            }
        }

        private void startCapture()
        {
            //TODO przeciążyć start capture o port i rozmiar bufora
            mFramesRepository.resumeCapture();
        }

        private void stopCapture()
        {
            mFramesRepository.pauseCapture();
           // mFramesRepository.stopCapture();
        }



        public int FramesCounter { get; set; } = 0;
        public int BufferSize { get; set; } = 0;
    }
}
