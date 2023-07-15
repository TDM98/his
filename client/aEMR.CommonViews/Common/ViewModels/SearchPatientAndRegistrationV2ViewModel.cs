using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.Controls;
using aEMR.Common.BaseModel;
using Castle.Core.Logging;

/*
* 20170220 #001 CMN: Disable loading registration on load view for performance
* 20170310 #002 CMN: Add Variable for visible admission checkbox
* 20170310 #003 CMN: Add Variable for visible admission checkbox
* 20171026 #004 CMN: Added Find ConsultingDiagnosys for Surgery Registrations
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISearchPatientAndRegistrationV2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchPatientAndRegistrationV2ViewModel: ViewModelBase, ISearchPatientAndRegistrationV2
        //, IHandle<PatientFindByChange>
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SearchPatientAndRegistrationV2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            authorization();

            eventArg.Subscribe(this);

            _searchCriteria = new PatientSearchCriteria();
            _searchRegCriteria = new SeachPtRegistrationCriteria();
            _logger = container.Resolve<ILogger>();
            _IsSearchGoToKhamBenh = false;

            if (Globals.PatientFindBy_ForConsultation != null)
            {
                PatientFindBy = Globals.PatientFindBy_ForConsultation.Value;
            }
            else
            {
                PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            }

            if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
            {
                bIsNgoaiTruChecked = true;
            }
            else
            {
                bIsNgoaiTruChecked = false;
            }
        }
        //==== #001
        private string _SearchQRCode;
        public string SearchQRCode
        {
            get { return _SearchQRCode; }
            set
            {
                _SearchQRCode = value;
                if (SearchCriteria != null && _SearchQRCode != null)
                {
                    try
                    {
                        string[] QRArray = _SearchQRCode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (QRArray.Length >= 15)
                        {
                            SearchCriteria.QRCode = new HIQRCode
                            {
                                HICardNo = QRArray[0],
                                FullName = QRArray[1],
                                DOB = DateTime.ParseExact(QRArray[2], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                Gender = new eHCMS.Services.Core.Gender(QRArray[3], QRArray[3] == "M" ? eHCMSResources.K0785_G1_1Nam : eHCMSResources.K0816_G1_2Nu),
                                Address = QRArray[4],
                                RegistrationCode = QRArray[5].Replace(" ", "").Replace("-", ""),
                                ValidDateFrom = DateTime.ParseExact(QRArray[6], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                ValidDateTo = DateTime.ParseExact(QRArray[7], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                ProvinceHICode = QRArray[11]
                            };
                        }
                        else
                            SearchCriteria.QRCode = null;
                    }
                    catch (Exception ex)
                    {
                        SearchCriteria = new PatientSearchCriteria();
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (SearchCriteria != null)
                    SearchCriteria.QRCode = null;
                NotifyOfPropertyChange(() => SearchQRCode);
            }
        }
        //==== #001
        private bool? _bSearchAdmittedInPtRegOnly = false;
        public bool? SearchAdmittedInPtRegOnly
        {
            get { return _bSearchAdmittedInPtRegOnly; }
            set
            {
                _bSearchAdmittedInPtRegOnly = value;
            }
        }

        private long _SearchByVregForPtOfType;
        public long SearchByVregForPtOfType 
        {
            get { return _SearchByVregForPtOfType; }
            set
            {
                _SearchByVregForPtOfType = value;
            }
        }

        //KMx: Cho phép tìm kiếm đăng ký của tất cả các khoa để Nhập thêm khoa (05/12/2014 11:29).
        private bool _CanSearhRegAllDept;
        public bool CanSearhRegAllDept
        {
            get { return _CanSearhRegAllDept; }
            set
            {
                _CanSearhRegAllDept = value;
                NotifyOfPropertyChange(() => CanSearhRegAllDept);
            }
        }

        private bool _bIsNgoaiTruChecked;
        public bool bIsNgoaiTruChecked
        {
            get { return _bIsNgoaiTruChecked; }
            set
            {
                _bIsNgoaiTruChecked = value;
                _bIsNoiTruChecked = !_bIsNgoaiTruChecked;
                NotifyOfPropertyChange(() => bIsNgoaiTruChecked);
            }
        }

        private bool _bIsNoiTruChecked;
        public bool bIsNoiTruChecked
        {
            get { return _bIsNoiTruChecked; }
            set
            {
                _bIsNoiTruChecked = value;
                _bIsNgoaiTruChecked = !bIsNoiTruChecked;
                NotifyOfPropertyChange(() => bIsNoiTruChecked);
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
                
                if (PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
                {
                    bIsNgoaiTruChecked = false;
                    bIsNoiTruChecked = true;
                }
                if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                {
                    bIsNgoaiTruChecked = true;
                    bIsNoiTruChecked = false;
                }

                NotifyOfPropertyChange(() => PatientFindBy);
            }                
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            authorization();
            ToggleVisibility(view);
            ChangeDefaultButton(view);
        }

        private bool _closeRegistrationFormWhenCompleteSelection = true;
        public bool CloseRegistrationFormWhenCompleteSelection
        {
            get { return _closeRegistrationFormWhenCompleteSelection; }
            set { _closeRegistrationFormWhenCompleteSelection = value; }
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

        private LeftModuleActive _leftModule;
        public LeftModuleActive LeftModule
        {
            get { return _leftModule; }
            set
            {
                _leftModule = value;
                NotifyOfPropertyChange(() => LeftModule);
            }
        }

        //private bool _isSearchingRegistration;
        //public bool IsSearchingRegistration
        //{
        //    get { return _isSearchingRegistration; }
        //    set
        //    {
        //        _isSearchingRegistration = value;
        //        NotifyOfPropertyChange(() => IsSearchingRegistration);
        //    }
        //}

        //private bool _isLoading;
        //public bool IsLoading
        //{
        //    get { return _isLoading; }
        //    set
        //    {
        //        _isLoading = value;
        //        NotifyOfPropertyChange(() => IsLoading);
        //    }
        //}

        /// <summary>
        /// Tìm kiếm bệnh nhân.
        /// Nếu có đúng 1 bệnh nhân thì trả về kết quả luôn.
        /// Nếu không có, hoặc có nhiều hơn 1 bệnh nhân thì mở popup để tìm kiếm thêm.
        /// </summary>
        public void SearchPatientCmd()
        {
            _searchCriteria.BirthDateBegin = null;
            _searchCriteria.BirthDateEnd = null;
            _searchCriteria.BirthDateEnabled = false;
            _searchCriteria.Gender = null;
            _searchCriteria.GenderEnabled = false;

            SearchPatients(0, 10, true);
        }

        public void SearchAppointmentCmd()
        {
            Action<IFindAppointment> onInitDlg = delegate (IFindAppointment vm)
            {
                if (!string.IsNullOrEmpty(SearchCriteria.PatientNameString)
               || !string.IsNullOrEmpty(SearchCriteria.InsuranceCard)
               || !string.IsNullOrEmpty(SearchCriteria.FullName)
               || !string.IsNullOrEmpty(SearchCriteria.PatientCode)
            )
                {
                    vm.SearchCriteria.PatientNameString = SearchCriteria.PatientNameString;
                    vm.SearchCriteria.InsuranceCard = SearchCriteria.InsuranceCard;
                    vm.SearchCriteria.FullName = SearchCriteria.FullName;
                    vm.SearchCriteria.PatientCode = SearchCriteria.PatientCode;
                    vm.SearchCmd();
                }
            };
            GlobalsNAV.ShowDialog<IFindAppointment>(onInitDlg);
        }
        /// <summary>
        /// Tìm kiếm đăng ký.
        /// Nếu có đúng 1 đăng ký thì trả về kết quả luôn.
        /// Nếu không có, hoặc có nhiều hơn 1 đăng ký thì mở popup để tìm kiếm thêm.
        /// </summary>
        public void SearchRegistrationCmd()
        {
            //GetCurrentDate();
            // TxD 02/08/2014 Use Global's Server Date instead so replace CoRoutine with it's required code block directly
            //Coroutine.BeginExecute(GetCurrentDate_New());
            
            curDate = Globals.GetCurServerDateTime().Date;
            
            _searchRegCriteria = new SeachPtRegistrationCriteria();

            _searchRegCriteria.SearchByVregForPtOfType = SearchByVregForPtOfType;

            _searchRegCriteria.FullName = _searchCriteria.FullName;
            _searchRegCriteria.HICard = _searchCriteria.InsuranceCard;
            _searchRegCriteria.PatientCode = _searchCriteria.PatientCode;
            _searchRegCriteria.PatientNameString = _searchCriteria.PatientNameString;
            _searchRegCriteria.PtRegistrationCode = _searchCriteria.PtRegistrationCode;

            _searchRegCriteria.PatientFindBy = PatientFindBy;

            if (IsSearchGoToKhamBenh)
            {
                _searchRegCriteria.KhamBenh = true;
            }
            _searchRegCriteria.FromDate = curDate.AddDays(-30);
            _searchRegCriteria.ToDate = curDate;
            _searchRegCriteria.IsAdmission = SearchAdmittedInPtRegOnly;

            if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
            {
                _searchRegCriteria.IsAdmission = null;
                _searchRegCriteria.FromDate = curDate;
                _searchRegCriteria.ToDate = curDate;
                if (_searchRegCriteria.KhamBenh && CheckTextBoxEmpty(_searchRegCriteria))
                {
                    _searchRegCriteria.DeptID = Globals.ObjRefDepartment.DeptID;
                    _searchRegCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;
                }
                else
                {
                    _searchRegCriteria.IsHoanTat = null;
                }
            }

            if (Globals.HomeModuleActive == HomeModuleActive.KHAMBENH && PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
            {
                SearchRegistrationDetails(0, 10, true);
            }
            else
            {
                if (PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
                {
                    SearchRegistrationsInPt(0, 10, true);
                }
                else
                {
                    SearchRegistrations(0, 10, true);
                }
            }
        }

        public void CreateNewPatientCmd()
        {
            Globals.EventAggregator.Publish(new CreateNewPatientEvent() { FullName = string.Empty });
        }

        private void SearchPatients(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1097_G1_DangTimBN));
            var t = new Thread(() =>
            {
            AxErrorEventArgs error = null;
            try
            {
                //IsLoading = true;
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var client = serviceFactory.ServiceInstance;
                    client.BeginSearchPatients(_searchCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                    {
                        int totalCount = 0;
                        IList<Patient> allItems = null;
                        bool bOK = false;
                        try
                        {
                            allItems = client.EndSearchPatients(out totalCount, asyncResult);
                            bOK = true;
                        }
                        catch (FaultException<AxException> fault)
                        {
                            error = new AxErrorEventArgs(fault);
                            _logger.Info(error.ToString());
                        }
                        catch (Exception ex)
                        {
                            error = new AxErrorEventArgs(ex);
                            _logger.Info(error.ToString());
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                        if (bOK)
                        {
                            if (allItems == null || allItems.Count == 0)
                            {
                                Globals.EventAggregator.Publish(new ResultNotFound<Patient>() { Message = eHCMSResources.Z0478_G1_KhongTimThayBenhNhan, SearchCriteria = _searchCriteria });
                            }
                            else if (allItems.Count == 1)
                            {
                                Globals.EventAggregator.Publish(new ResultFound<Patient>() { Result = allItems[0], SearchCriteria = _searchCriteria });
                            }
                            else
                            {
                                Action<IFindPatient> onInitDlg = delegate (IFindPatient vm)
                                {
                                    vm.SearchCriteria = _searchCriteria;
                                    vm.CopyExistingPatientList(allItems, _searchCriteria, totalCount);
                                };
                                GlobalsNAV.ShowDialog<IFindPatient>(onInitDlg);
                            }
                        }
                        SearchCriteria = new PatientSearchCriteria();
                    }), null)
                        ;
                }
            }
            catch (Exception ex)
            {
                error = new AxErrorEventArgs(ex);
                _logger.Info(error.ToString());
                this.HideBusyIndicator();
            }
            if (error != null)
            {
                //Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                Globals.ShowMessage(error.ToString(), eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        //KMx: Hàm này được copy và chỉnh sửa từ SearchRegistrations(). Lý do: Tách tìm đăng ký ngoại trú và nội trú ra riêng biệt (26/08/2014 17:54).
        private void SearchRegistrationsInPt(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator();

            // TxD 07/01/2015: Replaced Globals.IsAdmission with local 
            //_searchRegCriteria.IsAdmission = Globals.IsAdmission;
            _searchRegCriteria.IsAdmission = SearchAdmittedInPtRegOnly;

            _searchRegCriteria.IsDischarge = false;

            if (Globals.isAccountCheck && !CanSearhRegAllDept)
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

            //==== #001
            if (string.IsNullOrEmpty(_searchRegCriteria.PatientNameString))
            {
                Action<IFindRegistrationInPt> onInitDlg = delegate (IFindRegistrationInPt vm)
                {
                    vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                    //==== #003
                    vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                    //==== #003
                    vm.SearchCriteria = SearchRegCriteria;
                    vm.IsPopup = true;
                    vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                    vm.LeftModule = LeftModule;
                    vm.CanSearhRegAllDept = CanSearhRegAllDept;
                    vm.LoadRefDeparments();
                    if (IsSearchGoToKhamBenh)
                    {
                        vm.CloseAfterSelection = true;
                    }
                    else
                    {
                        vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                    }
                    vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                };
                this.HideBusyIndicator();
                GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);
                return;
            }
            //==== #001
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
                                    MessageBoxResult result = MessageBox.Show(eHCMSResources.A0734_G1_Msg_ConfTiepTucTimDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                                    if (result == MessageBoxResult.OK)
                                    {
                                        Action<IFindRegistrationInPt> onInitDlg = delegate (IFindRegistrationInPt vm)
                                        {
                                            // TxD 07/01/2015: Just added new property SearchAdmittedInPtRegOnly to IFindRegistrationInPt
                                            vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                                            //==== #003
                                            vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                                            //==== #003
                                            vm.SearchCriteria = SearchRegCriteria;
                                            vm.IsPopup = true;
                                            vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                            vm.LeftModule = LeftModule;

                                            vm.CanSearhRegAllDept = CanSearhRegAllDept;
                                            vm.LoadRefDeparments();
                                            if (IsSearchGoToKhamBenh)
                                            {
                                                //vm.SearchCriteria.FromDate = Globals.ServerDate.Value;
                                                //vm.SearchCriteria.ToDate = Globals.ServerDate.Value;
                                                vm.CloseAfterSelection = true;
                                            }
                                            else
                                            {
                                                vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                            }
                                            vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                        };
                                        this.HideBusyIndicator();
                                        GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);
                                    }
                                }
                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                {
                                    var home = Globals.GetViewModel<IHome>();

                                    var activeItem = home.ActiveContent;

                                    IRegistrationModule ModuleRegis = activeItem as IRegistrationModule;

                                    IConsultationModule ModuleConsult = activeItem as IConsultationModule;

                                    IStoreDeptHome ModuleStoreDept = activeItem as IStoreDeptHome;

                                    ITransactionModule ModuleTransaction = activeItem as ITransactionModule;

                                    IInPatientInstruction InPatientInstructionView = activeItem as IInPatientInstruction;

                                    if (ModuleStoreDept != null || ModuleTransaction != null)
                                    {
                                        //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                        Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                    }
                                    if (ModuleConsult != null)
                                    {
                                        IInPatientInstruction InPtInstructionVM = ModuleConsult.MainContent as IInPatientInstruction;
                                        IPaperReferalFull TransferFormView = ModuleConsult.MainContent as IPaperReferalFull;
                                        IConsultingDiagnosys ConsultingDiagnosysView = ModuleConsult.MainContent as IConsultingDiagnosys;
                                        if (ConsultingDiagnosysView != null)
                                        {
                                            Globals.EventAggregator.Publish(new RegistrationSelectedToHoiChan() { PtRegistration = allItems[0]});
                                        }
                                        if (TransferFormView != null)
                                        {
                                            Globals.EventAggregator.Publish(new RegistrationSelectedToTransfer() { PtRegistration = allItems[0], PatientFindBy = AllLookupValues.PatientFindBy.NOITRU });
                                        }
                                        else if (InPtInstructionVM != null)
                                        {
                                            Globals.EventAggregator.Publish(new RegistrationSelectedForInPtInstruction_1() { PtRegistration = allItems[0] });
                                        }
                                        else
                                        {
                                            //KMx: Không sử dụng chung event nữa (07/10/2014 15:18).
                                            //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                            //Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                            Globals.EventAggregator.Publish(new RegistrationSelectedForConsultation_K1() { Source = allItems[0] });
                                        }
                                    }
                                    if (InPatientInstructionView != null)
                                    {
                                        Globals.EventAggregator.Publish(new RegistrationSelectedForInPtInstruction_1() { PtRegistration = allItems[0] });
                                    }
                                    if (ModuleRegis != null)/*Di xuat vien*/
                                    {
                                        if (LeftModule == LeftModuleActive.DANGKY_NOITRU_MAU02)
                                        {
                                            Globals.EventAggregator.Publish(new SelectedRegistrationForTemp02_1() { Item = allItems[0] });
                                        }

                                        if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NOITRU)
                                        {
                                            //Bật các DV Ngoai Tru của BN này lên
                                            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0] });
                                            //Bật các DV Ngoai Tru của BN này lên
                                        }
                                        else
                                        {
                                            Globals.EventAggregator.Publish(new ResultFound<Patient> { Result = allItems[0].Patient });
                                        }
                                    }
                                }
                                else//tìm thấy nhiều kết quả
                                {
                                    Action<IFindRegistrationInPt> onInitDlg = delegate (IFindRegistrationInPt vm)
                                    {
                                        vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                                        //==== #003
                                        vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                                        //==== #003
                                        vm.SearchCriteria = SearchRegCriteria;
                                        vm.IsPopup = true;
                                        vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                        vm.LeftModule = LeftModule;
                                        vm.CanSearhRegAllDept = CanSearhRegAllDept;
                                        vm.LoadRefDeparments();


                                        if (IsSearchGoToKhamBenh)
                                        {
                                            //vm.SearchCriteria.FromDate = Globals.ServerDate.Value;
                                            //vm.SearchCriteria.ToDate = Globals.ServerDate.Value;
                                            vm.CloseAfterSelection = true;
                                        }
                                        else
                                        {
                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                        }
                                        vm.CopyExistingPatientList(allItems, _searchRegCriteria, totalCount);
                                    };
                                    GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);
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


        private void SearchRegistrations(int pageIndex, int pageSize, bool bCountTotal)
        {
            //this.DlgShowBusyIndicator();
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            //_searchRegCriteria.IsAdmission = Globals.IsAdmission;
            _searchRegCriteria.IsAdmission = SearchAdmittedInPtRegOnly;

            //IsSearchingRegistration = true;
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        //kiem tra co tim theo gia tri trong text box ko
                        client.BeginSearchRegistrations(_searchRegCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrations(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                                _logger.Info(error.ToString());
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                                _logger.Info(error.ToString());
                            }
                            finally
                            {
                                //this.DlgHideBusyIndicator();
                                this.HideBusyIndicator();
                            }

                            if (bOK)
                            {
                                if (allItems == null || allItems.Count == 0)
                                {
                                    if (IsSearchGoToKhamBenh == false)
                                    {
                                        Globals.EventAggregator.Publish(new ResultNotFound<PatientRegistration>()
                                                                            {
                                                                                Message = string.Format("{0}.", eHCMSResources.Z0083_G1_KhongTimThayDK),
                                                                                SearchCriteria = _searchRegCriteria
                                                                            });
                                        MessageBoxResult result = MessageBox.Show(eHCMSResources.A0734_G1_Msg_ConfTiepTucTimDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                                        if (result == MessageBoxResult.OK)
                                        {
                                            Action<IFindRegistration> onInitDlg = delegate (IFindRegistration vm)
                                            {
                                                vm.IsPopup = true;
                                                vm.SearchCriteria = _searchRegCriteria;
                                                vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                                vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                            };
                                            GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                        }
                                    }
                                    else
                                    {
                                        //Tam thoi Dinh mark lai. sao nay se thay doi ve van de nay
                                        //MessageBoxResult result = MessageBox.Show(eHCMSResources.A0735_G1_Msg_ConfChuyenSangTimBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                                        //if (result == MessageBoxResult.OK)
                                        //{
                                        //    var vm = Globals.GetViewModel<IFindPatient>();
                                        //    vm.SearchCriteria = new PatientSearchCriteria();
                                        //    vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                        //    vm.SearchCriteria.PatientNameString = SearchCriteria.PatientNameString;
                                        //    Globals.ShowDialog(vm as Conductor<object>);
                                        //}

                                        // sua lai cho nay 12/03/2013 thay IFindRegistration bang IFindRegistrationDetail
                                        Action<IFindRegistrationDetail> onInitDlg = delegate (IFindRegistrationDetail vm)
                                        {
                                            vm.IsPopup = true;
                                            vm.SearchCriteria = _searchRegCriteria;
                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                            vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                        };
                                        GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                    }
                                }
                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                {
                                    Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                    if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        //Bật các DV Ngoai Tru của BN này lên
                                        Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0] });
                                        //Bật các DV Ngoai Tru của BN này lên
                                    }
                                    else
                                    {
                                        var home = Globals.GetViewModel<IHome>();

                                        var activeItem = home.ActiveContent;

                                        IRegistrationModule ModuleRegis = activeItem as IRegistrationModule;

                                        IConsultationModule ModuleConsult = activeItem as IConsultationModule;

                                        IStoreDeptHome ModuleStoreDept = activeItem as IStoreDeptHome;

                                        ITransactionModule ModuleTransaction = activeItem as ITransactionModule;

                                        IPaperReferalFull TransferFormVM = activeItem as IPaperReferalFull;
                                        if (TransferFormVM != null)
                                        {
                                            Globals.EventAggregator.Publish(new RegistrationSelectedToTransfer() { PtRegistration = allItems[0], PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });
                                        }

                                        if (ModuleStoreDept != null || ModuleTransaction != null)
                                        {
                                            //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                        }
                                        if (ModuleConsult != null)
                                        {
                                            //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                        }
                                        if (ModuleRegis != null)/*Di xuat vien*/
                                        {
                                            if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NOITRU)
                                            {
                                                //Bật các DV Ngoai Tru của BN này lên
                                                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0] });
                                                //Bật các DV Ngoai Tru của BN này lên
                                            }
                                            else
                                            {
                                                Globals.EventAggregator.Publish(new ResultFound<Patient> { Result = allItems[0].Patient });
                                            }
                                        }
                                    }
                                }
                                else//tìm thấy nhiều kết quả
                                {
                                    if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        SearchRegCriteria.FromDate = curDate;
                                        SearchRegCriteria.ToDate = curDate;
                                    }                                    
                                    Action<IFindRegistration> onInitDlg = delegate (IFindRegistration vm)
                                    {
                                        vm.SearchCriteria = SearchRegCriteria;
                                        vm.IsPopup = true;
                                        vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                        if (IsSearchGoToKhamBenh)
                                        {
                                            //vm.SearchCriteria.FromDate = Globals.ServerDate.Value;
                                            //vm.SearchCriteria.ToDate = Globals.ServerDate.Value;
                                            vm.CloseAfterSelection = true;
                                        }
                                        else
                                        {
                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                        }
                                        vm.CopyExistingPatientList(allItems, _searchRegCriteria, totalCount);
                                    };
                                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    _logger.Info(error.ToString());
                    //this.DlgHideBusyIndicator();
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    //Globals.HideBusy();
                    //Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    Globals.ShowMessage(error.ToString(), eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        private void SearchRegistrationDetails(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationsForDiag(_searchRegCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistrationDetail> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrationsForDiag(out totalCount, asyncResult);
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

                            if (bOK)
                            {
                                if (allItems == null || allItems.Count == 0)
                                {
                                    // sua lai cho nay 12/03/2013 thay IFindRegistration bang IFindRegistrationDetail
                                    Action<IFindRegistrationDetail> onInitDlg = delegate (IFindRegistrationDetail vm)
                                    {
                                        vm.IsPopup = true;
                                        vm.SearchCriteria = _searchRegCriteria;
                                        //vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                        if (IsSearchGoToKhamBenh)
                                        {
                                            vm.CloseAfterSelection = true;
                                        }
                                        else
                                        {
                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                        }
                                        vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                    };
                                    GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                }
                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                {
                                    var home = Globals.GetViewModel<IHome>();
                                    var activeItem = home.ActiveContent;
                                    IConsultationModule ModuleConsult = activeItem as IConsultationModule;
                                    IPaperReferalFull TransferFormVM = ModuleConsult.MainContent as IPaperReferalFull;
                                   
                                    Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                    if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {                                        
                                        //Bật các DV Ngoai Tru của BN này lên
                                        //KMx: Sửa bỏ pop-up nhỏ, nếu như test chạy thành công thì sẽ bỏ dòng dưới.
                                        //Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0].PatientRegistration });
                                        if (TransferFormVM != null)
                                        {
                                            Globals.EventAggregator.Publish(new RegistrationSelectedToTransfer() { PtRegistration = allItems[0].PatientRegistration, PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });
                                        }
                                        else
                                        {
                                            Globals.EventAggregator.Publish(new RegDetailSelectedForConsultation() { Source = allItems[0] });
                                        }
                                        //Bật các DV Ngoai Tru của BN này lên
                                    }
                                    else
                                    {
                                        IRegistrationModule ModuleRegis = activeItem as IRegistrationModule;

                                        ModuleConsult = activeItem as IConsultationModule;

                                        IStoreDeptHome ModuleStoreDept = activeItem as IStoreDeptHome;

                                        ITransactionModule ModuleTransaction = activeItem as ITransactionModule;
                                      
                                        if (ModuleStoreDept != null || ModuleTransaction != null)
                                        {
                                            //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0].PatientRegistration });
                                        }
                                        if (ModuleConsult != null)
                                        {
                                            TransferFormVM = ModuleConsult.MainContent as IPaperReferalFull;
                                            if (TransferFormVM != null)
                                            {
                                                Globals.EventAggregator.Publish(new RegistrationSelectedToTransfer() { PtRegistration = allItems[0].PatientRegistration, PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });
                                            }
                                            else
                                            {
                                                //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                                Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0].PatientRegistration });
                                            }
                                        }
                                        if (ModuleRegis != null)/*Di xuat vien*/
                                        {
                                            if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NOITRU)
                                            {
                                                //Bật các DV Ngoai Tru của BN này lên
                                                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0].PatientRegistration });
                                                //Bật các DV Ngoai Tru của BN này lên
                                            }
                                            else
                                            {
                                                Globals.EventAggregator.Publish(new ResultFound<Patient> { Result = allItems[0].PatientRegistration.Patient });
                                            }
                                        }
                                    }
                                }
                                else//tìm thấy nhiều kết quả
                                {
                                    if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        SearchRegCriteria.FromDate = curDate;
                                        SearchRegCriteria.ToDate = curDate;
                                    }
                                    Action<IFindRegistrationDetail> onInitDlg = delegate (IFindRegistrationDetail vm)
                                    {
                                        vm.SearchCriteria = SearchRegCriteria;
                                        vm.IsPopup = true;
                                        vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                        if (IsSearchGoToKhamBenh)
                                        {
                                            //vm.SearchCriteria.FromDate = Globals.ServerDate.Value;
                                            //vm.SearchCriteria.ToDate = Globals.ServerDate.Value;
                                            vm.CloseAfterSelection = true;
                                        }
                                        else
                                        {
                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                        }
                                        vm.CopyExistingPatientList(allItems, _searchRegCriteria, totalCount);
                                    };
                                    GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                }
                            }
                        }), null)
                            ;
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

        private void SearchRegistrations_KhamBenh(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            //IsSearchingRegistration = true;
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrations(_searchRegCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistration> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrations(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                                ClientLoggerHelper.LogInfo(error.ToString());
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                                ClientLoggerHelper.LogInfo(error.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            if (bOK)
                            {
                                if (allItems == null || allItems.Count == 0)
                                {
                                    MessageBoxResult result = MessageBox.Show(eHCMSResources.A0735_G1_Msg_ConfChuyenSangTimBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                                    if (result == MessageBoxResult.OK)
                                    {
                                        Action<IFindPatient> onInitDlg = delegate (IFindPatient vm)
                                        {
                                            vm.SearchCriteria = new PatientSearchCriteria();
                                            vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                            vm.SearchCriteria.PatientNameString = SearchCriteria.PatientNameString;
                                        };
                                        GlobalsNAV.ShowDialog<IFindPatient>(onInitDlg);
                                    }
                                }
                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                {
                                    Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                    if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        //Bật các DV Ngoai Tru của BN này lên
                                        Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0] });
                                        //Bật các DV Ngoai Tru của BN này lên
                                    }
                                    else
                                    {
                                        var home = Globals.GetViewModel<IHome>();

                                        var activeItem = home.ActiveContent;

                                        IRegistrationModule ModuleRegis = activeItem as IRegistrationModule;

                                        IConsultationModule ModuleConsult = activeItem as IConsultationModule;

                                        IStoreDeptHome ModuleStoreDept = activeItem as IStoreDeptHome;

                                        ITransactionModule ModuleTransaction = activeItem as ITransactionModule;
                                        if (ModuleStoreDept != null || ModuleTransaction != null)
                                        {
                                            //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                        }
                                        if (ModuleConsult != null)
                                        {
                                            //Chon DK Noi Tru Nay Luon De Tien Hanh Di Khanh Benh Luon
                                            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                        }
                                        if (ModuleRegis != null)/*Di xuat vien*/
                                        {
                                            if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NOITRU)
                                            {
                                                //Bật các DV Ngoai Tru của BN này lên
                                                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0] });
                                                //Bật các DV Ngoai Tru của BN này lên
                                            }
                                            else
                                            {
                                                Globals.EventAggregator.Publish(new ResultFound<Patient> { Result = allItems[0].Patient });
                                            }
                                        }
                                    }
                                }
                                else//tìm thấy nhiều kết quả
                                {
                                    Action<IFindRegistration> onInitDlg = delegate (IFindRegistration vm)
                                    {
                                        vm.SearchCriteria = SearchRegCriteria;
                                        vm.IsPopup = true;
                                        vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                        if (IsSearchGoToKhamBenh)
                                        {
                                            vm.CloseAfterSelection = true;
                                        }
                                        else
                                        {
                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                        }
                                        vm.CopyExistingPatientList(allItems, _searchRegCriteria, totalCount);
                                    };
                                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    ClientLoggerHelper.LogInfo(error.ToString());
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    //Globals.HideBusy();
                    //EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    Globals.ShowMessage(error.ToString(), eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        public bool CheckTextBoxEmpty(SeachPtRegistrationCriteria searchCriteria) 
        {
            return !((searchCriteria.PatientNameString != null && searchCriteria.PatientNameString != "")
            || (searchCriteria.HICard != null && searchCriteria.HICard != "")
            || (searchCriteria.FullName != null && searchCriteria.FullName != "")
            || (searchCriteria.PatientCode != null && searchCriteria.PatientCode != ""));
                                              
        }

        private SearchRegistrationButtons _defaultButton = SearchRegistrationButtons.SEARCH_REGISTRATION;
        public void SetDefaultButton(SearchRegistrationButtons defaultButton)
        {
            _defaultButton = defaultButton;

            ChangeDefaultButton(this.GetView());
        }

        public void SetDefaultValue()
        {
            SearchPatientTextBox.Text="";
            SearchPatientTextBox.HICardNumber="";
            SearchPatientTextBox.FullName="";
            SearchPatientTextBox.PatientCode = "";
            SearchPatientTextBox.Focus();
        }

        private void ChangeDefaultButton(object currentView)
        {
            var view = currentView as ISearchPatientAndRegistrationView;
            if (view != null)
            {
                switch (_defaultButton)
                {
                    case SearchRegistrationButtons.SEARCH_PATIENT:
                        view.SetDefaultButton(view.SearchPatientButton);
                        break;

                    case SearchRegistrationButtons.SEARCH_APPOINTMENT:
                        view.SetDefaultButton(view.SearchAppointmentsButton);
                        break;

                    case SearchRegistrationButtons.CREATE_NEWPATIENT:
                        view.SetDefaultButton(view.NewPatientButton);
                        break;

                    default:
                        view.SetDefaultButton(view.SearchRegistrationButton);
                        break;
                }
            }
        }

        private SearchRegButtonsVisibility _buttonsVisibility = SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN;

        public void InitButtonVisibility(SearchRegButtonsVisibility values)
        {
            _buttonsVisibility = values;

            ToggleVisibility(this.GetView());
        }

        private void ToggleVisibility(object currentView)
        {
            var view = currentView as ISearchPatientAndRegistrationView;
            if (view != null)
            {
                view.InitButtonVisibility(_buttonsVisibility);
            }
        }

        AxSearchPatientTextBox SearchPatientTextBox;
        public void SearchPatientTextBox_Loaded(object sender,RoutedEventArgs e) 
        {
            SearchPatientTextBox = (AxSearchPatientTextBox)sender;
        }

        private bool _IsSearchGoToKhamBenh;
        public bool IsSearchGoToKhamBenh
        {
            get { return _IsSearchGoToKhamBenh; }
            set
            {
                _IsSearchGoToKhamBenh = value;
                NotifyOfPropertyChange(() => IsSearchGoToKhamBenh);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mTimBN = true;
        private bool _mThemBN = true;
        private bool _mTimDangKy = true;

        public bool mTimBN
        {
            get
            {
                return _mTimBN;
            }
            set
            {
                if (_mTimBN == value)
                    return;
                _mTimBN = value;
            }
        }
        public bool mThemBN
        {
            get
            {
                return _mThemBN;
            }
            set
            {
                if (_mThemBN == value)
                    return;
                _mThemBN = value;
            }
        }
        public bool mTimDangKy
        {
            get
            {
                return _mTimDangKy;
            }
            set
            {
                if (_mTimDangKy == value)
                    return;
                _mTimDangKy = value;
            }
        }

        #endregion
        #region binding visibilty

        //public HyperlinkButton lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            //lnkDelete = sender as HyperlinkButton;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion


        private bool _PatientFindByVisibility = false;
        public bool PatientFindByVisibility
        {
            get { return _PatientFindByVisibility; }
            set
            {
                if (_PatientFindByVisibility != value)
                {
                    _PatientFindByVisibility = value;
                    NotifyOfPropertyChange(() => PatientFindByVisibility);
                }
            }
        }

        RadioButton rdoNgoaiTru;
        public void rdoNgoaiTru_Loaded(object sender, RoutedEventArgs e)
        {
            rdoNgoaiTru = sender as RadioButton;
        }
        RadioButton rdoNoiTru;
        public void rdoNoiTru_Loaded(object sender, RoutedEventArgs e)
        {
            rdoNoiTru = sender as RadioButton;
        }
        RadioButton rdoCaHai;
        public void rdoCaHai_Loaded(object sender, RoutedEventArgs e)
        {
            rdoCaHai = sender as RadioButton;
        }


        private void SetPatientFindBy()
        {
            if (rdoNgoaiTru.IsChecked.Value)
            {
                PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            if (rdoNoiTru.IsChecked.Value)
            {
                PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            }

            bIsNgoaiTruChecked = rdoNgoaiTru.IsChecked.Value;

            Globals.PatientFindBy_ForConsultation = PatientFindBy;

            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = PatientFindBy });
        }

        public void rdoNgoaiTru_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy();
        }
        public void rdoNoiTru_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy();
        }
        public void rdoCaHai_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy();
        }

        //public void Handle(PatientFindByChange obj)
        //{
        //    if (obj != null)
        //    {
        //        PatientFindBy = obj.patientFindBy;
        //        Globals.PatientFindBy_ForConsultation = PatientFindBy;

        //        if (PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
        //        {
        //            bIsNgoaiTruChecked = false;
        //            bIsNoiTruChecked = true;
        //        }
        //        if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
        //        {
        //            bIsNgoaiTruChecked = true;
        //            bIsNoiTruChecked = false;
        //        }
        //    }
        //}

        // TxD 02/08/2014 : Replaced old method GetCurrentDate with the following new one using Globals Server Date 
        public void GetCurrentDate()
        {
            DateTime todayDate = Globals.GetCurServerDateTime();
            curDate = todayDate;
            _searchRegCriteria = new SeachPtRegistrationCriteria();
            _searchRegCriteria.FullName = _searchCriteria.FullName;
            _searchRegCriteria.HICard = _searchCriteria.InsuranceCard;
            _searchRegCriteria.PatientCode = _searchCriteria.PatientCode;
            _searchRegCriteria.PatientNameString = _searchCriteria.PatientNameString;
            _searchRegCriteria.PtRegistrationCode = _searchCriteria.PtRegistrationCode;
            if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
            {
                SearchRegCriteria.FromDate = todayDate;
                SearchRegCriteria.ToDate = todayDate;
            }
            else/*Noi Tru*/
            {
                SearchRegCriteria.FromDate = todayDate.AddDays(-30);
                SearchRegCriteria.ToDate = todayDate;
                //Noi tru khong gioi han ngay, vi BN Noi Tru nam 1 thang, 2 thang... Anh Tuan da xac nhan cho nay!
            }

            _searchRegCriteria.PatientFindBy = PatientFindBy;
            if (IsSearchGoToKhamBenh)
            {
                _searchRegCriteria.KhamBenh = true;
            }

            SearchRegistrations(0, 10, true);
        }

        private bool _IsGoToTransfer;
        public bool IsGoToTransfer
        {
            get
            {
                return _IsGoToTransfer;
            }
            set
            {
                _IsGoToTransfer = value;
                NotifyOfPropertyChange(() => IsGoToTransfer);
            }
        }
        /*▼====: #004*/
        #region Properties
        private bool _EnableSerchConsultingDiagnosy = false;
        public bool EnableSerchConsultingDiagnosy
        {
            get
            {
                return _EnableSerchConsultingDiagnosy;
            }
            set
            {
                _EnableSerchConsultingDiagnosy = value;
                NotifyOfPropertyChange("EnableSerchConsultingDiagnosy");
            }
        }
        private bool _IsConsultingHistoryView = false;
        public bool IsConsultingHistoryView
        {
            get
            {
                return _IsConsultingHistoryView;
            }
            set
            {
                if (_IsConsultingHistoryView == value) return;
                _IsConsultingHistoryView = value;
                NotifyOfPropertyChange(() => IsConsultingHistoryView);
            }
        }
        #endregion
        #region Events
        public void SerchConsultingDiagnosyCmd()
        {
            Action<IFindConsultingDiagnosy> onInitDlg = delegate (IFindConsultingDiagnosy mView)
            {
                mView.IsConsultingHistoryView = this.IsConsultingHistoryView;
            };
            GlobalsNAV.ShowDialog<IFindConsultingDiagnosy>(onInitDlg);
        }
        #endregion
        /*▲====: #004*/
    }
}

