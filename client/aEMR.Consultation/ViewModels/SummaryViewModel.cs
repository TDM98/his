using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using System.Windows;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.ServiceClient.Consultation_PCLs;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
 * 20190519 #001 TTM:   BM 0006857: Cho phép nhập tình trạng thể chất dấu hiệu sinh tồn trước khi bác sĩ chẩn đoán và ra toa (Thư ký y khoa sẽ nhập ở màn hình thông tin chung).
 * 20191121 #002 TTM:   BM 0019594: Fix lỗi không thêm được dị ứng, tình trạng thể chất nội trú
 * 20230201 #003 DatTB: Thêm trường dữ liệu về KSNK trong phần Thông tin chung NB nội trú
 * 20230318 #004 DatTB: Thêm biến lưu theo lần nhập viện
 * 20230503 #005 DatTB: Thêm this.HideBusyIndicator() khi lỗi cho các function
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ISummary)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SummaryViewModel : ViewModelBase, ISummary
        , IHandle<ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<Re_ReadAllergiesEvent>
        , IHandle<Re_ReadWarningEvent>
        , IHandle<RiskFactorSaveCompleteEvent>
        , IHandle<CommonClosedPhysicalForSummaryEvent>
        , IHandle<LoadGeneralInfoPageEvent>
        , IHandle<ShowSummaryInPT>
        , IHandle<SetPhysicalForSummary_InPtEvent>
        //▼===== #003
        , IHandle<SaveInfectionControlCompleteEvent>
        //▲===== #003
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get 
            { 
                //return _isLoading; 
                return false;
            }
            set
            {
                if (_isLoading!=value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        [ImportingConstructor]
        public SummaryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            PtSumDiagList = new PagedSortableCollectionView<DiagnosisTreatment>();
            PtSumDiagList.PageSize = 15;
            PtSumDiagList.OnRefresh += PtSumDiagList_OnRefresh;
            curRiskFactors = new RiskFactors();
            //▼===== #003
            curIC_MRBacteria = new InfectionControl();
            curIC_HosInfection = new InfectionControl();
            //▲===== #003
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                InitData(Registration_DataStorage.CurrentPatient);
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


        private void PtSumDiagList_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            InitData(PatientInfo);
        }

        #region Properties Member

        enum AllergyType
        {
            Drug = 0,
            DrugClass = 1,
            Others = 2
        };

        private Patient _PatientInfo;
        public Patient PatientInfo
        {
            get { return _PatientInfo; }
            set
            {
                if (_PatientInfo != value)
                {
                    _PatientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }

        private PatientRegistration _curPatientRegistration;
        public PatientRegistration curPatientRegistration
        {
            get
            {
                return _curPatientRegistration;
            }
            set
            {
                if (_curPatientRegistration == value)
                    return;
                _curPatientRegistration = value;
                NotifyOfPropertyChange(() => curPatientRegistration);
            }
        }

        private ObservableCollection<MDAllergy> _ptAllergyList;
        public ObservableCollection<MDAllergy> PtAllergyList
        {
            get
            {
                return _ptAllergyList;
            }
            set
            {
                if (_ptAllergyList != value)
                {
                    _ptAllergyList = value;
                    NotifyOfPropertyChange(() => PtAllergyList);
                }
            }
        }


        private MDWarning _ObjMDWarnings_ByPatientID;
        public MDWarning ObjMDWarnings_ByPatientID
        {
            get
            {
                return _ObjMDWarnings_ByPatientID;
            }
            set
            {
                if (_ObjMDWarnings_ByPatientID != value)
                {
                    _ObjMDWarnings_ByPatientID = value;
                    NotifyOfPropertyChange(() => ObjMDWarnings_ByPatientID);
                }
            }
        }




        private MDAllergy _selectedPtAllergies;
        public MDAllergy SelectedPtAllergies
        {
            get
            {
                return _selectedPtAllergies;
            }
            set
            {
                if (_selectedPtAllergies != value)
                {
                    _selectedPtAllergies = value;
                    NotifyOfPropertyChange(() => SelectedPtAllergies);
                }
            }
        }

        private string _sPtWarningList;
        public string SPtWarningList
        {
            get
            {
                return _sPtWarningList;
            }
            set
            {
                if (_sPtWarningList != value)
                {
                    _sPtWarningList = value;
                    NotifyOfPropertyChange(() => SPtWarningList);
                }
            }
        }

        private PhysicalExamination _pPhyExamItem;
        public PhysicalExamination PtPhyExamItem
        {
            get
            {
                return _pPhyExamItem;
            }
            set
            {
                if (_pPhyExamItem != value)
                {
                    _pPhyExamItem = value;
                    NotifyOfPropertyChange(() => PtPhyExamItem);
                }
            }
        }

        private PagedSortableCollectionView<DiagnosisTreatment> _PtSumDiagList;
        public PagedSortableCollectionView<DiagnosisTreatment> PtSumDiagList
        {
            get
            {
                return _PtSumDiagList;
            }
            set
            {
                if (_PtSumDiagList != value)
                {
                    _PtSumDiagList = value;
                    NotifyOfPropertyChange(() => PtSumDiagList);
                }
            }
        }

        private PatientServiceRecord _selectedPtConsultation;
        public PatientServiceRecord SelectedPtConsultation
        {
            get
            {
                return _selectedPtConsultation;
            }
            set
            {
                if (_selectedPtConsultation != value)
                {
                    _selectedPtConsultation = value;
                    NotifyOfPropertyChange(() => SelectedPtConsultation);
                }
            }
        }

        private PatientServiceRecord _SelectedPtDiagnosic;
        public PatientServiceRecord SelectedPtDiagnosic
        {
            get
            {
                return _SelectedPtDiagnosic;
            }
            set
            {
                if (_SelectedPtDiagnosic != value)
                {
                    _SelectedPtDiagnosic = value;
                    NotifyOfPropertyChange(() => SelectedPtDiagnosic);
                }
            }
        }

        private long _PatientID;
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(() => PatientID);
                }
            }
        }

        private bool _canSelectClearChk;
        public bool CanSelectClearChk
        {
            get
            {
                return _canSelectClearChk;
            }
            set
            {
                if (_canSelectClearChk != value)
                {
                    _canSelectClearChk = value;
                    NotifyOfPropertyChange(() => CanSelectClearChk);
                }
            }
        }

        private bool _canSelectUnknowChk;
        public bool CanSelectUnknowChk
        {
            get
            {
                return _canSelectUnknowChk;
            }
            set
            {
                if (_canSelectUnknowChk != value)
                {
                    _canSelectUnknowChk = value;
                    NotifyOfPropertyChange(() => CanSelectUnknowChk);
                }
            }
        }

        private bool _isUnknowChecked;
        public bool IsUnknowChecked
        {
            get
            {
                return _isUnknowChecked;
            }
            set
            {

                _isUnknowChecked = value;
                NotifyOfPropertyChange(() => IsUnknowChecked);
            }
        }

        private MDAllergy _allergy;
        public MDAllergy Allergy
        {
            get
            {
                return _allergy;
            }
            set
            {
                if (_allergy != value)
                {
                    _allergy = value;
                    NotifyOfPropertyChange(() => Allergy);
                }
            }
        }

        private ObservableCollection<RiskFactors> _allRiskFactors;
        public ObservableCollection<RiskFactors> allRiskFactors
        {
            get
            {
                return _allRiskFactors;
            }
            set
            {
                if (_allRiskFactors != value)
                {
                    _allRiskFactors = value;
                    NotifyOfPropertyChange(() => allRiskFactors);
                }
            }
        }

        private RiskFactors _curRiskFactors;
        public RiskFactors curRiskFactors
        {
            get
            {
                return _curRiskFactors;
            }
            set
            {
                if (_curRiskFactors != value)
                {
                    _curRiskFactors = value;
                    NotifyOfPropertyChange(() => curRiskFactors);
                }
            }
        }

        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void MDAllergies_ByPatientID(Int64 PatientID, int flag)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDAllergies_ByPatientID(PatientID,flag,  Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndMDAllergies_ByPatientID(asyncResult);

                            string str = "";

                            if (results != null)
                            {
                                if (PtAllergyList == null)
                                {
                                    PtAllergyList = new ObservableCollection<MDAllergy>();
                                }
                                else
                                {
                                    PtAllergyList.Clear();
                                }
                                foreach (MDAllergy p in results)
                                {
                                    PtAllergyList.Add(p);
                                    str += p.AllergiesItems.Trim() + ";";
                                }
                            }
                            if(!string.IsNullOrEmpty(str))
                            {
                                str = str.Substring(0, str.Length - 1);
                            }
                            Globals.Allergies = str;

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void MDWarnings_ByPatientID(Int64 PatientID, int flag)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDWarnings_ByPatientID(PatientID,flag, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var Res = contract.EndMDWarnings_ByPatientID(asyncResult);

                            if (Res.Count>0)
                            {
                                ObjMDWarnings_ByPatientID = Res[0];
                                SPtWarningList = ObjMDWarnings_ByPatientID.WarningItems.Trim();
                                CanSelectClearChk = true;
                            }
                            else
                            {
                                ObjMDWarnings_ByPatientID =new MDWarning();
                                ObjMDWarnings_ByPatientID.PatientID = PatientID;
                                ObjMDWarnings_ByPatientID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                                SPtWarningList = "";
                                CanSelectClearChk = false;
                            }

                            Globals.Warning = SPtWarningList.Trim();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void RiskFactorGet(long? PatientID)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRiskFactorGet(PatientID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            allRiskFactors = contract.EndRiskFactorGet(asyncResult).ToObservableCollection();
                            curRiskFactors = allRiskFactors != null && allRiskFactors.Count > 0 ? 
                                allRiskFactors[0] : new RiskFactors();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        public void hplDelete_Click() 
        {
            DeleteRiskFactor();
        }

        private void DeleteRiskFactor()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRiskFactorDelete(curRiskFactors.RiskFactorID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndRiskFactorDelete(asyncResult);
                            RiskFactorGet((long?)PatientInfo.PatientID);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void InitPhyExam(long patientID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLastPhyExamByPtID(patientID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            PtPhyExamItem = contract.EndGetLastPhyExamByPtID(asyncResult);
                            Globals.curPhysicalExamination = PtPhyExamItem;
                            //if(PtPhyExamItem==null)
                            //    PtPhyExamItem=new PhysicalExamination();
                        
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void InitDiagnosic(long patientID, int pageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            int total = 0;
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisTreatmentListByPtID(patientID,0 ,"",0,null, pageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDiagnosisTreatmentListByPtID(out total, asyncResult);
                            
                            if (results != null)
                            {
                                if (PtSumDiagList == null)
                                {
                                    PtSumDiagList = new PagedSortableCollectionView<DiagnosisTreatment>();
                                }
                                else
                                {
                                    PtSumDiagList.Clear();
                                }
                                PtSumDiagList.TotalItemCount = total;
                                foreach (DiagnosisTreatment p in results)
                                {
                                    PtSumDiagList.Add(p);
                                }
                                NotifyOfPropertyChange(() => PtSumDiagList);
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
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void InitData(Patient p)
        {
            if (p != null)
            {
                PatientInfo = p;
                PatientID = p.PatientID;
                MDAllergies_ByPatientID(PatientInfo.PatientID,1);/*1 la Active*/
                MDWarnings_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/

                //InitDiagnosic(PatientInfo.PatientID, 0, PtSumDiagList.PageSize);
                InitDiagnosic(PatientInfo.PatientID, PtSumDiagList.PageIndex, PtSumDiagList.PageSize);
                //InitPhyExam(PatientInfo.PatientID);
                //KMx: Khi chọn đăng ký để khám là đã load PhyExam rồi, không cần load lại (File: ConsultationModuleViewModel.cs (event ItemSelected<PatientRegistration, PatientRegistrationDetail>) 12/05/2014 17:12).
                PtPhyExamItem = Globals.curPhysicalExamination;
                RiskFactorGet((long?)PatientInfo.PatientID);
                //▼===== #003
                GetIC_MRBacteria((long?)PatientInfo.PatientID);
                GetIC_HosInfection((long?)PatientInfo.PatientID);
                //▲===== #003
            }
        }

        //public void hplClearAllergies(object sender, RoutedEventArgs e)
        //{
        //    if (PtAllergyList != null && PtAllergyList.Count > 0)
        //    {
        //        if (MessageBox.Show("Xóa Hết Dị Ứng!" + Environment.NewLine + " Bạn Có Muốn Xóa Không?", "Xóa Dị Ứng",
        //                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //        {
        //            DeleteAllergy(GetStaffLogin().StaffID);
        //        }
        //    }
        //}

        public void hpkEditAllergiesWarning()
        {
            Action<IAllergies> onInitDlg = delegate (IAllergies vm)
            {
                vm.Allergy = new MDAllergy();
                vm.Allergy.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                vm.Allergy.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                vm.Warning = ObjectCopier.DeepCopy(ObjMDWarnings_ByPatientID);
                vm.PatientID = PatientID;
                vm.CaseOfAllergyType = (long)AllergyType.Drug;
            };
            GlobalsNAV.ShowDialog<IAllergies>(onInitDlg);
        }

        public void hpkClearWarning(object sender, RoutedEventArgs e)
        {
            if (ObjMDWarnings_ByPatientID.WItemID<=0)
                return; 

            if (MessageBox.Show(eHCMSResources.A0153_G1_Msg_ConfXoaCBao, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                MDWarnings_IsDeleted(ObjMDWarnings_ByPatientID);
            }
        }

        public void hpkNewRiskFactor(object sender, RoutedEventArgs e)
        {
            if (PatientID<0) 
            {
                MessageBox.Show(eHCMSResources.A0378_G1_Msg_InfoChuaChonBN);
                return;
            }
            Action<IRiskFactors> onInitDlg = delegate (IRiskFactors vm)
            {
                vm.PatientID = PatientID;
                vm.curRiskFactors = ObjectCopier.DeepCopy(curRiskFactors);
            };
            GlobalsNAV.ShowDialog<IRiskFactors>(onInitDlg);
        }

        private void MDWarnings_IsDeleted(MDWarning Obj)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDWarnings_IsDeleted(ObjMDWarnings_ByPatientID,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndMDWarnings_IsDeleted(out Result, asyncResult);
                            
                            if(Result=="0")
                            {
                                MessageBox.Show(eHCMSResources.K0472_G1_XoaCBaoFail);
                            }
                            else
                            {
                                MDWarnings_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/    
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
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }
        

        #region IHandle<CommonClosedAlergyEvent> Members
       

        #endregion
        

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mThongTinChung_TimBN=Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TimBN
                        , (int)ePermission.mView);
            mThongTinChung_TimDK =Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TimDK
                        , (int)ePermission.mView);
	        mThongTinChung_ThongTinBN	=Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_ThongTinBN
                        , (int)ePermission.mView);
	        
	        mThongTinChung_DiUngCanhBao_ChinhSua=Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_DiUngCanhBao_ChinhSua
                        , (int)ePermission.mView);
            mThongTinChung_DiUngCanhBao_ThongTin = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_DiUngCanhBao_ThongTin
                        , (int)ePermission.mView) || mThongTinChung_DiUngCanhBao_ChinhSua;
	        
	        mThongTinChung_TinhTrangTheChat_ChinhSua=Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TinhTrangTheChat_ChinhSua
                        , (int)ePermission.mView);
            mThongTinChung_TinhTrangTheChat_ThongTin = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_TinhTrangTheChat_ThongTin
                        , (int)ePermission.mView) || mThongTinChung_TinhTrangTheChat_ChinhSua;

	        mThongTinChung_ThongTinLienHe_ChinhSua=Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_ThongTinLienHe_ChinhSua
                        , (int)ePermission.mView);
            mThongTinChung_ThongTinLienHe_ThongTin = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_ThongTinLienHe_ThongTin
                        , (int)ePermission.mView) || mThongTinChung_ThongTinLienHe_ChinhSua;
	        mThongTinChung_BHYT_ThongTin=Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_BHYT_ThongTin
                        , (int)ePermission.mView);
            mThongTinChung_XemLanKhamBenh = Globals.CheckAuthorization(Globals.listRefModule
                        , (int)eModules.mConsultation
                        , (int)eConsultation.mPtDashboardSummary
                        , (int)oConsultationEx.mThongTinChung_XemLanKhamBenh
                        , (int)ePermission.mView);
        }
