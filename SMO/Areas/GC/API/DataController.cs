using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using SMO.AppCode.GanttChart;

using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SMO.Areas.GC.API
{
    [GanttAPIExceptionFilter]
    public class DataController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetGanttChartData([FromUri]Guid projectId, [FromUri] bool isCostStructure)
        {
            var gantt = new GanttDto
            {
                data = new TaskController().Get(projectId, isCostStructure),
                links = new LinkController().Get(projectId)
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return Json(gantt, settings);
        }

        [HttpGet]
        public IHttpActionResult GetGanttChartDataVersion([FromUri] Guid projectId, [FromUri] bool isCostStructure, [FromUri] int version)
        {
            var gantt = new GanttDto
            {
                data = new TaskController().GetVersion(projectId, isCostStructure, version),
                links = new LinkController().Get(projectId)
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return Json(gantt, settings);
        }
    }
}
