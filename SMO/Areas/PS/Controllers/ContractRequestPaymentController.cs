using System;
using System.Web.Mvc;
using SMO.Repository.Implement.PS;
using SMO.Service.MD;
using SMO.Service.PS;

namespace SMO.Areas.PS.Controllers
{

    public class ContractRequestPaymentController : Controller
    {
        private ContractRequestPaymentService _service;

        public ContractRequestPaymentController()
        {
            _service = new ContractRequestPaymentService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid contractId)
        {
            _service.ObjDetail.CONTRACT_ID = contractId;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(ContractRequestPaymentService service)
        {
            service.Search();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create(Guid contractId)
        {
            _service.ObjDetail.CONTRACT_ID = contractId;
            _service.ObjDetail.PAYMENT_DATE = DateTime.Now;
            _service.ObjDetail.REFERENCE_FILE_ID = Guid.NewGuid();
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContractRequestPaymentService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.ObjDetail.ID = Guid.NewGuid();
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = $"SubmitIndexContractRequestPayment();Forms.Close('{service.ViewId}');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Edit(Guid id)
        {
            _service.Get(id);
            return PartialView(_service);
        }

        public ActionResult DetailStep(Guid id)
        {
            var item = _service.GetStep(id);
            return PartialView(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ContractRequestPaymentService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "SubmitIndexContractRequestPayment();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string pStrListSelected)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.Delete(pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitIndexContractRequestPayment();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }
    }
}