using NetCon.inter;
using NetCon.ui;
using NetCon.viewmodel;
using System.Windows;
using System.Windows.Controls;

namespace NetCon
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private MainWindowViewModel mMainWindowViewModel = new MainWindowViewModel();

        private Page[] mPages ={
            new FiltersPage(),
            new TunelsPage(),
            new ExportPage()
        };


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mMainWindowViewModel;
            contentFrame.Navigate(mPages[0]);


            // test code

            var impl = new NetConImpl();

            impl.startCapture();
            impl.stopCapture();

        }

        private void navigateToFiltersPage(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(mPages[0]);
        }

        private void navigateToTunelsPage(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(mPages[1]);
        }

        private void navigateToExportPage(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(mPages[2]);
        }
    }
}
