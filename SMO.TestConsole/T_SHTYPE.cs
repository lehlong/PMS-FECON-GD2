using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.TestConsole
{
    public class T_MD_SHTYPE
    {
        [RfcStructureField("SHTYP")]
        public string CODE { get; set; }

        [RfcStructureField("SHTYPDESC")]
        public string TEXT { get; set; }

        public string TEST { get; set; }
    }
}
