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
    /// Logika interakcji dla klasy HardwareFilter.xaml
    /// </summary>
    public partial class HardwareFilterPage : Page
    {
        private HardwareFilterViewModel hardwareFilterViewModel;
        public HardwareFilterPage(MainWindowViewModel mainWindowViewModel)
        {
            hardwareFilterViewModel = new HardwareFilterViewModel(mainWindowViewModel);
            InitializeComponent();
            this.DataContext = hardwareFilterViewModel;
        }
    }
}
