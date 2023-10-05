using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONTRACT_FILE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid CONTRACT_ID { get; set; }
        public virtual Guid FILE_ID { get; set; }
        public virtual string FILE_NAME { get; set; }
        public virtual string FILE_EXTENSION { get; set; }
        public virtual decimal FILE_SIZE { get; set; }
    }
}
