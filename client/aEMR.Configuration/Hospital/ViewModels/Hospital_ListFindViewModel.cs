using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using aEMR.CommonTasks;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
*/
namespace aEMR.Configuration.Hospital.ViewModels
{
    [Export(typeof(IHospital_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Hospital_ListFindViewModel : ViewModelBase, IHospital_ListFind
        , IHandle<Hospital_Event_Save>
    {
        protected override void OnActivate()
        {
            authorization();
            Debug.WriteLine("OnActivate");
            base.OnActivate();
        }
        protected override void OnDeactivate(bool close)
        {
            Debug.WriteLine("OnDeActivate");
            base.OnDeactivate(close);
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Hospital_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            //ObjHospital_GetAll = new ObservableCollection<DataEntities.Hospital>();
            SearchCriteria = new HospitalSearchCriteria();
            SearchCriteria.HosName = "";
            SearchCriteria.HosAddress= "";
            Coroutine.BeginExecute(LoadHospitalType());
            LoadProvinces();
            ObjHospitalPaging = new PagedSortableCollectionView<DataEntities.Hospital>();
            ObjHospitalPaging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjHospitalPaging_OnRefresh);
        }

        void ObjHospitalPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            HospitalPaging(ObjHospitalPaging.PageIndex,
                            ObjHospitalPaging.PageSize, false);
        }

        private Visibility _hplAddNewVisible = Visibility.Visible;
        public Visibility hplAddNewVisible
        {
            get { return _hplAddNewVisible; }
            set
            {
                _hplAddNewVisible = value;
                NotifyOfPropertyChange(() => hplAddNewVisible);
            }
        }

        private ObservableCollection<CitiesProvince> _provinces;
        public ObservableCollection<CitiesProvince> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                NotifyOfPropertyChange(() => Provinces);
            }
        }
        
