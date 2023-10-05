using NHibernate.Linq;
using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace SMO.Repository.Implement.PS
{
    public class ProjectRepo : PSRepository<T_PS_PROJECT>, IProjectRepo
    {
        public ProjectRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public IList<T_PS_PROJECT> Search(T_PS_PROJECT objFilter, int pageSize, int pageIndex, out int total, string userName, bool isViewFull)
        {
            var query = Queryable();

            if (objFilter.ID != Guid.Empty)
            {
                query = query.Where(x => x.ID == objFilter.ID);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.Contains(objFilter.NAME) || x.CODE.Contains(objFilter.NAME));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.TYPE))
            {
                query = query.Where(x => x.TYPE == objFilter.TYPE);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.STATUS))
            {
                query = query.Where(x => x.STATUS == objFilter.STATUS);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.PROJECT_LEVEL_CODE))
            {
                query = query.Where(x => x.PROJECT_LEVEL_CODE == objFilter.PROJECT_LEVEL_CODE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.DON_VI))
            {
                query = query.Where(x => x.DON_VI == objFilter.DON_VI);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.PHONG_BAN))
            {
                query = query.Where(x => x.PHONG_BAN == objFilter.PHONG_BAN);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.CUSTOMER_CODE))
            {
                query = query.Where(x => x.CUSTOMER_CODE == objFilter.CUSTOMER_CODE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.GIAM_DOC_DU_AN))
            {
                query = query.Where(x => x.GIAM_DOC_DU_AN == objFilter.GIAM_DOC_DU_AN);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.QUAN_TRI_DU_AN))
            {
                query = query.Where(x => x.QUAN_TRI_DU_AN == objFilter.QUAN_TRI_DU_AN);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.PHU_TRACH_CUNG_UNG))
            {
                query = query.Where(x => x.PHU_TRACH_CUNG_UNG == objFilter.PHU_TRACH_CUNG_UNG);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.QUAN_LY_HOP_DONG))
            {
                query = query.Where(x => x.QUAN_LY_HOP_DONG == objFilter.QUAN_LY_HOP_DONG);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.PROJECT_OWNER))
            {
                query = query.Where(x => x.PROJECT_OWNER == objFilter.PROJECT_OWNER);
            }

            if (!isViewFull)
            {
                var lstProjectOfUser = NHibernateSession.Query<T_PS_RESOURCE>().Where(x => x.USER_NAME == userName).Select(x => x.PROJECT_ID).ToList();
                query = query.Where(x => lstProjectOfUser.Contains(x.ID));
            }

            query = query.OrderByDescending(x => x.CREATE_DATE);

            var rowCount = query.ToFutureValue(x => x.Count());

            List<T_PS_PROJECT> lstProject = query.ToList();
            total = rowCount.Value;
            return lstProject;
            //return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }

        public void ResetStatus(Guid projectId, string updateBy, string modul)
        {
            var project = NHibernateSession.Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == projectId);
            if (project.STATUS == "03")
            {
                NHibernateSession.Save(new T_PS_PROGRESS_HISTORY
                {
                    ID = Guid.NewGuid(),
                    ACTOR = updateBy,
                    CREATE_BY = updateBy,
                    PROJECT_ID = projectId,
                    ACTION = "06",
                    PRE_STATUS = "",
                    DES_STATUS = "",
                    TAB_NAME = modul
                });
            }
            if (project.STATUS == "03" && project.STATUS_STRUCT_PLAN == "05")
            {
                Queryable().Where(x => x.ID == projectId)
                        .Update(x => new T_PS_PROJECT
                        {
                            STATUS_STRUCT_PLAN = "01",
                            UPDATE_BY = updateBy
                        });
                //// Tạo email
                //List<string> lstEmail = new List<string>();
                //var lstProjectUser = NHibernateSession.Query<T_PS_RESOURCE>().Where(x => x.PROJECT_ID == project.ID).ToList();
                //var lstUserApprove = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("PM")).ToList();
                //if (lstUserApprove.Count == 0)
                //{
                //    return;
                //}
                //string LOAI_CONG_VIEC = "{LOAI_CONG_VIEC}";
                //string MA = "{MA}";
                //string TEN = "{TEN}";
                //string Y_KIEN = "{Y_KIEN}";
                //string USER_NAME_NGUOI_DE_XUAT = "{USER_NAME_NGUOI_DE_XUAT}";
                //string HO_TEN_NGUOI_DE_XUAT = "{HO_TEN_NGUOI_DE_XUAT}";
                //string USER_NAME_NGUOI_PHE_DUYET = "{USER_NAME_NGUOI_PHE_DUYET}";
                //string HO_TEN_NGUOI_PHE_DUYET = "{HO_TEN_NGUOI_PHE_DUYET}";
                //string DON_VI_PHONG_BAN = "{DON_VI_PHONG_BAN}";
                //string TRANG_THAI = "{TRANG_THAI}";
                //string LINK_CHI_TIET = "{LINK_CHI_TIET}";
                //string VAI_TRO_TAI_DU_AN = "{VAI_TRO_TAI_DU_AN}";

                //var template = NHibernateSession.Query<T_CF_TEMPLATE_NOTIFY>().FirstOrDefault();
                //var url = ConfigurationManager.AppSettings["URL_WEB"];

                //var subjectTemplate = template.CONG_VIEC_HOAN_THANH_SUBJECT;
                //var bodyTemplate = template.CONG_VIEC_HOAN_THANH_BODY;

                //if (string.IsNullOrWhiteSpace(subjectTemplate) || string.IsNullOrWhiteSpace(bodyTemplate))
                //{
                //    return;
                //}

                //var contentSubject = subjectTemplate
                //        .Replace(LOAI_CONG_VIEC, "Dự án")
                //        .Replace(MA, project.CODE)
                //        .Replace(TEN, project.NAME)
                //        .Replace(TRANG_THAI, "Lập kế hoạch")
                //        .Replace(LINK_CHI_TIET, url + "Home/OpenProject?id=" + project.ID)
                //        .Replace(VAI_TRO_TAI_DU_AN, "")
                //        .Replace(Y_KIEN, "Phê duyệt lại dự án!")
                //        .Replace(USER_NAME_NGUOI_DE_XUAT, "")
                //        .Replace(USER_NAME_NGUOI_PHE_DUYET, string.Join(", ", lstUserApprove.Select(x => x.USER_NAME).ToList()))
                //        .Replace(HO_TEN_NGUOI_DE_XUAT, "")
                //        .Replace(DON_VI_PHONG_BAN, "")
                //        .Replace(HO_TEN_NGUOI_PHE_DUYET, "");

                //var contentBody = bodyTemplate
                //        .Replace(LOAI_CONG_VIEC, "Dự án")
                //        .Replace(MA, project.CODE)
                //        .Replace(TEN, project.NAME)
                //        .Replace(TRANG_THAI, "Lập kế hoạch")
                //        .Replace(LINK_CHI_TIET, url + "Home/OpenProject?id=" + project.ID)
                //        .Replace(VAI_TRO_TAI_DU_AN, "")
                //        .Replace(Y_KIEN, "Phê duyệt lại dự án!")
                //        .Replace(USER_NAME_NGUOI_DE_XUAT, "")
                //        .Replace(USER_NAME_NGUOI_PHE_DUYET, string.Join(", ", lstUserApprove.Select(x => x.USER_NAME).ToList()))
                //        .Replace(HO_TEN_NGUOI_DE_XUAT, "")
                //        .Replace(DON_VI_PHONG_BAN, "")
                //        .Replace(HO_TEN_NGUOI_PHE_DUYET, "");

                //foreach (var userApprove in lstUserApprove)
                //{
                //    var findUser = NHibernateSession.Query<T_AD_USER>().FirstOrDefault(x => x.USER_NAME == userApprove.USER_NAME);
                //    NHibernateSession.Save(new T_CM_EMAIL()
                //    {
                //        PKID = Guid.NewGuid().ToString(),
                //        EMAIL = findUser.EMAIL,
                //        SUBJECT = contentSubject,
                //        CONTENTS = contentBody
                //    });
                //}


                //// Tạo notify
                //var lstUserNotify = new List<string>();
                //lstUserNotify.AddRange(lstUserApprove.Select(x => x.USER_NAME).ToList());

                //foreach (var user in lstUserNotify)
                //{
                //    var newId = Guid.NewGuid().ToString();

                //    string strTemplate = @"
                //            <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                //                <div class='icon-circle {2}'>
                //                    <i class='material-icons'>{3}</i>
                //                </div>
                //                <div class='menu-info'>
                //                    <span>Dự án [{4} - {5}]  đang chờ phê duyệt!</span>
                //                    <p>
                //                        <i class='material-icons'>access_time</i> {6}
                //                    </p>
                //                </div>
                //            </a>
                //        ";

                //    string strContent = string.Format(strTemplate,
                //        newId,
                //        $"/PS/Project/Edit?id={project.ID}",
                //        "",
                //        "",
                //        project.CODE,
                //        project.NAME,
                //        DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                //    string strRawContent = $"Dự án {project.CODE} đang chờ phê duyệt!";

                //    NHibernateSession.Save(new T_CM_NOTIFY()
                //    {
                //        PKID = newId,
                //        CONTENTS = strContent,
                //        RAW_CONTENTS = strRawContent,
                //        USER_NAME = user
                //    });
                //}

                //try
                //{
                //    var link = url + "Home/SendNotify?lstUserName=" + string.Join(",", lstUserNotify);
                //    var client = new WebClient();
                //    client.DownloadString(link);
                //}
                //catch(Exception ex) {
                //    var test = ex;
                //}
                //var hasConfirmHistory = NHibernateSession.Query<T_PS_PROGRESS_HISTORY>().Any(x => x.PROJECT_ID == projectId && x.DES_STATUS == "05");
                //if (hasConfirmHistory)
                //{

                //}
            }
        }
    }
}
