
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using System;

    public sealed class EditAction : RecordingAction
    {
        #region Fields

        private readonly INavigationService _navigationService;

        #endregion

        #region Constructors

        public EditAction(INavigationService navigationService)
        {
            _navigationService = navigationService;
            IconUri = new Uri("/Assets/ActionIcons/edit.png", UriKind.RelativeOrAbsolute);
            Name = "edit";
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            ClosePopup();
            _navigationService.UriFor<EditRecordingViewModel>().WithParam(vm => vm.RecordingId, _recording.Id).Navigate();
        }

        #endregion
    }
}
