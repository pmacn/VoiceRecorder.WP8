
namespace VoiceRecorder.Model.Events
{
    public class RecordingCreated
    {
        public readonly Recording CreatedRecording;

        public RecordingCreated(Recording createdRecording)
        {
            CreatedRecording = createdRecording;
        }
    }
}
