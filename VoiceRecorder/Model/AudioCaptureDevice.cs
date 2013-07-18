
namespace VoiceRecorder.Model
{
    using Caliburn.Micro;
    using System;
    using System.Threading.Tasks;
    using Windows.Phone.Media.Capture;
    using Windows.Storage.Streams;

    public sealed class AudioCaptureDevice : PropertyChangedBase, IDisposable
    {
        #region Fields

        private IRandomAccessStream _recordingStream;

        private AudioVideoCaptureDevice _device;

        private bool _isRecording;

        private bool _isStarting;

        private bool _isStopping;

        private bool _isDisposed;

        #endregion

        #region Properties

        public bool IsRecording
        {
            get
            {
                ThrowIfDisposed();
                return _isRecording;
            }
            set
            {
                ThrowIfDisposed();
                if (_isRecording == value)
                    return;

                _isRecording = value;
                NotifyOfPropertyChange(() => IsRecording);
            }
        }

        #endregion

        #region Methods

        public async Task StartRecordingAsync(IRandomAccessStream stream)
        {
            ThrowIfDisposed();
            if (IsRecording || _isStarting)
                return;

            _isStarting = true;
            _device = await AudioVideoCaptureDevice.OpenForAudioOnlyAsync();
            //_device.AudioEncodingFormat = CameraCaptureAudioFormat.Aac; // AAC is default
            _recordingStream = stream;
            await _device.StartRecordingToStreamAsync(_recordingStream);
            IsRecording = true;
            _isStarting = false;
        }

        public async Task StopRecordingAsync()
        {
            ThrowIfDisposed();
            if (!IsRecording || _isStopping)
                return;

            _isStopping = true;
            await _device.StopRecordingAsync();
            await DisposeMembers();
            IsRecording = false;
            _isStopping = false;
        }

        #region IDisposable

        public async void Dispose()
        {
            _isDisposed = true;
            await DisposeMembers();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("AudioRecorder");
        }

        private async Task DisposeMembers()
        {
            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }
            if (_recordingStream != null)
            {
                await _recordingStream.FlushAsync();
                _recordingStream.Dispose();
                _recordingStream = null;
            }
        }

        #endregion

        #endregion
    }
}
