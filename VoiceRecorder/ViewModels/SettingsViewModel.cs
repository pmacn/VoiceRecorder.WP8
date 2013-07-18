using Caliburn.Micro;
using VoiceRecorder.Infrastructure;

namespace VoiceRecorder.ViewModels
{
    public class SettingsViewModel : Screen
    {
        #region Fields

        private string _artistName;

        private string _albumName;

        private bool _autoGenerateUniqueTrackNames;
        
        #endregion

        #region Constructors

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            LoadSettings();
        }

        protected override void OnDeactivate(bool close)
        {
            SaveSettings();
            base.OnDeactivate(close);
        }

        private void LoadSettings()
        {
            ArtistName = ApplicationSettings.ArtistName;
            AlbumName = ApplicationSettings.AlbumName;
            AutoGenerateUniqueTrackNames = ApplicationSettings.AutoGenerateUniqueTrackNames;
        }

        private void SaveSettings()
        {
            ApplicationSettings.ArtistName = ArtistName;
            ApplicationSettings.AlbumName = AlbumName;
            ApplicationSettings.AutoGenerateUniqueTrackNames = AutoGenerateUniqueTrackNames;
        }

        public string ArtistName
        {
            get
            {
                return _artistName;
            }
            set
            {
                if (_artistName == value)
                    return;

                _artistName = value;
                NotifyOfPropertyChange(() => ArtistName);
            }
        }

        public string AlbumName
        {
            get
            {
                return _albumName;
            }
            set
            {
                if (_albumName == value)
                    return;

                _albumName = value;
                NotifyOfPropertyChange(() => AlbumName);
            }
        }

        public bool AutoGenerateUniqueTrackNames
        {
            get
            {
                return _autoGenerateUniqueTrackNames;
            }
            set
            {
                if (_autoGenerateUniqueTrackNames == value)
                    return;

                _autoGenerateUniqueTrackNames = value;
                NotifyOfPropertyChange(() => AutoGenerateUniqueTrackNames);
            }
        }
    }
}
