
namespace VoiceRecorder.Model.Events
{
    public class RecordingStarted
    {
        public readonly string Name;

        public RecordingStarted(string name)
        {
            Name = name;
        }
    }
}
