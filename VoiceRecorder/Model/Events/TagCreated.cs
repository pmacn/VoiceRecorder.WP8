
namespace VoiceRecorder.Model.Events
{
    public class TagCreated
    {
        public readonly Tag Tag;

        public TagCreated(Tag tag)
        {
            Tag = tag;
        }
    }
}
