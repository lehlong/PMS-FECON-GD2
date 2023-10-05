using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class OrganizeService : GenericService<T_AD_ORGANIZE, OrganizeRepo>
    {
        public OrganizeService() : base()
        {

        }

        public List<NodeUser> BuildTreeOrg()
        {
            var lstNode = new List<NodeUser>();
            var serviceOrganize = new OrganizeService();
            serviceOrganize.GetAll();
            foreach (var organize in serviceOrganize.ObjList.Where(x => x.TYPE == "CP").OrderBy(x => x.C_ORDER).ToList())
            {
                var nodeOrganize = new NodeUser()
                {
                    id = organize.PKID,
                    pId = organize.PARENT,
                    name = organize.NAME,
                    companyCode = organize.COMPANY_CODE,
                    icon = "/Content/zTreeStyle/img/diy/donvi.gif",
                };
                lstNode.Add(nodeOrganize);
            }
            return lstNode;
        }

        public List<NodeOrganize> BuildTree()
        {
            var lstNode = new List<NodeOrganize>();
            this.GetAll();
            foreach (var item in this.ObjList.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeOrganize()
                {
                    id = item.PKID,
                    pId = item.PARENT,
                    name = item.NAME
                };
                lstNode.Add(node);
            }
            return lstNode;
        }

        public void UpdateItem()
        {
            if (this.ObjList.Count(x => string.IsNullOrEmpty(x.NAME)) > 0)
            {
                this.State = false;
                this.ErrorMessage = "Không được bỏ trống tên đơn vị!";
                return;
            }
            try
            {
                UnitOfWork.BeginTransaction();
                foreach (var item in this.ObjList)
                {
                    item.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    this.CurrentRepository.Update(item);
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

        public override void Create()
        {
            this.ObjDetail.PKID = Guid.NewGuid().ToString();
            base.Create();
        }

        public void UpdateTree(List<NodeOrganize> lstNode)
        {
            try
            {
                var strSql = "";
                var order = 0;
                UnitOfWork.BeginTransaction();

                foreach (var item in lstNode)
                {
                    if (!string.IsNullOrWhiteSpace(item.pId))
                    {
                        strSql = "UPDATE T_AD_ORGANIZE SET C_ORDER = :C_ORDER, PARENT = :PARENT WHERE PKID = :PKID";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("PARENT", item.pId)
                            .SetParameter("PKID", item.id)
                            .ExecuteUpdate();
                    }
                    else
                    {
                        strSql = "UPDATE T_AD_ORGANIZE SET C_ORDER = :C_ORDER, PARENT = '' WHERE PKID = :PKID";
                        UnitOfWork.GetSession().CreateSQLQuery(strSql)
                            .SetParameter("C_ORDER", order)
                            .SetParameter("PKID", item.id)
                            .ExecuteUpdate();
                    }
                    order++;
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

        public override void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                foreach (var item in lstId)
                {
                    UnitOfWork.BeginTransaction();
                    if (!this.CheckExist(x => x.PARENT == item))
                    {
                        this.CurrentRepository.Delete(item);
                    }
                    else
                    {
                        this.State = false;
                        this.ErrorMessage = "Đơn vị này đang là cha của đơn vị khác.";
                        UnitOfWork.Rollback();
                        return;
                    }
                    UnitOfWork.Commit();
                }
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
