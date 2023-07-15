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
using System.Linq;
using Ax.ViewContracts.SL;
using aEMR.CommonTasks;
using System.Net;
using System.Threading.Tasks;

/*
 * 20170220 #001 CMN:   Disable loading registration on load view for performance
 * 20170310 #002 CMN:   Add Variable for visible admission checkbox
 * 20170310 #003 CMN:   Add Variable for visible admission checkbox
 * 20171026 #004 CMN:   Added Find ConsultingDiagnosys for Surgery Registrations
 * 20180904 #005 TTM:   Nếu người dùng tìm rỗng thì show popup trống lên cho người dùng nhập tìm chi tiết. Không tự động tìm tất cả nữa.
 * 20181002 #006 TTM:   BM 0000121: Thêm cờ IsAllowSearchingPtByName để hạn chế tìm kiếm bằng tên. (20181002: cấu hình đang được bật là false => hạn chế tìm kiếm bằng tên)
 * 20181105 #007 TTM:   BM 0000097: Thêm ngày giờ mặc định để không phải query từ đầu đến cuối chương trình khi tìm cuộc hẹn của bệnh nhân => CHẬM.
 * 20181121 #008 TTM:   BM 0005299: Cho phép tìm kiếm bệnh nhân bằng tên kèm DOB
 * 20181227 #009 TNHX:  BM 0005471: Add Focus for txtName
 * 20190522 #010 TTM:   BM 0006771: Thêm câu cảnh báo khi người dùng quét trúng thẻ định dạng lỗi.
 * 20190702 #011 TNHX:  BM 0011923: Quét thẻ BH tìm không thấy nhưng không hiện popup
 * 20190816 #012 TBL:   Thêm 1 nút tìm những cuộc hẹn thuộc đối tượng khám sức khỏe, lọc đối tượng KSK theo tên công ty
 * 20191203 #013 TTM:   BM 0019688: [Tìm kiếm] Bổ sung popup tìm kiếm đăng ký cho khám sức khoẻ
 * 20200327 #014 TTM:   BM 0029050: [Tìm kiếm] Fix lỗi không tìm tất cả ở màn hình nhập viện do mặc định sai radiobutton Tất cả, đã xuất viện, chưa xuất viện.
 * 20200619 #015 TTM:   BM 0039267: Thêm chức năng điều trị ngoại trú (Bán thuốc).
 * 20200906 #016 TNHX:  BM: Tự động gọi STT sau khi trả tiền + tìm thông tin BN theo phiếu gọi
 * 20210704 #017 TNHX:  385 Lấy ngày tạo STT của QMS để làm ngày vào viện + Khi thành công chọn được BN thì xóa giờ của phiếu
 * 20220318 #018 QTD:   Thêm cờ tìm cho phép tìm BN chưa nhập viện ở màn hình tạm ứng 
 * 20230530 #019 DatTB:
 * + Thêm service tìm kiếm bệnh nhân bằng QRCode CCCD
 * + Thêm bệnh nhân bằng thông tin QRCode CCCD
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISearchPatientAndRegistration)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchPatientAndRegistrationViewModel : ViewModelBase, ISearchPatientAndRegistration
    //, IHandle<PatientFindByChange>
        , IHandle<CallNextTicketQMSEvent>
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SearchPatientAndRegistrationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
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

            // VuTTM - QMS Service
            IsQMSEnable = GlobalsNAV.IsQMSEnable();
            IsQMSEnable2 = GlobalsNAV.IsQMSEnable2();
            CurOrderNumber = null;
            if (Globals.DoctorAccountBorrowed != null && Globals.DoctorAccountBorrowed.Staff != null)
            {
                UserOfficialFullName = Globals.DoctorAccountBorrowed.Staff.FullName;
            }
        }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
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
                        //20190522 TTM: Chuyển từ RemoveEmptyEntries => None vì đã set cho trường lỗi định dạng từ sai => "", nếu sử dụng RemoveEmptyEntries => Array sẽ bị thiếu 1 trường. (14 thay vì 15).
                        //              => Không đúng điều kiện QRArray.Length >= 15
                        //string[] QRArray = _SearchQRCode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] QRArray = _SearchQRCode.Split(new string[] { "|" }, StringSplitOptions.None);
                        if (QRArray.Length >= 15)
                        {

                            try
                            {
                                SearchCriteria.QRCode = new HIQRCode
                                {
                                    HICardNo = QRArray[0],
                                    FullName = QRArray[1],
                                    DOB = DateTime.ParseExact(QRArray[2], "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    Gender = new eHCMS.Services.Core.Gender(QRArray[3], QRArray[3] == "M" ? "Nam" : "Nữ"),
                                    Address = QRArray[4],
                                    RegistrationCode = QRArray[5].Replace(" ", "").Replace("-", ""),
                                    ValidDateFrom = DateTime.ParseExact(QRArray[6], "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    // TxD 09/10/2018 The BHYT moi khong co gia tri Ngay Den 
                                    //ValidDateTo = DateTime.ParseExact(QRArray[7], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    ProvinceHICode = QRArray[11]
                                };
                            }
                            catch
                            {
                                //20181227 TTM: Trường hợp thẻ chỉ có năm sinh mà không có ngày tháng => dùng định dạng yyyy để Parse
                                SearchCriteria.QRCode = new HIQRCode
                                {
                                    HICardNo = QRArray[0],
                                    FullName = QRArray[1],
                                    DOB = DateTime.ParseExact(QRArray[2], "yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    Gender = new eHCMS.Services.Core.Gender(QRArray[3], QRArray[3] == "M" ? "Nam" : "Nữ"),
                                    Address = QRArray[4],
                                    RegistrationCode = QRArray[5].Replace(" ", "").Replace("-", ""),
                                    ValidDateFrom = DateTime.ParseExact(QRArray[6], "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    ProvinceHICode = QRArray[11]
                                };
                            }
                            if (!string.IsNullOrEmpty(QRArray[7]))
                            {
                                try
                                {
                                    SearchCriteria.QRCode.ValidDateTo = DateTime.ParseExact(QRArray[7], "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    SearchCriteria.QRCode.ValidDateTo = SearchCriteria.QRCode.ValidDateFrom.AddYears(1);
                                }
                            }
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
        //==== #019
        private string _SearchIDCardQRCode;
        public string SearchIDCardQRCode
        {
            get { return _SearchIDCardQRCode; }
            set
            {
                _SearchIDCardQRCode = value;
                if (SearchCriteria != null && _SearchIDCardQRCode != null)
                {
                    try
                    {
                        //20190522 TTM: Chuyển từ RemoveEmptyEntries => None vì đã set cho trường lỗi định dạng từ sai => "", nếu sử dụng RemoveEmptyEntries => Array sẽ bị thiếu 1 trường. (14 thay vì 15).
                        //              => Không đúng điều kiện QRArray.Length >= 15
                        //string[] QRArray = _SearchIDCardQRCode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] QRArray = _SearchIDCardQRCode.Split(new string[] { "|" }, StringSplitOptions.None);
                        if (QRArray.Length == 6 || QRArray.Length == 7)
                        {                            
                            try
                            {
                                if (QRArray.Length == 6)
                                {
                                    DateTime DOBTmp;
                                    try
                                    {
                                        DOBTmp = DateTime.ParseExact(QRArray[2], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    catch
                                    {
                                        DOBTmp = DateTime.ParseExact(QRArray[2], "yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    }

                                    SearchCriteria.IDCardQRCode = new IDCardQRCode
                                    {
                                        IDNumber = QRArray[0],
                                        FullName = QRArray[1],
                                        DOB = DOBTmp,
                                        Gender = new eHCMS.Services.Core.Gender(QRArray[3] == "Nam" ? "M" : "F", QRArray[3]),
                                        Address = QRArray[4],
                                        IDCreatedDate = DateTime.ParseExact(QRArray[5], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture)
                                    };
                                }
                                else if (QRArray.Length == 7)
                                {
                                    DateTime DOBTmp;
                                    try
                                    {
                                        DOBTmp = DateTime.ParseExact(QRArray[3], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    }
                                    catch
                                    {
                                        DOBTmp = DateTime.ParseExact(QRArray[3], "yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    }

                                    SearchCriteria.IDCardQRCode = new IDCardQRCode
                                    {
                                        IDNumber = QRArray[0],
                                        IDNumberOld = QRArray[1],
                                        FullName = QRArray[2],
                                        DOB = DOBTmp,
                                        Gender = new eHCMS.Services.Core.Gender(QRArray[4] == "Nam" ? "M" : "F", QRArray[4]),
                                        Address = QRArray[5],
                                        IDCreatedDate = DateTime.ParseExact(QRArray[6], "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture)
                                    };
                                }
                                else
                                {
                                    SearchCriteria.IDCardQRCode = new IDCardQRCode();
                                }
                            }
                            catch
                            {
                                SearchCriteria.IDCardQRCode = new IDCardQRCode();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SearchCriteria = new PatientSearchCriteria();
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (SearchCriteria != null)
                    SearchCriteria.IDCardQRCode = null;
                NotifyOfPropertyChange(() => SearchIDCardQRCode);
            }
        }
        //==== #019
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

        /// <summary>
        /// VuTTM - Enable control for applying QMS.
        /// </summary>
        private bool _IsQMSEnable2;
        public bool IsQMSEnable2
        {
            get { return _IsQMSEnable2; }
            set
            {
                _IsQMSEnable2 = value;
                NotifyOfPropertyChange(() => IsQMSEnable2);
            }
        }

        /// <summary>
        /// VuTTM - Enable control for applying QMS.
        /// </summary>
        private bool _IsQMSEnable;
        public bool IsQMSEnable
        {
            get { return _IsQMSEnable; }
            set
            {
                _IsQMSEnable = value;
                NotifyOfPropertyChange(() => IsQMSEnable);
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

        private long? _CurOrderNumber ;
        public long? CurOrderNumber
        {
            get { return _CurOrderNumber; }
            set
            {
                _CurOrderNumber = value;
                NotifyOfPropertyChange(() => CurOrderNumber);
            }
        }

        //▼====: #017
        private DateTime? _DateCreatedQMSTicket;
        public DateTime? DateCreatedQMSTicket
        {
            get { return _DateCreatedQMSTicket; }
            set
            {
                _DateCreatedQMSTicket = value;
                NotifyOfPropertyChange(() => DateCreatedQMSTicket);
            }
        }
        //▲====: #017

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
        private void SearchByNameAndDOB(string FullName, int DOBNumIndex)
        {
            _searchCriteria.FullName = FullName;
            _searchCriteria.DOBNumIndex = DOBNumIndex;
            _searchCriteria.PatientCode = null;
            SearchPatients(0, 10, true);
        }

        public void SearchPatientCmd()
        {
            //▼====== #005
            _searchCriteria.BirthDateBegin = null;
            _searchCriteria.BirthDateEnd = null;
            _searchCriteria.BirthDateEnabled = false;
            _searchCriteria.Gender = null;
            _searchCriteria.GenderEnabled = false;
            _searchCriteria.DOBNumIndex = 0;
            if (SearchPatientTextBox != null
                && String.IsNullOrEmpty(SearchPatientTextBox.Text))
            {
                if (GlobalsNAV.IsQMSEnable()
                    && (null == CurOrderNumber || 0 == CurOrderNumber))
                {
                    MessageBox.Show("Vui lòng nhập STT!");
                    return;
                }
                // QMS Service
                //ShowSearchPatientsDialog();
            }
            else
            {
                //▼====== #008
                if (_searchCriteria.PatientCode != null && _searchCriteria.PatientCode.Contains(":"))
                {
                    int DOBNumIndex = 0;
                    string FullName = "";
                    //20181122 TTM: Hàm GetValueForSearchByNameAndDOB dùng để lấy giá trị cho DOBNumIndex để tìm kiếm kèm DOB, nếu DOBNumIndex = 0 sẽ tìm kiếm bằng tên.
                    Globals.GetValueForSearchByNameAndDOB(_searchCriteria.PatientCode, out DOBNumIndex, out FullName);
                    SearchByNameAndDOB(FullName, DOBNumIndex);
                }
                //▲====== #008
                else
                {
                    if (Globals.ServerConfigSection.CommonItems.UseQMSSystem)
                    {
                        if (_searchCriteria != null && !string.IsNullOrEmpty(_searchCriteria.QMSSerial))
                        {
                            Coroutine.BeginExecute(GetPatientBySerialTicketNumber());
                        }
                        else
                        {
                            SearchPatients(0, 10, true);
                        }
                    }
                    else
                    {
                        SearchPatients(0, 10, true);
                    }
                }

            }
            //▲====== #005
        }

        public void SearchPatientCmdNext2()
        {
            SearchPatientCmdNext();
        }

        //2021-06-10 QTD Tìm bệnh nhân tiếp theo theo PatientCode
        public void SearchPatientCmdNext()
        {
            if (!GlobalsNAV.IsQMSEnable2()
                || 0 == Globals.DeptLocation.DeptLocationID)
            {
                IsQMSEnable2 = false;
            }

            if (!GlobalsNAV.IsQMSEnable()
                || 0 == Globals.DeptLocation.DeptLocationID)
            {
                IsQMSEnable = false;
            }

            if (!IsQMSEnable && !IsQMSEnable2)
            {
                MessageBox.Show("Thông tin phòng không hợp lệ. Vui lòng chọn lại phòng!");
                return;
            }

            OrderDTO curOrder = null;
            if (GlobalsNAV.IsQMSEnable() || GlobalsNAV.IsQMSEnable2())
            {
                curOrder = GlobalsNAV.GetNextWaitingOrder(Globals.DeptLocation.DeptLocationID);
            }

            if ((GlobalsNAV.IsQMSEnable() || GlobalsNAV.IsQMSEnable2()) && null == curOrder)
            {
                MessageBox.Show("Hệ thống chưa ghi nhận STT tiếp theo!");
                return;
            }

            if (GlobalsNAV.IsQMSEnable() && null != curOrder)
            {
                String msg = "Bạn đang gọi bệnh nhân\r\n";
                msg += String.Format("Số thứ tự: {0}\r\n", curOrder.orderNumber);
                msg += !String.IsNullOrEmpty(curOrder.patientName)
                    ? String.Format("Tên bệnh nhân: {0}\r\n", curOrder.patientName) : string.Empty;
                msg += !String.IsNullOrEmpty(curOrder.patientCode)
                    ? String.Format("Mã bệnh nhân: {0}\r\n", curOrder.patientCode) : string.Empty;

                MessageBox.Show(msg);
                curOrder.orderStatus = OrderDTO.CALLING_STATUS;
                curOrder.refDeptId = Globals.DeptLocation.DeptID;
                curOrder.refLocationId = Globals.DeptLocation.DeptLocationID;
                curOrder.startedServiceAt = Globals.GetCurServerDateTime()
                    .ToString(OrderDTO.DEFAULT_DATE_TIME_FORMAT);
                //GlobalsNAV.UpdateOrder(curOrder);
                CurOrderNumber = curOrder.orderNumber.Value;
                //▼====: #017
                DateCreatedQMSTicket = DateTime.Parse(curOrder.createdAt);
                //▲====: #017
            }

            if (IsQMSEnable2 && null != curOrder && !String.IsNullOrEmpty(curOrder.patientCode))
            {
                SearchPatientTextBox.Text = curOrder.patientCode;
                SearchRegistrationCmd();
                return;
            }

            if (GlobalsNAV.IsQMSEnable()
                && !String.IsNullOrEmpty(curOrder.patientCode))
            {
                _searchCriteria.PatientCode = curOrder.patientCode;
            }

            _searchCriteria.BirthDateBegin = null;
            _searchCriteria.BirthDateEnd = null;
            _searchCriteria.BirthDateEnabled = false;
            _searchCriteria.Gender = null;
            _searchCriteria.GenderEnabled = false;
            _searchCriteria.DOBNumIndex = 0;

            if (IsQMSEnable && !string.IsNullOrEmpty(_searchCriteria.PatientCode))
            {
                SearchPatients(0, 10, true);
            }
        }

        //▼====== #005
        private void ShowSearchPatientsDialog(IList<Patient> allItems = null, PatientSearchCriteria _searchCriteria = null, int totalCount = 0)
        {
            Action<IFindPatient> onInitDlg = (vm) =>
            {
                if (allItems == null || _searchCriteria == null)
                {
                    vm.Patients.Clear();
                    vm.SearchCriteria = new PatientSearchCriteria();
                }
                else
                {
                    vm.CopyExistingPatientList(allItems, _searchCriteria, totalCount);
                    vm.SearchCriteria = _searchCriteria;
                }
            };
            GlobalsNAV.ShowDialog<IFindPatient>(onInitDlg);
        }
        //▲====== #005
        public void SearchAppointmentCmd()
        {
            Action<IFindAppointment> onInitDlg = delegate (IFindAppointment vm)
            {
                if (!string.IsNullOrEmpty(SearchCriteria.PatientNameString)
                         || !string.IsNullOrEmpty(SearchCriteria.InsuranceCard)
                         || !string.IsNullOrEmpty(SearchCriteria.FullName)
                         || !string.IsNullOrEmpty(SearchCriteria.PatientCode))
                {
                    vm.SearchCriteria.PatientNameString = SearchCriteria.PatientNameString;
                    vm.SearchCriteria.InsuranceCard = SearchCriteria.InsuranceCard;
                    vm.SearchCriteria.FullName = SearchCriteria.FullName;
                    vm.SearchCriteria.PatientCode = SearchCriteria.PatientCode;
                    //▼====== #007
                    vm.SearchCriteria.DateFrom = Globals.GetCurServerDateTime().AddDays(-Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                    vm.SearchCriteria.DateTo = Globals.GetCurServerDateTime().AddDays(Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                    //▲====== #007
                    //▼===== 20200723 TTM: Fix lỗi mở popup hẹn bệnh, tình trạng là đã xác nhận nhưng vẫn load các loại tình trạng khác.
                    vm.SearchCriteria.V_ApptStatus = (long)AllLookupValues.ApptStatus.BOOKED;
                    //▲===== 
                    vm.SearchCmd();
                }
                else
                {
                    //▼====== #007
                    vm.SearchCriteria = new AppointmentSearchCriteria();
                    vm.SearchCriteria.DateFrom = Globals.GetCurServerDateTime().AddDays(-Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                    vm.SearchCriteria.DateTo = Globals.GetCurServerDateTime().AddDays(Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                    //▼===== 20200723 TTM: Fix lỗi mở popup hẹn bệnh, tình trạng là đã xác nhận nhưng vẫn load các loại tình trạng khác.
                    vm.SearchCriteria.V_ApptStatus = (long)AllLookupValues.ApptStatus.BOOKED;
                    //▲===== 
                    //▲====== #007
                }
            };
            GlobalsNAV.ShowDialog<IFindAppointment>(onInitDlg);
        }
        //▼====== #012
        public void SearchAppointmentKSKCmd()
        {
            Action<IFindAppointmentKSK> onInitDlg = delegate (IFindAppointmentKSK vm)
            {
                if (!string.IsNullOrEmpty(SearchCriteria.PatientNameString)
                         || !string.IsNullOrEmpty(SearchCriteria.InsuranceCard)
                         || !string.IsNullOrEmpty(SearchCriteria.FullName)
                         || !string.IsNullOrEmpty(SearchCriteria.PatientCode))
                {
                    vm.SearchCriteria.PatientNameString = SearchCriteria.PatientNameString;
                    vm.SearchCriteria.InsuranceCard = SearchCriteria.InsuranceCard;
                    vm.SearchCriteria.FullName = SearchCriteria.FullName;
                    vm.SearchCriteria.PatientCode = SearchCriteria.PatientCode;
                    vm.SearchCriteria.DateFrom = Globals.GetCurServerDateTime().AddDays(-Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                    vm.SearchCriteria.DateTo = Globals.GetCurServerDateTime().AddDays(Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                    vm.SearchCmd();
                }
                else
                {
                    vm.SearchCriteria = new AppointmentSearchCriteria();
                    vm.SearchCriteria.DateFrom = Globals.GetCurServerDateTime().AddDays(-Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                    vm.SearchCriteria.DateTo = Globals.GetCurServerDateTime().AddDays(Globals.ServerConfigSection.OutRegisElements.DayStartAndEndFindAppointment);
                }
            };
            GlobalsNAV.ShowDialog<IFindAppointmentKSK>(onInitDlg);
        }
        //▲====== #012
        /// <summary>
        /// Tìm kiếm đăng ký.
        /// Nếu có đúng 1 đăng ký thì trả về kết quả luôn.
        /// Nếu không có, hoặc có nhiều hơn 1 đăng ký thì mở popup để tìm kiếm thêm.
        /// </summary>
        public void SearchRegistrationCmd()
        {
            //▼====== #005: 20180906 TTM: Cần xem lại, chỗ này chỉ làm tạm thôi, cần phải fix lại vì không thể làm mỗi chỗ 1 ít mà nên tạo method mới để gọi => sau này chỉnh sửa dễ hơn.
            curDate = Globals.GetCurServerDateTime().Date;
            _searchRegCriteria = new SeachPtRegistrationCriteria();
            _searchRegCriteria.IsSearchByRegistrationDetails = IsSearchByRegistrationDetails;
            _searchRegCriteria.IsSearchOnlyProcedure = IsSearchOnlyProcedure;
            _searchRegCriteria.IsSearchForCashAdvance = IsSearchForCashAdvance;
            _searchRegCriteria.RegStatus = -1;
            if (IsSearchGoToKhamBenh)
            {
                _searchRegCriteria.KhamBenh = true;
            }
            _searchRegCriteria.PatientFindBy = PatientFindBy; //20180907 TTM: Thiếu dẫn đến lỗi không lấy đc bệnh nhân trong màn hình xuất cho bệnh nhân khoa nội trú
            if (SearchPatientTextBox != null && String.IsNullOrEmpty(SearchPatientTextBox.Text) && PatientFindBy != AllLookupValues.PatientFindBy.NOITRU)
            {
                if (PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
                {
                    _searchRegCriteria.FromDate = curDate.AddDays(-30);
                    _searchRegCriteria.ToDate = curDate;
                    _searchRegCriteria.IsAdmission = SearchAdmittedInPtRegOnly;
                    //▼===== #014
                    if (SearchAdmittedInPtRegOnly == false)
                    {
                        _searchRegCriteria.IsDischarge = null;
                    }
                    else
                    {
                        _searchRegCriteria.IsDischarge = false;
                    }
                    //▲===== #014
                    Action<IFindRegistrationInPt> onInitDlg = delegate (IFindRegistrationInPt vm)
                    {
                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                        //▲====== #006
                        vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                        //▼===== #014
                        vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                        //▲===== #014
                        if (SearchAdmittedInPtRegOnly != null)
                        {
                            vm.SetValueForDischargedStatus((bool)SearchAdmittedInPtRegOnly);
                        }
                        vm.SearchCriteria = SearchRegCriteria;
                        vm.IsPopup = true;
                        vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                        vm.LeftModule = LeftModule;
                        vm.CanSearhRegAllDept = CanSearhRegAllDept;
                        vm.LoadRefDeparments();
                        vm.IsProcedureEdit = IsProcedureEdit;
                        if (IsSearchGoToKhamBenh)
                        {
                            vm.CloseAfterSelection = true;
                        }
                        else
                        {
                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                        }
                    };
                    //GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);
                    GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 550));
                }
                else if (IsSearchOutPtRegistrationOnly)
                {
                    Action<IFindRegistration> onInitDlg = (vm) =>
                    {
                        vm.CloseAfterSelection = true;
                        var seachCritical = new SeachPtRegistrationCriteria { FromDate = curDate, ToDate = curDate, PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU, IsAdmission = Globals.IsAdmission };
                        vm.SearchCriteria = seachCritical;
                    };
                    //GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1020, 450));
                }
                else
                {
                    _searchRegCriteria.IsAdmission = null;
                    _searchRegCriteria.FromDate = curDate;
                    _searchRegCriteria.ToDate = curDate;

                    if (_searchRegCriteria.KhamBenh && !CheckTextBoxEmpty(_searchRegCriteria))
                    {
                        _searchRegCriteria.DeptID = Globals.ObjRefDepartment.DeptID;
                        _searchRegCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;
                    }
                    else if (IsSearchGoToConfirmHI)
                    {
                        _searchRegCriteria.IsHoanTat = true;
                    }
                    //20190525 TBL: Mac dinh _searchRegCriteria.IsHoanTat = null de lay len het
                    else
                    {
                        _searchRegCriteria.IsHoanTat = null;
                    }

                    //▼===== #013:  Tạo mới view để thực hiện theo ý anh Tuấn là tìm đăng ký khám sức khoẻ cho lựa chọn Công ty
                    if (IsSearchPhysicalExaminationOnly)
                    {
                        Action<IFindRegistrationDetailForMedicalExamination> onInitDlg = delegate (IFindRegistrationDetailForMedicalExamination vm)
                        {
                            vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                            vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                            vm.IsSearchPhysicalExaminationOnly = IsSearchPhysicalExaminationOnly;
                            vm.IsPopup = true;
                            vm.SearchCriteria = _searchRegCriteria;
                            vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                            vm.CloseAfterSelection = true;
                        };
                        GlobalsNAV.ShowDialog<IFindRegistrationDetailForMedicalExamination>(onInitDlg);
                    }
                    else
                    {
                        Action<IFindRegistrationDetail> onInitDlg = delegate (IFindRegistrationDetail vm)
                        {
                            //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                            //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                            vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                            vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                            //▲====== #006
                            vm.IsSearchPhysicalExaminationOnly = IsSearchPhysicalExaminationOnly;
                            vm.IsPopup = true;
                            vm.SearchCriteria = _searchRegCriteria;
                            vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                            vm.CloseAfterSelection = true;
                        };
                        GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 500));
                    }
                    //▲===== #013
                }
                return;
            }
            //▲====== #005
            //GetCurrentDate();
            // TxD 02/08/2014 Use Global's Server Date instead so replace CoRoutine with it's required code block directly
            //Coroutine.BeginExecute(GetCurrentDate_New());
            else
            {

                _searchRegCriteria.SearchByVregForPtOfType = SearchByVregForPtOfType;
                _searchRegCriteria.FullName = _searchCriteria.FullName;
                _searchRegCriteria.HICard = _searchCriteria.InsuranceCard;
                _searchRegCriteria.PatientCode = _searchCriteria.PatientCode;
                _searchRegCriteria.PatientNameString = _searchCriteria.PatientNameString;
                _searchRegCriteria.PtRegistrationCode = _searchCriteria.PtRegistrationCode;
                _searchRegCriteria.PatientFindBy = PatientFindBy;

                //▼====== #006: Nếu như cấu hình là hạn chế tìm tên BN thì cần chặn trước khi gọi Begin và End để đỡ mất thời gian
                if (!Globals.ServerConfigSection.ConsultationElements.IsAllowSearchingPtByName
                    && !String.IsNullOrEmpty(_searchRegCriteria.FullName)
                    && !IsSearchPtByNameChecked)
                {
                    MessageBox.Show(eHCMSResources.Z2304_G1_KhongTheTimDKBangTen);
                    return;
                }
                //▲====== #006

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
                    //▼===== #015: Nếu là bệnh nhân điều trị ngoại trú thì sẽ tìm mặc định 1 tháng trở lại.
                    if (!IsSearchOutPtTreatmentPre)
                    {
                        _searchRegCriteria.FromDate = curDate;
                    }
                    //▲===== 
                    _searchRegCriteria.ToDate = curDate;
                    if (_searchRegCriteria.KhamBenh && !CheckTextBoxEmpty(_searchRegCriteria))
                    {
                        _searchRegCriteria.DeptID = Globals.ObjRefDepartment.DeptID;
                        _searchRegCriteria.DeptLocationID = Globals.DeptLocation.DeptLocationID;
                        _searchRegCriteria.IsHoanTat = null;
                    }
                    else
                    {
                        _searchRegCriteria.IsHoanTat = null;
                    }
                }
                if (Globals.HomeModuleActive == HomeModuleActive.KHAMBENH && PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                {
                    if (IsSearchPhysicalExaminationOnly)
                    {
                        //▼===== #013: Thay đổi cách tìm đăng ký KSK. Tạo mới view để thực hiện theo ý anh Tuấn là tìm đăng ký cho lựa chọn Công ty
                        //SearchRegistrationDetails(0, 10, true, IsSearchPhysicalExaminationOnly);
                        SearchRegistrationDetailForMedicalExamination(0, 10, true, IsSearchPhysicalExaminationOnly);
                        //▲===== #013
                    }
                    else
                    {
                        SearchRegistrationDetails(0, 10, true);
                    }
                }
                else
                {
                    if (PatientFindBy == AllLookupValues.PatientFindBy.NOITRU)
                    {
                        SearchRegistrationsInPt(0, 10, true);
                    }
                    else
                    {
                        if (IsSearchRegisAndGetPrescript)
                        {
                            SearchRegistrationAndGetPrescription(0, 10, true);
                        }
                        else
                        {
                            SearchRegistrations(0, 10, true);
                        }
                    }
                }
            }
        }
        public void CreateNewPatientCmd()
        {
            if (GlobalsNAV.IsQMSEnable()
                && (null == CurOrderNumber || 0 == CurOrderNumber))
            {
                MessageBox.Show("Vui lòng nhập STT!");
                return;
            }

            long tmpOrderNumber = 0;
            if (null != CurOrderNumber)
            {
                tmpOrderNumber = CurOrderNumber.Value;
            }

            if (GlobalsNAV.IsQMSEnable()
                && !GlobalsNAV.ValidOrder(Globals.DeptLocation.DeptID, tmpOrderNumber, null))
            {
                return;
            }

            CurOrderNumber = null;
            Globals.EventAggregator.Publish(new CreateNewPatientEvent() {
                FullName = string.Empty,
                OrderNumber = tmpOrderNumber,
                ServiceStartedAt = Globals.GetCurServerDateTime()
            });
        }

        private bool HasOrderNumber()
        {
            return null != CurOrderNumber && 0 != CurOrderNumber;
        }

        private void SearchPatients(int pageIndex, int pageSize, bool bCountTotal)
        {
            if (GlobalsNAV.IsQMSEnable()
                && (0 == Globals.DeptLocation.DeptID || 0 == Globals.DeptLocation.DeptLocationID))
            {
                MessageBox.Show("Vui lòng chọn lại khoa/ phòng phù hợp!");
                return;
            }
            OrderDTO order = null;
            if (GlobalsNAV.IsQMSEnable()
                && !String.IsNullOrEmpty(_searchCriteria.PatientCode))
            {
                order = GlobalsNAV.GetOrderByDeptIdAndPatientCode(Globals.DeptLocation.DeptID,
                    _searchCriteria.PatientCode);
                //order = GlobalsNAV.GetOrderByPatientCode(Globals.DeptLocation.DeptLocationID,
                //    _searchCriteria.PatientCode);
            }
            if (GlobalsNAV.IsQMSEnable()
                && (!String.IsNullOrEmpty(_searchCriteria.PatientCode)
                    && null == order && !HasOrderNumber()))
            {
                MessageBox.Show("Vui lòng nhập STT!");
                return;
            }
            if (GlobalsNAV.IsQMSEnable()
                && (null != order && 0 != order.orderNumber))
            {
                CurOrderNumber = order.orderNumber.Value;
            }
            if (GlobalsNAV.IsQMSEnable()
                && !HasOrderNumber())
            {
                MessageBox.Show("Vui lòng nhập STT!");
                return;
            }

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
                                // QMS Service
                                if (GlobalsNAV.IsQMSEnable() && !HasOrderNumber())
                                {
                                    MessageBox.Show("Vui lòng nhập STT!", "Thông báo");
                                    return;
                                }
                                long tmpOrderNumber = 0;
                                if (null != CurOrderNumber)
                                {
                                    tmpOrderNumber = CurOrderNumber.Value;
                                }
                                

                                if (allItems == null || allItems.Count == 0)
                                {
                                    if (GlobalsNAV.IsQMSEnable()
                                        && !GlobalsNAV.ValidOrder(Globals.DeptLocation.DeptID, tmpOrderNumber, null))
                                    {
                                        return;
                                    }

                                    //▼====== #011
                                    //▼====== #010
                                    if (SearchCriteria != null && SearchCriteria.QRCode != null && !string.IsNullOrEmpty(SearchCriteria.QRCode.HICardNo) && (SearchCriteria.QRCode.Address == "" || SearchCriteria.QRCode.FullName == ""))
                                    {
                                        MessageBox.Show(eHCMSResources.Z2683_G1_ThBaoSaiDinhDangThe);
                                    }
                                    //▼==== #019
                                    if (SearchCriteria != null && (SearchCriteria.IDCardQRCode == null || string.IsNullOrEmpty(SearchCriteria.IDCardQRCode.IDNumber) || string.IsNullOrEmpty(SearchCriteria.IDCardQRCode.Address) || string.IsNullOrEmpty(SearchCriteria.IDCardQRCode.FullName)) && Globals.ServerConfigSection.CommonItems.UseIDCardQRCode)
                                    {
                                        MessageBox.Show(eHCMSResources.Z3324_G1_ThBaoSaiDinhDangTheCCCD);
                                    }
                                    //▲==== #019
                                    CurOrderNumber = null;
                                    Globals.EventAggregator.Publish(new ResultNotFound<Patient>() { Message = eHCMSResources.Z0478_G1_KhongTimThayBenhNhan, SearchCriteria = _searchCriteria, OrderNumber = tmpOrderNumber });
                                    //▲====== 
                                    //▲====== #011
                                }
                                else if (allItems.Count == 1)
                                {
                                    // VuTTM - Update the order number and service time
                                    //▼====: #017
                                    UpdateOrderNumberAndStartedTime(allItems[0], order);
                                    CurOrderNumber = null;
                                    Globals.EventAggregator.Publish(new ResultFound<Patient>() { Result = allItems[0], SearchCriteria = _searchCriteria });
                                    DateCreatedQMSTicket = null;
                                    //▲====: #017
                                }
                                else
                                {
                                    // VuTTM - Update the order number and service time
                                    //▼====: #017
                                    UpdateOrderNumberAndStartedTime(allItems, order);
                                    //▲====: #017
                                    CurOrderNumber = null;
                                    //▼====== #005
                                    //Action<IFindPatient> onInitDlg = delegate (IFindPatient vm)
                                    //{
                                    //    vm.SearchCriteria = _searchCriteria;
                                    //    vm.CopyExistingPatientList(allItems, _searchCriteria, totalCount);
                                    //};
                                    //GlobalsNAV.ShowDialog<IFindPatient>(onInitDlg);
                                    ShowSearchPatientsDialog(allItems, _searchCriteria, totalCount);
                                    //▲====== #005
                                }
                            }
                            SearchCriteria = new PatientSearchCriteria();
                        }), null);
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
        //▼====: #017
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patients"></param>
        private void UpdateOrderNumberAndStartedTime(IList<Patient> patients, OrderDTO orderOTD)
        {
            if (!GlobalsNAV.IsQMSEnable()
                || (null == patients || 0 == patients.Count))
            {
                return;
            }

            foreach (Patient patient in patients)
            {
                UpdateOrderNumberAndStartedTime(patient, orderOTD);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patient"></param>
        private void UpdateOrderNumberAndStartedTime(Patient patient, OrderDTO orderOTD)
        {
            if (!GlobalsNAV.IsQMSEnable()
                || null == patient)
            {
                return;
            }

            if (!HasOrderNumber())
            {
                MessageBox.Show("Vui lòng nhập STT!");
                return;
            }

            patient.OrderNumber = null != CurOrderNumber ? CurOrderNumber.Value : 0;
            patient.ServiceStartedAt = Globals.GetCurServerDateTime();
            if (orderOTD != null)
            {
                patient.DateCreatedQMSTicket = DateTime.Parse(orderOTD.createdAt);
            }
            else
            {
                patient.DateCreatedQMSTicket = DateCreatedQMSTicket;
            }
        }
        //▲====: #017

        //KMx: Hàm này được copy và chỉnh sửa từ SearchRegistrations(). Lý do: Tách tìm đăng ký ngoại trú và nội trú ra riêng biệt (26/08/2014 17:54).
        private void SearchRegistrationsInPt(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator();

            // TxD 07/01/2015: Replaced Globals.IsAdmission with local 
            //_searchRegCriteria.IsAdmission = Globals.IsAdmission;
            _searchRegCriteria.IsAdmission = SearchAdmittedInPtRegOnly;
            //▼===== #014
            if (SearchAdmittedInPtRegOnly == false)
            {
                _searchRegCriteria.IsDischarge = null;
            }
            else
            {
                _searchRegCriteria.IsDischarge = false;
            }
            //▲===== #14
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
                    //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                    //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                    vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                    vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                    //▲====== #006
                    vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                    //==== #003
                    vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                    //==== #003
                    if (SearchAdmittedInPtRegOnly != null)
                    {
                        vm.SetValueForDischargedStatus((bool)SearchAdmittedInPtRegOnly);
                    }
                    vm.SearchCriteria = SearchRegCriteria;
                    vm.IsPopup = true;
                    vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                    vm.LeftModule = LeftModule;
                    vm.CanSearhRegAllDept = CanSearhRegAllDept;
                    vm.LoadRefDeparments();
                    vm.IsProcedureEdit = IsProcedureEdit;
                    //▼===== #018
                    vm.IsSearchForCashAdvance = IsSearchForCashAdvance;
                    //▲===== #018
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
                GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 550));
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
                                            //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                            //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                            vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                            vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                            //▲====== #006
                                            // TxD 07/01/2015: Just added new property SearchAdmittedInPtRegOnly to IFindRegistrationInPt
                                            vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                                            //==== #003
                                            vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                                            //==== #003
                                            if (SearchAdmittedInPtRegOnly != null)
                                            {
                                                vm.SetValueForDischargedStatus((bool)SearchAdmittedInPtRegOnly);
                                            }
                                            vm.SearchCriteria = SearchRegCriteria;
                                            vm.IsPopup = true;
                                            vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                            vm.LeftModule = LeftModule;

                                            vm.CanSearhRegAllDept = CanSearhRegAllDept;
                                            vm.LoadRefDeparments();
                                            vm.IsProcedureEdit = IsProcedureEdit;
                                            //▼===== #018
                                            vm.IsSearchForCashAdvance = IsSearchForCashAdvance;
                                            //▲===== #018
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
                                        //GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);
                                        GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 550));
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
                                            Globals.EventAggregator.Publish(new RegistrationSelectedToHoiChan() { PtRegistration = allItems[0] });
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
                                            Globals.EventAggregator.Publish(new RegistrationSelectedForConsultation_K1() { Source = allItems[0], IsSearchByRegistrationDetails = IsSearchByRegistrationDetails });
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
                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        //▲====== #006
                                        vm.SearchAdmittedInPtRegOnly = SearchAdmittedInPtRegOnly;
                                        //==== #003
                                        vm.SearchOnlyNotAdmitted = SearchAdmittedInPtRegOnly != true;
                                        if (SearchAdmittedInPtRegOnly != null)
                                        {
                                            vm.SetValueForDischargedStatus((bool)SearchAdmittedInPtRegOnly);
                                        }
                                        //==== #003
                                        vm.SearchCriteria = SearchRegCriteria;
                                        vm.IsPopup = true;
                                        vm.IsSearchGoToKhamBenh = IsSearchGoToKhamBenh;
                                        vm.LeftModule = LeftModule;
                                        vm.CanSearhRegAllDept = CanSearhRegAllDept;
                                        vm.LoadRefDeparments();
                                        vm.IsProcedureEdit = IsProcedureEdit;

                                        //▼===== #018
                                        vm.IsSearchForCashAdvance = IsSearchForCashAdvance;
                                        //▲===== #018
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
                                    //GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg);
                                    GlobalsNAV.ShowDialog<IFindRegistrationInPt>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 550));
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
                                                //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                                //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                                vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                                vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                                //▲====== #006:
                                                vm.IsPopup = true;
                                                vm.SearchCriteria = _searchRegCriteria;
                                                vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                                vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                            };
                                            //GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                            GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1020, 450));
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
                                            //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                            //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                            vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                            vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                            //▲====== #006
                                            vm.IsPopup = true;
                                            vm.SearchCriteria = _searchRegCriteria;
                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                            vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                        };
                                        //GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                        GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 500));
                                    }
                                }
                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                {
                                    Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                    var home = Globals.GetViewModel<IHome>();
                                    var activeItem = home.ActiveContent;
                                    IRegistrationModule ModuleRegis = activeItem as IRegistrationModule;
                                    IConsultationModule ModuleConsult = activeItem as IConsultationModule;
                                    IStoreDeptHome ModuleStoreDept = activeItem as IStoreDeptHome;
                                    ITransactionModule ModuleTransaction = activeItem as ITransactionModule;
                                    IPaperReferalFull TransferFormVM = activeItem as IPaperReferalFull;
                                    if (ModuleStoreDept != null || ModuleTransaction != null)
                                    {
                                        if (SearchCriteria != null)
                                        {
                                            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                        }
                                    }
                                    if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        //Bật các DV Ngoai Tru của BN này lên
                                        Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0] });
                                        //Bật các DV Ngoai Tru của BN này lên
                                    }
                                    else
                                    {
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
                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        //▲====== #006
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
                                    //GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1020, 450));
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

        private void SearchRegistrationAndGetPrescription(int regPageIndex, int regPageSize, bool bregCountTotal)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));

            _searchRegCriteria.IsAdmission = SearchAdmittedInPtRegOnly;
            long paramPtRegistrationID = SearchRegisAndGetPrescriptPtRegID;
            var presSearchCriteria = new PrescriptionSearchCriteria { PtRegistrationID = paramPtRegistrationID };

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        //kiem tra co tim theo gia tri trong text box ko
                        client.BeginSearchRegistrationAndGetPrescription(_searchRegCriteria, regPageIndex, regPageSize, bregCountTotal, presSearchCriteria, 0, 100, true,
                                        Globals.DispatchCallback((asyncResult) =>
                                        {
                                            int regTotalCount = 0;
                                            int presTotalCount = 0;
                                            IList<PatientRegistration> allItems = null;
                                            IList<Prescription> retPrescriptionsLst = null;

                                            bool bOK = false;
                                            try
                                            {
                                                allItems = client.EndSearchRegistrationAndGetPrescription(out presTotalCount, out retPrescriptionsLst, out regTotalCount, asyncResult);
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
                                                                //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                                                //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                                                vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                                                vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                                                //▲====== #006:
                                                                vm.IsPopup = true;
                                                                vm.SearchCriteria = _searchRegCriteria;
                                                                vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                                                vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                                            };
                                                            //GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                                            GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1020, 450));
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
                                                            //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                                            //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                                            vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                                            vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                                            //▲====== #006
                                                            vm.IsPopup = true;
                                                            vm.SearchCriteria = _searchRegCriteria;
                                                            vm.CloseAfterSelection = CloseRegistrationFormWhenCompleteSelection;
                                                            vm.CopyExistingPatientList(null, vm.SearchCriteria, 0);
                                                        };
                                                        //GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                                        GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 500));
                                                    }
                                                }
                                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                                {
                                                    Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                                    var home = Globals.GetViewModel<IHome>();
                                                    var activeItem = home.ActiveContent;
                                                    IRegistrationModule ModuleRegis = activeItem as IRegistrationModule;
                                                    IConsultationModule ModuleConsult = activeItem as IConsultationModule;
                                                    IStoreDeptHome ModuleStoreDept = activeItem as IStoreDeptHome;
                                                    ITransactionModule ModuleTransaction = activeItem as ITransactionModule;
                                                    IPaperReferalFull TransferFormVM = activeItem as IPaperReferalFull;
                                                    if (ModuleStoreDept != null || ModuleTransaction != null)
                                                    {
                                                        if (SearchCriteria != null)
                                                        {
                                                            Globals.EventAggregator.Publish(new PatientSelectedGoToKhamBenh_InPt<PatientRegistration>() { Item = allItems[0] });
                                                        }
                                                    }
                                                    if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                                    {
                                                        if (IsSearchRegisAndGetPrescript && retPrescriptionsLst != null && retPrescriptionsLst.Count > 0)
                                                        {
                                                            Globals.EventAggregator.Publish(new RegisObjAndPrescriptionLst() { RegisObj = allItems[0], PrescriptLst = retPrescriptionsLst });
                                                            UpdatePrescriptionsDate(allItems[0].PtRegistrationID, Globals.GetCurServerDateTime());
                                                        }
                                                        else
                                                        {
                                                            //Bật các DV Ngoai Tru của BN này lên
                                                            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration>() { Item = allItems[0] });
                                                            //Bật các DV Ngoai Tru của BN này lên
                                                        }
                                                    }
                                                    else
                                                    {
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
                                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                                        //▲====== #006
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
                                                        vm.CopyExistingPatientList(allItems, _searchRegCriteria, regTotalCount);
                                                    };
                                                    //GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1020, 450));
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
                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        //▲====== #006
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
                                    //GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                    GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 500));
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
                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        //▲====== #006
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
                                    //GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                    GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 500));
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

        private void SearchRegistrationDetails(int pageIndex, int pageSize, bool bCountTotal, bool IsSearchPhysicalExaminationOnly)
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
                            try
                            {
                                allItems = client.EndSearchRegistrationsForDiag(out totalCount, asyncResult);
                                if (allItems.Count > 0)
                                {
                                    allItems = allItems.Where(x => x.RefMedicalServiceItem.IsMedicalExamination).ToList();
                                }
                                if (allItems == null || allItems.Count == 0)
                                {
                                    // sua lai cho nay 12/03/2013 thay IFindRegistration bang IFindRegistrationDetail
                                    Action<IFindRegistrationDetail> onInitDlg = delegate (IFindRegistrationDetail vm)
                                    {
                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        //▲====== #006
                                        vm.IsSearchPhysicalExaminationOnly = IsSearchPhysicalExaminationOnly;
                                        vm.IsPopup = true;
                                        vm.SearchCriteria = _searchRegCriteria;
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
                                    GlobalsNAV.ShowDialog(onInitDlg);
                                }
                                else if (allItems.Count == 1)/*Tim thay 1 DK*/
                                {
                                    Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                    if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        Globals.EventAggregator.Publish(new RegDetailSelectedForConsultation() { Source = allItems[0] });
                                    }
                                }
                                else//tìm thấy nhiều kết quả
                                {
                                    if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        SearchRegCriteria.FromDate = curDate;
                                        SearchRegCriteria.ToDate = curDate;
                                    }
                                    void onInitDlg(IFindRegistrationDetail vm)
                                    {
                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        //▲====== #006
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
                                    }
                                    //GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg);
                                    GlobalsNAV.ShowDialog<IFindRegistrationDetail>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1100, 500));
                                }
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
                                        //▼====== #006: Truyền giá trị để Popup khi show lên sẽ lấy giá trị của checkbox tìm bệnh nhân ở ngoài.
                                        //20181024 TNHX: [BM0002186] parent will set isSearchPtByNameChecked & IsAllowSearchingPtByName_Visible
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        //▲====== #006
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
                                    //GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg);
                                    GlobalsNAV.ShowDialog<IFindRegistration>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSizeInput(1020, 450));
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
            || (searchCriteria.PatientCode != null && searchCriteria.PatientCode != "")
            || (searchCriteria.QMSSerial != null && searchCriteria.QMSSerial != ""));

        }

        private SearchRegistrationButtons _defaultButton = SearchRegistrationButtons.SEARCH_REGISTRATION;
        public void SetDefaultButton(SearchRegistrationButtons defaultButton)
        {
            _defaultButton = defaultButton;

            ChangeDefaultButton(this.GetView());
        }

        public void SetDefaultValue()
        {
            SearchPatientTextBox.Text = "";
            SearchPatientTextBox.HICardNumber = "";
            SearchPatientTextBox.FullName = "";
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

        //▼====== #009
        AxSearchPatientTextBox SearchPatientTextBox;
        public void SearchPatientTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SearchPatientTextBox = (AxSearchPatientTextBox)sender;
            SearchPatientTextBox.Focus();
        }
        //▲====== #009

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

        private bool _IsSearchGoToConfirmHI;
        public bool IsSearchGoToConfirmHI
        {
            get { return _IsSearchGoToConfirmHI; }
            set
            {
                _IsSearchGoToConfirmHI = value;
                NotifyOfPropertyChange(() => IsSearchGoToConfirmHI);
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
            _searchRegCriteria.QMSSerial = _searchCriteria.QMSSerial;
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
        private bool _IsSearchOnlyProcedure;
        public bool IsSearchOnlyProcedure
        {
            get { return _IsSearchOnlyProcedure; }
            set
            {
                if (_IsSearchOnlyProcedure != value)
                {
                    _IsSearchOnlyProcedure = value;
                    NotifyOfPropertyChange("IsSearchOnlyProcedure");
                }
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

        //20181018 TNHX: [BM0002186] set default _IsSearchPtByNameChecked = true & IsAllowSearchingPtByName_Visible = false;
        //▼====== #006
        private bool _IsSearchPtByNameChecked = true;
        public bool IsSearchPtByNameChecked
        {
            get { return _IsSearchPtByNameChecked; }
            set
            {
                _IsSearchPtByNameChecked = value;
            }
        }

        private bool _IsAllowSearchingPtByName_Visible = false;
        public bool IsAllowSearchingPtByName_Visible
        {
            get { return _IsAllowSearchingPtByName_Visible; }
            set
            {
                _IsAllowSearchingPtByName_Visible = value;
                NotifyOfPropertyChange(() => IsAllowSearchingPtByName_Visible);
            }
        }
        //▲====== #006
        private bool _IsSearchByRegistrationDetails = false;
        public bool IsSearchByRegistrationDetails
        {
            get { return _IsSearchByRegistrationDetails; }
            set
            {
                _IsSearchByRegistrationDetails = value;
                NotifyOfPropertyChange(() => IsSearchByRegistrationDetails);
            }
        }

        private bool _IsProcedureEdit = false;
        public bool IsProcedureEdit
        {
            get
            {
                return _IsProcedureEdit;
            }
            set
            {
                if (_IsProcedureEdit != value)
                {
                    _IsProcedureEdit = value;
                    NotifyOfPropertyChange(() => IsProcedureEdit);
                }
            }
        }
        private bool _IsSearchOutPtRegistrationOnly = false;
        public bool IsSearchOutPtRegistrationOnly
        {
            get
            {
                return _IsSearchOutPtRegistrationOnly;
            }
            set
            {
                _IsSearchOutPtRegistrationOnly = value;
                NotifyOfPropertyChange(() => IsSearchOutPtRegistrationOnly);
            }
        }

        private bool _IsSearchPhysicalExaminationOnly = false;
        public bool IsSearchPhysicalExaminationOnly
        {
            get
            {
                return _IsSearchPhysicalExaminationOnly;
            }
            set
            {
                _IsSearchPhysicalExaminationOnly = value;
                NotifyOfPropertyChange(() => IsSearchPhysicalExaminationOnly);
            }
        }

        private bool _IsSearchRegisAndGetPrescript = false;
        public bool IsSearchRegisAndGetPrescript
        {
            get
            {
                return _IsSearchRegisAndGetPrescript;
            }
            set
            {
                _IsSearchRegisAndGetPrescript = value;
                NotifyOfPropertyChange(() => IsSearchRegisAndGetPrescript);
            }
        }

        private long _SearchRegisAndGetPrescriptPtRegID = 0;
        public long SearchRegisAndGetPrescriptPtRegID
        {
            get
            {
                return _SearchRegisAndGetPrescriptPtRegID;
            }
            set
            {
                _SearchRegisAndGetPrescriptPtRegID = value;
                NotifyOfPropertyChange(() => SearchRegisAndGetPrescriptPtRegID);
            }
        }
        //▼===== #013: Thay đổi cách tìm đăng ký KSK. Tạo mới view để thực hiện theo ý anh Tuấn là tìm đăng ký cho lựa chọn Công ty
        private void SearchRegistrationDetailForMedicalExamination(int pageIndex, int pageSize, bool bCountTotal, bool IsSearchPhysicalExaminationOnly)
        {

            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationsForMedicalExaminationDiag(_searchRegCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistrationDetail> allItems = null;
                            try
                            {
                                allItems = client.EndSearchRegistrationsForMedicalExaminationDiag(out totalCount, asyncResult);
                                if (allItems.Count > 0)
                                {
                                    allItems = allItems.Where(x => x.RefMedicalServiceItem.IsMedicalExamination).ToList();
                                }
                                if (allItems == null || allItems.Count == 0)
                                {
                                    Action<IFindRegistrationDetailForMedicalExamination> onInitDlg = delegate (IFindRegistrationDetailForMedicalExamination vm)
                                    {
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
                                        vm.IsSearchPhysicalExaminationOnly = IsSearchPhysicalExaminationOnly;
                                        vm.IsPopup = true;
                                        vm.SearchCriteria = _searchRegCriteria;
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
                                    GlobalsNAV.ShowDialog(onInitDlg);
                                }
                                else if (allItems.Count == 1)
                                {
                                    Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                    if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        Globals.EventAggregator.Publish(new RegDetailSelectedForConsultation() { Source = allItems[0] });
                                    }
                                }
                                else
                                {
                                    if (PatientFindBy == AllLookupValues.PatientFindBy.NGOAITRU)
                                    {
                                        SearchRegCriteria.FromDate = curDate;
                                        SearchRegCriteria.ToDate = curDate;
                                    }

                                    //▼===== #013:  Nếu bệnh nhân có nhiều dịch vụ khám sức khoẻ khác phòng nhàu thì dựa theo phòng mà chọn ra dịch vụ đúng phòng để load.
                                    //              Ví dụ: Bệnh nhân có 2 dịch vụ Khám sức khoẻ mắt và 1 dịch vụ khám sức khoẻ nội. Phòng đăng nhập của bác sĩ là khám sức khoẻ nội thì load đăng ký
                                    //              khám sức khoẻ nội lên để khám luôn không cần phải mở popup.
                                    DeptLocation tmpLocation = Globals.DeptLocation;
                                    if (tmpLocation != null)
                                    {
                                        List<PatientRegistrationDetail> tmpList = new List<PatientRegistrationDetail>();
                                        tmpList = allItems.Where(x => x.DeptLocID == tmpLocation.DeptLocationID).ToList();
                                        string StringToFindAll = "*";
                                        if (tmpList != null && tmpList.Count == 1 && string.Compare(SearchCriteria.FullName, StringToFindAll) != 0)
                                        {
                                            Globals.PatientFindBy_ForConsultation = PatientFindBy;
                                            if (Globals.PatientFindBy_ForConsultation != null && Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
                                            {
                                                Globals.EventAggregator.Publish(new RegDetailSelectedForConsultation() { Source = tmpList[0] });
                                            }
                                            return;
                                        }
                                    }
                                    //▲===== #013
                                    void onInitDlg(IFindRegistrationDetailForMedicalExamination vm)
                                    {
                                        vm.IsSearchPtByNameChecked = IsSearchPtByNameChecked;
                                        vm.IsAllowSearchingPtByName_Visible = IsAllowSearchingPtByName_Visible;
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
                                    }
                                    GlobalsNAV.ShowDialog<IFindRegistrationDetailForMedicalExamination>(onInitDlg);
                                }
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
        //▲===== #013
        private bool _bEnableForConsultation = false;
        public bool bEnableForConsultation
        {
            get { return _bEnableForConsultation; }
            set
            {
                _bEnableForConsultation = value;
                NotifyOfPropertyChange(() => bEnableForConsultation);
            }
        }

        #region Ticket
        //▼===== #016
        public void GetNextTicketIssueCmd()
        {
            if (Globals.DeptLocation == null)
            {
                return;
            }
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetNextTicketIssue(Globals.DeptLocation.DeptLocationID, Globals.LoggedUserAccount.StaffID ?? 0, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mNextTicketIssue = mContract.EndGetNextTicketIssue(asyncResult);
                                //Globals.EventAggregator.Publish(new SetTicketNumnerTextForPatientRegistrationView { Item = mNextTicketIssue });
                                if (mNextTicketIssue != null)
                                {
                                    Globals.EventAggregator.Publish(new SetTicketNumnerTextForPatientRegistrationView { Item = mNextTicketIssue });
                                    Globals.EventAggregator.Publish(new SetTicketIssueForPatientRegistrationView { Item = mNextTicketIssue, IsLoadPatientInfo = true });
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z2969_G1_HetSTTDeGoi);
                                }
                                //Globals.EventAggregator.Publish(new SetTicketIssueForPatientRegistrationView { Item = mNextTicketIssue, IsLoadPatientInfo = true });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }
        //▲===== #016

        public void ClearCurrentTicketIssueCmd()
        {
            Globals.EventAggregator.Publish(new ItemSelected<TicketIssue> { Item = new TicketIssue() });
        }
        public void RecalTicketIssueCmd()
        {
            //Action<IRecalQMSTicket> onInitDlg = delegate (IRecalQMSTicket vm) { };
            //GlobalsNAV.ShowDialog<IRecalQMSTicket>(onInitDlg);
            GetTicketByPatientCodeCmd();
        }
        public void GetTicketByPatientCodeCmd()
        {
            if (string.IsNullOrEmpty(SearchCriteria.PatientCode))
            {
                MessageBox.Show(eHCMSResources.Z2975_G1_NhapMaBNDeTimKiem);
                return;
            }
            var t = new Thread(() =>
            {
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetTicketByPatientCode(SearchCriteria.PatientCode, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mNextTicketIssue = mContract.EndGetTicketByPatientCode(asyncResult);
                                if (mNextTicketIssue != null && mNextTicketIssue.TicketNumberSeq > 0)
                                {
                                    MessageBoxResult result = MessageBox.Show(eHCMSResources.Z2971_G1_BanCoMuonTaoMoiDangKy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                                    if (result == MessageBoxResult.OK)
                                    {
                                        mNextTicketIssue.V_TicketStatus = (int)V_TicketStatus_Enum.TKT_BEING_CALLED;
                                        Globals.EventAggregator.Publish(new SetTicketForNewRegistrationAgain { Item = mNextTicketIssue });
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z2973_G1_KhongTheLayDuocThongTinDeDK);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    }
                }
            });
            t.Start();
        }
        public void ListTicketIssueQMSCmd()
        {
            Action<IListTicketIssueQMS> onInitDlg = delegate (IListTicketIssueQMS vm) { };
            GlobalsNAV.ShowDialog<IListTicketIssueQMS>(onInitDlg);
        }

        private string _UserOfficialFullName;
        public string UserOfficialFullName
        {
            get { return _UserOfficialFullName; }
            set
            {
                if (_UserOfficialFullName != value)
                {
                    _UserOfficialFullName = value;
                }
                NotifyOfPropertyChange(() => UserOfficialFullName);
            }
        }
        private bool _IsShowBtnChooseUserOfficial = false;
        public bool IsShowBtnChooseUserOfficial
        {
            get { return _IsShowBtnChooseUserOfficial; }
            set
            {
                if (_IsShowBtnChooseUserOfficial != value)
                {
                    _IsShowBtnChooseUserOfficial = value;
                }
                NotifyOfPropertyChange(() => IsShowBtnChooseUserOfficial);
            }
        }
        public void ChooseUserOfficialCmd()
        {
            void onInitDlg(IDoctorBorrowedAccount vm) {
                vm.IsPopupView = true;
                vm.PatientFindBy = (int)PatientFindBy;
            }
            GlobalsNAV.ShowDialog<IDoctorBorrowedAccount>(onInitDlg);
        }
        private bool _IsShowCallQMS = false;
        public bool IsShowCallQMS
        {
            get { return _IsShowCallQMS; }
            set
            {
                if (_IsShowCallQMS != value)
                {
                    _IsShowCallQMS = value;
                }
                NotifyOfPropertyChange(() => IsShowCallQMS);
            }
        }
        private bool _IsShowCallButton = false;
        public bool IsShowCallButton
        {
            get { return _IsShowCallButton; }
            set
            {
                if (_IsShowCallButton != value)
                {
                    _IsShowCallButton = value;
                }
                NotifyOfPropertyChange(() => IsShowCallButton);
            }
        }
        public bool VisibleForCallButton
        {
            get { return _IsShowCallButton && _IsShowCallQMS && !_IsShowGetTicketButton; }
        }
        private bool _IsShowAppointmentAndKSK = true;
        public bool IsShowAppointmentAndKSK
        {
            get { return _IsShowAppointmentAndKSK; }
            set
            {
                if (_IsShowAppointmentAndKSK != value)
                {
                    _IsShowAppointmentAndKSK = value;
                }
                NotifyOfPropertyChange(() => IsShowAppointmentAndKSK);
            }
        }
        private bool _IsShowGetTicketButton = false;
        public bool IsShowGetTicketButton
        {
            get { return _IsShowGetTicketButton; }
            set
            {
                if (_IsShowGetTicketButton != value)
                {
                    _IsShowGetTicketButton = value;
                }
                if (_IsShowGetTicketButton)
                {
                    EnableSerchConsultingDiagnosy = false;
                    IsShowAppointmentAndKSK = false;
                }
                NotifyOfPropertyChange(() => IsShowGetTicketButton);
            }
        }

        private IEnumerator<IResult> GetPatientBySerialTicketNumber()
        {
            var GetTicketBySerialTask = new GenericCoRoutineTask(GetTicketBySerial);
            yield return GetTicketBySerialTask;
        }
        private void GetTicketBySerial(GenericCoRoutineTask genTask)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new QMSService.QMSServiceClient())
                    {
                        bool bContinue = true;
                        var mContract = mFactory.ServiceInstance;
                        PatientSearchCriteria tmpSearch = new PatientSearchCriteria();
                        mContract.BeginRecalQMSTicket(Globals.DeptLocation.DeptLocationID, _searchCriteria.QMSSerial, Globals.GetCurServerDateTime(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mNextTicketIssue = mContract.EndRecalQMSTicket(asyncResult);
                                if (mNextTicketIssue != null && mNextTicketIssue.TicketNumberSeq > 0)
                                {
                                    Globals.EventAggregator.Publish(new SetTicketIssueForPatientRegistrationView { Item = mNextTicketIssue, IsLoadPatientInfo = true });
                                    SearchCriteria = new PatientSearchCriteria();
                                }
                                else
                                {
                                    MessageBox.Show("Số thứ tự không hợp lệ.");
                                    bContinue = false;
                                }
                            }
                            finally
                            {
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
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        //▼===== #016
        private bool _AutoCalQMSTicket = true;
        public bool AutoCalQMSTicket
        {
            get { return _AutoCalQMSTicket; }
            set
            {
                _AutoCalQMSTicket = value;
                NotifyOfPropertyChange(() => AutoCalQMSTicket);
            }
        }

        CheckBox chkAutoCalQMSTicket;
        public void chkAutoCalQMSTicket_Loaded(object sender, RoutedEventArgs e)
        {
            chkAutoCalQMSTicket = sender as CheckBox;
        }

        public void chkAutoCalQMSTicket_UnCheck(object sender, RoutedEventArgs e)
        {
            if (chkAutoCalQMSTicket == null)
            {
                return;
            }
            chkAutoCalQMSTicket.IsChecked = false;
        }

        public void Handle(CallNextTicketQMSEvent j)
        {
            if (AutoCalQMSTicket)
            {
                GetNextTicketIssueCmd();
            }
        }
        //▲===== #016
        #endregion

        #region OutPtTreatmentPre
        //▼===== #015
        private bool _IsSearchOutPtTreatmentPre;
        public bool IsSearchOutPtTreatmentPre
        {
            get { return _IsSearchOutPtTreatmentPre; }
            set
            {
                if (_IsSearchOutPtTreatmentPre != value)
                {
                    _IsSearchOutPtTreatmentPre = value;
                    NotifyOfPropertyChange(() => IsSearchOutPtTreatmentPre);
                }
            }
        }
        //▲===== 
        #endregion
        private void UpdatePrescriptionsDate(long PtRegistrationID, DateTime PrescriptionsDate)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginUpdatePrescriptionsDate(PtRegistrationID, PrescriptionsDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                client.EndUpdatePrescriptionsDate(asyncResult);
                            }
                            catch (Exception innerEx)
                            {
                                MessageBox.Show(innerEx.Message);
                            }
                        }), null);
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
            });
            t.Start();
        }

        private bool _IsSearchForCashAdvance = false;
        public bool IsSearchForCashAdvance
        {
            get { return _IsSearchForCashAdvance; }
            set
            {
                if (_IsSearchForCashAdvance != value)
                {
                    _IsSearchForCashAdvance = value;
                }
                NotifyOfPropertyChange(() => IsSearchForCashAdvance);
            }
        }
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