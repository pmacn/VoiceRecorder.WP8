using System;
using VoiceRecorder.Model;

namespace VoiceRecorder.ViewModels
{
    public sealed class DeleteAction : RecordingAction
    {
        private readonly IRecordingManager _recordingManager;
        public DeleteAction(IRecordingManager recordingManager)
        {
            _recordingManager = recordingManager;
            IconUri = new Uri("/Assets/ActionIcons/delete.png", UriKind.Relative);
        }

        public override async void Execute()
        {
            ClosePopup();
            await _recordingManager.DeleteAsync(_recording.Id);
        }
    }
}
