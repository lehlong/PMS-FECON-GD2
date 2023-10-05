using System;

namespace SMO.AppCode.GranttChart
{
    public class UpdateTasksTotalDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public decimal Total { get; set; }
    }
}
