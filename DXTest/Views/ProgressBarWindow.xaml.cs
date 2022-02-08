using DXTest.ViewModels;
using System.Threading;
using System.Windows;

namespace DXTest.Views
{
    /// <summary>
    /// Interaction logic for ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        public ProgressBarWindow(CancellationTokenSource cts)
        {
            InitializeComponent();
            DataContext = new ProgressBarWindowViewModel(cts, this);
            Closed += (DataContext as ProgressBarWindowViewModel).OnWindowClosed;
        }
    }
}