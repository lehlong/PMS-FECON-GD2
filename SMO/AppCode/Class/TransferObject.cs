using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SMO
{
    public class TransferObject
    {
        private object _data;

        public string Type { get; set; }

        public bool State { get; set; }

        public MessageObject Message { get; set; }

        public object Data
        {
            get
            {
                return this._data;
            }
            set
            {
                if (value == null)
                    return;
                this._data = value;
                this.DataType = this._data.GetType().ToString();
            }
        }

        public string DataType { get; private set; }

        public object ExtData { get; set; }

        public string ExtDataType { get; set; }

        public TransferObject()
        {
            this.Type = TransferType.Json;
            this.Message = new MessageObject();
        }

        public JsonResult ToJsonResult()
        {
            return new JsonNetResult()
            {
                Data = (object)this,
                ContentType = (string)null,
                ContentEncoding = (Encoding)null,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public void MergeModelState(ModelStateDictionary _modelState)
        {
            if (_modelState != null && !_modelState.IsValid)
            {
                this.Message.Code = "0001";
                this.Message.Message = "Error when validate model state";
                string str = "";
                foreach (ModelState modelState in (IEnumerable<ModelState>)_modelState.Values)
                {
                    if (modelState.Errors.Count > 0)
                    {
                        foreach (ModelError modelError in (Collection<ModelError>)modelState.Errors)
                            str = str + string.Format("{0}{1}", modelError.ErrorMessage, "<br/>");
                    }
                }
                this.Message.Detail = str;
            }
        }
    }

    public static class TransferType
    {
        public static readonly string Unknow = "UNKNOW";
        public static readonly string Redirect = "REDIRECT";
        public static readonly string Json = "JSON";
        public static readonly string Message = "MESSAGE";
        public static readonly string Alert = "ALERT";
        public static readonly string Dialog = "DIALOG";
        public static readonly string JsFunction = "JSFUNCTION";
        public static readonly string JsCommand = "JSCOMMAND";
        public static readonly string AlertSuccess = "ALERTSUCCESS";
        public static readonly string AlertInfo = "ALERTINFO";
        public static readonly string AlertWarning = "ALERTWARNING";
        public static readonly string AlertDanger = "ALERTDANGER";

        private static string _separator;
        private static string _multiTypePattern;

        public static string Separator
        {
            get
            {
                return TransferType._separator;
            }
            set
            {
                TransferType._separator = value;
                TransferType._multiTypePattern = "{0}" + TransferType._separator + "{1}";
            }
        }

        public static string MessageAndRedirect
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Message, (object)TransferType.Redirect);
            }
        }

        public static string MessageAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Message, (object)TransferType.JsFunction);
            }
        }

        public static string MessageAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Message, (object)TransferType.JsCommand);
            }
        }

        public static string JsFunctionAndMessage
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsFunction, (object)TransferType.Message);
            }
        }

        public static string JsCommandAndMessage
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsCommand, (object)TransferType.Message);
            }
        }

        public static string AlertAndRedirect
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Alert, (object)TransferType.Redirect);
            }
        }

        public static string AlertAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Alert, (object)TransferType.JsFunction);
            }
        }

        public static string AlertAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Alert, (object)TransferType.JsCommand);
            }
        }

        public static string JsFunctionAndAlert
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsFunction, (object)TransferType.Alert);
            }
        }

        public static string JsCommandAndAlert
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsCommand, (object)TransferType.Alert);
            }
        }

        public static string DialogAndRedirect
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Dialog, (object)TransferType.Redirect);
            }
        }

        public static string DialogAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Dialog, (object)TransferType.JsFunction);
            }
        }

        public static string DialogAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.Dialog, (object)TransferType.JsCommand);
            }
        }

        public static string JsFunctionAndDialog
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsFunction, (object)TransferType.Dialog);
            }
        }

        public static string JsCommandAndDialog
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsCommand, (object)TransferType.Dialog);
            }
        }

        public static string AlertSuccessAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertSuccess, (object)TransferType.JsFunction);
            }
        }

        public static string AlertSuccessAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertSuccess, (object)TransferType.JsCommand);
            }
        }

        public static string JsFunctionAndAlertSuccess
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsFunction, (object)TransferType.AlertSuccess);
            }
        }

        public static string JsCommandAndAlertSuccess
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsCommand, (object)TransferType.AlertSuccess);
            }
        }




        public static string AlertInfoAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertInfo, (object)TransferType.JsFunction);
            }
        }

        public static string AlertInfoAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertSuccess, (object)TransferType.JsCommand);
            }
        }

        public static string JsFunctionAndAlertInfo
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsFunction, (object)TransferType.AlertInfo);
            }
        }

        public static string JsCommandAndAlertInfo
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsCommand, (object)TransferType.AlertInfo);
            }
        }




        public static string AlertDangerAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertDanger, (object)TransferType.JsFunction);
            }
        }

        public static string AlertDangerAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertDanger, (object)TransferType.JsCommand);
            }
        }

        public static string JsFunctionAndAlertDanger
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsFunction, (object)TransferType.AlertDanger);
            }
        }

        public static string JsCommandAndAlertDanger
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsCommand, (object)TransferType.AlertDanger);
            }
        }








        public static string AlertWarningAndJsFunction
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertWarning, (object)TransferType.JsFunction);
            }
        }

        public static string AlertWarningAndJsCommand
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.AlertWarning, (object)TransferType.JsCommand);
            }
        }

        public static string JsFunctionAndAlertWarning
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsFunction, (object)TransferType.AlertWarning);
            }
        }

        public static string JsCommandAndAlertWarning
        {
            get
            {
                return string.Format(TransferType._multiTypePattern, (object)TransferType.JsCommand, (object)TransferType.AlertWarning);
            }
        }

        static TransferType()
        {
            TransferType.Separator = "_";
        }
    }
}