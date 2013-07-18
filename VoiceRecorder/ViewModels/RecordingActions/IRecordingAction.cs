namespace VoiceRecorder.ViewModels
{
    using System;
    using VoiceRecorder.Model;

    public interface IRecordingAction
    {
        Uri IconUri { get; }
        string Name { get; }
        void Execute();
        void SetRecording(Recording recording);
    }
}