using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PROJECT_STRUCT_SAP_Map : BaseMapping<T_PS_PROJECT_STRUCT_SAP>
    {
        public T_PS_PROJECT_STRUCT_SAP_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.PROJECT_ID);
            Map(x => x.PROJECT_STRUCT_ID);
            Map(x => x.ACTION);
        }
    }
}
