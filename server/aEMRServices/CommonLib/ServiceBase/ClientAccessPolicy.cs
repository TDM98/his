using System.IO;
using System.ServiceModel.Web;
using System.Text;  

namespace eHCMS
{
    public class ClientAccessPolicy : IClientAccessPolicy
    {
        #region IClientAccessPolicy Members

        public Stream GetClientAccessPolicy()
        {
            string str = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                <access-policy>
                                  <cross-domain-access>
                                    <policy>
                                      <allow-from http-request-headers=""*"">
                                        <domain uri=""*""/>
                                      </allow-from>
                                      <grant-to>
                                        <resource path=""/"" include-subpaths=""true""/>
                                      </grant-to>
                                    </policy>
                                  </cross-domain-access>
                                </access-policy>";
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        #endregion
    }
}
