
namespace VoiceRecorder.Model.Events
{
    using System;

    public class TagRenamed
    {
        #region Fields

        private readonly Guid TagId;

        private readonly string NewName;

        #endregion

        #region Constructors

        public TagRenamed(Guid tagId, string newName)
        {
            TagId = tagId;
            NewName = newName;
        }

        #endregion
    }
}
