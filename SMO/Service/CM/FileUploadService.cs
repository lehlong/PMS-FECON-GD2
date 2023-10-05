using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using SMO.Service.AD;
using SMO.Service.Class;
using System;
using System.Collections.Generic;

namespace SMO.Service.CM
{
    public class FileUploadService : GenericService<T_CM_FILE_UPLOAD, FileUploadRepo>
    {
        public FileUploadService() : base()
        {

        }

        public void Create(List<FILE_STREAM> lstFile)
        {
            try
            {
                var systemConfigService = new SystemConfigService();
                systemConfigService.GetConfig();

                if (systemConfigService.ObjDetail.Connection == null)
                {
                    throw new Exception("Chưa cấu hình đầy đủ thông tin lưu trữ file!");
                }

                UnitOfWork.BeginTransaction();
                foreach (var item in lstFile)
                {
                    var fileUpload = new T_CM_FILE_UPLOAD()
                    {
                        PKID = item.PKID,
                        CONNECTION_ID = systemConfigService.ObjDetail.CURRENT_CONNECTION,
                        //DATABASE_NAME = systemConfigService.ObjDetail.CURRENT_DATABASE_NAME,
                        FILE_EXT = item.FILE_EXT,
                        FILE_NAME = item.FILE_NAME,
                        FILE_OLD_NAME = item.FILE_OLD_NAME,
                        FILE_SIZE = item.FILE_SIZE,
                        DIRECTORY_PATH = item.DIRECTORY_PATH,
                        CREATE_BY = ProfileUtilities.User.USER_NAME
                    };

                    this.CurrentRepository.Create(fileUpload);

                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                throw new Exception("Lưu file vào T_CM_FILE_UPLOAD lỗi", ex);
            }
        }
    }
}
