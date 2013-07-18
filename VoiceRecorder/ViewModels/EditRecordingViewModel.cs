
using System.Linq;

namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using System;
    using Model;
    using Model.Events;

    public sealed class EditRecordingViewModel : Screen, IHandle<TagAddedToRecording>, IHandle<TagRemovedFromRecording>
    {
        #region Fields

        private readonly IRecordingManager _recordingManager;

        private readonly ITagManager _tagManager;

        private readonly IWindowManager _windowManager;

        private string _name;
        
        private Recording _recording;

        private Guid _recordingId;

        #endregion

        public EditRecordingViewModel(IRecordingManager recordingManager, ITagManager tagManager, IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            _tagManager = tagManager;
            _windowManager = windowManager;
            _recordingManager = recordingManager;
            Tags = new BindableCollection<Tag>();
            Tags.CollectionChanged += (s, e) => NotifyOfPropertyChange(() => HasTags);
            SelectedTags = new BindableCollection<Tag>();
        }

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public Guid RecordingId
        {
            get { return _recordingId; }
            set
            {
                if (_recordingId == value)
                    return;

                _recordingId = value;
                LoadRecording();
            }
        }

        public IObservableCollection<Tag> Tags { get; set; }

        public IObservableCollection<Tag> SelectedTags { get; set; }

        public bool HasTags
        {
            get { return Tags.Any(); }
        }

        #endregion

        #region Methods

        protected async override void OnDeactivate(bool close)
        {
            if (Name != _recording.Name)
                await _recordingManager.RenameAsync(_recordingId, Name);

            base.OnDeactivate(close);
        }

        public void ShowTagSelector()
        {
            var viewModel = IoC.Get<SelectTagsViewModel>();
            viewModel.RecordingId = RecordingId;
            _windowManager.ShowDialog(viewModel);
        }

        public async void RemoveSelectedTags()
        {
            var tagsToRemove = SelectedTags.ToList();
            foreach (var tag in tagsToRemove)
            {
                await _recordingManager.RemoveTag(_recordingId, tag.Id);
            }
        }

        private async void LoadRecording()
        {
            _recording = await _recordingManager.GetRecordingAsync(_recordingId);
            if (_recording == null)
                return;

            DisplayName = _recording.Name;
            Name = _recording.Name;
            var tags = await _recordingManager.GetTagsFor(_recordingId);
            Tags.Clear();
            if(tags != null)
                Tags.AddRange(tags);
        }

        #endregion

        public void Handle(TagRemovedFromRecording message)
        {
            if (message.RecordingId != RecordingId)
                return;

            var tag = Tags.FirstOrDefault(t => t.Id == message.TagId);
            if(tag != null)
                Tags.Remove(tag);
        }

        public async void Handle(TagAddedToRecording message)
        {
            if (message.RecordingId != RecordingId)
                return;

            var tag = await _tagManager.GetById(message.TagId);
            if(tag != null)
                Tags.Add(tag);
        }
    }
}
