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
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NetCon.ViewModel
{
    /// <summary>
    /// Possible data export targets.
    /// </summary>
    public enum ExportTargetDesc
    {
        NONE,
        LocalDB,
        ExternalDB,
        Azure
    }
    
    /// <summary>
    /// Represents a single data export target combobox item.
    /// Value - enum value of an item. Desc - user-friendly description to be shown in combobox.
    /// </summary>
    public class ComboBoxItem
    {
        public ExportTargetDesc Value { get; set; }
        public String Desc { get; set; }
    }

    public class ExportPageViewModel : INotifyPropertyChanged
    {
        // TODO: Contain all exporters, their descriptions and enums in a single class.
        
        /// <summary>
        /// Data export target combobox items.
        /// </summary>
        public List<ComboBoxItem> ComboBoxItems { get; } = new List<ComboBoxItem>
        {
            new ComboBoxItem{ Value = ExportTargetDesc.LocalDB, Desc = "Lokalna baza MSSQL" },
            new ComboBoxItem{ Value = ExportTargetDesc.ExternalDB, Desc = "Zewnętrzna baza danych" },
            new ComboBoxItem{ Value = ExportTargetDesc.Azure, Desc = "Azure" }
        };

        internal bool CheckConnectionLocalDB()
        {
            return localDBExporter.CanConnect;
        }

        private ExportTargetDesc CurrentExportTarget { get; set; } = ExportTargetDesc.NONE;

        LocalDBExporter localDBExporter = new LocalDBExporter();

        internal void DEBUG_SendByte(byte data)
        {
            localDBExporter.StoreByte(data);
        }

        internal void SetCurrentExportTarget(ExportTargetDesc newTarget)
        {
            CurrentExportTarget = newTarget;
        }

        internal void SetMdfFilePath(string path)
        {
            localDBExporter.SetMdfFile(path);
        }

        private MainWindowViewModel mainWindowSharedViewModel;
        private IPcapWriter<model.Frame> mPcapWriter = new PcapWriterImpl();

        private IFrameRepository<model.Frame> mFramesRepository = FrameRepositoryImpl.instance;

        public String FileURLLabel { get; set; } = "";

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

        /// <summary>
        /// <see cref="FileExportOption"/>
        /// </summary>
        private bool _fileExportOption = false;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// If true - frames will be exported to a *.pcap file.
        /// </summary>
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

            new SubjectObserver<model.Frame>(frame =>
            {
                if (mPcapWriter.isInitialized && FileExportOption)
                {
                    mPcapWriter.WriteFrame(frame);
                }
            }).Subscribe(mFramesRepository.FrameSubject);

            new SubjectObserver<model.Frame>(frame =>
            {
                if (StorageExportOption)
                {
                    switch (CurrentExportTarget)
                    {
                        case ExportTargetDesc.LocalDB:
                            switch (frame.usefulDataType)
                            {
                                case DataType.Bytes:
                                    if (!localDBExporter.StoreByte(frame.usefulData[0]))
                                        mainWindowSharedViewModel.logError("Some or all data couldn't be stored!");
                                    break;
                                // TODO: Add remaining data types.
                            }
                            break;
                            // TODO: Add remaining exporters.
                    }
                }
            }).Subscribe(mFramesRepository.EthernetFrameSubject);

            new SubjectObserver<CaptureState>(state =>
            {
                if(state is CaptureState.CaptureOn)
                {
                    if (!mPcapWriter.isInitialized && FileExportOption)
                        mPcapWriter.InitWrite(FileExportName);

                    IsExportEnabled = false;
                    if (StorageExportOption)
                    {
                        switch (CurrentExportTarget)
                        {
                            case ExportTargetDesc.LocalDB:
                                localDBExporter.StartNewStoring();
                                break;
                                // TODO: Add remaining exporters.
                        }
                    }
                }
                else if(state is CaptureState.CaptureOff)
                {
                    if (mPcapWriter.isInitialized)
                        mPcapWriter.EndWrite();

                    if (StorageExportOption)
                    {
                        switch (CurrentExportTarget)
                        {
                            case ExportTargetDesc.LocalDB:
                                if (!localDBExporter.FinishStoring())
                                    mainWindowSharedViewModel.logError("Some or all data couldn't be stored!");
                                break;
                                // TODO: Add remaining exporters.
                        }
                    }
                    
                    IsExportEnabled = true;
                }
            }).Subscribe(mFramesRepository.CaptureState);
        }

    }
}
