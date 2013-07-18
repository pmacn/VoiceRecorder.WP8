
namespace VoiceRecorder.Model
{
    using Microsoft.Phone.Tasks;
    using System;

    public class RecordingPlayer
    {
        public static void PlayAsync(string recordingPath)
        {
            new MediaPlayerLauncher
            {
                Media = new Uri(recordingPath, UriKind.Relative)
            }.Show();
        }
    }
}
