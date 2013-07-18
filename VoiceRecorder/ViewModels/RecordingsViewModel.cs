using System.Collections.Generic;
using Caliburn.Micro;
using VoiceRecorder.Infrastructure;
using VoiceRecorder.Model;
using VoiceRecorder.Model.Commands;
using VoiceRecorder.Model.Events;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;


namespace VoiceRecorder.ViewModels
{
    public class FilteredRecordings : INotifyCollectionChanged, IEnumerable<RecordingViewModel>
    {
        private readonly RecordingsCollection _recordings;

        private readonly IObservableCollection<Guid> _tagIds;

        private readonly RecordingViewModelFactory _factory;

        public FilteredRecordings(RecordingsCollection recordings, IObservableCollection<Guid> tagIds, RecordingViewModelFactory factory)
        {
            _recordings = recordings;
            _tagIds = tagIds;
            _factory = factory;
            _recordings.CollectionChanged += RaiseCollectionChanged;
            _tagIds.CollectionChanged += RaiseCollectionChanged;
        }

        private void RaiseCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator<RecordingViewModel> GetEnumerator()
        {
            var filteredRecordings = (_tagIds.Any()
                                         ? _recordings.Where(r => _tagIds.Intersect(r.TagIds).Any())
                                         : _recordings).ToList();

            return filteredRecordings.Select(_factory.Create).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public class RecordingsCollection : INotifyCollectionChanged, IEnumerable<Recording>, IHandle<RecordingCreated>, IHandle<RecordingDeleted>
    {
        private readonly ObservableCollection<Recording> _collection;

        public RecordingsCollection(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");

            _collection = new ObservableCollection<Recording>();
            eventAggregator.Subscribe(this);
        }

        private void RaiseCollectionChanged()
        {
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator<Recording> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public void Load(IEnumerable<Recording> recordings)
        {
            foreach (var recording in recordings)
            {
                _collection.Add(recording);
            }

            RaiseCollectionChanged();
        }

        public void Handle(RecordingCreated message)
        {
            _collection.Add(message.CreatedRecording);
            RaiseCollectionChanged();
        }

        public void Handle(RecordingDeleted message)
        {
            var recording = _collection.FirstOrDefault(r => r.Id == message.RecordingId);
            if(recording == null)
                return;
            
            _collection.Remove(recording);
            RaiseCollectionChanged();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    // TODO: This viewmodel needs some serious cleanup, It also has too many responsibilities and dependencies
    public sealed class RecordingsViewModel :
        Conductor<Screen>.Collection.AllActive,
        IHandle<RecordingStarted>,
        IHandle<RecordingStopped>,
        IHandle<StartRecording>,
        IHandle<TagDeleted>
    {
        #region Fields

        private bool _isListFiltered;

        private readonly IEventAggregator _eventAggregator;

        private readonly IRecordingManager _recordingManager;

        private readonly IAudioRecorder _audioRecorder;

        private readonly INavigationService _navigationService;

        private readonly IWindowManager _windowManager;

        private readonly FilteredRecordings _recordings;

        private readonly RecordingsCollection _recordingsCollection;

        private readonly IObservableCollection<Guid> _tagIdsToFilterBy;

        private string _recordingMessage;
        
        private bool _isRecordingInProgress;

        #endregion

        #region Constructors

        public RecordingsViewModel(
            IAudioRecorder audioRecorder,
            IRecordingManager manager,
            IEventAggregator eventAggregator,
            INavigationService navigationService,
            IWindowManager windowManager,
            RecordingViewModelFactory factory)
        {
            _windowManager = windowManager;
            DisplayName = "recordings";
            _navigationService = navigationService;
            _audioRecorder = audioRecorder;
            _recordingManager = manager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _recordingsCollection = new RecordingsCollection(_eventAggregator);
            _tagIdsToFilterBy = new BindableCollection<Guid>();
            _recordings = new FilteredRecordings(_recordingsCollection, _tagIdsToFilterBy, factory);
            _recordings.CollectionChanged += (sender, args) => UpdateItems();
        }

        #endregion

        #region Properties

        public bool IsRecording
        {
            get
            {
                return _isRecordingInProgress;
            }
            set
            {
                if (_isRecordingInProgress == value)
                    return;

                _isRecordingInProgress = value;
                NotifyOfPropertyChange(() => IsRecording);
            }
        }

        public string RecordingMessage
        {
            get { return _recordingMessage; }
            set
            {
                if (_recordingMessage == value)
                    return;

                _recordingMessage = value;
                NotifyOfPropertyChange(() => RecordingMessage);
            }
        }

        public bool IsListFiltered
        {
            get
            {
                return _isListFiltered;
            }
            set
            {
                if (_isListFiltered == value)
                    return;

                _isListFiltered = value;
                NotifyOfPropertyChange(() => IsListFiltered);
            }
        }

        #endregion

        #region Methods

        protected override void OnInitialize()
        {
            base.OnInitialize();
            LoadRecordings();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            UpdateFilters();
        }

        public async Task StartRecording(string desiredName)
        {
            RecordingMessage = "Starting recording...";
            IsRecording = true;
            var name = String.IsNullOrWhiteSpace(desiredName) ? "Untitled" : desiredName;
            var recordingFailedToStart = false;
            var recording = new Recording(Guid.NewGuid(), name);
            
            try
            {
                await _audioRecorder.StartRecording(recording);
                await _recordingManager.SaveAsync(recording);
            }
            catch
            {
                recordingFailedToStart = true;
            }

            if (recordingFailedToStart)
            {
                await _audioRecorder.StopRecording();
                IsRecording = false;
                MessageBox.Show("Unable to start recording.");
            }
        }

        public async Task StopRecording()
        {
            try
            {
                await _audioRecorder.StopRecording();
            }
            catch
            {
                MessageBox.Show("Unable to stop recording. If you tried to stop it too soon after starting, try again.");
            }
        }

        public void ShowFilterSettingsDialog()
        {
            var viewModel = IoC.Get<FilterSettingsViewModel>(); // HACK: ServiceLocator!
            viewModel.Deactivated += FilterDialogDeactivated;
            _windowManager.ShowDialog(viewModel);
        }

        private void FilterDialogDeactivated(object sender, DeactivationEventArgs deactivationEventArgs)
        {
            var dialog = sender as FilterSettingsViewModel;
            if (dialog != null)
                dialog.Deactivated -= FilterDialogDeactivated;

            UpdateFilters();
        }

        public void ShowNewRecordingDialog()
        {
            var dialog = IoC.Get<NewRecordingViewModel>(); // HACK: Service locator
            _windowManager.ShowDialog(dialog);
        }

        private void UpdateItems()
        {
            var recordings = _recordings.ToList();
            var recordingsToAdd = recordings.Except(Items).ToList();
            var recordingsToRemove = Items.Except(recordings).ToList();
            Items.AddRange(recordingsToAdd);
            Items.RemoveRange(recordingsToRemove);
        }

        public void NavigateToAbout()
        {
            _navigationService.UriFor<AboutViewModel>().Navigate();
        }

        public void NavigateToSettings()
        {
            _navigationService.UriFor<SettingsViewModel>()
                              .Navigate();
        }

        private void UpdateFilters()
        {
            var newFilters = ApplicationSettings.TagsToFilterBy.ToList();
            var filtersToRemove = _tagIdsToFilterBy.Except(newFilters).ToList();
            if (filtersToRemove.Any())
                _tagIdsToFilterBy.RemoveRange(filtersToRemove);

            var filtersToAdd = newFilters.Except(_tagIdsToFilterBy).ToList();
            if (filtersToAdd.Any())
                _tagIdsToFilterBy.AddRange(filtersToAdd);
        }

        private async void LoadRecordings()
        {
            _recordingsCollection.Load(await _recordingManager.GetRecordingsAsync());
        }

        #endregion

        #region EventHandlers

        public void Handle(RecordingStarted @event)
        {
            IsRecording = true;
            RecordingMessage = String.Format("Recording {0}", @event.Name);
        }

        public void Handle(RecordingStopped @event)
        {
            IsRecording = false;
        }

        public async void Handle(StartRecording @event)
        {
            await StartRecording(@event.DesiredName);
        }

        public void Handle(TagDeleted message)
        {
            _tagIdsToFilterBy.Remove(message.TagId);
            ApplicationSettings.TagsToFilterBy = _tagIdsToFilterBy;
        }

        #endregion
    }   
}