using System;
using System.IO;
using System.Windows;

namespace DXTest.Services
{
    public class Reader : Stream
    {
        public readonly int TotalPercentage = 100;
        public int CurrentProgress
        {
            get => _currentProgress;
            private set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        OnProgressChanged?.Invoke(CurrentProgress);
                    });
                }
            }
        }

        private int _currentProgress = 0;
        private readonly FileStream fs;
        private long _currentLength;
        private long _totalLength;

        public event Action<int> OnProgressChanged;
        public Reader(FileStream fs, Action<int> onValueChanged)
        {
            this.fs = fs;
            _totalLength = fs.Length;
            OnProgressChanged = onValueChanged;
        }

        public override bool CanRead => fs.CanRead;

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = fs.Read(buffer, offset, count);
            _currentLength += read;
            var newProgress = (int)(_currentLength / (double)_totalLength * TotalPercentage);
            CurrentProgress = newProgress;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
