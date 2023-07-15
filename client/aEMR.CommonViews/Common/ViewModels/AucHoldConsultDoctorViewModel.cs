using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Utilities;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.ObjectModel;
using System.Linq;
using eHCMSCommon.Utilities;

/*
 * 20180908 #001 TBL: Lay them chung chi hanh nghe 
 * 20211004 #002 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 * 20230424 #003 QTD: Lọc danh sách người thực hiện
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAucHoldConsultDoctor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AucHoldConsultDoctorViewModel : Conductor<object>, IAucHoldConsultDoctor
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AucHoldConsultDoctorViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            ShowStopUsing = false;
            CurrentDeptID = 0;
            eventAgr.Subscribe(this);
        }

        private long _CurrentDeptID = 0;
        public long CurrentDeptID
        {
            get { return _CurrentDeptID; }
            set
            {
                _CurrentDeptID = value;
                NotifyOfPropertyChange(() => CurrentDeptID);
            }
        }

        private bool _ShowStopUsing = false;
        public bool ShowStopUsing
        {
            get { return _ShowStopUsing; }
            set
            {
                _ShowStopUsing = value;
                NotifyOfPropertyChange(() => ShowStopUsing);
            }
        }

        private bool _IsDoctorOnly = false;
        public bool IsDoctorOnly
        {
            get { return _IsDoctorOnly; }
            set
            {
                _IsDoctorOnly = value;
                NotifyOfPropertyChange(() => IsDoctorOnly);
            }
        }

        private long _StaffID;
        public long StaffID
        {
            get { return _StaffID; }
            set
            {
                _StaffID = value;
                NotifyOfPropertyChange(() => StaffID);
            }
        }

        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        //KMx: Vì ViewModel này nhiều chổ sử dụng và mặc định là tìm Nhân viên đăng ký, nhưng "Bảng kê chi tiết khám bệnh" cần tìm theo Bác sĩ
        //     Nên tạo ra biến này để những cái cũ vẫn hoạt động bình thường.
        private long _StaffCatType = (long)V_StaffCatType.NhanVienQuayDangKy;
        public long StaffCatType
        {
            get {return _StaffCatType;}
            set
            {
                _StaffCatType = value;
                NotifyOfPropertyChange(() => StaffCatType);
            }
        }

        //KMx: Nếu tìm nhiều loại nhân viên cùng 1 lúc thì set IsMultiStaffCatType = true (27/12/2014 13:57).
        private bool _IsMultiStaffCatType;
        public bool IsMultiStaffCatType
        {
            get { return _IsMultiStaffCatType; }
            set
            {
                _IsMultiStaffCatType = value;
                NotifyOfPropertyChange(() => IsMultiStaffCatType);
            }
        }

        private ObservableCollection<long> _StaffCatTypeList = new ObservableCollection<long>();
        public ObservableCollection<long> StaffCatTypeList
        {
            get { return _StaffCatTypeList; }
            set
            {
                _StaffCatTypeList = value;
                NotifyOfPropertyChange(() => StaffCatTypeList);
            }
        }

        #region nhan vien
        private ObservableCollection<Staff> _StaffCatgs;
        public ObservableCollection<Staff> StaffCatgs
        {
            get
            {
                return _StaffCatgs;
            }
            set
            {
                if (_StaffCatgs != value)
                {
                    _StaffCatgs = value;
                    NotifyOfPropertyChange(() => StaffCatgs);
                }
            }
        }

        private void SearchStaffCatgs(string SearchKeys)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //contract.BeginSearchStaffCat(new Staff { FullName = SearchKeys, RefStaffCategory = new RefStaffCategory { StaffCatgID = (long)StaffCatg.NhanVienDangKy } }, 0, 1000, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginSearchStaffCat(new Staff { FullName = SearchKeys, RefStaffCategory = new RefStaffCategory { StaffCatgID = StaffCatType } }, 0, 1000, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchStaffCat(asyncResult);
                            if (results != null)
                            {
                                StaffCatgs = new ObservableCollection<Staff>();
                                foreach (Staff p in results)
                                {
                                    if(IsDoctorOnly && p.RefStaffCategory != null)
                                    {
                                        if (p.RefStaffCategory.StaffCatgDescription.Contains("Bs"))
                                        {
                                            StaffCatgs.Add(p);
                                        }
                                    }
                                    else
                                    {
                                        StaffCatgs.Add(p);
                                    }
                                }
                                NotifyOfPropertyChange(() => StaffCatgs);
                            }
                            aucHoldConsultDoctor.ItemsSource = StaffCatgs;
                            aucHoldConsultDoctor.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void setDefault(string DefaultStaffName = null)
        {
            if (aucHoldConsultDoctor == null) return;
            StaffID = 0;
            aucHoldConsultDoctor.Text = DefaultStaffName;
        }

        AutoCompleteBox aucHoldConsultDoctor;

        public void aucHoldConsultDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            aucHoldConsultDoctor = sender as AutoCompleteBox;
        }

        public void aucHoldConsultDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            if (Globals.AllStaffs == null || Globals.AllStaffs.Count <= 0)
            {
                return;
            }

            if (IsMultiStaffCatType)
            {
                if (StaffCatTypeList == null || StaffCatTypeList.Count <= 0)
                {
                    return;
                }
                if (!IsDoctorOnly)
                {
                    if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
                    {
                        StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && StaffCatTypeList.Contains(o.RefStaffCategory.V_StaffCatType)
                                                        && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                                        && o.ListDeptResponsibilities != null && ((o.ListDeptResponsibilities.Contains(CurrentDeptID.ToString()) || CurrentDeptID == 0))
                                                        && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                    }
                    else
                    {
                        StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && StaffCatTypeList.Contains(o.RefStaffCategory.V_StaffCatType)
                                                        && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                                        && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                    }                        
                }
                else
                {
                    if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
                    {
                        StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && StaffCatTypeList.Contains(o.RefStaffCategory.V_StaffCatType)
                                                       && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                                       && (VNConvertString.ConvertString(o.RefStaffCategory.StaffCatgDescription).ToLower().Contains("bs"))
                                                       && o.ListDeptResponsibilities != null && ((o.ListDeptResponsibilities.Contains(CurrentDeptID.ToString()) || CurrentDeptID == 0))
                                                       && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                    }
                    else
                    {
                        StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && StaffCatTypeList.Contains(o.RefStaffCategory.V_StaffCatType)
                                                          && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                                          && (VNConvertString.ConvertString(o.RefStaffCategory.StaffCatgDescription).ToLower().Contains("bs"))
                                                          && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                    }                       
                }
                NotifyOfPropertyChange(() => StaffCatgs);
                aucHoldConsultDoctor.ItemsSource = StaffCatgs;
                aucHoldConsultDoctor.PopulateComplete();
            }
            else
            {
                //▼====: #003
                if (PCLResultParamImpID != 0)
                {
                    string PCLResultParamImpIDStr = "|" + PCLResultParamImpID + "|";
                    StaffCatgs = Globals.AllStaffs.Where(o => VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower())
                                    && o.ListDeptResponsibilities != null && (o.ListDeptResponsibilities.Contains(CurrentDeptID.ToString()) || CurrentDeptID == 0)
                                    && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)
                                    && o.ListPCLResultParamImpID != null && o.ListPCLResultParamImpID.Contains(PCLResultParamImpIDStr)).ToObservableCollection();
                }
                else
                {
                    if (!IsDoctorOnly)
                    {
                       
                        if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
                        {
                            StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && o.RefStaffCategory.V_StaffCatType == StaffCatType
                                            && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                            && o.ListDeptResponsibilities != null && ((o.ListDeptResponsibilities.Contains(CurrentDeptID.ToString()) || CurrentDeptID == 0))
                                            && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                        }
                        else
                        {
                            StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && o.RefStaffCategory.V_StaffCatType == StaffCatType
                                                && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                                && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                        }
                    }
                    else
                    {
                        if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
                        {
                            StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && o.RefStaffCategory.V_StaffCatType == StaffCatType
                                            && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                            && (VNConvertString.ConvertString(o.RefStaffCategory.StaffCatgDescription).ToLower().Contains("bs"))
                                            && o.ListDeptResponsibilities != null && ((o.ListDeptResponsibilities.Contains(CurrentDeptID.ToString()) || CurrentDeptID == 0))
                                            && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                        }
                        else
                        {
                            StaffCatgs = Globals.AllStaffs.Where(o => o.RefStaffCategory != null && o.RefStaffCategory.V_StaffCatType == StaffCatType
                                                && (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower()))
                                                && (VNConvertString.ConvertString(o.RefStaffCategory.StaffCatgDescription).ToLower().Contains("bs"))
                                                && ((!ShowStopUsing && !o.IsStopUsing) || ShowStopUsing)).ToObservableCollection();
                        }
                    }
                }
                //▲====: #003
                NotifyOfPropertyChange(() => StaffCatgs);
                aucHoldConsultDoctor.ItemsSource = StaffCatgs;
                aucHoldConsultDoctor.PopulateComplete();
                //if (StaffCatType == (long)V_StaffCatType.NhanVienQuayDangKy)
                //{
                //    if (Globals.AllRegisStaff != null && Globals.AllRegisStaff.Count > 0)
                //    {
                //        StaffCatgs = Globals.AllRegisStaff.Where(o => (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower())
                //            //|| o.FullName.ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower())
                //            )).ToObservableCollection();

                //        NotifyOfPropertyChange(() => StaffCatgs);
                //        aucHoldConsultDoctor.ItemsSource = StaffCatgs;
                //        aucHoldConsultDoctor.PopulateComplete();

                //    }
                //    else
                //    {
                //        this.SearchStaffCatgs(e.Parameter);
                //    }
                //}

                //if (StaffCatType == (long)V_StaffCatType.BacSi)
                //{
                //    if (Globals.AllDoctorStaff != null
                //    && Globals.AllDoctorStaff.Count > 0)
                //    {
                //        StaffCatgs = Globals.AllDoctorStaff.Where(o => (VNConvertString.ConvertString(o.FullName).ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower())
                //            //|| o.FullName.ToLower().Contains(VNConvertString.ConvertString(e.Parameter).ToLower())
                //            )).ToObservableCollection();

                //        NotifyOfPropertyChange(() => StaffCatgs);
                //        aucHoldConsultDoctor.ItemsSource = StaffCatgs;
                //        aucHoldConsultDoctor.PopulateComplete();

                //    }
                //    else
                //    {
                //        this.SearchStaffCatgs(e.Parameter);
                //    }
                //}
            }
        }

        public void aucHoldConsultDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (aucHoldConsultDoctor.SelectedItem != null)
            {
                StaffID = (aucHoldConsultDoctor.SelectedItem as Staff).StaffID;
                StaffName = (aucHoldConsultDoctor.SelectedItem as Staff).FullName;
                /*▼====: #001*/
                CertificateNumber = (aucHoldConsultDoctor.SelectedItem as Staff).SCertificateNumber;
                /*▲====: #001*/
            }
            else
            {
                StaffID = 0;
                StaffName = "";
            }
        }

        /*▼====: #001*/
        private string _CertificateNumber;
        public string CertificateNumber
        {
            get { return _CertificateNumber; }
            set
            {
                _CertificateNumber = value;
                NotifyOfPropertyChange(() => CertificateNumber);
            }
        }
        /*▲====: #001*/

        //▼====: #003
        private long _PCLResultParamImpID = 0;
        public long PCLResultParamImpID
        {
            get { return _PCLResultParamImpID; }
            set
            {
                _PCLResultParamImpID = value;
                NotifyOfPropertyChange(() => PCLResultParamImpID);
            }
        }
        //▲====: #003
        #endregion
    }
}
