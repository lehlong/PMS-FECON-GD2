using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace SMO
{
    public static class FormDataUtils
    {
        /// <summary>
        /// Hàm tạo AjaxOptions
        /// </summary>
        /// <param name="viewId">Là element html parent của form chứa updateTargetId</param>
        /// <param name="updateTargetId">Là element được cập nhật nội dung vào</param>
        /// <returns></returns>
        public static AjaxOptions GetAjaxOptions(string viewId = "", string updateTargetId = "")
        {
            if (!string.IsNullOrWhiteSpace(viewId))
            {
                viewId = "#" + viewId;
            }
            var result = new AjaxOptions
            {
                HttpMethod = "post",
                OnBegin = "Forms.AjaxBeginHandler",
                OnSuccess = "Forms.AjaxSuccessHandler(data, status, xhr, '" + viewId + "', '" + updateTargetId + "')",
                OnFailure = "Forms.AjaxErrorHandler",
                OnComplete = "Forms.AjaxCompleteHandler"
            };
            return result;
        }
    }
}