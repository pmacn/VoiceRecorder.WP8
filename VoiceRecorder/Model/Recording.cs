using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace VoiceRecorder.Model
{
    [Table(Name = "Recording")]
    public class Recording : PropertyChangedBase, IEquatable<Recording>
    {
        #region Fields

        private readonly EntitySet<RecordingTag> _recordingTags;

        private DateTimeOffset _dateCreated;

        private Guid _id;

        private string _name;

        #endregion

        #region Constructors

        public Recording()
        {
            _recordingTags = new EntitySet<RecordingTag>();
        }

        public Recording(Guid id, string name)
            : this()
        {
            Id = id;
            Name = name;
            DateCreated = DateTimeOffset.Now;
        }

        public Recording(Guid id, DateTime dateCreated)
            : this()
        {
            Id = id;
            DateCreated = dateCreated;
        }

        #endregion

        #region Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "UNIQUEIDENTIFIER NOT NULL", CanBeNull = false)]
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

        [Column(DbType = "DATETIME NOT NULL")]
        public DateTimeOffset DateCreated
        {
            get { return _dateCreated; }
            set
            {
                if (_dateCreated == value)
                    return;

                _dateCreated = value;
                NotifyOfPropertyChange(() => DateCreated);
                NotifyOfPropertyChange(() => FormattedDateCreated);
            }
        }

        [Association(Storage = "_recordingTags", OtherKey = "RecordingId", DeleteRule = "CASCADE")]
        public EntitySet<RecordingTag> RecordingTags
        {
            get { return _recordingTags; }
            set
            {
                _recordingTags.Assign(value);
                NotifyOfPropertyChange(() => RecordingTags);
            }
        }

        public string FormattedDateCreated
        {
            get { return DateCreated.ToString("G"); }
        }

        #endregion

        #region Methods

        public IEnumerable<Guid> TagIds
        {
            get { return RecordingTags.Select(rt => rt.TagId); }
        }

        public bool Equals(Recording other)
        {
            if (other == null)
                return false;

            return Id.Equals(other.Id);
        }

        public void AddTag(Guid tagId)
        {
            RecordingTags.Add(new RecordingTag {RecordingId = Id, TagId = tagId});
        }

        public void RemoveTag(Guid tagId)
        {
            List<RecordingTag> tags = RecordingTags.Where(rt => rt.TagId == tagId).ToList();
            foreach (RecordingTag tag in tags)
                RecordingTags.Remove(tag);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Recording);
        }

        #endregion
    }
}