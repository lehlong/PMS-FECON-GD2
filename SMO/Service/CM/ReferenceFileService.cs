using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using System;
using NHibernate.Linq;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections.Generic;
using SMO.Service.Class;

namespace SMO.Service.CM
{
    public class ReferenceFileService : GenericService<T_CM_REFERENCE_FILE, ReferenceFileRepo>
    {
        public string ListFileDelete { get; set; }
        public List<T_CM_REFERENCE_LINK> ListLink { get; set; }
        public ReferenceFileService() : base()
        {

        }

        public void GetListFiles()
        {
            this.ObjList = this.CurrentRepository.Queryable().Where(x => x.REFERENCE_ID == ObjDetail.REFERENCE_ID).Fetch(x => x.FileUpload).ToList();
            this.ListLink = UnitOfWork.GetSession().Query<T_CM_REFERENCE_LINK>().Where(x => x.REFERENCE_ID == ObjDetail.REFERENCE_ID).ToList();
        }

        public void Update(HttpRequestBase Request, List<string> lstLink)
        {
            try
            {
                var lstFileStream = new List<FILE_STREAM>();
                for (int i = 0; i < Request.Files.AllKeys.Length; i++)
                {
                    var file = Request.Files[i];
                    var code = Guid.NewGuid();
                    lstFileStream.Add(new FILE_STREAM()
                    {
                        PKID = code,
                        FILE_OLD_NAME = file.FileName,
                        FILE_NAME = code + Path.GetExtension(file.FileName),
                        FILE_EXT = Path.GetExtension(file.FileName),
                        FILE_SIZE = file.ContentLength,
                        FILESTREAM = Request.Files[i]
                    });
                }
                FileStreamService.InsertFile(lstFileStream);

                UnitOfWork.BeginTransaction();

                if (lstLink != null)
                {
                    UnitOfWork.GetSession().Query<T_CM_REFERENCE_LINK>().Where(x => x.REFERENCE_ID == this.ObjDetail.REFERENCE_ID).Delete();
                    foreach (var link in lstLink)
                    {
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            UnitOfWork.GetSession().Save(new T_CM_REFERENCE_LINK()
                            {
                                ID = Guid.NewGuid(),
                                REFERENCE_ID = this.ObjDetail.REFERENCE_ID,
                                LINK = link,
                                CREATE_BY = ProfileUtilities.User.USER_NAME
                            });
                        }
                    }
                }

                foreach (var item in lstFileStream)
                {
                    var referenceFile = new T_CM_REFERENCE_FILE()
                    {
                        REFERENCE_ID = this.ObjDetail.REFERENCE_ID,
                        FILE_ID = item.PKID,
                        CREATE_BY = ProfileUtilities.User.USER_NAME
                    };

                    this.CurrentRepository.Create(referenceFile);

                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
                this.State = false;
            }
        }
    }
}
