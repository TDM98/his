using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using System.Windows.Controls;
using Castle.Windsor;
/*
 * 20170103 #001 CMN:   Added StaffDept
 * 20171124 #002 CMN:   Added Daily diagnostic
 * 20181022 #003 TTM:   BM 0003214: Fix lỗi PatientInfo không hiển thị cho khám bệnh nội trú (Chuyển sang cách làm mới comment activeControl).
 * 20181121 #004 TTM:   BM 0005257: Tạo mới màn hình Thông tin chung - nội trú, đổi tên màn hình Thông tin chung => Thông tin chung - Ngoại trú.
 * 20190517 #005 TNHX:  BM 0006878: Create report BCThoiGianBNChoTaiBV
 * 20190531 #006 TTM:   BM 0006684: Xây dựng màn hình cập nhật chẩn đoán ra toa - báo cáo BHYT.
 * 20190623 #007 TTM:   BM 0011797: Xây dựng màn hình kiểm tra lịch sử khám chữa bệnh của bác sĩ.
 * 20190917 #008 TNHX:  Add report for Consultation
 * 20191010 #009 TTM:   BM 0017443: [Kiểm soát nhiễm khuẩn]: Bổ sung màn hình hội chẩn.
 * 20220214 #011 QTD:   BC Bệnh đặc trưng
 * 20220305 #012 QTD:   BC Đo DHST
 * 20220624 #013 QTD:   BC BN trễ hẹn điều trị ngoại trú
 * 20220728 #014 DatTB:
 * + Báo cáo giao ban ngoại trú
 * + Báo cáo chỉ định cận lâm sàng – khoa khám
 * 20220927 #015 DatTB: Báo cáo danh sách bệnh nhân ĐTNT
 * 20221022 #016 DatTB: Báo cáo BN trễ hẹn điều trị ngoại trú: Thêm cột "Ngày tái khám", Thêm bộ lọc "Nhóm bệnh" ĐTNT
 * 20221220 #017 DatTB: Thêm biến xác định mẫu nội bộ, BYT
 * 20230218 #018 DatTB: Thêm Báo cáo thời gian tư vấn cấp toa/chỉ định
 * 20230513 #019 DatTB: Thêm Báo cáo thời gian chờ tại bệnh viện
 * 20230517 #020 DatTB: Khóa các chức năng lưu xóa giấy chuyển tuyến màn hình yêu cầu.
 * 20230621 #021 DatTB: Thêm sổ xét nghiệm
 */

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultationTopMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationTopMenuViewModel : Conductor<object>, IConsultationTopMenu
    //, IHandle<LocationSelected>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public ConsultationTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            Globals.PageName = "";
            Globals.TitleForm = "";
            Globals.EventAggregator.Subscribe(this);

            authorization();

            if (Globals.allContraIndicatorDrugsRelToMedCond == null)
            {
                GetAllContrainIndicatorDrugs();
            }

        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            IsDoctorStaffLogged = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null && Globals.LoggedUserAccount.Staff.StaffCatgID == (long)StaffCatg.Bs;

            bCommonRecs = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtDashboardCommonRecs);
            bConsultationCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtPMRConsultationNew);

            bPrescriptionCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtePrescriptionTab);

            bSummaryCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtePatientSummaryRecord);

            bAppointmentRequestCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPteAppointmentTab);
            mHenCanLamSang = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtHenXetNghiem);


            bPatientPCLRequestCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtPCLRequest);
            bPatientPCLImagingResultsCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPatientPCLImagingResultsCmd);
            bPatientPCLLaboratoryResultsCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPatientPCLLaboratoryResultsCmd);
            bPSRCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mPtePatientSummaryRecord);

            mThongKe_DSBenhNhanDaKham = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mThongKe_DSBenhNhanDaKham);
            mThongKe_DSBacSiKham = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                                 , (int)eConsultation.mThongKe_DSBacSiKham);
            mThongKe_BangKeChiTietKhamBenh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                          , (int)eConsultation.mThongKe_BangKeChiTietKhamBenh);

            mConsultingDiagnosysEditAuth = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mConsultingDiagnosy
                                                   , (int)oConsultationEx.mConsultingDiagnosys_ConsultingEdit, (int)ePermission.mView);
            mConsultingDiagnosysFullOpAuth = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_Full, (int)ePermission.mView);
            if (mConsultingDiagnosysFullOpAuth) mConsultingDiagnosysEditAuth = true;
            gPrevSurgeryListReport = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_PrevSurgeryList, (int)ePermission.mView);
            if (Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_InCompleteFileList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_AppliedList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_OperatedList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_WaitForSurgeryList, (int)ePermission.mView)
                    || Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_DuraGraftList, (int)ePermission.mView))
                gSummarySurgeryListReport = true;
            else
                gSummarySurgeryListReport = false;

            mConsultationsOtherMenu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                     , (int)eConsultation.mConsultationsOtherMenu);
            mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                         , (int)eConsultation.mConsultationsOtherMenu, (int)oConsultationEx.mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime, (int)ePermission.mView);
            mProcedureCmd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                , (int)eConsultation.mProcedureCmd, (int)oConsultationEx.mProcedureEdit, (int)ePermission.mView);
            //▼==== #014
            mBCGiaoBanKhoaKhamBenh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                         , (int)eConsultation.mBCGiaoBanKhoaKhamBenh);
            mBCChiDinhCLSKhoaKham = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                          , (int)eConsultation.mBCChiDinhCLSKhoaKham);
            //▲==== #014

            //▼==== #015
            mBCDanhSachBenhNhanDTNT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                          , (int)eConsultation.mBCDanhSachBenhNhanDTNT);
            //▲==== #015
            //▼==== #018
            mBCThGianTuVanCapToa = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                          , (int)eConsultation.mBCThGianTuVanCapToa);

            mBCThGianTuVanChiDinh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                          , (int)eConsultation.mBCThGianTuVanChiDinh);
            //▲==== #018

            //▼==== #019
            mBCThGianCho_Khukham = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                          , (int)eConsultation.mBCThGianCho_Khukham);
            //▲==== #019

            mSoBHYT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConsultation
                                     , (int)eConsultation.mSoBHYT);
            mLichSu = bSummaryCmd;
            mKhamBenh = bSummaryCmd || bCommonRecs || bConsultationCmd || bPrescriptionCmd || bPSRCmd || mProcedureCmd;
            mCanLamSang = bPatientPCLRequestCmd || bPatientPCLLaboratoryResultsCmd || bPatientPCLImagingResultsCmd;
            mHenCanLamSang = mHenCanLamSang;
            mThongKe = mThongKe_DSBenhNhanDaKham || mThongKe_DSBacSiKham || bSummaryCmd || mThongKe_BangKeChiTietKhamBenh || mSoBHYT
            //▼==== #014
            || mBCChiDinhCLSKhoaKham || mBCGiaoBanKhoaKhamBenh
            //▲==== #014
            //▼==== #015
            || mBCDanhSachBenhNhanDTNT
            //▲==== #015
            //▼==== #018
            || mBCThGianTuVanCapToa || mBCThGianTuVanChiDinh
            //▲==== #018
            //▼==== #019
            || mBCThGianCho_Khukham;
            //▲==== #019
        }

        #region Property
        private bool _bCommonRecs = true;
        private bool _bConsultationCmd = true;
        private bool _bPrescriptionCmd = true;
        private bool _bSummaryCmd = true;
        private bool _bAppointmentRequestCmd = true;
        private bool _bPatientPCLRequestCmd = true;
        private bool _bPatientPCLImagingResultsCmd = true;
        private bool _bPatientPCLLaboratoryResultsCmd = true;
        private bool _bPSRCmd = true;

        private bool _mThongKe_DSBenhNhanDaKham = true;
        private bool _mThongKe_DSBacSiKham = true;
        private bool _mThongKe_BangKeChiTietKhamBenh = true;
        private bool _mThongKe = true;
        private bool _mHenCanLamSang = true;
        private bool _mCanLamSang = true;
        private bool _mKhamBenh = true;
        private bool _mLichSu = true;
        private bool _mProcedureCmd = true;
        //▼==== #014
        private bool _mBCChiDinhCLSKhoaKham = true;
        private bool _mBCGiaoBanKhoaKhamBenh = true;

        public bool mBCChiDinhCLSKhoaKham
        {
            get
            {
                return _mBCChiDinhCLSKhoaKham;
            }
            set
            {
                if (_mBCChiDinhCLSKhoaKham == value)
                    return;
                _mBCChiDinhCLSKhoaKham = value;
                NotifyOfPropertyChange(() => mBCChiDinhCLSKhoaKham);
            }
        }

        public bool mBCGiaoBanKhoaKhamBenh
        {
            get
            {
                return _mBCGiaoBanKhoaKhamBenh;
            }
            set
            {
                if (_mBCGiaoBanKhoaKhamBenh == value)
                    return;
                _mBCGiaoBanKhoaKhamBenh = value;
                NotifyOfPropertyChange(() => mBCGiaoBanKhoaKhamBenh);
            }
        }
        //▲==== #014
        //▼==== #015
        private bool _mBCDanhSachBenhNhanDTNT = true;
        public bool mBCDanhSachBenhNhanDTNT
        {
            get
            {
                return _mBCDanhSachBenhNhanDTNT;
            }
            set
            {
                if (_mBCDanhSachBenhNhanDTNT == value)
                    return;
                _mBCDanhSachBenhNhanDTNT = value;
                NotifyOfPropertyChange(() => mBCDanhSachBenhNhanDTNT);
            }
        }
        //▲==== #015


        //▼==== #016
        private bool _mBCThGianTuVanCapToa = true;
        public bool mBCThGianTuVanCapToa
        {
            get
            {
                return _mBCThGianTuVanCapToa;
            }
            set
            {
                if (_mBCThGianTuVanCapToa == value)
                    return;
                _mBCThGianTuVanCapToa = value;
                NotifyOfPropertyChange(() => mBCThGianTuVanCapToa);
            }
        }

        private bool _mBCThGianTuVanChiDinh = true;
        public bool mBCThGianTuVanChiDinh
        {
            get
            {
                return _mBCThGianTuVanChiDinh;
            }
            set
            {
                if (_mBCThGianTuVanChiDinh == value)
                    return;
                _mBCThGianTuVanChiDinh = value;
                NotifyOfPropertyChange(() => mBCThGianTuVanChiDinh);
            }
        }
        //▲==== #016

        //▼==== #019
        private bool _mBCThGianCho_Khukham = true;
        public bool mBCThGianCho_Khukham
        {
            get
            {
                return _mBCThGianCho_Khukham;
            }
            set
            {
                if (_mBCThGianCho_Khukham == value)
                    return;
                _mBCThGianCho_Khukham = value;
                NotifyOfPropertyChange(() => mBCThGianCho_Khukham);
            }
        }
        //▲==== #019


        public bool bCommonRecs
        {
            get
            {
                return _bCommonRecs;
            }
            set
            {
                if (_bCommonRecs == value)
                    return;
                _bCommonRecs = value;
            }
        }
        public bool bConsultationCmd
        {
            get
            {
                return _bConsultationCmd;
            }
            set
            {
                if (_bConsultationCmd == value)
                    return;
                _bConsultationCmd = value;
                NotifyOfPropertyChange(() => bConsultationCmd_InPt);
            }
        }
        public bool bPrescriptionCmd
        {
            get
            {
                return _bPrescriptionCmd;
            }
            set
            {
                if (_bPrescriptionCmd == value)
                    return;
                _bPrescriptionCmd = value;
            }
        }
        public bool bSummaryCmd
        {
            get
            {
                return _bSummaryCmd;
            }
            set
            {
                if (_bSummaryCmd == value)
                    return;
                _bSummaryCmd = value;
            }
        }
        public bool bAppointmentRequestCmd
        {
            get
            {
                return _bAppointmentRequestCmd;
            }
            set
            {
                if (_bAppointmentRequestCmd == value)
                    return;
                _bAppointmentRequestCmd = value;
            }
        }
        public bool bPatientPCLRequestCmd
        {
            get
            {
                return _bPatientPCLRequestCmd;
            }
            set
            {
                if (_bPatientPCLRequestCmd == value)
                    return;
                _bPatientPCLRequestCmd = value;
            }
        }
        public bool bPatientPCLImagingResultsCmd
        {
            get
            {
                return _bPatientPCLImagingResultsCmd;
            }
            set
            {
                if (_bPatientPCLImagingResultsCmd == value)
                    return;
                _bPatientPCLImagingResultsCmd = value;
            }
        }
        public bool bPatientPCLLaboratoryResultsCmd
        {
            get
            {
                return _bPatientPCLLaboratoryResultsCmd;
            }
            set
            {
                if (_bPatientPCLLaboratoryResultsCmd == value)
                    return;
                _bPatientPCLLaboratoryResultsCmd = value;
            }
        }
        public bool bPSRCmd
        {
            get
            {
                return _bPSRCmd;
            }
            set
            {
                if (_bPSRCmd == value)
                    return;
                _bPSRCmd = value;
            }
        }

        public bool mThongKe_DSBenhNhanDaKham
        {
            get
            {
                return _mThongKe_DSBenhNhanDaKham
                ;
            }
            set
            {
                if (_mThongKe_DSBenhNhanDaKham
                 == value)
                    return;
                _mThongKe_DSBenhNhanDaKham
                 = value;
                NotifyOfPropertyChange(() => mThongKe_DSBenhNhanDaKham
                );
            }
        }
        public bool mThongKe_DSBacSiKham
        {
            get
            {
                return _mThongKe_DSBacSiKham;
            }
            set
            {
                if (_mThongKe_DSBacSiKham == value)
                    return;
                _mThongKe_DSBacSiKham = value;
                NotifyOfPropertyChange(() => mThongKe_DSBacSiKham);
            }
        }

        public bool mThongKe_BangKeChiTietKhamBenh
        {
            get
            {
                return _mThongKe_BangKeChiTietKhamBenh;
            }
            set
            {
                if (_mThongKe_BangKeChiTietKhamBenh == value)
                    return;
                _mThongKe_BangKeChiTietKhamBenh = value;
                NotifyOfPropertyChange(() => mThongKe_BangKeChiTietKhamBenh);
            }
        }

        public bool mThongKe
        {
            get
            {
                return _mThongKe;
            }
            set
            {
                if (_mThongKe == value)
                    return;
                _mThongKe = value;
                NotifyOfPropertyChange(() => mThongKe);
            }
        }


        public bool mHenCanLamSang
        {
            get
            {
                return _mHenCanLamSang;
            }
            set
            {
                if (_mHenCanLamSang == value)
                    return;
                _mHenCanLamSang = value;
                NotifyOfPropertyChange(() => mHenCanLamSang);
            }
        }

        public bool mCanLamSang
        {
            get
            {
                return _mCanLamSang;
            }
            set
            {
                if (_mCanLamSang == value)
                    return;
                _mCanLamSang = value;
                NotifyOfPropertyChange(() => mCanLamSang);
            }
        }

        public bool mKhamBenh
        {
            get
            {
                return _mKhamBenh;
            }
            set
            {
                if (_mKhamBenh == value)
                    return;
                _mKhamBenh = value;
                NotifyOfPropertyChange(() => mKhamBenh);
            }
        }

        public bool mLichSu
        {
            get
            {
                return _mLichSu;
            }
            set
            {
                if (_mLichSu == value)
                    return;
                _mLichSu = value;
                NotifyOfPropertyChange(() => mLichSu);
            }
        }

        public bool mProcedureCmd
        {
            get
            {
                return _mProcedureCmd;
            }
            set
            {
                if (_mProcedureCmd == value)
                    return;
                _mProcedureCmd = value;
                NotifyOfPropertyChange(() => mProcedureCmd);
            }
        }

        private bool _VisibleDailyDiagnostic = Globals.ServerConfigSection.CommonItems.IsUseDailyDiagnostic;
        public bool VisibleDailyDiagnostic
        {
            get
            {
                return _VisibleDailyDiagnostic;
            }
            set
            {
                _VisibleDailyDiagnostic = value;
                NotifyOfPropertyChange(() => VisibleDailyDiagnostic);
            }
        }

        private bool _mConsultingDiagnosysEditAuth = true;
        public bool mConsultingDiagnosysEditAuth
        {
            get
            {
                return _mConsultingDiagnosysEditAuth;
            }
            set
            {
                if (_mConsultingDiagnosysEditAuth == value) return;
                _mConsultingDiagnosysEditAuth = value;
                NotifyOfPropertyChange(() => mConsultingDiagnosysEditAuth);
            }
        }
        private bool _mConsultingDiagnosysFullOpAuth = true;

        public bool mConsultingDiagnosysFullOpAuth
        {
            get
            {
                return _mConsultingDiagnosysFullOpAuth;
            }
            set
            {
                if (_mConsultingDiagnosysFullOpAuth == value) return;
                _mConsultingDiagnosysFullOpAuth = value;
                NotifyOfPropertyChange(() => mConsultingDiagnosysFullOpAuth);
            }
        }

        private bool _mConsultationsOtherMenu = true;
        public bool mConsultationsOtherMenu
        {
            get
            {
                return _mConsultationsOtherMenu;
            }
            set
            {
                if (_mConsultationsOtherMenu == value) return;
                _mConsultationsOtherMenu = value;
                NotifyOfPropertyChange(() => mConsultationsOtherMenu);
            }
        }

        private bool _mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime = true;
        public bool mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime
        {
            get
            {
                return _mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime;
            }
            set
            {
                if (_mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime == value) return;
                _mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime = value;
                NotifyOfPropertyChange(() => mEditDiagAndPrescriptionWithoutChangeStaffIDAndDatetime);
            }
        }
        private bool _mSoBHYT = true;
        public bool mSoBHYT
        {
            get
            {
                return _mSoBHYT;
            }
            set
            {
                if (_mSoBHYT == value) return;
                _mSoBHYT = value;
                NotifyOfPropertyChange(() => mSoBHYT);
            }
        }
        #endregion

        public void GetAllContrainIndicatorDrugs()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllContrainIndicatorDrugs(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllContrainIndicatorDrugs(asyncResult);
                            if (results != null)
                            {
                                if (Globals.allContraIndicatorDrugsRelToMedCond == null)
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
                                }
                                else
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond.Clear();
                                }
                                foreach (var p in results)
                                {
                                    Globals.allContraIndicatorDrugsRelToMedCond.Add(p);
                                }
                                NotifyOfPropertyChange(() => Globals.allContraIndicatorDrugsRelToMedCond);
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

        //public void AppointmentRequestCmd(object source)
        //{
        //    SetHyperlinkSelectedStyle(source as Button);
        //    Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_HENCLS_HENCLS;
        //    var Conslt = Globals.GetViewModel<IConsultationModule>();
        //    var apptVm = Globals.GetViewModel<IAppointmentRequest>();

        //    Conslt.MainContent = apptVm;
        //    (Conslt as Conductor<object>).ActivateItem(apptVm);
        //}

        private void PCLRequestCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //KMx: Phân biệt Left Module Active giữa Phiếu YC Hình ảnh và Phiếu YC Xét nghiệm rõ ràng, không để chung (25/05/2014 10:05)
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAU;
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequestImage>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLRequestCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            Globals.TitleForm = string.Format("{0} ", eHCMSResources.P0382_G1_PhYeuCauHA);
            PCLRequestCmd_In(source);

        }

        private void MedServiceRequest_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientMedicalServiceRequest>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void MedServiceRequest(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            Globals.TitleForm = eHCMSResources.Z2290_G1_PhieuYCDichVu;
            MedServiceRequest_In(source);
        }
        private void PCLLaboratoryRequestCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //KMx: Phân biệt Left Module Active giữa Phiếu YC Hình ảnh và Phiếu YC Xét nghiệm rõ ràng, không để chung (25/05/2014 10:05)
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAU;
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequest>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLLaboratoryRequestCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            Globals.TitleForm = eHCMSResources.P0383_G1_PhYeuCauXetNghiem;
            PCLLaboratoryRequestCmd_In(source);
        }

        private void PCLImagingResultsCmd_In(object source, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU)
        {
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && !CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            else if (V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU && !CheckAvailable(AllLookupValues.V_PCLRequestType.NOI_TRU))
            {
                return;
            }
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_KETQUAHINHANH;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLImagingResult>();
            VM.V_RegistrationType = V_RegistrationType;
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
            ////Load OutStandingstask
            //var shell = Globals.GetViewModel<IHome>();
            //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCL_ViewResults_Image_OutstandingTask>();
            //shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
            //(shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
        }
        public void PCLImagingResultsCmd(object source, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU)
        {
            PCLImagingResultsCmd_In(source);
        }

        //PCLExamTargetCmd
        private void PCLExamTargetCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.LeftModuleActive = LeftModuleActive.NONE;

            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<ICommonPCLExamTarget>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTargetCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0479_G1_DanhSachCLSSo;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                PCLExamTargetCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTargetCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        public void PCLImagingExtResultsCmd(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_NGOAIVIEN_HINHANH;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLDeptImagingExtHome>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void PCLLaboratoryResultsCmd_In(object source, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU)
        {
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && !CheckAvailable(AllLookupValues.V_PCLRequestType.NGOAI_TRU))
            {
                return;
            }
            else if (V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU && !CheckAvailable(AllLookupValues.V_PCLRequestType.NOI_TRU))
            {
                return;
            }
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_KETQUAXETNGHIEM;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLLaboratoryResult>();
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }
        public void PCLLaboratoryResultsCmd(object source)
        {
            PCLLaboratoryResultsCmd_In(source);
        }

        private void DiagnosisTreatmentByDoctorStaffIDCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IDiagnosisTreatmentByDoctorStaffID>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                Globals.LeftModuleActive = LeftModuleActive.NONE;
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var VM = Globals.GetViewModel<IDiagnosisTreatmentByDoctorStaffID>();

                Conslt.MainContent = VM;
                (Conslt as Conductor<object>).ActivateItem(VM);
            }
        }

        public void DiagnosisTreatmentByDoctorStaffIDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K2944_G1_DSBNDaKham;
            DiagnosisTreatmentByDoctorStaffIDCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    DiagnosisTreatmentByDoctorStaffIDCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            DiagnosisTreatmentByDoctorStaffIDCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }

        private void AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.NONE;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IAllDiagnosisGroupByDoctorStaffIDDeptLocationID>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K3075_G1_DSTatCaBsiKham;
            AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            AllDiagnosisGroupByDoctorStaffIDDeptLocationIDCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }


        //public void Handle(LocationSelected message)
        //{
        //    if (message != null && message.DeptLocation != null)
        //    {
        //        var regModule = Globals.GetViewModel<IConsultationModule>();
        //        if (message.ItemActivated == null)
        //        {
        //            //không làm gì hết vì chưa chọn phòng khám
        //        }
        //        else
        //        {
        //            regModule.MainContent = message.ItemActivated;
        //            (regModule as Conductor<object>).ActivateItem(message.ItemActivated);
        //        }

        //    }
        //}


        #region "CRUD Khám Bệnh"
        private void SummaryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_THONGTINCHUNG;

            Globals.PageName = Globals.TitleForm;

            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            homeVM.IsExpandOST = true;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IPtDashboardSummary>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var PrescriptionVM = Globals.GetViewModel<IPtDashboardSummary>();
                PrescriptionVM.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                Conslt.MainContent = PrescriptionVM;
                (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
            }
        }

        public void SummaryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2346_G1_ThongTinChungNgoaiTru;

            SummaryCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    SummaryCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            SummaryCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }

        private void CommonRecs_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_TONGQUAT;
            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<ICommonRecs>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var CommonRecs = Globals.GetViewModel<ICommonRecs>();

                Conslt.MainContent = CommonRecs;
                this.ActivateItem(CommonRecs);
            }
        }

        public void CommonRecs(object source)
        {
            Globals.TitleForm = eHCMSResources.G1527_G1_TQuat;

            CommonRecs_In(source);
        }

        private void ConsultationCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN;
            Globals.PageName = Globals.TitleForm;
            Globals.ConsultationIsChildWindow = false;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IConsultations>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var ConsultationVM = Globals.GetViewModel<IConsultations>();
                ConsultationVM.IsShowEditTinhTrangTheChat = true;
                Conslt.MainContent = ConsultationVM;
                (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
            }
        }

        public void ConsultationCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1746_G1_CDoan;
            ConsultationCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    ConsultationCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            ConsultationCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }

        public void Consultation_V2Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1746_G1_CDoan;
            SetHyperlinkSelectedStyle(source as Button);
            //if (Globals.PatientAllDetails.PtRegistrationInfo == null || Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            //{
            //    Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN;
            //    Globals.PageName = Globals.TitleForm;
            //    Globals.ConsultationIsChildWindow = false;
            //    var Conslt = Globals.GetViewModel<IConsultationModule>();
            //    var ConsultationVM = Globals.GetViewModel<IConsultations_V2>();
            //    Conslt.MainContent = ConsultationVM;
            //    (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
            //}
            //else
            //{
            //    Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            //    Globals.PageName = Globals.TitleForm;
            //    var Conslt = Globals.GetViewModel<IConsultationModule>();
            //    var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            //    ConsultationVM.IsDiagnosisOutHospital = true;
            //    //▼====== #003
            //    //ConsultationVM.activeControl();
            //    //▲====== #003
            //    Conslt.MainContent = ConsultationVM;
            //    (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
            //}
        }

        private void PrescriptionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA;
            Globals.PageName = Globals.TitleForm;

            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IePrescriptions>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var PrescriptionVM = Globals.GetViewModel<IePrescriptions>();

                Conslt.MainContent = PrescriptionVM;
                (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
            }

        }

        public void PrescriptionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.R0501_G1_RaToa;
            PrescriptionCmd_In(source);
        }
        private void ConsultationSummaryCmd_InPt_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mContent = Globals.GetViewModel<IConsultationsSummary_InPt>();
            mContent.Title = Globals.TitleForm;
            mModule.MainContent = mContent;
            (mModule as Conductor<object>).ActivateItem(mContent);
        }
        public void ConsultationSummaryCmd_InPt(object source)
        {
            Globals.TitleForm = eHCMSResources.T2128_G1_KBNoiTru;
            if (string.IsNullOrEmpty(Globals.TitleForm))
            {
                ConsultationSummaryCmd_InPt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                //Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                //{
                //    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                //    {
                        ConsultationSummaryCmd_InPt_In(source);
                //    }
                //});
            }
        }
        //▼====== #004:
        public void SummaryCmd_InPt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2347_G1_ThongTinChungNoiTru;

            SummaryCmd_In_Pt(source);
        }

        private void SummaryCmd_In_Pt(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_THONGTINCHUNG_NOITRU;
            Globals.PageName = Globals.TitleForm;
            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var PrescriptionVM = Globals.GetViewModel<IPtDashboardSummary>();
                PrescriptionVM.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
                Conslt.MainContent = PrescriptionVM;
                (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
            }
        }
        //▲====== #004
        private void Consultation_InPt_Cmd_OutDept_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;


            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            //▼====== #003
            //ConsultationVM.activeControl();
            //▲====== #003
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);

        }

        public void Consultation_InPt_OutDept_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1777_G1_CDoanXKhoa;
            Consultation_InPt_Cmd_OutDept_In(source);
        }
        /*▼====: #002*/
        public void Consultation_InPt_Daily_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2157_G1_ChanDoanHangNgay;

            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            ConsultationVM.IsDailyDiagnosis = true;
            //▼====== #003
            //ConsultationVM.activeControl();
            //▲====== #003
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
        }
        /*▲====: #002*/
        public void ProcedureEditCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2157_G1_ChanDoanHangNgay;
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            ConsultationVM.IsDailyDiagnosis = true;
            ConsultationVM.IsProcedureEdit = true;
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
        }
        public void PhysicalTherapyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2775_G1_LieuTrinhDieuTri;
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            ConsultationVM.IsDailyDiagnosis = true;
            ConsultationVM.IsPhysicalTherapy = true;
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);
        }
        public void InfectionCaseCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2860_G2_DieuTriNhiemKhuan;
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mViewContent = Globals.GetViewModel<IInfectionCaseCollection>();
            mModule.MainContent = mViewContent;
            (mModule as Conductor<object>).ActivateItem(mViewContent);
        }
        public void InfectionCaseStatisticsCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2860_G2_DanhSachDotDieuTri;
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mViewContent = Globals.GetViewModel<IInfectionCaseStatistics>();
            mModule.MainContent = mViewContent;
            (mModule as Conductor<object>).ActivateItem(mViewContent);
        }
        private void Consultation_InPt_Cmd_OutHos_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU;
            Globals.PageName = Globals.TitleForm;


            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultationVM = Globals.GetViewModel<IConsultations_InPt>();
            ConsultationVM.IsDiagnosisOutHospital = true;
            //▼====== #003
            //ConsultationVM.activeControl();
            //▲====== #003
            Conslt.MainContent = ConsultationVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultationVM);

        }

        public void Consultation_InPt_OutHos_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1771_G1_CDoanNhapXV;
            Consultation_InPt_Cmd_OutHos_In(source);
        }

        private void InPrescriptionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA_XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var PrescriptionVM = Globals.GetViewModel<IeInPrescriptions>();

            Conslt.MainContent = PrescriptionVM;
            (Conslt as Conductor<object>).ActivateItem(PrescriptionVM);
        }

        public void InPrescriptionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1444_G1_ToaXV;
            InPrescriptionCmd_In(source);
        }

        private void InPatientDischargeCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var consultModule = Globals.GetViewModel<IConsultationModule>();
            var InPtDischargeNewVM = Globals.GetViewModel<IDischargeNew>();

            InPtDischargeNewVM.IsConsultation = true;

            InPtDischargeNewVM.InitView(true);

            consultModule.MainContent = InPtDischargeNewVM;
            (consultModule as Conductor<object>).ActivateItem(InPtDischargeNewVM);
        }

        public void InPatientDischargeCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0482_G1_XuatVienBenhNhan;
            InPatientDischargeCmd_In(source);
        }

        private void PatientInstructionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA_XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var medicalInstructionVM = Globals.GetViewModel<IInPatientInstruction>();

            Conslt.MainContent = medicalInstructionVM;
            (Conslt as Conductor<object>).ActivateItem(medicalInstructionVM);
        }

        public void PatientInstructionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1444_G1_ToaXV;
            PatientInstructionCmd_In(source);
        }

        private void SurgeryBookingCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_RATOA_XUATVIEN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var medicalInstructionVM = Globals.GetViewModel<ISurgeryBooking>();

            Conslt.MainContent = medicalInstructionVM;
            (Conslt as Conductor<object>).ActivateItem(medicalInstructionVM);

        }
        public void SurgeryBookingCmd(object source)
        {
            Globals.TitleForm = "LẬP CA PHẪU THUẬT";
            SurgeryBookingCmd_In(source);
        }

        public void SurgicalReportInPtCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2145_G1_TuongTrinhPhauThuat;
            SurgicalReportInPtCmd_In(source);
        }

        private void SurgicalReportInPtCmd_In(object source)
        {
            //SetHyperlinkSelectedStyle(source as Button);
            //Globals.PageName = Globals.TitleForm;
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_THONGTINCHUNG_NOITRU;
            //var Conslt = Globals.GetViewModel<IConsultationModule>();
            //var medicalInstructionVM = Globals.GetViewModel<ISurgicalReport>();
            //medicalInstructionVM.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            //Conslt.MainContent = medicalInstructionVM;
            //(Conslt as Conductor<object>).ActivateItem(medicalInstructionVM);
        }

        public void SurgicalReportCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2145_G1_TuongTrinhPhauThuat;
            SurgicalReportCmd_In(source);
        }

        private void SurgicalReportCmd_In(object source)
        {
            //SetHyperlinkSelectedStyle(source as Button);
            //Globals.PageName = Globals.TitleForm;
            //Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_THONGTINCHUNG;
            //var Conslt = Globals.GetViewModel<IConsultationModule>();
            //var medicalInstructionVM = Globals.GetViewModel<ISurgicalReport>();
            //medicalInstructionVM.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            //Conslt.MainContent = medicalInstructionVM;
            //(Conslt as Conductor<object>).ActivateItem(medicalInstructionVM);
        }

        private void PatientSummaryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_LSBENHAN;
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var apptVm = Globals.GetViewModel<IPatientTreeForm>();

            Conslt.MainContent = apptVm;
            (Conslt as Conductor<object>).ActivateItem(apptVm);
        }

        public void PatientSummaryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2588_G1_LSuBA;
            PatientSummaryCmd_In(source);
        }
        #endregion


        #region History Khám Bệnh


        private void ConsultRoomDetailCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            Globals.LeftModuleActive = LeftModuleActive.NONE;

            Globals.PageName = Globals.TitleForm;

            var homeVM = Globals.GetViewModel<IHome>();
            //homeVM.FindRegistrationCmdVisibility = false;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var ConsultRoomDetailVM = Globals.GetViewModel<IConsultRoomDetail>();
            ConsultRoomDetailVM.LoadRefDept(V_DeptTypeOperation.KhoaNgoaiTru);
            Conslt.MainContent = ConsultRoomDetailVM;
            (Conslt as Conductor<object>).ActivateItem(ConsultRoomDetailVM);

        }

        public void ConsultRoomDetailCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1035_G1_BKeCTietKB;
            ConsultRoomDetailCmd_In(source);
        }
        #endregion

        #region Hẹn Cận Lâm Sàng
        private void PCLRequestHenCLSCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_HENCLS_HENCLS;
            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IPatientAppointments_PCL_InConsultation>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var Conslt = Globals.GetViewModel<IConsultationModule>();
                var VM = Globals.GetViewModel<IPatientAppointments_PCL_InConsultation>();
                VM.IsCreateApptFromConsultation = true;
                Conslt.MainContent = VM;
                (Conslt as Conductor<object>).ActivateItem(VM);
            }

        }

        public void PCLRequestHenCLSCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T1464_G1_HenCLS;
            PCLRequestHenCLSCmd_In(source);

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    PCLRequestHenCLSCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            PCLRequestHenCLSCmd_In(source);
            //            GlobalsNAV.msgb = null;
            //        }
            //    });
            //}
        }
        #endregion


        public void PrescriptionNoteTemplatesCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T3744_G1_MauLoiDan;
            Globals.PageName = Globals.TitleForm;

            SetHyperlinkSelectedStyle(source as Button);
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPrescriptionNoteTemplates>();

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }

        public void ManagePatientDetailsCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_QUANLY_THONGTIN_BN;

            Globals.PageName = Globals.TitleForm;

            //LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IManagePatientDetails>();

            VM.PageTitle = eHCMSResources.G0525_G1_TTinBN;
            VM.RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;

            VM.mNhanBenh_ThongTin_Sua = false;
            VM.mNhanBenh_TheBH_ThemMoi = false;
            VM.mNhanBenh_TheBH_XacNhan = false;
            VM.mNhanBenh_DangKy = false;
            VM.mNhanBenh_TheBH_Sua = false;


            VM.mPatient_TimBN = true;
            VM.mPatient_ThemBN = true;
            VM.mPatient_TimDangKy = false;

            VM.mInfo_CapNhatThongTinBN = true;
            VM.mInfo_XacNhan = false;
            VM.mInfo_XoaThe = false;
            VM.mInfo_XemPhongKham = false;

            Globals.IsAdmission = null;

            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);

            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });

        }
        #region menu
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
            Globals.IsAdmission = true;
        }

        ILeftMenuView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as ILeftMenuView;
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }
        private void SetHyperlinkSelectedStyle(Button lnk)
        {
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
            if (lnk != null)
            {
                lnk.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            }
        }
        #endregion

        #region Cận lâm sàng nội trú
        public void PCLLaboratoryRequestCmd_InPt(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NOI_TRU))
            {
                return;
            }
            Globals.TitleForm = string.Format("{0} - {1}", eHCMSResources.P0383_G1_PhYeuCauXetNghiem, eHCMSResources.T3713_G1_NoiTru);
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM_NT;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequest>();
            VM.IsShowCheckBoxPayAfter = false;
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }
        public void PCLImageRequestCmd_InPt(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NOI_TRU))
            {
                return;
            }
            Globals.TitleForm = string.Format("{0} - {1}", eHCMSResources.P0382_G1_PhYeuCauHA, eHCMSResources.T3713_G1_NoiTru);
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH_NT;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequestImage>();
            VM.IsShowCheckBoxPayAfter = false;
            Conslt.MainContent = VM;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }
        public void PCLImageRequestCmd_OutPtTreatment(object source)
        {
            if (!CheckAvailable(AllLookupValues.V_PCLRequestType.NOI_TRU))
            {
                return;
            }
            Globals.TitleForm = string.Format("{0} - {1}", eHCMSResources.Z2779_G1_DichVuDTNgoaiTru, eHCMSResources.T3713_G1_NoiTru);
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH_NT;
            Globals.PageName = Globals.TitleForm;
            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var VM = Globals.GetViewModel<IPatientPCLRequestImage>();
            Conslt.MainContent = VM;
            VM.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.GeneralSugery;
            (Conslt as Conductor<object>).ActivateItem(VM);
        }
        public void PCLLaboratoryResultsCmd_InPt(object source)
        {
            PCLLaboratoryResultsCmd_In(source, (long)AllLookupValues.RegistrationType.NOI_TRU);
        }
        public void PCLImagingResultsCmd_InPt(object source)
        {
            PCLImagingResultsCmd_In(source, (long)AllLookupValues.RegistrationType.NOI_TRU);
            //do something after completed PCL request InPt
        }
        public bool CheckAvailable(AllLookupValues.V_PCLRequestType PCLRequestType)
        {
            //if (Globals.PatientAllDetails.PtRegistrationInfo == null)
            //{
            //    return true;
            //}
            //if (PCLRequestType == AllLookupValues.V_PCLRequestType.NGOAI_TRU 
            //    && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            //{
            //    MessageBox.Show(eHCMSResources.Z0395_G1_DKKgPhaiNgoaiTru);
            //    return false;
            //}
            //if (PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU 
            //    && Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            //{
            //    MessageBox.Show(eHCMSResources.A0494_G1_Msg_InfoKhTheThaoTac);
            //    return false;
            //}
            return true;
        }
        #endregion
        //==== #001
        private void StaffDeptPresence_In(bool IsUpdateRequiredNumber)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var StaffDeptPresenceVm = Globals.GetViewModel<IStaffPresence>();
            StaffDeptPresenceVm.IsUpdateRequiredNumber = IsUpdateRequiredNumber;
            (module as Conductor<object>).ActivateItem(StaffDeptPresenceVm);
            module.MainContent = StaffDeptPresenceVm;
        }
        public void UpdateRequiredNumberCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSLanguage.eHCMSResources.Z1924_G1_QLyChiTieuNhanSu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StaffDeptPresence_In(true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StaffDeptPresence_In(true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void UpdatePresenceDailyCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSLanguage.eHCMSResources.Z1925_G1_QLyTinhHinhKhoaNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                StaffDeptPresence_In(false);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        StaffDeptPresence_In(false);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TransferForm_In(int V_TransferFormType)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var TransferFromVm = Globals.GetViewModel<IPaperReferalFull>();
            TransferFromVm.V_TransferFormType = V_TransferFormType;
            //▼==== #020
            TransferFromVm.CanEditTransferForm = false;
            //▲==== #020
            (module as Conductor<object>).ActivateItem(TransferFromVm);
            module.MainContent = TransferFromVm;
        }

        public void TransferToCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = "Giấy chuyển đi";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_Di);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_Di);
                        GlobalsNAV.msgb = null;
                    }
                });
            }

        }

        public void TransferFromCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = "Giấy chuyển đến";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DEN);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DEN);
                        GlobalsNAV.msgb = null;
                    }
                });
            }

        }
        public void TransferPCLCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = "Giấy chuyển đi làm cận lâm sàng";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferForm_In((int)AllLookupValues.V_TransferFormType.CHUYEN_DI_CLS);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //==== #001

        private void HoiChan_In(byte aViewCase = 0)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<IConsultingDiagnosys>();
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }
        public void HoiChanCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2776_G1_KhamHoiChan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoiChan_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoiChan_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void HoiChanNoiCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2777_G1_KhamHoiChanNoi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoiChan_In(1);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoiChan_In(1);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void SurgeryCmd_In(bool IsWaitOnly = false)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<IConsultingDiagnosysReport>();
            Vm.IsWaitOnly = IsWaitOnly;
            Vm.TitleForm = Globals.PageName;
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }
        public void SurgeryCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2143_G1_BCBNChiDinhMo.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SurgeryCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SurgeryCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void WaitForSurgeryCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2182_G1_DanhSachBNDuKienPT.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SurgeryCmd_In(true);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SurgeryCmd_In(true);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportSurgeryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var Conslt = Globals.GetViewModel<IConsultationModule>();
            var vm = Globals.GetViewModel<IReportSurgeryDept>();

            Conslt.MainContent = vm;
            (Conslt as Conductor<object>).ActivateItem(vm);

        }
        public void ReportSurgeryCmd(object source)
        {
            Globals.TitleForm = "THỐNG KÊ BỆNH NHÂN KHOA PHẪU THUẬT";
            ReportSurgeryCmd_In(source);
        }

        private bool _EnableTestFunction = Globals.ServerConfigSection.CommonItems.EnableTestFunction;
        public bool EnableTestFunction
        {
            get
            {
                return _EnableTestFunction;
            }
            set
            {
                _EnableTestFunction = value;
                NotifyOfPropertyChange(() => EnableTestFunction);
            }
        }

        private bool _PrevSurgeryListReport = true;
        public bool gPrevSurgeryListReport
        {
            get
            {
                return _PrevSurgeryListReport;
            }
            set
            {
                _PrevSurgeryListReport = value;
                NotifyOfPropertyChange(() => gPrevSurgeryListReport);
            }
        }

        private bool _SummarySurgeryListReport = true;
        public bool gSummarySurgeryListReport
        {
            get
            {
                return _SummarySurgeryListReport;
            }
            set
            {
                _SummarySurgeryListReport = value;
                NotifyOfPropertyChange(() => gSummarySurgeryListReport);
            }
        }

        private void TreatmentRegimenCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<ITreatmentRegimen>();
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }
        public void TreatmentRegimenCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2264_G1_PhacDoDieuTri;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TreatmentRegimenCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TreatmentRegimenCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ConsultationSummaryCmd_In(object source)
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            Globals.PageName = Globals.TitleForm;
            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            homeVM.IsExpandOST = true;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mView = Globals.GetViewModel<IConsultationsSummary>();
            mView.IsShowEditTinhTrangTheChat = false;
            mModule.MainContent = mView;
            (mModule as Conductor<object>).ActivateItem(mView);
        }
        public void ConsultationSummaryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2119_G1_KB;
            ConsultationSummaryCmd_In(source);
        }
        private void ConsultationSummary_V2Cmd_In(object source, bool IsOutPtTreatmentProgram = false)
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            Globals.PageName = Globals.TitleForm;
            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            homeVM.IsExpandOST = true;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mView = Globals.GetViewModel<IConsultationsSummary_V2>();
            mView.IsShowEditTinhTrangTheChat = false;
            mModule.MainContent = mView;
            mView.IsOutPtTreatmentProgram = IsOutPtTreatmentProgram;
            (mModule as Conductor<object>).ActivateItem(mView);
        }
        public void ConsultationSummary_V2Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2119_G1_KB;
            ConsultationSummary_V2Cmd_In(source);
        }
        public void OutPtTreatmentProgramCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2119_G1_KB;
            ConsultationSummary_V2Cmd_In(source, true);
        }
        private bool _IsDoctorStaffLogged = true;
        public bool IsDoctorStaffLogged
        {
            get => _IsDoctorStaffLogged; set
            {
                _IsDoctorStaffLogged = value;
                NotifyOfPropertyChange(() => IsDoctorStaffLogged);
            }
        }

        private void ShortHandDictionaryMappingCmd_In(bool IsWaitOnly = false)
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<IShortHandDictionaryMapping>();
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }
        public void ShortHandDictionaryMappingCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2403_G1_TuDienVietTat;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ShortHandDictionaryMappingCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ShortHandDictionaryMappingCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Procedure_Cmd_In(object source, bool IsOutPtTreatmentProgram = false)
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;
            SetHyperlinkSelectedStyle(source as Button);
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            Globals.PageName = Globals.TitleForm;
            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            homeVM.IsExpandOST = true;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mView = Globals.GetViewModel<IConsultationsSummary_V2>();
            mView.IsShowEditTinhTrangTheChat = false;
            mView.IsSearchOnlyProcedure = true;
            mModule.MainContent = mView;
            mView.IsOutPtTreatmentProgram = IsOutPtTreatmentProgram;
            (mModule as Conductor<object>).ActivateItem(mView);
        }
        public void Procedure_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K0746_G1_TThuat;
            Procedure_Cmd_In(source);
        }
        private void MedicalExaminationResultCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<IMedicalExaminationResult>();
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }
        public void MedicalExaminationResultCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.Z2670_G1_KhamSucKhoe;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                MedicalExaminationResultCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        MedicalExaminationResultCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BaoCaoGiaoBanCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2437_G2_BaoCaoGiaoBan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoGiaoBanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoGiaoBanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BaoCaoGiaoBanCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BAO_CAO_GIAO_BAN;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }


        public void BaoCaoThuThuatNgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2597_G1_BCThuThuatNgoaiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoThuThuatNgoaiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoThuThuatNgoaiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BaoCaoThuThuatNgoaiTruCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_NHAP_PTTT_KHOA_PHONG;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mXemChiTiet = false;
            //reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.ShowLocation = true;

            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BaoCaoPT_TTChuaThucHien_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2655_G1_BCPT_TTChuaThucHien;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoPT_TTChuaThucHien_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoPT_TTChuaThucHien_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BaoCaoPT_TTChuaThucHien_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BaoCaoPT_TTChuaThucHien;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        //▼====: #005
        public void BCThoigianBNChoTaiBV_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2678_G1_BCThoiGianBNChoTaiBV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThoigianBNChoTaiBV_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThoigianBNChoTaiBV_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCThoigianBNChoTaiBV_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCThoiGianBNChoTaiBV;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #005

        //▼====== #006
        private void btnUpdateDiagAndPrescriptionForBHYT_In(object source)
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;

            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            Globals.PageName = Globals.TitleForm;

            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;

            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mView = Globals.GetViewModel<IConsultationsSummary>();
            mView.IsShowEditTinhTrangTheChat = false;

            mView.IsUpdateWithoutChangeDoctorIDAndDatetime = true;
            mView.CheckVisibleForTabControl();
            mModule.MainContent = mView;
            //(mModule as Conductor<object>).ActivateItem(mView);
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void btnUpdateDiagAndPrescriptionForBHYT(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2685_G1_CapNhatChanDoanVaToaBCBHYT;
            if (string.IsNullOrEmpty(Globals.TitleForm))
            {
                btnUpdateDiagAndPrescriptionForBHYT_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        btnUpdateDiagAndPrescriptionForBHYT_In(source);
                    }
                });
            }
        }
        //▲====== #006

        //▼====== #007
        private void btnCheckDoctorDiagAndPrescription_In(object source)
        {
            Globals.ConsultationIsChildWindow = true;
            Globals.PrescriptionIsChildWindow = false;
            Globals.LeftModuleActive = LeftModuleActive.KHAMBENH_CHANDOAN_RATOA;
            Globals.PageName = Globals.TitleForm;
            var homeVM = Globals.GetViewModel<IHome>();
            homeVM.FindRegistrationCmdVisibility = true;
            homeVM.IsExpandOST = true;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mView = Globals.GetViewModel<ICheckDoctorDiagAndPrescription>();
            mModule.MainContent = mView;
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void btnCheckDoctorDiagAndPrescription(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2744_G1_KiemTraLichSuKCB;
            if (string.IsNullOrEmpty(Globals.TitleForm))
            {
                btnCheckDoctorDiagAndPrescription_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        btnCheckDoctorDiagAndPrescription_In(source);
                    }
                });
            }
        }
        //▲====== #007
        //▼====: #008
        public void BCCLSDuocChiDinh_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2821_G1_BCCLSDuocChiDinh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCCLSDuocChiDinh_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCCLSDuocChiDinh_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCCLSDuocChiDinh_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KB_BCCLSDuocChiDinh;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCBsyChiDinhCLS_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z2821_G1_BCBsyChiDinhCLS;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBsyChiDinhCLS_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBsyChiDinhCLS_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCBsyChiDinhCLS_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.KB_BCBsyChiDinhCLS;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #008
        public bool bConsultationCmd_InPt
        {
            get
            {
                return bConsultationCmd && !Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis;
            }
        }
        //▼===== #009
        private void HoiChanBN_In()
        {
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<IConsultationModule>();
            var Vm = Globals.GetViewModel<IDiagnosysConsultation>();
            (module as Conductor<object>).ActivateItem(Vm);
            module.MainContent = Vm;
        }
        public void HoiChanBNCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.TitleForm = eHCMSResources.R0271_G1_HoiChan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoiChanBN_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoiChanBN_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲===== #009

        //▼===== #010
        public void TinhHinhHoatDongCLS_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2427_G1_TinhHinhHoatDongCLS;
            TinhHinhHoatDongCLS_Cmd_In(source);
        }

        private void TinhHinhHoatDongCLS_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.TinhHinhHoatDongCLS_KK;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲===== #010


        private void btnUpdateDiagConfirmInPT_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mContent = Globals.GetViewModel<IConsultationsSummary_InPt>();
            mContent.IsUpdateDiagConfirmInPT = true;
            mContent.Title = Globals.TitleForm;
            mModule.MainContent = mContent;
            (mModule as Conductor<object>).ActivateItem(mContent);
        }
        public void btnUpdateDiagConfirmInPT(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2961_G1_CNCDXN;
            if (string.IsNullOrEmpty(Globals.TitleForm))
            {
                btnUpdateDiagConfirmInPT_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        btnUpdateDiagConfirmInPT_In(source);
                    }
                });
            }
        }

        public void PerformSmallProcedureAutoCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K0746_G1_TThuat;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                PerformSmallProcedureAutoCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PerformSmallProcedureAutoCmd_In(source);
                        GlobalsNAV.msgb = null;
                        GlobalsNAV.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void PerformSmallProcedureAutoCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var receiveVm = Globals.GetViewModel<IPerformSmallProcedureAuto>();
            regModule.MainContent = receiveVm;
            ((Conductor<object>)regModule).ActivateItem(receiveVm);

            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
        }

        //▼====#010
        public void BCTGianChoTrongBVCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3117_G1_BC_TGianChoTrongBV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCTGianChoTrongBVCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCTGianChoTrongBVCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BCTGianChoTrongBVCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_TGianChoTrongBV;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mXemChiTiet = false;
            //reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;

            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====#010
        public void SoKhamBenh_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3090_G1_SoKhamBenh;
            SoKhamBenh_Cmd_In(source);
        }

        private void SoKhamBenh_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoKhamBenh;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void SoThuThuat_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3091_G1_SoThuThuat;
            SoThuThuat_Cmd_In(source);
        }

        private void SoThuThuat_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoThuThuat;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void SoSieuAm_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3092_G1_SoSieuAm;
            SoSieuAm_Cmd_In(source);
        }

        private void SoSieuAm_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoSieuAm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void SoXquang_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3093_G1_SoXquang;
            SoXquang_Cmd_In(source);
        }

        private void SoXquang_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoXQuang;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void SoDienTim_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3094_G1_SoDienTim;
            SoDienTim_Cmd_In(source);
        }

        private void SoDienTim_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoDienTim;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void SoNoiSoi_Cmd(object source)
        {
            Globals.TitleForm = "Sổ Nội Soi";
            SoNoiSoi_Cmd_In(source);
        }

        private void SoNoiSoi_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoNoiSoi;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void SoDoChucNangHoHap_Cmd(object source)
        {
            Globals.TitleForm = "Sổ Đo Chức Năng Hô Hấp";
            SoDoChucNangHoHap_Cmd_In(source);
        }

        private void SoDoChucNangHoHap_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoDoChucNangHoHap;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        //▼==== #021
        public void SoXetNghiem_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3089_G1_SoXetNghiem;
            SoXetNghiem_Cmd_In(source);
        }

        private void SoXetNghiem_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRptSoXetNghiem;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemChiTiet = false;
            //▼==== #017
            reportVm.ShowHealthRecordsType = true;
            //▲==== #017
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #021

        private void btnDoctorBorrowedAccount_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var mModule = Globals.GetViewModel<IConsultationModule>();
            var mContent = Globals.GetViewModel<IDoctorBorrowedAccount>();
            //mContent.Title = Globals.TitleForm;
            mModule.MainContent = mContent;
            (mModule as Conductor<object>).ActivateItem(mContent);
        }
        public void btnDoctorBorrowedAccount(object source)
        {
            Globals.TitleForm = "Cập nhật Bác sĩ khám chính thức";
            if (string.IsNullOrEmpty(Globals.TitleForm))
            {
                btnDoctorBorrowedAccount_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        btnDoctorBorrowedAccount_In(source);
                    }
                });
            }
        }
        private void BCChiTietKhamBenhCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_ChiTietKhamBenh;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.isAllStaff = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void BCChiTietKhamBenhCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3115_G1_BC_BenhNhanKhamBenh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCChiTietKhamBenhCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCChiTietKhamBenhCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        //▼====#011
        public void BCBenhDacTrung_Cmd()
        {
            Globals.TitleForm = "Báo cáo Bệnh đặc trưng";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBenhDacTrung_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBenhDacTrung_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCBenhDacTrung_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.TEMP79a_BCBENHDACTRUNG;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.ShowMoreThreeDays = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====#011

        //▼====#012
        public void BCDoDHST_Cmd()
        {
            Globals.TitleForm = "Báo cáo Đo dấu hiệu sinh tồn";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDoDHST_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDoDHST_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCDoDHST_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_DoDHST;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mDepartment = true;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.IsShowWeb = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====#012
        //▼====#013
        public void BCBNTreHenNgoaiTru_Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z3241_G1_BCBNTreHenDieuTriNgoaiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCBNTreHenNgoaiTru_Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCBNTreHenNgoaiTru_Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCBNTreHenNgoaiTru_Cmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BC_BNTreHenNgoaiTru;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mXemChiTiet = false;
            reportVm.isAllStaff = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.IsShowWeb = false;
            reportVm.RptParameters.HideFindPatient = false;
            //▼==== #016
            reportVm.ShowOutPtTreatmentType = true;
            //▲==== #016
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====#013


        //▼==== #014
        public void BCGiaoBanKhoaKhamBenhCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3252_G1_BCGiaoBanKhoaKhamBenh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCGiaoBanKhoaKhamBenhCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCGiaoBanKhoaKhamBenhCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCGiaoBanKhoaKhamBenhCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCGiaoBanKhoaKhamBenh;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void BCChiDinhCLSKhoaKhamCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3253_G1_BCChiDinhCLSKhoaKham;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCChiDinhCLSKhoaKhamCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCChiDinhCLSKhoaKhamCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCChiDinhCLSKhoaKhamCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCChiDinhCLSKhoaKham;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #014

        //▼==== #015
        public void BCDanhSachBenhNhanDTNTCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3294_G1_BCDanhSachBenhNhanDTNT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCDanhSachBenhNhanDTNTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCDanhSachBenhNhanDTNTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCDanhSachBenhNhanDTNTCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCDanhSachBenhNhanDTNT;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.mXemIn = false;
            reportVm.isAllStaff = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #015

        //▼==== #018
        public void BCThGianTuVanCapToaCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3307_G1_BCThGianTuVanCapToa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThGianTuVanCapToaCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThGianTuVanCapToaCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCThGianTuVanCapToaCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCThGianTuVanCapToa;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.mXemIn = false;
            reportVm.isAllStaff = false;
            reportVm.IsShowPatientType = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        
        public void BCThGianTuVanChiDinhCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3308_G1_BCThGianTuVanChiDinh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThGianTuVanChiDinhCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThGianTuVanChiDinhCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCThGianTuVanChiDinhCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCThGianTuVanChiDinh;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.mXemIn = false;
            reportVm.isAllStaff = false;
            reportVm.IsShowPatientType = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #018
        //▼==== #019

        public void BCThGianCho_KhukhamCmd()
        {
            Globals.TitleForm = eHCMSResources.Z3320_G1_BCThGianCho_Khukham;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BCThGianCho_KhukhamCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BCThGianCho_KhukhamCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BCThGianCho_KhukhamCmd_In()
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IConsultationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRpt_BCThGianCho_Khukham;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.mXemIn = false;
            reportVm.isAllStaff = false;
            reportVm.IsShowPatientType = true;
            reportVm.ShowExaminationProcess = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #019
    }
}