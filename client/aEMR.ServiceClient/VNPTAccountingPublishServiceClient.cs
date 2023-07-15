using aEMR.Infrastructure.ServiceCore;
using eHCMSLanguage;
using System.Collections.Generic;
using VNPTAccountingServiceProxy;

namespace aEMR.ServiceClient
{
    public class VNPTAccountingPublishServiceClient : ServiceClientFactory<IVNPTAccountingPublishService>
    {
        public static readonly Dictionary<int, string> ErrorCodeDetails = new Dictionary<int, string>() {
            { -1, eHCMSResources.Z2439_G1_TaiKhoanDangNhapSaiHoacKQ },
            { -2, eHCMSResources.Z2439_G1_ThemMoiKhongThanhCong },
            { -3, eHCMSResources.Z2439_G1_DuLieuDauVaoKhongDungQD },
            { -5, eHCMSResources.Z2439_G1_BenhNhanDaTonTai }
        };
        public static readonly Dictionary<string, string> ImportAndPublishInvErrorCodeDetails = new Dictionary<string, string>() {
            { "ERR:1", eHCMSResources.Z2439_G1_TaiKhoanDangNhapSaiHoacKQ },
            { "ERR:2", eHCMSResources.Z2649_G1_HoaDonKhongTonTai },
            { "ERR:3", eHCMSResources.Z2439_G1_DuLieuDauVaoKhongDungQD },
            { "ERR:5", eHCMSResources.Z2439_G1_PhatHanhKhongThanhCong },
            { "ERR:6", eHCMSResources.Z2439_G1_KhongDuSoHDChoLoPH },
            { "ERR:7", eHCMSResources.Z2439_G1_TKKhongPHHoacKhongCoCty },
            { "ERR:8", eHCMSResources.Z2649_G1_KhongTheThucHien },
            { "ERR:9", eHCMSResources.Z2649_G1_TrangThaiKhongPhuHop },
            { "ERR:10", eHCMSResources.Z2439_G1_LoCoSHDVuotQuaSLTD },
            { "ERR:13", string.Format(eHCMSResources.Z0634_G1_0DaTonTai, eHCMSResources.Z2439_G1_invKey) },
            { "ERR:19", eHCMSResources.Z2649_G1_MauSoKhongKhop },
            { "ERR:20", eHCMSResources.Z2439_G1_MauSoVaKyHieuKhongPH }
        };
        public override string EndPointName
        {
            get { return "BasicHttpBinding_PublishService"; }
        }
        public override string EndPointAddress
        {
            get { return "http://tempuri.org"; }
        }
    }
}