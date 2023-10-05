using SMO.Core.Common;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_COMMENT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string MESSENGER { get; set; }
        public virtual string IS_FILE { get; set; }
        public virtual string PATH_FILE { get; set; }
        public virtual string MIME_TYPE { get; set; }

    }
}
