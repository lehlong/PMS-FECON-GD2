using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using System;

namespace SMO.Service.CM
{
    public class CommentService : GenericService<T_CM_COMMENT, CommentRepo>
    {
        public CommentService() : base()
        {

        }

        public void GetComments()
        {
            this.ObjList = UnitOfWork.Repository<CommentRepo>().GetCommentsOfDocument(this.ObjDetail.REFRENCE_ID);
        }

        public override void Create()
        {
            this.ObjDetail.CODE = Guid.NewGuid().ToString();
            this.ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
            base.Create();
            if (!this.State)
            {
                return;
            }
        }
    }
}
