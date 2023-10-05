using SharpSapRfc;
using SMO.Core.Entities;
using System.Collections.Generic;

namespace SMO.SAPINT.Function
{
    public class SynMD_PO_Function : RfcFunctionObject<IEnumerable<T_MD_PO>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_PO"; }
        }

        public override IEnumerable<T_MD_PO> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_PO>("T_PO");
        }
    }

    public class SynMD_Batch_Function : RfcFunctionObject<IEnumerable<T_MD_BATCH>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_BATCH_ND"; }
        }

        public override IEnumerable<T_MD_BATCH> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_BATCH>("T_BATCH");
        }
    }

    public class SynMD_Customer_Function : RfcFunctionObject<IEnumerable<T_MD_CUSTOMER_OLD>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_CUSTOMER"; }
        }

        public override IEnumerable<T_MD_CUSTOMER_OLD> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_CUSTOMER_OLD>("T_CUSTOMER");
        }
    }

    public class SynMD_Contract_Function : RfcFunctionObject<IEnumerable<T_MD_CONTRACT>>
    {
        public override string FunctionName
        {
            get { return "ZFM_MD_GET_CONTRACT"; }
        }

        public override IEnumerable<T_MD_CONTRACT> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_CONTRACT>("T_CONTRACT");
        }
    }

    public class SynMD_Condition_Function : RfcFunctionObject<IEnumerable<T_MD_CONDITION>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_CONDTEXT"; }
        }

        public override IEnumerable<T_MD_CONDITION> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_CONDITION>("T_CONDITION");
        }
    }

    public class Syn_Condition_Function : RfcFunctionObject<IEnumerable<T_MD_CONDITION>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_GET_CONDITION"; }
        }

        public override IEnumerable<T_MD_CONDITION> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_CONDITION>("CONDITION");
        }
    }

    public class SynMD_DC_Function : RfcFunctionObject<IEnumerable<T_MD_DC>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_DC_ND"; }
        }

        public override IEnumerable<T_MD_DC> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_DC>("T_DC");
        }
    }

    public class SynMD_Division_Function : RfcFunctionObject<IEnumerable<T_MD_DIVISION>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_DIVISION_ND"; }
        }

        public override IEnumerable<T_MD_DIVISION> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_DIVISION>("T_DIVISION");
        }
    }

    public class SynMD_Material_Function : RfcFunctionObject<IEnumerable<T_MD_MATERIAL>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_MATERIAL"; }
        }

        public override IEnumerable<T_MD_MATERIAL> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_MATERIAL>("T_MATERIAL");
        }
    }

    public class SynMD_PaymentTerm_Function : RfcFunctionObject<IEnumerable<T_MD_PAYMENT_TERM>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_PAYMENTTERM_ND"; }
        }

        public override IEnumerable<T_MD_PAYMENT_TERM> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_PAYMENT_TERM>("T_PAYMENTTERM");
        }
    }

    public class SynMD_PoType_Function : RfcFunctionObject<IEnumerable<T_MD_POTYPE>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_POTYPE_ND"; }
        }

        public override IEnumerable<T_MD_POTYPE> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_POTYPE>("T_POTYPE");
        }
    }

    public class SynMD_Route_Function : RfcFunctionObject<IEnumerable<T_MD_ROUTE>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_ROUTE"; }
        }

        public override IEnumerable<T_MD_ROUTE> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_ROUTE>("T_ROUTE");
        }
    }

    public class SynMD_Dischard_Function : RfcFunctionObject<IEnumerable<T_MD_DISCHARD>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_DISCHARD"; }
        }

        public override IEnumerable<T_MD_DISCHARD> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_DISCHARD>("T_DISCHARD");
        }
    }

    public class SynMD_SaleOffice_Function : RfcFunctionObject<IEnumerable<T_MD_SALEOFFICE>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_SALEOFFICE"; }
        }

        public override IEnumerable<T_MD_SALEOFFICE> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_SALEOFFICE>("T_SALEOFFICE");
        }
    }


    public class SynMD_ShType_Function : RfcFunctionObject<IEnumerable<T_MD_SHTYPE>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_SHTYPE_ND"; }
        }

        public override IEnumerable<T_MD_SHTYPE> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_SHTYPE>("T_SHTYPE");
        }
    }

    public class SynMD_SlocCH_Function : RfcFunctionObject<IEnumerable<T_MD_SLOCCH>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_SLOCCH_ND"; }
        }

        public override IEnumerable<T_MD_SLOCCH> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_SLOCCH>("T_SLOCCH");
        }
    }

    public class SynMD_SoType_Function : RfcFunctionObject<IEnumerable<T_MD_SOTYPE>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_SOTYPE_ND"; }
        }

        public override IEnumerable<T_MD_SOTYPE> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_SOTYPE>("T_SOTYPE");
        }
    }

    //public class SynMD_TKTN_Function : RfcFunctionObject<IEnumerable<T_MD_T>>
    //{
    //    public override string FunctionName
    //    {
    //        get { return "ZBAPI_FM_TKTN_ND"; }
    //    }

    //    public override IEnumerable<T_MD_SOTYPE> GetOutput(RfcResult result)
    //    {
    //        return result.GetTable<T_MD_SOTYPE>("T_SOTYPE");
    //    }
    //}

    public class SynMD_Transmode_Function : RfcFunctionObject<IEnumerable<T_MD_TRANSMODE>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_TRANSMODE_ND"; }
        }

        public override IEnumerable<T_MD_TRANSMODE> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_TRANSMODE>("T_TRANSMODE");
        }
    }

    public class SynMD_Transunit_Function : RfcFunctionObject<IEnumerable<T_MD_TRANSUNIT>>
    {
        public override string FunctionName
        {
            get { return "ZBAPI_FM_TRANSUNIT_ND"; }
        }

        public override IEnumerable<T_MD_TRANSUNIT> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_TRANSUNIT>("T_TRANSUNIT");
        }
    }

    public class SynMD_Unit_Function : RfcFunctionObject<IEnumerable<T_MD_UNIT_OLD>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_UNIT"; }
        }

        public override IEnumerable<T_MD_UNIT_OLD> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_UNIT_OLD>("T_UNIT");
        }
    }

    public class SynMD_Vehicle_Function : RfcFunctionObject<IEnumerable<VEHICLE_COMPARTMENT>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_VEHICLE"; }
        }

        public override IEnumerable<VEHICLE_COMPARTMENT> GetOutput(RfcResult result)
        {
            return result.GetTable<VEHICLE_COMPARTMENT>("T_VEHICLE");
        }
    }

    public class SynMD_Vendor_Function : RfcFunctionObject<IEnumerable<T_MD_VENDOR_OLD>>
    {
        public override string FunctionName
        {
            get { return "ZSMO_MD_GET_VENDOR"; }
        }

        public override IEnumerable<T_MD_VENDOR_OLD> GetOutput(RfcResult result)
        {
            return result.GetTable<T_MD_VENDOR_OLD>("T_VENDOR");
        }
    }
}
