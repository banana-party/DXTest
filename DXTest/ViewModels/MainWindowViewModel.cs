using DevExpress.Mvvm;
using DXTest.Services;
using DXTest.Views;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DXTest.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DataTable _table;
        public DataTable Table
        {
            get => _table;
            set
            {
                _table = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Table"));
            }
        }
        #region Commands
        private AsyncCommand _openFileDialogCommand;
        public AsyncCommand OpenFileDialogCommand => _openFileDialogCommand ?? (_openFileDialogCommand = new AsyncCommand(ExecuteOpenFileDialogCommand));
        private async Task ExecuteOpenFileDialogCommand()
        {
            var open = new OpenFileDialog();
            CancellationTokenSource cts = new CancellationTokenSource();
            if (open.ShowDialog() == true)
            {
                var pb = new ProgressBarWindow(cts);
                pb.Show();
                try
                {
                    var reader = new CsvReadService(open.FileName, (pb.DataContext as ProgressBarWindowViewModel).OnValueChanged);
                    Table = await reader.ReadCsvAsync(cts.Token);
                }
                catch(OperationCanceledException)
                {
                    MessageBox.Show("Operation was cancelled by user.", "Cancel", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch(ArgumentException e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            
        }
        #endregion
    }
}
