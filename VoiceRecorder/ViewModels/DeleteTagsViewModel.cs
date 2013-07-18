using System.Linq;
using Caliburn.Micro;
using System.Collections.Generic;
using VoiceRecorder.Model;

namespace VoiceRecorder.ViewModels
{
    public class DeleteTagsViewModel : Screen
    {
        #region Fields

        private readonly ITagManager _tagManager;

        private readonly IEnumerable<Tag> _tagsToDelete;

        #endregion

        #region Constructors

        public DeleteTagsViewModel(ITagManager tagManager, IEnumerable<Tag> tagsToDelete)
        {
            _tagManager = tagManager;
            _tagsToDelete = tagsToDelete;
            HasTags = _tagsToDelete.Any();
        }

        #endregion

        public bool HasTags { get; set; }

        #region Methods

        public async void DeleteTags()
        {
            foreach (var tag in _tagsToDelete)
            {
                await _tagManager.Delete(tag.Id);
            }

            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }

        #endregion
    }
}
