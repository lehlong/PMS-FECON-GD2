using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class RightService : GenericService<T_AD_RIGHT, RightRepo>
    {
        public RightService() : base()
        {

        }

        public List<NodeRight> BuildTree()
        {
            var lstNode = new List<NodeRight>();
            this.GetAll();
            foreach (var item in this.ObjList.OrderBy(x => x.C_ORDER))
            {
                var node = new NodeRight()
                {
                    id = item.CODE,
                    pId = item.PARENT,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.NAME
                };
                lstNode.Add(node);
            }
            return lstNode;
        }

        public override void Create()
        {
            try
            {
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE))
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

        public void UpdateTree(List<NodeRight> lstNode)
        {
            try
            {
                var strSql = new StringBuilder();
                var order = 0;
                foreach (var item in lstNode)
                {
                    if (!string.IsNullOrWhiteSpace(item.pId))
                    {
                        strSql.AppendFormat("UPDATE T_AD_RIGHT SET C_ORDER = {0}, PARENT = N'{1}' WHERE CODE = N'{2}';", order, item.pId, item.id);
                    }
                    else
                    {
                        strSql.AppendFormat("UPDATE T_AD_RIGHT SET C_ORDER = {0} WHERE CODE = N'{1}';", order, item.id);
                    }
                    order++;
                }
                UnitOfWork.BeginTransaction();
                UnitOfWork.GetSession().CreateSQLQuery(strSql.ToString()).ExecuteUpdate();
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
                        this.ErrorMessage = "Quyền này đang là cha của các quyền khác.";
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
