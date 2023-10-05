using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public class PO_Status
    {
        public const string KhoiTao = "01";
        public const string ChoPheDuyet = "02";
        public const string TuChoi = "03";
        //public const string KinhDoanhPheDuyet = "04";
        //public const string KeToanPheDuyet = "05";
        public const string DangPheDuyet = "05";
        public const string DaPheDuyet = "06";
        public const string HuyPheDuyet = "06_2";
        public const string VaoKho = "07";
        public const string HuyVaoKho = "08";
        public const string DaTaoTichKe = "09";
        public const string DangXuatHang = "10";
        public const string DaXacNhanThucXuat = "11";
        public const string DaDongBoLenSap = "12";
        public const string RoiKho = "13";
        public const string DaChuyenNguonHangGui = "15";
        public const string TGBX_Huy = "16";
        public const string TGBX_MoiTao = "17";
        public const string TGBX_LenhTaoBangTay = "18";
        public const string DaInHoaDon = "19";
        public const string HuyDonHang = "20";
        public const string DaNhanHang = "21";
        public const string DaChuyenNhuong = "22";

        public const string InLenhXuat = "94";
        public const string DaTaoDonChiTiet = "95";
        public const string TamNgungDonHang = "96";
        public const string TiepTucDonHang = "97";
        public const string ThayDoiThongTin = "98";
        public const string DangXuLyTrongKho = "99";


        public static List<string> ListSearchTGBX = new List<string>()
        {
            VaoKho, DaTaoTichKe, DangXuatHang, DaXacNhanThucXuat,TGBX_MoiTao,TGBX_LenhTaoBangTay
        };

        public static List<string> ListSearchTGBX_XBTX = new List<string>()
        {
            DaPheDuyet, VaoKho, DaTaoTichKe, DangXuatHang, DaXacNhanThucXuat,TGBX_MoiTao,TGBX_LenhTaoBangTay
        };

        public static List<string> ListSearchDangLayHang = new List<string>()
        {
            VaoKho,DaTaoTichKe,DangXuatHang,DaXacNhanThucXuat,DaDongBoLenSap
        };

        public static List<string> ListSearchDaLayHang = new List<string>()
        {
            RoiKho,DaInHoaDon,DaNhanHang,DaChuyenNguonHangGui
        };

        public static List<string> ListSearchAssignVehicle = new List<string>()
        {
            DaPheDuyet,HuyVaoKho
        };

        public static List<string> ListSearchCancel = new List<string>()
        {
            DaPheDuyet,HuyVaoKho,ChoPheDuyet
        };

        public static List<string> ListStatus = new List<string>() {
            KhoiTao,ChoPheDuyet,TuChoi,
            HuyPheDuyet, DaPheDuyet,VaoKho,HuyVaoKho,TGBX_MoiTao, TGBX_LenhTaoBangTay,
            DaTaoTichKe,DangXuatHang,DaXacNhanThucXuat,DaDongBoLenSap,DaInHoaDon,RoiKho,DaNhanHang,
            TGBX_Huy, HuyDonHang,DaChuyenNguonHangGui, DaChuyenNhuong
        };

        public static List<string> ListStatusSearch = new List<string>() {
            KhoiTao,ChoPheDuyet,TuChoi,
            DaPheDuyet,VaoKho,DangXuLyTrongKho,
            HuyVaoKho,RoiKho, DaNhanHang,HuyDonHang,DaChuyenNguonHangGui
        };

        public static bool ValidateStatus(string status)
        {
            if (ListStatus.Exists(x => x == status))
            {
                return true;
            }
            return false;
        }

        public static string GetStatusText(string status)
        {
            switch (status)
            {
                case KhoiTao:
                    return "Khởi tạo";
                case ChoPheDuyet:
                    return "Chờ phê duyệt";
                case DangPheDuyet:
                    return "Chờ phê duyệt";
                case TuChoi:
                    return "Từ chối";
                case HuyPheDuyet:
                    return "Hủy phê duyệt";
                case DaPheDuyet:
                    return "Đã phê duyệt";
                case VaoKho:
                    return "Đã vào kho";
                case HuyVaoKho:
                    return "PT Hủy vào kho";
                case DaTaoTichKe:
                    return "Đang X.Lý xuất hàng";
                case DangXuatHang:
                    return "Đang X.Lý xuất hàng";
                case DaXacNhanThucXuat:
                    return "Đang X.Lý xuất hàng";
                case DaDongBoLenSap:
                    return "Đang X.Lý xuất hàng";
                case RoiKho:
                    return "Đã rời kho";
                case TGBX_Huy:
                    return "Đang X.Lý xuất hàng";
                case TGBX_MoiTao:
                    return "Đang X.Lý xuất hàng";
                case TGBX_LenhTaoBangTay:
                    return "Đang X.Lý xuất hàng";
                case DaInHoaDon:
                    return "Đã in chứng từ";
                case DaNhanHang:
                    return "Đã nhận hàng";
                case HuyDonHang:
                    return "Hủy đơn hàng";
                case DangXuLyTrongKho:
                    return "Đang X.Lý xuất hàng";
                case ThayDoiThongTin:
                    return "Thay đổi thông tin";
                case TamNgungDonHang:
                    return "Tạm ngưng đơn hàng";
                case TiepTucDonHang:
                    return "Tiếp tục đơn hàng";
                case DaChuyenNguonHangGui:
                    return "Đã chuyển nguồn hàng gửi";
                case DaChuyenNhuong:
                    return "Đã ủy quyền nhận hàng";
                case DaTaoDonChiTiet:
                    return "Đã tạo đơn chi tiết";
                case InLenhXuat:
                    return "In lệnh xuất";
                default:
                    return status;
            }
        }

        public static string GetStatusTextSaleManager(string status)
        {
            switch (status)
            {
                case KhoiTao:
                    return "Khởi tạo";
                case ChoPheDuyet:
                    return "Chờ phê duyệt";
                case TuChoi:
                    return "Từ chối";
                case DangPheDuyet:
                    return "Chờ phê duyệt";
                //case KinhDoanhPheDuyet:
                //    return "Kinh doanh đã phê duyệt";
                //case KeToanPheDuyet:
                //    return "Kế toán đã phê duyệt";
                case HuyPheDuyet:
                    return "Hủy phê duyệt";
                case DaPheDuyet:
                    return "Đã phê duyệt";
                case VaoKho:
                    return "Đã vào kho";
                case HuyVaoKho:
                    return "PT Hủy vào kho";
                case DaTaoTichKe:
                    return "Đã tạo tích kê";
                case DangXuatHang:
                    return "Đang xuất hàng";
                case DaXacNhanThucXuat:
                    return "Đã xác nhận thực xuất";
                case DaDongBoLenSap:
                    return "Đã đồng bộ lên SAP";
                case RoiKho:
                    return "Đã rời kho";
                case DaChuyenNguonHangGui:
                    return "Đã chuyển nguồn hàng gửi";
                case TGBX_Huy:
                    return "Bị hủy trên TGBX";
                case TGBX_MoiTao:
                    return "Mới tạo trên TGBX";
                case TGBX_LenhTaoBangTay:
                    return "Lệnh đã được tạo bằng tay trên TGBX";
                case DaInHoaDon:
                    return "Đã in chứng từ";
                case DaNhanHang:
                    return "Đã nhận hàng";
                case HuyDonHang:
                    return "Hủy đơn hàng";
                case DangXuLyTrongKho:
                    return "Đang X.Lý xuất hàng";
                case ThayDoiThongTin:
                    return "Thay đổi thông tin";
                case TamNgungDonHang:
                    return "Tạm ngưng đơn hàng";
                case TiepTucDonHang:
                    return "Tiếp tục đơn hàng";
                case DaChuyenNhuong:
                    return "Đã ủy quyền nhận hàng";
                case DaTaoDonChiTiet:
                    return "Đã tạo đơn chi tiết";
                case InLenhXuat:
                    return "In lệnh xuất";
                default:
                    return status;
            }
        }

        public static string GetStatusColor(string status)
        {
            switch (status)
            {
                case KhoiTao:
                    return "bg-blue-grey";
                case ChoPheDuyet:
                    return "bg-brown";
                case TuChoi:
                    return "bg-red";
                //case KinhDoanhPheDuyet:
                //    return "bg-brown";
                case DangPheDuyet:
                    return "bg-brown";
                case DaPheDuyet:
                    return "bg-purple";
                case VaoKho:
                    return "bg-teal";
                case HuyVaoKho:
                    return "bg-pink";
                case DaTaoTichKe:
                    return "bg-green";
                case DangXuatHang:
                    return "bg-green";
                case DaXacNhanThucXuat:
                    return "bg-green";
                case DaDongBoLenSap:
                    return "bg-blue";
                case RoiKho:
                    return "bg-blue";
                case DaChuyenNguonHangGui:
                    return "bg-blue";
                case TGBX_Huy:
                    return "bg-pink";
                case TGBX_MoiTao:
                    return "bg-green";
                case TGBX_LenhTaoBangTay:
                    return "bg-green";
                case DaInHoaDon:
                    return "bg-green";
                case DaNhanHang:
                    return "bg-green";
                case HuyDonHang:
                    return "bg-grey";
                case DangXuLyTrongKho:
                    return "bg-green";
                case ThayDoiThongTin:
                    return "bg-deep-orange";
                case TamNgungDonHang:
                    return "bg-deep-orange";
                case TiepTucDonHang:
                    return "bg-green";
                case DaChuyenNhuong:
                    return "bg-red";
                case DaTaoDonChiTiet:
                    return "bg-green";
                case InLenhXuat:
                    return "bg-green";
                default:
                    return "";
            }
        }

        public static string GetStatusColorSaleManager(string status)
        {
            switch (status)
            {
                case KhoiTao:
                    return "bg-blue-grey";
                case ChoPheDuyet:
                    return "bg-brown";
                case TuChoi:
                    return "bg-red";
                //case KinhDoanhPheDuyet:
                //    return "bg-blue";
                case DangPheDuyet:
                    return "bg-brown";
                case DaPheDuyet:
                    return "bg-purple";
                case VaoKho:
                    return "bg-teal";
                case HuyVaoKho:
                    return "bg-pink";
                case DaTaoTichKe:
                    return "bg-green";
                case DangXuatHang:
                    return "bg-light-green";
                case DaXacNhanThucXuat:
                    return "bg-cyan";
                case DaDongBoLenSap:
                    return "bg-blue";
                case RoiKho:
                    return "bg-blue";
                case DaChuyenNguonHangGui:
                    return "bg-blue";
                case TGBX_Huy:
                    return "bg-pink";
                case TGBX_MoiTao:
                    return "bg-green";
                case TGBX_LenhTaoBangTay:
                    return "bg-green";
                case DaInHoaDon:
                    return "bg-green";
                case DaNhanHang:
                    return "bg-green";
                case HuyDonHang:
                    return "bg-grey";
                case DangXuLyTrongKho:
                    return "bg-green";
                case ThayDoiThongTin:
                    return "bg-deep-orange";
                case TamNgungDonHang:
                    return "bg-deep-orange";
                case TiepTucDonHang:
                    return "bg-green";
                case DaChuyenNhuong:
                    return "bg-red";
                case DaTaoDonChiTiet:
                    return "bg-green";
                case InLenhXuat:
                    return "bg-green";
                default:
                    return "";
            }
        }

        public static string GetStatusIcon(string status)
        {
            switch (status)
            {
                case KhoiTao:
                    return "note_add";
                case ChoPheDuyet:
                    return "";
                case TuChoi:
                    return "cancel";
                //case KinhDoanhPheDuyet:
                //    return "";
                //case KeToanPheDuyet:
                    //return "";
                case DaPheDuyet:
                    return "done_all";
                case VaoKho:
                    return "input";
                case HuyVaoKho:
                    return "cancel";
                case DaTaoTichKe:
                    return "flag";
                case DangXuatHang:
                    return "move_to_inbox";
                case DaXacNhanThucXuat:
                    return "check";
                case DaDongBoLenSap:
                    return "cloud_done";
                case RoiKho:
                    return "local_shipping";
                //case DaChuyenNguonHangGui:
                //    return "";
                case TGBX_Huy:
                    return "cancel";
                case TGBX_MoiTao:
                    return "check";
                case TGBX_LenhTaoBangTay:
                    return "check";
                case DaInHoaDon:
                    return "print";
                case HuyDonHang:
                    return "delete";
                case DaChuyenNhuong:
                    return "swap_calls";
                case InLenhXuat:
                    return "print";
                default:
                    return "";
            }
        }

        public static string GetStatusNotifyText(string status)
        {
            switch (status)
            {
                case TuChoi:
                    return "đã bị từ chối";
                //case KinhDoanhPheDuyet:
                //    return "";
                //case KeToanPheDuyet:
                //    return "";
                case HuyPheDuyet:
                    return "đã hủy phê duyệt";
                case DaPheDuyet:
                    return "đã được phê duyệt";
                case VaoKho:
                    return "đã vào kho";
                case HuyVaoKho:
                    return "phương tiện đã hủy vào kho";
                case DaTaoTichKe:
                    return "đã được in tích kê";
                case DangXuatHang:
                    return "đang xuất hàng";
                case DaXacNhanThucXuat:
                    return "đã xác nhận thực xuất";
                case DaDongBoLenSap:
                    return "đã đồng bộ lên SAP";
                case RoiKho:
                    return "đã rời kho";
                //case DaChuyenNguonHangGui:
                //    return "";
                case TGBX_Huy:
                    return "đã hủy lấy hàng";
                case TGBX_MoiTao:
                    return "";
                case TGBX_LenhTaoBangTay:
                    return "";
                case DaInHoaDon:
                    return "đã in chứng từ";
                case HuyDonHang:
                    return "đã hủy đơn hàng";
                case DaChuyenNhuong:
                    return "đã ủy quyền nhận hàng";
                default: 
                    return "";
            }
        }

        public static string ConvertStatusTGBX(string status)
        {
            status = status.Trim();
            if (status == "1")
            {
                return PO_Status.TGBX_MoiTao;
            }
            else if (status == "2")
            {
                return PO_Status.TGBX_LenhTaoBangTay;
            }
            else if (status == "3")
            {
                return PO_Status.DaTaoTichKe;
            }
            else if (status == "31")
            {
                return PO_Status.DaXacNhanThucXuat;
            }
            else if (status == "4")
            {
                return PO_Status.DaXacNhanThucXuat;
            }
            else if (status == "5")
            {
                return PO_Status.DaDongBoLenSap;
            }
            return status;
        }
    }
}