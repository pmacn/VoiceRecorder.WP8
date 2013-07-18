using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceRecorder.Model.Events
{
    public class TagFilterChanged
    {
        public IList<Guid> TagsIdsAdded;
        public IList<Guid> TagIdsRemoved;
    }
}
