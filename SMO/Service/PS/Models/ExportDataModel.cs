using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class ExportDataModel
    {
        public string Name { get; set; }
        public IList<IEnumerable<string>> Data { get; set; }
        public IEnumerable<IEnumerable<string>> Header { get; set; }
    }
}
