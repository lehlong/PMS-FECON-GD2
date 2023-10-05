using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_MA_TRA_CUU_Map : BaseMapping<T_CM_MA_TRA_CUU>
    {
        public T_CM_MA_TRA_CUU_Map()
        {
            Table("T_CM_MA_TRA_CUU");
            Id(x => x.MA_TRA_CUU);
        }
    }
}
