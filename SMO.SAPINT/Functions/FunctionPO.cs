using SharpSapRfc;
using SMO.Core.Entities;
using SMO.SAPINT.Class;
using System.Collections.Generic;

namespace SMO.SAPINT.Function
{
    public class Create_PO_Function : RfcFunctionObjectWithOutputMSG<string>
    {
        public override string FunctionName
        {
            get { return "ZFM_SMO_CREATE"; }
        }

        public override string GetOutput(RfcResult result)
        {
            return "";
        }
    }

    

    public class Update_PO_Function : RfcFunctionObjectWithOutputMSG<string>
    {
        public override string FunctionName
        {
            get { return "ZFM_SMO_CHANGE "; }
        }

        public override string GetOutput(RfcResult result)
        {
            return "";
        }
    }

    public class Get_PO_Function : RfcFunctionObjectWithOutputMSG<string>
    {
        public override string FunctionName
        {
            get { return "ZFM_SMO_GET_SAPINF"; }
        }

        public override string GetOutput(RfcResult result)
        {
            return "";
        }
    }

    public class Get_TonHG_Function : RfcFunctionObject<IEnumerable<ZBAPI_STR_HANGGUI>>
    {
        public override string FunctionName
        {
            get { return "ZFM_GET_SMO2"; }
        }

        public override IEnumerable<ZBAPI_STR_HANGGUI> GetOutput(RfcResult result)
        {
            return result.GetTable<ZBAPI_STR_HANGGUI>("T_EXPORT");
        }
    }

    public class TEST_Create_SO_Function : RfcFunctionObjectWithOutputMSG<string>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_CREATE_SO"; }
        }

        public override string GetOutput(RfcResult result)
        {
            return result.GetOutput<string>("SALESDOCUMENT");
        }
    }

    public class TEST_Create_DO_Function : RfcFunctionObjectWithOutputMSG<string>
    {
        public override string FunctionName
        {
            get { return "ZFM_DO_CREATE"; }
        }

        public override string GetOutput(RfcResult result)
        {
            return result.GetOutput<string>("O_DO");
        }
    }

    public class TEST_Create_PGI_Function : RfcFunctionObjectWithOutputMSG<string>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_PGI_7"; }
        }

        public override string GetOutput(RfcResult result)
        {
            return result.GetOutput<string>("SUBRC");
        }
    }
}
