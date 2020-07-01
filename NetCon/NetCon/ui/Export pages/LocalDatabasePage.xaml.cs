using NetCon.viewmodel;
using System;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows;

namespace NetCon.ui.Export_pages
{
    /// <summary>
    /// Interaction logic for LocalDatabasePage.xaml
    /// </summary>
    public partial class LocalDatabasePage : Page, ILabeled
    {
        private ExportPageViewModel ViewModel;

        
        
        public LocalDatabasePage(ExportPageViewModel viewmodel)
        {
            InitializeComponent();
            ViewModel = viewmodel;
            this.DataContext = ViewModel;
        }

        public string Label { get; } = "Lokalna baza danych";

        private void BtnFileSelector_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Plik bazy danych|*.mdf";
            dialog.DefaultExt = "mdf";
            dialog.FileName = "Database.mdf";
            dialog.Title = "Select database file location";
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (dialog.ShowDialog() == true)
            {
                ViewModel.FileURL = dialog.FileName;
            }

        }

        private void BtnConnectionCheck_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement
        }
    }
}
