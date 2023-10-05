using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO
{
    public abstract class BaseController<T, TRepo> : Controller where T: BaseEntity where TRepo : GenericRepository<T>
    {
        protected GenericService<T, TRepo> _serviceBase;

        public BaseController()
        {
        }

        public virtual ActionResult Index()
        {
            return PartialView(_serviceBase);
        }

        [ValidateAntiForgeryToken]
        public virtual ActionResult List(GenericService<T, TRepo> service)
        {
            _serviceBase.ObjDetail = service.ObjDetail;
            _serviceBase.Search();
            return PartialView(_serviceBase);
        }

        public virtual ActionResult Create()
        {
            return PartialView(_serviceBase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(GenericService<T, TRepo> service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _serviceBase.ObjDetail = service.ObjDetail;
            _serviceBase.Create();
            if (_serviceBase.State)
            {
                SMOUtilities.GetMessage("1001", _serviceBase, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _serviceBase, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public virtual ActionResult ToggleActive(string id)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _serviceBase.ToggleActive(id);
            var castService = (BaseService)_serviceBase;
            if (castService.State)
            {
                SMOUtilities.GetMessage("1002", castService, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", castService, result);
            }
            return result.ToJsonResult();
        }
    }
}