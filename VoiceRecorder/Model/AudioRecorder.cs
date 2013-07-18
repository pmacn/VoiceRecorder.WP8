

namespace VoiceRecorder.Model
{
    using Caliburn.Micro;
    using System;
    using System.Threading.Tasks;
    using Events;

    public class AudioRecorder : PropertyChangedBase, IAudioRecorder, IDisposable
    {
        #region Fields

        private readonly IRecordingStreamManager _streamManager;

        private readonly IEventAggregator _eventAggregator;

        private AudioCaptureDevice _device;

        private bool _isDisposed;

        #endregion

        #region Constructors

        public AudioRecorder(
            IRecordingStreamManager streamManager,
            IEventAggregator eventAggregator,
            AudioCaptureDevice device)
        {
            _eventAggregator = eventAggregator;
            _streamManager = streamManager;
            _device = device;
        }

        #endregion

        #region Methods

        public async Task StartRecording(Recording recording)
        {
            GuardFromCallingWhenDisposed();
            if (_device.IsRecording)
                throw new InvalidOperationException("Cannot start a recording when already recording.");

            var stream = await _streamManager.GetOrCreateStreamAsync(recording.Id);
            await _device.StartRecordingAsync(stream);
            _eventAggregator.Publish(new RecordingStarted(recording.Name));
        }

        public async Task StopRecording()
        {
            GuardFromCallingWhenDisposed();
            if (!_device.IsRecording)
                return;
            
            await _device.StopRecordingAsync();
            _eventAggregator.Publish(new RecordingStopped());
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isDisposed = true;
                if (_device != null)
                {
                    _device.Dispose();
                    _device = null;
                }
            }
        }

        private void GuardFromCallingWhenDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("AudioRecorder");
        }

        #endregion

        #endregion
    }
}