using NetCon.inter;
using NetCon.ui;
using NetCon.viewmodel;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace NetCon
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 

   

    public partial class MainWindow : Window
    {
        public MainWindowViewModel mMainWindowViewModel;

        private Page[] mPages ={
            new FiltersPage(),
            new TunelsPage(),
            new ExportPage()
        };
        string retText = "";
        public MainWindow()
        {
            InitializeComponent();
            mMainWindowViewModel = new MainWindowViewModel(this);
            this.DataContext = mMainWindowViewModel;
            
            //contentFrame.Navigate(mPages[0]);
        }

        private void navigateToFiltersPage(object sender, RoutedEventArgs e)
        {
            //contentFrame.Navigate(mPages[0]);
        }

        private void navigateToTunelsPage(object sender, RoutedEventArgs e)
        {
            //contentFrame.Navigate(mPages[1]);
        }

        private void navigateToExportPage(object sender, RoutedEventArgs e)
        {
           // contentFrame.Navigate(mPages[2]);
        }

        public void SetIksde(String str)
        {
            //this.iksde_label.Content = str;
        }
    }
}
