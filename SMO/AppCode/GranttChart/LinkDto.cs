using SMO.Core.Entities.PS;

using System;

namespace SMO.AppCode.GanttChart
{
    public class LinkDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid Source { get; set; }
        public Guid Target { get; set; }

        public static explicit operator LinkDto(T_PS_REFERENCE link)
        {
            return new LinkDto
            {
                Id = link.ID,
                Type = link.TYPE,
                Source = link.SOURCE_ID,
                Target = link.TARGET_ID
            };
        }

        public static explicit operator T_PS_REFERENCE(LinkDto link)
        {
            return new T_PS_REFERENCE
            {
                ID = link.Id,
                TYPE = link.Type,
                SOURCE_ID = link.Source,
                TARGET_ID = link.Target
            };
        }
    }
}