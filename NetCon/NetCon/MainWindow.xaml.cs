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
            new CapturePage(),
            new FiltersPage(),
            new ExportPage()
        };

        public MainWindow()
        {
            InitializeComponent();
            mMainWindowViewModel = new MainWindowViewModel();
            this.DataContext = mMainWindowViewModel;
            
            contentFrame.Navigate(mPages[0]);
        }

        private void navigateToCapturePage(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(mPages[0]);
        }

        private void navigateToFiltersPage(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(mPages[1]);
        }

        private void navigateToExportPage(object sender, RoutedEventArgs e)
        {
           contentFrame.Navigate(mPages[2]);
        }



    }
}
