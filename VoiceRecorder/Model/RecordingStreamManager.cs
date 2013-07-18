
namespace VoiceRecorder.Model
{
    using System;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;

    public class RecordingStreamManager : IRecordingStreamManager
    {
        #region Fields

        private const string FolderName = "Items";

        #endregion

        #region Methods

        public async Task<IRandomAccessStream> GetOrCreateStreamAsync(Guid recordingId)
        {
            var recordingFileName = recordingId.ToString();
            var folder = await GetRecordingsFolderAsync();
            var file = await folder.CreateFileAsync(recordingFileName, CreationCollisionOption.OpenIfExists);
            return await file.OpenAsync(FileAccessMode.ReadWrite);
        }

        public async Task DeleteStreamAsync(Guid recordingId)
        {
            var file = await GetRecordingFileAsync(recordingId);
            await file.DeleteAsync();
        }

        public async Task<string> GetPathAsync(Guid recordingId)
        {
            var file = await GetRecordingFileAsync(recordingId);
            return file.Path;
        }

        public async Task<StorageFile> GetStorageFileAsync(Recording recording)
        {
            return await GetRecordingFileAsync(recording.Id);
        }
 
        private static async Task<StorageFolder> GetRecordingsFolderAsync()
        {
            return await ApplicationData.Current
                                        .LocalFolder
                                        .CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);
        }

        private static async Task<StorageFile> GetRecordingFileAsync(Guid recordingId)
        {
            var folder = await GetRecordingsFolderAsync();
            return await folder.GetFileAsync(recordingId.ToString());
        }
        
        #endregion
   }
}