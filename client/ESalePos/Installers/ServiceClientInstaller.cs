using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;


namespace aEMRClient.Installers
{
    public class ServiceClientInstaller : IWindsorInstaller
    {
        private readonly static string BaseAddress =  ConfigurationManager.AppSettings["BaseAddress"];

        private IWcfEndpoint CreateDefaultEndpoint(string relativeAddress)
        {
            //return
            //    WcfEndpoint.BoundTo(new BasicHttpBinding
            //    {
            //        MaxBufferSize = 2147483647,
            //        MaxReceivedMessageSize = 2147483647,
            //        OpenTimeout = new TimeSpan(0, 0, 10, 0),
            //        ReceiveTimeout = new TimeSpan(0, 0, 10, 0),
            //        SendTimeout = new TimeSpan(0, 0, 10, 0)
            //    }).At(BaseAddress + relativeAddress);

            var myReaderQuotas = new XmlDictionaryReaderQuotas
            {
                MaxStringContentLength = 2147483647,
                MaxArrayLength = 2147483647,
                MaxBytesPerRead = 2147483647,
                MaxDepth = 2147483647,
                MaxNameTableCharCount = 2147483647
            };

            return
                WcfEndpoint.BoundTo(new BasicHttpBinding
                {
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647,
                    OpenTimeout = new TimeSpan(0, 0, 10, 0),
                    ReceiveTimeout = new TimeSpan(0, 0, 10, 0),
                    SendTimeout = new TimeSpan(0, 0, 10, 0),
                    ReaderQuotas = myReaderQuotas
                }).At(BaseAddress + relativeAddress);
            

        }

        private void RegisterDefaultClientModel<T>(IWindsorContainer container, string relativeAddress) where T : class 
        {
            container.Register(Component.For<T>()
                                  .AsWcfClient(new DefaultClientModel
                                  {
                                      Endpoint = CreateDefaultEndpoint(relativeAddress)
                                  }));
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //container.Register(Component.For<IEndpointBehavior>()
            //                       .ImplementedBy<ExtendMaxItemsInObjectGraphBehavior>());

            //RegisterDefaultClientModel<IReportItemService>(container, ServiceClientAddress.ReportItemService);
            //RegisterDefaultClientModel<ISalegroupService>(container, ServiceClientAddress.SaleGroupService);
            //RegisterDefaultClientModel<ILookupService>(container, ServiceClientAddress.LookupService);
            //RegisterDefaultClientModel<IItemService>(container, ServiceClientAddress.ItemServiceAddress);
            //RegisterDefaultClientModel<ICategoryService>(container, ServiceClientAddress.CategoryServiceAddress);
            //RegisterDefaultClientModel<IDepartmentService>(container, ServiceClientAddress.AuthenticateService);
            //RegisterDefaultClientModel<IAuthenticateService>(container, ServiceClientAddress.AuthenticateService);
            //RegisterDefaultClientModel<IInwardGoodService>(container, ServiceClientAddress.InwardGoodService);
            //RegisterDefaultClientModel<IOutwardGoodService>(container, ServiceClientAddress.OutwardGoodService);
            //RegisterDefaultClientModel<ISupplierService>(container, ServiceClientAddress.SupplierService);
            //RegisterDefaultClientModel<ICustomerService>(container, ServiceClientAddress.CustomerService);
            //RegisterDefaultClientModel<IInvoiceService>(container, ServiceClientAddress.InvoiceService);
            //RegisterDefaultClientModel<IItemTypeService>(container, ServiceClientAddress.ItemTypeServiceAddress);
            //RegisterDefaultClientModel<IItemFieldManagementService>(container, ServiceClientAddress.ItemFieldManagementServiceAddress);
            //RegisterDefaultClientModel<IInventoryService>(container, ServiceClientAddress.InventoryService);
            //RegisterDefaultClientModel<IWarehouseService>(container, ServiceClientAddress.WarehouseService);
            //RegisterDefaultClientModel<IReasonService>(container, ServiceClientAddress.ReasonService);
            //RegisterDefaultClientModel<ICreditnoteService>(container, ServiceClientAddress.CreditnoteService);
            //RegisterDefaultClientModel<IRefundCashService>(container, ServiceClientAddress.RefundCashService);
            //RegisterDefaultClientModel<IGiftcardService>(container, ServiceClientAddress.GiftcardService);            
            //RegisterDefaultClientModel<IMultimediaService>(container, ServiceClientAddress.MultimediaService);
            //RegisterDefaultClientModel<IUserManageService>(container, ServiceClientAddress.UserManageService);
            //RegisterDefaultClientModel<ISuburbService>(container, ServiceClientAddress.SuburbService);
            //RegisterDefaultClientModel<IExchangeItemService>(container, ServiceClientAddress.ExchangeItemService);
        }
    }
}
