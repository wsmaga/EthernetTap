using NetCon.ui.Export_pages;
using NetCon.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetCon.ui
{
    /// <summary>
    /// Logika interakcji dla klasy ExportPage.xaml
    /// </summary>
    public partial class ExportPage : Page
    {
        private Page[] Subpages { get; }
        
        private ExportPageViewModel viewModel;
        public ExportPage(MainWindowViewModel sharedViewModel)
        {
            InitializeComponent();
            viewModel = new ExportPageViewModel(sharedViewModel);
            
            this.DataContext = viewModel;

            // Initialize all subpages:
            Subpages = new Page[]
            {
                new LocalDatabasePage(viewModel),
                new RemoteDatabasePage(viewModel),
                new AzurePage(viewModel)
            };
        }

        /// <summary>
        /// Invoked on changing export target combobox selection. Navigates to proper subpage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NetCon.viewmodel.ComboBoxItem selected = (NetCon.viewmodel.ComboBoxItem)ExportTargetSelector.SelectedItem;
            switch (selected.Value)
            {
                case ExportTargetDesc.LocalDB:
                    ExportTargetPage.Navigate(Subpages[0]);
                    break;

                case ExportTargetDesc.ExternalDB:
                    ExportTargetPage.Navigate(Subpages[1]);
                    break;

                case ExportTargetDesc.Azure:
                    ExportTargetPage.Navigate(Subpages[2]);
                    break;
            }
        }
    }
}
