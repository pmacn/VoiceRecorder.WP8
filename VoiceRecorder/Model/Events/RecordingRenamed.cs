
namespace VoiceRecorder.Model.Events
{
    using System;

    public class RecordingRenamed
    {
        public readonly Guid RecordingId;

        public readonly string NewName;

        public RecordingRenamed(Guid recordingId, string newName)
        {
            RecordingId = recordingId;
            NewName = newName;
        }
    }
}
