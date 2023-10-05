using SMO.Service.PS.Models.Report;

using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class ConfigTemplateModel
    {
        public ConfigTemplateModel()
        {
            HeaderParams = new Dictionary<HeaderPosition, string>();
            BoldRowIndexes = new List<int>();
            PercentageRowIndexes = new List<int>();
            ColumnsNormal = new List<int>();
        }
        /// <summary>
        /// Dòng bắt đầu điền dữ liệu
        /// </summary>
        public int StartRow { get; set; }
        /// <summary>
        /// Cột bắt đầu dữ liệu là số
        /// </summary>
        public int StartColumnNumber { get; set; }
        /// <summary>
        /// Cột kết thúc dữ liệu là số
        /// </summary>
        public int? EndColumnNumber { get; set; }
        public IDictionary<HeaderPosition, string> HeaderParams { get; set; }
        public int[] ColumnsPercentage { get; set; }
        public int[] ColumnsVolume { get; set; }
        public IList<int> BoldRowIndexes { get; internal set; }
        public IList<int> PercentageRowIndexes { get; internal set; }
        public IList<int> ColumnsNormal { get; internal set; }
    }
}
