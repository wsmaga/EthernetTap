using NetCon.model;
using NetCon.repo;
using NetCon.ui;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.viewmodel
{
    class ExportPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mainWindowSharedViewModel;

        private bool _fileExportOption = false;
        public bool fileExportOption { get {
                return _fileExportOption;
            } set {
                _fileExportOption = value;
                if (value)
                {
                    mainWindowSharedViewModel.logInfo("Zaznaczono opcję eksportu do pliku *.pcap");
                }
                else
                {
                    mainWindowSharedViewModel.logInfo("Odznaczono opcję eksportu do pliku *.pcap");
                }
                
            } }

        public ExportPageViewModel(MainWindowViewModel sharedViewModel)
        {
            mainWindowSharedViewModel = sharedViewModel;

            new SubjectObserver<Frame>(frame =>
            {
                Console.WriteLine(frame);
            }).Subscribe(
            FrameRepositoryImpl.instance.FrameSubject);
        }

    }
}
