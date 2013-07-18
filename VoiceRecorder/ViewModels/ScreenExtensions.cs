using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using VoiceRecorder.Model;

namespace VoiceRecorder.ViewModels
{
    public static class ScreenExtensions
    {
        public static void ShowRecordingActionsPopup(this Screen @this, Recording recording)
        {
            var settings = new Dictionary<string, object>();
            var view = @this.GetView() as UIElement;
            if (view == null)
                return;

            var point = view.TransformToVisual(null).Transform(new Point());
            settings.Add("VerticalOffset", point.Y);
            var vm = IoC.Get<RecordingActionsViewModel>();
            vm.SetRecording(recording);
            @this.Deactivated += (s, e) => vm.ClosePopup();
            var windowManager = IoC.Get<IWindowManager>();
            windowManager.ShowPopup(vm, settings: settings);
        }
    }
}