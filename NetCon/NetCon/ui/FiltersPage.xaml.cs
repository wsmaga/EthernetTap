using NetCon.inter;
using NetCon.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Logika interakcji dla klasy Page1.xaml
    /// </summary>
    public partial class FiltersPage : Page
    {
        private FiltersPageViewModel viewModel;
        public FiltersPage(MainWindowViewModel sharedViewModel)
        {
            InitializeComponent();
            viewModel = new FiltersPageViewModel(sharedViewModel);
            this.DataContext = viewModel;
        }
    }
}
