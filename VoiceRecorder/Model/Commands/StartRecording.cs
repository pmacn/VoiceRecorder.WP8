
namespace VoiceRecorder.Model.Commands
{
    public class StartRecording
    {
        public readonly string DesiredName;

        public StartRecording(string desiredName)
        {
            DesiredName = desiredName;
        }
    }
}
