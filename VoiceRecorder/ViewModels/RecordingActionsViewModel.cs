
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using System.Collections.Generic;
    using System.Windows.Controls.Primitives;
    using Model;
    using Model.Events;

    public class RecordingActionsViewModel : Screen, IHandle<RecordingStarted>
    {
        private IEnumerable<IRecordingAction> _actions;

        private Recording _recording;
        
        public RecordingActionsViewModel(IEnumerable<IRecordingAction> actions, IEventAggregator eventAggregator)
        {
            Actions = actions;
            eventAggregator.Subscribe(this);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            App.Current.RootVisual.MouseLeftButtonDown += ClosePopup;
        }

        private void ClosePopup(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClosePopup();
        }

        public void ClosePopup()
        {
            App.Current.RootVisual.MouseLeftButtonDown -= ClosePopup;
            var popup = GetPopupElement();
            if(popup != null)
                popup.IsOpen = false;
        }

        private Popup GetPopupElement()
        {
            return GetView() as Popup;
        }

        public void SetRecording(Recording recording)
        {
            _recording = recording;
            foreach (var item in Actions)
            {
                item.SetRecording(_recording);
                var recordingAction = item as RecordingAction;
                if (recordingAction != null)
                    recordingAction.ClosePopup = () =>
                    {
                        GetPopupElement().IsOpen = false;
                        App.Current.RootVisual.MouseLeftButtonDown -= ClosePopup;
                    };
            }
        }

        public IEnumerable<IRecordingAction> Actions
        {
            get
            {
                return _actions;
            }
            private set
            {
                if (ReferenceEquals(_actions, value))
                    return;

                _actions = value;
                NotifyOfPropertyChange(() => Actions);
            }
        }

        public void Handle(RecordingStarted message)
        {
            ClosePopup();
        }
    }
}
