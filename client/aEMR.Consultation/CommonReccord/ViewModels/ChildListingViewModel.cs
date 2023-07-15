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
using aEMR.DataContracts;
using System.ServiceModel;
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(IChildListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChildListingViewModel : ViewModelBase, IChildListing
        , IHandle<InPatientRegistrationSelectedForConsultation>
        , IHandle<BirthCertificates_Event_Save>
        , IHandle<UpdateCompleted<Patient>>
        , IHandle<RegistrationSelectedForConsultation_K1>
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
        public IPatientInfo UCPatientProfileInfo { get; set; }
        [ImportingConstructor]
        public ChildListingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _searchCriteria = new PatientSearchCriteria();
            _searchRegCriteria = new SeachPtRegistrationCriteria();
            //Khi khoi tao module thi load menu ben trai luon.
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);
            authorization();

            _ListBirthCertificates = new ObservableCollection<BirthCertificates>();
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(UCPatientProfileInfo);
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
        AxSearchPatientTextBox SearchPatientTextBox;
        public void SearchPatientTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SearchPatientTextBox = (AxSearchPatientTextBox)sender;
            SearchPatientTextBox.Focus();
        }
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
        private ObservableCollection<BirthCertificates> _ListBirthCertificates;
        public ObservableCollection<BirthCertificates> ListBirthCertificates
        {
            get { return _ListBirthCertificates; }
            set
            {
                _ListBirthCertificates = value;
                NotifyOfPropertyChange(() => ListBirthCertificates);
            }
        }

        #endregion
        private BirthCertificates _CurrentBirthCertificates = new BirthCertificates();
        public BirthCertificates CurrentBirthCertificates
        {
            get
            {
                return _CurrentBirthCertificates;
            }
            set
            {
                _CurrentBirthCertificates = value;
                NotifyOfPropertyChange(() => CurrentBirthCertificates);
            }
        }
        public void InitValueBeforeSave()
        {
            if (String.IsNullOrEmpty(Registration_DataStorage.CurrentPatient.IDNumber))
            {
                MessageBox.Show("Bệnh nhân không có giấy chứng minh");
                return;
            }
            if (Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == UCPatientProfileInfo.CurrentRegistration.PtRegistrationID)
            {
                MessageBox.Show("Thông tin mẹ và con trùng nhau");
                return;
            }
            if (!CheckPtRegistrationID_Child())
            {
                MessageBox.Show("Đã tồn tại thông tin con trong danh sách");
                return;
            }

            CurrentBirthCertificates.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            CurrentBirthCertificates.V_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
            CurrentBirthCertificates.PtRegistrationID_Child = UCPatientProfileInfo.CurrentRegistration.PtRegistrationID;
            CurrentBirthCertificates.BirthDate = UCPatientProfileInfo.CurrentRegistration.ExamDate;
            CurrentBirthCertificates.IsDelete = false;
            Action<IChildListingEdit> onInitDlg = delegate (IChildListingEdit proAlloc)
            {
                proAlloc.CurrentBirthCertificates = CurrentBirthCertificates;
            };
            GlobalsNAV.ShowDialog<IChildListingEdit>(onInitDlg, null, false, false, null);
            GetBirthCertificates_ByPtRegID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU);
        }
        private bool CheckPtRegistrationID_Child()
        {
            foreach (var item in ListBirthCertificates)
            {
                if(item.PtRegistrationID_Child == UCPatientProfileInfo.CurrentRegistration.PtRegistrationID)
                {
                    return false;
                }
            }
            return true;
        }
        public void btnNew()
        {
            //SaveBirthCertificates(false);
            InitValueBeforeSave();
           
        }
        private void SaveBirthCertificates()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveBirthCertificates(CurrentBirthCertificates, (long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime().ToString("ddMMyyyy"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool result = contract.EndSaveBirthCertificates(asyncResult);
                            if (result)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                            }
                            GetBirthCertificates_ByPtRegID(CurrentBirthCertificates.PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU);
                            this.HideBusyIndicator();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void hplDelete_Click(object selectedItem)
        {
            if (MessageBox.Show("Bạn có muốn xóa không?", "Thông báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                CurrentBirthCertificates = selectedItem as BirthCertificates;
                CurrentBirthCertificates.IsDelete = true;
                SaveBirthCertificates();
            }
        }
        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                BirthCertificates SelectedBirthCertificates = ObjectCopier.DeepCopy((selectedItem as BirthCertificates));
                Action<IChildListingEdit> onInitDlg = delegate (IChildListingEdit proAlloc)
                {
                    proAlloc.CurrentBirthCertificates = SelectedBirthCertificates;
                };
                GlobalsNAV.ShowDialog<IChildListingEdit>(onInitDlg, null, false, false, null);
                GetBirthCertificates_ByPtRegID(SelectedBirthCertificates.PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU);
            }
        }
        public void hplPreview_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.eItem = ReportName.XRpt_GiayChungSinh;
                    proAlloc.ID = ObjectCopier.DeepCopy((selectedItem as BirthCertificates)).BirthCertificateID;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }

        public void grdCommonRecord_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        #region method
        private void GetBirthCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetBirthCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int tempCode;
                                var result = contract.EndGetBirthCertificates_ByPtRegID(out tempCode, asyncResult);
                                if (result != null)
                                {
                                    ListBirthCertificates = new ObservableCollection<BirthCertificates>();
                                    ListBirthCertificates = result.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //this.HideBusyIndicator();
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
        public void Handle(RegistrationSelectedForConsultation_K1 message)
        {
            _ListBirthCertificates = new ObservableCollection<BirthCertificates>();

            if (message.Source != null && message.Source.PtRegistrationID > 0)
            {
                GetBirthCertificates_ByPtRegID(message.Source.PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU);
            }
        }
        public void Handle(InPatientRegistrationSelectedForConsultation message)
        {

            //UCPatientProfileInfo.CurrentPatient = new Patient();
            //UCPatientProfileInfo.CurrentRegistration = new PatientRegistration();
            //UCPatientProfileInfo = null;
            _ListBirthCertificates = new ObservableCollection<BirthCertificates>();

            if (message.Source != null && message.Source.PtRegistrationID > 0)
            {
               GetBirthCertificates_ByPtRegID(message.Source.PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU);
            }
        }
        public void Handle(BirthCertificates_Event_Save message)
        {
            _ListBirthCertificates = new ObservableCollection<BirthCertificates>();
            GetBirthCertificates_ByPtRegID(message.Result.PtRegistrationID, (long)AllLookupValues.RegistrationType.NOI_TRU);
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
        private SeachPtRegistrationCriteria _searchRegCriteria;
        public SeachPtRegistrationCriteria SearchRegCriteria
        {
            get { return _searchRegCriteria; }
            set
            {
                _searchRegCriteria = value;
                NotifyOfPropertyChange(() => SearchRegCriteria);
            }
        }
        private PatientSearchCriteria _searchCriteria;
        public PatientSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }
        private DateTime _curDate;
        public DateTime curDate
        {
            get { return _curDate; }
            set
            {
                _curDate = value;
                NotifyOfPropertyChange(() => curDate);
            }
        }
        public void SearchRegistrationCmd()
        {
            //▼====== #005: 20180906 TTM: Cần xem lại, chỗ này chỉ làm tạm thôi, cần phải fix lại vì không thể làm mỗi chỗ 1 ít mà nên tạo method mới để gọi => sau này chỉnh sửa dễ hơn.
            curDate = Globals.GetCurServerDateTime().Date;
            _searchRegCriteria = new SeachPtRegistrationCriteria();
            //_searchRegCriteria.IsSearchByRegistrationDetails = IsSearchByRegistrationDetails;
            //_searchRegCriteria.IsSearchOnlyProcedure = IsSearchOnlyProcedure;
            //if (IsSearchGoToKhamBenh)
            //{
            //    _searchRegCriteria.KhamBenh = true;
            //}
            _searchRegCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU; //20180907 TTM: Thiếu dẫn đến lỗi không lấy đc bệnh nhân trong màn hình xuất cho bệnh nhân khoa nội trú
            if (SearchPatientTextBox != null && String.IsNullOrEmpty(SearchPatientTextBox.Text))
            {

                //_searchRegCriteria.FromDate = curDate.AddDays(-30);
                //_searchRegCriteria.ToDate = curDate;
                //_searchRegCriteria.IsDischarge = false;

                //Action<IFindRegistrationInPt> onInitDlg = delegate (IFindRegistrationInPt vm)
                //{
                //        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                //        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                //        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                //    vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                //        //▲====== #006
                //        vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                //        //▼===== #014
                //        vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                //        //▲===== #014
                //        if (SearchAdmittedInPtRegOnly != null)
                //    {
                //        vm.SetValueForDischargedStatus((bool)SearchAdmittedInPtRegOnly);
                //    }
                //    vm.SearchCriteria = SearchRegCriteria;
                //    vm.IsPopup = true;
                //    vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                //    vm.LeftModule = LeftModule;
                //    vm.CanSearhRegAllDept = CanSearhRegAllDept;
                //    vm.LoadRefDeparments();
                //    vm.IsProcedureEdit = IsProcedureEdit;
                //    if (IsSearchGoToKhamBenh)
                //    {
                //        vm.CloseAfterSelection = true;
                //    }
                //    else
                //    {
                //        vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                //    }
                //};
                //GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);


                //return;
            }
            else
            {


                _searchRegCriteria.FullName = _searchCriteria.FullName;
                _searchRegCriteria.HICard = _searchCriteria.InsuranceCard;
                _searchRegCriteria.PatientCode = _searchCriteria.PatientCode;
                _searchRegCriteria.PatientNameString = _searchCriteria.PatientNameString;
                _searchRegCriteria.PtRegistrationCode = _searchCriteria.PtRegistrationCode;
                _searchRegCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                //▼====== #006: Nếu như cấu hình là hạn chế tìm tên BN thì cần chặn trước khi gọi Begin và End để đỡ mất thời gian
                if (!Globals.ServerConfigSection.ConsultationElements.IsAllowSearchingPtByName
                    && !String.IsNullOrEmpty(_searchRegCriteria.FullName)
                    && false)
                {
                    MessageBox.Show(eHCMSResources.Z2304_G1_KhongTheTimDKBangTen);
                    return;
                }
                //▲====== #006

                //if (IsSearchGoToKhamBenh)
                //{
                //    _searchRegCriteria.KhamBenh = true;
                //}
                _searchRegCriteria.FromDate = curDate.AddDays(-30);
                _searchRegCriteria.ToDate = curDate;
                _searchRegCriteria.IsAdmission = true;



                SearchRegistrationsInPt(0, 10, true);


            }
        }
        private void SearchRegistrationsInPt(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator();
            _searchRegCriteria.IsDischarge = false;
            if (Globals.isAccountCheck)
            {
                if (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count <= 0)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1092_G1_ChuaPhanQuyenTrachNhiemKh), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }

                if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(Globals.ObjRefDepartment.DeptID))
                {
                    _searchRegCriteria.DeptID = Globals.ObjRefDepartment.DeptID;
                }
                else
                {
                    _searchRegCriteria.DeptID = Globals.LoggedUserAccount.DeptIDResponsibilityList[0];
                }
            }
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        //kiem tra co tim theo gia tri trong text box ko
                        client.BeginSearchRegistrationsInPt(_searchRegCriteria, pageIndex, pageSize, bCountTotal, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrationsInPt(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                            if (bOK)
                            {
                                if (allItems == null || allItems.Count == 0)
                                {
                                    Globals.EventAggregator.Publish(new ResultNotFound<PatientRegistration>()
                                    {
                                        Message = string.Format("{0}.", eHCMSResources.Z0083_G1_KhongTimThayDK),
                                        SearchCriteria = _searchRegCriteria
                                    });
                                    MessageBoxResult result = MessageBox.Show(eHCMSResources.A0734_G1_Msg_ConfTiepTucTimDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                {
                                    UCPatientProfileInfo.CurrentRegistration = allItems[0];
                                    UCPatientProfileInfo.CurrentPatient = allItems[0].Patient;
                                    CurrentBirthCertificates = new BirthCertificates();
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    this.HideBusyIndicator();
                    //IsSearchingRegistration = false;
                }
                if (error != null)
                {
                    //Globals.HideBusy();
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        public void Handle(UpdateCompleted<Patient> message)
        {
            if (message != null && message.Item != null && Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                Registration_DataStorage.CurrentPatient.IDNumber = message.Item.IDNumber;
            }
        }
    }
}