//public void GetCurrentDate()
//{
//    var t = new Thread(() =>
//    {
//        try
//        {
//            using (var serviceFactory = new CommonServiceClient())
//            {
//                var contract = serviceFactory.ServiceInstance;

//                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
//                {
//                    try
//                    {
//                        DateTime date = contract.EndGetDate(asyncResult);
//                        curDate = date;
//                        _searchRegCriteria = new SeachPtRegistrationCriteria();
//                        _searchRegCriteria.FullName = _searchCriteria.FullName;
//                        _searchRegCriteria.HICard = _searchCriteria.InsuranceCard;
//                        _searchRegCriteria.PatientCode = _searchCriteria.PatientCode;
//                        _searchRegCriteria.PatientNameString = _searchCriteria.PatientNameString;
//                        if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
//                        {
//                            SearchRegCriteria.FromDate = date;
//                            SearchRegCriteria.ToDate = date;
//                        }
//                        else/*Noi Tru*/
//                        {
//                            SearchRegCriteria.FromDate = date.AddDays(-30);
//                            SearchRegCriteria.ToDate = date;
//                            //Noi tru khong gioi han ngay, vi BN Noi Tru nam 1 thang, 2 thang... Anh Tuan da xac nhan cho nay!
//                        }

//                        _searchRegCriteria.PatientFindBy = PatientFindBy;
//                        if (IsSearchGoToKhamBenh)
//                        {
//                            _searchRegCriteria.KhamBenh = true;
//                        }

