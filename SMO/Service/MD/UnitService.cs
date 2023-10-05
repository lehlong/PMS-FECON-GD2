using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using System;
using System.IO;

namespace SMO.Service.MD
{
    public class UnitService : GenericService<T_MD_UNIT, UnitRepo>
    {
        public UnitService() : base()
        {

        }

        public void ExportExcel(ref MemoryStream outFileStream, string path)
        {
            try
            {
                this.GetAll();
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);
                
                var startRow = 6;
                foreach (var item in this.ObjList)
                {
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow++, 5);
                    rowCur.Cells[0].SetCellValue(item.CODE);
                    rowCur.Cells[1].SetCellValue(item.NAME);
                    rowCur.Cells[2].SetCellValue(item.SKF);
                }
                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }
        public override void Create()
        {
            try
            {
                this.ObjDetail.ACTIVE = true;
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE))
                {
                    base.Create();
                }
                else
                {
                    this.State = false;
                    this.MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
