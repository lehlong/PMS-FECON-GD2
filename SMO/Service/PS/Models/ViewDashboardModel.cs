using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO.Service.PS.Models
{
    public class ViewDashboardModel
    {
        public decimal CA { get; set; }
        public decimal? BAC { get; set; }
        public decimal? TSLNKH { get; set; }
        public decimal? WP { get; set; }
        public decimal WD { get; set; }
        public decimal? SPI { get; set; }
        public decimal ACW { get; set; }
        public decimal API { get; set; }
        public decimal? PE { get; set; }
        public decimal AC { get; set; }
        public decimal? BCWP { get; set; }
        public decimal? CPI { get; set; }
        public decimal NT { get; set; }
        public decimal? EAC { get; set; }
        public decimal? DTDK { get; set; }
        public decimal? TSLNDK { get; set; }
        public decimal TSLNTT { get; set; }
        public List<string> ConfigDashboard { get; set; }
        public Array DataDashboardBoq { get; set; }
        public Array DataDashboardCost { get; set; }
        public ArrayList DataDashboardCostLevel2 { get; set; }
    }
}