//                        SearchRegistrations(0, 10, true);

//                    }
//                    catch (FaultException<AxException> fault)
//                    {
//                        ClientLoggerHelper.LogInfo(fault.ToString());
//                    }
//                    catch (Exception ex)
//                    {
//                        ClientLoggerHelper.LogInfo(ex.ToString());
//                    }

//                }), null);
//            }
//        }
//        catch (Exception ex)
//        {
//            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//        }
//    });
//    t.Start();
//}


//private IEnumerator<IResult> GetCurrentDate_New()
//{
//    var loadCurrentDate = new LoadCurrentDateTask();
//    yield return loadCurrentDate;

//    if (loadCurrentDate.CurrentDate == DateTime.MinValue)
//    {
//        curDate = Globals.ServerDate.Value.Date;
//        MessageBoxTask _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1257_G1_KgLayDcNgThTuServer), eHCMSResources.G0442_G1_TBao);
//        yield return _msgTask;
//    }
//    else
//    {
//        curDate = loadCurrentDate.CurrentDate.Date;
//    }

//    _searchRegCriteria = new SeachPtRegistrationCriteria();
//    _searchRegCriteria.FullName = _searchCriteria.FullName;
//    _searchRegCriteria.HICard = _searchCriteria.InsuranceCard;
//    _searchRegCriteria.PatientCode = _searchCriteria.PatientCode;
//    _searchRegCriteria.PatientNameString = _searchCriteria.PatientNameString;

