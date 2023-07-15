using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
/*
* 20181023 #001 TTM:   BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.    
* 20200121 #002 TBL:   BM 0021818: Chỉnh sửa lại cách nhập dữ liệu màn hình Theo dõi sinh hiệu - Nội trúx
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(INutritionalRating)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class NutritionalRatingViewModel : ViewModelBase, INutritionalRating
        , IHandle<ShowInPatientInfoForConsultation>
        , IHandle<NutritionalRating_Event_Save>
    {
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public NutritionalRatingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Khi khoi tao module thi load menu ben trai luon.
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);
            authorization();
       
            _ListNutritionalRating = new ObservableCollection<NutritionalRating>();
        }
   
        protected override void OnActivate()
        {
            base.OnActivate();
        }
        public void grdCommonRecordLoaded(object sender, RoutedEventArgs e)
        {
            grdCommonRecord = sender as AxDataGridEx;
            if (!mTongQuat_XemThongTin)
            {
                grdCommonRecord.IsReadOnly = true;
            }
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                grdCommonRecord.IsEnabled = true;
            }
            else
            {
                grdCommonRecord.IsEnabled = false;
            }
        }
        public AxDataGridEx grdCommonRecord { get; set; }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }


            mTongQuat_XemThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_XemThongTin, (int)ePermission.mView);
            mTongQuat_ChinhSuaThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_ChinhSuaThongTin, (int)ePermission.mView);
        }

        #region account checking

        private bool _mTongQuat_XemThongTin = true;
        private bool _mTongQuat_ChinhSuaThongTin = true && Globals.isConsultationStateEdit;

        public bool mTongQuat_XemThongTin
        {
            get
            {
                return _mTongQuat_XemThongTin;
            }
            set
            {
                if (_mTongQuat_XemThongTin == value)
                    return;
                _mTongQuat_XemThongTin = value;
            }
        }
        public bool mTongQuat_ChinhSuaThongTin
        {
            get
            {
                return _mTongQuat_ChinhSuaThongTin;
            }
            set
            {
                if (_mTongQuat_ChinhSuaThongTin == value)
                    return;
                _mTongQuat_ChinhSuaThongTin = value && Globals.isConsultationStateEdit;
            }
        }

        #endregion
        #region binding visibilty



        #endregion
        #region property
        private ObservableCollection<NutritionalRating> _ListNutritionalRating;
        public ObservableCollection<NutritionalRating> ListNutritionalRating
        {
            get { return _ListNutritionalRating; }
            set
            {
                _ListNutritionalRating = value;
                NotifyOfPropertyChange(() => ListNutritionalRating);
            }
        }

        #endregion

        public void btnNew()
        {
            if(Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0)
            {
                return;
            }
            void onInitDlg(INutritionalRating_AddEdit typeInfo)
            {
                typeInfo.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                //typeInfo.IsBMIBelow205 = Registration_DataStorage.Cure
                if (Registration_DataStorage.PtPhyExamList != null && (Registration_DataStorage.PtPhyExamList.Count() > 0)
                    && Registration_DataStorage.PtPhyExamList.LastOrDefault().BMI < 20.5)
                {
                    typeInfo.IsBMIBelow205 = true;
                }
                else
                {
                    typeInfo.IsBMIBelow205 = false;
                }
                typeInfo.InitializeNewItem();
            }
            GlobalsNAV.ShowDialog<INutritionalRating_AddEdit>(onInitDlg, null, false, true, Globals.GetTwoFourthWidthDefaultDialogViewSize());
        }
     
        public void hplDelete_Click(object selectedItem)
        {
           if( MessageBox.Show("Bạn có muốn xóa không?", "Thông báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeleteNutritionalRating(ObjectCopier.DeepCopy((selectedItem as NutritionalRating)).NutritionalRatingID);
            }
        }
        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                void onInitDlg(INutritionalRating_AddEdit typeInfo)
                {
                    typeInfo.CurrentNutritionalRating = ObjectCopier.DeepCopy((selectedItem as NutritionalRating));
                }
                GlobalsNAV.ShowDialog<INutritionalRating_AddEdit>(onInitDlg, null, false, true, Globals.GetTwoFourthWidthDefaultDialogViewSize());
            }
        }
        public void hplPreview_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.eItem = ReportName.XRpt_NutritionalRating;
                    proAlloc.NutritionalRatingID = ObjectCopier.DeepCopy((selectedItem as NutritionalRating)).NutritionalRatingID;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
         
        public void grdCommonRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
   
        #region method
        private void GetNutritionalRating(long PtRegistrationID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetNutritionalRatingByPtRegistrationID(PtRegistrationID,Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    ListNutritionalRating = new ObservableCollection<NutritionalRating>();
                                    var items = contract.EndGetNutritionalRatingByPtRegistrationID(asyncResult);
                                    if (items != null && items.Count > 0)
                                    {
                                        foreach (var tp in items)
                                        {
                                            ListNutritionalRating.Add(tp);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
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
        private void DeleteNutritionalRating(long NutritionalRatingID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteNutritionalRating(NutritionalRatingID, (long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime(),
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListNutritionalRating = new ObservableCollection<NutritionalRating>();
                                var bOK = contract.EndDeleteNutritionalRating(asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show("Xóa thành công thành công");
                                    Globals.EventAggregator.Publish(new NutritionalRating_Event_Save() { Result = true });
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
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
        #endregion

        #region Handle
        public void Handle(ShowInPatientInfoForConsultation message)
        {
            _ListNutritionalRating = new ObservableCollection<NutritionalRating>();

            if (message.PtRegistration != null && message.PtRegistration.PtRegistrationID > 0)
            {
                GetNutritionalRating(message.PtRegistration.PtRegistrationID);
            }
        }
        public void Handle(NutritionalRating_Event_Save message)
        {
            _ListNutritionalRating = new ObservableCollection<NutritionalRating>();
            GetNutritionalRating(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
        }
        #endregion
        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
    }
}