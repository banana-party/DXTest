using System;
using System.IO;
using System.Windows;

namespace DXTest.Services
{
    public class Reader : Stream
    {
        public readonly int TotalPrecentage = 99;
        public int CurrentProgress
        {
            get => _currentProgress;
            set
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

        public override bool CanSeek => throw new System.NotImplementedException();

        public override bool CanWrite => throw new System.NotImplementedException();

        public override long Length => throw new System.NotImplementedException();

        public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = fs.Read(buffer, offset, count);
            _currentLength += read;
            var newProgress = (int)(_currentLength / (double)_totalLength * TotalPrecentage);
            CurrentProgress = newProgress;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}
