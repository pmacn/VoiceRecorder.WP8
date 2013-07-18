
namespace VoiceRecorder.Model
{
    using Caliburn.Micro;
    using System;
    using System.Data.Linq.Mapping;

    [Table(Name = "RecordingTag")]
    public class RecordingTag : PropertyChangedBase
    {
        #region Fields

        #endregion

        #region Properties

        [Column(IsPrimaryKey=true)]
        public Guid RecordingId { get; set; }

        [Column(IsPrimaryKey=true)]
        public Guid TagId { get; set; }

        #endregion
    }
}
