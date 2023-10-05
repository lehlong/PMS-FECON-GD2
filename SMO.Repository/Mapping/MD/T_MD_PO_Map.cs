using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PO_Map : BaseMapping<T_MD_PO>
    {
        public T_MD_PO_Map()
        {
            Table("T_MD_PO");
            Id(x => x.PKID);
            Map(x => x.PO_NUMBER);
            Map(x => x.PO_ITEM);
            Map(x => x.PO_TYPE);
            Map(x => x.PO_ORG);
            Map(x => x.PO_GROUP);
            Map(x => x.PO_DATE);
            Map(x => x.MATERIAL_CODE);
            Map(x => x.PLANT_CODE);
            Map(x => x.QUANITY);
            Map(x => x.UNIT_CODE);
            Map(x => x.PO_LOCK);
        }
    }
}
