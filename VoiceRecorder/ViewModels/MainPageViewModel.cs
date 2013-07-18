
namespace VoiceRecorder.ViewModels
{
    using Caliburn.Micro;
    using Caliburn.Micro.BindableAppBar;
    using System;
    using System.Linq;
    using VoiceRecorder.Model.Commands;

    public sealed class MainPageViewModel : Conductor<IScreen>.Collection.OneActive
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion
        
        #region Constructors

        public MainPageViewModel(
            IEventAggregator eventAggregator,
            RecordingsViewModel recordings,
            TileViewModel pinTile,
            TagsViewModel tags)
        {
            _eventAggregator = eventAggregator;
            DisplayName = "VOICERECORDER";
            Items.Add(recordings);
            Items.Add(pinTile);
            Items.Add(tags);
            if(Items.Any())
                ActivateItem(Items.First());
        }

        #endregion

        #region Properties

        public string StartAction { get; set; }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();
            AppBarConductor.Mixin(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (StartAction == "Record")
                StartRecording();
        }

        public void StartRecording()
        {
            _eventAggregator.Publish(new StartRecording(String.Empty));
        }

        #endregion
    }
}
