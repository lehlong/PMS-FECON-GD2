using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSapRfc;

namespace SMO.SAPINT.Class
{
    public class BAPIDOCHDRR
    {
        [RfcStructureField("RVRS_NO")]
        public string RVRS_NO { get; set; }
    }
}
