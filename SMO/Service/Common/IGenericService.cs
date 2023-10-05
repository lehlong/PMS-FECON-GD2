using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service
{
    public interface IGenericService<T>
    {
        void Create();
        void Update();
        void Delete(List<object> lstId);
        void Delete(string strLstSelected);
        void Get(object id, dynamic param = null);
        void GetAll();
        void Search();
        bool CheckExist(Func<T, bool> predicate);
        void ToggleActive(object id);
        bool GetState();
    }
}
