using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;

namespace aEMR.Configuration.MedServiceItemPrice.ViewModels
{
    [Export(typeof(IMedServiceItemPrice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedServiceItemPriceViewModel : Conductor<object>, IMedServiceItemPrice
        ,IHandle<MedServiceItemPrice_AddEditViewModel_Save_Event>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedServiceItemPriceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }



        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);

        }

        private PagedSortableCollectionView<DataEntities.MedServiceItemPrice> _ObjMedServiceItemPriceByDeptMedServItemID_Paging;
        public PagedSortableCollectionView<DataEntities.MedServiceItemPrice> ObjMedServiceItemPriceByDeptMedServItemID_Paging
        {
            get { return _ObjMedServiceItemPriceByDeptMedServItemID_Paging; }
            set
            {
                _ObjMedServiceItemPriceByDeptMedServItemID_Paging = value;
                NotifyOfPropertyChange(()=>ObjMedServiceItemPriceByDeptMedServItemID_Paging);
            }
        }

        private void MedServiceItemPriceByDeptMedServItemID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Giá..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginMedServiceItemPriceByDeptMedServItemID_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.MedServiceItemPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndMedServiceItemPriceByDeptMedServItemID_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                            ObjMedServiceItemPriceByDeptMedServItemID_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjMedServiceItemPriceByDeptMedServItemID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjMedServiceItemPriceByDeptMedServItemID_Paging.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }

        private MedServiceItemPriceSearchCriteria _SearchCriteria;
        public MedServiceItemPriceSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }
     
        private object _ISearchCriteria;
        public object ISearchCriteria
        {
            get
            {
                return _ISearchCriteria;
            }
            set
            {
                _ISearchCriteria = value;
                NotifyOfPropertyChange(() => ISearchCriteria);
            }
        }

        public void LoadForm()
        {
            SearchCriteria = (ISearchCriteria as MedServiceItemPriceSearchCriteria);
            ObjDeptMedServiceItems_Current = (IDeptMedServiceItems_Current as DataEntities.DeptMedServiceItems);

            ObjMedServiceItemPriceByDeptMedServItemID_Paging=new PagedSortableCollectionView<DataEntities.MedServiceItemPrice>();
            ObjMedServiceItemPriceByDeptMedServItemID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjMedServiceItemPriceByDeptMedServItemID_Paging_OnRefresh);

            ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageIndex = 0;
            MedServiceItemPriceByDeptMedServItemID_Paging(0, ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageSize, true);
        }

        void ObjMedServiceItemPriceByDeptMedServItemID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            MedServiceItemPriceByDeptMedServItemID_Paging(ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageIndex, ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageSize, false);
        }


        private DataEntities.DeptMedServiceItems _ObjDeptMedServiceItems_Current;
        public DataEntities.DeptMedServiceItems ObjDeptMedServiceItems_Current
        {
            get { return _ObjDeptMedServiceItems_Current; }
            set
            {
                _ObjDeptMedServiceItems_Current = value;
                NotifyOfPropertyChange(() => ObjDeptMedServiceItems_Current);
            }
        }

        private object _IDeptMedServiceItems_Current;
        public object IDeptMedServiceItems_Current
        {
            get { return _IDeptMedServiceItems_Current; }
            set
            {
                _IDeptMedServiceItems_Current = value;
                NotifyOfPropertyChange(() => IDeptMedServiceItems_Current);
            }
        }

        public void cboPriceTypeSelectedItemChanged(object selectedIndex)
        {
            if (selectedIndex != null)
            {
                SearchCriteria.V_TypePrice = int.Parse(selectedIndex.ToString())-1;
                ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageIndex = 0;
                MedServiceItemPriceByDeptMedServItemID_Paging(0, ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageSize, true);
            }
        }

        public void btFind()
        {
            ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageIndex = 0;
            MedServiceItemPriceByDeptMedServItemID_Paging(0, ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageSize, true);
        }

        private bool CheckDateValid()
        {
            if (IStatusCheckFindDate)
            {
                if (SearchCriteria.FromDate != null && SearchCriteria.ToDate != null)
                {
                    if (SearchCriteria.FromDate > SearchCriteria.ToDate)
                    {
                        MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                        return false;
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0885_G1_Msg_InfoNhapTuNgDenNg2, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }

        private bool _IStatusFromDate = false;
        public bool IStatusFromDate
        {
            get { return _IStatusFromDate; }
            set { _IStatusFromDate = value;
                NotifyOfPropertyChange(()=>IStatusFromDate);
            }
        }

        private bool _IStatusToDate=false;
        public bool IStatusToDate
        {
            get { return _IStatusToDate; }
            set
            {
                _IStatusToDate = value;
                NotifyOfPropertyChange(() => IStatusToDate);
            }
        }

        private bool _IStatusCheckFindDate;
        public bool IStatusCheckFindDate
        {
            get { return _IStatusCheckFindDate; }
            set { _IStatusCheckFindDate = value;
                NotifyOfPropertyChange(()=>IStatusCheckFindDate);
            }
        }
             

        public void chkFindByDate_Click(object args)
        {
            IStatusCheckFindDate = (((System.Windows.Controls.Primitives.ToggleButton)(((System.Windows.RoutedEventArgs)(args)).OriginalSource)).IsChecked.Value);
            IStatusFromDate = IStatusCheckFindDate;
            IStatusToDate = IStatusCheckFindDate;
            if (IStatusCheckFindDate == false)
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }
        }

        public void hplAddNewPrice()
        {
            //var typeInfo = Globals.GetViewModel<IMedServiceItemPrice_AddEdit>();
            //typeInfo.IDeptMedServiceItems_Current = ObjDeptMedServiceItems_Current;
            //typeInfo.TitleForm = "Thêm Giá Mới (" + ObjDeptMedServiceItems_Current.ObjRefMedicalServiceItem.MedServiceName.Trim() + ")" ;

            //DataEntities.MedServiceItemPrice ObjMedServiceItemPrice_New=new DataEntities.MedServiceItemPrice();
            //ObjMedServiceItemPrice_New.StaffID= Globals.LoggedUserAccount.Staff != null
            //                                                                   ? Globals.LoggedUserAccount.Staff.
            //                                                                         StaffID
            //                                                                   : (Globals.LoggedUserAccount.StaffID.
            //                                                                          HasValue
            //                                                                          ? Globals.LoggedUserAccount.
            //                                                                                StaffID.Value
            //                                                                          : -1);
            
            //ObjMedServiceItemPrice_New.ApprovedStaffID = ObjMedServiceItemPrice_New.StaffID;
            //ObjMedServiceItemPrice_New.EffectiveDate = DateTime.Now;
            //ObjMedServiceItemPrice_New.MedServiceID = ObjDeptMedServiceItems_Current.MedServiceID;
            //typeInfo.IMedServiceItemPrice_Save = ObjMedServiceItemPrice_New;

            //typeInfo.LoadForm();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IMedServiceItemPrice_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.IDeptMedServiceItems_Current = ObjDeptMedServiceItems_Current;
                typeInfo.TitleForm = "Thêm Giá Mới (" + ObjDeptMedServiceItems_Current.ObjRefMedicalServiceItem.MedServiceName.Trim() + ")";

                DataEntities.MedServiceItemPrice ObjMedServiceItemPrice_New = new DataEntities.MedServiceItemPrice();
                ObjMedServiceItemPrice_New.StaffID = Globals.LoggedUserAccount.Staff != null
                                                                                   ? Globals.LoggedUserAccount.Staff.
                                                                                         StaffID
                                                                                   : (Globals.LoggedUserAccount.StaffID.
                                                                                          HasValue
                                                                                          ? Globals.LoggedUserAccount.
                                                                                                StaffID.Value
                                                                                          : -1);

                ObjMedServiceItemPrice_New.ApprovedStaffID = ObjMedServiceItemPrice_New.StaffID;
                ObjMedServiceItemPrice_New.EffectiveDate = DateTime.Now;
                ObjMedServiceItemPrice_New.MedServiceID = ObjDeptMedServiceItems_Current.MedServiceID;
                typeInfo.IMedServiceItemPrice_Save = ObjMedServiceItemPrice_New;

                typeInfo.LoadForm();
            };
            GlobalsNAV.ShowDialog<IMedServiceItemPrice_AddEdit>(onInitDlg);
        }

        public void hplEditPrice_Click(object datacontext)
        {
            DataEntities.MedServiceItemPrice p = (datacontext as DataEntities.MedServiceItemPrice);

            if (p.PriceType == "PriceOld")
            {
                MessageBox.Show(eHCMSResources.Z0661_G1_GiaCuTrongQuaKhu, eHCMSResources.Z0660_G1_SuaGia, MessageBoxButton.OK);
            }
            else
            {
                //var typeInfo = Globals.GetViewModel<IMedServiceItemPrice_AddEdit>();
                //typeInfo.IDeptMedServiceItems_Current = ObjDeptMedServiceItems_Current;

                //typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDeptMedServiceItems_Current.ObjRefMedicalServiceItem.MedServiceName);
                
                //p.StaffID = Globals.LoggedUserAccount.Staff != null
                //                                                                   ? Globals.LoggedUserAccount.Staff.
                //                                                                         StaffID
                //                                                                   : (Globals.LoggedUserAccount.StaffID.
                //                                                                          HasValue
                //                                                                          ? Globals.LoggedUserAccount.
                //                                                                                StaffID.Value
                //                                                                          : -1);

                //p.ApprovedStaffID = p.StaffID;

                //typeInfo.IMedServiceItemPrice_Save =ObjectCopier.DeepCopy(p);

                //typeInfo.LoadForm();

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IMedServiceItemPrice_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.IDeptMedServiceItems_Current = ObjDeptMedServiceItems_Current;

                    typeInfo.TitleForm = string.Format(eHCMSResources.Z0664_G1_HChinhGia0, ObjDeptMedServiceItems_Current.ObjRefMedicalServiceItem.MedServiceName);

                    p.StaffID = Globals.LoggedUserAccount.Staff != null
                                                                                       ? Globals.LoggedUserAccount.Staff.
                                                                                             StaffID
                                                                                       : (Globals.LoggedUserAccount.StaffID.
                                                                                              HasValue
                                                                                              ? Globals.LoggedUserAccount.
                                                                                                    StaffID.Value
                                                                                              : -1);

                    p.ApprovedStaffID = p.StaffID;

                    typeInfo.IMedServiceItemPrice_Save = ObjectCopier.DeepCopy(p);

                    typeInfo.LoadForm();
                };
                GlobalsNAV.ShowDialog<IMedServiceItemPrice_AddEdit>(onInitDlg);
            }
        }

        public void hplDeletePrice_Click(object datacontext)
        {
            DataEntities.MedServiceItemPrice p = (datacontext as DataEntities.MedServiceItemPrice);

            if (p.CanDelete.Value)
            {
                if (MessageBox.Show(eHCMSResources.A0159_G1_Msg_ConfXoaGia, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MedServiceItemPrice_MarkDelete(p.MedServItemPriceID);
                }
            }
            else
            {
                if (p.PriceType == "PriceCurrent")
                {
                    MessageBox.Show(eHCMSResources.A0552_G1_Msg_InfoKhDcXoaGiaDangApDung, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                }
            }
        }
        private void MedServiceItemPrice_MarkDelete(Int64 MedServItemPriceID)
        {
             string Result = "";

             //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

             var t = new Thread(() =>
             {
                 IsLoading = true;

                 using (var serviceFactory = new ConfigurationManagerServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginMedServiceItemPrice_MarkDelete(MedServItemPriceID, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             contract.EndMedServiceItemPrice_MarkDelete(out Result, asyncResult);
                             if (Result == "Delete-1")
                             {
                                 ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageIndex = 0;
                                 MedServiceItemPriceByDeptMedServItemID_Paging(0, ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageSize, true);
                                 MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.A0481_G1_Msg_XoaGia, MessageBoxButton.OK);
                             }
                             else
                             {
                                 MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.A0481_G1_Msg_XoaGia, MessageBoxButton.OK);
                             }
                         }
                         catch (Exception ex)
                         {
                             Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                         }
                         finally
                         {
                             //Globals.IsBusy = false;
                             IsLoading = false;
                         }
                     }), null);
                 }


             });
             t.Start();
        }

        public void btClose()
        {
            TryClose();
        }

        public void Handle(MedServiceItemPrice_AddEditViewModel_Save_Event message)
        {
            if(message!=null)
            {
                ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageIndex = 0;
                MedServiceItemPriceByDeptMedServItemID_Paging(0, ObjMedServiceItemPriceByDeptMedServItemID_Paging.PageSize, true);
            }
        }
    }
}
