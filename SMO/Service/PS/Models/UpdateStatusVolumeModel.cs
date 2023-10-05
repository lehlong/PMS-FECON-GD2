using System;

namespace SMO.Service.PS.Models
{
    public class UpdateStatusVolumeModel
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string Note { get; set; }
        public string SAP_DOCID { get; set; }
    }
}
