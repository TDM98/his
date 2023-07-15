using System;
using System.ServiceModel;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EPos.BusinessLayers.ServiceClients;
using EPos.DataTransferObjects;
using EPos.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestClientService
{
    [TestClass]
    public class TestServiceClient
    {
        [TestMethod]
        public void TestAuthenticateServiceClient()
        {
            var container = new WindsorContainer();
            container.AddFacility<WcfFacility>()
                .Register(Component.For<IAuthenticateService>().AsWcfClient(new DefaultClientModel
                                                                                {
                                                                                    Endpoint = WcfEndpoint.BoundTo(new BasicHttpBinding())
                                                                                    .At("http://localhost:19047/Services/AuthenticateService.svc")
                                                                                }));

            var authenticateService = container.Resolve<IAuthenticateService>();

            var staff = new StaffDto();

            var isCompleted = false;

            Assert.IsNotNull(authenticateService);

            authenticateService.BeginWcfCall(
                    s => s.Authenticate("admin", "password")
                    ,
                    (asyncCall) =>
                    {
                        staff = asyncCall.End();
                        isCompleted = true;
                        
                    }

                    , null);

            while (!isCompleted)
            {
                
            }

            Assert.IsTrue( staff != null && staff.Sitepk > 0);

        }
    }
}
