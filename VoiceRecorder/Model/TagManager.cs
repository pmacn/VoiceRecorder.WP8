#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using VoiceRecorder.Data;
using VoiceRecorder.Model.Events;

#endregion

namespace VoiceRecorder.Model
{
    public interface ITagManager
    {
        Task<IEnumerable<Tag>> GetAll();
        Task<CreateTagResult> Create(string desiredName);
        Task<DeleteTagResult> Delete(Guid tagId);
        Task<RenameTagResult> Rename(Guid tagId, string desiredName);
        Task<Tag> GetById(Guid guid);
    }

    public class TagManager : ITagManager
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly IRecordingManager _recordingManager;

        private readonly RecordingsContext _context;

        private List<Tag> _cache;

        public TagManager(IEventAggregator eventAggregator, RecordingsContext context, IRecordingManager recordingManager)
        {
            _eventAggregator = eventAggregator;
            _context = context;
            _recordingManager = recordingManager;
        }

        public async Task<IEnumerable<Tag>> GetAll()
        {
            return _cache ?? (_cache = _context.Tags.ToList());
        }

        public async Task<RenameTagResult> Rename(Guid tagId, string desiredName)
        {
            var tag = await GetById(tagId);
            if (tag == null)
                return RenameTagResult.TagWithNameAlreadyExists;

            tag.Name = desiredName;
            try
            {
                _context.SubmitChanges();
                return RenameTagResult.Success;
            }
            catch
            {
                return RenameTagResult.UndefinedFailure;
            }
        }

        public async Task<Tag> GetById(Guid tagId)
        {
            return (await GetAll()).SingleOrDefault(t => t.Id.Equals(tagId));
        }

        public async Task<CreateTagResult> Create(string desiredName)
        {
            if ((await GetAll()).Any(t => t.Name.Equals(desiredName)))
                return CreateTagResult.TagWithNameAlreadyExists;

            var newTag = new Tag(desiredName);
            _context.Tags.InsertOnSubmit(newTag);
            try
            {
                _context.SubmitChanges();
            }
            catch
            {
                return CreateTagResult.UndefinedFailure;
            }

            _cache.Add(newTag);
            _eventAggregator.Publish(new TagCreated(newTag));
            return CreateTagResult.Success;
        }

        public async Task<DeleteTagResult> Delete(Guid tagId)
        {
            var tag = await GetById(tagId);
            if (tag == null)
                return DeleteTagResult.TagDoesNotExist;

            var recordings = (await _recordingManager.GetRecordingsAsync()).Where(r => r.RecordingTags.Any(rt => rt.TagId.Equals(tagId)));
            foreach (var recording in recordings)
            {
                await _recordingManager.RemoveTag(recording.Id, tagId);
            }
            _context.Tags.DeleteOnSubmit(tag);
            try
            {
                _context.SubmitChanges();
            }
            catch (Exception)
            {
                return DeleteTagResult.UndefinedFailure;
            }

            _cache.Remove(tag);
            _eventAggregator.Publish(new TagDeleted(tagId));
            return DeleteTagResult.Success;
        }
    }

    public enum CreateTagResult
    {
        Success,
        TagWithNameAlreadyExists,
        UndefinedFailure
    }

    public enum RenameTagResult
    {
        Success,
        TagDoesNotExist,
        TagWithNameAlreadyExists,
        UndefinedFailure
    }

    public enum DeleteTagResult
    {
        Success,
        TagDoesNotExist,
        UndefinedFailure
    }
}