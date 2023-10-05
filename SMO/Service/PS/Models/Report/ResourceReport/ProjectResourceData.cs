using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO.Service.PS.Models.Report.ResourceReport
{
    public class ProjectResourceData
    {
        public int? Stt { get; set; }
        public string Username { get; set; }
        public string ProjectRole { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string TypeResource { get; set; }
        public string OtherResource { get; set; }
        public string Department { get; set; }
        public string TitleResource { get; set; }
        public string NumberCccd { get; set; }
        public DateTime? FromDate { get;set; }
        public DateTime? ToDate { get; set; } 

    }
}