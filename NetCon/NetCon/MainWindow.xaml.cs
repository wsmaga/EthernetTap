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

        public const string INFO_COLOR = "#0779e4";
        public const string ACTION_COLOR = "#ff5200";

        public MainWindowViewModel mMainWindowViewModel;

        private Page[] mPages;

        public MainWindow()
        {
            InitializeComponent();
            mMainWindowViewModel = new MainWindowViewModel();
            this.DataContext = mMainWindowViewModel;
            
            mPages = new Page[]{
                new CapturePage(mMainWindowViewModel),
                new FiltersPage(),
                new ExportPage()
            };


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
