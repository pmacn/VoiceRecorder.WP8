
namespace VoiceRecorder.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Linq;

    public class ApplicationSettings
    {
        #region Fields

        private static readonly IsolatedStorageSettings _settings;
        // Keys
        private const string TagsToFilterByKey = "TagsToFilterBy";
        private const string ArtistNameKey = "ArtistName";
        private const string AlbumNameKey = "AlbumName";
        private const string AutoGenerateUniqueTrackNamesKey = "AutoGenerateUniqueTrackNames";
        // Defaults
        private static readonly IEnumerable<Guid> TagsToFilterByDefault = new List<Guid>().AsEnumerable();
        private const string ArtistNameDefault = "Voice Recorder";
        private const string AlbumNameDefault = "Voice recordings";
        private const bool AutoGenerateUniqueTrackNamesDefault = true;

        #endregion

        #region Constructors

        static ApplicationSettings()
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;
        }

        #endregion

        private static bool AddOrUpdateValue(string key, Object value)
        {
            var valueChanged = false;

            if (_settings.Contains(key))
            {
                if (_settings[key] != value)
                {
                    _settings[key] = value;
                    valueChanged = true;
                }
            }
            else
            {
                _settings.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        private static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            if (_settings.Contains(key))
            {
                value = (T)_settings[key];
            }
            else
            {
                value = defaultValue;
            }

            return value;
        }

        private static void Save()
        {
            _settings.Save();
        }

        public static IEnumerable<Guid> TagsToFilterBy
        {
            get
            {
                return GetValueOrDefault(TagsToFilterByKey, TagsToFilterByDefault);
            }
            set
            {
                if (AddOrUpdateValue(TagsToFilterByKey, value))
                {
                    Save();
                }
            }
        }

        public static string ArtistName
        {
            get
            {
                return GetValueOrDefault(ArtistNameKey, ArtistNameDefault);
            }
            set
            {
                if (AddOrUpdateValue(ArtistNameKey, value))
                    Save();
            }
        }

        public static string AlbumName
        {
            get
            {
                return GetValueOrDefault(AlbumNameKey, AlbumNameDefault);
            }
            set
            {
                if (AddOrUpdateValue(AlbumNameKey, value))
                    Save();
            }
        }

        public static bool AutoGenerateUniqueTrackNames
        {
            get
            {
                return GetValueOrDefault(AutoGenerateUniqueTrackNamesKey, AutoGenerateUniqueTrackNamesDefault);
            }
            set
            {
                if (AddOrUpdateValue(AutoGenerateUniqueTrackNamesKey, value))
                    Save();
            }
        }
    }
}
