using NetCon.util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetCon.ViewModel
{
    class FiltersPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainWindowViewModel mainWindowSharedViewModel;
        public ObservableCollection<string> filterDefinitions { get; private set; } = new ObservableCollection<string>();
        public string newFilterText { get; set; }
        public string selectedFilterDefiniton { get; set; }

        private ICommand _addFilterCommand;
        public ICommand AddFilterCommand
        {
            get
            {
                return _addFilterCommand ?? (_addFilterCommand = new CommandHandler(
                    () =>
                    {
                        filterDefinitions.Add(newFilterText);
                        mainWindowSharedViewModel.logInfo($"Dodano filtr {newFilterText}");
                    },

                    ()=> 
                    { return true; }));
            }
        }

        private ICommand _deleteFilterCommand;
        public ICommand DeleteFilterCommand
        {
            get
            {
                return _deleteFilterCommand ?? (_deleteFilterCommand = new CommandHandler(
                    () =>
                    {
                        mainWindowSharedViewModel.logInfo($"Usunięto filtr {selectedFilterDefiniton}");
                        filterDefinitions.Remove(selectedFilterDefiniton);
                    },
                    ()=> {
                        return filterDefinitions.Count >0 &&
                               selectedFilterDefiniton!=null;
                    }));
            }
        }

        private ICommand _shiftUpFilterCommand;
        public ICommand ShiftUpFilterCommand
        {
            get
            {
                return _shiftUpFilterCommand ?? (_shiftUpFilterCommand = new CommandHandler(
                    () => { 
                        filterDefinitions.Move(filterDefinitions.IndexOf(selectedFilterDefiniton), filterDefinitions.IndexOf(selectedFilterDefiniton) - 1);
                        mainWindowSharedViewModel.logInfo($"Przesunięto filtr {selectedFilterDefiniton} na pozycję {filterDefinitions.IndexOf(selectedFilterDefiniton)}");
                    },
                    () => { return filterDefinitions.IndexOf(selectedFilterDefiniton) > 0; }
                    ));
            }
        }
        private ICommand _shiftDownFilterCommand;
        public ICommand ShiftDownFilterCommand
        {
            get
            {
                return _shiftDownFilterCommand ?? (_shiftDownFilterCommand = new CommandHandler(
                    () => {
                        filterDefinitions.Move(filterDefinitions.IndexOf(selectedFilterDefiniton), filterDefinitions.IndexOf(selectedFilterDefiniton) + 1);
                        mainWindowSharedViewModel.logInfo($"Przesunięto filtr {selectedFilterDefiniton} na pozycję {filterDefinitions.IndexOf(selectedFilterDefiniton)}");
                    },
                    () => { return 
                        selectedFilterDefiniton !=null && 
                        filterDefinitions.Count>1 && 
                        filterDefinitions.IndexOf(selectedFilterDefiniton) < filterDefinitions.Count-1; }
                    ));
            }
        }

        public FiltersPageViewModel(MainWindowViewModel sharedViewModel)
        {
            mainWindowSharedViewModel = sharedViewModel;
        }

    }

}
