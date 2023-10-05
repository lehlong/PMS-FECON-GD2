using Newtonsoft.Json;

using SMO.AppCode.GanttChart;
using SMO.AppCode.GranttChart;
using SMO.Core.Entities.PS;
using SMO.Service.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace SMO.Areas.GC.API
{
    [GanttAPIExceptionFilter]
    public class TaskController : ApiController
    {
        private readonly ProjectStructService _service;

        public TaskController()
        {
            _service = new ProjectStructService();
        }
        // GET api/Task
        public IEnumerable<TaskDto> Get(Guid projectId, bool isCostStructure)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("message", nameof(projectId));
            }
            _service.ObjDetail.PROJECT_ID = projectId;

            if (isCostStructure)
            {
                _service.ObjDetail.TYPE = string.Join(",", new List<string>()
                                            {
                                                ProjectEnum.PROJECT.ToString(),
                                                ProjectEnum.ACTIVITY.ToString(),
                                                ProjectEnum.WBS.ToString()
                                            });
            }
            else
            {
                _service.ObjDetail.TYPE = string.Join(",", new List<string>()
                                            {
                                                ProjectEnum.PROJECT.ToString(),
                                                ProjectEnum.BOQ.ToString(),
                                            });
            }
            _service.Search();

            var taskDtos = _service.ObjList.Where(x => x.ACTIVE == true).Select(t => (TaskDto)t).ToList();

            _service.InitContractCode(taskDtos, isCostStructure);
            return taskDtos;
        }

        // GET api/Task/5
        [HttpGet]
        [Route("api/task/GetTaskById")]
        public IHttpActionResult GetTaskById(Guid id)
        {
            _service.Get(id);
            if (_service.State)
            {
                if (_service.ObjDetail != null)
                {
                    return Ok(new
                    {
                        task = JsonConvert.SerializeObject(_service.ObjDetail),
                        status = "found"
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = "not_found"
                    });
                }
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

        // PUT api/Task/5
        [HttpPost]
        public IHttpActionResult EditTask(Guid id, TaskDto taskDto)
        {
            var updatedTask = (T_PS_PROJECT_STRUCT)taskDto;
            updatedTask.UPDATE_BY = taskDto.User;
            _service.ObjDetail = updatedTask;

            _service.Update(taskDto);
            if (_service.State)
            {
                return Ok(new
                {
                    action = "updated",
                    status = "success",
                    //executeMethod = "BuildTree"
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

        [HttpPost]
        [Route("api/task/CreateTask")]
        public IHttpActionResult CreateTask(TaskDto taskDto)
        {
            var newTask = (T_PS_PROJECT_STRUCT)taskDto;
            newTask.CREATE_BY = taskDto.User;
            var id = Guid.NewGuid();
            newTask.ID = id;
            if (taskDto.Type == ProjectEnum.ACTIVITY.ToString())
            {
                newTask.ACTIVITY_ID = Guid.NewGuid();
            }
            _service.ObjDetail = newTask;
            _service.ObjDetail.STATUS = ProjectStructStatus.KHOI_TAO.GetValue();
            _service.Create();
            if (_service.State)
            {
                return Ok(new
                {
                    tid = newTask.ID,
                    code = _service.ObjDetail.GEN_CODE,
                    action = "inserted",
                    status = "success",
                    statusCode = _service.ObjDetail.STATUS,
                    //executeMethod = "BuildTree"
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
        [HttpPost]
        [Route("api/task/UpdateOrder")]
        public IHttpActionResult UpdateOrder(UpdateTasksOrderDto tasksOrderDto)
        {
            _service.UpdateTasksOrder(tasksOrderDto);
            if (_service.State)
            {
                return Ok();
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
        [HttpPost]
        [Route("api/task/UpdateTasksTotal")]
        public IHttpActionResult UpdateTasksTotal(bool isCustomer, IEnumerable<UpdateTasksTotalDto> data)
        {
            _service.UpdateTasksTotal(isCustomer, data);
            if (_service.State)
            {
                return Ok();
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

        // DELETE api/Task/5
        [HttpDelete]
        public IHttpActionResult DeleteTask(Guid id)
        {
            _service.Get(id);
            var currentUsername = Request.Headers.GetCookies("Login").FirstOrDefault()?.Cookies.FirstOrDefault(x => x.Name == "Login")?.Values.Get("USER_NAME");
            if (string.IsNullOrEmpty(currentUsername))
            {
                return Ok(new
                {
                    message = "Unauthorized",
                    messageCode = "401",
                    status = "error"
                });
            }
            if (_service.ObjDetail.ID != Guid.Empty)
            {
                _service.Delete(id, currentUsername);
            }
            else
            {
                return Ok(new
                {
                    status = "ok"
                });
            }

            if (_service.State)
            {
                return Ok(new
                {
                    action = "deleted",
                    status = "success",
                    executeMethod = "BuildTree"
                });
            }
            else
            {
                return Ok(new
                {
                    message = _service.ErrorMessage + "(hoặc đã phát sinh dữ liệu thực tế)",
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
