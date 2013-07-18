
namespace VoiceRecorder.Model.Events
{
    using System;

    public class TagDeleted
    {
        public readonly Guid TagId;

        public TagDeleted(Guid tagId)
        {
            TagId = tagId;
        }
    }
}
