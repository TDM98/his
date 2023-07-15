using eHCMSLanguage;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using DataEntities;
using System.Linq;
using System.Collections.Generic;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.ReportModel.ReportModels;
using aEMR.ServiceClient;
using Microsoft.Win32;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using DevExpress.ReportServer.Printing;
using System.ComponentModel;
/*
* 20170512 #001 CMN: Added Control and method for filter PCL Report
* 20190315 #002 TNHX: [BM0006654] create report "BaoCaoNhapXuatTheoMucDich" && Get List WareHouse for selected
* 20190417 #003 TNHX: [BM0006753] create report "BCPXKhoBHYTChuaThuTien"
* 20190419 #004 TNHX: [BM0006757] create report "BCThoiGianBNChoXN"
* 20190426 #005 TNHX: [BM0006797] add report BCChiTietDVCLS
* 20190427 #006 TNHX: [BM0006799] add report BCDoanhThuThuocBHYTNgTru
* 20190517 #007 TNHX: [BM0006878] add report BCThoigianBNChoTaiBV
* 20190604 #008 TNHX: [BMBM0011782] Apply new Lookup for HIStore
* 20190608 #009 TNHX: [BM0006715] Export excel for accountant
* 20190611 #010 TNHX: [BM0011776] Create report BCChiTietNhapTuNCC
* 20190611 #011 TNHX: [BM0011776] Create report BCBNHoanTra
* 20190618 #012 TNHX: [BM0011776] Create report KHTH_TKTinhHinhKCB
* 20190620 #013 TNHX: [BM0011868] Create report KT_TinhHinhHoatDongDV
* 20190702 #014 TNHX: [BM0011914] Create report BCBNXuatVienConNoVienPhi
* 20190704 #015 TNHX: [BM0011776] Create report KT_BCTraSau
* 20190917 #016 TNHX: Add report for Consultation
* 20200324 #017 TNHX: [BM ] Create report DLS_BCXuatThuocBNKhoaPhong
* 20200407 #018 TNHX: [BM ] Create report BC_BNTaiKhamBenhManTinh
* 20200624 #020 TNHX: [BM ] Create export excel for BC_NHAP_PTTT_KHOA_PHONG
* 20200725 #021 TTM:    BM 0039327: Mở lại báo cáo chi tiết bảng kê thu phí khám chữa bệnh và CĐHA
* 20201213 #022 TNHX:   BM: Thêm báo cáo cho KHTH
* 20210615 #023 DatTB: Báo Cáo bệnh nhân khám bệnh BC_BenhNhanKhamBenh
* 20210615 #024 DatTB: Báo Cáo Bệnh Nhân Hẹn Tái Khám Bệnh Đặc Trưng BC_BNHenTaiKhamBenhDacTrung
* 20210615 #024 DatTB: Báo Cáo Thời Gian Chờ Trong Bệnh Viện BC_TGianChoTrongBV
* 20220214 #025 QTD:   Báo cáo bệnh đặc trưng
* 20220215 #026 QTD:   Export Excel BC quyết toán
* 20220217 #027 QTD:   Thêm Filter BC XRptDLS_BCXuatThuocBNKhoaPhong
* 20220305 #028 QTD:   Thêm BC đo DHST
* 20220330 #029 DatTB:  Thêm Báo cáo quản lý kiểm duyệt hồ sơ KHTH
* 20220407 #030 DatTB:  Thêm Báo cáo kiểm tra lịch sử KCB DLS
* 20220517 #031 DatTB: Thêm trường chọn CLS Đã/Chưa thực hiện
* 20220518 #032 DatTB: Báo cáo danh sách dịch vụ kỹ thuật xét nghiệm
* 20220518 #033 DatTB: Báo cáo lượt xét nghiệm
* 20220524 #034 DatTB: Thêm ShowDischarge chọn đã/chưa xuất viện
* 20220523 #035 DatTB: Báo cáo Doanh thu theo Khoa
* 20220524 #036 DatTB: Thêm radio chọn chi tiết/ tổng hợp để xem in hoặc xuất excel
* 20220525 #037 DatTB: Thêm Báo Cáo Hoãn/Miễn Tạm Ứng Nội Trú
* 20220527 #038 BLQ: Thêm Báo cáo tổng hợp 
* - Thu tiền viện phí ngoại trú
* - Hủy hoàn tiền viện phí ngoại trú
* - Thu tiền tạm ứng ngoại trú
* - Hủy hoàn tiền tạm ứng ngoại trú
* 20220620 #039 DatTB: Thêm filter danh mục xét nghiệm
* - Thêm điều kiện hiển thị filter danh mục xét nghiệm
* 20220624 #040 QTD    BC BN Trễ hẹn điều trị ngoại trú
* 20220727 #041 DatTB: Thêm biến IsExportExcel (Stored có nhưng không thấy truyền lên)
* 20220728 #042 DatTB:
* + Báo cáo giao ban ngoại trú
* + Báo cáo chỉ định cận lâm sàng – khoa khám
* 20220807 #041 DatTB: Báo cáo thống kê số lượng hồ sơ điều trị ngoại trú
* + Tạo màn hình, thêm các trường lọc dữ liệu.
* + Thêm trường phòng khám sau khi chọn khoa.
* + Validate các trường lọc.
* + Thêm điều kiện để lấy khoa theo list DeptID.
* 20220808 #044 QTD: Báo cáo DS BN điều trị ngoại trú
* 20220927 #045 DatTB: Báo cáo danh sách bệnh nhân ĐTNT
* 20221019 #046 BLQ: Thêm báo cáo Phát hành thẻ Khám chữa bệnh,  BÁO CÁO THỐNG KÊ SỐ LƯỢNG THẺ KHÁM CHỮA BỆNH
* 20221022 #047 DatTB: Báo cáo BN trễ hẹn điều trị ngoại trú: Thêm cột "Ngày tái khám", Thêm bộ lọc "Nhóm bệnh" ĐTNT
* 20221117 #048 DatTB: - IssueID: 2241 | Thêm trường chọn mẫu cho các sổ khám bệnh.
* 20221125 #049 BLQ: Thêm báo cáo giờ làm thêm bác sĩ
* 20221128 #050 DatTB: Thêm báo cáo thao tác người dùng
* 20221201 #051 DatTB: Thêm báo cáo thống kê hồ sơ ĐTNT
* 20221220 #052 DatTB: Thêm biến xác định mẫu nội bộ, BYT
* 20230111 #053 DatTB: Thêm ComboBox đối tượng khám bệnh
* 20230114 #054 DatTB: Clone báo cáo chi tiết DC&CLS
* 20230304 #056 DatTB: Thêm bộ lọc kho nhận
* 20230218 #055 DatTB: Thêm Báo cáo thời gian tư vấn cấp toa/chỉ định
* 20230309 #056 BLQ: Thêm báo cáo DLS
* 20230513 #057 DatTB: Thêm Báo cáo thời gian chờ tại bệnh viện
* 20230526 #058 DatTB: Thêm Báo cáo thống kê đơn thuốc điện tử
* 20230626 #059 QTD:   Báo cáo DS bệnh nhân gửi SMS
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICommonReportByDDMMYYYY)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportByDDMMYYYYViewModel : Conductor<object>, ICommonReportByDDMMYYYY
    {
        [ImportingConstructor]
        public ReportByDDMMYYYYViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            RptParameters = new ReportParameters
            {
                V_PaymentMode = new Lookup()
            };
            FillInPatientDeptStatus_Idx();
            FillCondition();
            FillMonth();
            FillQuarter();
            FillYear();
            Lookup firstItem = new Lookup
            {
                LookupID = 0,
                ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
            };

            AllPaymentMode = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PAYMENT_MODE).ToObservableCollection();

            AllPaymentMode.Insert(0, firstItem);

            SetDefaultPaymentMode();//--27/01/2021 DatTB

            //▼==== #041
            OutPtTreatmentStatus = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_OutPtTreatmentStatus).ToObservableCollection();

            OutPtTreatmentStatus.Insert(0, firstItem);

            SetDefaultOutPtTreatmentStatus();

            GetAllOutpatientTreatmentType();
            //▲==== #041

            //▼==== #048
            HealthRecordsTypes = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_HealthRecordsType).ToObservableCollection();

            SetDefaultHealthRecordsType();
            //▲==== #048

            //▼==== #057
            ExaminationProcess = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ExaminationProcess).ToObservableCollection();

            SetDefaultExaminationProcess();
            //▲==== #057

            //▼==== #058
            HIReportStatus = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_HIReportStatus).ToObservableCollection();

            SetDefaultHIReportStatus();
            //SetDefaultFindPatients();
            //▲==== #058

            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            //IsEnableRoleUser = PermissionManager.IsAdminUser();
            SelectedStaff = Globals.LoggedUserAccount.Staff;
            if (!Globals.isAccountCheck)
            {
                isAllStaff = true;
            }

            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();

            DepartmentContent.AddSelectOneItem = false;

            if (!Globals.isAccountCheck)
            {
                DepartmentContent.AddSelectedAllItem = true;
            }

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;
                DepartmentContent.LoadData();
            }

            //▼==== #041
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DepartmentContent_PropertyChanged);
            //▲==== #041

            LoadLocations(Globals.AllRefDepartmentList.Where(x => x.V_DeptTypeOperation == 7010).FirstOrDefault().DeptID);
            Coroutine.BeginExecute(DoGetDrugClassList());

            FromDateTime = Globals.GetViewModel<IMinHourDateControl>();
            FromDateTime.DateTime = Globals.GetCurServerDateTime();

            ToDateTime = Globals.GetViewModel<IMinHourDateControl>();
            ToDateTime.DateTime = Globals.GetCurServerDateTime();

            //▼==== #059
            SMSStatusCollections = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_SendSMSStatus).ToObservableCollection();
            SMSStatusCollections.Insert(0, new Lookup { LookupID = 0, ObjectValue = "Tất cả" });
            SetDefaultSMSStatusCollections();
            //▲==== #059
        }

        #region Properties Member
        private IMinHourDateControl _FromDateTime;
        public IMinHourDateControl FromDateTime
        {
            get { return _FromDateTime; }
            set
            {
                _FromDateTime = value;
                NotifyOfPropertyChange(() => FromDateTime);
            }
        }
        private IMinHourDateControl _ToDateTime;
        public IMinHourDateControl ToDateTime
        {
            get { return _ToDateTime; }
            set
            {
                _ToDateTime = value;
                NotifyOfPropertyChange(() => ToDateTime);
            }
        }
        public void LoadListStaff(byte type)
        {
            Coroutine.BeginExecute(LoadStaffHaveRegistrationList(type));
        }

        private IEnumerator<IResult> LoadStaffHaveRegistrationList(byte type)
        {
            var paymentTypeTask = new LoadStaffHaveRegistrationListTask(false, true, type);
            yield return paymentTypeTask;
            StaffList = paymentTypeTask.StaffList;
            yield break;
        }

        private bool _IsReportForKHTH = false;
        public bool IsReportForKHTH
        {
            get
            {
                return _IsReportForKHTH;
            }
            set
            {
                if (_IsReportForKHTH == value)
                    return;
                _IsReportForKHTH = value;
                if (DepartmentContent != null && _IsReportForKHTH == true)
                {
                    DepartmentContent.AddSelectedAllItem = true;
                    DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;
                    DepartmentContent.LoadData();
                }
                NotifyOfPropertyChange(() => IsReportForKHTH);
            }
        }
        private bool _IsDateTime = false;
        public bool IsDateTime
        {
            get
            {
                return _IsDateTime;
            }
            set
            {
                if (_IsDateTime == value)
                    return;
                _IsDateTime = value;
                if (_IsDateTime == true)
                {
                    IsDate = Visibility.Collapsed;
                }
                NotifyOfPropertyChange(() => IsDateTime);
            }
        }
        private bool _IsShowPaymentMode;
        public bool IsShowPaymentMode
        {
            get
            {
                return _IsShowPaymentMode;
            }
            set
            {
                if (_IsShowPaymentMode == value)
                    return;
                _IsShowPaymentMode = value;
                NotifyOfPropertyChange(() => IsShowPaymentMode);
            }
        }

        private int _ReportSwitch;
        public int ReportSwitch
        {
            get
            {
                return _ReportSwitch;
            }
            set
            {
                if (_ReportSwitch == value)
                    return;
                _ReportSwitch = value;
                NotifyOfPropertyChange(() => ReportSwitch);
            }
        }

        private ObservableCollection<Staff> _StaffList;
        public ObservableCollection<Staff> StaffList
        {
            get
            {
                return _StaffList;
            }
            set
            {
                if (_StaffList == value)
                    return;
                _StaffList = value;
                NotifyOfPropertyChange(() => StaffList);
            }
        }

        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                    return;
                _SelectedStaff = value;
                NotifyOfPropertyChange(() => SelectedStaff);
            }
        }

        private string _strHienThi = Globals.PageName;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        public class Condition
        {
            private readonly string _Text;
            private readonly long _Value;
            public string Text { get { return _Text; } }
            public long Value { get { return _Value; } }
            public Condition(string theText, long theValue)
            {
                _Text = theText;
                _Value = theValue;
            }
        }

        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
                NotifyOfPropertyChange(() => IsEnabledcbxCondition);
                IsExportVisible = false;
                /* ==== #001 ====*/
                if (eItem == ReportName.ULTRASOUND_STATISTICS)
                {
                    LoadLocations(Globals.AllRefDepartmentList.Where(x => x.V_DeptTypeOperation == 7013).FirstOrDefault().DeptID);
                    LoadPCLResultParams((long)PCLExamTypeCategory.UltraSound);
                    ShowLocation = true;
                    ShowPCLParams = true;
                }
                //▼====: #009, #030, #035, #041
                else if (eItem == ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT || eItem == ReportName.BCChiTietDV_CLS || eItem == ReportName.KT_BaoCaoDoanhThuNgTru
                    || eItem == ReportName.ThongKeDsachKBNoiTruTheoBacSi || eItem == ReportName.ThongKeDsachKBNgTruTheoBacSi || eItem == ReportName.BangKeBacSiThucHienPT_TT
                    || eItem == ReportName.BangKeBanLeHangHoaDV || eItem == ReportName.KTTH_BC_XUAT_VIEN || eItem == ReportName.KTTH_BC_CHI_DINH_NHAP_VIEN
                    || eItem == ReportName.BangKeBacSiThucHienCLS || eItem == ReportName.BangKeThuTamUngNT || eItem == ReportName.BangKeThuHoanUngNT
                    || eItem == ReportName.BC_BNTaiKhamBenhManTinh || eItem == ReportName.BC_ToaThuocHangNgay_DLS || eItem == ReportName.BC_ToaThuocHangNgay_KD || eItem == ReportName.BC_ThuocYCuDoDangCuoiKy
                    || eItem == ReportName.BC_NHAP_PTTT_KHOA_PHONG || eItem == ReportName.XRptSoDienTim || eItem == ReportName.XRptSoKhamBenh || eItem == ReportName.XRptSoThuThuat
                    || eItem == ReportName.XRptSoSieuAm || eItem == ReportName.XRptSoXetNghiem || eItem == ReportName.XRptSoXQuang || eItem == ReportName.BC_NHAP_PTTT_KHOA_PHONG_KHTH
                    || eItem == ReportName.BC_BenhNhanKhamBenh || eItem == ReportName.BC_BNHenTaiKhamBenhDacTrung || eItem == ReportName.BC_CONGNO_NOITRU
                    || eItem == ReportName.BC_ChiTietKhamBenh || eItem == ReportName.BC_HuyDichVu_NgT || eItem == ReportName.TEMP79a_BCBENHDACTRUNG || eItem == ReportName.REPORT_PATIENT_SETTLEMENT || eItem == ReportName.BC_DoDHST
                    || eItem == ReportName.KTTH_BC_QLKiemDuyetHoSo || eItem == ReportName.DLS_BCKiemTraLichSuKCB || eItem == ReportName.DLS_BCThongTinDanhMucThuoc
                    || eItem == ReportName.XRptSoNoiSoi || eItem == ReportName.BCTinhHinhCLS_DaThucHien || eItem == ReportName.BaoCaoDoanhThuTheoKhoa || eItem == ReportName.XRptSoDoChucNangHoHap
                //▲====: #009, #030, #035
                    || eItem == ReportName.KTTH_BC_QLKiemDuyetHoSo || eItem == ReportName.DLS_BCKiemTraLichSuKCB || eItem == ReportName.DLS_BCThongTinDanhMucThuoc
                    || eItem == ReportName.XRptSoNoiSoi || eItem == ReportName.BCTinhHinhCLS_DaThucHien || eItem == ReportName.BaoCaoDoanhThuTheoKhoa || eItem == ReportName.XRptSoDoChucNangHoHap
                    || eItem == ReportName.BCThongKeSLHoSoDTNT || eItem == ReportName.BCDanhSachBNDTNT_KHTH
                //▲====: #009, #030, #035, #041
                //▼==== #045
                    || eItem == ReportName.BCDanhSachBenhNhanDTNT
                //▲==== #045
                //▼==== #045
                    || eItem == ReportName.XRpt_PhatHanhTheKCB
                //▲==== #045
                //▼==== #050
                    || eItem == ReportName.BCThaoTacNguoiDung
                //▲==== #050
                //▼==== #051
                    || eItem == ReportName.BCThongKeHoSoDTNT
                    //▲==== #051
                    //▼==== #049
                    || eItem == ReportName.XRpt_GioLamThemBacSi
                    //▲==== #049
                    //▼==== #053
                    || eItem == ReportName.BCThGianTuVanCapToa || eItem == ReportName.BCThGianTuVanChiDinh
                    //▲==== #035
                    //▼==== #056
                    || eItem == ReportName.BC_DLS_KhamBenhNgoaiTru || eItem == ReportName.BC_DLS_CLSNgoaiTru || eItem == ReportName.BC_DLS_ThuocNgoaiTru
                    //▲==== #056
                    //▼==== #057
                    || eItem == ReportName.XRpt_BCThGianCho_Khukham
                    //▲==== #057
                    ////▼==== #058
                    //|| eItem == ReportName.XRpt_BCTThongKeDTDT
                    ////▲==== #058
                    || eItem == ReportName.BaoCaoXNChuaNhapKetQua)
                {
                    IsExportVisible = true;
                }
                //▼==== #032, #033
                else if (eItem == ReportName.DsachBNChiDinhThucHienXN || eItem == ReportName.TinhHinhHoatDongCLS_XN || eItem == ReportName.BCDsachDichVuKyThuatXN || eItem == ReportName.BaoCaoLuotXetNghiem)
                {
                    LoadPCLSections_All();
                    LoadExamTypes_All();
                }
                //▲==== #032, #033
                /* ==== #001 ====*/
                if (eItem == ReportName.BC_CONGNO_NOITRU)
                {
                    DepartmentContent.AddSelectedAllItem = true;
                    DepartmentContent.LoadData(0, true);
                }
                //▼==== #041
                if (eItem == ReportName.BCThongKeSLHoSoDTNT || eItem == ReportName.BCDanhSachBNDTNT_KHTH)
                {
                    var FilterDept = new ObservableCollection<long>();
                    FilterDept.Add((long)AllLookupValues.DeptID.CapCuu);
                    FilterDept.Add((long)AllLookupValues.DeptID.KhoaKham);
                    GetDepartmentByListID(FilterDept);
                    DepartmentContent.AddSelectedAllItem = true;
                    DepartmentContent.LoadData(0, true, false, true);
                }
                //▲==== #041
                if (eItem == ReportName.BangKeBacSiThucHienCLS)
                {
                    mSettlement = true;
                }
                if (eItem == ReportName.BangKeBacSiThucHienPT_TT || eItem == ReportName.DS_BNTiepNhanTheoDT)
                {
                    IsDateTime = true;
                }
                if (eItem == ReportName.BaoCaoDSBenhNhanCapNhatKetQua)
                {
                    IsVisibleKSK = true;
                }
            }
        }

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }

        private ReportParameters _RptParameters;
        public ReportParameters RptParameters
        {
            get { return _RptParameters; }
            set
            {
                if (_RptParameters != value)
                {
                    _RptParameters = value;
                    NotifyOfPropertyChange(() => RptParameters);
                }
            }
        }

        private ObservableCollection<int> _ListMonth;
        public ObservableCollection<int> ListMonth
        {
            get { return _ListMonth; }
            set
            {
                if (_ListMonth != value)
                {
                    _ListMonth = value;
                    NotifyOfPropertyChange(() => ListMonth);
                }
            }
        }

        private ObservableCollection<int> _ListQuartar;
        public ObservableCollection<int> ListQuartar
        {
            get { return _ListQuartar; }
            set
            {
                if (_ListQuartar != value)
                {
                    _ListQuartar = value;
                    NotifyOfPropertyChange(() => ListQuartar);
                }
            }
        }


        private ObservableCollection<int> _ListYear;
        public ObservableCollection<int> ListYear
        {
            get { return _ListYear; }
            set
            {
                if (_ListYear != value)
                {
                    _ListYear = value;
                    NotifyOfPropertyChange(() => ListYear);
                }
            }
        }

        private ObservableCollection<Lookup> _AllPaymentMode;
        public ObservableCollection<Lookup> AllPaymentMode
        {
            get
            {
                return _AllPaymentMode;
            }
            set
            {
                if (_AllPaymentMode != value)
                {
                    _AllPaymentMode = value;
                    NotifyOfPropertyChange(() => AllPaymentMode);
                }
            }
        }

        private void SetDefaultPaymentMode()
        {
            if (AllPaymentMode != null)
            {
                RptParameters.V_PaymentMode = AllPaymentMode.FirstOrDefault(); //--27/01/2021 DatTB Fix Select default
            }
        }

        private ObservableCollection<Condition> _Conditions;
        public ObservableCollection<Condition> Conditions
        {
            get
            {
                return _Conditions;
            }
            set
            {
                if (_Conditions != value)
                {
                    _Conditions = value;
                    NotifyOfPropertyChange(() => Conditions);
                }
            }
        }

        private Condition _CurrentCondition;
        public Condition CurrentCondition
        {
            get
            {
                return _CurrentCondition;
            }
            set
            {
                if (_CurrentCondition != value)
                {
                    _CurrentCondition = value;
                    NotifyOfPropertyChange(() => CurrentCondition);
                }
            }
        }


        private ObservableCollection<Condition> _InPatientDeptStatus_Idx;
        public ObservableCollection<Condition> InPatientDeptStatus_Idx
        {
            get
            {
                return _InPatientDeptStatus_Idx;
            }
            set
            {
                if (_InPatientDeptStatus_Idx != value)
                {
                    _InPatientDeptStatus_Idx = value;
                    NotifyOfPropertyChange(() => InPatientDeptStatus_Idx);
                }
            }
        }

        private Condition _CurrentInPatientDeptStatus;
        public Condition CurrentInPatientDeptStatus
        {
            get
            {
                return _CurrentInPatientDeptStatus;
            }
            set
            {
                if (_CurrentInPatientDeptStatus != value)
                {
                    _CurrentInPatientDeptStatus = value;
                    NotifyOfPropertyChange(() => CurrentInPatientDeptStatus);
                }
            }
        }


        private Visibility _IsMonth;
        public Visibility IsMonth
        {
            get
            { return _IsMonth; }
            set
            {
                if (_IsMonth != value)
                {
                    _IsMonth = value;
                    NotifyOfPropertyChange(() => IsMonth);
                }
            }
        }

        private Visibility _IsDate = Visibility.Collapsed;
        public Visibility IsDate
        {
            get
            { return _IsDate; }
            set
            {
                if (_IsDate != value)
                {
                    _IsDate = value;
                    NotifyOfPropertyChange(() => IsDate);
                }
            }
        }

        private Visibility _IsQuarter;
        public Visibility IsQuarter
        {
            get
            { return _IsQuarter; }
            set
            {
                if (_IsQuarter != value)
                {
                    _IsQuarter = value;
                    NotifyOfPropertyChange(() => IsQuarter);
                }
            }
        }

        private Visibility _IsYear;
        public Visibility IsYear
        {
            get
            { return _IsYear; }
            set
            {
                if (_IsYear != value)
                {
                    _IsYear = value;
                    NotifyOfPropertyChange(() => IsYear);
                }
            }
        }

        int FindPatient = (int)AllLookupValues.V_FindPatientType.NGOAI_TRU;
        public void rdtNgoaiTru_Checked(object sender, RoutedEventArgs e)
        {
            FindPatient = (int)AllLookupValues.V_FindPatientType.NGOAI_TRU;
            CheckSettlement();
        }

        public void rdtNoiTru_Checked(object sender, RoutedEventArgs e)
        {
            FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
        }
        public void rdtTatCa_Checked(object sender, RoutedEventArgs e)
        {
            FindPatient = -1;
            CheckSettlement();
        }
        public void rdtKSK_Checked(object sender, RoutedEventArgs e)
        {
            FindPatient = (int)AllLookupValues.PatientFindBy.KSK;
        }
        //▼==== #031
        int PCLStatus = (int)AllLookupValues.PCLStatus.DaThucHien;
        public void rdtPCLDaTH_Checked(object sender, RoutedEventArgs e)
        {
            PCLStatus = (int)AllLookupValues.PCLStatus.DaThucHien;
        }

        public void rdtPCLChuaTH_Checked(object sender, RoutedEventArgs e)
        {
            PCLStatus = (int)AllLookupValues.PCLStatus.ChuaThucHien;
        }
        //▲==== #031

        //▼==== #034
        bool HasDischarge = true;
        public void rdtDaXV_Checked(object sender, RoutedEventArgs e)
        {
            HasDischarge = true;
        }

        public void rdtChuaXV_Checked(object sender, RoutedEventArgs e)
        {
            HasDischarge = false;
        }
        //▲==== #034

        int V_RegForPatientOfType = (int)AllLookupValues.V_RegForPatientOfType.Unknown;
        public void rdtDKNoiTru_Checked(object sender, RoutedEventArgs e)
        {
            V_RegForPatientOfType = (int)AllLookupValues.V_RegForPatientOfType.Unknown;
        }

        public void rdtDKVangLai_Checked(object sender, RoutedEventArgs e)
        {
            V_RegForPatientOfType = (int)AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI;
        }

        public void rdtDKTienGiaiPhau_Checked(object sender, RoutedEventArgs e)
        {
            V_RegForPatientOfType = (int)AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT;
        }

        private bool _mSettlement = false;
        public bool mSettlement
        {
            get
            {
                return _mSettlement;
            }
            set
            {
                _mSettlement = value;
                NotifyOfPropertyChange(() => mSettlement);
            }
        }
        private bool _Settlement = true;
        public bool Settlement
        {
            get
            {
                return _Settlement;
            }
            set
            {
                _Settlement = value;
                NotifyOfPropertyChange(() => Settlement);
            }
        }
        public void rdtSettled_Checked(object sender, RoutedEventArgs e)
        {
            CheckSettlement();
        }

        private void CheckSettlement()
        {
            if (eItem != ReportName.BangKeBacSiThucHienCLS)
            {
                return;
            }
            if (!Settlement && FindPatient != (int)AllLookupValues.V_FindPatientType.NOI_TRU)
            {
                MessageBox.Show("Chưa quyết toán chỉ áp dụng cho nội trú");
                Settlement = true;
            }
        }
        #endregion

        #region FillData Member

        private void FillInPatientDeptStatus_Idx()
        {
            if (InPatientDeptStatus_Idx == null)
            {
                InPatientDeptStatus_Idx = new ObservableCollection<Condition>();
            }
            else
            {
                InPatientDeptStatus_Idx.Clear();
            }

            InPatientDeptStatus_Idx.Add(new Condition(eHCMSResources.T0822_G1_TatCa, 0));
            InPatientDeptStatus_Idx.Add(new Condition(eHCMSResources.Z1271_G1_NhapKhoa, 1));
            InPatientDeptStatus_Idx.Add(new Condition(eHCMSResources.Z1272_G1_DaDTri, 2));
            InPatientDeptStatus_Idx.Add(new Condition(eHCMSResources.Z1273_G1_XuatKhoa, 3));
            InPatientDeptStatus_Idx.Add(new Condition(eHCMSResources.G2900_G1_XV, 5));
            InPatientDeptStatus_Idx.Add(new Condition("Không đồng ý nhập viện", 6));
            InPatientDeptStatus_Idx.Add(new Condition("Huỷ HSBA", 7));

            CurrentInPatientDeptStatus = InPatientDeptStatus_Idx[2];
        }

        private void FillMonth()
        {
            if (ListMonth == null)
            {
                ListMonth = new ObservableCollection<int>();
            }
            else
            {
                ListMonth.Clear();
            }
            for (int i = 1; i < 13; i++)
            {
                ListMonth.Add(i);
            }
            RptParameters.Month = Globals.ServerDate.Value.Month;
        }

        private void FillQuarter()
        {
            if (ListQuartar == null)
            {
                ListQuartar = new ObservableCollection<int>();
            }
            else
            {
                ListQuartar.Clear();
            }
            for (int i = 1; i < 5; i++)
            {
                ListQuartar.Add(i);
            }
            int Month = Globals.ServerDate.Value.Month;
            if (Month <= 3)
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 1;
            }
            else if ((Month >= 4) && (Month <= 6))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 2;
            }
            else if ((Month >= 7) && (Month <= 9))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 3;
            }
            else // 4th Quarter = October 1 to December 31
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 4;
            }
        }

        private void FillYear()
        {
            if (ListYear == null)
            {
                ListYear = new ObservableCollection<int>();
            }
            else
            {
                ListYear.Clear();
            }
            int year = Globals.ServerDate.Value.Year;
            for (int i = year; i > year - 3; i--)
            {
                ListYear.Add(i);
            }
            RptParameters.Year = year;
        }

        private void FillCondition()
        {
            if (Conditions == null)
            {
                Conditions = new ObservableCollection<Condition>();
            }
            else
            {
                Conditions.Clear();
            }

            Conditions.Add(new Condition(eHCMSResources.Z0938_G1_TheoQuy, 0));
            Conditions.Add(new Condition(eHCMSResources.Z0939_G1_TheoTh, 1));
            Conditions.Add(new Condition(eHCMSResources.G0375_G1_TheoNg, 2));

            CurrentCondition = Conditions.LastOrDefault();
            ByDate();
        }

        public void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCondition != null)
            {
                switch (CurrentCondition.Value)
                {
                    case 0:
                        ByQuarter();
                        break;
                    case 1:
                        ByMonth();
                        break;
                    case 2:
                        ByDate();
                        break;
                }
            }
        }

        private void ByDate()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Visible;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
            if (eItem == ReportName.BangKeBacSiThucHienPT_TT || eItem == ReportName.DS_BNTiepNhanTheoDT)
            {
                IsDateTime = true;
            }
        }

        private void ByMonth()
        {
            IsMonth = Visibility.Visible;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Visible;
            if (eItem == ReportName.BangKeBacSiThucHienPT_TT || eItem == ReportName.DS_BNTiepNhanTheoDT)
            {
                IsDateTime = false;
            }
        }

        private void ByQuarter()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Visible;
            IsYear = Visibility.Visible;
            if (eItem == ReportName.BangKeBacSiThucHienPT_TT || eItem == ReportName.DS_BNTiepNhanTheoDT)
            {
                IsDateTime = false;
            }
        }

        #endregion

        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }


        //▼==== #041
        private void GetDepartmentByListID(ObservableCollection<long> FilterDept)
        {
            DepartmentContent.LstRefDepartment = null;

            var tmpFilterDept = new ObservableCollection<long>();

            foreach (var gDept in FilterDept)
            {
                if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(gDept))
                {
                    tmpFilterDept.Add(gDept);
                }
            }
            DepartmentContent.LstRefDepartment = tmpFilterDept;
        }

        void DepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (DepartmentContent.SelectedItem != null)
                {
                    LoadLocations(DepartmentContent.SelectedItem.DeptID);
                }
            }
        }
        //▲==== #041

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private bool _mDepartment;
        public bool mDepartment
        {
            get
            {
                return _mDepartment;
            }
            set
            {
                if (_mDepartment == value)
                    return;
                _mDepartment = value;
                NotifyOfPropertyChange(() => mDepartment);
            }
        }


        private bool _mInPatientDeptStatus;
        public bool mInPatientDeptStatus
        {
            get
            {
                return _mInPatientDeptStatus;
            }
            set
            {
                if (_mInPatientDeptStatus == value)
                    return;
                _mInPatientDeptStatus = value;
                NotifyOfPropertyChange(() => mInPatientDeptStatus);
            }
        }

        private bool _mRegistrationType;
        public bool mRegistrationType
        {
            get
            {
                return _mRegistrationType;
            }
            set
            {
                if (_mRegistrationType == value)
                    return;
                _mRegistrationType = value;
                NotifyOfPropertyChange(() => mRegistrationType);
            }
        }

        private bool _mXemChiTiet = false;
        public bool mXemChiTiet
        {
            get
            {
                return _mXemChiTiet;
            }
            set
            {
                if (_mXemChiTiet == value)
                    return;
                _mXemChiTiet = value;
                NotifyOfPropertyChange(() => mXemChiTiet);
            }
        }

        private bool _mXemIn = true;
        private bool _mIn = false;

        public bool mXemIn
        {
            get
            {
                return _mXemIn;
            }
            set
            {
                if (_mXemIn == value)
                    return;
                _mXemIn = value;
                NotifyOfPropertyChange(() => mXemIn);
            }
        }
        public bool mIn
        {
            get
            {
                return _mIn;
            }
            set
            {
                if (_mIn == value)
                    return;
                _mIn = value;
                NotifyOfPropertyChange(() => mIn);
            }
        }

        private bool _isAucStaff;
        public bool isAucStaff
        {
            get
            {
                return _isAucStaff;
            }
            set
            {
                if (_isAucStaff != value)
                {
                    _isAucStaff = value;
                    NotifyOfPropertyChange(() => isAucStaff);
                    if (isAucStaff)
                    {
                        //SelectedStaff = Globals.LoggedUserAccount.Staff;
                    }
                    else
                    {
                        SelectedStaff = Globals.LoggedUserAccount.Staff;
                    }
                }
            }
        }

        private bool _isAllStaff = true;
        public bool isAllStaff
        {
            get
            {
                return _isAllStaff;
            }
            set
            {
                if (_isAllStaff != value)
                {
                    _isAllStaff = value;
                    NotifyOfPropertyChange(() => isAllStaff);
                }
            }
        }

        private bool _IsEnableRoleUser;
        public bool IsEnableRoleUser
        {
            get { return _IsEnableRoleUser; }
            set
            {
                _IsEnableRoleUser = value;
                NotifyOfPropertyChange(() => IsEnableRoleUser);
            }
        }

        private void GetReport(ReportName _eItem, bool isDetail = false)
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            string reportHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            string reportHospitalCode = Globals.ServerConfigSection.Hospitals.HospitalCode;
            //▼===== #021
            if (_eItem == ReportName.BANGLETHUPHI_KB_CDHA_THEONGAY && isDetail)
            {
                _eItem = ReportName.BANGLETHUPHICHITIET_KB_CDHA_THEONGAY;
            }
            //▲===== #021
            switch (_eItem)
            {
                case ReportName.BAO_CAO_VIEN_PHI_BHYT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoThuVienPhiBHYT").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffCreate"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffCreate"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["Case"].Value = Case;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0; //--26/01/2021 DatTB Thêm biến lọc theo PT Thanh Toán
                    break;
                case ReportName.BAO_CAO_VIEN_PHI_NGOAI_TRU:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoThuVienPhiNgoaiTru").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffCreate"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffCreate"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["Case"].Value = Case;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0; //--26/01/2021 DatTB Thêm biến lọc theo PT Thanh Toán
                    break;
                case ReportName.DANHSACH_DANGKY_BHYT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptListPatientInsurance").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["FindPatient"].Value = FindPatient;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["StaffName"].Value = SelectedStaff != null ? SelectedStaff.FullName : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    break;
                case ReportName.BAOCAO_MIENGIAM_NGOAITRU_TV:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoMienGiamNgoaiTru_TV").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffName"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffName"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["LoginStaffName"].Value = Globals.LoggedUserAccount.Staff.FullName;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    break;
                case ReportName.BAOCAO_MIENGIAM_NOITRU_TV:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoMienGiamNoiTru_TV").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffName"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffName"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["LoginStaffName"].Value = Globals.LoggedUserAccount.Staff.FullName;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    break;
                case ReportName.BAO_CAO_TRE_EM_DUOI_6_TUOI:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoTreEmDuoi6Tuoi").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.DS_BNTiepNhanTheoDT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptDSBNTiepNhanTheoDoiTuong").PreviewModel;
                    rParams["FromDate"].Value = FromDateTime.DateTime;
                    rParams["ToDate"].Value = ToDateTime.DateTime;
                    rParams["LoginStaffName"].Value = Globals.LoggedUserAccount.Staff.FullName;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    break;
                case ReportName.TKe_TNhanTheoDiaBanCuTru:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptTKeTiepNhanTheoDiaBanCTru").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoginStaffName"].Value = Globals.LoggedUserAccount.Staff.FullName;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.BANGKETHUPHI_XN_THEONGAY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeThuPhiXNTheoNgay").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ShowNgayThang"].Value = RptParameters.Show;
                    rParams["paraNote"].Value = string.Format("{0} ", eHCMSResources.G2613_G1_XN.ToUpper()) + RptParameters.Show;
                    rParams["paraStaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["paraStaffName"].Value = SelectedStaff != null ? SelectedStaff.FullName : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.BANGLETHUPHI_KB_CDHA_THEONGAY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeThuPhiKhamVaCDHATheoNgay").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ShowNgayThang"].Value = RptParameters.Show;
                    rParams["paraNote"].Value = string.Format("{0} ", eHCMSResources.Z1133_G1_CDHA.ToUpper()) + RptParameters.Show;
                    rParams["paraStaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["paraStaffName"].Value = SelectedStaff != null ? SelectedStaff.FullName : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;

                case ReportName.BangKeThuTamUngNT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeThuTamUngNT").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0;
                    if (RptParameters.V_PaymentMode != null && RptParameters.V_PaymentMode.LookupID > 0)
                    {
                        rParams["strPaymentMode"].Value = RptParameters.V_PaymentMode.ObjectValue;
                    }
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    break;

                case ReportName.BangKeThuHoanUngNT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeThuHoanUngNT").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    break;

                case ReportName.BangKeThuTienVienPhiNT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeThuTienVienPhiNT").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    break;

                case ReportName.BANGLETHUPHICHITIET_KB_CDHA_THEONGAY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBangKeChiTietThuPhiXetNghiemCDHA").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ShowNgayThang"].Value = RptParameters.Show;
                    //rParams["paraNote"].Value = string.Format("{0} ", eHCMSResources.Z1133_G1_CDHA.ToUpper()) + RptParameters.Show;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    //rParams["paraStaffName"].Value = SelectedStaff != null ? SelectedStaff.FullName : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;

                case ReportName.PHIEUNOPTIEN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPhieuNopTien").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    //rParams["ShowNgayThang"].Value = RptParameters.Show;
                    //rParams["paraNote"].Value = string.Format("{0} ", eHCMSResources.Z1133_G1_CDHA.ToUpper()) + RptParameters.Show;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["StaffName"].Value = SelectedStaff != null ? SelectedStaff.FullName : "";
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;

                case ReportName.BAOCAO_THUTIEN_TAMUNG:
                    //KMx: Sử dụng chung cho Tạm ứng và Thanh toán luôn (03/01/2015 14:03).
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoThuTienTamUng").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["StaffName"].Value = SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName) ? string.Format("{0}: ", eHCMSResources.N0194_G1_NhVien) + SelectedStaff.FullName : "";

                    rParams["ReportType"].Value = ReportSwitch;

                    rParams["V_RegForPatientOfType"].Value = V_RegForPatientOfType;

                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0;


                    if (RptParameters.V_PaymentMode != null && RptParameters.V_PaymentMode.LookupID > 0)
                    {
                        rParams["strPaymentMode"].Value = string.Format("{0}: ", eHCMSResources.T1541_G1_HThucNop) + RptParameters.V_PaymentMode.ObjectValue;
                    }

                    string strRegistrationType = "";

                    if (V_RegForPatientOfType == (int)AllLookupValues.V_RegForPatientOfType.Unknown)
                    {
                        strRegistrationType = string.Format(" {0}", eHCMSResources.K1197_G1_BNNoiTru.ToUpper());
                    }
                    else if (V_RegForPatientOfType == (int)AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
                    {
                        strRegistrationType = string.Format(" {0}", eHCMSResources.Z1135_G1_BNVLai.ToUpper());
                    }
                    else if (V_RegForPatientOfType == (int)AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT)
                    {
                        strRegistrationType = string.Format(" {0}", eHCMSResources.Z1136_G1_BNGPhauKTC.ToUpper());
                    }
                    else
                    {
                        strRegistrationType = "";
                    }

                    if (ReportSwitch == 0)
                    {
                        rParams["ReportTitle"].Value = eHCMSResources.Z1137_G1_BCTgHopThuPhiTU.ToUpper() + strRegistrationType;
                    }
                    else if (ReportSwitch == 1)
                    {
                        rParams["ReportTitle"].Value = eHCMSResources.Z1138_G1_BCTgHopThuPhiTUBill.ToUpper() + strRegistrationType;
                    }
                    else if (ReportSwitch == 2)
                    {
                        rParams["ReportTitle"].Value = eHCMSResources.Z1140_G1_BCTToanChoBN.ToUpper() + strRegistrationType;
                    }
                    else
                    {
                        rParams["ReportTitle"].Value = "";
                    }
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;

                    break;

                case ReportName.REPORT_PATIENT_SETTLEMENT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptPatientSettlementReport").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    //Thêm điều kiện lọc theo phương thức thanh toán
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0;
                    if (RptParameters.V_PaymentMode != null && RptParameters.V_PaymentMode.LookupID > 0)
                    {
                        rParams["strPaymentMode"].Value = RptParameters.V_PaymentMode.ObjectValue;
                    }
                    //rParams["StaffName"].Value = SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName) ? "Nhân viên: " + SelectedStaff.FullName : "";
                    rParams["StaffName"].Value = SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    break;

                case ReportName.REPORT_OUTWARD_MEDDEPT_INFLOW:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptOutwardMedDeptInflowReport").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    rParams["StaffName"].Value = SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName) ? string.Format("{0}: ", eHCMSResources.N0194_G1_NhVien) + SelectedStaff.FullName : "";
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;

                case ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptInPatientImportExportDepartment").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    //rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["ReportTitle"].Value = eHCMSResources.Z1142_G1_BCBNNpXuatKhoa.ToUpper();
                    rParams["Status"].Value = CurrentInPatientDeptStatus != null ? (int)CurrentInPatientDeptStatus.Value : -1;
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    //▼==== #041
                    rParams["IsExportExcel"].Value = false;
                    //▲==== #041
                    break;

                case ReportName.REPORT_PATIENT_ARE_TREATED:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptInPatientImportExportDepartment").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    //rParams["FromDate"].Value = Globals.GetCurServerDateTime().Date;
                    //rParams["ToDate"].Value = Globals.GetCurServerDateTime().Date;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    //rParams["ReportDate"].Value = string.Format("{0} ", eHCMSResources.N0045_G1_Ng.ToUpper()) + Globals.GetCurServerDateTime().Date.ToString("dd/MM/yyyy");
                    rParams["ReportTitle"].Value = "BÁO CÁO BỆNH NHÂN ĐANG ĐIỀU TRỊ NỘI TRÚ";
                    rParams["Status"].Value = 4; // 0 là tất cả trạng thái; 4 là trạng thái bệnh nhân đang điều trị.
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    break;

                case ReportName.REPORT_INPT_NOT_PAY_CASH_ADVANCE:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptInPatientNotPayCashAdvance").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                // HPT: thống kê danh sách phiếu thu khác (26/03/2016 9:48)
                case ReportName.BAOCAO_THUTIEN_KHAC:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BaoCaoPhieuThuKhac").PreviewModel;
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate.Value.AddDays(1);
                        rParams["DateShow"].Value = RptParameters.Show;
                        rParams["parStaffID"].Value = SelectedStaff != null ? (int)(SelectedStaff.StaffID) : 0;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                case ReportName.REPORT_INPT_NOT_PAY_ALL_BILL:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptInPatientDischargeNotPayAllBill").PreviewModel;
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ReportDate"].Value = RptParameters.Show;
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                case ReportName.ULTRASOUND_STATISTICS:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptUltrasoundStatistics").PreviewModel;
                        rParams["parQuarter"].Value = RptParameters.Quarter;
                        rParams["parMonth"].Value = RptParameters.Month;
                        rParams["parYear"].Value = RptParameters.Year;
                        rParams["parFlag"].Value = RptParameters.Flag;
                        rParams["parFromDate"].Value = RptParameters.FromDate;
                        rParams["parToDate"].Value = RptParameters.ToDate;
                        rParams["parReportDate"].Value = RptParameters.Show;
                        rParams["parStaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                        rParams["parStaffName"].Value = SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName) ? string.Format("{0}: ", eHCMSResources.Z1903_G1_BSiSieuAm) + SelectedStaff.FullName : "";
                        /*==== #001 ====*/
                        if (SelectedLocation != null)
                            rParams["DeptLocationID"].Value = Convert.ToInt32(SelectedLocation.DeptLocationID);
                        if (SelectedPCLResultParams != null)
                            rParams["PCLResultParamImpID"].Value = SelectedPCLResultParams == null ? (int?)null : Convert.ToInt32(SelectedPCLResultParams.PCLResultParamImpID);
                        /*==== #001 ====*/
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                        break;
                    }
                case ReportName.BaoCaoThuTienNgoaiTruTheoBienLai:
                    {
                        ReportModel = null;
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XrptBangKeThuTienNgoaiTruTheoBienLai").PreviewModel;
                        rParams["parFromDate"].Value = RptParameters.FromDate;
                        rParams["parToDate"].Value = RptParameters.ToDate;
                        rParams["parStaffID"].Value = isAllStaff ? 0 : (SelectedStaff != null ? SelectedStaff.StaffID : 0);
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                        break;
                    }
                case ReportName.TinhHinhHoatDongCLS_XN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.TinhHinhHoatDongCLS").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.TinhHinhHoatDongCLS_KK:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRpt_TinhHinhHoatDongCLS_KK").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.DsachBNChiDinhThucHienXN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.DsachBNChiDinhThucHienXN").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.BangKeBacSiThucHienCLS:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptBangKeBacSiThucHienCLS").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["Settlement"].Value = Settlement;
                    break;
                case ReportName.BAO_CAO_GIAO_BAN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptBaoCaoGiaoBan").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.KT_BaoCaoDoanhThuNgTru:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptBaoCaoDoanhThuNgoaiTru").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //▼==== #053
                    rParams["PatientClassID"].Value = (SelectedPatientClassification != null) ? SelectedPatientClassification.PatientClassID : 0;
                    //▲==== #053
                    break;
                case ReportName.TinhHinhHoatDongCLS:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_TinhHinhHoatDongCLS").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.ThongKeDsachKBNoiTruTheoBacSi:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ThongKeDsachKBTheoBacSi").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.ThongKeDsachKBNgTruTheoBacSi:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ThongKeDsachKBNgTruTheoBacSi").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.BangKeBacSiThucHienPT_TT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BangKeBacSiThucHienPT_TT").PreviewModel;
                    rParams["parFromDate"].Value = FromDateTime.DateTime;
                    rParams["parToDate"].Value = ToDateTime.DateTime;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                // Báo cáo nhập pttt của khoa phòng
                case ReportName.BC_NHAP_PTTT_KHOA_PHONG:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BaoCaoNhapPTTTKhoaPhong").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";

                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;  // thêm khoa 25022020
                    rParams["parFindPatient"].Value = FindPatient; // thêm nội trú ngoại trú 25022020

                    if (SelectedLocation != null)
                        rParams["DeptLocationID"].Value = Convert.ToInt32(SelectedLocation.DeptLocationID);

                    break;
                // Báo cáo pttt chưa thực hiện
                case ReportName.BaoCaoPT_TTChuaThucHien:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BaoCaoPTTTChuaThucHien").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;

                case ReportName.BangKeBanLeHangHoaDV:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BangKeBanLeHangHoaDV").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.KTTH_BCTinhHinhDichLay:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCTinhHinhDichLay").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.BC_CONGNO_NOITRU:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBaoCaoCongNoNoiTru").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.KTTH_BC_XUAT_VIEN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ThongKeDsachBNXuatVien").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parDeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                    break;
                case ReportName.KTTH_BC_CHI_DINH_NHAP_VIEN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ThongKeDsachBsiChiDinhNhapVien").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parDeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                    break;
                //▲====: #002
                case ReportName.TK_NX_THEOMUCDICH:
                    ReportModel = null;
                    if (_StoreType != null)
                    {
                        switch (_StoreType)
                        {
                            case (long)AllLookupValues.StoreType.STORAGE_CLINIC:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ClinicDeptInOutStatisticsDetail").PreviewModel;
                                } else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ClinicDeptInOutStatistics").PreviewModel;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatisticsDetail").PreviewModel;
                                }
                                else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatistics").PreviewModel;
                                break;
                            case (long)AllLookupValues.StoreType.STORAGE_EXTERNAL:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_DrugInOutStatisticsDetail").PreviewModel;
                                }
                                else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_DrugInOutStatistics").PreviewModel;
                                break;
                            default:
                                if (isDetail)
                                {
                                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatisticsDetail").PreviewModel;
                                }
                                else ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_MedDeptInOutStatistics").PreviewModel;
                                break;
                        }
                        rParams["parFromDate"].Value = RptParameters.FromDate;
                        rParams["parToDate"].Value = RptParameters.ToDate;
                        rParams["StoreID"].Value = CurStore.StoreID;
                        rParams["StoreName"].Value = CurStore.swhlName;
                        rParams["parHospitalName"].Value = reportHospitalName;
                        rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    }
                    break;
                //▲====: #002
                //▼====== #003
                case ReportName.BCPXKhoBHYTChuaThuTien:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XtraReports.XRpt_BCPXKhoBHYTChuaThuTien").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["parStaffID"].Value = (SelectedStaff != null) ? SelectedStaff.StaffID : 0;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Show"].Value = RptParameters.Show;
                    break;
                //▲====: #003
                //▼====== #004
                case ReportName.BCThoiGianBNChoXN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XtraReports.XRptBCThoiGianBNChoXn").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▲====: #004
                //▼====: #005
                //case ReportName.BCChiTietDV_CLS:
                //    ReportModel = null;
                //    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCChiTietDV_CLS").PreviewModel;
                //    rParams["parFromDate"].Value = RptParameters.FromDate;
                //    rParams["parToDate"].Value = RptParameters.ToDate;
                //    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                //    rParams["parFindPatient"].Value = FindPatient;
                //    rParams["parHospitalName"].Value = reportHospitalName;
                //    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                //    break;
                //▲====: #005
                //▼==== #054
                case ReportName.BCChiTietDV_CLS:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCChiTietDV_CLS_New").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▲==== #054
                //▼====: #006
                case ReportName.BCDoanhThuThuocBHYTNgTru:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCDoanhThuThuocBHYT").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parFindPatient"].Value = RptParameters.FindPatient;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▲====: #006
                //▼====: #007
                case ReportName.BCThoiGianBNChoTaiBV:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.QualityManagement.XRptBCThoiGianBNChoTaiBV").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▲====: #007
                ////▼====: #010
                //case ReportName.BCChiTietNhapTuNCC:
                //    ReportModel = null;
                //    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.Accountant.XRpt_BCNhapTuNCC").PreviewModel;
                //    rParams["FromDate"].Value = RptParameters.FromDate;
                //    rParams["ToDate"].Value = RptParameters.ToDate;
                //    rParams["StorageName"].Value = (StoreCbx != null && StoreCbx.Count() > 2) ? StoreCbx[0].swhlName : "";
                //    rParams["StoreID"].Value = (StoreCbx != null && StoreCbx.Count() > 2) ? StoreCbx[0].StoreID : 1;
                //    rParams["DateShow"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                //    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                //    break;
                ////▲====: #010
                //▼====: #011
                case ReportName.BCBNHoanTraBienLai:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCBNHoanTra").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    rParams["parStaffID"].Value = SelectedStaff != null ? (SelectedStaff.StaffID) : 0;
                    break;
                //▲====: #011
                //▼====: #012
                case ReportName.KHTH_TinhHinhKCB:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_TinhHinhHoatDongKhamBenh_TongHop").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                //▲====: #012
                //▼====: #013
                case ReportName.KT_TinhHinhHoatDongDV:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_TinhHinhHoatDongDV").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parIsDetail"].Value = isDetail;
                    break;
                //▲====: #013
                //▼====: #014
                case ReportName.KT_BCBNXuatVienConNoVienPhi:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptInPatientDischargeNotPayAllBill").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : 0;
                    break;
                //▲====: #014
                //▼====: #015
                case ReportName.KT_BCBNTraSau:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCBNTraSau").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▲====: #015
                case ReportName.KHTH_BCCongTacKCB:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptBCCongTacKCB").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    //rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    //rParams["parHospitalName"].Value = reportHospitalName;
                    //rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▼====: #016
                case ReportName.KB_BCCLSDuocChiDinh:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptGroupCLSReqByDoctor").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    break;
                case ReportName.KB_BCBsyChiDinhCLS:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptCLSReqByDoctor").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    break;
                //▲====: #016
                //▼====: #017
                case ReportName.DLS_BCXuatThuocBNKhoaPhong:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRptDLS_BCXuatThuocBNKhoaPhong").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["MoreThreeDay"].Value = MoreThreeDays;

                    //▼====: #027
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : 0;
                    rParams["PatientType"].Value = LoaiBN;
                    rParams["DrugClassID"].Value = RptParameters.SelectedDrugClass != null && RptParameters.SelectedDrugClass.DrugClassID > 0 ? (int)RptParameters.SelectedDrugClass.DrugClassID : 0;
                    //▲====: #027
                    break;
                //▲====: #017
                //▼====: #018
                case ReportName.BC_BNTaiKhamBenhManTinh:
                    //ReportModel = null;
                    //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.BC_BNTaiKhamBenhManTinh").PreviewModel;
                    //rParams["parFromDate"].Value = RptParameters.FromDate;
                    //rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    //rParams["PatientAges"].Value = PatientAges;
                    //break;
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                //▲====: #018
                //▼====: #023
                case ReportName.BC_BenhNhanKhamBenh:
                    //ReportModel = null;
                    //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.BC_BNTaiKhamBenhManTinh").PreviewModel;
                    //rParams["parFromDate"].Value = RptParameters.FromDate;
                    //rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    //rParams["PatientAges"].Value = PatientAges;
                    //break;
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                //▲====: #023
                //▼====: #024
                case ReportName.BC_BNHenTaiKhamBenhDacTrung:
                    //ReportModel = null;
                    //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.BC_BNTaiKhamBenhManTinh").PreviewModel;
                    //rParams["parFromDate"].Value = RptParameters.FromDate;
                    //rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    //rParams["PatientAges"].Value = PatientAges;
                    //break;
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                //▲====: #024
                //▼====: #025
                case ReportName.BC_TGianChoTrongBV:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptBC_TGianChoTrongBV").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    break;
                //▲====: #025
                case ReportName.XRptSoKhamBenh:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoKhamBenh").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoKhamBenhNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.XRptSoThuThuat:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoThuThuat").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoThuThuatNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.XRptSoSieuAm:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoSieuAm").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoSieuAmNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.XRptSoXQuang:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoXQuang").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoXQuangNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.XRptSoDienTim:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoDienTim").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoDienTimNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.XRptSoXetNghiem:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoXetNghiem").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRptSoXetNghiemNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    //rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                //▲====: #018
                //▼====: #021
                case ReportName.BC_NHAP_PTTT_KHOA_PHONG_KHTH:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BaoCaoNhapPTTTKhoaPhong_KHTH").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;  // thêm khoa 25022020
                    rParams["parFindPatient"].Value = FindPatient; // thêm nội trú ngoại trú 25022020
                    if (SelectedLocation != null)
                        rParams["DeptLocationID"].Value = Convert.ToInt32(SelectedLocation.DeptLocationID);
                    break;
                case ReportName.BC_XUAT_VIEN_KHTH:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_ThongKeDsachBNXuatVien_KHTH").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parDeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : 0;
                    break;
                //▲====: #021
                case ReportName.RptPatientDecease:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_PatientDeaease").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    break;
                case ReportName.BCDichVuCLSDangThucHien_PhanTuyen:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCDichVuCLSDangThucHien_PhanTuyen").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                    if (SelectedLocation != null)
                        rParams["DeptLocationID"].Value = Convert.ToInt32(SelectedLocation.DeptLocationID);
                    break;
                case ReportName.BCChiTietDV_CLS_KHTH:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCChiTietDV_CLS_KHTH").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▼====: #20210312 QTD BC Thanh toan qua the
                case ReportName.KT_BC_ThanhToanQuaThe:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCThuTienThe").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                //▲====: #015
                case ReportName.BC_Cong_Suat_Giuong_Benh:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BaoCaoCongSuatGiuongBenh").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : 0;
                    break;
                case ReportName.BC_GiaoBan_Theo_KhoaLS_KHTH:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.BaoCaoGiaoBanTheoKhoaLS_KHTH").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["DeptID"].Value = 0;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.BC_GiaoBan_Phong_KHTH:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XrptBaoCaoGiaoBanPhong_KHTH").PreviewModel;
                    rParams["FromDate"].Value = FromDateTime.DateTime;
                    rParams["ToDate"].Value = ToDateTime.DateTime;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                case ReportName.BC_ChiTietKhamBenh:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;

                case ReportName.BC_HuyDichVu_NgT:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                //▼====: #025
                case ReportName.TEMP79a_BCBENHDACTRUNG:
                    if (CurrentCondition.Value != 2)
                    {
                        MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien + ", Vui lòng chọn xem theo ngày!");
                        return;
                    }
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRpt_Temp79aBCBenhDacTrung").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = "Thời gian từ " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                //▲====: #025

                //▼====: #028
                case ReportName.BC_DoDHST:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRptPhysicalExamination_ByDeptID").PreviewModel;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["DateShow"].Value = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parDeptName"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptName : "Tất cả khoa phòng";
                    rParams["IsWeb"].Value = isWeb;
                    break;
                //▲====: #028

                //▼====: #029
                case ReportName.KTTH_BC_QLKiemDuyetHoSo:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BC_QLKiemDuyetHoSo_KHTH").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parDeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                    break;
                //▲====: #029

                //▼====: #029
                case ReportName.DLS_BCKiemTraLichSuKCB:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_DLS_BCKiemTraLichSuKCB").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parDeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                    break;
                //▲====: #029
                case ReportName.XRptSoNoiSoi:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoNoiSoi").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoNoiSoiNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalCode"].Value = reportHospitalCode;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                case ReportName.XRptSoDoChucNangHoHap:
                    ReportModel = null;
                    //▼==== #052
                    if (Globals.ServerConfigSection.Hospitals.HospitalCode == "96160")
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoDoChucNangHoHap").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XRptSoDoChucNangHoHapNew").PreviewModel;
                        rParams["V_HealthRecordsType"].Value = (RptParameters.V_HealthRecordsType.LookupID != 0) ? RptParameters.V_HealthRecordsType.LookupID : 0;
                    }
                    //▲==== #052
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalCode"].Value = reportHospitalCode;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    //rParams["parIsDetail"].Value = isDetail;
                    break;
                //▼====: #032
                case ReportName.BCDsachDichVuKyThuatXN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XtraReports.BCDsachDichVuKyThuatXN").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parPCLStatus"].Value = PCLStatus;
                    rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parPCLExamTypeID"].Value = (SelectedPCLExamType != null) ? SelectedPCLExamType.PCLExamTypeID : 0;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                //▲====: #032
                //▼====: #033
                case ReportName.BaoCaoLuotXetNghiem:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XtraReports.BaoCaoLuotXetNghiem").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parPCLStatus"].Value = PCLStatus;
                    rParams["parSectionID"].Value = (SelectedPCLSections != null) ? SelectedPCLSections.PCLSectionID : 0;
                    rParams["parPCLExamTypeID"].Value = (SelectedPCLExamType != null) ? SelectedPCLExamType.PCLExamTypeID : 0;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                //▲====: #033
                //▼====: #035
                case ReportName.BaoCaoDoanhThuTheoKhoa:
                    ReportModel = null;

                    if (rdtPrintDetail)
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_TongHopDoanhThuTheoKhoa").PreviewModel;
                    else
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_TongHopDoanhThuTheoKhoa").PreviewModel;

                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parDeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parIsDetail"].Value = rdtPrintDetail;
                    rParams["parDeptName"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptName : "Tất cả khoa phòng";
                    rParams["parHasDischarge"].Value = HasDischarge;
                    break;
                //▲====: #035
                //▼====: #037
                case ReportName.BaoCaoHoanMienTamUngNoiTru:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XtraReports.XRptInPatientPostponementCashAdvance").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                //▲====: #037
                //▼====: #038
                case ReportName.BC_THU_TIEN_VIEN_PHI_NGOAI_TRU:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBC_ThuTienVienPhiNgoaiTru").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffCreate"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffCreate"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["Case"].Value = Case;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0; //--26/01/2021 DatTB Thêm biến lọc theo PT Thanh Toán
                    break;
                case ReportName.BC_HOAN_TIEN_VIEN_PHI_NGOAI_TRU:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBC_HoanTienVienPhiNgoaiTru").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffCreate"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffCreate"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["Case"].Value = Case;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0; //--26/01/2021 DatTB Thêm biến lọc theo PT Thanh Toán
                    break;
                case ReportName.BC_THU_TIEN_TAM_UNG_NOI_TRU:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBC_ThuTienTamUngNoiTru").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffCreate"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffCreate"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["Case"].Value = Case;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0; //--26/01/2021 DatTB Thêm biến lọc theo PT Thanh Toán
                    break;
                case ReportName.BC_HOAN_TIEN_TAM_UNG_NOI_TRU:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.XRptBC_HoanTienTamUngNoiTru").PreviewModel;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["StaffID"].Value = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                    if (SelectedStaff != null && SelectedStaff.FullName != null && SelectedStaff.FullName != "")
                    {
                        rParams["StaffCreate"].Value = SelectedStaff.FullName;
                    }
                    else
                    {
                        rParams["StaffCreate"].Value = eHCMSResources.T0822_G1_TatCa;
                    }
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["Case"].Value = Case;
                    rParams["V_PaymentMode"].Value = RptParameters.V_PaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0; //--26/01/2021 DatTB Thêm biến lọc theo PT Thanh Toán
                    break;
                //▲====: #038                
                //▼====: #040
                case ReportName.BC_BNTreHenNgoaiTru:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRpt_BCBenhTreHenDieuTriNgoaiTru").PreviewModel;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["DateShow"].Value = "Từ ngày " + RptParameters.FromDate.GetValueOrDefault().ToShortDateString() + " đến " + RptParameters.ToDate.GetValueOrDefault().ToShortDateString();
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : Globals.LoggedUserAccount.Staff.FullName;
                    //▼==== #047
                    rParams["OutpatientTreatmentTypeID"].Value = SelectedOutpatientTreatmentType != null ? SelectedOutpatientTreatmentType.OutpatientTreatmentTypeID : 0;
                    //▲==== #047
                    break;
                //▲====: #040            
                //▼==== #042
                case ReportName.BCGiaoBanKhoaKhamBenh:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptBaoCaoGiaoBanKhoaKhamBenh").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                case ReportName.BCChiDinhCLSKhoaKham:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptBaoCaoChiDinhCLSKhoaKham").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalAddress"].Value = reportHospitalAddress;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                //▲==== #042
                case ReportName.BaoCaoDSBenhNhanCapNhatKetQua:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XtraReports.XRptBaoCaoDSBenhNhanCapNhatKetQua").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parFindPatient"].Value = FindPatient;
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                //▼==== #046
                case ReportName.XRpt_PhatHanhTheKCB:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRpt_PhatHanhTheKCB").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["ReportDate"].Value = RptParameters.Show;
                    break;
                case ReportName.XRpt_ThongKeSoLuongThe:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XtraReports.XRpt_ThongKeSoLuongThe").PreviewModel;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲==== #046
                //▼==== #053
                case ReportName.BaoCaoKSNK:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptConsultations.XtraReports.XRpt_BaoCaoKSNK").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                    rParams["parFindInfectionControl"].Value = FindInfectionControl;
                    rParams["parDeptName"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptName : "Tất cả khoa phòng";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    break;
                //▲==== #053
                //▼==== #056
                case ReportName.BC_XuatKP_Thuoc:
                    if (CurStore == null || CurStore.StoreID == -1)
                    {
                        Globals.ShowMessage(eHCMSResources.K0333_G1_ChonKhoXuat, eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                    if (CurInStore == null || CurInStore.StoreID == -1)
                    {
                        Globals.ShowMessage(eHCMSResources.K0330_G1_ChonKhoNhan, eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRpt_BC_XuatKP_Thuoc").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["OutStoreID"].Value = CurStore.StoreID;
                    rParams["InStoreID"].Value = CurInStore.StoreID;
                    rParams["OutStoreName"].Value = CurStore.swhlName;
                    rParams["InStoreName"].Value = CurInStore.swhlName;
                    rParams["V_MedProductType"].Value = (long)AllLookupValues.MedProductType.THUOC;
                    break;
                case ReportName.BC_XuatKP_YCu:
                    if (CurStore == null || CurStore.StoreID == -1)
                    {
                        Globals.ShowMessage(eHCMSResources.K0333_G1_ChonKhoXuat, eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                    if (CurInStore == null || CurInStore.StoreID == -1)
                    {
                        Globals.ShowMessage(eHCMSResources.K0330_G1_ChonKhoNhan, eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRpt_BC_XuatKP_YCu").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["OutStoreID"].Value = CurStore.StoreID;
                    rParams["InStoreID"].Value = CurInStore.StoreID;
                    rParams["OutStoreName"].Value = CurStore.swhlName;
                    rParams["InStoreName"].Value = CurInStore.swhlName;
                    rParams["V_MedProductType"].Value = (long)AllLookupValues.MedProductType.Y_CU;
                    break;
                //▲==== #056
                //▼==== #058
                case ReportName.XRpt_BCTThongKeDTDT:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports.XRpt_BCTThongKeDTDT").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["PatientType"].Value = FindPatient;
                    rParams["V_HIReportStatus"].Value = RptParameters.V_HIReportStatus != null ? (long)RptParameters.V_HIReportStatus.LookupID : 0;
                    break;
                //▲==== #058

                //▼==== #059
                case ReportName.XRpt_AppointmentLab:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptAppointment.XRpt_AppointmentLab").PreviewModel;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["parFromDate"].Value = RptParameters.FromDate;
                    rParams["parToDate"].Value = RptParameters.ToDate;
                    rParams["V_SendSMSStatus"].Value = SelectedStatusSMS != null ? SelectedStatusSMS.LookupID : 0;
                    rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                    rParams["parDepartmentOfHealth"].Value = reportDepartmentOfHealth;
                    rParams["parHospitalName"].Value = reportHospitalName;
                    break;
                //▲==== #059
            }
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        #region Print Member

        public void btnXemIn(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport(_eItem);
            }
        }

        public void btnXemChiTiet(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport(_eItem, true);
            }
        }

        #endregion

        ComboBox cbx_ChooseKho = null;
        public void cbx_ChooseKho_Loaded(object sender, RoutedEventArgs e)
        {
            cbx_ChooseKho = sender as ComboBox;
        }

        private bool GetParameters()
        {
            bool result = true;
            if (RptParameters == null)
            {
                return false;
            }
            if (isAucStaff)
            {
                SelectedStaff = new Staff { StaffID = aucHoldConsultDoctor.StaffID, FullName = aucHoldConsultDoctor.StaffName };
            }

            if (CurrentCondition == null)
            {
                CurrentCondition = new Condition(eHCMSResources.Z0938_G1_TheoQuy, 0);
            }
            if (CurrentCondition.Value == 0)
            {
                RptParameters.Flag = 0;
                RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.Q0486_G1_Quy.ToUpper()) + RptParameters.Quarter.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + RptParameters.Year.ToString();

            }
            else if (CurrentCondition.Value == 1)
            {
                RptParameters.Flag = 1;
                RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G0039_G1_Th.ToUpper()) + RptParameters.Month.ToString() + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam) + RptParameters.Year.ToString();
            }
            else
            {
                RptParameters.Flag = 2;
                if (RptParameters.FromDate == null || RptParameters.ToDate == null)
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0364_G1_ChonNgThCanXemBC), eHCMSResources.G0442_G1_TBao);
                    result = false;
                }
                else
                {
                    if ((RptParameters.FromDate.GetValueOrDefault() > RptParameters.ToDate.GetValueOrDefault() && IsEnabledToDatePicker)
                        || (FromDateTime.DateTime.GetValueOrDefault() > ToDateTime.DateTime.GetValueOrDefault()))
                    {
                        MessageBox.Show(eHCMSResources.A0857_G1_Msg_InfoNgThangKhHopLe2);
                        return false;
                    }
                    if (RptParameters.FromDate.GetValueOrDefault().Date == RptParameters.ToDate.GetValueOrDefault().Date)
                    {
                        RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.N0045_G1_Ng.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy");

                    }
                    else
                    {
                        RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy") + string.Format(" - {0} ", eHCMSResources.K3192_G1_DenNg.ToUpper()) + RptParameters.ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                    }
                }
            }
            return result;
        }

        public bool IsEnabledcbxCondition
        {
            get
            {
                //▼==== #030, #032, #033, #035
                return eItem != ReportName.BAOCAO_THUTIEN_KHAC && eItem != ReportName.REPORT_INPT_NOT_PAY_ALL_BILL && eItem != ReportName.BaoCaoThuTienNgoaiTruTheoBienLai
                    && eItem != ReportName.DsachBNChiDinhThucHienXN && eItem != ReportName.TinhHinhHoatDongCLS && eItem != ReportName.KT_BC_ThanhToanQuaThe
                    && eItem != ReportName.BC_ChiTietKhamBenh && eItem != ReportName.BC_HuyDichVu_NgT && eItem != ReportName.BC_DoDHST && eItem != ReportName.TEMP79a_BCBENHDACTRUNG
                    && eItem != ReportName.DLS_BCXuatThuocBNKhoaPhong && eItem != ReportName.BC_GiaoBan_Theo_KhoaLS_KHTH && eItem != ReportName.KTTH_BC_QLKiemDuyetHoSo
                    && eItem != ReportName.DLS_BCKiemTraLichSuKCB && eItem != ReportName.DLS_BCThongTinDanhMucThuoc && eItem != ReportName.BC_GiaoBan_Phong_KHTH
                    && eItem != ReportName.BangKeBacSiThucHienPT_TT && eItem != ReportName.DS_BNTiepNhanTheoDT && eItem != ReportName.XRptSoNoiSoi && eItem != ReportName.BCTinhHinhCLS_DaThucHien
                    && eItem != ReportName.BCDsachDichVuKyThuatXN && eItem != ReportName.BaoCaoLuotXetNghiem && eItem != ReportName.BaoCaoDoanhThuTheoKhoa
                    //▼==== #038
                    && eItem != ReportName.BC_THU_TIEN_VIEN_PHI_NGOAI_TRU && eItem != ReportName.BC_HOAN_TIEN_VIEN_PHI_NGOAI_TRU
                    && eItem != ReportName.BC_THU_TIEN_TAM_UNG_NOI_TRU && eItem != ReportName.BC_HOAN_TIEN_TAM_UNG_NOI_TRU
                     //▲==== #038
                     //▼==== #040
                     && eItem != ReportName.BC_BNTreHenNgoaiTru
                     && eItem != ReportName.XRptSoDoChucNangHoHap
                     //▲==== #040
                     && eItem != ReportName.XRptSoDoChucNangHoHap
                     && eItem != ReportName.BCDanhSachBNDTNT_KHTH
                     //▲==== #030, #032, #033, #035
                     //▼==== #045
                     && eItem != ReportName.BCDanhSachBenhNhanDTNT
                     //▲==== #045
                     //▼==== #046
                     && eItem != ReportName.XRpt_PhatHanhTheKCB && eItem != ReportName.XRpt_ThongKeSoLuongThe
                     //▲==== #046
                     //▼==== #049
                     && eItem != ReportName.XRpt_GioLamThemBacSi
                     //▲==== #049
                     //▼==== #053
                     && eItem != ReportName.BCThGianTuVanCapToa && eItem != ReportName.BCThGianTuVanChiDinh
                    //▲==== #053
                    //▼==== #057
                    && eItem != ReportName.XRpt_BCThGianCho_Khukham;
                //▲==== #057
            }
        }
        public bool ShowAges { get; set; } = false;

        private int _PatientAges = 60;
        public int PatientAges
        {
            get
            {
                return _PatientAges;
            }
            set
            {
                if (_PatientAges == value)
                {
                    return;
                }
                _PatientAges = value;
                NotifyOfPropertyChange(() => PatientAges);
            }
        }
        //▲====: #018

        public bool IsEnabledFromDatePicker { get; set; } = true;

        public bool IsEnabledToDatePicker { get; set; } = true;
        public bool ShowMoreThreeDays { get; set; } = false;

        private bool _MoreThreeDays = false;
        public bool MoreThreeDays
        {
            get { return _MoreThreeDays; }
            set
            {
                _MoreThreeDays = value;
                NotifyOfPropertyChange(() => MoreThreeDays);
            }
        }

        /*==== #001 ====*/
        private bool _ShowLocation = false;
        public bool ShowLocation
        {
            get { return _ShowLocation; }
            set
            {
                _ShowLocation = value;
                NotifyOfPropertyChange(() => ShowLocation);
            }
        }

        private bool _ShowPCLParams = false;
        public bool ShowPCLParams
        {
            get { return _ShowPCLParams; }
            set
            {
                _ShowPCLParams = value;
                NotifyOfPropertyChange(() => ShowPCLParams);
            }
        }

        private bool _ShowPCLSection = false;
        public bool ShowPCLSection
        {
            get { return _ShowPCLSection; }
            set
            {
                _ShowPCLSection = value;
                NotifyOfPropertyChange(() => ShowPCLSection);
            }
        }

        private ObservableCollection<DeptLocation> _AllLocations;
        public ObservableCollection<DeptLocation> AllLocations
        {
            get { return _AllLocations; }
            set
            {
                _AllLocations = value;
                NotifyOfPropertyChange(() => AllLocations);
            }
        }

        private DeptLocation _SelectedLocation;
        public DeptLocation SelectedLocation
        {
            get { return _SelectedLocation; }
            set
            {
                _SelectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
            }
        }

        private ObservableCollection<PCLResultParamImplementations> _AllPCLResultParams;
        public ObservableCollection<PCLResultParamImplementations> AllPCLResultParams
        {
            get { return _AllPCLResultParams; }
            set
            {
                _AllPCLResultParams = value;
                NotifyOfPropertyChange(() => AllPCLResultParams);
            }
        }

        private PCLResultParamImplementations _SelectedPCLResultParams;
        public PCLResultParamImplementations SelectedPCLResultParams
        {
            get { return _SelectedPCLResultParams; }
            set
            {
                _SelectedPCLResultParams = value;
                NotifyOfPropertyChange(() => SelectedPCLResultParams);
            }
        }

        private ObservableCollection<PCLSection> _AllPCLSections;
        public ObservableCollection<PCLSection> AllPCLSections
        {
            get { return _AllPCLSections; }
            set
            {
                _AllPCLSections = value;
                NotifyOfPropertyChange(() => AllPCLSections);
            }
        }

        private PCLSection _SelectedPCLSections;
        public PCLSection SelectedPCLSections
        {
            get { return _SelectedPCLSections; }
            set
            {
                _SelectedPCLSections = value;
                NotifyOfPropertyChange(() => SelectedPCLSections);
            }
        }

        //▼==== #039
        private bool _ShowPCLExamType = false;
        public bool ShowPCLExamType
        {
            get { return _ShowPCLExamType; }
            set
            {
                _ShowPCLExamType = value;
                NotifyOfPropertyChange(() => ShowPCLExamType);
            }
        }

        private ObservableCollection<PCLExamType> _AllPCLExamTypes;
        public ObservableCollection<PCLExamType> AllPCLExamTypes
        {
            get { return _AllPCLExamTypes; }
            set
            {
                _AllPCLExamTypes = value;
                NotifyOfPropertyChange(() => AllPCLExamTypes);
            }
        }

        private PCLExamType _SelectedPCLExamType;
        public PCLExamType SelectedPCLExamType
        {
            get { return _SelectedPCLExamType; }
            set
            {
                _SelectedPCLExamType = value;
                NotifyOfPropertyChange(() => SelectedPCLExamType);
            }
        }

        private void LoadExamTypes_All()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypes_All(Globals.DispatchCallback((asyncResult) =>
                        {
                            AllPCLExamTypes = contract.EndPCLExamTypes_All(asyncResult).ToObservableCollection();
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void CboPCLExamType_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = AllPCLExamTypes.Where(x => x.PCLExamTypeName.ToLower().Contains(cboContext.SearchText.ToLower())).ToObservableCollection();
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void CboPCLExamType_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedPCLExamType = ((AutoCompleteBox)sender).SelectedItem as PCLExamType;
        }

        //▲==== #039

        //▼==== #041
        private ObservableCollection<DeptLocation> _LocationsInDept;
        public ObservableCollection<DeptLocation> LocationsInDept
        {
            get
            {
                return _LocationsInDept;
            }
            set
            {
                _LocationsInDept = value;
                NotifyOfPropertyChange(() => LocationsInDept);
            }
        }
        //▲==== #041

        private void LoadLocations(long? deptID, long? V_RoomFunction = null)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLocationsByDeptID(deptID, V_RoomFunction, Globals.DispatchCallback((asyncResult) =>
                        {
                            AllLocations = new ObservableCollection<DeptLocation>(contract.EndGetAllLocationsByDeptID(asyncResult));

                            //▼==== #041
                            if (deptID != 0 && AllLocations != null)
                            {
                                LocationsInDept = new ObservableCollection<DeptLocation>(AllLocations);
                            }
                            else
                            {
                                LocationsInDept = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                            LocationsInDept.Insert(0, itemDefault);
                            SelectedLocation = itemDefault;
                            //▲==== #041
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void LoadPCLResultParams(long? PCLExamTypeSubCategoryID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPCLResultParamByCatID(PCLExamTypeSubCategoryID, Globals.DispatchCallback((asyncResult) =>
                        {
                            AllPCLResultParams = contract.EndGetPCLResultParamByCatID(asyncResult);
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void LoadPCLSections_All()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLSections_All(Globals.DispatchCallback((asyncResult) =>
                        {
                            AllPCLSections = contract.EndPCLSections_All(asyncResult).ToObservableCollection();
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void CboPCLSection_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = AllPCLSections.Where(x => x.PCLSectionName.ToLower().Contains(cboContext.SearchText.ToLower())).ToObservableCollection();
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void CboPCLSection_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedPCLSections = ((AutoCompleteBox)sender).SelectedItem as PCLSection;
        }

        public void cboRoom_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = AllLocations.Where(x => x.Location.LocationName.ToLower().Contains(cboContext.SearchText.ToLower())).ToObservableCollection();
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void cboRoom_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedLocation = ((AutoCompleteBox)sender).SelectedItem as DeptLocation;
        }

        public void cboPCLType_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = AllPCLResultParams.Where(x => x.ParamName.ToLower().Contains(cboContext.SearchText.ToLower())).ToObservableCollection();
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void cboPCLType_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedPCLResultParams = ((AutoCompleteBox)sender).SelectedItem as PCLResultParamImplementations;
        }

        /*==== #001 ====*/
        private bool _IsExportVisible = false;
        public bool IsExportVisible
        {
            get
            {
                return _IsExportVisible;
            }
            set
            {
                _IsExportVisible = value;
                NotifyOfPropertyChange(() => IsExportVisible);
            }
        }

        //▼====: #009
        public void btnExportExcel()
        {
            if (GetParameters())
            {
                DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
                RptParameters.reportName = eItem;
                RptParameters.ReportType = ReportType.BAOCAO_TONGHOP_KT;
                string reportHospitalName = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                string reportDepartmentOfHealth = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                string reportHospitalAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
                RptParameters.Show = "";
                RptParameters.FindPatient = FindPatient;
                switch (eItem)
                {
                    case ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT:
                        rParams["Quarter"].Value = RptParameters.Quarter;
                        rParams["Month"].Value = RptParameters.Month;
                        rParams["Year"].Value = RptParameters.Year;
                        rParams["flag"].Value = RptParameters.Flag;
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        rParams["ReportDate"].Value = RptParameters.Show;
                        rParams["ReportTitle"].Value = eHCMSResources.Z1142_G1_BCBNNpXuatKhoa.ToUpper();
                        rParams["Status"].Value = CurrentInPatientDeptStatus != null ? (int)CurrentInPatientDeptStatus.Value : -1;
                        rParams["DeptID"].Value = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                        RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                        RptParameters.Show = "IMPORT_EXPORT_DEPARTMENT";
                        break;
                    case ReportName.KTTH_BC_XUAT_VIEN:
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                        break;
                    case ReportName.KTTH_BC_CHI_DINH_NHAP_VIEN:
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                        break;
                    case ReportName.BangKeThuTamUngNT:
                        RptParameters.StaffID = SelectedStaff != null ? (int)SelectedStaff.StaffID : 0;
                        RptParameters.V_PaymentMode.LookupID = AllPaymentMode != null ? (int)RptParameters.V_PaymentMode.LookupID : 0;
                        break;
                    //▼====: #018
                    case ReportName.BC_BNTaiKhamBenhManTinh:
                        RptParameters.SearchID = PatientAges;
                        break;
                    //▲====: #018
                    //▼====: #023
                    case ReportName.BC_BenhNhanKhamBenh:
                        //RptParameters.SearchID = PatientAges;
                        break;
                    //▲====: #023
                    //▼====: #024
                    case ReportName.BC_BNHenTaiKhamBenhDacTrung:

                        break;
                    //▲====: #024
                    //▼====: #019
                    case ReportName.BC_ThuocYCuDoDangCuoiKy:
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                        break;
                    //▲====: #019
                    //▼====: #020
                    case ReportName.BC_NHAP_PTTT_KHOA_PHONG:
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;  // thêm khoa 25022020
                        RptParameters.DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0;
                        break;
                    //▲====: #020
                    case ReportName.BC_NHAP_PTTT_KHOA_PHONG_KHTH:
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;  // thêm khoa 25022020
                        RptParameters.DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0;
                        break;
                    case ReportName.BC_CONGNO_NOITRU:
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : 0;
                        RptParameters.ReportType = ReportType.BAOCAO_TONGHOP_KT;
                        //rParams["parHospitalAddress"].Value = reportHospitalAddress;
                        //rParams["parHospitalName"].Value = reportHospitalName;
                        //rParams["parStaffName"].Value = (SelectedStaff != null && !string.IsNullOrWhiteSpace(SelectedStaff.FullName)) ? SelectedStaff.FullName : "";
                        //RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                        break;
                    case ReportName.BC_ChiTietKhamBenh:
                        RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                        break;
                    case ReportName.BC_HuyDichVu_NgT:
                        RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                        break;
                    case ReportName.TEMP79a_BCBENHDACTRUNG:
                        if (CurrentCondition.Value != 2)
                        {
                            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien + " Vui lòng chọn xem theo ngày!");
                            return;
                        }
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                        break;
                    case ReportName.REPORT_PATIENT_SETTLEMENT:
                        if (CurrentCondition.Value != 2)
                        {
                            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien + " Vui lòng chọn xem theo ngày!");
                            return;
                        }
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        break;
                    //▼====: #028
                    case ReportName.BC_DoDHST:
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                        RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                        break;
                    //▼====: #028
                    case ReportName.BangKeBacSiThucHienCLS:
                        RptParameters.Settlement = Settlement;
                        break;
                    //▼====: #035
                    case ReportName.BaoCaoDoanhThuTheoKhoa:
                        RptParameters.IsDetail = rdtPrintDetail;
                        break;
                    //▲====: #035
                    case ReportName.BangKeBacSiThucHienPT_TT:
                        RptParameters.FromDate = FromDateTime.DateTime;
                        RptParameters.ToDate = ToDateTime.DateTime;
                        break;
                    //▼==== #041
                    case ReportName.BCThongKeSLHoSoDTNT:
                        if (SelectedLocation == null || SelectedLocation.LID == -1)
                        {
                            Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.P0385_G1_Pg), eHCMSResources.G0442_G1_TBao);
                            return;
                        }
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? (int)DepartmentContent.SelectedItem.DeptID : -1;
                        RptParameters.DeptLocID = SelectedLocation != null ? SelectedLocation.LID : 0;
                        RptParameters.OutpatientTreatmentTypeID = SelectedOutpatientTreatmentType != null ? SelectedOutpatientTreatmentType.OutpatientTreatmentTypeID : 0;
                        break;
                    //▲==== #041
                    //▼====: #044
                    case ReportName.BCDanhSachBNDTNT_KHTH:
                        if (SelectedLocation == null || SelectedLocation.DeptLocationID == -1)
                        {
                            Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.P0385_G1_Pg), eHCMSResources.G0442_G1_TBao);
                            return;
                        }
                        RptParameters.DeptID = DepartmentContent != null && DepartmentContent.SelectedItem != null ? DepartmentContent.SelectedItem.DeptID : -1;
                        RptParameters.DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0;
                        RptParameters.OutpatientTreatmentTypeID = SelectedOutpatientTreatmentType != null ? SelectedOutpatientTreatmentType.OutpatientTreatmentTypeID : 0;
                        break;
                    //▲====: #044
                    //▼====: #045
                    case ReportName.BCDanhSachBenhNhanDTNT:
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        break;
                    //▲====: #045
                    //▼====: #046
                    case ReportName.XRpt_PhatHanhTheKCB:
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        break;
                    //▲====: #045
                    //▼====: #049
                    case ReportName.XRpt_GioLamThemBacSi:
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        break;
                    //▲====: #049
                    //▼==== #051
                    case ReportName.BCThongKeHoSoDTNT:
                        RptParameters.YNOutPtTreatmentCode = CtrYNOutPtTreatmentCode.SelectedIndex;
                        RptParameters.YNOutPtTreatmentType = CtrYNOutPtTreatmentType.SelectedIndex;
                        RptParameters.YNOutPtTreatmentProgram = CtrYNOutPtTreatmentProgram.SelectedIndex;
                        RptParameters.YNOutPtTreatmentFinal = CtrYNOutPtTreatmentFinal.SelectedIndex;
                        break;
                    //▲==== #051
                    //▼==== #053
                    case ReportName.BCThGianTuVanCapToa:
                        RptParameters.PatientType = LoaiBN;
                        break;
                    case ReportName.BCThGianTuVanChiDinh:
                        RptParameters.PatientType = LoaiBN;
                        break;
                    //▲==== #053
                    //▼==== #056
                    case ReportName.BC_DLS_KhamBenhNgoaiTru:
                        if (CurrentCondition.Value != 2)
                        {
                            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien + " Vui lòng chọn xem theo ngày!");
                            return;
                        }
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        break;
                    case ReportName.BC_DLS_CLSNgoaiTru:
                        if (CurrentCondition.Value != 2)
                        {
                            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien + " Vui lòng chọn xem theo ngày!");
                            return;
                        }
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        break;
                    case ReportName.BC_DLS_ThuocNgoaiTru:
                        if (CurrentCondition.Value != 2)
                        {
                            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien + " Vui lòng chọn xem theo ngày!");
                            return;
                        }
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        break;
                    //▲==== #056
                    case ReportName.BaoCaoXNChuaNhapKetQua:
                        if (CurrentCondition.Value != 2)
                        {
                            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien + " Vui lòng chọn xem theo ngày!");
                            return;
                        }
                        rParams["FromDate"].Value = RptParameters.FromDate;
                        rParams["ToDate"].Value = RptParameters.ToDate;
                        rParams["FindPatient"].Value = RptParameters.FindPatient;
                        break;
                    //▼==== #057
                    case ReportName.XRpt_BCThGianCho_Khukham:
                        RptParameters.PatientType = LoaiBN;
                        RptParameters.V_ExaminationProcess = RptParameters.V_ExaminationProcess != null ? RptParameters.V_ExaminationProcess : new Lookup();
                        break;
                        //▲==== #057
                }

                SaveFileDialog objSFD = new SaveFileDialog();
                if (Globals.ServerConfigSection.CommonItems.ApplyNewFuncExportExcel)
                {
                    objSFD = new SaveFileDialog()
                    {
                        DefaultExt = ".xlsx",
                        //Filter = "Excel xls (*.xls)|*.xls",
                        //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                        Filter = "Excel(2003) (.xls)|*.xls|Excel(2010) (.xlsx)|*.xlsx",
                        FilterIndex = 2
                    };
                }
                else
                {
                    objSFD = new SaveFileDialog()
                    {
                        DefaultExt = ".xls",
                        Filter = "Excel xls (*.xls)|*.xls",
                        //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                        FilterIndex = 1
                    };
                }
                if (objSFD.ShowDialog() != true)
                {
                    return;
                }
                ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            }
        }
        //▲====: #009

        private bool _vBaoCaoVienPhiBHYT = false;
        public bool vBaoCaoVienPhiBHYT
        {
            get
            {
                return _vBaoCaoVienPhiBHYT;
            }
            set
            {
                if (_vBaoCaoVienPhiBHYT == value)
                {
                    return;
                }
                _vBaoCaoVienPhiBHYT = value;
                NotifyOfPropertyChange(() => vBaoCaoVienPhiBHYT);
            }
        }
        private int _Case = 1;
        public int Case
        {
            get
            {
                return _Case;
            }
            set
            {
                if (_Case == value)
                {
                    return;
                }
                _Case = value;
                NotifyOfPropertyChange(() => Case);
            }
        }

        //▼====: #002
        private bool _ChonKho = false;
        public bool ChonKho
        {
            get
            {
                return _ChonKho;
            }
            set
            {
                if (_ChonKho == value)
                    return;
                _ChonKho = value;
                NotifyOfPropertyChange(() => ChonKho);
            }
        }

        private long? _StoreType = 0;
        public void GetListStore(long? StoreType)
        {
            _StoreType = StoreType;
            Coroutine.BeginExecute(DoGetStore(StoreType));
        }

        private IEnumerator<IResult> DoGetStore(long? StoreType)
        {
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null, false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        //▼====: #008
        private ObservableCollection<RefStorageWarehouseLocation> GetStoresForDrugDeptExportDetail()
        {
            return new ObservableCollection<RefStorageWarehouseLocation> {
                new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc nội trú", StoreID = (long)AllLookupValues.MedProductType.THUOC },
                new RefStorageWarehouseLocation { swhlName = "Kho lẻ vật tư y tế", StoreID = (long)AllLookupValues.MedProductType.Y_CU },
                new RefStorageWarehouseLocation { swhlName = "Kho lẻ thuốc BHYT ngoại trú", StoreID = Globals.ServerConfigSection.PharmacyElements.HIStorageID }
               };
        }

        public void cboStoreCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurStore != null && CurStore.StoreID == 1)
            {
                WarehouseCollection = GetStoresForDrugDeptExportDetail();
                if (WarehouseCollection != null)
                {
                    OutStore = WarehouseCollection.FirstOrDefault();
                }
            }
            else if (CurStore != null && CurStore.StoreID != Globals.ServerConfigSection.PharmacyElements.HIStorageID)
            {
                WarehouseCollection = Globals.checkStoreWareHouse(0, false, false);
                if (WarehouseCollection != null)
                {
                    OutStore = WarehouseCollection.FirstOrDefault();
                }
            }
        }
        //▲====: #008

        private RefStorageWarehouseLocation _OutStore;
        public RefStorageWarehouseLocation OutStore
        {
            get => _OutStore; set
            {
                _OutStore = value;
                NotifyOfPropertyChange(() => OutStore);
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _WarehouseCollection;
        public ObservableCollection<RefStorageWarehouseLocation> WarehouseCollection
        {
            get => _WarehouseCollection; set
            {
                _WarehouseCollection = value;
                NotifyOfPropertyChange(() => WarehouseCollection);
            }
        }

        private RefStorageWarehouseLocation _CurStore;
        public RefStorageWarehouseLocation CurStore
        {
            get { return _CurStore; }
            set
            {
                _CurStore = value;
                NotifyOfPropertyChange(() => CurStore);
            }
        }
        //▲====: #002

        //▼====: #027
        private bool _IsShowPatientType = false;
        public bool IsShowPatientType
        {
            get
            {
                return _IsShowPatientType;
            }
            set
            {
                if (_IsShowPatientType == value)
                    return;
                _IsShowPatientType = value;
                NotifyOfPropertyChange(() => IsShowPatientType);
            }
        }

        private bool _IsShowFaName = false;
        public bool IsShowFaName
        {
            get
            {
                return _IsShowFaName;
            }
            set
            {
                if (_IsShowFaName == value)
                    return;
                _IsShowFaName = value;
                NotifyOfPropertyChange(() => IsShowFaName);
            }
        }

        private ObservableCollection<DrugClass> _familytherapies;
        public ObservableCollection<DrugClass> FamilyTherapies
        {
            get
            {
                return _familytherapies;
            }
            set
            {
                if (_familytherapies != value)
                {
                    _familytherapies = value;
                    NotifyOfPropertyChange(() => FamilyTherapies);
                }
            }
        }
        private long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        private IEnumerator<IResult> DoGetDrugClassList()
        {
            var paymentTypeTask = new LoadDrugDeptClassListTask(V_MedProductType, false, true);
            yield return paymentTypeTask;
            FamilyTherapies = paymentTypeTask.DrugClassList;
            SetDefaultFamilyTherapies();
            yield break;
        }

        private void SetDefaultFamilyTherapies()
        {
            if (FamilyTherapies != null)
            {
                RptParameters.SelectedDrugClass = FamilyTherapies.FirstOrDefault();
            }
        }

        int LoaiBN = -1;
        public void rdtTatCaLoai_Checked(object sender, RoutedEventArgs e)
        {
            LoaiBN = -1;
        }

        public void rdtCoBHYT_Checked(object sender, RoutedEventArgs e)
        {
            LoaiBN = 1;
        }

        public void rdtKhongBHYT_Checked(object sender, RoutedEventArgs e)
        {
            LoaiBN = 0;
        }
        //▲====: #027

        //▼====: #028
        private bool _IsShowWeb = false;
        public bool IsShowWeb
        {
            get
            {
                return _IsShowWeb;
            }
            set
            {
                if (_IsShowWeb == value)
                    return;
                _IsShowWeb = value;
                NotifyOfPropertyChange(() => IsShowWeb);
            }
        }

        private bool _isWeb = false;
        public bool isWeb
        {
            get
            {
                return _isWeb;
            }
            set
            {
                if (_isWeb == value)
                    return;
                _isWeb = value;
                NotifyOfPropertyChange(() => isWeb);
            }
        }
        //▲====: #028

        //▼====: #036
        private bool _rdtPrint = false;
        public bool rdtPrint
        {
            get
            {
                return _rdtPrint;
            }
            set
            {
                if (_rdtPrint == value)
                    return;
                _rdtPrint = value;
                NotifyOfPropertyChange(() => rdtPrint);
            }
        }

        bool rdtPrintDetail = true;
        public void rdtPrintCT_Checked(object sender, RoutedEventArgs e)
        {
            rdtPrintDetail = true;
        }

        public void rdtPrintTH_Checked(object sender, RoutedEventArgs e)
        {
            rdtPrintDetail = false;
        }
        //▲====: #036

        //▼==== #041
        private bool _ShowLocationSelect;
        public bool ShowLocationSelect
        {
            get
            {
                return _ShowLocationSelect;
            }
            set
            {
                if (_ShowLocationSelect == value)
                    return;
                _ShowLocationSelect = value;
                NotifyOfPropertyChange(() => ShowLocationSelect);
            }
        }

        private bool _IsShowOutPtTreatmentStatus;
        public bool IsShowOutPtTreatmentStatus
        {
            get
            {
                return _IsShowOutPtTreatmentStatus;
            }
            set
            {
                if (_IsShowOutPtTreatmentStatus == value)
                    return;
                _IsShowOutPtTreatmentStatus = value;
                NotifyOfPropertyChange(() => IsShowOutPtTreatmentStatus);
            }
        }

        private bool _ShowOutPtTreatmentType;
        public bool ShowOutPtTreatmentType
        {
            get { return _ShowOutPtTreatmentType; }
            set
            {
                _ShowOutPtTreatmentType = value;
                NotifyOfPropertyChange(() => ShowOutPtTreatmentType);
            }
        }

        private ObservableCollection<Lookup> _OutPtTreatmentStatus;
        public ObservableCollection<Lookup> OutPtTreatmentStatus
        {
            get
            {
                return _OutPtTreatmentStatus;
            }
            set
            {
                if (_OutPtTreatmentStatus != value)
                {
                    _OutPtTreatmentStatus = value;
                    NotifyOfPropertyChange(() => OutPtTreatmentStatus);
                }
            }
        }

        private void SetDefaultOutPtTreatmentStatus()
        {
            if (OutPtTreatmentStatus != null)
            {
                RptParameters.V_OutPtTreatmentStatus = OutPtTreatmentStatus.FirstOrDefault(); //--27/01/2021 DatTB Fix Select default
            }
        }

        private List<OutpatientTreatmentType> _OutpatientTreatmentTypes;
        public List<OutpatientTreatmentType> OutpatientTreatmentTypes
        {
            get { return _OutpatientTreatmentTypes; }
            set
            {
                _OutpatientTreatmentTypes = value;
                NotifyOfPropertyChange(() => OutpatientTreatmentTypes);
            }
        }

        private OutpatientTreatmentType _SelectedOutpatientTreatmentType;
        public OutpatientTreatmentType SelectedOutpatientTreatmentType
        {
            get { return _SelectedOutpatientTreatmentType; }
            set
            {
                _SelectedOutpatientTreatmentType = value;
                NotifyOfPropertyChange(() => SelectedOutpatientTreatmentType);
            }
        }

        ComboBox cboOutPtTreatmentType { get; set; }
        public void cboOutPtTreatmentType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            cboOutPtTreatmentType = sender as ComboBox;
            if (cboOutPtTreatmentType == null)
            {
                return;
            }
            SelectedOutpatientTreatmentType = cboOutPtTreatmentType.SelectedItem as OutpatientTreatmentType;
        }

        private void SetDefaultOutpatientTreatmentType()
        {
            if (OutpatientTreatmentTypes != null)
            {
                SelectedOutpatientTreatmentType = OutpatientTreatmentTypes.FirstOrDefault();
            }
        }

        private void GetAllOutpatientTreatmentType()
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetAllOutpatientTreatmentType(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            OutpatientTreatmentTypes = CurrentContract.EndGetAllOutpatientTreatmentType(asyncResult);

                            var itemDefault = new OutpatientTreatmentType
                            {
                                OutpatientTreatmentTypeID = 0,
                                OutpatientTreatmentName = "--Tất cả--"
                            };
                            OutpatientTreatmentTypes.Insert(0, itemDefault);

                            SetDefaultOutpatientTreatmentType();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        //▲==== #041
        private bool _IsVisibleKSK = false;
        public bool IsVisibleKSK
        {
            get
            {
                return _IsVisibleKSK;
            }
            set
            {
                _IsVisibleKSK = value;
                NotifyOfPropertyChange(() => IsVisibleKSK);
            }
        }

        //▼==== #048
        private bool _ShowHealthRecordsType;
        public bool ShowHealthRecordsType
        {
            get
            {
                return _ShowHealthRecordsType;
            }
            set
            {
                if (_ShowHealthRecordsType == value)
                    return;
                _ShowHealthRecordsType = value;
                NotifyOfPropertyChange(() => ShowHealthRecordsType);
            }
        }

        private ObservableCollection<Lookup> _HealthRecordsTypes;
        public ObservableCollection<Lookup> HealthRecordsTypes
        {
            get
            {
                return _HealthRecordsTypes;
            }
            set
            {
                if (_HealthRecordsTypes != value)
                {
                    _HealthRecordsTypes = value;
                    NotifyOfPropertyChange(() => HealthRecordsTypes);
                }
            }
        }

        private void SetDefaultHealthRecordsType()
        {
            if (HealthRecordsTypes != null)
            {
                RptParameters.V_HealthRecordsType = HealthRecordsTypes.FirstOrDefault();
            }
        }
        //▲==== #048

        //▼==== #051
        private bool _ShowYNOutPtTreatmentCode = false;
        public bool ShowYNOutPtTreatmentCode
        {
            get
            {
                return _ShowYNOutPtTreatmentCode;
            }
            set
            {
                if (_ShowYNOutPtTreatmentCode == value)
                    return;
                _ShowYNOutPtTreatmentCode = value;
                NotifyOfPropertyChange(() => ShowYNOutPtTreatmentCode);
            }
        }

        private bool _ShowYNOutPtTreatmentType = false;
        public bool ShowYNOutPtTreatmentType
        {
            get
            {
                return _ShowYNOutPtTreatmentType;
            }
            set
            {
                if (_ShowYNOutPtTreatmentType == value)
                    return;
                _ShowYNOutPtTreatmentType = value;
                NotifyOfPropertyChange(() => ShowYNOutPtTreatmentType);
            }
        }

        private bool _ShowYNOutPtTreatmentProgram = false;
        public bool ShowYNOutPtTreatmentProgram
        {
            get
            {
                return _ShowYNOutPtTreatmentProgram;
            }
            set
            {
                if (_ShowYNOutPtTreatmentProgram == value)
                    return;
                _ShowYNOutPtTreatmentProgram = value;
                NotifyOfPropertyChange(() => ShowYNOutPtTreatmentProgram);
            }
        }

        private bool _ShowYNOutPtTreatmentFinal = false;
        public bool ShowYNOutPtTreatmentFinal
        {
            get
            {
                return _ShowYNOutPtTreatmentFinal;
            }
            set
            {
                if (_ShowYNOutPtTreatmentFinal == value)
                    return;
                _ShowYNOutPtTreatmentFinal = value;
                NotifyOfPropertyChange(() => ShowYNOutPtTreatmentFinal);
            }
        }

        private ComboBox CtrYNOutPtTreatmentCode;
        private ComboBox CtrYNOutPtTreatmentType;
        private ComboBox CtrYNOutPtTreatmentProgram;
        private ComboBox CtrYNOutPtTreatmentFinal;

        public void cboYNOutPtTreatmentCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrYNOutPtTreatmentCode = sender as ComboBox;
        }
        public void cboYNOutPtTreatmentCode_Loaded(object sender, RoutedEventArgs e)
        {
            CtrYNOutPtTreatmentCode = sender as ComboBox;
        }

        public void cboYNOutPtTreatmentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrYNOutPtTreatmentType = sender as ComboBox;
        }
        public void cboYNOutPtTreatmentType_Loaded(object sender, RoutedEventArgs e)
        {
            CtrYNOutPtTreatmentType = sender as ComboBox;
        }

        public void cboYNOutPtTreatmentProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrYNOutPtTreatmentProgram = sender as ComboBox;
        }
        public void cboYNOutPtTreatmentProgram_Loaded(object sender, RoutedEventArgs e)
        {
            CtrYNOutPtTreatmentProgram = sender as ComboBox;
        }

        public void cboYNOutPtTreatmentFinal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrYNOutPtTreatmentFinal = sender as ComboBox;
        }
        public void cboYNOutPtTreatmentFinal_Loaded(object sender, RoutedEventArgs e)
        {
            CtrYNOutPtTreatmentFinal = sender as ComboBox;
        }
        //▲==== #051
        //▼==== #053
        int FindInfectionControl = -1;
        public void rdtICMRBacteria_Checked(object sender, RoutedEventArgs e)
        {
            FindInfectionControl = 0;
        }

        public void rdtICHosInfection_Checked(object sender, RoutedEventArgs e)
        {
            FindInfectionControl = 1;
        }
        public void rdtICTatCa_Checked(object sender, RoutedEventArgs e)
        {
            FindInfectionControl = -1;
        }
        //▲==== #053
        //▼==== #053
        private bool _ShowPatientClassification;
        public bool ShowPatientClassification
        {
            get { return _ShowPatientClassification; }
            set
            {
                _ShowPatientClassification = value;
                if (_ShowPatientClassification)
                {
                    Coroutine.BeginExecute(DoLoadDataForTheFirstTime());
                }
                NotifyOfPropertyChange(() => ShowPatientClassification);
            }
        }

        private ObservableCollection<PatientClassification> _patientClassifications;

        public ObservableCollection<PatientClassification> PatientClassifications
        {
            get { return _patientClassifications; }
            set
            {
                _patientClassifications = value;
                NotifyOfPropertyChange(() => PatientClassifications);
            }
        }

        private void SetDefaultPatientClassification()
        {
            if (PatientClassifications != null)
            {
                SelectedPatientClassification = PatientClassifications.FirstOrDefault();
            }
        }

        private PatientClassification _SelectedPatientClassification;

        public PatientClassification SelectedPatientClassification
        {
            get { return _SelectedPatientClassification; }
            set
            {
                _SelectedPatientClassification = value;
                NotifyOfPropertyChange(() => SelectedPatientClassification);
            }
        }
        public IEnumerator<IResult> DoLoadDataForTheFirstTime()
        {
            yield return Loader.Show(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0669_G1_DangLayDLieu));

            var patientClassificationTask = new LoadPatientClassificationsTask();
            yield return patientClassificationTask;

            var result = patientClassificationTask.PatientClassifications.Where(X => X.PatientClassID.Equals(Convert.ToInt64(AllLookupValues.PatientClassification.BN_KHONG_BHYT))
                            || X.PatientClassID.Equals(Convert.ToInt64(AllLookupValues.PatientClassification.BN_CO_BHYT))
                            || X.PatientClassID.Equals(Convert.ToInt64(AllLookupValues.PatientClassification.TRA_SAU))).ToList();

            PatientClassifications = new ObservableCollection<PatientClassification>(result);

            var itemDefault = new PatientClassification
            {
                PatientClassID = 0,
                PatientClassName = "--Tất cả--"
            };
            PatientClassifications.Insert(0, itemDefault);

            SetDefaultPatientClassification();

            yield return Loader.Hide();
        }
        //▲==== #053

        //▼==== #056
        private bool _ChonKhoNhan = false;
        public bool ChonKhoNhan
        {
            get
            {
                return _ChonKhoNhan;
            }
            set
            {
                if (_ChonKhoNhan == value)
                    return;
                _ChonKhoNhan = value;
                NotifyOfPropertyChange(() => ChonKhoNhan);
            }
        }
        public void cboInStoreCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurInStore != null && CurInStore.StoreID == 1)
            {
                WarehouseCollection = GetStoresForDrugDeptExportDetail();
                if (WarehouseCollection != null)
                {
                    InStore = WarehouseCollection.FirstOrDefault();
                }
            }
            else if (CurInStore != null && CurInStore.StoreID != Globals.ServerConfigSection.PharmacyElements.HIStorageID)
            {
                WarehouseCollection = Globals.checkStoreWareHouse(0, false, false);
                if (WarehouseCollection != null)
                {
                    InStore = WarehouseCollection.FirstOrDefault();
                }
            }
        }

        public void GetListInStore(long? StoreType)
        {
            _StoreType = StoreType;
            Coroutine.BeginExecute(DoGetInStore(StoreType));
        }

        private IEnumerator<IResult> DoGetInStore(long? StoreType)
        {
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null, false, true);
            yield return paymentTypeTask;
            InStoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private ObservableCollection<RefStorageWarehouseLocation> _InStoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> InStoreCbx
        {
            get
            {
                return _InStoreCbx;
            }
            set
            {
                if (_InStoreCbx != value)
                {
                    _InStoreCbx = value;
                    NotifyOfPropertyChange(() => InStoreCbx);
                }
            }
        }

        private RefStorageWarehouseLocation _CurInStore;
        public RefStorageWarehouseLocation CurInStore
        {
            get => _CurInStore; set
            {
                _CurInStore = value;
                NotifyOfPropertyChange(() => CurInStore);
            }
        }

        private RefStorageWarehouseLocation _InStore;
        public RefStorageWarehouseLocation InStore
        {
            get => _InStore; set
            {
                _InStore = value;
                NotifyOfPropertyChange(() => InStore);
            }
        }
        //▲==== #056

        //▼==== #057
        private bool _ShowExaminationProcess;
        public bool ShowExaminationProcess
        {
            get
            {
                return _ShowExaminationProcess;
            }
            set
            {
                if (_ShowExaminationProcess == value)
                    return;
                _ShowExaminationProcess = value;
                NotifyOfPropertyChange(() => ShowExaminationProcess);
            }
        }

        private ObservableCollection<Lookup> _ExaminationProcess;
        public ObservableCollection<Lookup> ExaminationProcess
        {
            get
            {
                return _ExaminationProcess;
            }
            set
            {
                if (_ExaminationProcess != value)
                {
                    _ExaminationProcess = value;
                    NotifyOfPropertyChange(() => ExaminationProcess);
                }
            }
        }

        private void SetDefaultExaminationProcess()
        {
            if (ExaminationProcess != null)
            {
                RptParameters.V_ExaminationProcess = ExaminationProcess.FirstOrDefault();
            }
        }
        //▲==== #057
        //▼==== #058
        private bool _ShowHIReportFindPatient;
        public bool ShowHIReportFindPatient
        {
            get
            {
                return _ShowHIReportFindPatient;
            }
            set
            {
                if (_ShowHIReportFindPatient == value)
                    return;
                _ShowHIReportFindPatient = value;
                NotifyOfPropertyChange(() => ShowHIReportFindPatient);
            }
        }

        private bool _ShowHIReportStatus;
        public bool ShowHIReportStatus
        {
            get
            {
                return _ShowHIReportStatus;
            }
            set
            {
                if (_ShowHIReportStatus == value)
                    return;
                _ShowHIReportStatus = value;
                NotifyOfPropertyChange(() => ShowHIReportStatus);
            }
        }

        private ObservableCollection<Lookup> _HIReportStatus;
        public ObservableCollection<Lookup> HIReportStatus
        {
            get
            {
                return _HIReportStatus;
            }
            set
            {
                if (_HIReportStatus != value)
                {
                    _HIReportStatus = value;
                    NotifyOfPropertyChange(() => HIReportStatus);
                }
            }
        }

        private void SetDefaultHIReportStatus()
        {
            if (HIReportStatus != null)
            {
                var itemDefault = new Lookup
                {
                    LookupID = 0,
                    ObjectValue = "--Tất cả--"
                };
                HIReportStatus.Insert(0, itemDefault);

                RptParameters.V_HIReportStatus = HIReportStatus.FirstOrDefault();
            }
        }

        private ComboBox CtrFindPatient;

        public void cboFindPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CtrFindPatient = sender as ComboBox;

            if (CtrFindPatient.SelectedIndex == 0)
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.NGOAITRU;
            }
            else if (CtrFindPatient.SelectedIndex == 1)
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.NOITRU;
            }
            else if (CtrFindPatient.SelectedIndex == 2)
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.CAHAI;
            }
            else
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.DTNGOAITRU;
            }
        }
        public void cboFindPatient_Loaded(object sender, RoutedEventArgs e)
        {
            CtrFindPatient = sender as ComboBox;

            if (CtrFindPatient.SelectedIndex == 0)
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.NGOAITRU;
            }
            else if (CtrFindPatient.SelectedIndex == 1)
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.NOITRU;
            }
            else if (CtrFindPatient.SelectedIndex == 2)
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.CAHAI;
            }
            else
            {
                FindPatient = (int)AllLookupValues.PatientFindBy.DTNGOAITRU;
            }
        }

        //private static readonly KeyValuePair<string, int>[] _FindPatients = {
        //    new KeyValuePair<string, int>(eHCMSResources.T3719_G1_Mau20NgTru, (int)AllLookupValues.PatientFindBy.NGOAITRU),
        //    new KeyValuePair<string, int>(eHCMSResources.T3713_G1_NoiTru, (int)AllLookupValues.PatientFindBy.NOITRU),
        //    new KeyValuePair<string, int>(eHCMSResources.T0822_G1_TatCa, (int)AllLookupValues.PatientFindBy.CAHAI),
        //    new KeyValuePair<string, int>(eHCMSResources.Z2949_G1_DieuTriNgoaiTru, (int)AllLookupValues.PatientFindBy.DTNGOAITRU)
        //};

        //public KeyValuePair<string, int>[] FindPatients
        //{
        //    get
        //    {
        //        return _FindPatients;
        //    }
        //}

        //private void SetDefaultFindPatients()
        //{
        //    if (FindPatients != null)
        //    {
        //        FindPatient = FindPatients.FirstOrDefault().Value;
        //    }
        //}
        //▲==== #058

        //▼==== #059
        private bool _ShowSMSStatus;
        public bool ShowSMSStatus
        {
            get
            {
                return _ShowSMSStatus;
            }
            set
            {
                if (_ShowSMSStatus == value)
                    return;
                _ShowSMSStatus = value;
                NotifyOfPropertyChange(() => ShowSMSStatus);
            }
        }

        private ObservableCollection<Lookup> _SMSStatusCollections;
        public ObservableCollection<Lookup> SMSStatusCollections
        {
            get
            {
                return _SMSStatusCollections;
            }
            set
            {
                if (_SMSStatusCollections != value)
                {
                    _SMSStatusCollections = value;
                    NotifyOfPropertyChange(() => SMSStatusCollections);
                }
            }
        }

        private void SetDefaultSMSStatusCollections()
        {
            if (SMSStatusCollections != null)
            {
                SelectedStatusSMS = SMSStatusCollections.FirstOrDefault();
            }
        }

        private Lookup _SelectedStatusSMS;
        public Lookup SelectedStatusSMS
        {
            get
            {
                return _SelectedStatusSMS;
            }
            set
            {
                if (_SelectedStatusSMS == value)
                    return;
                _SelectedStatusSMS = value;
                NotifyOfPropertyChange(() => SelectedStatusSMS);
            }
        }
        //▲==== #059
    }
}
