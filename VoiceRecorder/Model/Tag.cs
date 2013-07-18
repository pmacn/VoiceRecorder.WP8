
namespace VoiceRecorder.Model
{
    using Caliburn.Micro;
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    [Table(Name = "Tag")]
    public class Tag : PropertyChangedBase, IEquatable<Tag>
    {
        #region Fields

        private readonly EntitySet<RecordingTag> _recordingTags;

        private Guid _id;

        private string _name;

        #endregion

        #region Constructors

        public Tag()
        {
            _recordingTags = new EntitySet<RecordingTag>();
        }

        public Tag(string name)
            : this()
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        #endregion

        #region Properties

        [Column(DbType = "UNIQUEIDENTIFIER", CanBeNull = false, IsPrimaryKey = true, IsDbGenerated = false)]
        public Guid Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                    return;

                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        [Column(DbType = "NVARCHAR(50)")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        [Association(Storage="_recordingTags", OtherKey = "TagId", DeleteRule="CASCADE")]
        public EntitySet<RecordingTag> RecordingTags
        {
            get
            {
                return _recordingTags;
            }
            set
            {
                _recordingTags.Assign(value);
                NotifyOfPropertyChange(() => RecordingTags);
            }
        }

        #endregion
        
        #region Methods

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Tag);
        }

        public bool Equals(Tag other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id);
        }

        #endregion
    }
}