using System;

namespace VoiceRecorder.Model.Events
{
    public class TagAddedToRecording
    {
        public readonly Guid TagId;

        public readonly Guid RecordingId;

        public TagAddedToRecording(Guid tagId, Guid recordingId)
        {
            TagId = tagId;
            RecordingId = recordingId;
        }
    }
}
