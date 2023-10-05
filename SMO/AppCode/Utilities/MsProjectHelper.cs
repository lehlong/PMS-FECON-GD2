using net.sf.mpxj;
using net.sf.mpxj.MpxjUtilities;
using net.sf.mpxj.reader;

using SMO.Core.Entities.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.AppCode.Utilities
{
    public class MsProjectHelper
    {
        const string PMS_TYPE = "PMS_TYPE";
        public MsProjectHelper(string fileName)
        {
            FileName = fileName;
            DictKeys = new Dictionary<int, ValueTuple<Guid, Guid>>();
        }

        public string FileName { get; }
        public  Dictionary<int, ValueTuple<Guid, Guid>> DictKeys { get; }

        private ProjectFile ReadMsProjectFile()
        {
            ProjectReader reader = ProjectReaderUtility.getProjectReader(FileName);
            ProjectFile mpx = reader.read(FileName);

            return mpx;
        }

        internal IEnumerable<T_PS_PROJECT_STRUCT> GetTasks(Guid projectId, Guid projectStructId)
        {
            var mpx = ReadMsProjectFile();
            var currentUser = ProfileUtilities.User?.USER_NAME;
            var allTasks = mpx.Tasks.ToIEnumerable<Task>().ToList();
            foreach (var task in allTasks)
            {
                var type = task.GetFieldByAlias(PMS_TYPE)?.ToString();
                var dbId = Guid.NewGuid();
                var elementId = Guid.NewGuid();
                var parentId = task.ParentTask == null ? (Guid?)null : task.ParentTask.ID.intValue() == 0  ? projectStructId : DictKeys[task.ParentTask.ID.intValue()].Item1;
                if (DictKeys.ContainsKey(task.ID.intValue()))
                {
                    continue;
                }
                DictKeys.Add(task.ID.intValue(), ValueTuple.Create(dbId, elementId));
                var structure = new T_PS_PROJECT_STRUCT
                {
                    ID = dbId,
                    TYPE = type,
                    START_DATE = task.Start.ToDateTime(),
                    FINISH_DATE = task.Finish.ToDateTime(),
                    C_ORDER = task.ID.intValue(),
                    TEXT = task.Name,
                    ACTIVE = true,
                    CREATE_BY = currentUser,
                    PROJECT_ID = projectId,
                    PARENT_ID = parentId,
                };
                if (type == ProjectEnum.ACTIVITY.ToString())
                {
                    var activity = InitActivity(task, currentUser, projectId);
                    structure.ACTIVITY_ID = activity.ID;
                    structure.WBS_ID = activity.WBS_PARENT_ID;
                    structure.Activity = activity;
                } else if (type == ProjectEnum.WBS.ToString())
                {
                    var wbs = InitWbs(task, currentUser, projectId);
                    structure.Wbs = wbs;
                    structure.WBS_ID = wbs.ID;
                }
                else if (type == ProjectEnum.BOQ.ToString())
                {
                    var boq = InitBoq(task, currentUser, projectId);
                    structure.Boq = boq;
                    structure.BOQ_ID = boq.ID;
                }

                yield return structure;
            }
            //return new List<T_PS_PROJECT_STRUCT>();
        }

        private T_PS_BOQ InitBoq(Task task, string currentUser, Guid projectId)
        {
            return new T_PS_BOQ
            {
                ID = DictKeys[task.ID.intValue()].Item2,
                START_DATE = task.Start.ToDateTime(),
                FINISH_DATE = task.Finish.ToDateTime(),
                REFERENCE_FILE_ID = Guid.NewGuid(),
                CREATE_BY = currentUser,
                TEXT = task.Name,
                PROJECT_ID = projectId,
            };
        }

        private T_PS_WBS InitWbs(Task task, string currentUser, Guid projectId)
        {
            return new T_PS_WBS
            {
                ID = DictKeys[task.ID.intValue()].Item2,
                START_DATE = task.Start.ToDateTime(),
                FINISH_DATE = task.Finish.ToDateTime(),
                REFERENCE_FILE_ID = Guid.NewGuid(),
                CREATE_BY = currentUser,
                TEXT = task.Name,
                PROJECT_ID = projectId,
                WBS_PARENT_ID = DictKeys[task.ParentTask.ID.intValue()].Item2,
                
            };
        }

        private T_PS_ACTIVITY InitActivity(Task task, string currentUser, Guid projectId)
        {
            return new T_PS_ACTIVITY
            {
                ID = DictKeys[task.ID.intValue()].Item2,
                START_DATE = task.Start.ToDateTime(),
                FINISH_DATE = task.Finish.ToDateTime(),
                REFERENCE_FILE_ID = Guid.NewGuid(),
                CREATE_BY = currentUser,
                TEXT = task.Name,
                PROJECT_ID = projectId,
                WBS_PARENT_ID = DictKeys[task.ParentTask.ID.intValue()].Item2,
            };
        }
    }
}
