using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace SMO.AppCode.GanttChart
{
    public class GanttAPIExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {

            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                action = "error",
                message = context.Exception.Message
            });
        }
    }
}