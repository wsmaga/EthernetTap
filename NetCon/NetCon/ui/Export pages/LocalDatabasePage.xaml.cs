using NetCon.ViewModel;
using System;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows;

namespace NetCon.ui.Export_pages
{
    /// <summary>
    /// Interaction logic for LocalDatabasePage.xaml
    /// </summary>
    public partial class LocalDatabasePage : Page
    {
        private ExportPageViewModel ViewModel;

        public LocalDatabasePage(ExportPageViewModel viewmodel)
        {
            InitializeComponent();
            ViewModel = viewmodel;
            this.DataContext = viewmodel;
        }

        private void BtnFileSelector_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Plik bazy danych|*.mdf";
            dialog.DefaultExt = "mdf";
            dialog.Title = "Select database file location";
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.ValidateNames = false;

            if (dialog.ShowDialog() == true)
            {
                ViewModel.SetMdfFilePath(dialog.FileName);
                ViewModel.FileURLLabel = dialog.FileName;
                
            }
        }

        private void BtnConnectionCheck_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement
            byte data = 69;
            ViewModel.DEBUG_SendByte(data);
        }
    }
}
