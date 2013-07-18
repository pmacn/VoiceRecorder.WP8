
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using System;

    public sealed class CopyToMediaLibraryAction : RecordingAction
    {
        private readonly IWindowManager _windowManager;

        public CopyToMediaLibraryAction(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            IconUri = new Uri("/Assets/ActionIcons/save.png", UriKind.RelativeOrAbsolute);
            Name = "export";
        }

        public override void Execute()
        {
            var dialog = IoC.Get<ExportRecordingViewModel>();
            dialog.RecordingToExport = _recording;
            _windowManager.ShowDialog(dialog);
        }
    }
}
