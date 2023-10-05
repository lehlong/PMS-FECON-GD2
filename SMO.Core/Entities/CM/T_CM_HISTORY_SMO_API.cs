using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CM_HISTORY_SMO_API : BaseEntity
    {
        public virtual int ID { get; set; }
        public virtual string C_FUNCTION { get; set; }
        public virtual string PARAMETER { get; set; }
        public virtual string RESULT { get; set; }

        public virtual string SEARCH_TEXT { get; set; }
        public virtual DateTime? SEARCH_FROM_DAY { get; set; }
        public virtual DateTime? SEARCH_TO_DAY { get; set; }
    }
}
