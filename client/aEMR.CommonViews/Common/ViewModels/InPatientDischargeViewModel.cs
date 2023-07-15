using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
using DataEntities;
using Service.Core.Common;
using eHCMSCommon.Utilities;
using System.Text;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using aEMR.ViewContracts.Configuration;
using aEMR.Common.BaseModel;
/*
* 20170927 #001 CMN: Added DeadReason
* 20180424 #002 CMN: Prevented Users which not Administrator could revert discharged date when registration was reported
* 20181121 #003 TTM: BM 0005257: Thêm mới out standing task nội trú, tạo sự kiện để load lại thông tin (Do việc load lại thông tin chỉ xảy ra khi chuyển đổi màn hình, hiện tại chọn thông tin
*                    load tại màn hình nên cần sự kiện load lại.      
* 20191127 #004 TBL: BM 0019662: So sánh theo thời gian xác nhận xuất viện với thời gian ra chẩn đoán xác nhận và cấu hình
* 20200708 #005 TTM: BM 0039309: Lỗi không tự động lấy chẩn đoán cuối cùng làm chẩn đoán xuất viện ở màn hình ra toa và màn hình lưu thông tin xuất viện
* 20200803 #006 TTM: BM 0040427: Lỗi không load lại dữ liệu sau khi huỷ xuất viện dẫn đến không cho mở popup xác nhận chẩn đoán lại sau khi huỷ xuất viện.
* 20200820 #007 TTM: BM 0040440: Lỗi không clear MH khi load BN mới ở màn hình xuất viện bệnh nhân.
* 20220711 #008 DatTB: 
* + Thay đổi trạng thái khi gửi lần 2.
* + Màn hình nội trú thêm điều kiện gửi lần 2.
* 20220717 #009 DatTB: Refactor code, thêm khóa nút gửi hồ sơ
* 20220823 #010 BLQ: thêm điều kiện kiểm tra khi chọn lưu thông tin xuất viện
* 20220909 #011 DatTB: Thêm event load "Lời dặn" qua tab Toa thuốc xuất viện
* 20221019 #012 DatTB:
* + Ràng buộc mở/khóa các nút Lưu TT XV, Hủy XV, Gửi hồ sơ theo quy trình
* + Kiểm tra danh mục dịch vụ trước khi gửi kiểm duyệt hồ sơ
* 20221114 #013 DatTB: Thêm biến để nhận biết gửi HSBA so với điều dưỡng xác nhận xuất viện ra viện.
* 20230418 #014 QTD: Xem/In giấy ra viện mới
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDischargeNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientDischargeViewModel : ViewModelBase, IDischargeNew
        , IHandle<ResultFound<Patient>>
        , IHandle<ItemSelected<Patient>>
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<BedPatientDischarge>
        , IHandle<ReturnItem<BedPatientAllocs, object>>
        , IHandle<SetInPatientInfoAndRegistrationForInPatientDischarge>
        , IHandle<LoadDiagnosisTreatmentConfirmedForPrescript>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientDischargeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            eventArg.Subscribe(this);
            //var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            //searchPatientAndRegVm.mTimBN = mXuatVien_Patient_TimBN;
            //searchPatientAndRegVm.mThemBN = mXuatVien_Patient_ThemBN;
            //searchPatientAndRegVm.mTimDangKy = mXuatVien_Patient_TimDangKy;
            //searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            //searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            //searchPatientAndRegVm.PatientFindByVisibility = false;
            //searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            //searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            //SearchRegistrationContent = searchPatientAndRegVm;
            //ActivateItem(searchPatientAndRegVm);

            //var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            //patientInfoVm.mInfo_CapNhatThongTinBN = mXuatVien_Info_CapNhatThongTinBN;
            //patientInfoVm.mInfo_XacNhan = mXuatVien_Info_XacNhan;
            //patientInfoVm.mInfo_XoaThe = mXuatVien_Info_XoaThe;
            //patientInfoVm.mInfo_XemPhongKham = mXuatVien_Info_XemPhongKham;
            //PatientSummaryInfoContent = patientInfoVm;
            //ActivateItem(patientInfoVm);

            //DischargeInfoContent = Globals.GetViewModel<IDischargeInfo>();

            //KMx: Cấu hình của view này sai rồi, khi nào có thời gian thì làm cấu hình lại.
            //Lưu ý: View này có 2 link (Xuất viện, Xuất viện (bác sĩ)), phải check operation ở LeftMenu rồi truyền vào, không phải check trong view này (23/01/2015 17:43).
            //authorization();

        }

        public void InitView(bool usedInConsultationModule)
        {
            if (!usedInConsultationModule)
            {
                var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
                searchPatientAndRegVm.mTimBN = mXuatVien_Patient_TimBN;
                searchPatientAndRegVm.mThemBN = mXuatVien_Patient_ThemBN;
                searchPatientAndRegVm.mTimDangKy = mXuatVien_Patient_TimDangKy;
                searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
                searchPatientAndRegVm.PatientFindByVisibility = false;
                searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

                SearchRegistrationContent = searchPatientAndRegVm;
                ActivateItem(searchPatientAndRegVm);
            }

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mXuatVien_Info_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mXuatVien_Info_XacNhan;
            patientInfoVm.mInfo_XoaThe = mXuatVien_Info_XoaThe;
            patientInfoVm.mInfo_XemPhongKham = false;
            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            DischargeInfoContent = Globals.GetViewModel<IDischargeInfo>();
            DischargeInfoContent.IsConsultation = IsConsultation;

            if (!Globals.isAccountCheck || IsConsultation)
            {
                ShowRevertDischargeBtn = true;
            }

            if (IsConsultation && Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
            {
                long curInPtRegisID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                OpenRegistration(curInPtRegisID);
            }

        }

        private bool _enableSearchAllDepts = false;
        public bool EnableSearchAllDepts
        {
            get { return _enableSearchAllDepts; }
            set
            {
                _enableSearchAllDepts = value;
                if (SearchRegistrationContent != null)
                {
                    SearchRegistrationContent.CanSearhRegAllDept = _enableSearchAllDepts;
                }
            }
        }


        private AllLookupValues.PatientFindBy _PatientFindBy;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                _PatientFindBy = value;
                NotifyOfPropertyChange(() => PatientFindBy);
            }
        }



        protected override void OnActivate()
        {
            base.OnActivate();

            // TxD 22/06/2017: Moved all the following to Constructor and InitView because sometimes OnActivate is not invoked by Caliburn Micro (could be Caliburn's bug)
            //Globals.EventAggregator.Subscribe(this);
            //if (IsConsultation && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
            //{
            //    long curInPtRegisID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            //    OpenRegistration(curInPtRegisID);
            //}
            //▼====== #003
            if (Globals.LeftModuleActive == LeftModuleActive.XUATVIEN)
            {
                var homevm = Globals.GetViewModel<IHome>();
                IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
                ostvm.WhichVM = SetOutStandingTask.XUATVIEN;
                homevm.OutstandingTaskContent = ostvm;
                homevm.IsExpandOST = true;
            }
            //▲====== #003
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
            //▼====== #003
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
            //▲====== #003
        }

        #region private primative

        private string _DeptLocTitle;
        public string DeptLocTitle
        {
            get
            {
                return _DeptLocTitle;
            }
            set
            {
                _DeptLocTitle = value;
                NotifyOfPropertyChange(() => DeptLocTitle);
            }
        }

        private ISearchPatientAndRegistration _searchRegistrationContent;

        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        private IPatientSummaryInfoV2 _patientSummaryInfoContent;

        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _patientSummaryInfoContent; }
            set
            {
                _patientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }

        private IDischargeInfo _dischargeInfoContent;
        public IDischargeInfo DischargeInfoContent
        {
            get { return _dischargeInfoContent; }
            set
            {
                _dischargeInfoContent = value;
                NotifyOfPropertyChange(() => DischargeInfoContent);
            }
        }

        private bool _registrationLoading;

        public bool RegistrationLoading
        {
            get
            { return _registrationLoading; }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);
            }
        }

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
                PatientSummaryInfoContent.CurrentPatient = _currentPatient;
            }
        }

        private PatientRegistration _currentRegistration;

        public PatientRegistration CurrentRegistration
        {
            get { return _currentRegistration; }
            set
            {
                _currentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);

                //NotifyOfPropertyChange(() => CanSaveDischargeCmd);
                //NotifyOfPropertyChange(() => CanConfirmDischargeCmd);
                if (_currentRegistration != null)
                {
                    List<RefDepartment> listDepts = new List<RefDepartment>();
                    //bool bAlreadyAdded = false;
                    foreach (var inDeptItem in _currentRegistration.AdmissionInfo.InPatientDeptDetails)
                    {
                        RefDepartment refDept = new RefDepartment();
                        refDept = Globals.AllRefDepartmentList.Where(item => (item.DeptID == inDeptItem.DeptLocation.DeptID)).FirstOrDefault();

                        if (refDept != null && !listDepts.Any(x => x.DeptID == refDept.DeptID))
                        {
                            listDepts.Add(refDept);
                        }

                        //KMx: Code bên dưới bị lỗi trong trường hợp: BN nhập vào khoa NTM, B, B thì listDepts chỉ add khoa B mà không add khoa NTM vào combobox cho user chọn khoa xuất viện (28/10/2015 16:56).
                        //foreach (var addedDept in listDepts)
                        //{
                        //    if (addedDept.DeptID == refDept.DeptID)
                        //    {
                        //        bAlreadyAdded = true;
                        //        break;
                        //    }
                        //}
                        //if (!bAlreadyAdded)
                        //{
                        //    listDepts.Add(refDept);
                        //}


                    }
                    if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Count() == 0) // Must be Administrator account otherwise review here
                    {
                        EnableSearchAllDepts = true;
                    }
                    DischargeInfoContent.SetDischargeDeptSelection(listDepts, EnableSearchAllDepts);
                }
                else
                {
                    DischargeInfoContent.SetDischargeDeptSelection(null, false);
                }
                PatientSummaryInfoContent.CurrentPatientRegistration = CurrentRegistration;
            }
        }

        private bool _patientLoading;
        public bool PatientLoading
        {
            get { return _patientLoading; }
            set
            {
                _patientLoading = value;
                NotifyOfPropertyChange(() => PatientLoading);
            }
        }

        private string _billMessage;
        public string billMessage
        {
            get { return _billMessage; }
            set
            {
                _billMessage = value;
                NotifyOfPropertyChange(() => billMessage);
            }
        }

        #endregion
        public void ResetForm()
        {
            CurrentRegistration = null;
            CurrentDiagnosisTreatment = null;
            DiagnosisTreatmentCollection = null;
            DischargeInfoContent.Registration = null;
            CurrentPatient = null;

            PatientSummaryInfoContent.SetPatientHISumInfo(null);

            PatientSummaryInfoContent.CurrentPatient = null;
            PatientSummaryInfoContent.CurrentPatientClassification = null;
            //DischargeInfoContent.DischargeDate = null;
            DischargeInfoContent.DeceasedInfo = null;

        }
        #region Handle
        public void Handle(BedPatientDischarge message)
        {
            if (message != null)
            {
                //Xuat vien cho benh nhan nay luon
                Discharge();
            }
        }

        public void Handle(ResultFound<Patient> message)
        {
            if (this.GetView() != null && message != null)
            {
                ResetForm();

                CurrentPatient = message.Result;
                if (CurrentPatient != null)
                {
                    SetCurrentPatient(CurrentPatient);
                    //Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = CurPatient });
                }
            }
        }

        public void Handle(ItemSelected<Patient> message)
        {
            if (this.GetView() != null && message != null)
            {
                ResetForm();

                CurrentPatient = message.Item;
                if (CurrentPatient != null)
                {
                    SetCurrentPatient(CurrentPatient);
                }
            }
        }

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (GetView() != null && message != null && message.Item != null)
            {
                ResetForm();

                OpenRegistration(message.Item.PtRegistrationID);
            }
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (this.GetView() != null && message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0478_G1_KhongTimThayBenhNhan),
                                                          "eHCMS says", MessageBoxButton.OK);
            }
        }

        public void Handle(ReturnItem<BedPatientAllocs, object> message)
        {
            if (this.GetView() != null && CurrentRegistration != null)
            {
                if (message.Item.BedPatientID > 0 && message.Item.CheckOutDate == null)
                {
                    if (MessageBox.Show(eHCMSResources.A0151_G1_Msg_ConfTraGiuong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        ReturnBedPatientAllocs(message.Item.BedPatientID);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0714_G1_Msg_InfoTraGiuongFail));
                }
            }
        }

        private void ReturnBedPatientAllocs(long BedPatientID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteBedPatientAllocs(BedPatientID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteBedPatientAllocs(asyncResult);
                            if (results == true)
                            {
                                MessageBox.Show(eHCMSResources.K0220_G1_TraGiuongOk);
                                if (CurrentRegistration != null)
                                {
                                    OpenRegistration(CurrentRegistration.PtRegistrationID);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        #endregion

        #region load bill

        private IInPatientBillingInvoiceDetailsListing _editingInvoiceDetailsContent;
        public IInPatientBillingInvoiceDetailsListing EditingInvoiceDetailsContent
        {
            get { return _editingInvoiceDetailsContent; }
            set
            {
                _editingInvoiceDetailsContent = value;
                NotifyOfPropertyChange(() => EditingInvoiceDetailsContent);
            }
        }
        private InPatientBillingInvoice _editingBillingInvoice;
        public InPatientBillingInvoice EditingBillingInvoice
        {
            get
            {
                return _editingBillingInvoice;
            }
            set
            {
                if (_editingBillingInvoice != value)
                {
                    _editingBillingInvoice = value;
                    NotifyOfPropertyChange(() => EditingBillingInvoice);
                    //NotifyOfPropertyChange(() => EditingBillingInvoiceTitle);
                    EditingInvoiceDetailsContent.BillingInvoice = _editingBillingInvoice;
                    EditingInvoiceDetailsContent.ResetView();
                }
            }
        }

        private bool CheckIfInvoiceIsEmpty(InPatientBillingInvoice inv)
        {
            if (inv.RegistrationDetails != null && inv.RegistrationDetails.Count > 0)
            {
                bool flag = true;
                int count = 0;
                if (inv.PclRequests != null && inv.PclRequests.Count > 0)
                {
                    if (inv.PclRequests.Any(req => req.PatientPCLRequestIndicators != null && req.PatientPCLRequestIndicators.Count > 0))
                    {
                        return false;
                    }
                }
                if (inv.OutwardDrugClinicDeptInvoices != null && inv.OutwardDrugClinicDeptInvoices.Count > 0)
                {
                    return inv.OutwardDrugClinicDeptInvoices.All(drugInv => drugInv.OutwardDrugClinicDepts == null || drugInv.OutwardDrugClinicDepts.Count <= 0);
                }

                foreach (var item in inv.RegistrationDetails)
                {
                    if (item.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes
                        == (long)AllLookupValues.V_RefMedicalServiceTypes.GIUONGBENH)
                    {
                        count++;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                if (count > 0
                    && MessageBox.Show(count.ToString() + string.Format(" {0}", eHCMSResources.K2731_G1_DVGBenhChuaLamBill), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
                return flag;
            }

            return true;
        }
        //public void LoadBillCmd(ManualResetEvent autoResetEvent)
        //{
        //    var t = new Thread(() =>
        //    {
        //        AxErrorEventArgs error = null;
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginLoadInPatientRegItemsIntoBill(CurrentRegistration.PtRegistrationID, null,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            var inv = contract.EndLoadInPatientRegItemsIntoBill(asyncResult);
        //                            if (inv != null)
        //                            {
        //                                CheckIfInvoiceIsEmpty(inv);
        //                            }
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {
        //                            error = new AxErrorEventArgs(fault);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            error = new AxErrorEventArgs(ex);
        //                        }
        //                        finally
        //                        {
        //                            Globals.IsBusy = false;
        //                        }

        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            error = new AxErrorEventArgs(ex);
        //        }
        //        finally
        //        {
        //            if (error != null)
        //            {
        //                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
        //            }
        //        }

        //    });
        //    t.Start();
        //}
        #endregion
        public void SetCurrentPatient(object patient)
        {
            Patient p = patient as Patient;
            if (p == null)
            {
                return;
            }
            if (p.PatientID <= 0)
            {
                CurrentPatient = null;
            }

            PatientSummaryInfoContent.SetPatientHISumInfo(null);

            if (p.PatientID > 0)
            {
                GetPatientByID(p.PatientID);
            }
        }

        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBillingInvoices = true;
            LoadRegisSwitch.IsGetBedAllocations = true;
            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(1)" });
            }
            else
            {
                if (loadRegTask.Registration.AdmissionInfo == null || loadRegTask.Registration.AdmissionInfo.InPatientAdmDisDetailID <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0217_G1_Msg_InfoBNChuaVien_KhTheXV);
                    yield break;
                }
                CurrentRegistration = loadRegTask.Registration;
                DischargeDateFromInPtRec = loadRegTask.Registration.AdmissionInfo.DischargeDate;
                ShowOldRegistration(CurrentRegistration);
                yield return GenericCoRoutineTask.StartTask(GetPaymentInfo_Action);
            }
        }

        public void OpenRegistration(long regID)
        {
            RegistrationLoading = true;
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) => { RegistrationLoading = false; });
        }

        public IEnumerator<IResult> DoGetPatientByID(long patientID)
        {
            var loadPatientTask = new LoadPatientTask(patientID);
            yield return loadPatientTask;
            CurrentPatient = loadPatientTask.CurrentPatient;

            if (CurrentPatient == null)
            {
                yield break;
            }

            if (CurrentPatient.LatestRegistration_InPt == null)
            {
                MessageBox.Show(eHCMSResources.A0234_G1_Msg_InfoBNChuaCoDKNoiTru);
                yield break;
            }
            //Neu la dang ky noi tru. 
            if (CurrentPatient.LatestRegistration_InPt.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED
                //|| CurrentPatient.LatestRegistration_InPt.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING
                )
            {
                //Mở đăng ký còn đang sử dụng
                var loadRegTask = new LoadRegistrationInfoTask(CurrentPatient.LatestRegistration_InPt.PtRegistrationID, (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                yield return loadRegTask;
                if (loadRegTask.Registration == null)
                {
                    //Thong bao khong load duoc dang ky
                    Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(2)" });
                }
                else
                {
                    if (loadRegTask.Registration.AdmissionInfo == null || loadRegTask.Registration.AdmissionInfo.InPatientAdmDisDetailID <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0217_G1_Msg_InfoBNChuaVien_KhTheXV);
                        yield break;
                    }
                    CurrentRegistration = loadRegTask.Registration;
                    ShowOldRegistration(CurrentRegistration);
                }
            }
            else
            {
                string status = Helpers.GetEnumDescription(CurrentPatient.LatestRegistration_InPt.RegistrationStatus);
                MessageBox.Show(string.Format(eHCMSResources.W0951_G1_W, status));
            }

        }

        private void GetPatientByID(long patientID)
        {
            PatientLoading = true;
            Coroutine.BeginExecute(DoGetPatientByID(patientID), null, (o, e) => { PatientLoading = false; });
        }

        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            CurrentRegistration = regInfo;

            InitRegistration();

            if (PatientSummaryInfoContent != null)
            {

                PatientSummaryInfoContent.CurrentPatient = CurrentPatient;

                PatientSummaryInfoContent.SetPatientHISumInfo(CurrentRegistration.PtHISumInfo);
            }
            PatientSummaryInfoContent.CurrentPatientClassification = CurrentRegistration.PatientClassification;
            CurrentDiagnosisTreatment = null;
            DiagnosisTreatmentCollection = null;
            DischargeInfoContent.Registration = CurrentRegistration;

            if (!CurrentRegistration.AdmissionInfo.DischargeDate.HasValue || CurrentRegistration.AdmissionInfo.DischargeDate == null
                || CurrentRegistration.AdmissionInfo.DischargeDate.Value == DateTime.MinValue)
            {
                BeginEdit();
                if (DischargeInfoContent != null && DischargeInfoContent.Registration != null && DischargeInfoContent.Registration.AdmissionInfo != null && DischargeInfoContent.DischargeDateContent != null)
                {
                    if (CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate.HasValue)
                    {
                        DischargeInfoContent.DischargeDateContent.DateTime = CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate.Value;
                    }
                    else
                    {
                        //KMx: Mặc định để trống, để user tự nhập vào (03/06/2015 15:43).
                        //DischargeInfoContent.DischargeDateContent.DateTime = Globals.GetCurServerDateTime();
                        DischargeInfoContent.DischargeDateContent.DateTime = null;
                    }
                }
            }
            else
            {
                DischargeInfoContent.DischargeDateContent.DateTime = DischargeInfoContent.Registration.AdmissionInfo.DischargeDate;
                EndEdit();
            }

            if (CurrentRegistration.AdmissionInfo.V_DischargeType.HasValue && CurrentRegistration.AdmissionInfo.V_DischargeType.Value > 0)
            {
                if (DischargeInfoContent != null && DischargeInfoContent.Registration != null && DischargeInfoContent.Registration.AdmissionInfo != null)
                {
                    if (DischargeInfoContent.DischargeTypeContent != null)
                    {
                        DischargeInfoContent.DischargeTypeContent.SetSelectedID(DischargeInfoContent.Registration.AdmissionInfo.V_DischargeType.ToString());
                    }
                    if (DischargeInfoContent.DischargeConditionContent != null)
                    {
                        DischargeInfoContent.DischargeConditionContent.SetSelectedID(DischargeInfoContent.Registration.AdmissionInfo.VDischargeCondition.ToString());
                    }
                    /*▼====: #001*/
                    if (DischargeInfoContent.DeadReasonContent != null)
                    {
                        DischargeInfoContent.DeadReasonContent.SetSelectedID(DischargeInfoContent.Registration.AdmissionInfo.V_DeadReason.ToString());
                    }
                    /*▲====: #001*/
                }
                // TxD 20/01/2015: Not EndEdit here, only when DischargeDate has value ie. after confirm
                //EndEdit();
            }
            else
            {
                //▼===== #007
                if (DischargeInfoContent.DischargeTypeContent != null)
                {
                    DischargeInfoContent.DischargeTypeContent.EnumType = typeof(AllLookupValues.V_DischargeType);
                    DischargeInfoContent.DischargeTypeContent.AddSelectOneItem = true;
                    DischargeInfoContent.DischargeTypeContent.LoadData();
                }
                if (DischargeInfoContent.DischargeConditionContent != null)
                {
                    DischargeInfoContent.DischargeConditionContent.EnumType = typeof(AllLookupValues.DischargeCondition);
                    DischargeInfoContent.DischargeConditionContent.AddSelectOneItem = true;
                    DischargeInfoContent.DischargeConditionContent.LoadData();
                }
                //▲===== #007
            }

            //KMx: Xuất viện không được lấy chẩn đoán ngoại trú (11/02/2015 09:59).
            //DiagnosisIcd10Items_Load(CurrentRegistration.Patient.PatientID);

            //▼====: #002
            if (CurrentRegistration != null && CurrentRegistration.HIReportID > 0 && DischargeInfoContent != null)
            {
                DischargeInfoContent.gIsReported = true;
            }
            else if (DischargeInfoContent != null)
            {
                DischargeInfoContent.gIsReported = false;
            }
            gEnableRevertDischargeBtn = !(CurrentRegistration != null && CurrentRegistration.HIReportID > 0 && Globals.isAccountCheck);
            //▲====: #002
            CurrentDiagnosisTreatment = null;
            LoadDiagnosisTreatmentCollectionTask(null, DischargeInfoContent.Registration.PtRegistrationID);
            //▼==== #012
            CheckMedicalFilesForDischarge();
            //▲==== #012
        }

        private void InitRegistration()
        {
            if (CurrentRegistration.PtRegistrationID <= 0)
            {
                CurrentRegistration.Patient = CurrentPatient;
            }
            else
            {
                _currentPatient = CurrentRegistration.Patient;

                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }

        //public bool CanSaveDischargeCmd
        //{
        //    get
        //    {
        //        if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null)
        //        {
        //            if (CurrentRegistration.AdmissionInfo.DischargeDate == null)
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}

        //public bool CanConfirmDischargeCmd
        //{
        //    get
        //    {
        //        if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null)
        //        {
        //            if (CurrentRegistration.AdmissionInfo.DischargeDate == null)
        //            {
        //                return true;
        //            }
        //        }
        //        return false;
        //    }
        //}

        private void GetDischargeInfoOnForm()
        {
            var admInfo = CurrentRegistration.AdmissionInfo;

            //admInfo.DischargeDate = DischargeInfoContent.DischargeDate;

            if (DischargeInfoContent.HasDeceaseInfo)
            {
                if (admInfo.DeceasedInfo.DSNumber > 0)
                {
                    admInfo.DeceasedInfo.EntityState = EntityState.MODIFIED;
                }
                admInfo.DeceasedInfo.IsPostMorternExam = DischargeInfoContent.IsMortem;
                if (!DischargeInfoContent.IsMortem)
                {
                    admInfo.DeceasedInfo.PostMortemExamDiagnosis = null;
                    admInfo.DeceasedInfo.PostMortemExamCode = null;
                }
            }
            else
            {
                if (admInfo.DeceasedInfo != null)
                {
                    admInfo.DeceasedInfo.EntityState = EntityState.DELETED_MODIFIED;
                }
                admInfo.DSNumber = null;
            }

        }

        public bool ValidateDischargeInfo(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (PatientSummaryInfoContent.CurrentPatient == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0148_G1_HayChon1BN, new string[] { "CurrentPatient" });
                result.Add(item);
            }

            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        #region kiem tra truoc khi xua vien
        public bool checkBedPatientAlloc(ObservableCollection<BedPatientAllocs> bedPatientAllocses)
        {
            foreach (var bedPatientAllocse in bedPatientAllocses)
            {
                if (bedPatientAllocse.IsActive)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckDischargeInfo(IDischargeInfo info, bool bConfirmPtDischarged)
        {
            if (DischargeInfoContent.Registration != null && DischargeInfoContent.Registration.AdmissionInfo != null)
            {
                if (DischargeInfoContent.DischargeConditionContent != null)
                {
                    DischargeInfoContent.Registration.AdmissionInfo.VDischargeCondition = DischargeInfoContent.DischargeConditionContent.SelectedItem != null && DischargeInfoContent.DischargeConditionContent.SelectedItem.EnumItem != null ? (AllLookupValues.DischargeCondition)DischargeInfoContent.DischargeConditionContent.SelectedItem.EnumItem : 0;
                }
                /*▼====: #001*/
                if (DischargeInfoContent.DeadReasonContent != null)
                {
                    DischargeInfoContent.Registration.AdmissionInfo.V_DeadReason = DischargeInfoContent.DeadReasonContent.SelectedItem != null && DischargeInfoContent.DeadReasonContent.SelectedItem.EnumItem != null ? (AllLookupValues.DeadReason?)DischargeInfoContent.DeadReasonContent.SelectedItem.EnumItem : null;
                }
                /*▲====: #001*/
                if (DischargeInfoContent.DischargeTypeContent != null)
                {
                    DischargeInfoContent.Registration.AdmissionInfo.V_DischargeType = DischargeInfoContent.DischargeTypeContent.SelectedItem != null && DischargeInfoContent.DischargeTypeContent.SelectedItem.EnumItem != null ? (AllLookupValues.V_DischargeType)DischargeInfoContent.DischargeTypeContent.SelectedItem.EnumItem : 0;
                }
                if (DischargeInfoContent.DischargeDateContent != null)
                {
                    if (DischargeInfoContent.DischargeDateContent.HourPart == null || DischargeInfoContent.DischargeDateContent.MinutePart == null)
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0603_G1_Msg_InfoNhapTGianXV), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    if (DischargeInfoContent.DischargeDateContent.DatePart.HasValue && Globals.ServerConfigSection.InRegisElements.CheckDischargeDate && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysInDischargeForm >= 0)
                    {
                        Int32 NumOfDays = (DischargeInfoContent.DischargeDateContent.DatePart.GetValueOrDefault() - Globals.GetCurServerDateTime().Date).Days;
                        if (NumOfDays > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysInDischargeForm)
                        {
                            string msg = Globals.ServerConfigSection.InRegisElements.NumOfOverDaysInDischargeForm == 0 ? eHCMSResources.Z1877_G1_NgXVKgDcLonHonNgHTai
                                            : string.Format(eHCMSResources.Z1876_G1_NgXVLonHonNgHTai, Globals.ServerConfigSection.InRegisElements.NumOfOverDaysInDischargeForm);
                            MessageBox.Show(msg, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK);
                            return false;
                        }
                    }
                    DischargeInfoContent.Registration.AdmissionInfo.DischargeDate = DischargeInfoContent.DischargeDateContent.DateTime;
                }

                DischargeInfoContent.Registration.AdmissionInfo.DischargeDepartment = DischargeInfoContent.SelectedDischargeDepartment;

                if (DischargeInfoContent.Registration.AdmissionInfo.DischargeDepartment == null || DischargeInfoContent.Registration.AdmissionInfo.DischargeDepartment.DeptID <= 0)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0387_G1_Msg_InfoChuaChonKhoaXV));
                    return false;
                }

                //if (!Globals.ServerConfigSection.InRegisElements.DischargeInPtWith2Steps || bConfirmPtDischarged)
                //{
                if (DischargeInfoContent.Registration.AdmissionInfo.DischargeDate == null)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0394_G1_Msg_InfoChuaChonNgXV));
                    return false;
                }
                //}

                if (DischargeInfoContent.Registration.AdmissionInfo.V_DischargeType == null || DischargeInfoContent.Registration.AdmissionInfo.V_DischargeType <= 0)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0390_G1_Msg_InfoChuaChonLoaiXV));
                    return false;
                }

                if (DischargeInfoContent.Registration.AdmissionInfo.V_DischargeType != AllLookupValues.V_DischargeType.RA_VIEN && DischargeInfoContent.Registration.AdmissionInfo.ConfirmNotTreatedAsInPt)
                {
                    MessageBox.Show(eHCMSResources.A0901_G1_Msg_InfoXNhanBNKhDTriNTru);
                    return false;
                }

                if (DischargeInfoContent.Registration.AdmissionInfo.VDischargeCondition == null || DischargeInfoContent.Registration.AdmissionInfo.VDischargeCondition <= 0)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0385_G1_Msg_InfoChuaChonKQDieuTri));
                    return false;
                }

                if (DischargeInfoContent.HasDeceaseInfo)
                {
                    if (DischargeInfoContent.Registration.AdmissionInfo.DeceasedInfo == null)
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.A0667_G1_Msg_InfoKhCoTTinTuVong));
                        return false;
                    }
                    if (DischargeInfoContent.Registration.AdmissionInfo.DeceasedInfo.DeceasedDateTime == DateTime.MinValue)
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.A0400_G1_Msg_InfoChuaChonTGianTuVong));
                        return false;
                    }
                    if (DischargeInfoContent.Registration.AdmissionInfo.DeceasedInfo.DeceasedDateTime > DateTime.Now)
                    {
                        MessageBox.Show("Thời gian tử vong đang lớn hơn thời gian hiện tại");
                        return false;
                    }
                    if (DischargeInfoContent.Registration.AdmissionInfo.DeceasedInfo.V_CategoryOfDecease <= 0)
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.A0395_G1_Msg_InfoChuaChonNgNhanTuVong));
                        return false;
                    }
                    //if (DischargeInfoContent.DeadReasonContent.SelectedItem.EnumIntValue == 0)
                    //{
                    //    MessageBox.Show("Chưa nhập Tử vong(phẫu thuật)");
                    //    return false;
                    //}
                    if (string.IsNullOrWhiteSpace(DischargeInfoContent.Registration.AdmissionInfo.DeceasedInfo.MainReasonOfDecease))
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.A0421_G1_Msg_InfoChuaChonNgNhanChinhTuVong));
                        return false;
                    }
                    if (DischargeInfoContent.IsMortem)
                    {
                        if (string.IsNullOrWhiteSpace(DischargeInfoContent.DeceasedInfo.PostMortemExamDiagnosis))
                        {
                            MessageBox.Show("Chưa nhập chẩn đoán giải phẫu tử thi");
                            return false;
                        }
                    }
                }

                //20201204 TVN nếu loại xuất viện là chuyển viện thì phải yêu cầu nhập giấy chuyển viện
                // 202012331 TNHX: Bảo đã thêm store nên bỏ comment
                if (DischargeInfoContent.Registration.AdmissionInfo.V_DischargeType == AllLookupValues.V_DischargeType.CHUYEN_TUYEN_CHUYEN_MON
                    || DischargeInfoContent.Registration.AdmissionInfo.V_DischargeType == AllLookupValues.V_DischargeType.CHUYEN_VIEN_NGUOI_BENH)
                {
                    if (DischargeInfoContent.Registration.AdmissionInfo.TransferFormID == 0)
                    {
                        if (DischargeInfoContent.CurrentTransferForm.TransferFormID == 0)
                        {
                            MessageBox.Show("Chưa nhập giấy chuyển tuyến");
                            return false;
                        }
                    }
                }
                else
                {
                    //if(DischargeInfoContent.Registration.AdmissionInfo.TransferFormID > 0 || DischargeInfoContent.CurrentTransferForm.TransferFormID > 0)
                    //{
                    //    MessageBox.Show("BN đang có giấy chuyển tuyến. Vui lòng xóa giấy chuyển tuyến");
                    //    return false;
                    //}    
                }
                if (UseOnlyDailyDiagnosis)
                {
                    //CMN: Đã có sẵn chẩn đoán xuất viện
                    if (DiagnosisTreatmentCollection != null && DiagnosisTreatmentCollection.Any(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                    {
                        CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.First(x => x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS);
                    }
                    else if (!DischargeInfoContent.Registration.AdmissionInfo.ConfirmNotTreatedAsInPt) //20191223 TBL: BM 0021750: Nếu BN XN không điều trị nội trú thì không cần kiểm tra CĐ xác nhận
                    {
                        if (CurrentDiagnosisTreatment == null || CurrentDiagnosisTreatment.DTItemID == 0 || CurrentDiagnosisTreatment.IsAdmission)
                        {
                            MessageBox.Show(eHCMSResources.Z2895_G1_Msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        if (DischargeInfoContent.Registration.AdmissionInfo.DischargeDepartment.DeptID != CurrentDiagnosisTreatment.Department.DeptID)
                        {
                            MessageBox.Show(eHCMSResources.Z2896_G1_Msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        //▼====== #004
                        //if ((Globals.GetCurServerDateTime() - CurrentDiagnosisTreatment.DiagnosisDate).TotalMinutes > Globals.ServerConfigSection.ConsultationElements.ConsultMinTimeReqBeforeExit * 60)
                        //{
                        //    MessageBox.Show(string.Format(eHCMSResources.Z2896_G1_ChanDoanQuaTGXacNhan, string.Format("{0} {1}", Globals.ServerConfigSection.ConsultationElements.ConsultMinTimeReqBeforeExit, eHCMSResources.T1209_G1_GioL)), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        //    return false;
                        //}
                        string[] ListStr = Globals.ServerConfigSection.ConsultationElements.ConsultMinTimeReqBeforeExit.Split(new char[] { ';' });
                        int MinTimeReq = Convert.ToInt16(ListStr[1]);
                        if (MinTimeReq > 0 && (DischargeInfoContent.DischargeDateContent.DateTime.Value - CurrentDiagnosisTreatment.DiagnosisDate).TotalMinutes > MinTimeReq * 60)
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z2896_G1_ChanDoanQuaTGXacNhan, string.Format("{0} {1}", MinTimeReq, eHCMSResources.T1209_G1_GioL)), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        //▲====== #004
                    }
                }
                //if (DischargeInfoContent.Registration.AdmissionInfo.DiagnosisTreatmentInfo == null || String.IsNullOrEmpty(DischargeInfoContent.Registration.AdmissionInfo.DiagnosisTreatmentInfo.DiagnosisFinal))
                //{
                //    MessageBox.Show("Chưa nhập bệnh chính.");
                //    return false;
                //}
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckDischargeBilling(IDischargeInfo info)
        {
            foreach (var item in info.BillingInvoiceListingContent.BillingInvoices)
            {
                if (item.V_InPatientBillingInvStatus == AllLookupValues.V_InPatientBillingInvStatus.NEW)
                {
                    if (item.PaidTime == null)
                    {
                        info.isNotPayment = true;
                        return false;
                    }
                }
            }
            info.isNotPayment = false;
            return true;
        }


        #endregion

        public void TraGiuong(ObservableCollection<BedPatientAllocs> allBedPatientAlloc)
        {
            Action<IcwdBedPatientCommon> onInitDlg = delegate (IcwdBedPatientCommon cwdBedPatientVM)
            {
                cwdBedPatientVM.isDelete = true;
                cwdBedPatientVM.isDeleteAll = true;
                cwdBedPatientVM.IsEdit = false;
                cwdBedPatientVM.allBedPatientAllocs = allBedPatientAlloc;
                this.ActivateItem(cwdBedPatientVM);
            };
            GlobalsNAV.ShowDialog<IcwdBedPatientCommon>(onInitDlg);
        }

        WarningWithConfirmMsgBoxTask confirmBeforeDischarge = null;
        WarningWithConfirmMsgBoxTask errorMessageBox = null;

        public IEnumerator<IResult> CheckValid_New(bool bConfirmPtDischarged)
        {
            if (DischargeInfoContent.Registration == null || DischargeInfoContent.Registration.PtRegistrationID == 0)
            {
                yield break;
            }
            //CMN: Load danh sách chẩn đoán vì màn hình xác nhận xuất viện không có được apply giá trị danh sách chẩn đoán
            if (bConfirmPtDischarged && (DiagnosisTreatmentCollection == null || DiagnosisTreatmentCollection.Count == 0))
            {
                var CurrentTask = new GenericCoRoutineTask(LoadDiagnosisTreatmentCollectionTask, DischargeInfoContent.Registration.PtRegistrationID);
                yield return CurrentTask;
            }
            if (!CheckDischargeInfo(DischargeInfoContent, bConfirmPtDischarged))
            {
                //20200414 TBL: BM 0031120: Nếu lưu thông tin xuất viện không thành công thì set lại thời gian xuất viện = null, để khi ra toa xuất viện sẽ kiểm tra thời gian này nếu khác null sẽ không cho ra toa XV
                if (DischargeInfoContent.Registration != null && DischargeInfoContent.Registration.AdmissionInfo != null)
                {
                    DischargeInfoContent.Registration.AdmissionInfo.DischargeDate = null;
                }
                yield break;
            }
            //▼====: #010
            //20220825 BLQ: bỏ không check các điều kiện xuất viện khi lưu thông tin xuất viện
            if (Globals.ServerConfigSection.InRegisElements.DischargeInPtWith2Steps && bConfirmPtDischarged)
            //▲====: #010
            {
                yield return GenericCoRoutineTask.StartTask(CheckBeforeDischarge_Action);

                if (!string.IsNullOrEmpty(ErrorMessages))
                {
                    ErrorMessages = string.Format("{0}: ", eHCMSResources.Z1100_G1_BNKgTheXV) + Environment.NewLine + ErrorMessages;

                    errorMessageBox = new WarningWithConfirmMsgBoxTask(ErrorMessages, "", false);
                    yield return errorMessageBox;

                    errorMessageBox = null;
                    yield break;
                }

                if (!string.IsNullOrEmpty(ConfirmMessages))
                {
                    ConfirmMessages = string.Format(eHCMSResources.Z1101_G1_XNhanTrcKhiXV, ConfirmMessages);
                    confirmBeforeDischarge = new WarningWithConfirmMsgBoxTask(ConfirmMessages, eHCMSResources.Z1109_G1_TiepTucXV);
                    yield return confirmBeforeDischarge;
                    if (!confirmBeforeDischarge.IsAccept)
                    {
                        confirmBeforeDischarge = null;
                        yield break;
                    }
                    confirmBeforeDischarge = null;
                }
            }
            // TxD 20/01/2015 If we use Discharge in 2 Step then:
            //  Step 1: Has No Discharge Date
            //  Step 2: Confirm and update with Discharge Date
            if (Globals.ServerConfigSection.InRegisElements.DischargeInPtWith2Steps && bConfirmPtDischarged == false)
            {
                CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate = CurrentRegistration.AdmissionInfo.DischargeDate;
                CurrentRegistration.AdmissionInfo.DischargeDate = null;
            }
            if (bConfirmPtDischarged && Globals.ServerConfigSection.CommonItems.IsApplyAutoCreateHIReport)
            {
                btnConfirm();
            }
            else
            {
                Discharge();
            }
        }

        //KMx: Chỉ có link Xuất viện của Điều dưỡng mới được thấy nút xác nhận, còn link Bác Sĩ thì không (23/01/2015 12:14).
        private bool _IsShowConfirmDischargeBtn;
        public bool IsShowConfirmDischargeBtn
        {
            get { return _IsShowConfirmDischargeBtn; }
            set
            {
                _IsShowConfirmDischargeBtn = value;
            }
        }

        private bool _showConfirmDischargeBtn = Globals.ServerConfigSection.InRegisElements.DischargeInPtWith2Steps;
        public bool ShowConfirmDischargeBtn
        {
            get { return _showConfirmDischargeBtn && IsShowConfirmDischargeBtn; }
            set
            {
                _showConfirmDischargeBtn = value;
            }
        }
        public void ConfirmDischargeCmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null || CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID <= 0)
            {
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z1524_G1_XNhanTTinXV))
            {
                return;
            }
            if (CurrentRegistration.AdmissionInfo.V_DischargeType.HasValue && CurrentRegistration.AdmissionInfo.VDischargeCondition.HasValue &&
                DischargeDateFromInPtRec != null && DischargeDateFromInPtRec.HasValue)
            {
                MessageBox.Show(eHCMSResources.A0226_G1_Msg_InfoBNDaXV);
                return;
            }
            Coroutine.BeginExecute(CheckValid_New(true));
        }

        // TxD 29/03/2015 Added Temporary Discharge Date to allow for a temporary state of being discharge so the Discharge can still be updated 
        //                  and we can still create a new InPt registration to allow for new admission
        public void TemporaryDischargeCmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null || CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID <= 0)
            {
                return;
            }

            if (CurrentRegistration.AdmissionInfo.V_DischargeType.HasValue && CurrentRegistration.AdmissionInfo.VDischargeCondition.HasValue &&
                DischargeDateFromInPtRec != null && DischargeDateFromInPtRec.HasValue)
            {
                MessageBox.Show(eHCMSResources.A0226_G1_Msg_InfoBNDaXV);
                return;
            }

            CurrentRegistration.AdmissionInfo.TempDischargeDate = Globals.GetCurServerDateTime();

            Discharge();

        }

        private DateTime? DischargeDateFromInPtRec = null;

        public void SaveDischargeCmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null || CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID <= 0)
            {
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSLanguage.eHCMSResources.Z1525_G1_CNhatTTinXV))
            {
                return;
            }
            if (CurrentRegistration.AdmissionInfo.V_DischargeType.HasValue && CurrentRegistration.AdmissionInfo.VDischargeCondition.HasValue &&
                DischargeDateFromInPtRec != null && DischargeDateFromInPtRec.HasValue)
            {
                MessageBox.Show(eHCMSResources.A0226_G1_Msg_InfoBNDaXV);
                return;
            }
            if (string.IsNullOrEmpty(CurrentRegistration.AdmissionInfo.DischargeStatus))
            {
                MessageBox.Show("Chưa nhập tình trạng ra viện của bệnh nhân");
                return;
            }
            if (string.IsNullOrEmpty(CurrentRegistration.AdmissionInfo.Comment))
            {
                MessageBox.Show("Chưa nhập lời dặn");
                return;
            }
            if (CurrentRegistration.AdmissionInfo.DeceasedInfo!= null && CurrentRegistration.Patient!=null && (string.IsNullOrEmpty(CurrentRegistration.Patient.IDNumber)
               || CurrentRegistration.Patient.NationalityID==null))
            {
                MessageBox.Show("Bệnh nhân tử vong chưa nhập đầy dủ thông tin hành chánh không thể xuất viện."+Environment.NewLine+"Vui lòng cập nhật thông tin hành chánh và tải lại");
                return;
            }
            if (!DischargeInfoContent.CheckValidForDischaregePaper())
            {
                return;
            }
            Coroutine.BeginExecute(CheckValid_New(false));

            //▼==== #011
            Globals.EventAggregator.Publish(new OnChangedUpdateAdmDisDetails() { PrescriptionDoctorAdvice = CurrentRegistration.AdmissionInfo.Comment });
            //▲==== #011
        }

        public void Discharge()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateDischargeInfo(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            this.ShowBusyIndicator();
            GetDischargeInfoOnForm();
            var t = new Thread(() =>
            {
                //AxErrorEventArgs error = null;

                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddInPatientDischarge(CurrentRegistration.AdmissionInfo, Globals.LoggedUserAccount.Staff.StaffID, (UseOnlyDailyDiagnosis && CurrentDiagnosisTreatment != null ? (long?)CurrentDiagnosisTreatment.DTItemID : null), Globals.DispatchCallback((asyncResult) =>
                        {
                            bool bOK = false;
                            try
                            {
                                List<string> errorMessages;
                                bOK = contract.EndAddInPatientDischarge(out errorMessages, asyncResult);
                                if (!bOK)
                                {
                                    StringBuilder st = new StringBuilder();
                                    st.AppendLine(eHCMSResources.Z1526_G1_TTinXVLuuKgThCong + Environment.NewLine);
                                    if (errorMessages != null && errorMessages.Count > 0)
                                    {
                                        foreach (var item in errorMessages)
                                        {
                                            st.AppendLine(item + Environment.NewLine);
                                        }
                                        MessageBox.Show(st.ToString());
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));
                                    OpenRegistration(CurrentRegistration.PtRegistrationID);
                                    //20191120 TBL: Khi lưu xuất viện thành công thì bắn sự kiện để màn hình Toa thuốc XV lấy xác nhận chẩn đoán xuất viện làm xác nhận chẩn đoán của toa thuốc xuất viện
                                    if (CurrentDiagnosisTreatment != null)
                                    {
                                        Globals.EventAggregator.Publish(new LoadDiagnosisTreatmentConfirmedForDischarge { DiagnosisTreatment = CurrentDiagnosisTreatment });
                                    }
                                }
                            }
                            //catch (FaultException<AxException> fault)
                            //{
                            //    bOK = false;
                            //    error = new AxErrorEventArgs(fault);
                            //}
                            catch (Exception ex)
                            {
                                bOK = false;
                                //error = new AxErrorEventArgs(ex);
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                //if (error != null)
                                //{
                                //    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                                //}
                                //else
                                //{
                                //    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));
                                //}

                                this.HideBusyIndicator();
                                IsLoading = false;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    //error = new AxErrorEventArgs(ex);
                    //if (error != null)
                    //{
                    //    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    //}
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });
            t.Start();
        }

        private bool _ShowRevertDischargeBtn;
        public bool ShowRevertDischargeBtn
        {
            get
            {
                return _ShowRevertDischargeBtn;
            }
            set
            {
                _ShowRevertDischargeBtn = value;
                NotifyOfPropertyChange(() => ShowRevertDischargeBtn);
            }
        }

        public void RevertDischargeCmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null)
            {
                MessageBox.Show(eHCMSResources.A0633_G1_Msg_InfoKhongCoDKBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z1527_G1_HuyXNhanXV))
            {
                return;
            }
            if (CurrentRegistration.AdmissionInfo.DischargeDate == null && CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate == null)
            {
                MessageBox.Show(eHCMSResources.A0218_G1_Msg_InfoCNhatTThaiXVFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0137_G1_Msg_ConfCNhatXV_ChuaXV, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RevertDischarge();
            }
        }

        public TreatmentProcess CurrentTreatmentProcess { get; set; } = new TreatmentProcess();
        public void PhieuTT_QTDTCmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null)
            {
                MessageBox.Show(eHCMSResources.A0633_G1_Msg_InfoKhongCoDKBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate == null)
            {
                MessageBox.Show("Chưa có thông tin xuất viện không thể tạo phiếu tóm tắt quá trình điều trị", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            CheckTreatmentProcess();
        }

        private void CheckTreatmentProcess()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTreatmentProcessByPtRegistrationID(CurrentRegistration.PtRegistrationID,  /*TMA*/
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {

                                CurrentTreatmentProcess = contract.EndGetTreatmentProcessByPtRegistrationID(asyncResult);
                                GetPhieuTT_QTDT();
                            }
                            catch (Exception ex)
                            {

                                ClientLoggerHelper.LogError(ex.Message);

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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void GetPhieuTT_QTDT()
        {
            //PatientRegistration CurRegistration = Registration_DataStorage.CurrentPatientRegistration;
            var mEvent = new TreatmentProcessEvent();
            if (CurrentTreatmentProcess != null && CurrentTreatmentProcess.TreatmentProcessID > 0)
            {
                CurrentTreatmentProcess.FromDate = Convert.ToDateTime(CurrentRegistration.AdmissionInfo.AdmissionDate);
                CurrentTreatmentProcess.ToDate = Convert.ToDateTime(CurrentRegistration.AdmissionInfo.DischargeDate);
                mEvent.Item = CurrentTreatmentProcess;
            }
            else
            {
                mEvent.Item = new TreatmentProcess();
                mEvent.Item.CurPatientRegistration = new PatientRegistration();
                mEvent.Item.TreatmentProcessID = (long)0;
                mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)CurrentRegistration.PtRegistrationID;
                if (CurrentRegistration.HisID != null)
                    mEvent.Item.CurPatientRegistration.HisID = (long)CurrentRegistration.HisID.Value;
                if (CurrentRegistration != null)
                {
                    if (CurrentRegistration.HealthInsurance != null)
                        mEvent.Item.CurPatientRegistration.HealthInsurance = CurrentRegistration.HealthInsurance;
                    if (CurrentRegistration.Patient != null)
                    {
                        mEvent.Item.CurPatientRegistration.Patient = CurrentRegistration.Patient;
                    }
                    if (CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo != null && CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo.DiagnosisFinal != null)
                    {
                        mEvent.Item.Diagnosis = CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo.DiagnosisFinal;
                        mEvent.Item.Treatments = CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo.Treatment;
                    }
                    mEvent.Item.FromDate = Convert.ToDateTime(CurrentRegistration.AdmissionInfo.AdmissionDate);
                    mEvent.Item.ToDate = Convert.ToDateTime(CurrentRegistration.AdmissionInfo.DischargeDate);

                }
            }
            Action<IPhieuTomTat_QuaTrinhDieuTri> onInitDlg = delegate (IPhieuTomTat_QuaTrinhDieuTri PhieuTT_QuaTrinhDieuTri)
            {
                PhieuTT_QuaTrinhDieuTri.IsThisViewDialog = true;
                if (CurrentRegistration != null && CurrentRegistration.InPtRegistrationID != 0)
                {

                }
                this.ActivateItem(PhieuTT_QuaTrinhDieuTri);
                PhieuTT_QuaTrinhDieuTri.SetCurrentInformation(mEvent);
            };
            GlobalsNAV.ShowDialog<IPhieuTomTat_QuaTrinhDieuTri>(onInitDlg);
        }
        public void RevertDischarge()
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRevertDischarge(CurrentRegistration.AdmissionInfo, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                bool bOK = false;
                                try
                                {
                                    bOK = contract.EndRevertDischarge(asyncResult);

                                    if (bOK)
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0826_G1_CNhatTThaiThCong));
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail));
                                    }
                                    //▼===== #006
                                    IConsultationModule consultVM = Globals.GetViewModel<IConsultationModule>();
                                    if (consultVM.MainContent is IConsultationsSummary_InPt)
                                    {
                                        (consultVM.MainContent as IConsultationsSummary_InPt).CallSetInPatientInfoForConsultation(Registration_DataStorage.CurrentPatientRegistration, Registration_DataStorage.CurrentPatientRegistrationDetail);
                                    }
                                    else
                                    {
                                        OpenRegistration(CurrentRegistration.PtRegistrationID);
                                    }
                                    //▲===== #006
                                }
                                catch (Exception ex)
                                {
                                    bOK = false;
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception)
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void EditDischargeCmd()
        {
            BeginEdit();
        }


        public bool IsEditing
        {
            get
            {
                return true;
            }
        }

        private bool _IsConsultation;
        public bool IsConsultation
        {
            get
            {
                return _IsConsultation;
            }
            set
            {
                if (_IsConsultation == value)
                {
                    return;
                }
                _IsConsultation = value;
                NotifyOfPropertyChange(() => IsConsultation);
            }
        }


        PatientRegistration _tempRegistration = null;
        bool _HasDeceaseInfo_Backup;
        protected void BeginEdit()
        {
            _tempRegistration = CurrentRegistration.DeepCopy();
            if (DischargeInfoContent != null)
            {
                _HasDeceaseInfo_Backup = DischargeInfoContent.HasDeceaseInfo;
            }
            //if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null
            //    && CurrentRegistration.AdmissionInfo.DeceasedInfo == null)
            //{
            //    CurrentRegistration.AdmissionInfo.DeceasedInfo = new DeceasedInfo();
            //}
            //IsEditing = true;
        }
        protected void CancelEdit()
        {
            //KMx: Phải set HasDeceaseInfo trước CurrentRegistration, vì trong HasDeceaseInfo có new DeceasedInfo(); (17/01/2015 16:49).
            if (DischargeInfoContent != null)
            {
                DischargeInfoContent.HasDeceaseInfo = _HasDeceaseInfo_Backup;
            }

            CurrentRegistration = _tempRegistration;

            _tempRegistration = null;
            //IsEditing = false;
        }
        protected void EndEdit()
        {
            _tempRegistration = null;
            //IsEditing = false;
        }

        private DiagnosisIcd10Items _mainDiagnosisIcd10Item;
        public DiagnosisIcd10Items MainDiagnosisIcd10Item
        {
            get
            {
                return _mainDiagnosisIcd10Item;
            }
            set
            {
                _mainDiagnosisIcd10Item = value;
                NotifyOfPropertyChange(() => MainDiagnosisIcd10Item);
                if (CurrentRegistration.AdmissionInfo != null
                    && (CurrentRegistration.AdmissionInfo.DischargeDate == null
                        || CurrentRegistration.AdmissionInfo.DischargeDate.Value == DateTime.MinValue))
                {
                    if (_mainDiagnosisIcd10Item != null)
                    {
                        CurrentRegistration.AdmissionInfo.DischargeCode = _mainDiagnosisIcd10Item.ICD10Code;
                        if (_mainDiagnosisIcd10Item.DiseasesReference != null)
                        {
                            CurrentRegistration.AdmissionInfo.DischargeNote = _mainDiagnosisIcd10Item.DiseasesReference.DiseaseNameVN;
                        }
                    }
                    else
                    {
                        CurrentRegistration.AdmissionInfo.DischargeCode = null;
                        CurrentRegistration.AdmissionInfo.DischargeNote = null;
                    }
                }
            }
        }
        private ObservableCollection<DiagnosisIcd10Items> _allExtraDiagnosisIcd10Items;
        public ObservableCollection<DiagnosisIcd10Items> AllExtraDiagnosisIcd10Items
        {
            get
            {
                return _allExtraDiagnosisIcd10Items;
            }
            set
            {
                _allExtraDiagnosisIcd10Items = value;
                NotifyOfPropertyChange(() => AllExtraDiagnosisIcd10Items);

                if (DischargeInfoContent != null)
                {
                    DischargeInfoContent.AllExtraDiagnosisIcd10Items = _allExtraDiagnosisIcd10Items;
                }
            }
        }
        /// <summary>
        /// Load thong tin ICD10 cua dang ky hien tai.
        /// Sau nay se dua id dang ky. Ham hien tai chi dua ID benh nhan.
        /// </summary>
        /// <param name="ServiceRecID"></param>
        /// <param name="PatientID"></param>
        /// <param name="Last"></param>
        private void DiagnosisIcd10Items_Load(long? PatientID)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDiagnosisIcd10Items_Load(null, PatientID, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            ObservableCollection<DiagnosisIcd10Items> allItems = null;

                            var pId = (long)asyncResult.AsyncState;
                            if (CurrentRegistration != null && CurrentRegistration.Patient != null
                                && CurrentRegistration.Patient.PatientID > 0
                                && CurrentRegistration.Patient.PatientID == pId)
                            {
                                if (results != null)
                                {
                                    allItems = new ObservableCollection<DiagnosisIcd10Items>();
                                    foreach (var item in results)
                                    {
                                        if (item.IsMain)
                                        {
                                            MainDiagnosisIcd10Item = item;
                                        }
                                        else
                                        {
                                            allItems.Add(item);
                                        }
                                    }
                                }
                                else
                                {
                                    MainDiagnosisIcd10Item = null;
                                }
                                AllExtraDiagnosisIcd10Items = allItems;
                            }
                            else
                            {
                                MainDiagnosisIcd10Item = null;
                                AllExtraDiagnosisIcd10Items = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;

                        }

                    }), PatientID);

                }

            });

            t.Start();
        }

        //KMx: Cấu hình của view này sai rồi, khi nào có thời gian thì làm cấu hình lại.
        //Lưu ý: View này có 2 link (Xuất viện, Xuất viện (bác sĩ)), phải check operation ở LeftMenu rồi truyền vào, không phải check trong view này (23/01/2015 17:43).
        //public void authorization()
        //{
        //    if (!Globals.isAccountCheck)
        //    {
        //        return;
        //    }
        //    mXuatVien_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                       , (int)ePatient.mDischarge,
        //                                       (int)oRegistrionEx.mXuatVien_Luu, (int)ePermission.mView);
        //    mXuatVien_Sua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                       , (int)ePatient.mDischarge,
        //                                       (int)oRegistrionEx.mXuatVien_Sua, (int)ePermission.mView);

        //    //phan nay nam trong module chung ne
        //    mXuatVien_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mRegister,
        //                                     (int)oRegistrionEx.mXuatVien_Patient_TimBN, (int)ePermission.mView);
        //    mXuatVien_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mXuatVien_Patient_ThemBN, (int)ePermission.mView);
        //    mXuatVien_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mXuatVien_Patient_TimDangKy, (int)ePermission.mView);

        //    mXuatVien_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mXuatVien_Info_CapNhatThongTinBN, (int)ePermission.mView);
        //    mXuatVien_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mXuatVien_Info_XacNhan, (int)ePermission.mView);
        //    mXuatVien_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mXuatVien_Info_XoaThe, (int)ePermission.mView);
        //    mXuatVien_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mXuatVien_Info_XemPhongKham, (int)ePermission.mView);
        //}
        #region checking account

        private bool _mXuatVien_Luu = true;
        private bool _mXuatVien_Sua = true;
        public bool mXuatVien_Luu
        {
            get
            {
                return _mXuatVien_Luu;
            }
            set
            {
                if (_mXuatVien_Luu == value)
                    return;
                _mXuatVien_Luu = value;
                NotifyOfPropertyChange(() => mXuatVien_Luu);
            }
        }


        public bool mXuatVien_Sua
        {
            get
            {
                return _mXuatVien_Sua;
            }
            set
            {
                if (_mXuatVien_Sua == value)
                    return;
                _mXuatVien_Sua = value;
                NotifyOfPropertyChange(() => mXuatVien_Sua);
            }
        }


        //phan nay nam trong module chung
        private bool _mXuatVien_Patient_TimBN = true;
        private bool _mXuatVien_Patient_ThemBN = true;
        private bool _mXuatVien_Patient_TimDangKy = true;

        private bool _mXuatVien_Info_CapNhatThongTinBN = true;
        private bool _mXuatVien_Info_XacNhan = true;
        private bool _mXuatVien_Info_XoaThe = true;
        private bool _mXuatVien_Info_XemPhongKham = true;

        public bool mXuatVien_Patient_TimBN
        {
            get
            {
                return _mXuatVien_Patient_TimBN;
            }
            set
            {
                if (_mXuatVien_Patient_TimBN == value)
                    return;
                _mXuatVien_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mXuatVien_Patient_TimBN);
            }
        }

        public bool mXuatVien_Patient_ThemBN
        {
            get
            {
                return _mXuatVien_Patient_ThemBN;
            }
            set
            {
                if (_mXuatVien_Patient_ThemBN == value)
                    return;
                _mXuatVien_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mXuatVien_Patient_ThemBN);
            }
        }

        public bool mXuatVien_Patient_TimDangKy
        {
            get
            {
                return _mXuatVien_Patient_TimDangKy;
            }
            set
            {
                if (_mXuatVien_Patient_TimDangKy == value)
                    return;
                _mXuatVien_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mXuatVien_Patient_TimDangKy);
            }
        }

        public bool mXuatVien_Info_CapNhatThongTinBN
        {
            get
            {
                return _mXuatVien_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mXuatVien_Info_CapNhatThongTinBN == value)
                    return;
                _mXuatVien_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mXuatVien_Info_CapNhatThongTinBN);
            }
        }

        public bool mXuatVien_Info_XacNhan
        {
            get
            {
                return _mXuatVien_Info_XacNhan;
            }
            set
            {
                if (_mXuatVien_Info_XacNhan == value)
                    return;
                _mXuatVien_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mXuatVien_Info_XacNhan);
            }
        }

        public bool mXuatVien_Info_XoaThe
        {
            get
            {
                return _mXuatVien_Info_XoaThe;
            }
            set
            {
                if (_mXuatVien_Info_XoaThe == value)
                    return;
                _mXuatVien_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mXuatVien_Info_XoaThe);
            }
        }

        public bool mXuatVien_Info_XemPhongKham
        {
            get
            {
                return _mXuatVien_Info_XemPhongKham;
            }
            set
            {
                if (_mXuatVien_Info_XemPhongKham == value)
                    return;
                _mXuatVien_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mXuatVien_Info_XemPhongKham);
            }
        }
        #endregion

        public void PrintCmd()
        {
            if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
                {
                    reportVm.ID = CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID; ;
                    reportVm.eItem = ReportName.THONG_TIN_XUAT_VIEN;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }


        #region Thông tin thanh toán (15/12/2014 14:41).
        public decimal DebtRemaining
        {
            get
            {
                return (_totalLiabilities + _TotalRefundMoney) - _sumOfAdvance;
            }
        }

        private decimal _TotalRefundMoney;
        public decimal TotalRefundMoney
        {
            get { return _TotalRefundMoney; }
            set
            {
                _TotalRefundMoney = value;
                NotifyOfPropertyChange(() => TotalRefundMoney);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }

        private decimal _totalLiabilities;
        public decimal TotalLiabilities
        {
            get { return _totalLiabilities; }
            set
            {
                _totalLiabilities = value;
                NotifyOfPropertyChange(() => TotalLiabilities);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }


        private decimal _sumOfAdvance;
        public decimal SumOfAdvance
        {
            get { return _sumOfAdvance; }
            set
            {
                _sumOfAdvance = value;
                NotifyOfPropertyChange(() => SumOfAdvance);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
                NotifyOfPropertyChange(() => TotalPatientPaid_Finalized);
            }
        }

        private decimal _SumOfCashAdvBalanceAmount;
        public decimal SumOfCashAdvBalanceAmount
        {
            get { return _SumOfCashAdvBalanceAmount; }
            set
            {
                _SumOfCashAdvBalanceAmount = value;
                NotifyOfPropertyChange(() => SumOfCashAdvBalanceAmount);
            }
        }

        //Load thông tin thanh toán (18/09/2014 15:46).
        private void GetPaymentInfo_Action(GenericCoRoutineTask genTask)
        {
            if (CurrentRegistration == null || CurrentRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                genTask.ActionComplete(false);
            }

            decimal totalLiabilities = 0;
            decimal sumOfAdvance = 0;
            decimal totalPatientPaymentPaidInvoice = 0;
            decimal totalRefundPatient = 0;
            decimal totalCashAdvBalanceAmount = 0;
            decimal totalPtPayment_NotFinalized = 0;
            decimal totalPtPaid_NotFinalized = 0;
            decimal totalSupportFund_NotFinalized = 0;
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientRegistrationAndPaymentInfo(CurrentRegistration.PtRegistrationID, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                decimal TotalCharityOrgPayment = 0;
                                var result = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities, out sumOfAdvance, out totalPatientPaymentPaidInvoice, out totalRefundPatient, out totalCashAdvBalanceAmount,
                                                                                                out TotalCharityOrgPayment, out totalPtPayment_NotFinalized, out totalPtPaid_NotFinalized, out totalSupportFund_NotFinalized, asyncResult);
                                if (result)
                                {
                                    TotalLiabilities = totalLiabilities;
                                    SumOfAdvance = sumOfAdvance;
                                    TotalRefundMoney = totalRefundPatient;
                                    SumOfCashAdvBalanceAmount = totalCashAdvBalanceAmount;
                                    TotalSupportFund = TotalCharityOrgPayment;
                                    TotalPatientPayment_NotFinalized = totalPtPayment_NotFinalized;
                                    TotalPatientPaid_NotFinalized = totalPtPaid_NotFinalized;
                                    TotalSupportFund_NotFinalized = totalSupportFund_NotFinalized;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                //KMx: A.Tuấn dặn check null.
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(bContinue);
                                }
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);

                    //KMx: A.Tuấn dặn check null.
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });

            t.Start();

        }
        #endregion


        #region KMx: Kiểm tra trước khi xuất viện (16/12/2014 08:37).

        private string _errorMessages;
        public string ErrorMessages
        {
            get { return _errorMessages; }
            set
            {
                _errorMessages = value;
                NotifyOfPropertyChange(() => ErrorMessages);
            }
        }

        private string _confirmMessages;
        public string ConfirmMessages
        {
            get { return _confirmMessages; }
            set
            {
                _confirmMessages = value;
                NotifyOfPropertyChange(() => ConfirmMessages);
            }
        }

        private void CheckBeforeDischarge_Action(GenericCoRoutineTask genTask)
        {
            if (CurrentRegistration == null || CurrentRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                genTask.ActionComplete(false);
            }

            if (CurrentRegistration.AdmissionInfo == null || CurrentRegistration.AdmissionInfo.DischargeDepartment == null)
            {
                MessageBox.Show(eHCMSResources.A0387_G1_Msg_InfoChuaChonKhoaXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                genTask.ActionComplete(false);
            }

            ErrorMessages = "";
            ConfirmMessages = "";

            string errorMsg = "";
            string confirmMsg = "";

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckBeforeDischarge(CurrentRegistration.PtRegistrationID, CurrentRegistration.AdmissionInfo.DischargeDepartment.DeptID
                            , CurrentRegistration.AdmissionInfo.ConfirmNotTreatedAsInPt
                            , (CurrentRegistration.AdmissionInfo.DischargeDate == null ? CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate : CurrentRegistration.AdmissionInfo.DischargeDate)
                            //▼==== #013
                            , false
                            //▲==== #013
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var result = contract.EndCheckBeforeDischarge(out errorMsg, out confirmMsg, asyncResult);

                                    if (result)
                                    {
                                        ErrorMessages = errorMsg;
                                        ConfirmMessages = confirmMsg;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A078_G1_Msg_InfoKTraXVFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        bContinue = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogError(ex.Message);
                                    bContinue = false;
                                }
                                finally
                                {
                                    //KMx: A.Tuấn dặn check null.
                                    if (genTask != null)
                                    {
                                        genTask.ActionComplete(bContinue);
                                    }
                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);

                    //KMx: A.Tuấn dặn check null.
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });

            t.Start();

        }
        #endregion

        public decimal BalanceCreditRemaining
        {
            get
            {
                //decimal calcBal = _sumOfAdvance - (_totalLiabilities + _TotalRefundMoney) + _TotalSupportFund;
                decimal calcBal = _TotalPatientPaid_NotFinalized + _TotalSupportFund_NotFinalized - _TotalPatientPayment_NotFinalized;
                if (tbTotBalCredit != null)
                {
                    if (calcBal >= 0)
                        tbTotBalCredit.Foreground = new SolidColorBrush(Colors.Black);
                    else
                        tbTotBalCredit.Foreground = new SolidColorBrush(Colors.Red);
                }
                return calcBal;
            }
        }


        private TextBlock tbTotBalCredit = null;
        public void TotalBalanceCredit_Loaded(object source)
        {
            if (source != null)
            {
                tbTotBalCredit = source as TextBlock;
            }
        }

        private decimal _TotalSupportFund;
        public decimal TotalSupportFund
        {
            get
            {
                return _TotalSupportFund;
            }
            set
            {
                _TotalSupportFund = value;
                NotifyOfPropertyChange(() => TotalSupportFund);
            }
        }
        private decimal _TotalPatientPayment_NotFinalized;
        public decimal TotalPatientPayment_NotFinalized
        {
            get
            {
                return _TotalPatientPayment_NotFinalized;
            }
            set
            {
                _TotalPatientPayment_NotFinalized = value;
                NotifyOfPropertyChange(() => TotalPatientPayment_NotFinalized);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }

        private decimal _TotalPatientPaid_NotFinalized;
        public decimal TotalPatientPaid_NotFinalized
        {
            get
            {
                return _TotalPatientPaid_NotFinalized;
            }
            set
            {
                _TotalPatientPaid_NotFinalized = value;
                NotifyOfPropertyChange(() => TotalPatientPaid_NotFinalized);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
                NotifyOfPropertyChange(() => TotalPatientPaid_Finalized);
            }
        }

        private decimal _TotalSupportFund_NotFinalized;
        public decimal TotalSupportFund_NotFinalized
        {
            get
            {
                return _TotalSupportFund_NotFinalized;
            }
            set
            {
                _TotalSupportFund_NotFinalized = value;
                NotifyOfPropertyChange(() => TotalSupportFund_NotFinalized);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }

        public decimal TotalPatientPaid_Finalized
        {
            get
            {
                return _sumOfAdvance - (_TotalPatientPaid_NotFinalized + _TotalRefundMoney);
            }
        }

        #region Properties
        private bool _gEnableRevertDischargeBtn = false;
        public bool gEnableRevertDischargeBtn
        {
            get
            {
                return _gEnableRevertDischargeBtn;
            }
            set
            {
                if (_gEnableRevertDischargeBtn == value) return;
                _gEnableRevertDischargeBtn = value;
                NotifyOfPropertyChange(() => gEnableRevertDischargeBtn);
            }
        }
        #endregion

        //▼====== #003
        public void Handle(SetInPatientInfoAndRegistrationForInPatientDischarge message)
        {
            if (message != null && Registration_DataStorage.CurrentPatientRegistration != null)
            {
                //▼===== 20191012 TTM: Do đây là màn hình xài chung nên Registration_DataStorage đã có dữ liệu từ tìm kiếm của chẩn đoán => Không cần load lại
                //OpenRegistration(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                OpenRegistrationNew();
            }
        }
        //▲====== #003
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
        public void OpenRegistrationNew()
        {
            Coroutine.BeginExecute(DoOpenRegistrationNew(), null, (o, e) => { });
        }
        public IEnumerator<IResult> DoOpenRegistrationNew()
        {
            CurrentRegistration = Registration_DataStorage.CurrentPatientRegistration;

            DischargeDateFromInPtRec = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.DischargeDate;

            ShowOldRegistration(CurrentRegistration);

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo_Action);
        }
        private bool _UseOnlyDailyDiagnosis = Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis;
        public bool UseOnlyDailyDiagnosis
        {
            get
            {
                return _UseOnlyDailyDiagnosis;
            }
            set
            {
                if (_UseOnlyDailyDiagnosis == value)
                {
                    return;
                }
                _UseOnlyDailyDiagnosis = value;
                NotifyOfPropertyChange(() => UseOnlyDailyDiagnosis);
            }
        }
        private DiagnosisTreatment CurrentDiagnosisTreatment { get; set; }
        private IList<DiagnosisTreatment> DiagnosisTreatmentCollection { get; set; }
        public void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection)
        {
            CurrentDiagnosisTreatment = null;
            DiagnosisTreatmentCollection = aDiagnosisTreatmentCollection;
            if (DiagnosisTreatmentCollection != null && DiagnosisTreatmentCollection.Count > 0)
            {
                if (DiagnosisTreatmentCollection.Any(x => x.ConfirmedForPrescriptID.GetValueOrDefault(0) > 0
                                                    || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
                {
                    CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.First(x => x.ConfirmedForPrescriptID.GetValueOrDefault(0) > 0
                                                                                || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS);
                }
                else
                {
                    CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.OrderByDescending(x => x.DTItemID).FirstOrDefault();
                }
            }
        }
        public void ConfirmDiagnosisTreatmentCmd()
        {
            Coroutine.BeginExecute(CallConfirmDiagnosisTreatmentCmd());
        }
        private IEnumerator<IResult> CallConfirmDiagnosisTreatmentCmd()
        {
            if (ShowConfirmDischargeBtn && (DiagnosisTreatmentCollection == null || DiagnosisTreatmentCollection.Count == 0))
            {
                var CurrentTask = new GenericCoRoutineTask(LoadDiagnosisTreatmentCollectionTask, DischargeInfoContent.Registration.PtRegistrationID);
                yield return CurrentTask;
            }
            CurrentDiagnosisTreatment = null;
            IConfirmDiagnosisTreatment DialogView = Globals.GetViewModel<IConfirmDiagnosisTreatment>();
            DialogView.ApplyDiagnosisTreatmentCollection(DiagnosisTreatmentCollection);
            //20191104 TBL: Lấy khoa đang nằm không phải là khoa tạm
            if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.InPatientDeptDetails != null && CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0)
            {
                DialogView.DeptID = CurrentRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => !x.IsTemp).First().DeptLocation.DeptID;
            }
            //20191115 TBL: Chỉ truyền cờ này khi là màn hình Toa thuốc xuất viện hoặc Xuất viện BN
            DialogView.IsPreAndDischargeView = true;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, new Size(1200, 600));
            CurrentDiagnosisTreatment = DialogView.CurrentDiagnosisTreatment;
            if (CurrentDiagnosisTreatment != null)
            {
                CurrentRegistration.AdmissionInfo.DiagnosisTreatmentInfo = CurrentDiagnosisTreatment;
            }
        }
        public void LoadDiagnosisTreatmentCollectionTask(GenericCoRoutineTask aGenTask, object aPtRegistrationID)
        {
            long PtRegistrationID = Convert.ToInt64(aPtRegistrationID);
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagnosisTreatment_InPt_ByPtRegID(PtRegistrationID, null, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ItemCollection = mContract.EndGetDiagnosisTreatment_InPt_ByPtRegID(asyncResult);
                            if (ItemCollection == null)
                            {
                                DiagnosisTreatmentCollection = new List<DiagnosisTreatment>();
                            }
                            else
                            {
                                DiagnosisTreatmentCollection = ItemCollection.ToList();
                                if (DiagnosisTreatmentCollection.Any(x => x.ConfirmedForPrescriptID.GetValueOrDefault(0) > 0))
                                {
                                    CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.First(x => x.ConfirmedForPrescriptID.GetValueOrDefault(0) > 0);
                                    CurrentRegistration.AdmissionInfo.DischargeStatus = CurrentDiagnosisTreatment.OrientedTreatment;
                                    CurrentRegistration.AdmissionInfo.TreatmentDischarge = 
                                        string.IsNullOrEmpty(CurrentRegistration.AdmissionInfo.TreatmentDischarge) ? CurrentDiagnosisTreatment.Treatment 
                                        : CurrentRegistration.AdmissionInfo.TreatmentDischarge;
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
                            if (aGenTask != null)
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        //20191120 TBL: Khi bên màn hình Toa thuốc XV được lưu thành công thì chẩn đoán của toa thuốc sẽ thành xác nhận chẩn đoán của xuất viện
        public void Handle(LoadDiagnosisTreatmentConfirmedForPrescript message)
        {
            if (message != null && Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count > 0)
            {
                foreach (var record in Registration_DataStorage.PatientServiceRecordCollection)
                {
                    foreach (var dt in record.DiagnosisTreatments)
                    {
                        if (dt.ConfirmedForPrescriptID.GetValueOrDefault(0) > 0)
                        {
                            CurrentDiagnosisTreatment = dt;
                            DischargeInfoContent.Registration.AdmissionInfo.DiagnosisTreatmentInfo = dt;
                            break;
                        }
                    }
                }
            }
        }
        //▼===== #005
        public void SetLastDiagnosisForConfirm()
        {
            if (DiagnosisTreatmentCollection != null && DiagnosisTreatmentCollection.Count > 0
                && !DiagnosisTreatmentCollection.Any(x => x.ConfirmedForPrescriptID > 0
                    || x.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS))
            {
                CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.OrderByDescending(x => x.DTItemID).FirstOrDefault();
                //▼===== 20200813 TTM: Khi lấy chẩn đoán cuối cùng cho màn hình xuất viện thì lấy chẩn đoán đó hiển thị ở ô chẩn đoán của 
                //                     DischargeInfoView luôn.
                if (DischargeInfoContent != null
                    && DischargeInfoContent.Registration != null
                    && DischargeInfoContent.Registration.AdmissionInfo != null)
                {
                    DischargeInfoContent.Registration.AdmissionInfo.DiagnosisTreatmentInfo = CurrentDiagnosisTreatment;
                    DischargeInfoContent.Registration.AdmissionInfo.DischargeStatus = CurrentDiagnosisTreatment.OrientedTreatment;
                    CurrentRegistration.AdmissionInfo.TreatmentDischarge = string.IsNullOrEmpty(CurrentRegistration.AdmissionInfo.TreatmentDischarge) 
                        ? CurrentDiagnosisTreatment.Treatment : CurrentRegistration.AdmissionInfo.TreatmentDischarge;
                }
                //▲===== 
            }
        }
        //▲===== #005

        //▼=== #009
        private bool _eCMFInfo = false;
        public bool eCMFInfo
        {
            get
            {
                return _eCMFInfo;
            }
            set
            {
                if (_eCMFInfo == value)
                {
                    return;
                }
                _eCMFInfo = value;
                NotifyOfPropertyChange(() => eCMFInfo);
            }
        }
        //▲==== #009

        public void SaveCMFInfoCmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null || Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null)
            {
                MessageBox.Show(eHCMSResources.A0633_G1_Msg_InfoKhongCoDKBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z1527_G1_HuyXNhanXV))
            {
                return;
            }
            if (CurrentRegistration.AdmissionInfo.DischargeDate == null && CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate == null)
            {
                MessageBox.Show(eHCMSResources.A0218_G1_Msg_InfoCNhatTThaiXVFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //▼==== #008
            if (Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles != null)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles.CheckMedicalFileID != 0 
                    && Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles.V_CheckMedicalFilesStatus != (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So)
                {
                    MessageBox.Show("Hồ sơ đã được gửi lên KHTH");
                    return;
                }
            }

            if (Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles == null 
                || Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles.CheckMedicalFileID == 0)
            {
                Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles = new CheckMedicalFiles
                {
                    CheckMedicalFileID = 0,
                    PtRegistrationID = CurrentRegistration.PtRegistrationID,
                    SendRequestStaffID = Globals.LoggedUserAccount.StaffID ?? 0,
                    V_CheckMedicalFilesStatus = (long)AllLookupValues.V_CheckMedicalFilesStatus.Tao_Moi
                };
            }
            else
            {
                Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles = new CheckMedicalFiles
                {
                    CheckMedicalFileID = 0,
                    PtRegistrationID = CurrentRegistration.PtRegistrationID,
                    SendRequestStaffID = Globals.LoggedUserAccount.StaffID ?? 0,
                    V_CheckMedicalFilesStatus = (long)AllLookupValues.V_CheckMedicalFilesStatus.Cho_Duyet_Lai
                };
            }
            //▲==== #008
            if (Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles != null)
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PatientRegistrationServiceClient())
                        {
                            var client = serviceFactory.ServiceInstance;
                            client.BeginSaveMedicalFiles(Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles, false, (long)CurrentRegistration.V_RegistrationType
                                , 0, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var bOK = client.EndSaveMedicalFiles(out long CheckMedicalFileIDNew, asyncResult);
                                        if (bOK)
                                        {
                                            MessageBox.Show("Gửi hồ sơ lên KHTH thành công");
                                            CurrentRegistration.CheckMedicalFiles.CheckMedicalFileID = CheckMedicalFileIDNew;
                                            Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles.CheckMedicalFileID = CheckMedicalFileIDNew;//==== #008
                                            eCMFInfo = false; //==== #009
                                            //▼==== #012
                                            OpenRegistration(CurrentRegistration.PtRegistrationID);
                                            //▲==== #012
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
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
                        MessageBox.Show(ex.Message);
                        this.HideBusyIndicator();
                    }
                });

                t.Start();
            }
        }

        public void PhieuXNDT_CovidCmd()
        {
            if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0)
            {
                if (!CurrentRegistration.AdmissionInfo.IsTreatmentCOVID)
                {
                    MessageBox.Show("Bệnh nhân không điều trị Covid không thể tạo giấy xác nhận điều trị Covid");
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
                {
                    reportVm.ID = CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID;
                    reportVm.eItem = ReportName.XRpt_XacNhan_DieuTri_Covid;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
        public void PhieuGBNDT_CovidCmd()
        {
            if (CurrentRegistration != null && CurrentRegistration.AdmissionInfo != null && CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0)
            {
                if (!CurrentRegistration.AdmissionInfo.IsTreatmentCOVID)
                {
                    MessageBox.Show("Bệnh nhân không điều trị Covid không thể tạo giấy xác nhận điều trị Covid");
                    return;
                }
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
                {
                    reportVm.ID = CurrentRegistration.AdmissionInfo.InPatientAdmDisDetailID;
                    reportVm.eItem = ReportName.XRpt_GiaoNhan_BenhNhan_Covid;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }
        public void btnConfirm()
        {
            IsLoading = true;
            if ((GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.APIKey == null || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token))
                || (GlobalsNAV.gLoggedHIAPIUser != null && GlobalsNAV.gLoggedHIAPIUser.APIKey != null && GlobalsNAV.gLoggedHIAPIUser.APIKey.expires_in <= DateTime.Now))
            {
                GlobalsNAV.LoginHIAPI();
            }
            if (!Globals.ServerConfigSection.CommonItems.NewMethodToReport4210)
            {
                Coroutine.BeginExecute(CreateHIReportXml_Routine());
            }
            else
            {
                Coroutine.BeginExecute(CreateHIReportOutInPtXml_Routine());
            }
        }
        private IEnumerator<IResult> CreateHIReportXml_Routine()
        {
            if (CurrentRegistration == null || CurrentRegistration.HIReportID != 0) yield break;

            //this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Join(",", string.Format("{0}-{1}", CurrentRegistration.RegistrationType.RegTypeID == 2 ? 3 : CurrentRegistration.RegistrationType.RegTypeID, CurrentRegistration.PtRegistrationID));

            var mCreateHIReportFileTask = new GenericCoRoutineTask(CreateHIReportFileTask, mTitle);
            yield return mCreateHIReportFileTask;

            HealthInsuranceReport mHealthInsuranceReport = mCreateHIReportFileTask.GetResultObj(0) as HealthInsuranceReport;

            var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
            yield return mCreateHIReportXmlTask;

            HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
            mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
            mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
            mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

            var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
            yield return mUpdateHIReportTask;

            CurrentRegistration.HIReportID = mHealthInsuranceReport.HIReportID;
            CurrentRegistration.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
            CurrentRegistration.V_ReportStatus = new Lookup();
            CurrentRegistration.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
            CurrentRegistration.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;

            //MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

            //this.HideBusyIndicator();
        }
        private IEnumerator<IResult> CreateHIReportOutInPtXml_Routine()
        {
            if (CurrentRegistration == null || CurrentRegistration.HIReportID != 0) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Join(",", string.Format("{0}-{1}", CurrentRegistration.RegistrationType.RegTypeID == 2 ? 3: CurrentRegistration.RegistrationType.RegTypeID, CurrentRegistration.PtRegistrationID));

            var mCreateHIReportFileTask = new GenericCoRoutineTask(CreateHIReportOutInPtFileTask, mTitle);
            yield return mCreateHIReportFileTask;

            HealthInsuranceReport mHealthInsuranceReport = mCreateHIReportFileTask.GetResultObj(0) as HealthInsuranceReport;

            if (mHealthInsuranceReport != null)
            {
                if (mHealthInsuranceReport.HIReportID > 0)
                {
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    CurrentRegistration.HIReportID = mHealthInsuranceReport.HIReportID;
                    CurrentRegistration.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                    CurrentRegistration.V_ReportStatus = new Lookup();
                    CurrentRegistration.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                    CurrentRegistration.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;

                }
                if (mHealthInsuranceReport.HIReportOutPt > 0)
                {
                    mHealthInsuranceReport.HIReportID = mHealthInsuranceReport.HIReportOutPt;
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    CurrentRegistration.HIReportID = mHealthInsuranceReport.HIReportID;
                    CurrentRegistration.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                    CurrentRegistration.V_ReportStatus = new Lookup();
                    CurrentRegistration.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                    CurrentRegistration.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;

                }
            }
            //MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

            //this.HideBusyIndicator();
        }
        private string _OutputErrorMessage;
        public string OutputErrorMessage
        {
            get => _OutputErrorMessage; set
            {
                _OutputErrorMessage = value;
                NotifyOfPropertyChange(() => OutputErrorMessage);
            }
        }
        private string gIAPISendHIReportAddressParams = "api/egw/guiHoSoGiamDinh4210?token={0}&id_token={1}&username={2}&password={3}&loaiHoSo=3&maTinh={4}&maCSKCB={5}";
        private string gIAPISendHIReportAddress = "http://egw.baohiemxahoi.gov.vn/";
        private void CreateHIReportFileTask(GenericCoRoutineTask genTask, object aRegistrationIDList)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        HealthInsuranceReport mHealthInsuranceReport = new HealthInsuranceReport { Title = string.Format("BC-{0}", aRegistrationIDList.ToString()), RegistrationIDList = aRegistrationIDList.ToString(), V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID }, V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Pending, FromDate = Globals.GetCurServerDateTime(), ToDate = Globals.GetCurServerDateTime() };
                        contract.BeginCreateHIReport_V2(mHealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mHIReportID;
                                    var mResultVal = contract.EndCreateHIReport_V2(out mHIReportID, asyncResult);
                                    if (!mResultVal || mHIReportID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        IsLoading = false;
                                        //this.HideBusyIndicator();
                                    }
                                    else
                                    {
                                        mHealthInsuranceReport.HIReportID = mHIReportID;
                                        genTask.AddResultObj(mHealthInsuranceReport);
                                        genTask.ActionComplete(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Catch => {0}", OutputErrorMessage));
                                    //MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                    //this.HideBusyIndicator();
                                    IsLoading = false;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    //this.HideBusyIndicator();
                    IsLoading = false;
                }
            });
            t.Start();
        }
        private void CreateHIReportXmlTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIXmlReport9324_AllTab123_InOneRpt((aHealthInsuranceReport as HealthInsuranceReport).HIReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var ReportStream = contract.EndGetHIXmlReport9324_AllTab123_InOneRpt(asyncResult);
                                    string mHIAPICheckHICardAddress = string.Format(gIAPISendHIReportAddressParams, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password, Globals.ServerConfigSection.Hospitals.HospitalCode.Length < 2 ? "" : Globals.ServerConfigSection.Hospitals.HospitalCode.Substring(0, 2), Globals.ServerConfigSection.Hospitals.HospitalCode);
                                    string mRestJson = GlobalsNAV.GetRESTServiceJSon(gIAPISendHIReportAddress, mHIAPICheckHICardAddress, ReportStream);
                                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = GlobalsNAV.ConvertJsonToObject<HIAPIUploadHIReportXmlResult>(mRestJson);
                                    if (mHIAPIUploadHIReportXmlResult.maKetQua == 200)
                                    {
                                        genTask.AddResultObj(mHIAPIUploadHIReportXmlResult);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        var mErrorMessage = string.IsNullOrEmpty(mHIAPIUploadHIReportXmlResult.maGiaoDich) ? GetErrorMessageFromErrorCode(mHIAPIUploadHIReportXmlResult.maKetQua) : mHIAPIUploadHIReportXmlResult.maGiaoDich;
                                        if (!string.IsNullOrEmpty(mErrorMessage))
                                        {
                                            mErrorMessage = string.Format(" - {0}", mErrorMessage);
                                        }
                                        OutputErrorMessage += Environment.NewLine + string.Format("{0}: {1}{2}", eHCMSResources.T0074_G1_I, mHIAPIUploadHIReportXmlResult.maKetQua, mErrorMessage);
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Else => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        //this.HideBusyIndicator();
                                        IsLoading = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Catch => {0}", ex.Message));
                                    genTask.ActionComplete(false);
                                    //this.HideBusyIndicator();
                                    IsLoading = false;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    //this.HideBusyIndicator();
                    IsLoading = false;
                }
            });
            t.Start();
        }
        private void UpdateHIReportTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateHIReportStatus(aHealthInsuranceReport as HealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndUpdateHIReportStatus(asyncResult))
                                    {
                                        genTask.ActionComplete(true);
                                        Discharge();
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(false);
                                        //this.HideBusyIndicator();
                                        IsLoading = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                    //this.HideBusyIndicator();
                                    IsLoading = false;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    //this.HideBusyIndicator();
                    IsLoading = false;
                }
            });
            t.Start();
        }
        private void CreateHIReportOutInPtFileTask(GenericCoRoutineTask genTask, object aRegistrationIDList)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        HealthInsuranceReport mHealthInsuranceReport = new HealthInsuranceReport { Title = string.Format("BC-{0}", aRegistrationIDList.ToString()), RegistrationIDList = aRegistrationIDList.ToString(), V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID }, V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Pending, FromDate = Globals.GetCurServerDateTime(), ToDate = Globals.GetCurServerDateTime() };
                        contract.BeginCreateHIReportOutInPt(mHealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mHIReportID;
                                    long mHIReportOutPt;
                                    var mResultVal = contract.EndCreateHIReportOutInPt(out mHIReportID, out mHIReportOutPt, asyncResult);
                                    if (mResultVal || mHIReportID > 0 || mHIReportOutPt > 0)
                                    {
                                        if (mHIReportID > 0)
                                        {
                                            mHealthInsuranceReport.HIReportID = mHIReportID;
                                        }
                                        if (mHIReportOutPt > 0)
                                        {
                                            mHealthInsuranceReport.HIReportOutPt = mHIReportOutPt;
                                        }
                                        genTask.AddResultObj(mHealthInsuranceReport);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        //this.HideBusyIndicator();
                                        IsLoading = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Catch => {0}", OutputErrorMessage));
                                    genTask.ActionComplete(false);
                                    //this.HideBusyIndicator();
                                    IsLoading = false;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    //this.HideBusyIndicator();
                    IsLoading = false;
                }
            });

            t.Start();
        }
        private string GetErrorMessageFromErrorCode(int aErrorCode)
        {
            switch (aErrorCode)
            {
                case 1001:
                    return "Kích thước file quá lớn (20MB trong khoảng thời gian từ 8 giờ đến 11 giờ và từ 14 giờ đến 19 giờ từ thứ 2 đến thứ 6,100MB với các thời gian khác)";
                case 205:
                    return "Lỗi sai định dạng tham số";
                case 401:
                    return "Sai tài khoản hoặc mật khẩu";
                case 123:
                    return "Sai định dạng file";
                case 124:
                    return "Lỗi khi lưu dữ liệu( file sẽ được tự động gửi lại)";
                case 701:
                    return "Lỗi thời gian gửi,thời gian quyết toán chỉ dc trong tháng hoặc đến mồng 5 tháng sau";
                case 500:
                    return "Lỗi trong quá trình gửi dữ liệu.Vui lòng liên hệ với nhân viên hỗ trợ để được hướng dẫn cụ thể";
                default:
                    return "";
            }
        }


        public bool ShowPrintTemp01KBCB
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.PrintTemp01KBCB;
            }
        }

        public void Print01KBCBCmd()
        {
            if (CurrentRegistration == null)
            {
                return;
            }
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentRegistration.PtRegistrationID;
                proAlloc.eItem = ReportName.TEMP12_TONGHOP;
                proAlloc.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
                if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                {
                    proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                }
                else
                {
                    proAlloc.StaffFullName = "";
                }
            };
            GlobalsNAV.ShowDialog(onInitDlg, null, false, true, new Size(1500, 1000));
        }
        //▼=== #012
        private bool _eSaveDischarge = false;
        public bool eSaveDischarge
        {
            get
            {
                return _eSaveDischarge;
            }
            set
            {
                if (_eSaveDischarge == value)
                {
                    return;
                }
                _eSaveDischarge = value;
                NotifyOfPropertyChange(() => eSaveDischarge);
            }
        }
        
        private void CheckMedicalFilesForDischarge()
        {
            if (CurrentRegistration.AdmissionInfo.DischargeDate == null && CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate == null)
            {
                eSaveDischarge = true;
                gEnableRevertDischargeBtn = false;
            }
            else
            {
                eSaveDischarge = false;
                gEnableRevertDischargeBtn = true;
            }

            //if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles != null
            //    && Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles.CheckMedicalFileID == 0
            //    && CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate != null)
            if (CurrentRegistration.CheckMedicalFiles != null
                && CurrentRegistration.CheckMedicalFiles.CheckMedicalFileID == 0
                && CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate != null)
            {
                if (!eSaveDischarge)
                {
                    eCMFInfo = true;
                }
                else
                {
                    eCMFInfo = false;
                }
            }
            else
            {
                //if (Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So)
                if (CurrentRegistration.CheckMedicalFiles.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So)
                {
                    if (CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate != null)
                    {
                        if (CurrentRegistration.AdmissionInfo.IsReturning)
                        {
                            eCMFInfo = false;
                        }
                        else
                        {
                            eCMFInfo = true;
                        }
                        gEnableRevertDischargeBtn = true;
                    }
                    else
                    {
                        if (!eSaveDischarge)
                        {
                            eCMFInfo = true;
                        }
                        else
                        {
                            eCMFInfo = false;
                        }
                        gEnableRevertDischargeBtn = false;
                    }
                }
                else
                {
                    eCMFInfo = false;
                    gEnableRevertDischargeBtn = false;
                }
            }
        }

        public void SaveCMFInfo()
        {
            if (CurrentRegistration == null || CurrentRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurrentRegistration.AdmissionInfo == null || CurrentRegistration.AdmissionInfo.DischargeDepartment == null)
            {
                MessageBox.Show(eHCMSResources.A0387_G1_Msg_InfoChuaChonKhoaXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ErrorMessages = "";
            ConfirmMessages = "";

            string errorMsg = "";
            string confirmMsg = "";

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckBeforeDischarge(CurrentRegistration.PtRegistrationID, CurrentRegistration.AdmissionInfo.DischargeDepartment.DeptID
                            , CurrentRegistration.AdmissionInfo.ConfirmNotTreatedAsInPt
                            , (CurrentRegistration.AdmissionInfo.DischargeDate == null ? CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate : CurrentRegistration.AdmissionInfo.DischargeDate)
                            //▼==== #013
                            , true
                            //▲==== #013
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var result = contract.EndCheckBeforeDischarge(out errorMsg, out confirmMsg, asyncResult);

                                    if (result)
                                    {
                                        ErrorMessages = errorMsg;
                                        ConfirmMessages = confirmMsg;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A078_G1_Msg_InfoKTraXVFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        bContinue = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogError(ex.Message);
                                    bContinue = false;
                                }
                                finally
                                {
                                    if (ErrorMessages != "")
                                    {
                                        errorMessageBox = new WarningWithConfirmMsgBoxTask(ErrorMessages, "", false);
                                        errorMessageBox.Execute(null);
                                    }
                                    else
                                    {
                                        SaveCMFInfoCmd();
                                    }
                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });

            t.Start();

        }
        //▲==== #012
        private SummaryMedicalRecords _CurSummaryMedicalRecords;
        public SummaryMedicalRecords CurSummaryMedicalRecords
        {
            get
            {
                return _CurSummaryMedicalRecords;
            }
            set
            {
                if (_CurSummaryMedicalRecords != value)
                {
                    _CurSummaryMedicalRecords = value;
                    NotifyOfPropertyChange(() => CurSummaryMedicalRecords);
                }
            }
        }
        public void PhieuTT_HSBACmd()
        {
            if (CurrentRegistration == null || CurrentRegistration.AdmissionInfo == null)
            {
                MessageBox.Show(eHCMSResources.A0633_G1_Msg_InfoKhongCoDKBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentRegistration.AdmissionInfo.DischargeDetailRecCreatedDate == null)
            {
                MessageBox.Show("Chưa có thông tin xuất viện không thể tạo phiếu tóm tắt quá trình điều trị", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            GetSummaryMedicalRecords_ByPtRegID(CurrentRegistration.PtRegistrationID, (long)CurrentRegistration.V_RegistrationType);
        }
        private void GetSummaryMedicalRecords_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetSummaryMedicalRecords_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                SummaryMedicalRecords result = contract.EndGetSummaryMedicalRecords_ByPtRegID(asyncResult);
                                CurSummaryMedicalRecords = result ?? new SummaryMedicalRecords();
                                //IsHaveHSBA = CurSummaryMedicalRecords.SummaryMedicalRecordID > 0;

                                Action<ISummaryMedicalRecords> onInitDlg = delegate (ISummaryMedicalRecords summaryMedicalRecords)
                                {
                                    this.ActivateItem(summaryMedicalRecords);
                                    summaryMedicalRecords.PtRegistration = CurrentRegistration;
                                    summaryMedicalRecords.SetCurrentInformation(CurSummaryMedicalRecords.DeepCopy());
                                };
                                GlobalsNAV.ShowDialog<ISummaryMedicalRecords>(onInitDlg);
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

        //▼===== #014
        public void PrintCmdNew()
        {
            if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID > 0 && (long)CurrentRegistration.V_RegistrationType > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
                {
                    reportVm.RegistrationID = CurrentRegistration.PtRegistrationID;
                    reportVm.V_RegistrationType = (long)CurrentRegistration.V_RegistrationType;
                    reportVm.eItem = ReportName.XRpt_DisChargePapers;
                };
                GlobalsNAV.ShowDialog(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
        //▲===== #014
    }
}
