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

namespace NetCon.ui.Export_pages
{
    /// <summary>
    /// Interaction logic for AzurePage.xaml
    /// </summary>
    public partial class AzurePage : Page, ILabeled
    {

        public AzurePage(ExportPageViewModel viewmodel)
        {
            InitializeComponent();
            this.DataContext = viewmodel;
        }

        public string Label { get; } = "Chmura Azure";
    }
}
