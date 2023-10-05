using SharpSapRfc;
using System.Collections.Generic;

namespace SMO.SAPINT
{
    public abstract class RfcFunctionObjectWithOutputMSG<T> : RfcFunctionObject<T>
    {
        public BAPIRET2 GetMSGOutput(string strName = "O_MSGID")
        {
            try
            {
                return this.Result.GetOutput<BAPIRET2>(strName);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (System.Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return new BAPIRET2();
            }
        }

        public IEnumerable<BAPIRET2> GetTableMSGOutput(string table = "")
        {
            try
            {
                return this.Result.GetTable<BAPIRET2>(table);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (System.Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return new List<BAPIRET2>();
            }
        }

        public string GetAllStringMessage(string table = "")
        {
            var msg = this.GetMSGOutput();
            var tableMsg = this.GetTableMSGOutput(table);
            var template = "Type: {0}. Message: {1} {2} {3} {4} {5} <br/>";
            var strResult = string.Empty;
            strResult += string.Format(template, msg.TYPE, msg.MESSAGE, msg.MESSAGE_V1, msg.MESSAGE_V2, msg.MESSAGE_V3, msg.MESSAGE_V4);
            foreach (var item in tableMsg)
            {
                strResult += string.Format(template, item.TYPE, item.MESSAGE, item.MESSAGE_V1, item.MESSAGE_V2, item.MESSAGE_V3, item.MESSAGE_V4);
            }
            return strResult;
        }
    }
}
