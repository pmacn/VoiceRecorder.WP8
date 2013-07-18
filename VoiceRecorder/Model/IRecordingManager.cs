
namespace VoiceRecorder.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRecordingManager
    {
        Task<IEnumerable<Recording>> GetRecordingsAsync();
        Task<Recording> GetRecordingAsync(Guid value);
        Task<SaveRecordingResult> SaveAsync(Recording recording);
        Task<DeleteRecordingResult> DeleteAsync(Guid recordingId);
        Task RenameAsync(Guid recordingId, string newName);

        // TODO: move to tagrepo
        Task<IEnumerable<Tag>> GetTagsFor(Guid guid);
        Task AddTag(Guid recordingId, Guid tagId);
        Task RemoveTag(Guid guid1, Guid guid2);
    }

    public enum SaveRecordingResult
    {
        Success,
        UndefinedFailure
    }

    public enum DeleteRecordingResult
    {
        Success,
        RecordingNotFound,
        UndefinedFailure
    }
}