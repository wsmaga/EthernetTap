using NetCon.export;
using NetCon.export.services;
using NetCon.filtering;
using NetCon.model;
using NetCon.repo;
using NetCon.util;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NetCon.viewmodel
{
    class ExportPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mainWindowSharedViewModel;
        private IPcapWriter<Frame> mPcapWriter = new PcapWriterImpl();

        private readonly IFrameRepository<Frame> mFramesRepository = FrameRepositoryImpl.instance;
        private readonly TargetDataRepository targetDataRepository = TargetDataRepository.GetInstance();

        private readonly DOMapper doMapper = DOMapper.GetInstance();
        private readonly DatabaseService databaseService = DatabaseService.GetInstance();

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

        private bool _databaseExportOption = false;
        public bool DatabaseExportOption
        {
            get => _databaseExportOption;
            set
            {
                _databaseExportOption = value;
                if (value)
                {
                    mainWindowSharedViewModel.logInfo("Zaznaczono opcję eksportu do bazy danych");
                }
                else
                {
                    mainWindowSharedViewModel.logInfo("Odznaczono opcję eksportu do bazy danych");
                }
            }
        }

        public bool SettingsChangeEnabled { get; set; } = true;

        private String _serverAddress = "(LocalDB)\\MSSQLLocalDB";
        public String ServerAddress
        {
            get => _serverAddress;
            set => _serverAddress = value;
        }

        private String _databaseName = "EthernetTap";
        public String DatabaseName
        {
            get => _databaseName;
            set => _databaseName = value;
        }

        private ICommand _testConnectionButtonCommand;
        public ICommand TestConnectionButtonCommand
        {
            get => _testConnectionButtonCommand ?? (_testConnectionButtonCommand = new CommandHandler(
                    () =>
                    {
                        SettingsChangeEnabled = false;
                        AllowUIToUpdate();
                        CheckDatabaseConnection();
                        SettingsChangeEnabled = true;
                    },
                    () =>
                    {
                        return DatabaseExportOption && SettingsChangeEnabled;
                    }
                    ));
        }

        public ExportPageViewModel(MainWindowViewModel sharedViewModel)
        {
            mainWindowSharedViewModel = sharedViewModel;

            SetupPcapWriter();
            SetupSettingsChangeEnabler();



            //var entityBuilder = new EntityConnectionStringBuilder();
            //string connectionString = string.Format(
            //    "Data source={0};Integrated Security=true;Database={1};Timeout=5;MultipleActiveResultSets=True;App=EntityFramework;attachdbfilename=C:\\Users\\Ola\\test.mdf;",
            //    ServerAddress,
            //    DatabaseName
            //);
            //entityBuilder.ProviderConnectionString = connectionString;
            //entityBuilder.Provider = "System.Data.SqlClient";

            //entityBuilder.Metadata = @"res://*/export.DBModel.csdl|res://*/export.DBModel.ssdl|res://*/export.DBModel.msl";



            //var serializer = new JavaScriptSerializer();
            //new SubjectObserver<TargetDataDto>(data =>
            //{
            //    Console.WriteLine(serializer.Serialize(data));
            //    using (DBModelContext dbModelContext = new DBModelContext(entityBuilder.ConnectionString))
            //    {
            //        var lel = dbModelContext.Database.CompatibleWithModel(true);
            //        Target target = doMapper.ToEntity(data);
            //        TargetName targetName = dbModelContext.TargetNameSet.Find(69);
            //        if (targetName == null)
            //        {
            //            targetName = new TargetName();
            //            targetName.id = 69;
            //            targetName.name = "lel";
            //            targetName.creationDate = DateTime.Now;
            //            targetName.modifyDate = DateTime.Now;
            //        }
            //        target.TargetName = targetName;
            //        dbModelContext.TargetSet.Add(target);
            //        dbModelContext.SaveChanges();
            //    }
            //}).Subscribe(targetDataRepository.FrameDataSubject);

        }

        private void SetupSettingsChangeEnabler()
        {
            new SubjectObserver<CaptureState>(state =>
            {
                if (state is CaptureState.CaptureOn)
                {
                    SettingsChangeEnabled = false;
                }
                else if (state is CaptureState.CaptureOff)
                {
                    SettingsChangeEnabled = true;
                }
            }).Subscribe(mFramesRepository.CaptureState);
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

        private void CheckDatabaseConnection()
        {
            if (String.IsNullOrWhiteSpace(ServerAddress) || String.IsNullOrWhiteSpace(DatabaseName))
            {
                MessageBox.Show(
                    "Adres serwera oraz nazwa bazy danych nie mogą być puste!",
                    "Uwaga!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            switch (databaseService.CheckConnection(ServerAddress, DatabaseName))
            {
                case DBConnectionStatus.OK:
                    MessageBox.Show("Udało się połączyć z podaną bazą danych.");
                    break;
                case DBConnectionStatus.BadServer:
                    MessageBox.Show(
                        "Nie udało się połączyć z podanym serwerem!",
                        "ERROR!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    break;
                case DBConnectionStatus.BadDB:
                    MessageBoxResult result = MessageBox.Show(
                        "Nie udało się połączyć z podaną bazą danych na serwerze! Podana baza jest niedostępna lub nie istnieje.\n\n" +
                        "Czy chcesz spróbować utworzyć nową bazę danych o podanej nazwie?",
                        "Uwaga!",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );
                    if (result == MessageBoxResult.Yes)
                        if (databaseService.InitializeDatabase(ServerAddress, DatabaseName))
                            MessageBox.Show("Udało się stworzyć nową bazę danych.");
                        else
                            MessageBox.Show(
                                "Nie udało się utworzyć bazy danych na podanym serwerze!",
                                "ERROR!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                    break;
                default:
                    break;
            }
        }

        private void AllowUIToUpdate()
        // https://stackoverflow.com/questions/37787388/how-to-force-a-ui-update-during-a-lengthy-task-on-the-ui-thread
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            System.Windows.Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(delegate { })
            );
        }

    }
}
