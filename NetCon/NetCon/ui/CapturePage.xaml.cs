using NetCon.viewmodel;
using System.Windows.Controls;

namespace NetCon.ui
{
    /// <summary>
    /// Logika interakcji dla klasy CapturePage.xaml
    /// </summary>
    public partial class CapturePage : Page
    {
        private CapturePageViewModel viewModel;
        public CapturePage(MainWindowViewModel sharedViewModel)
        {
            InitializeComponent();
            viewModel = new CapturePageViewModel(sharedViewModel);
            this.DataContext = viewModel;
        }
    }
}