#region Checking Account

        private bool _mThongTinChung_TimBN = true;
        private bool _mThongTinChung_TimDK	= true;
	    private bool _mThongTinChung_ThongTinBN	= true;
	    private bool _mThongTinChung_DiUngCanhBao_ThongTin	= true;
        private bool _mThongTinChung_DiUngCanhBao_ChinhSua = true && Globals.isConsultationStateEdit;
	    private bool _mThongTinChung_TinhTrangTheChat_ThongTin= true;
        private bool _mThongTinChung_TinhTrangTheChat_ChinhSua = true && Globals.isConsultationStateEdit;
	    private bool _mThongTinChung_ThongTinLienHe_ThongTin= true;
	    private bool _mThongTinChung_ThongTinLienHe_ChinhSua= true;
	    private bool _mThongTinChung_BHYT_ThongTin= true;
        private bool _mThongTinChung_XemLanKhamBenh = true;

        public bool mThongTinChung_TimBN
        {
            get
            {
                return _mThongTinChung_TimBN;
            }
            set
            {
                if (_mThongTinChung_TimBN == value)
                    return;
                _mThongTinChung_TimBN = value;
            }
        }
        public bool mThongTinChung_TimDK
        {
            get
            {
                return _mThongTinChung_TimDK;
            }
            set
            {
                if (_mThongTinChung_TimDK == value)
                    return;
                _mThongTinChung_TimDK = value;
            }
        }
        public bool mThongTinChung_ThongTinBN
        {
            get
            {
                return _mThongTinChung_ThongTinBN;
            }
            set
            {
                if (_mThongTinChung_ThongTinBN == value)
                    return;
                _mThongTinChung_ThongTinBN = value;
            }
        }
        public bool mThongTinChung_DiUngCanhBao_ThongTin
        {
            get
            {
                return _mThongTinChung_DiUngCanhBao_ThongTin;
            }
            set
            {
                if (_mThongTinChung_DiUngCanhBao_ThongTin == value)
                    return;
                _mThongTinChung_DiUngCanhBao_ThongTin = value;
                
            }
        }
        public bool mThongTinChung_DiUngCanhBao_ChinhSua
        {
            get
            {
                return _mThongTinChung_DiUngCanhBao_ChinhSua;
            }
            set
            {
                if (_mThongTinChung_DiUngCanhBao_ChinhSua == value)
                    return;
                _mThongTinChung_DiUngCanhBao_ChinhSua = value && Globals.isConsultationStateEdit;
                NotifyOfPropertyChange(() => mThongTinChung_DiUngCanhBao_ChinhSua);
            }
        }
        public bool mThongTinChung_TinhTrangTheChat_ThongTin
        {
            get
            {
                return _mThongTinChung_TinhTrangTheChat_ThongTin;
            }
            set
            {
                if (_mThongTinChung_TinhTrangTheChat_ThongTin == value)
                    return;
                _mThongTinChung_TinhTrangTheChat_ThongTin = value;
            }
        }
        public bool mThongTinChung_TinhTrangTheChat_ChinhSua
        {
            get
            {
                return _mThongTinChung_TinhTrangTheChat_ChinhSua;
            }
            set
            {
                if (_mThongTinChung_TinhTrangTheChat_ChinhSua == value)
                    return;
                _mThongTinChung_TinhTrangTheChat_ChinhSua = value && Globals.isConsultationStateEdit;
                NotifyOfPropertyChange(() => mThongTinChung_TinhTrangTheChat_ChinhSua);
            }
        }
        public bool mThongTinChung_ThongTinLienHe_ThongTin
        {
            get
            {
                return _mThongTinChung_ThongTinLienHe_ThongTin;
            }
            set
            {
                if (_mThongTinChung_ThongTinLienHe_ThongTin == value)
                    return;
                _mThongTinChung_ThongTinLienHe_ThongTin = value;
            }
        }
        public bool mThongTinChung_ThongTinLienHe_ChinhSua
        {
            get
            {
                return _mThongTinChung_ThongTinLienHe_ChinhSua;
            }
            set
            {
                if (_mThongTinChung_ThongTinLienHe_ChinhSua == value)
                    return;
                _mThongTinChung_ThongTinLienHe_ChinhSua = value;
            }
        }
        public bool mThongTinChung_BHYT_ThongTin
        {
            get
            {
                return _mThongTinChung_BHYT_ThongTin;
            }
            set
            {
                if (_mThongTinChung_BHYT_ThongTin == value)
                    return;
                _mThongTinChung_BHYT_ThongTin = value;
            }
        }
        public bool mThongTinChung_XemLanKhamBenh
        {
            get
            {
                return _mThongTinChung_XemLanKhamBenh;
            }
            set
            {
                if (_mThongTinChung_XemLanKhamBenh == value)
                    return;
                _mThongTinChung_XemLanKhamBenh = value;
            }
        }
        //private bool _bhpkEditPhysicalExam = true;
        //private bool _bhpkEditAllergiesWarning = true;
        //private bool _bhpkNewPhysicalExam = true;
        //private bool _bhpkClearWarning = true;
        //private bool _bhpkEditDemographics = true;
        
        //public bool bhpkEditPhysicalExam
        //{
        //    get
        //    {
        //        return _bhpkEditPhysicalExam;
        //    }
        //    set
        //    {
        //        if (_bhpkEditPhysicalExam == value)
        //            return;
        //        _bhpkEditPhysicalExam = value;
        //    }
        //}
        //public bool bhpkEditAllergiesWarning
        //{
        //    get
        //    {
        //        return _bhpkEditAllergiesWarning;
        //    }
        //    set
        //    {
        //        if (_bhpkEditAllergiesWarning == value)
        //            return;
        //        _bhpkEditAllergiesWarning = value;
        //    }
        //}
        //public bool bhpkNewPhysicalExam
        //{
        //    get
        //    {
        //        return _bhpkNewPhysicalExam;
        //    }
        //    set
        //    {
        //        if (_bhpkNewPhysicalExam == value)
        //            return;
        //        _bhpkNewPhysicalExam = value;
        //    }
        //}
        //public bool bhpkClearWarning
        //{
        //    get
        //    {
        //        return _bhpkClearWarning;
        //    }
        //    set
        //    {
        //        if (_bhpkClearWarning == value)
        //            return;
        //        _bhpkClearWarning = value;
        //    }
        //}
        //public bool bhpkEditDemographics
        //{
        //    get
        //    {
        //        return _bhpkEditDemographics;
        //    }
        //    set
        //    {
        //        if (_bhpkEditDemographics == value)
        //            return;
        //        _bhpkEditDemographics = value;
        //    }
        //}
