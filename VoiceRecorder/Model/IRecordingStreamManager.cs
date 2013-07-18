
namespace VoiceRecorder.Model
{
    using System;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;

    public interface IRecordingStreamManager
    {
        Task<IRandomAccessStream> GetOrCreateStreamAsync(Guid recordingId);
        Task DeleteStreamAsync(Guid recordingId);
        Task<string> GetPathAsync(Guid recordingId);
        Task<StorageFile> GetStorageFileAsync(Recording recording);
    }
}
