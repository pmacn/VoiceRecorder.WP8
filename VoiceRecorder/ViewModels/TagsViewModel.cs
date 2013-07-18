
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Model;
    using Model.Events;

    public sealed class TagsViewModel : Screen, IHandle<TagCreated>, IHandle<TagDeleted>
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private readonly ITagManager _tagManager;
        
        private readonly IWindowManager _windowManager;

        private string _newTagName;

        #endregion
        
        #region Constructors

        public TagsViewModel(IEventAggregator eventAggregator, ITagManager tagManager, IWindowManager windowManager)
        {
            _windowManager = windowManager;
            _tagManager = tagManager;
            DisplayName = "tags";
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            Tags = new BindableCollection<Tag>();
            SelectedTags = new BindableCollection<Tag>();
        }

        #endregion

        public IObservableCollection<Tag> Tags { get; set; }

        public IObservableCollection<Tag> SelectedTags { get; set; }

        public string NewTagName
        {
            get
            {
                return _newTagName;
            }
            set
            {
                if (_newTagName == value)
                    return;

                _newTagName = value;
                NotifyOfPropertyChange(() => NewTagName);
            }
        }

        #region Methods

        public async void Add()
        {
            if (String.IsNullOrWhiteSpace(NewTagName))
            {
                MessageBox.Show("You must provide a name for your new tag.", "Need a name", MessageBoxButton.OK);
                return;
            }

            var result = await _tagManager.Create(NewTagName);
            switch (result)
            {
                case CreateTagResult.Success:
                    NewTagName = String.Empty;
                    break;

                case CreateTagResult.TagWithNameAlreadyExists:
                    MessageBox.Show(String.Format("A tag with the name {0} already exists, tag names have to be unique.", NewTagName), "Tag already exists", MessageBoxButton.OK);
                    break;

                case CreateTagResult.UndefinedFailure:
                    MessageBox.Show("Something went wrong when trying to create your tag, you can try again.", "Undefined error:", MessageBoxButton.OK);
                    break;
            }
        }

        public async void DeleteSelected()
        {
            foreach (var id in SelectedTags.Select(t => t.Id).ToList())
            {
                var result = await _tagManager.Delete(id);
                switch (result)
                {
                    case DeleteTagResult.Success:
                        _eventAggregator.Publish(new TagDeleted(id));    
                        break;

                    case DeleteTagResult.UndefinedFailure:
                        MessageBox.Show("Something went wrong when trying to delete your tag, please try again.", "Undefined error", MessageBoxButton.OK);
                        break;
                }
            }
        }

        protected async override void OnInitialize()
        {
            base.OnInitialize();
            await LoadTags();
        }

        private async Task LoadTags()
        {
            Tags.AddRange(await _tagManager.GetAll());
        }

        public void ShowNewTagDialog()
        {
            var dialog = IoC.Get<NewTagViewModel>();
            _windowManager.ShowDialog(dialog);
        }

        public void DeleteSelectedTags()
        {
            var dialog = new DeleteTagsViewModel(_tagManager, SelectedTags.ToList());
            _windowManager.ShowDialog(dialog);
        }

        #region Event handlers

        public void Handle(TagCreated message)
        {
            Tags.Add(message.Tag);
        }

        public void Handle(TagDeleted message)
        {
            var deletedTag = Tags.SingleOrDefault(t => t.Id == message.TagId);
            if (deletedTag != null)
                Tags.Remove(deletedTag);
        }

        #endregion

        #endregion
   }
}
