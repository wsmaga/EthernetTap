using NetCon.export;
using NetCon.model;
using NetCon.repo;
using NetCon.util;
using System.ComponentModel;

namespace NetCon.viewmodel
{
    class ExportPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mainWindowSharedViewModel;
        private IPcapWriter<Frame> mPcapWriter = new PcapWriterImpl();

        private IFrameRepository<Frame> mFramesRepository = FrameRepositoryImpl.instance;

        private bool _fileExportOption = false;
        public bool FileExportOption
        {
            get => _fileExportOption;
            set
            {
                _fileExportOption = value;
                if (value)
                {
                    mainWindowSharedViewModel.logInfo("Zaznaczono opcję eksportu do pliku *.pcap");
                }
                else
                {
                    mainWindowSharedViewModel.logInfo("Odznaczono opcję eksportu do pliku *.pcap");
                }

            }
        }

        public ExportPageViewModel(MainWindowViewModel sharedViewModel)
        {
            mainWindowSharedViewModel = sharedViewModel;

            SetupPcapWriter();

            
        }

        private void SetupPcapWriter()
        {
            new SubjectObserver<Frame>(frame =>
            {
                if (mPcapWriter.isInitialized)
                {
                    mPcapWriter.WriteFrame(frame);
                }
            }).Subscribe(mFramesRepository.FrameSubject);

            new SubjectObserver<CaptureState>(state =>
            {
                if (state is CaptureState.CaptureOn)
                {
                    if (!mPcapWriter.isInitialized)
                        mPcapWriter.InitWrite("myFrames.pcap");
                }
                else if (state is CaptureState.CaptureOff)
                {
                    if (mPcapWriter.isInitialized)
                        mPcapWriter.EndWrite();
                }
            }).Subscribe(mFramesRepository.CaptureState);
        }

    }
}
