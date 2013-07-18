
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using Model.Commands;

    public class NewRecordingViewModel : Screen
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        public NewRecordingViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion

        #region Properties

        public string DesiredRecordingName { get; set; }

        #endregion

        #region Methods

        public void Create()
        {
            _eventAggregator.Publish(new StartRecording(DesiredRecordingName));
            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }

        #endregion
    }
}
