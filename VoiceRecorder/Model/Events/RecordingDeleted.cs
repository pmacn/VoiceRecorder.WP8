
namespace VoiceRecorder.Model.Events
{
    using System;

    public class RecordingDeleted
    {
        public readonly Guid RecordingId;

        public RecordingDeleted(Guid recordingId)
        {
            RecordingId = recordingId;            
        }
    }
}
