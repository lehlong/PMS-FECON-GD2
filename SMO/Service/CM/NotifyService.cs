using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using System;
using System.Linq;
using NHibernate.Linq;

namespace SMO.Service.CM
{
    public class NotifyService : GenericService<T_CM_NOTIFY, NotifyRepo>
    {
        public int IntCountNew { get; set; }
        public NotifyService() : base()
        {

        }

        public void UpdateNotifyIsViewed(string userName)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                var strSQL = "UPDATE T_CM_NOTIFY SET IS_COUNTED = 'Y' WHERE USER_NAME = :USER_NAME";
                UnitOfWork.GetSession().CreateSQLQuery(strSQL)
                    .SetParameter("USER_NAME", userName)
                    .ExecuteUpdate();

                UnitOfWork.Commit();
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                UnitOfWork.Rollback();
                this.State = false;
            }
        }

        public void UpdateNotifyIsReaded(string pkId)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                var strSQL = "UPDATE T_CM_NOTIFY SET IS_REAED = 'Y', IS_COUNTED = 'Y' WHERE PKID = :PKID";
                UnitOfWork.GetSession().CreateSQLQuery(strSQL)
                    .SetParameter("PKID", pkId)
                    .ExecuteUpdate();

                UnitOfWork.Commit();
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                UnitOfWork.Rollback();
                this.State = false;
            }
        }

        public void GetNotifyOfUser(string userName)
        {
            var query = UnitOfWork.GetSession().Query<T_CM_NOTIFY>();
            this.ObjList = query.Where(x => x.USER_NAME == userName).OrderByDescending(x => x.CREATE_DATE).Take(10).Skip(0).ToList();
            this.IntCountNew = query.Where(x => x.USER_NAME == userName && !x.IS_COUNTED).Count();
        }
    }
}
