
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using Model;

    public class RecordingViewModelFactory
    {
        #region Methods

        public RecordingViewModel Create(Recording recording)
        {
            var vm = IoC.Get<RecordingViewModel>();
            vm.SetRecording(recording);
            return vm;
        }

        #endregion
    }
}
