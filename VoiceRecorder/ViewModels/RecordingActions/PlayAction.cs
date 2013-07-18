
namespace VoiceRecorder.ViewModels
{
    using System;
    using VoiceRecorder.Model;

    public class PlayAction : RecordingAction
    {
        #region Fields

        private readonly IRecordingStreamManager _streamManager;

        #endregion

        public PlayAction(IRecordingStreamManager streamManager)
        {
            _streamManager = streamManager;
            IconUri = new Uri("/Assets/ActionIcons/play.png", UriKind.RelativeOrAbsolute);
            Name = "play";
        }

        public override async void Execute()
        {
            var streamPath = await _streamManager.GetPathAsync(_recording.Id);
            RecordingPlayer.PlayAsync(streamPath);
        }
    }
}