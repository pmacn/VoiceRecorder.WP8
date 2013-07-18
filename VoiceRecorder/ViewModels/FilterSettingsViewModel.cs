using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Linq;
using VoiceRecorder.Infrastructure;
using VoiceRecorder.Model;
using System.Diagnostics.Contracts;
using VoiceRecorder.Model.Events;

namespace VoiceRecorder.ViewModels
{
    public class FilterSettingsViewModel : Screen, IHandle<TagDeleted>, IHandle<TagCreated>
    {
        private readonly ITagManager _tagManager;

        private TagFilterChanged _changes;
        
        public FilterSettingsViewModel(ITagManager tagManager, IEventAggregator eventAggregator)
        {
            _tagManager = tagManager;
            eventAggregator.Subscribe(this);
            Tags = new BindableCollection<Tag>();
            SelectedTags = new BindableCollection<Tag>();
        }

        public IObservableCollection<Tag> Tags { get; set; }

        public IObservableCollection<Tag> SelectedTags { get; set; }

        protected async override void OnInitialize()
        {
            base.OnInitialize();
            await LoadAvailableTags();
            LoadSelectedTags();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _changes = new TagFilterChanged();
        }

        protected override void OnDeactivate(bool close)
        {
            ApplicationSettings.TagsToFilterBy = SelectedTags.Select(t => t.Id).ToList();
            base.OnDeactivate(close);
        }

        private async Task LoadAvailableTags()
        {
            Tags.AddRange((await _tagManager.GetAll()).ToList());
        }

        private void LoadSelectedTags()
        {
            var tagIdsToFilterBy = ApplicationSettings.TagsToFilterBy.ToList();
            SelectedTags.AddRange(Tags.Where(t => tagIdsToFilterBy.Contains(t.Id)).ToList());
        }

        public void Handle(TagDeleted message)
        {
            var tag = Tags.FirstOrDefault(t => t.Id == message.TagId);
            if(tag == null)
                return;

            Tags.Remove(tag);
        }

        public void Handle(TagCreated message)
        {
            Tags.Add(message.Tag);
        }
    }
}