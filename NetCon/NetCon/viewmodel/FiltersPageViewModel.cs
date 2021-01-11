﻿using NetCon.filtering;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Input;

namespace NetCon.viewmodel
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
                        try
                        {
                            var newFilterNoWhitespace = new string(newFilterText.Where(c => !char.IsWhiteSpace(c)).ToArray());
                            FilterDto filterDto = FilterDto.Deserialize(newFilterNoWhitespace);
                            FilterDomain filterDomain = FilterDomain.New(filterDto);
                            if(filterDomain.Targets.Any(t=>t.Threshold?.Type==ThresholdType.NONE))
                            {
                                var dialogResult = MessageBox.Show("Jeden lub więcej target w filtrze posiada niepoprawny threshold. Podczas filtracji zostanie on zignorowany.\nCzy chcesz dodać ten filtr?", "Niepoprawny threshold", MessageBoxButtons.YesNo);
                                if (dialogResult != DialogResult.Yes)
                                    return;
                            }
                            if (filterDefinitions.IndexOf(newFilterNoWhitespace) ==-1)
                            { 
                                filterDefinitions.Add(newFilterNoWhitespace);
                                mainWindowSharedViewModel.logInfo($"Dodano filtr {newFilterNoWhitespace}");
                                newFilterText = "";
                            }
                            else
                                MessageBox.Show("Podany filtr już istnieje");
                        }
                        catch(ArgumentException ex)
                        {
                            MessageBox.Show($"Błędna definicja filtru\n{ex.Message}","Błąd dodawania filtru");
                        }
                            
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
                        int filterIndex = filterDefinitions.IndexOf(selectedFilterDefiniton);
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
                        int index = filterDefinitions.IndexOf(selectedFilterDefiniton);
                        filterDefinitions.Move(index, index - 1);
                        mainWindowSharedViewModel.logInfo($"Przesunięto filtr {selectedFilterDefiniton} na pozycję {index}");
                    },
                    () => { return selectedFilterDefiniton != null && filterDefinitions.Count > 1 &&
                        filterDefinitions.IndexOf(selectedFilterDefiniton) > 0; }
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
                        int index = filterDefinitions.IndexOf(selectedFilterDefiniton);
                        filterDefinitions.Move(index, index + 1);
                        mainWindowSharedViewModel.logInfo($"Przesunięto filtr {selectedFilterDefiniton} na pozycję {index}");
                    },
                    () => { return 
                        selectedFilterDefiniton !=null && 
                        filterDefinitions.Count>1 && 
                        filterDefinitions.IndexOf(selectedFilterDefiniton) < filterDefinitions.Count-1; }
                    ));
            }
        }

        private ICommand _applyFilters;
        public ICommand ApplyFilters
        {
            get
            {
                return _applyFilters ?? (_applyFilters = new CommandHandler(
                    () => {
                        
                        BinaryFormatter formatter = new BinaryFormatter();
                        FileStream stream = new FileStream("filters.bin", FileMode.Create);
                        try
                        {
                            formatter.Serialize(stream, filterDefinitions.ToList());
                            MessageBox.Show($"Poprawnie załadowano filtry","Załadowano filtry");
                            mainWindowSharedViewModel.logInfo($"Poprawnie załadowano filtry");
                        }
                        catch (SerializationException)
                        {
                            MessageBox.Show("Bład serializacji");
                            mainWindowSharedViewModel.logInfo($"Bład serializacji");
                        }
                        finally
                        {
                            stream.Close();
                        }
                    },
                    () => {
                        return filterDefinitions.Count > 0;
                    }
                    ));
            }
        }
        private ICommand _loadFilters;
        public ICommand LoadFilters
        {
            get
            {
                return _loadFilters ?? (_loadFilters = new CommandHandler(
                    () => {
                        List<string> loadedFilters=null;
                        if (File.Exists("filters.bin"))
                        {
                            FileStream stream = new FileStream("filters.bin", FileMode.Open);
                            try
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                loadedFilters = (List<string>)formatter.Deserialize(stream);
                                stream.Close();
                                ObservableCollection<string> newFilterDefinitions = new ObservableCollection<string>();
                                foreach (string s in loadedFilters)
                                {
                                    try
                                    {
                                        FilterDomain.New(FilterDto.Deserialize(s));
                                        newFilterDefinitions.Add(s);
                                        mainWindowSharedViewModel.logInfo($"Załadowano filtry");
                                    }
                                    catch(ArgumentException)
                                    {
                                        if (MessageBox.Show("Plik z filtrami jest uszkodzony. Czy chcesz usunąć stary plik?", "Błąd pliku", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                        {
                                            File.Delete("filters.bin");
                                        }
                                        return;
                                    }  
                                }
                                filterDefinitions = newFilterDefinitions;


                            }
                            catch (SerializationException)
                            {
                                if (MessageBox.Show("Deserializacja filtrów z pliku jest niemożliwa. Czy chcesz usunąć stary plik?", "Błąd deserializacji", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    try
                                    {
                                        File.Delete("filters.bin");
                                    }
                                    catch (IOException)
                                    {
                                        MessageBox.Show("Nie udało sie usunąć pliku. Plik jest otwarty w innym programie");
                                    }
                                    File.Delete("filters.bin");
                                }
                            }
                            finally
                            {
                                stream.Close();
                            }
                        }
                        else
                            MessageBox.Show("Nie istnieje plik z filtrami");


                    },
                    () => {
                        return File.Exists("filters.bin");
                    }
                    ));
            }
        }

        public FiltersPageViewModel(MainWindowViewModel sharedViewModel)
        {
            mainWindowSharedViewModel = sharedViewModel;
        }

    }

}
