using NetCon.export;
using NetCon.model;
using NetCon.repo;
using NetCon.ui;
using NetCon.ui.Export_pages;
using NetCon.util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NetCon.viewmodel
{
    public class ExportPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mainWindowSharedViewModel;
        private IPcapWriter<model.Frame> mPcapWriter = new PcapWriterImpl();

        private IFrameRepository<model.Frame> mFramesRepository = FrameRepositoryImpl.instance;

        public List<Page> Pages { get; } = new List<Page>();

        public String FileURL { get; set; } = null;

        /// <summary>
        /// Name of the frames export file, used by IPcapWriterImpl.
        /// </summary>
        public string FileExportName { get; set; } = "capturedFrames.pcap";

        /// <summary>
        /// When false - disables export so it's impossible to modify config during capturing.
        /// </summary>
        public bool IsExportEnabled { get; set; } = true;

        /// <summary>
        /// <see cref="StorageExportOption"/>
        /// </summary>
        private bool _storageExportOption = false;
        /// <summary>
        /// If true - data will be exported to a data storage system like DB, Azure, etc.
        /// </summary>
        public bool StorageExportOption
        {
            get => _storageExportOption;
            set
            {
                _storageExportOption = value;
                if(value)
                    mainWindowSharedViewModel.logInfo("Zaznaczono opcję eksportu do systemu magazynowania danych");
                else
                    mainWindowSharedViewModel.logInfo("Odznaczono opcję eksportu do systemu magazynowania danych");
            }
        }

        private bool _fileExportOption = false;
        public bool FileExportOption
        { 
            get => _fileExportOption;
            set 
            {
                _fileExportOption = value;
                if (value)
                {
                    mainWindowSharedViewModel.logInfo("Zaznaczono opcję eksportu do pliku " + FileExportName);
                }
                else
                {
                    mainWindowSharedViewModel.logInfo("Odznaczono opcję eksportu do pliku " + FileExportName);
                }
                
            }
        }

        public ExportPageViewModel(MainWindowViewModel sharedViewModel)
        {
            // Shared viewmodel of main window:
            mainWindowSharedViewModel = sharedViewModel;

            // Add all subpages:
            Pages.Add(new LocalDatabasePage(this));
            Pages.Add(new RemoteDatabasePage(this));
            Pages.Add(new AzurePage(this));

            new SubjectObserver<model.Frame>(frame =>
            {
                if (mPcapWriter.isInitialized && FileExportOption)
                {
                    mPcapWriter.WriteFrame(frame);
                }
            }).Subscribe(mFramesRepository.FrameSubject);

            new SubjectObserver<CaptureState>(state =>
            {
                if(state is CaptureState.CaptureOn)
                {
                    if (!mPcapWriter.isInitialized && FileExportOption)
                        mPcapWriter.InitWrite(FileExportName);

                    IsExportEnabled = false;
                }
                else if(state is CaptureState.CaptureOff)
                {
                    if (mPcapWriter.isInitialized)
                        mPcapWriter.EndWrite();

                    IsExportEnabled = true;
                }
            }).Subscribe(mFramesRepository.CaptureState);
        }

    }
}
