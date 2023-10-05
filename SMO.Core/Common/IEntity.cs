using System;

namespace SMO.Core.Interface
{
    public interface IEntity
    {
        string CREATE_BY { get; set; }

        DateTime? CREATE_DATE { get; set; }

        string UPDATE_BY { get; set; }

        DateTime? UPDATE_DATE { get; set; }
    }
}
