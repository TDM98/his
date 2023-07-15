using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Common.PagedCollectionView;

namespace aEMR.Configuration.ListDeptLocation_ByPCLExamTypeID.ViewModels
{
    [Export(typeof(IListDeptLocation_ByPCLExamTypeID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ListDeptLocation_ByPCLExamTypeIDViewModel : Conductor<object>, IListDeptLocation_ByPCLExamTypeID
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ListDeptLocation_ByPCLExamTypeIDViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private long _PCLExamTypeID;
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                _PCLExamTypeID = value;
                NotifyOfPropertyChange(() => PCLExamTypeID);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ListDeptLocation_ByPCLExamTypeID();
        }


        private PagedCollectionView _ObjListDeptLocation_ByPCLExamTypeIDList;
        public PagedCollectionView ObjListDeptLocation_ByPCLExamTypeIDList
        {
            get
            {
                return _ObjListDeptLocation_ByPCLExamTypeIDList;
            }
            set
            {
                if (_ObjListDeptLocation_ByPCLExamTypeIDList != value)
                {
                    _ObjListDeptLocation_ByPCLExamTypeIDList = value;
                    NotifyOfPropertyChange(() => ObjListDeptLocation_ByPCLExamTypeIDList);
                }
            }
        }

        private void ListDeptLocation_ByPCLExamTypeID()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginListDeptLocation_ByPCLExamTypeID(PCLExamTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndListDeptLocation_ByPCLExamTypeID(asyncResult);

                            ObjListDeptLocation_ByPCLExamTypeIDList = new PagedCollectionView(results);
                            ObjListDeptLocation_ByPCLExamTypeIDList.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("RefDepartment.DeptName"));

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingOrderID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.DeptLocation p = (selectedItem as DataEntities.DeptLocation);

            if (p != null && p.DeptLocationID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.Location.LocationName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PCLExamTypeLocations_MarkDeleted(PCLExamTypeID,p.DeptLocationID);
                }
            }
        }

        private void PCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID)
        {
            string Result = "";

            var t = new Thread(() =>
            {  

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeLocations_MarkDeleted(PCLExamTypeID, DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPCLExamTypeLocations_MarkDeleted(out Result, asyncResult);
                            switch (Result)
                            {
                                case "0":
                                    {
                                        MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
                                case "1":
                                    {
                                        ListDeptLocation_ByPCLExamTypeID();
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

    }
}
