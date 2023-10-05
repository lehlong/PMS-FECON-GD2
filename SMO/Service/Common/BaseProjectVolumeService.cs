using SMO.Core.Common;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;
using SMO.Service.AD;
using SMO.Service.CF;
using SMO.Service.CM;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMO.Service.Common
{
    public class BaseProjectVolumeService<TEntity, TRepo> : GenericService<TEntity, TRepo>
        where TEntity : BaseProjectVolumeEntity
        where TRepo : GenericRepository<TEntity>
    {

        internal T_MD_VENDOR GetVendor(string vendorCode)
        {
            return UnitOfWork.Repository<VendorRepo>().Get(vendorCode);
        }
        internal T_MD_CUSTOMER GetCustomer(Guid projectId)
        {
            var project = UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == projectId);
            return UnitOfWork.Repository<CustomerRepo>().Get(project.CUSTOMER_CODE);
        }
        internal IList<T_PS_TIME> GetProjectTime()
        {
            return UnitOfWork.Repository<ProjectTimeRepo>().Queryable()
                        .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID)
                        .OrderBy(x => x.C_ORDER)
                        .ToList();
        }

        internal void SynToSAP()
        {

        }

        public virtual ProjectWorkVolumeStatus UpdateStatus(UpdateStatusVolumeModel model, bool isAccept)
        {
            try
            {
                var currentObj = CurrentRepository.Get(model.Id);
                UnitOfWork.Clear();

                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var statusFromAction = GetStatusFromAction(model.Action.GetEnum<ProjectWorkVolumeAction>());

                var previousStatus = currentObj.STATUS;
                currentObj.STATUS = statusFromAction.GetValue();
                currentObj.SAP_DOCID = model.SAP_DOCID;
                if (currentObj.STATUS == ProjectWorkVolumeStatus.XAC_NHAN.GetValue() || currentObj.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue())
                {
                    currentObj.USER_XAC_NHAN = currentUser;
                }
                if (currentObj.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue() || currentObj.STATUS == ProjectWorkVolumeStatus.TU_CHOI.GetValue())
                {
                    currentObj.USER_PHE_DUYET = currentUser;
                    if (currentObj.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                    {
                        currentObj.SAP_DOCID = model.SAP_DOCID;
                    }
                }
                if (currentObj.STATUS == ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue())
                {
                    currentObj.USER_XAC_NHAN = "";
                    currentObj.USER_PHE_DUYET = "";
                }

                UnitOfWork.Repository<VolumeProgressHistoryRepo>()
                    .Create(new T_PS_VOLUME_PROGRESS_HISTORY
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = currentObj.PROJECT_ID,
                        ACTOR = currentUser,
                        CREATE_BY = currentUser,
                        ACTION = model.Action,
                        DES_STATUS = statusFromAction.GetValue(),
                        PRE_STATUS = previousStatus,
                        NOTE = model.Note,
                        RESOURCE_ID = model.Id,
                        IS_CUSTOMER = currentObj.IS_CUSTOMER,
                        IS_ACCEPT = isAccept
                    });

                CurrentRepository.Update(currentObj);
                UnitOfWork.Commit();
                this.CreateNotiEmail(currentObj, model);
                //this.CreateNotify(currentObj, model);
                return statusFromAction;
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
                return ProjectWorkVolumeStatus.CHO_XAC_NHAN;
            }
        }
        internal ProjectWorkVolumeStatus GetStatusFromAction(ProjectWorkVolumeAction action)
        {
            switch (action)
            {
                case ProjectWorkVolumeAction.TAO_MOI:
                    return ProjectWorkVolumeStatus.KHOI_TAO;
                case ProjectWorkVolumeAction.GUI:
                    return ProjectWorkVolumeStatus.CHO_XAC_NHAN;
                case ProjectWorkVolumeAction.KHONG_XAC_NHAN:
                    return ProjectWorkVolumeStatus.KHONG_XAC_NHAN;
                case ProjectWorkVolumeAction.XAC_NHAN:
                    return ProjectWorkVolumeStatus.XAC_NHAN;
                case ProjectWorkVolumeAction.PHE_DUYET:
                    return ProjectWorkVolumeStatus.PHE_DUYET;
                case ProjectWorkVolumeAction.TU_CHOI:
                    return ProjectWorkVolumeStatus.TU_CHOI;
                case ProjectWorkVolumeAction.HUY_PHE_DUYET:
                    return ProjectWorkVolumeStatus.XAC_NHAN;
                default:
                    return ProjectWorkVolumeStatus.KHOI_TAO;
            }
        }

        public void CreateNotiEmail(TEntity currentObj, UpdateStatusVolumeModel model)
        {
            try
            {
                List<string> lstEmail = new List<string>();
                var lstProjectUser = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == currentObj.PROJECT_ID).ToList();
                var lstUserApprove = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("PM")).ToList();
                var lstUserTNQS = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("TNQS")).ToList();
                var lstUserSM = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("SM")).ToList();

                
                //var lstCodeVaiTro = currentUser.PROJECT_ROLE_ID.Split(',').ToList();
                //var lstVaiTro = UnitOfWork.GetSession().Query<T_MD_PROJECT_ROLE>().Where(x => lstCodeVaiTro.Contains(x.ID)).Select(x => x.NAME).ToList();
                var project = UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == currentObj.PROJECT_ID);

                string LOAI_CONG_VIEC = "{LOAI_CONG_VIEC}";
                string MA = "{MA}";
                string TEN = "{TEN}";
                string Y_KIEN = "{Y_KIEN}";
                string USER_NAME_NGUOI_DE_XUAT = "{USER_NAME_NGUOI_DE_XUAT}";
                string HO_TEN_NGUOI_DE_XUAT = "{HO_TEN_NGUOI_DE_XUAT}";
                string USER_NAME_NGUOI_PHE_DUYET = "{USER_NAME_NGUOI_PHE_DUYET}";
                string HO_TEN_NGUOI_PHE_DUYET = "{HO_TEN_NGUOI_PHE_DUYET}";
                string TRANG_THAI = "{TRANG_THAI}";
                string LINK_CHI_TIET = "{LINK_CHI_TIET}";
                string VAI_TRO_TAI_DU_AN = "{VAI_TRO_TAI_DU_AN}";

                var serviceTemplate = new ConfigTemplateNotifyService();
                serviceTemplate.GetAll();
                if (serviceTemplate.ObjList.Count() == 0)
                {
                    return;
                }

                var loaiCV = string.Empty;
                var urlDetail = string.Empty;
                if (typeof(TEntity) == typeof(T_PS_VOLUME_WORK))
                {
                    urlDetail = "/Home/OpenVolumeWork?id=" + currentObj.ID + "&partnerCode=" + currentObj.VENDOR_CODE + "&projectId=" + currentObj.PROJECT_ID + "&isCustomer=" + currentObj.IS_CUSTOMER;
                    if (currentObj.IS_CUSTOMER)
                    {
                        loaiCV = "Khối lượng thực hiện khách hàng";
                    }
                    else
                    {
                        loaiCV = "Khối lượng thực hiện thầu phụ";
                    }
                }
                else if (typeof(TEntity) == typeof(T_PS_VOLUME_ACCEPT))
                {
                    urlDetail = "/Home/OpenVolumeAccept?id=" + currentObj.ID + "&partnerCode=" + currentObj.VENDOR_CODE + "&projectId=" + currentObj.PROJECT_ID + "&isCustomer=" + currentObj.IS_CUSTOMER;
                    if (currentObj.IS_CUSTOMER)
                    {
                        loaiCV = "Khối lượng nghiệm thu khách hàng";
                    }
                    else
                    {
                        loaiCV = "Khối lượng nghiệm thu thầu phụ";
                    }
                }

                var template = serviceTemplate.ObjList.FirstOrDefault();

                var serviceEmail = new EmailNotifyService();
                var url = HttpContext.Current.Request.Url.Host;

                var contentSubjectXuLy = template.CONG_VIEC_XU_LY_SUBJECT
                        .Replace(LOAI_CONG_VIEC, loaiCV)
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME);


                var contentBodyXuLy = template.CONG_VIEC_XU_LY_BODY
                        .Replace(LOAI_CONG_VIEC, loaiCV)
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME)
                        .Replace(TRANG_THAI, currentObj.STATUS.GetEnum<ProjectWorkVolumeStatus>().GetName())
                        .Replace(LINK_CHI_TIET, url + urlDetail)
                        .Replace(Y_KIEN, model.Note)
                        .Replace(HO_TEN_NGUOI_DE_XUAT, ProfileUtilities.User.FULL_NAME);


                var contentSubjectHoanThanh = template.CONG_VIEC_HOAN_THANH_SUBJECT
                        .Replace(LOAI_CONG_VIEC, loaiCV)
                        .Replace(TRANG_THAI, currentObj.STATUS.GetEnum<ProjectWorkVolumeStatus>().GetName())
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME);


                var contentBodyHoanThanh = template.CONG_VIEC_HOAN_THANH_BODY
                        .Replace(LOAI_CONG_VIEC, loaiCV)
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME)
                        .Replace(TRANG_THAI, currentObj.STATUS.GetEnum<ProjectWorkVolumeStatus>().GetName())
                        .Replace(LINK_CHI_TIET, url + urlDetail)
                        .Replace(Y_KIEN, model.Note)
                        .Replace(HO_TEN_NGUOI_PHE_DUYET, ProfileUtilities.User.FULL_NAME);


                UnitOfWork.BeginTransaction();

                if (currentObj.STATUS == ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue()){
                    foreach (var user in lstUserTNQS)
                    {
                        UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            EMAIL = user.User.EMAIL,
                            SUBJECT = contentSubjectXuLy,
                            CONTENTS = contentBodyXuLy.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                        });
                    }

                    foreach (var user in lstUserSM)
                    {
                        UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            EMAIL = user.User.EMAIL,
                            SUBJECT = contentSubjectXuLy,
                            CONTENTS = contentBodyXuLy.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                        });
                    }
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.XAC_NHAN.GetValue())
                {
                    foreach (var user in lstUserApprove)
                    {
                        UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            EMAIL = user.User.EMAIL,
                            SUBJECT = contentSubjectXuLy,
                            CONTENTS = contentBodyXuLy.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                        });
                    }
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue())
                {
                    UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        EMAIL = currentObj.USER_CREATE.EMAIL,
                        SUBJECT = contentSubjectHoanThanh,
                        CONTENTS = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, "")
                    });
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                {
                    UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        EMAIL = currentObj.USER_CREATE.EMAIL,
                        SUBJECT = contentSubjectHoanThanh,
                        CONTENTS = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, "")
                    });

                    foreach (var user in lstUserTNQS)
                    {
                        UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            EMAIL = user.User.EMAIL,
                            SUBJECT = contentSubjectHoanThanh,
                            CONTENTS = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                        });
                    }

                    foreach (var user in lstUserSM)
                    {
                        UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            EMAIL = user.User.EMAIL,
                            SUBJECT = contentSubjectHoanThanh,
                            CONTENTS = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                        });
                    }
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.TU_CHOI.GetValue())
                {
                    UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        EMAIL = currentObj.USER_CREATE.EMAIL,
                        SUBJECT = contentSubjectHoanThanh,
                        CONTENTS = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, "")
                    });

                    foreach (var user in lstUserTNQS)
                    {
                        UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            EMAIL = user.User.EMAIL,
                            SUBJECT = contentSubjectHoanThanh,
                            CONTENTS = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                        });
                    }

                    foreach (var user in lstUserSM)
                    {
                        UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            EMAIL = user.User.EMAIL,
                            SUBJECT = contentSubjectHoanThanh,
                            CONTENTS = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                        });
                    }
                }


                UnitOfWork.Commit();
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();
            }
        }

        public void CreateNotify(TEntity currentObj, UpdateStatusVolumeModel model)
        {
            try
            {
                var lstProjectUser = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == currentObj.PROJECT_ID).ToList();
                var lstUserApprove = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("PM")).ToList();
                var lstUserTNQS = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("TNQS")).ToList();
                var lstUserSM = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("SM")).ToList();

                var project = UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == currentObj.PROJECT_ID);

                var loaiCV = string.Empty;
                var urlDetail = string.Empty;
                if (typeof(TEntity) == typeof(T_PS_VOLUME_WORK))
                {
                    urlDetail = $"/PS/VolumeWork/IndexVolumeWork/{currentObj.ID}?projectId={currentObj.PROJECT_ID}&isCustomer={currentObj.IS_CUSTOMER}&partnerCode={currentObj.VENDOR_CODE}";
                    if (currentObj.IS_CUSTOMER)
                    {
                        loaiCV = "Khối lượng thực hiện khách hàng";
                    }
                    else
                    {
                        loaiCV = "Khối lượng thực hiện thầu phụ";
                    }
                }
                else if (typeof(TEntity) == typeof(T_PS_VOLUME_ACCEPT))
                {
                    urlDetail = $"/PS/VolumeAccept/IndexAcceptVolume/{currentObj.ID}?projectId={currentObj.PROJECT_ID}&isCustomer={currentObj.IS_CUSTOMER}&partnerCode={currentObj.VENDOR_CODE}";
                    if (currentObj.IS_CUSTOMER)
                    {
                        loaiCV = "Khối lượng nghiệm thu khách hàng";
                    }
                    else
                    {
                        loaiCV = "Khối lượng nghiệm thu thầu phụ";
                    }
                }

                string strTemplate = @"
                            <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                                <div class='icon-circle {2}'>
                                    <i class='material-icons'>{3}</i>
                                </div>
                                <div class='menu-info'>
                                    <span>{4} [{5} - {6}] {}!</span>
                                    <p>
                                        <i class='material-icons'>access_time</i> {7}
                                    </p>
                                </div>
                            </a>
                        ";

                var lstUserNotify = new List<string>();

                UnitOfWork.BeginTransaction();

                if (currentObj.STATUS == ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue())
                {
                    lstUserNotify.AddRange(lstUserTNQS.Select(x => x.USER_NAME).ToList());
                    
                    foreach (var user in lstUserTNQS)
                    {
                        var newId = Guid.NewGuid().ToString();
                        string strContent = string.Format(strTemplate,
                                newId,
                                urlDetail,
                                loaiCV,
                                project.CODE,
                                project.NAME,
                                "chờ xác nhận",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        string strRawContent = $"{loaiCV} Dự án {project.CODE} đang chờ xác nhận!";

                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }


                    lstUserNotify.AddRange(lstUserSM.Select(x => x.USER_NAME).ToList());
                    foreach (var user in lstUserSM)
                    {
                        var newId = Guid.NewGuid().ToString();
                        string strContent = string.Format(strTemplate,
                                newId,
                                urlDetail,
                                loaiCV,
                                project.CODE,
                                project.NAME,
                                "chờ xác nhận",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        string strRawContent = $"{loaiCV} Dự án {project.CODE} đang chờ xác nhận!";

                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.XAC_NHAN.GetValue())
                {
                    lstUserNotify.AddRange(lstUserApprove.Select(x => x.USER_NAME).ToList());

                    foreach (var user in lstUserApprove)
                    {
                        var newId = Guid.NewGuid().ToString();
                        string strContent = string.Format(strTemplate,
                                newId,
                                urlDetail,
                                loaiCV,
                                project.CODE,
                                project.NAME,
                                "chờ phê duyệt",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        string strRawContent = $"{loaiCV} Dự án {project.CODE} đang chờ phê duyệt!";

                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue())
                {
                    lstUserNotify.Add(currentObj.CREATE_BY);

                    var newId = Guid.NewGuid().ToString();
                    string strContent = string.Format(strTemplate,
                            newId,
                            urlDetail,
                            loaiCV,
                            project.CODE,
                            project.NAME,
                            "không được xác nhận",
                            DateTime.Now.ToString(Global.DateTimeToStringFormat));

                    string strRawContent = $"{loaiCV} Dự án {project.CODE} không được xác nhận!";

                    UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        RAW_CONTENTS = strRawContent,
                        USER_NAME = currentObj.CREATE_BY
                    });
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                {
                    lstUserNotify.Add(currentObj.CREATE_BY);

                    var newId = Guid.NewGuid().ToString();
                    string strContent = string.Format(strTemplate,
                            newId,
                            urlDetail,
                            loaiCV,
                            project.CODE,
                            project.NAME,
                            "đã được phê duyệt",
                            DateTime.Now.ToString(Global.DateTimeToStringFormat));

                    string strRawContent = $"{loaiCV} Dự án {project.CODE} đã được phê duyệt!";

                    UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        RAW_CONTENTS = strRawContent,
                        USER_NAME = currentObj.CREATE_BY
                    });

                    lstUserNotify.AddRange(lstUserTNQS.Select(x => x.USER_NAME).ToList());

                    foreach (var user in lstUserTNQS)
                    {
                        newId = Guid.NewGuid().ToString();
                        strContent = string.Format(strTemplate,
                                newId,
                                urlDetail,
                                loaiCV,
                                project.CODE,
                                project.NAME,
                                "đã được phê duyệt",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        strRawContent = $"{loaiCV} Dự án {project.CODE} đã được phê duyệt!";

                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }


                    lstUserNotify.AddRange(lstUserSM.Select(x => x.USER_NAME).ToList());
                    foreach (var user in lstUserSM)
                    {
                        newId = Guid.NewGuid().ToString();
                        strContent = string.Format(strTemplate,
                                newId,
                                urlDetail,
                                loaiCV,
                                project.CODE,
                                project.NAME,
                                "đã được phê duyệt",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        strRawContent = $"{loaiCV} Dự án {project.CODE} đã được phê duyệt!";

                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue())
                {
                    lstUserNotify.Add(currentObj.CREATE_BY);

                    var newId = Guid.NewGuid().ToString();
                    string strContent = string.Format(strTemplate,
                            newId,
                            urlDetail,
                            loaiCV,
                            project.CODE,
                            project.NAME,
                            "không được xác nhận",
                            DateTime.Now.ToString(Global.DateTimeToStringFormat));

                    string strRawContent = $"{loaiCV} Dự án {project.CODE} không được xác nhận!";

                    UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        RAW_CONTENTS = strRawContent,
                        USER_NAME = currentObj.CREATE_BY
                    });
                }

                if (currentObj.STATUS == ProjectWorkVolumeStatus.TU_CHOI.GetValue())
                {
                    lstUserNotify.Add(currentObj.CREATE_BY);

                    var newId = Guid.NewGuid().ToString();
                    string strContent = string.Format(strTemplate,
                            newId,
                            urlDetail,
                            loaiCV,
                            project.CODE,
                            project.NAME,
                            "đã bị từ chối",
                            DateTime.Now.ToString(Global.DateTimeToStringFormat));

                    string strRawContent = $"{loaiCV} Dự án {project.CODE} đã bị từ chối!";

                    UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        RAW_CONTENTS = strRawContent,
                        USER_NAME = currentObj.CREATE_BY
                    });

                    lstUserNotify.AddRange(lstUserTNQS.Select(x => x.USER_NAME).ToList());

                    foreach (var user in lstUserTNQS)
                    {
                        newId = Guid.NewGuid().ToString();
                        strContent = string.Format(strTemplate,
                                newId,
                                urlDetail,
                                loaiCV,
                                project.CODE,
                                project.NAME,
                                "đã bị từ chối",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        strRawContent = $"{loaiCV} Dự án {project.CODE} đã bị từ chối!";

                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }


                    lstUserNotify.AddRange(lstUserSM.Select(x => x.USER_NAME).ToList());
                    foreach (var user in lstUserSM)
                    {
                        newId = Guid.NewGuid().ToString();
                        strContent = string.Format(strTemplate,
                                newId,
                                urlDetail,
                                loaiCV,
                                project.CODE,
                                project.NAME,
                                "đã bị từ chối",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        strRawContent = $"{loaiCV} Dự án {project.CODE} đã bị từ chối!";

                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }
                }

                UnitOfWork.Commit();

                SMOUtilities.SendNotify(lstUserNotify);
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();
            }
        }
    }
}
