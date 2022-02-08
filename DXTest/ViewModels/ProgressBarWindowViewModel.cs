using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using DevExpress.Mvvm;

namespace DXTest.ViewModels
{
    public class ProgressBarWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Window _window;
        private readonly CancellationTokenSource _cts;
        private int _progressBarValue;

        public ProgressBarWindowViewModel(CancellationTokenSource cts, Window window)
        {
            _cts = cts;
            _window = window;
        }

        #region Properties
        public int ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressBarValue"));
            }
        }
        #endregion
        #region Methods
        public void OnValueChanged(int value)
        {
            ProgressBarValue = value;
            if (value == 100)
            {
                _window.Close();
            }
        }

        public void OnWindowClosed(object sender, EventArgs e)
        {
            if (ProgressBarValue != 100)
                _cts.Cancel();           
            _window.Close();
        }
        #endregion
        #region Commands
        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        private void ExecuteCancelCommand()
        {
            OnWindowClosed(this, null);
        }
        #endregion
    }
}