//    _searchRegCriteria.PatientFindBy = PatientFindBy;

//    if (IsSearchGoToKhamBenh)
//    {
//        _searchRegCriteria.KhamBenh = true;
//    }
//    _searchRegCriteria.FromDate = curDate.AddDays(-30);
//    _searchRegCriteria.ToDate = curDate;
//    _searchRegCriteria.IsAdmission = Globals.IsAdmission;

//    if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
//    {
//        _searchRegCriteria.IsAdmission = null;
//        _searchRegCriteria.FromDate = curDate;
//        _searchRegCriteria.ToDate = curDate;
//        if (_searchRegCriteria.KhamBenh
//            && CheckTextBoxEmpty(_searchRegCriteria))
//        {
//            _searchRegCriteria.DeptID = Globals.ObjRefDepartment.DeptID;
//            _searchRegCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;
//        }
//        else
//        {
//            _searchRegCriteria.IsHoanTat = null;
//        }
//    }

//    if (Globals.HomeModuleActive == HomeModuleActive.KHAMBENH
//        && PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
//    {
//        SearchRegistrationDetails(0, 10, true);
//    }
//    else
//    {
//        SearchRegistrations(0, 10, true);
//    }
//    yield break;
//}

//private bool? _IsAdmission;
//public bool? IsAdmission
//{
//    get { return _IsAdmission; }
//    set
//    {
//        _IsAdmission = value;
//        //SearchRegCriteria.IsAdmission = IsAdmission;
//        NotifyOfPropertyChange(() => IsAdmission);
//    }
//}
