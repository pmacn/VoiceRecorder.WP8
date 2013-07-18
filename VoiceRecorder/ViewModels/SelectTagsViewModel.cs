using System.Linq;
using Caliburn.Micro;
using System;
using VoiceRecorder.Model;

namespace VoiceRecorder.ViewModels
{
    public class SelectTagsViewModel : Screen
    {
        private readonly ITagManager _tagManager;

        private readonly IRecordingManager _recordingManager;

        private Guid _recordingId;

        private bool _hasTags;

        public SelectTagsViewModel(ITagManager tagManager, IRecordingManager recordingManager)
        {
            _recordingManager = recordingManager;
            _tagManager = tagManager;
            Tags = new BindableCollection<Tag>();
            SelectedTags = new BindableCollection<Tag>();
            Tags.CollectionChanged += (sender, args) => HasTags = Tags.Any();
        }

        public bool HasTags
        {
            get { return _hasTags; }
            set
            {
                if (value.Equals(_hasTags)) return;
                _hasTags = value;
                NotifyOfPropertyChange(() => HasTags);
            }
        }

        public IObservableCollection<Tag> Tags { get; set; }

        public IObservableCollection<Tag> SelectedTags { get; set; }

        public Guid RecordingId
        {
            get { return _recordingId; }
            set
            {
                if (value.Equals(_recordingId))
                    return;

                _recordingId = value;
                NotifyOfPropertyChange(() => RecordingId);
                LoadTags();
            }
        }

        private async void LoadTags()
        {
            Tags.Clear();
            var tags = await _tagManager.GetAll();
            var usedTags = await _recordingManager.GetTagsFor(RecordingId);
            if(tags != null)
                Tags.AddRange(tags.Except(usedTags));
        }

        protected async override void OnDeactivate(bool close)
        {
            foreach (var tag in SelectedTags.ToList())
            {
                await _recordingManager.AddTag(RecordingId, tag.Id);
            }

            base.OnDeactivate(close);
        }
    }
}
