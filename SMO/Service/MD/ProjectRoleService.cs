using NHibernate.Linq;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.MD
{
    public class ProjectRoleService : GenericService<T_MD_PROJECT_ROLE, ProjectRoleRepo>
    {
        public ProjectRoleService() : base()
        {

        }

        public override void Create()
        {
            try
            {
                this.ObjDetail.ACTIVE = true;
                if (!this.CheckExist(x => x.ID == this.ObjDetail.ID))
                {
                    base.Create();
                }
                else
                {
                    this.State = false;
                    this.MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public List<NodeRight> BuildTreeRight(string projectRoleId)
        {
            var lstNode = new List<NodeRight>();

            var lstAllRight = UnitOfWork.GetSession().Query<T_AD_PROJECT_RIGHT>().OrderBy(x => x.C_ORDER).ToList();
            lstAllRight.Add(new T_AD_PROJECT_RIGHT() { CODE = "R00", NAME = "Tất cả quyền trong dự án" });
            var lstRightOfRole = UnitOfWork.Repository<ProjectRoleRightRepo>().Queryable().Where(x => x.PROJECT_ROLE_ID == projectRoleId).ToList();

            foreach (var item in lstAllRight)
            {
                var node = new NodeRight()
                {
                    id = item.CODE,
                    pId = item.PARENT,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.NAME
                };
                if (lstRightOfRole.Count(x => x.RIGHT_CODE == item.CODE) > 0)
                    node.@checked = "true";

                if (string.IsNullOrWhiteSpace(item.PARENT))
                {
                    node.pId = "R00";
                }
                lstNode.Add(node);
            }
            return lstNode;
        }

        public void UpdateRight(string projectRoleId, string rightList)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                UnitOfWork.GetSession().Query<T_MD_PROJECT_ROLE_RIGHT>().Where(x => x.PROJECT_ROLE_ID == projectRoleId).Delete();

                var lstRight = rightList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in lstRight)
                {
                    UnitOfWork.GetSession().Save(new T_MD_PROJECT_ROLE_RIGHT()
                    {
                        ID = Guid.NewGuid().ToString(),
                        PROJECT_ROLE_ID = projectRoleId,
                        RIGHT_CODE = item,
                        CREATE_BY = ProfileUtilities.User.USER_NAME
                    });
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
