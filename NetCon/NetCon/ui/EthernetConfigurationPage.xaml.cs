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
    /// Interaction logic for EthernetConfigurationPage.xaml
    /// </summary>
    public partial class EthernetConfigurationPage : Page
    {
        private EthernetConfigurationViewModel viewModel;

        public EthernetConfigurationPage()
        {
        }

        public EthernetConfigurationPage(MainWindowViewModel sharedViewModel)
        {
            viewModel = new EthernetConfigurationViewModel(sharedViewModel);
            this.DataContext = viewModel;
            InitializeComponent();
        }
    }
}
