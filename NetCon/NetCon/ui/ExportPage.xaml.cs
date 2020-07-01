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
        private ExportPageViewModel viewModel;
        public ExportPage(MainWindowViewModel sharedViewModel)
        {
            InitializeComponent();
            viewModel = new ExportPageViewModel(sharedViewModel);
            
            this.DataContext = viewModel;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExportTargetPage.Navigate(ExportTargetSelector.SelectedItem);
        }
    }
}
