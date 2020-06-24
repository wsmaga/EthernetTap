using Kaitai;
using NetCon.model;
using NetCon.parsing;
using NetCon.repo;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NetCon.viewmodel
{
    class CapturePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mMainWindowSharedViewModel;
        private IFrameRepository<Frame> mFramesRepository = FrameRepositoryImpl.instance;
        private FrameParser frameParser;
        private int port = 0;

        public bool initError { get; set; } = false;
        public bool isCapturing { get; set; } = false;
        public int FramesCounter { get; set; } = 0;
        public int BufferSize { get; set; } = 0;

        public CapturePageViewModel(MainWindowViewModel sharedViewModel)
        {
            mMainWindowSharedViewModel = sharedViewModel;
            frameParser = new FrameParser(mFramesRepository.FrameSubject);
            //Observing subjects
           /* new SubjectObserver<Frame>(frame =>
            {
                string rawDataString = BitConverter.ToString(frame.RawData).Replace("-", "").ToLower();
                frameParser.Parse(rawDataString);
                //FramesCounter=frameParser.AllFrames.Count;
            }).Subscribe(mFramesRepository.FrameSubject);*/

            new SubjectObserver<Frame>(frame =>
            {
                FramesCounter++;
            }).Subscribe(frameParser.EthernetFrameSubject);

            new SubjectObserver<CaptureState>(state =>
            {
                if (state is CaptureState.CaptureInitialized)
                {
                    mMainWindowSharedViewModel.logInfo("Połączenie nawiązane");
                }
                if (state is CaptureState.CaptureOn)
                {
                    isCapturing = true;
                    mMainWindowSharedViewModel.logAction("Rozpoczęto przechwytywanie ramek");
                }
                else if (state is CaptureState.CaptureOff)
                {
                    isCapturing = false;
                    mMainWindowSharedViewModel.logInfo("Zakończono przechwytywanie ramek");
                }
                else if (state is CaptureState.CaptureError)
                {
                    isCapturing = false;
                    initError = true;   //TODO sprawdzić typ wyjątku i ustawiać init error tylko jeśli wystapi problem z inicjalizacją
                    mMainWindowSharedViewModel.logAction(((CaptureState.CaptureError)state).Error.Message);
                }
            }).Subscribe(mFramesRepository.CaptureState);


            //Setup capture 

            ApplicationConfig config = ConfigFileHandler<ApplicationConfig>.ReadSettings();

            if(config == null)
            {
                mMainWindowSharedViewModel.logAction("Błąd podczas wczytywania ustawień. Załadowano wartości domyślne");
                config = new ApplicationConfig
                {
                    port = 3,
                    bufferSize = 16
                };
            }

            BufferSize = config.bufferSize;
            port = config.port;

            Task.Run(() => mFramesRepository.InitCapture());

        }

        public String PortText
        {
            get => port.ToString();
            set
            {
                try
                {
                    port = Int32.Parse(value);
                }
                catch (Exception e){}
            }
        }

        private ICommand _startButtonCommand;
        public ICommand StartButtonCommand
        {
            get => _startButtonCommand ?? (_startButtonCommand = new CommandHandler(
                    () =>
                    {
                        startCapture();
                    },
                    () => { return !isCapturing && !initError; }));
        }

        private ICommand _stopButtonCommand;
        public ICommand StopButtonCommand
        {
            get => _stopButtonCommand ?? (_stopButtonCommand = new CommandHandler(
                    () =>
                    {
                        stopCapture();
                    },
                    () => { return isCapturing; }));
        }

        private ICommand _saveChangesButtonCommand;
        public ICommand SaveChangesButtonCommand
        {
            get => _saveChangesButtonCommand ?? (_saveChangesButtonCommand = new CommandHandler(
                    () => {
                        ConfigFileHandler<ApplicationConfig>.WriteSettings(new ApplicationConfig
                        {
                            port = this.port,
                            bufferSize = this.BufferSize
                        }
                        );
                        MessageBox.Show(
                            "Zmiany zostaną zastosowane po ponownym uruchomieniu aplikacji",
                            "NetCon v2",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    },
                    () => { return true; }
                    ));
        }


        private void startCapture()
        {
            //TODO przeciążyć start capture o port i rozmiar bufora
            mFramesRepository.StartCapture();
        }

        private void stopCapture()
        {
            mFramesRepository.StopCapture();
        }
    }
}
