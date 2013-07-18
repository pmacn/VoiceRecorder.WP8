
namespace VoiceRecorder.ViewModels
{
    using System;
    using VoiceRecorder.Model;

    public abstract class RecordingAction : IRecordingAction
    {
        protected Recording _recording;

        public virtual Uri IconUri { get; protected set; }

        public virtual string Name { get; protected set; }

        public abstract void Execute();

        public Action ClosePopup { get; set; }

        public void SetRecording(Recording recording)
        {
            _recording = recording;
        }
    }
}
