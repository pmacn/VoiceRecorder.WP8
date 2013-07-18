using System;

namespace VoiceRecorder.Model.Events
{
    public class TagRemovedFromRecording
    {
        public readonly Guid TagId;

        public readonly Guid RecordingId;

        public TagRemovedFromRecording(Guid tagId, Guid recordingId)
        {
            TagId = tagId;
            RecordingId = recordingId;
        }
    }
}
