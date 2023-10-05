using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CM_FILE_UPLOAD : BaseEntity
    {
        public virtual Guid PKID { get; set; }
        public virtual string FILE_NAME { get; set; }
        public virtual string FILE_OLD_NAME { get; set; }
        public virtual string FILE_EXT { get; set; }
        public virtual decimal FILE_SIZE { get; set; }
        public virtual string CONNECTION_ID { get; set; }
        public virtual string DATABASE_NAME { get; set; }
        public virtual string DIRECTORY_PATH { get; set; }
    }
}
