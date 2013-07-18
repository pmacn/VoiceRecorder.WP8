
namespace VoiceRecorder.Model
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using VoiceRecorder.Data;
    using VoiceRecorder.Model.Events;

    // TODO: This needs to become more of a repository
    public class RecordingManager : PropertyChangedBase, IRecordingManager
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private readonly IRecordingStreamManager _streamManager;

        private readonly RecordingsContext _context;

        private List<Recording> _cache;

        #endregion

        #region Constructors

        public RecordingManager(
            IEventAggregator eventAggregator,
            IRecordingStreamManager streamManager,
            RecordingsContext context)
        {
            _context = context;
            _eventAggregator = eventAggregator;
            _streamManager = streamManager;
        }

        #endregion
        
        #region Methods

        public async Task<SaveRecordingResult> SaveAsync(Recording recording)
        {
            try
            {
                _context.Recordings.InsertOnSubmit(recording);
                _context.SubmitChanges();
                _cache.Add(recording);
                _eventAggregator.Publish(new RecordingCreated(recording));
                return SaveRecordingResult.Success;
            }
            catch
            {
                return SaveRecordingResult.UndefinedFailure;
            }
        }

        public async Task<DeleteRecordingResult> DeleteAsync(Guid recordingId)
        {
            var recording = await GetRecordingAsync(recordingId);
            if (recording == null)
                return DeleteRecordingResult.RecordingNotFound;

            try
            {
                _context.Recordings.DeleteOnSubmit(recording);
                _context.SubmitChanges();
                await _streamManager.DeleteStreamAsync(recordingId);
                _eventAggregator.Publish(new RecordingDeleted(recordingId));
                return DeleteRecordingResult.Success;
            }
            catch
            {
                return DeleteRecordingResult.UndefinedFailure;
            }
        }

        public async Task RenameAsync(Guid recordingId, string newName)
        {
            var recordingToRename = await GetRecordingAsync(recordingId);
            if (recordingToRename == null)
                return;

            recordingToRename.Name = newName;
            _context.SubmitChanges();
            _eventAggregator.Publish(new RecordingRenamed(recordingId, newName));
        }

        public async Task<IEnumerable<Recording>> GetRecordingsAsync()
        {
            return _cache ?? (_cache = _context.Recordings.ToList());
        }

        public async Task<Recording> GetRecordingAsync(Guid recordingId)
        {
            return (await GetRecordingsAsync()).SingleOrDefault(r => r.Id.Equals(recordingId));
        }

        public async Task AddTag(Guid recordingId, Guid tagId)
        {
            var recording = await GetRecordingAsync(recordingId);
            if (recording == null)
                return;

            if(recording.RecordingTags.Any(rt => rt.TagId.Equals(tagId)))
                return;

            recording.AddTag(tagId);
            _context.SubmitChanges();
            _eventAggregator.Publish(new TagAddedToRecording(tagId, recordingId));
        }

        public async Task RemoveTag(Guid recordingId, Guid tagId)
        {
            var recording = await GetRecordingAsync(recordingId);
            if (recording == null)
                return;

            var recordingTag = recording.RecordingTags.SingleOrDefault(rt => rt.TagId.Equals(tagId));
            if (recordingTag == null)
                return;

            recording.RecordingTags.Remove(recordingTag);
            _context.RecordingTags.DeleteOnSubmit(recordingTag);
            _context.SubmitChanges();
            _eventAggregator.Publish(new TagRemovedFromRecording(tagId, recordingId));
        }

        public async Task<IEnumerable<Tag>> GetTagsFor(Guid recordingId)
        {
            var tagIds = _context.RecordingTags.Where(rt => rt.RecordingId.Equals(recordingId)).Select(rt => rt.TagId);
            return _context.Tags.Where(t => tagIds.Contains(t.Id)).ToList();
        }

        #endregion
    }
}