using aEMR.Infrastructure.ServiceCore;
using eHCMSLanguage;
using System.Collections.Generic;
using VNPTAccountingServiceProxy;

namespace aEMR.ServiceClient
{
    public class VNPTAccountingPortalServiceClient : ServiceClientFactory<IVNPTAccountingPortalService>
    {
        //public const string gAdminUserName = "vientimtphcmservice";
        //public const string gAdminUserPass = "123456aA@";
        //public const string gAccountUserName = "vientimtphcmadmin";
        //public const string gAccountUserPass = "123456aA@";
        public static readonly Dictionary<string, string> ErrorCodeDetails = new Dictionary<string, string>() {
            { "ERR:1", eHCMSResources.Z2439_G1_TaiKhoanDangNhapSaiHoacKQ },
            { "ERR:3", eHCMSResources.A0727_G1_Msg_ConfThemMoiBN }
        };
        public override string EndPointName
        {
            get { return "BasicHttpBinding_PortalService"; }
        }
        public override string EndPointAddress
        {
            get { return "http://tempuri.org"; }
        }
    }
}