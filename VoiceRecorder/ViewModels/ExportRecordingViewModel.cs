using Caliburn.Micro;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VoiceRecorder.Infrastructure;
using VoiceRecorder.Model;
using Windows.Storage;
using Windows.Storage.Streams;

namespace VoiceRecorder.ViewModels
{
    public class ExportRecordingViewModel : Screen
    {
        private readonly IRecordingStreamManager _streamManager;

        public ExportRecordingViewModel(IRecordingStreamManager streamManager)
        {
            _streamManager = streamManager;
        }

        public Recording RecordingToExport { get; set; }

        public async void Export()
        {
            TryClose();
            using (var library = new MediaLibrary())
            {
                var existingTrackNames =
                    library.Songs.Where(
                        s => s.Album.Name == ApplicationSettings.AlbumName && s.Artist.Name == ApplicationSettings.ArtistName)
                           .Select(s => s.Name)
                           .ToList();
                var trackName = ApplicationSettings.AutoGenerateUniqueTrackNames
                                    ? GetUniqueTrackName(RecordingToExport.Name, existingTrackNames)
                                    : RecordingToExport.Name;

                var metaData = new SongMetadata
                                   {
                                       ArtistName = ApplicationSettings.ArtistName,
                                       AlbumName = ApplicationSettings.AlbumName,
                                       Name = trackName
                                   };

                await CopyFileIntoIsoStore(await _streamManager.GetStorageFileAsync(RecordingToExport));
                var recordingUri = new Uri(RecordingToExport.Id.ToString(), UriKind.RelativeOrAbsolute);
                try
                {
                    library.SaveSong(recordingUri, metaData, SaveSongOperation.CopyToLibrary);
                }
                catch (InvalidOperationException e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }

                MessageBox.Show(
                    String.Format("Copied this recording to the media library{3}Track: {0}{3}Album: {1}{3}Artist {2}",
                                  trackName, ApplicationSettings.AlbumName, ApplicationSettings.ArtistName, Environment.NewLine));
            }
        }

        public void Cancel()
        {
            TryClose();
        }

        private static string GetUniqueTrackName(string desiredName, ICollection<string> existingTracks)
        {
            var nameCandidate = desiredName;
            var counter = 1;
            while (existingTracks.Contains(nameCandidate))
                nameCandidate = String.Format("{0} ({1})", desiredName, counter++);

            return nameCandidate;
        }

        private static async Task CopyFileIntoIsoStore(IStorageFile sourceFile)
        {
            using (var s = await sourceFile.OpenReadAsync())
            using (var dr = new DataReader(s))
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            using (var targetFile = isoStore.CreateFile(sourceFile.Name))
            {
                var data = new byte[s.Size];
                await dr.LoadAsync((uint)s.Size);
                dr.ReadBytes(data);
                targetFile.Write(data, 0, data.Length);
            }
        }
    }
}
