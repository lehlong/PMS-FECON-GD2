using NHibernate.Type;
using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PROJECT_Map : BaseMapping<T_PS_PROJECT>
    {
        public T_PS_PROJECT_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.NAME);
            Map(x => x.DESCRIPTION);
            Map(x => x.CODE);
            Map(x => x.TIME_TYPE).Not.Nullable();
            Map(x => x.START_DATE).Not.Update();
            Map(x => x.FINISH_DATE);
            Map(x => x.TYPE);
            Map(x => x.PROJECT_LEVEL_CODE);
            Map(x => x.PROJECT_OWNER);
            Map(x => x.STATUS);

            Map(x => x.STATUS_CUSTOMER_PLAN_COST);
            Map(x => x.STATUS_CUSTOMER_PLAN_PROGRESS);
            Map(x => x.STATUS_CUSTOMER_PLAN_QUANTITY);
            Map(x => x.STATUS_VENDOR_PLAN_COST);
            Map(x => x.STATUS_VENDOR_PLAN_PROGRESS);
            Map(x => x.STATUS_VENDOR_PLAN_QUANTITY);
            Map(x => x.STATUS_SL_DT);
            Map(x => x.STATUS_STRUCT_BOQ);
            Map(x => x.STATUS_STRUCT_COST);
            Map(x => x.STATUS_STRUCT_PLAN);
            Map(x => x.REFERENCE_FILE_ID).Not.Update();

            Map(x => x.DON_VI);
            Map(x => x.PHONG_BAN);
            Map(x => x.DIA_DIEM);
            Map(x => x.CUSTOMER_CODE);
            Map(x => x.GIAM_DOC_DU_AN);
            Map(x => x.QUAN_TRI_DU_AN);

            Map(x => x.PHU_TRACH_CUNG_UNG);
            Map(x => x.QUAN_LY_HOP_DONG);
            Map(x => x.KHU_VUC);
            Map(x => x.NGAY_QUYET_TOAN);
            Map(x => x.HAN_BAO_HANH);

            Map(x => x.FINISH_DATE_ACTUAL);
            Map(x => x.NGAY_TRIEN_KHAI);
            Map(x => x.PLAN_METHOD);
            Map(x => x.PLANT);
            Map(x => x.SALE_ORG);
            Map(x => x.DISTRIBUTION);
            Map(x => x.FACTORY_CALENDAR);
            Map(x => x.TIME_UNIT);
            Map(x => x.PUR_GROUP);
            Map(x => x.IS_CREATE_ON_SAP).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.TOTAL_BOQ);
            Map(x => x.TOTAL_COST);
            Map(x => x.FILE_NAME);

            References(x => x.ProjectLevel).Columns("PROJECT_LEVEL_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Period).Columns("TIME_TYPE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.ProjectOwner).Columns("PROJECT_OWNER").Not.Insert().Not.Update().LazyLoad();
            References(x => x.DonVi).Columns("DON_VI").Not.Insert().Not.Update().LazyLoad();
            References(x => x.KhuVuc).Columns("KHU_VUC").Not.Insert().Not.Update().LazyLoad();
            References(x => x.PhongBan).Columns("PHONG_BAN").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Customer).Columns("CUSTOMER_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.QuanTriDuAn).Columns("QUAN_TRI_DU_AN").Not.Insert().Not.Update().LazyLoad();
            References(x => x.GiamDocDuAn).Columns("GIAM_DOC_DU_AN").Not.Insert().Not.Update().LazyLoad();
            References(x => x.PhuTrachCungUng).Columns("PHU_TRACH_CUNG_UNG").Not.Insert().Not.Update().LazyLoad();
            References(x => x.QuanLyHopDong).Columns("QUAN_LY_HOP_DONG").Not.Insert().Not.Update().LazyLoad();
            References(x => x.ProjectType).Columns("TYPE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
