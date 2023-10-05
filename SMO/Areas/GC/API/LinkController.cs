using SMO.AppCode.GanttChart;
using SMO.Core.Entities.PS;
using SMO.Service.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SMO.Areas.GC.API
{
    [AuthorizeCustom]
    [GanttAPIExceptionFilter]
    public class LinkController : ApiController
    {
        private readonly ReferenceService _service;

        public LinkController()
        {
            _service = new ReferenceService();
        }
        // GET api/Link
        [HttpGet]
        [MyValidateAntiForgeryToken]
        public IEnumerable<LinkDto> Get(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("message", nameof(projectId));
            }
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.Search();
            // select project or show all
            return _service.ObjList
                .Select(t => (LinkDto)t);
        }

        // POST api/Link
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public IHttpActionResult CreateLink(T_PS_REFERENCE link)
        {
            link.ID = Guid.NewGuid();

            _service.ObjDetail = link;
            _service.Create();

            if (_service.State)
            {
                return Ok(new
                {
                    tid = _service.ObjDetail.ID,
                    action = "inserted",
                    status = "success"
                });
            }
            else
            {
                return Ok(new
                {
                    message = _service.ErrorMessage,
                    messageCode = _service.MesseageCode,
                    status = "error"
                });
            }
        }

        // PUT api/Link/5
        [HttpPut]
        public IHttpActionResult EditLink(Guid id, LinkDto linkDto)
        {
            var clientLink = (T_PS_REFERENCE)linkDto;
            clientLink.ID = id;
            _service.ObjDetail = clientLink;
            _service.Update();

            if (_service.State)
            {
                return Ok(new
                {
                    action = "updated",
                    status = "success"
                });
            }
            else
            {
                return Ok(new
                {
                    message = _service.ErrorMessage,
                    messageCode = _service.MesseageCode,
                    status = "error"
                });
            }
        }

        // DELETE api/Link/5
        [HttpDelete]
        [MyValidateAntiForgeryToken]
        [Route("api/link/{id}")]
        public IHttpActionResult DeleteLink(string id, string version)
        {
            _service.Get(id);
            if (_service.ObjDetail != null)
            {
                    _service.Delete(id);
            }

            if (_service.State)
            {
                return Ok(new
                {
                    action = "deleted",
                    status = "success"
                });
            }
            else
            {
                return Ok(new
                {
                    message = _service.ErrorMessage,
                    messageCode = _service.MesseageCode,
                    status = "error"
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.UnitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
