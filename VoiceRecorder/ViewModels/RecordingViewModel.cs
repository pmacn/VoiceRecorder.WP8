
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using System;
    using Model;
    using Model.Events;

    public sealed class RecordingViewModel : Screen, IHandle<RecordingRenamed>
    {
        #region Fields

        private readonly IRecordingManager _recordingManager;

        private readonly IEventAggregator _eventAggregator;

        private readonly IRecordingStreamManager _streamManager;

        private Guid _recordingId;

        private string _name;

        private DateTimeOffset _dateCreated;

        #endregion

        #region Constructors

        public RecordingViewModel(
            IRecordingManager recordingManager,
            IEventAggregator eventAggregator,
            IRecordingStreamManager streamManager)
        {
            _recordingManager = recordingManager;
            _eventAggregator = eventAggregator;
            _streamManager = streamManager;
            _eventAggregator.Subscribe(this);
        }

        #endregion
        
        #region Properties

        public void SetRecording(Recording recording)
        {
            Recording = recording;
            _recordingId = Recording.Id;
            Name = Recording.Name;
            DateCreated = Recording.DateCreated;
        }

        public Recording Recording { get; private set; }

        public Guid RecordingId
        {
            get { return _recordingId; }
            set
            {
                if (_recordingId == value)
                    return;

                _recordingId = value;
                NotifyOfPropertyChange(() => RecordingId);
            }
        }

        public string Name
        {
            get { return _name; }
            private set
            {
                if (_name == value)
                    return;

                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public DateTimeOffset DateCreated
        {
            get { return _dateCreated; }
            set
            {
                if (_dateCreated == value)
                    return;

                _dateCreated = value;
                NotifyOfPropertyChange(() => DateCreated);
            }
        }

        #endregion

        #region Methods
        
        #region Commands

        public void NavigateToDetails()
        {
            this.ShowRecordingActionsPopup(Recording);
        }

        #endregion

        #region EventHandlers

        public void Handle(RecordingRenamed @event)
        {
            if (_recordingId.Equals(@event.RecordingId))
                Name = @event.NewName;
        }

        #endregion

        #endregion
    }
}
