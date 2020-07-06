using NetCon.ViewModel;
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
    /// Interaction logic for RemoteDatabasePage.xaml
    /// </summary>
    public partial class RemoteDatabasePage : Page
    {
        public RemoteDatabasePage(ExportPageViewModel viewmodel)
        {
            InitializeComponent();
            this.DataContext = viewmodel;
        }
    }
}
