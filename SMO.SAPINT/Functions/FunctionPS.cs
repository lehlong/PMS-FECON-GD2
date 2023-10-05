using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.SAPINT.Functions
{
    public class FunctionPS
    {
        public class Create_Project_Function : RfcFunctionObjectWithOutputMSG<string>
        {
            public override string FunctionName
            {
                get { return "ZBAPI_IMPORT_CJ20N"; }
            }

            public override string GetOutput(RfcResult result)
            {
                return "";
            }
        }

        public class Update_Project_Function : RfcFunctionObjectWithOutputMSG<string>
        {
            public override string FunctionName
            {
                get { return "ZBAPI_CHANGE_CJ20N"; }
            }

            public override string GetOutput(RfcResult result)
            {
                return "";
            }
        }

        public class Update_Order_Project_Function : RfcFunctionObjectWithOutputMSG<string>
        {
            public override string FunctionName
            {
                get { return "ZBAPI_SORT_CJ20N"; }
            }

            public override string GetOutput(RfcResult result)
            {
                return "";
            }
        }

        public class Create_ThucHien_Function : RfcFunctionObjectWithOutputMSG<string>
        {
            public override string FunctionName
            {
                get { return "ZBAPI_ITHUCHIEN"; }
            }

            public override string GetOutput(RfcResult result)
            {
                return "";
            }
        }

        public class Delete_ThucHien_Function : RfcFunctionObjectWithOutputMSG<string>
        {
            public override string FunctionName
            {
                get { return "ZBAPI_POSTINGS_REVERSE"; }
            }

            public override string GetOutput(RfcResult result)
            {
                return "";
            }
        }
    }
}