#endregion
        public void hpkEditPhysicalExam()
        {
            Action<IcwPhysiscalExam> onInitDlg = delegate (IcwPhysiscalExam proAlloc)
            {
                proAlloc.PatientID = PatientID;

                if (PtPhyExamItem == null)
                {
                    proAlloc.IsVisibility = Visibility.Collapsed;
                    proAlloc.isEdit = false;
                }
                else
                {
                    proAlloc.PtPhyExamItem = ObjectCopier.DeepCopy(PtPhyExamItem);
                    proAlloc.IsVisibility = Visibility.Visible;
                    proAlloc.isEdit = true;
                }
                //▼====== #001: Lý do cần thêm chỗ này: Vì đã chuyển từ logic cũ (Lấy tất cả Tình trạng thể chất sau đó chọn cái cuối cùng hiển thị => Không hợp lý)
                //              Chuyển sang xài logic mới, mỗi tình trạng thể chất của bệnh nhân đi kèm với một đăng ký. Tức là tình trạng thể chất của bệnh nhân phải đi kèm với đăng ký (Vì tình trạng thể chất của bệnh nhân là ngay lúc
                //              bệnh nhân khám mới chính xác).
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                {
                    proAlloc.PtPhyExamItem.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    proAlloc.PtPhyExamItem.V_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
                }
                //▲====== #001
            };
            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg);
        }


        public void hpkNewPhysicalExam()
        {
            Action<IcwPhysiscalExam> onInitDlg = delegate (IcwPhysiscalExam proAlloc)
            {
                proAlloc.PatientID = PatientID;
                proAlloc.IsVisibility = Visibility.Collapsed;
                proAlloc.PtPhyExamItem = new PhysicalExamination();
            };
            GlobalsNAV.ShowDialog<IcwPhysiscalExam>(onInitDlg);
        }

        public void hpkBedPatientAlloc(object sender, RoutedEventArgs e)
        {
            Action<IBedPatientAlloc> onInitDlg = delegate (IBedPatientAlloc BedPatientAllocVM)
            {
                //goi toi childwindow dat giuong
                BedPatientAllocVM.PatientInfo = PatientInfo;
                BedPatientAllocVM.curPatientRegistration = curPatientRegistration;
                this.ActivateItem(BedPatientAllocVM);
                //Globals.ShowDialog(BedPatientAllocVM as Conductor<object>);
            };
            GlobalsNAV.ShowDialog<IBedPatientAlloc>(onInitDlg);

            //Globals.LoadDynamicModule<IBedPatientAlloc>("eHCMS.Configuration.xap");
        }

        //▼====== #001: Khi đã lưu thì sẽ bắn sự kiện để load lại thông tin đã lưu, chỗ này là chỗ nhận thông tin để đi load lại.
        #region IHandle<CommonClosedPhysicalForSummaryEvent> Members
        public void Handle(CommonClosedPhysicalForSummaryEvent message)
        {
            long tmpPtRegistrationID = 0;
            long tmpV_RegistrationType = 0;
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null)
            {
                tmpPtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                tmpV_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
            }
            GetPhyExam_ByPtRegID(tmpPtRegistrationID, tmpV_RegistrationType);
        }
        private void GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPhyExam_ByPtRegID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PtPhyExamItem = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                            Globals.curPhysicalExamination = PtPhyExamItem;
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
            });
            t.Start();
        }
        #endregion
        //▲====== #001
        public void Handle(RiskFactorSaveCompleteEvent message)
        {
            RiskFactorGet((long?)PatientInfo.PatientID);
        }

        public void InitConsultationInfo(Patient patientInfo)
        {
            mThongTinChung_DiUngCanhBao_ChinhSua = true && Globals.isConsultationStateEdit;
            mThongTinChung_TinhTrangTheChat_ChinhSua = true && Globals.isConsultationStateEdit;
            authorization();
            NotifyOfPropertyChange(() => mThongTinChung_DiUngCanhBao_ChinhSua);
            NotifyOfPropertyChange(() => mThongTinChung_TinhTrangTheChat_ChinhSua);

            if (patientInfo != null)
            {
                InitData(patientInfo);
            }
        }

        public void Handle(ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            if (message != null)
            {
                InitConsultationInfo(message.Pt);
            }

            //KMx: Cắt code, chuyển qua hàm InitConsultationInfo, để cho event LoadGeneralInfoPageEvent sử dụng chung (10/10/2014 17:13).
            //mThongTinChung_DiUngCanhBao_ChinhSua = true && Globals.isConsultationStateEdit;
            //mThongTinChung_TinhTrangTheChat_ChinhSua = true && Globals.isConsultationStateEdit;
            //authorization();
            //NotifyOfPropertyChange(() => mThongTinChung_DiUngCanhBao_ChinhSua);
            //NotifyOfPropertyChange(() => mThongTinChung_TinhTrangTheChat_ChinhSua);
                     
            //if(message!=null)
            //{
            //    InitData(message.Pt);
            //}
        }

        public void Handle(LoadGeneralInfoPageEvent message)
        {
            if (message != null)
            {
                InitConsultationInfo(message.Patient);
            }
        }

        public void hplDelete_Click(object selectedItem)
        {
            MDAllergy p = (selectedItem as MDAllergy);

            if (p != null && p.AItemID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.AllergiesItems.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MDAllergies_IsDeleted(p);
                }
            }
        }


        private void MDAllergies_IsDeleted(MDAllergy Obj)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDAllergies_IsDeleted(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result="";
                            contract.EndMDAllergies_IsDeleted(out Result, asyncResult);

                            if(Result=="0")
                            {
                                MessageBox.Show(eHCMSResources.K0473_G1_XoaDiUngFail);
                            }
                            else
                            {
                                MDAllergies_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
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
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }


        public void Handle(Re_ReadAllergiesEvent message)
        {
            if(this.GetView()!=null)
            {
                MDAllergies_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
            }
        }

        public void Handle(Re_ReadWarningEvent message)
        {
            if (this.GetView() != null)
            {
                MDWarnings_ByPatientID(PatientInfo.PatientID, 1);/*1 la Active*/
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
        //▼===== #002
        public void Handle(ShowSummaryInPT message)
        {
            if (message != null)
            {
                InitConsultationInfo(message.Patient);
            }
        }
        public void Handle(SetPhysicalForSummary_InPtEvent message)
        {
            if(message != null)
            {
                PtPhyExamItem = Globals.curPhysicalExamination;
            }
        }
        //▲===== #002

        //▼===== #003
        private ObservableCollection<InfectionControl> _allIC_MRBacteria;
        public ObservableCollection<InfectionControl> allIC_MRBacteria
        {
            get
            {
                return _allIC_MRBacteria;
            }
            set
            {
                if (_allIC_MRBacteria != value)
                {
                    _allIC_MRBacteria = value;
                    NotifyOfPropertyChange(() => allIC_MRBacteria);
                }
            }
        }

        private InfectionControl _curIC_MRBacteria;
        public InfectionControl curIC_MRBacteria
        {
            get
            {
                return _curIC_MRBacteria;
            }
            set
            {
                if (_curIC_MRBacteria != value)
                {
                    _curIC_MRBacteria = value;
                    NotifyOfPropertyChange(() => curIC_MRBacteria);
                }
            }
        }
        
        private ObservableCollection<InfectionControl> _allIC_HosInfection;
        public ObservableCollection<InfectionControl> allIC_HosInfection
        {
            get
            {
                return _allIC_HosInfection;
            }
            set
            {
                if (_allIC_HosInfection != value)
                {
                    _allIC_HosInfection = value;
                    NotifyOfPropertyChange(() => allIC_HosInfection);
                }
            }
        }
        
        private InfectionControl _curIC_HosInfection;
        public InfectionControl curIC_HosInfection
        {
            get
            {
                return _curIC_HosInfection;
            }
            set
            {
                if (_curIC_HosInfection != value)
                {
                    _curIC_HosInfection = value;
                    NotifyOfPropertyChange(() => curIC_HosInfection);
                }
            }
        }

        private void GetIC_MRBacteria(long? PatientID)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetInfectionControlByPatientID(PatientID, 0, Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientAdmDisDetailID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            allIC_MRBacteria = contract.EndGetInfectionControlByPatientID(asyncResult).ToObservableCollection();
                            curIC_MRBacteria = allIC_MRBacteria != null && allIC_MRBacteria.Count > 0 ?
                                allIC_MRBacteria[0] : new InfectionControl();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void GetIC_HosInfection(long? PatientID)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetInfectionControlByPatientID(PatientID, 1, Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientAdmDisDetailID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            allIC_HosInfection = contract.EndGetInfectionControlByPatientID(asyncResult).ToObservableCollection();
                            curIC_HosInfection = allIC_HosInfection != null && allIC_HosInfection.Count > 0 ?
                                allIC_HosInfection[0] : new InfectionControl();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                        finally
                        {
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });
            t.Start();
        }

        public void Handle(SaveInfectionControlCompleteEvent message)
        {
            GetIC_MRBacteria((long?)PatientInfo.PatientID);
            GetIC_HosInfection((long?)PatientInfo.PatientID);
        }

        public void hpkNewInfectionControl(object sender, RoutedEventArgs e)
        {
            if (PatientID < 0)
            {
                MessageBox.Show(eHCMSResources.A0378_G1_Msg_InfoChuaChonBN);
                return;
            }
            Action<IInfectionControl> onInitDlg = delegate (IInfectionControl vm)
            {
                vm.PatientID = PatientID;
                //▼==== #004
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0)
                {
                    vm.InPatientAdmDisDetailID = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientAdmDisDetailID;
                }
                //▲==== #004
                vm.curIC_MRBacteria = ObjectCopier.DeepCopy(curIC_MRBacteria);
                vm.curIC_HosInfection = ObjectCopier.DeepCopy(curIC_HosInfection);
            };
            GlobalsNAV.ShowDialog<IInfectionControl>(onInitDlg);
        }
        //▲===== #003

        //▼===== #005
        public void hplEditMRBacteria_Click(RoutedEventArgs e)
        {
            hpkEdiInfectionControl(e, 0);
        }

        public void hplEditHosInfection_Click(RoutedEventArgs e)
        {
            hpkEdiInfectionControl(e, 1);
        }

        public void hpkEdiInfectionControl(RoutedEventArgs e, int BacteriaType)
        {
            if (PatientID < 0)
            {
                MessageBox.Show(eHCMSResources.A0378_G1_Msg_InfoChuaChonBN);
                return;
            }
            Action<IInfectionControl> onInitDlg = delegate (IInfectionControl vm)
            {
                if (BacteriaType == 0)
                {
                    vm.isEditMRBacteria = true;
                }
                else
                {
                    vm.isEditHosInfection = true;
                }

                vm.PatientID = PatientID;
                //▼==== #004
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                    && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientAdmDisDetailID > 0)
                {
                    vm.InPatientAdmDisDetailID = Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientAdmDisDetailID;
                }
                //▲==== #004
                vm.curIC_MRBacteria = ObjectCopier.DeepCopy(curIC_MRBacteria);
                vm.curIC_HosInfection = ObjectCopier.DeepCopy(curIC_HosInfection);
            };
            GlobalsNAV.ShowDialog<IInfectionControl>(onInitDlg);
        }
        //▲===== #005

    }
}