        public void LoadProvinces()
        {

            if (Globals.allCitiesProvince != null && Globals.allCitiesProvince.Count > 0)
            {
                Provinces = Globals.allCitiesProvince.ToObservableCollection();
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllProvinces(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<CitiesProvince> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllProvinces(asyncResult);
                                if (Globals.allCitiesProvince == null)
                                {
                                    Globals.allCitiesProvince = new List<CitiesProvince>(allItems);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            Provinces = allItems != null ? new ObservableCollection<CitiesProvince>(allItems) : null;
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
      

        private HospitalSearchCriteria _SearchCriteria;
        public HospitalSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DataEntities.Hospital> _ObjHospitalPaging;
        public PagedSortableCollectionView<DataEntities.Hospital> ObjHospitalPaging
        {
            get { return _ObjHospitalPaging; }
            set
            {
                _ObjHospitalPaging = value;
                NotifyOfPropertyChange(() => ObjHospitalPaging);
            }
        }


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mEdit);
            //bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mDelete);
            //bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                            , (int)eConfiguration_Management.mDanhMucPhong,
            //                            (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mView);
            //bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                    , (int)eConfiguration_Management.mDanhMucPhong,
            //                                    (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mAdd);
        }

        #region checking account
        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bbtSearch = true;
        private bool _bhplAddNew = true;
        public bool bhplEdit
        {
            get
            {
                return _bhplEdit;
            }
            set
            {
                if (_bhplEdit == value)
                    return;
                _bhplEdit = value;
            }
        }

        public bool bhplDelete
        {
            get
            {
                return _bhplDelete;
            }
            set
            {
                if (_bhplDelete == value)
                    return;
                _bhplDelete = value;
            }
        }

        public bool bbtSearch
        {
            get
            {
                return _bbtSearch;
            }
            set
            {
                if (_bbtSearch == value)
                    return;
                _bbtSearch = value;
            }
        }

        public bool bhplAddNew
        {
            get
            {
                return _bhplAddNew;
            }
            set
            {
                if (_bhplAddNew == value)
                    return;
                _bhplAddNew = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }

        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }
        #endregion
        private ObservableCollection<Lookup> _HospitalType;
        public ObservableCollection<Lookup> HospitalType
        {
            get { return _HospitalType; }
            set
            {
                _HospitalType = value;
                NotifyOfPropertyChange(() => HospitalType);
            }
        }

        private IEnumerator<IResult> LoadHospitalType()
        {
            var paymentModeTask = new LoadLookupListTask(LookupValues.HOSPITAL_TYPE);
            yield return paymentModeTask;
            HospitalType = paymentModeTask.LookupList;
            yield break;
        }
        public void btSearch()
        {
            ObjHospitalPaging.PageIndex = 0;
            HospitalPaging(0, ObjHospitalPaging.PageSize, true);
        }

        public void Hospital_MarkDeleted(Int64 IDCode)
        {
            string Result = "";
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHospital_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndHospital_MarkDelete(out Result, asyncResult);
                                //if (Result == "InUse")
                                //{
                                //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
                                //}
                                if (Result == "0")
                                {
                                    Globals.ShowMessage("Thất bại", "Thông báo");
                                }
                                if (Result == "1")
                                {
                                    ObjHospitalPaging.PageIndex = 0;
                                    HospitalPaging(0, ObjHospitalPaging.PageSize, true);
                                    Globals.ShowMessage("Dừng sử dụng Hospital", "Thông báo");
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void hplDelete_Click(object selectedItem)
        {

            DataEntities.Hospital p = (selectedItem as DataEntities.Hospital);
            //if (p != null && p.IDCode > 0)
            //{
            //    if (MessageBox.Show(string.Format("Bạn có muốn dừng Hospital này", p.Hospital10Code), "Tạm dừng Hospital", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        Hospital_MarkDeleted(p.IDCode);
            //    }
            //}
        }

        private object _Hospital_Current;
        public object Hospital_Current
        {
            get { return _Hospital_Current; }
            set
            {
                _Hospital_Current = value;
                NotifyOfPropertyChange(() => Hospital_Current);
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            Hospital_Current = eventArgs.Value as DataEntities.Hospital;
            Globals.EventAggregator.Publish(new dgHospitalListClickSelectionChanged_Event() { Hospital_Current = eventArgs.Value });
        }

        public void dtgListSelectionChanged(object args)
        {
            if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            {
                if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
                {
                    Hospital_Current =
                        ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0];
                    var typeInfo = Globals.GetViewModel<IHospital_ListFind>();
                    typeInfo.Hospital_Current = (DataEntities.Hospital)Hospital_Current;
               
                    Globals.EventAggregator.Publish(new dgHospitalListClickSelectionChanged_Event()
                    {
                        Hospital_Current = ((object[]) (((System.Windows.Controls.SelectionChangedEventArgs) (args)).AddedItems))[0]
                    });
                }
            }
        }

        private void HospitalPaging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.ShowBusyIndicator(eHCMSResources.K3054_G1_DSPg);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginHospitalPaging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.Hospital> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndHospitalPaging(out Total, asyncResult);
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
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            ObjHospitalPaging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjHospitalPaging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjHospitalPaging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void cboCitiesProvinceSelectedItemChanged(object selectedItem)
        {
            SearchCriteria.CityProvinceID = (selectedItem as DataEntities.CitiesProvince).CityProvinceID;
            ObjHospitalPaging.PageIndex = 0;
            HospitalPaging(0, ObjHospitalPaging.PageSize, true);
        }

        public void hplAddNew_Click()
        {
            Action<IHospitalAutoCompleteEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.Title= "Thêm Mới Bệnh viện";
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IHospitalAutoCompleteEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.IsUpdate = true;
                    typeInfo.SelectedHospital = ObjectCopier.DeepCopy((selectedItem as DataEntities.Hospital));
                    typeInfo.Title = "Hiệu Chỉnh (" + (selectedItem as DataEntities.Hospital).HosName.Trim() + ")";
                };
                GlobalsNAV.ShowDialog(onInitDlg);
            }
        }

        public void Handle(Hospital_Event_Save message)
        {
            ObjHospitalPaging.PageIndex = 0;
            HospitalPaging(0, ObjHospitalPaging.PageSize, true);
        }
    }
}
