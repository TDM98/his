using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using System.Windows.Controls;
using Castle.Windsor;
/*
 * 20221012 #001 QTD:  Bỏ set Globals.isAccountCheck cho quyền ĐK admin
 */
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IRegistrationLeftMenu))]
    public class RegistrationLeftMenuViewModel : Conductor<object>, IRegistrationLeftMenu
        , IHandle<LocationSelected>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public RegistrationLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
            //Globals.EventAggregator.Subscribe(this);
            Globals.IsAdmission = null;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();

            Globals.EventAggregator.Subscribe(this);
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }
        public void authorization()
        {
            //KMx: Cho dù là Admin Quầy ĐK cũng không được gán Globals.isAccountCheck = false. Vì Globals.isAccountCheck = false chỉ dành cho admin hệ thống (04/03/2014 10:21).
            //Hiện tại có quá nhiều chỗ sử dụng Globals.isAccountCheck nên không dám bỏ ra.
            //▼====: #001
            //if (Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            //           , (int)ePatient.mAdmin))
            //    Globals.isAccountCheck = false;

            //if (!Globals.isAccountCheck)
            //{
            //    resetAdmin();
            //    return;
            //}

            if (Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient, (int)ePatient.mAdmin) || !Globals.isAccountCheck)
            {
                resetAdmin();
                return;
            }
            //▲====: #001

            //bSearchPatientCmd = Globals.listRefModule[(int)eModules.mPatient]
            //            .lstFunction[(int)ePatient.mPtDashboardCommonRecs].mFunction != null;


            mRegister = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mRegister);
            mProcessPayment = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mProcessPayment);
            mReceivePatient = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mReceivePatient);
            mKiemToan = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mKiemToan);
            mReportPaymentReceipt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mReportPaymentReceipt);
            mRegSummary = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mRegSummary);
            mReCalcHiCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mXacNhanLaiBH);

            mBaoCaoCLSSo = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mBaoCaoCLSSo);
            mBaoCaoDanhSachDangKyBHYT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mBaoCaoDanhSachDangKyBHYT);
            mBaoCaoNhanhKhuKhamBenh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mBaoCaoNhanhKhuKhamBenh);
            mBaoCaoBangKeThuPhiXN = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mBaoCaoBangKeThuPhiXN);
            mBaoCaomBaoCaoBangKeThuPhiXN_HA = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mBaoCaomBaoCaoBangKeThuPhiXN_HA);
            mBaoCaoPhieuNopTien = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mBaoCaoPhieuNopTien);
            mBaoCaoTongHopDoanhThuPK = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mBaoCaoTongHopDoanhThuPK);

            #region Nội Trú

            mReceiveInPatient = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReceiveInPatient);

            mReceiveInPatient_HI = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReceiveInPatient_HI);

            mTemp02 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mTemp02);

            mReCalcHiCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mXacNhanLaiBH);

            mInPatientAdmission = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mInPatientAdmission);

            mInPatientAdmissionManage = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mInPatientAdmissionManage);

            mReCalcBillingInvoice = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReCalcBillingInvoice);

            mInPatientRegister_TV = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mInPatientRegister_TV);

            mInPatientRegister = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mInPatientRegister);

            mHighTechBillingInvoice = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mHighTechBillingInvoice);

            mInPatientProcessPayment = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mInPatientProcessPayment);

            mInPatientSettlement_TV = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mInPatientSettlement_TV);

            mInPatientSettlement = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mInPatientSettlement);

            mChuyenKhoa = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mChuyenKhoa);

            mDischarge = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
                        , (int)ePatient.mDischarge);

            mDischarge_BS = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mDischarge_BS);

            mSuggestAdvanceMoney_TV = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mSuggestAdvanceMoney_TV);

            mSuggestAdvanceMoney = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mSuggestAdvanceMoney);

            mSuggestPayment_TV = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mSuggestPayment_TV);

            mSuggestPayment = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mSuggestPayment);

            mAdvanceMoney = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mAdvanceMoney);

            mInPatientPayment = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mInPatientPayment);

            mRptNotPayCashAdvance = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mRptNotPayCashAdvance);

            mReportAdvanceMoney = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReportAdvanceMoney);

            mReportAdvanceMoneyForBill = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReportAdvanceMoneyForBill);

            mReportRepayAdvanceMoney = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReportRepayAdvanceMoney);

            mReportPatientSettlement = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReportPatientSettlement);

            mReportOutwardMedDeptInflow = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mReportOutwardMedDeptInflow);

            mRptInPtImportExportDept = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mRptInPtImportExportDept);

            mRptPtAreTreated = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mPatient
            , (int)ePatient.mRptPtAreTreated);
            #endregion


            mNgoaiTru = mRegister || mProcessPayment || mReceivePatient;
            mNoiTru = mReceiveInPatient || mReceiveInPatient_HI || mTemp02 || mReCalcHiCmd || mInPatientAdmission || mInPatientAdmissionManage || mReCalcBillingInvoice
                || mInPatientRegister_TV || mInPatientRegister || mHighTechBillingInvoice || mInPatientProcessPayment || mInPatientSettlement_TV || mInPatientSettlement || mChuyenKhoa || mDischarge || mDischarge_BS || mSuggestAdvanceMoney_TV || mSuggestAdvanceMoney || mSuggestPayment_TV || mSuggestPayment
                || mAdvanceMoney || mInPatientPayment || mRptNotPayCashAdvance || mReportAdvanceMoney || mReportAdvanceMoneyForBill || mReportRepayAdvanceMoney || mReportPatientSettlement || mReportOutwardMedDeptInflow || mRptInPtImportExportDept || mRptPtAreTreated;
            mBaoCao = mReportPaymentReceipt || mKiemToan || mRegSummary
                 ||mBaoCaoCLSSo||mBaoCaoDanhSachDangKyBHYT||mBaoCaoNhanhKhuKhamBenh||mBaoCaoBangKeThuPhiXN
                 ||mBaoCaomBaoCaoBangKeThuPhiXN_HA||mBaoCaoTongHopDoanhThuPK;

            //------------------------------------

            mNhanBenhBH_ThongTin_Sua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_ThongTin_Sua, (int)ePermission.mView);
            mNhanBenhBH_TheBH_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_TheBH_ThemMoi, (int)ePermission.mView);
            mNhanBenhBH_TheBH_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_TheBH_XacNhan, (int)ePermission.mView);
            mNhanBenhBH_DangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_DangKy, (int)ePermission.mView);

            mNhanBenhBH_TheBH_Sua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceivePatient,
                                               (int)oRegistrionEx.mNhanBenhBH_TheBH_Sua, (int)ePermission.mView);


            mNhanBenhNoiTru_ThongTin_Sua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceiveInPatient,
                                               (int)oRegistrionEx.mNhanBenhNoiTru_ThongTin_Sua, (int)ePermission.mView);
            mNhanBenhNoiTru_TheBH_ThemMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceiveInPatient,
                                               (int)oRegistrionEx.mNhanBenhNoiTru_TheBH_ThemMoi, (int)ePermission.mView);
            mNhanBenhNoiTru_TheBH_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceiveInPatient,
                                               (int)oRegistrionEx.mNhanBenhNoiTru_TheBH_XacNhan, (int)ePermission.mView);
            mNhanBenhNoiTru_DangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceiveInPatient,
                                               (int)oRegistrionEx.mNhanBenhNoiTru_DangKy, (int)ePermission.mView);

            mNhanBenhNoiTru_TheBH_Sua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mReceiveInPatient,
                                               (int)oRegistrionEx.mNhanBenhNoiTru_TheBH_Sua, (int)ePermission.mView);

            //phan nay nam trong module chung ne
            mNhanBenhBH_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mReceivePatient,
                                             (int)oRegistrionEx.mNhanBenhBH_Patient_TimBN, (int)ePermission.mView);
            mNhanBenhBH_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceivePatient,
                                                 (int)oRegistrionEx.mNhanBenhBH_Patient_ThemBN, (int)ePermission.mView);
            mNhanBenhBH_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceivePatient,
                                                 (int)oRegistrionEx.mNhanBenhBH_Patient_TimDangKy, (int)ePermission.mView);

            mNhanBenhBH_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceivePatient,
                                                 (int)oRegistrionEx.mNhanBenhBH_Info_CapNhatThongTinBN, (int)ePermission.mView);
            mNhanBenhBH_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceivePatient,
                                                 (int)oRegistrionEx.mNhanBenhBH_Info_XacNhan, (int)ePermission.mView);
            mNhanBenhBH_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceivePatient,
                                                 (int)oRegistrionEx.mNhanBenhBH_Info_XoaThe, (int)ePermission.mView);
            mNhanBenhBH_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceivePatient,
                                                 (int)oRegistrionEx.mNhanBenhBH_Info_XemPhongKham, (int)ePermission.mView);

            //phan nay nam trong module chung ne
            mNhanBenhNoiTru_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mReceiveInPatient,
                                             (int)oRegistrionEx.mNhanBenhNoiTru_Patient_TimBN, (int)ePermission.mView);
            mNhanBenhNoiTru_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceiveInPatient,
                                                 (int)oRegistrionEx.mNhanBenhNoiTru_Patient_ThemBN, (int)ePermission.mView);
            mNhanBenhNoiTru_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceiveInPatient,
                                                 (int)oRegistrionEx.mNhanBenhNoiTru_Patient_TimDangKy, (int)ePermission.mView);

            mNhanBenhNoiTru_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceiveInPatient,
                                                 (int)oRegistrionEx.mNhanBenhNoiTru_Info_CapNhatThongTinBN, (int)ePermission.mView);
            mNhanBenhNoiTru_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceiveInPatient,
                                                 (int)oRegistrionEx.mNhanBenhNoiTru_Info_XacNhan, (int)ePermission.mView);
            mNhanBenhNoiTru_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceiveInPatient,
                                                 (int)oRegistrionEx.mNhanBenhNoiTru_Info_XoaThe, (int)ePermission.mView);
            mNhanBenhNoiTru_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mReceiveInPatient,
                                                 (int)oRegistrionEx.mNhanBenhNoiTru_Info_XemPhongKham, (int)ePermission.mView);




        }
        public void resetAdmin()
        {
            mRegister = true;
            mProcessPayment = true;
            mInPatientRegister = true;
            mInPatientProcessPayment = true;
            mReceivePatient = true;
            mReceiveInPatient = true;
            mInPatientAdmission = true;
            mInPatientAdmissionManage = true;
            mDischarge = true;
            mChuyenKhoa = true;
            mKiemToan = true;
            mReportPaymentReceipt = true;
            mRegSummary = true;
        }

        private eLeftMenuByPTType _leftMenuByPTType = eLeftMenuByPTType.NONE;
        public eLeftMenuByPTType LeftMenuByPTType
        {
            get { return _leftMenuByPTType; }
            set
            {
                _leftMenuByPTType = value;
                NotifyOfPropertyChange(() => LeftMenuByPTType);
            }
        }


        private V_DeptTypeOperation _V_DeptTypeOperation;
        public V_DeptTypeOperation V_DeptTypeOperation
        {
            get { return _V_DeptTypeOperation; }
            set
            {
                _V_DeptTypeOperation = value;
                NotifyOfPropertyChange(() => V_DeptTypeOperation);
                Globals.V_DeptTypeOperation = V_DeptTypeOperation;
            }
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
        #region bool properties


        private bool _mRegister = true;
        private bool _mProcessPayment = true;
        private bool _mInPatientRegister = true;
        private bool _mHighTechBillingInvoice = true;
        private bool _mInPatientRegister_TV = true;
        private bool _mInPatientProcessPayment = true;
        private bool _mReceivePatient = true;
        private bool _mReceiveInPatient = true;
        private bool _mInPatientAdmission = true;
        private bool _mInPatientAdmissionManage = true;
        private bool _mChuyenKhoa = true;
        private bool _mDischarge = true;
        private bool _mDischarge_BS = true;
        private bool _mKiemToan = true;
        private bool _mReportPaymentReceipt = true;
        private bool _mRegSummary = true;
        private bool _mAdvanceMoney = true;
        private bool _mReCalcHiCmd = true;

        private bool _mInPt_ConfirmHI_Only = false;

        private bool _mNgoaiTru = true;
        private bool _mNoiTru = true;
        private bool _mBaoCao = true;

        public bool mRegister
        {
            get
            {
                return _mRegister;
            }
            set
            {
                if (_mRegister == value)
                    return;
                _mRegister = value;
                NotifyOfPropertyChange(() => mRegister);
            }
        }

        public bool mProcessPayment
        {
            get
            {
                return _mProcessPayment;
            }
            set
            {
                if (_mProcessPayment == value)
                    return;
                _mProcessPayment = value;
                NotifyOfPropertyChange(() => mProcessPayment);
            }
        }

        public bool mInPatientRegister
        {
            get
            {
                return _mInPatientRegister;
            }
            set
            {
                if (_mInPatientRegister == value)
                    return;
                _mInPatientRegister = value;
                NotifyOfPropertyChange(() => mInPatientRegister);
            }
        }

        public bool mHighTechBillingInvoice
        {
            get
            {
                return _mHighTechBillingInvoice;
            }
            set
            {
                if (_mHighTechBillingInvoice == value)
                    return;
                _mHighTechBillingInvoice = value;
                NotifyOfPropertyChange(() => mHighTechBillingInvoice);
            }
        }

        public bool mInPatientRegister_TV
        {
            get
            {
                return _mInPatientRegister_TV;
            }
            set
            {
                if (_mInPatientRegister_TV == value)
                    return;
                _mInPatientRegister_TV = value;
                NotifyOfPropertyChange(() => mInPatientRegister_TV);
            }
        }

        public bool mInPt_ConfirmHI_Only
        {
            get
            {
                return _mInPt_ConfirmHI_Only;
            }
            set
            {
                if (_mInPt_ConfirmHI_Only == value)
                    return;
                _mInPt_ConfirmHI_Only = value;
                NotifyOfPropertyChange(() => mInPt_ConfirmHI_Only);
            }
        }

        public bool mInPatientProcessPayment
        {
            get
            {
                return _mInPatientProcessPayment;
            }
            set
            {
                if (_mInPatientProcessPayment == value)
                    return;
                _mInPatientProcessPayment = value;
                NotifyOfPropertyChange(() => mInPatientProcessPayment);
            }
        }

        public bool mReceivePatient
        {
            get
            {
                return _mReceivePatient;
            }
            set
            {
                if (_mReceivePatient == value)
                    return;
                _mReceivePatient = value;
                NotifyOfPropertyChange(() => mReceivePatient);
            }
        }

        public bool mReceiveInPatient
        {
            get
            {
                return _mReceiveInPatient;
            }
            set
            {
                if (_mReceiveInPatient == value)
                    return;
                _mReceiveInPatient = value;
                NotifyOfPropertyChange(() => mReceiveInPatient);
            }
        }

        private bool _mReceiveInPatient_HI = true;
        public bool mReceiveInPatient_HI
        {
            get
            {
                return _mReceiveInPatient_HI;
            }
            set
            {
                if (_mReceiveInPatient_HI == value)
                    return;
                _mReceiveInPatient_HI = value;
                NotifyOfPropertyChange(() => mReceiveInPatient_HI);
            }
        }

        private bool _mTemp02 = true;
        public bool mTemp02
        {
            get
            {
                return _mTemp02;
            }
            set
            {
                if (_mTemp02 == value)
                    return;
                _mTemp02 = value;
                NotifyOfPropertyChange(() => mTemp02);
            }
        }

        private bool _mReCalcBillingInvoice = true;
        public bool mReCalcBillingInvoice
        {
            get
            {
                return _mReCalcBillingInvoice;
            }
            set
            {
                if (_mReCalcBillingInvoice == value)
                    return;
                _mReCalcBillingInvoice = value;
                NotifyOfPropertyChange(() => mReCalcBillingInvoice);
            }
        }

        private bool _mInPatientSettlement = true;
        public bool mInPatientSettlement
        {
            get
            {
                return _mInPatientSettlement;
            }
            set
            {
                if (_mInPatientSettlement == value)
                    return;
                _mInPatientSettlement = value;
                NotifyOfPropertyChange(() => mInPatientSettlement);
            }
        }

        private bool _mInPatientSettlement_TV = true;
        public bool mInPatientSettlement_TV
        {
            get
            {
                return _mInPatientSettlement_TV;
            }
            set
            {
                if (_mInPatientSettlement_TV == value)
                    return;
                _mInPatientSettlement_TV = value;
                NotifyOfPropertyChange(() => mInPatientSettlement_TV);
            }
        }

        private bool _mSuggestAdvanceMoney_TV = true;
        public bool mSuggestAdvanceMoney_TV
        {
            get
            {
                return _mSuggestAdvanceMoney_TV;
            }
            set
            {
                if (_mSuggestAdvanceMoney_TV == value)
                    return;
                _mSuggestAdvanceMoney_TV = value;
                NotifyOfPropertyChange(() => mSuggestAdvanceMoney_TV);
            }
        }

        private bool _mSuggestAdvanceMoney = true;
        public bool mSuggestAdvanceMoney
        {
            get
            {
                return _mSuggestAdvanceMoney;
            }
            set
            {
                if (_mSuggestAdvanceMoney == value)
                    return;
                _mSuggestAdvanceMoney = value;
                NotifyOfPropertyChange(() => mSuggestAdvanceMoney);
            }
        }

        private bool _mSuggestPayment_TV = true;
        public bool mSuggestPayment_TV
        {
            get
            {
                return _mSuggestPayment_TV;
            }
            set
            {
                if (_mSuggestPayment_TV == value)
                    return;
                _mSuggestPayment_TV = value;
                NotifyOfPropertyChange(() => mSuggestPayment_TV);
            }
        }

        private bool _mSuggestPayment = true;
        public bool mSuggestPayment
        {
            get
            {
                return _mSuggestPayment;
            }
            set
            {
                if (_mSuggestPayment == value)
                    return;
                _mSuggestPayment = value;
                NotifyOfPropertyChange(() => mSuggestPayment);
            }
        }

        private bool _mInPatientPayment = true;
        public bool mInPatientPayment
        {
            get
            {
                return _mInPatientPayment;
            }
            set
            {
                if (_mInPatientPayment == value)
                    return;
                _mInPatientPayment = value;
                NotifyOfPropertyChange(() => mInPatientPayment);
            }
        }

        private bool _mRptNotPayCashAdvance = true;
        public bool mRptNotPayCashAdvance
        {
            get
            {
                return _mRptNotPayCashAdvance;
            }
            set
            {
                if (_mRptNotPayCashAdvance == value)
                    return;
                _mRptNotPayCashAdvance = value;
                NotifyOfPropertyChange(() => mRptNotPayCashAdvance);
            }
        }

        private bool _mReportAdvanceMoney = true;
        public bool mReportAdvanceMoney
        {
            get
            {
                return _mReportAdvanceMoney;
            }
            set
            {
                if (_mReportAdvanceMoney == value)
                    return;
                _mReportAdvanceMoney = value;
                NotifyOfPropertyChange(() => mReportAdvanceMoney);
            }
        }

        private bool _mReportAdvanceMoneyForBill = true;
        public bool mReportAdvanceMoneyForBill
        {
            get
            {
                return _mReportAdvanceMoneyForBill;
            }
            set
            {
                if (_mReportAdvanceMoneyForBill == value)
                    return;
                _mReportAdvanceMoneyForBill = value;
                NotifyOfPropertyChange(() => mReportAdvanceMoneyForBill);
            }
        }

        private bool _mReportRepayAdvanceMoney = true;
        public bool mReportRepayAdvanceMoney
        {
            get
            {
                return _mReportRepayAdvanceMoney;
            }
            set
            {
                if (_mReportRepayAdvanceMoney == value)
                    return;
                _mReportRepayAdvanceMoney = value;
                NotifyOfPropertyChange(() => mReportRepayAdvanceMoney);
            }
        }

        private bool _mReportPatientSettlement = true;
        public bool mReportPatientSettlement
        {
            get
            {
                return _mReportPatientSettlement;
            }
            set
            {
                if (_mReportPatientSettlement == value)
                    return;
                _mReportPatientSettlement = value;
                NotifyOfPropertyChange(() => mReportPatientSettlement);
            }
        }

        private bool _mReportOutwardMedDeptInflow = true;
        public bool mReportOutwardMedDeptInflow
        {
            get
            {
                return _mReportOutwardMedDeptInflow;
            }
            set
            {
                if (_mReportOutwardMedDeptInflow == value)
                    return;
                _mReportOutwardMedDeptInflow = value;
                NotifyOfPropertyChange(() => mReportOutwardMedDeptInflow);
            }
        }

        private bool _mRptInPtImportExportDept = true;
        public bool mRptInPtImportExportDept
        {
            get
            {
                return _mRptInPtImportExportDept;
            }
            set
            {
                if (_mRptInPtImportExportDept == value)
                    return;
                _mRptInPtImportExportDept = value;
                NotifyOfPropertyChange(() => mRptInPtImportExportDept);
            }
        }

        private bool _mRptPtAreTreated = true;
        public bool mRptPtAreTreated
        {
            get
            {
                return _mRptPtAreTreated;
            }
            set
            {
                if (_mRptPtAreTreated == value)
                    return;
                _mRptPtAreTreated = value;
                NotifyOfPropertyChange(() => mRptPtAreTreated);
            }
        }

        public bool mInPatientAdmission
        {
            get
            {
                return _mInPatientAdmission;
            }
            set
            {
                if (_mInPatientAdmission == value)
                    return;
                _mInPatientAdmission = value;
                NotifyOfPropertyChange(() => mInPatientAdmission);
            }
        }

        public bool mInPatientAdmissionManage
        {
            get
            {
                return _mInPatientAdmissionManage;
            }
            set
            {
                if (_mInPatientAdmissionManage == value)
                    return;
                _mInPatientAdmissionManage = value;
                NotifyOfPropertyChange(() => mInPatientAdmissionManage);
            }
        }

        public bool mChuyenKhoa
        {
            get
            {
                return _mChuyenKhoa;
            }
            set
            {
                if (_mChuyenKhoa == value)
                    return;
                _mChuyenKhoa = value;
                NotifyOfPropertyChange(() => mChuyenKhoa);
            }
        }

        public bool mDischarge
        {
            get
            {
                return _mDischarge;
            }
            set
            {
                if (_mDischarge == value)
                    return;
                _mDischarge = value;
                NotifyOfPropertyChange(() => mDischarge);
            }
        }

        public bool mDischarge_BS
        {
            get
            {
                return _mDischarge_BS;
            }
            set
            {
                if (_mDischarge_BS == value)
                    return;
                _mDischarge_BS = value;
                NotifyOfPropertyChange(() => mDischarge_BS);
            }
        }

        public bool mKiemToan
        {
            get
            {
                return _mKiemToan;
            }
            set
            {
                if (_mKiemToan == value)
                    return;
                _mKiemToan = value;
                NotifyOfPropertyChange(() => mKiemToan);
            }
        }

        public bool mReportPaymentReceipt
        {
            get
            {
                return _mReportPaymentReceipt;
            }
            set
            {
                if (_mReportPaymentReceipt == value)
                    return;
                _mReportPaymentReceipt = value;
                NotifyOfPropertyChange(() => mReportPaymentReceipt);
            }
        }

        public bool mRegSummary
        {
            get
            {
                return _mRegSummary;
            }
            set
            {
                if (_mRegSummary == value)
                    return;
                _mRegSummary = value;
                NotifyOfPropertyChange(() => mRegSummary);
            }
        }

        public bool mAdvanceMoney
        {
            get
            {
                return _mAdvanceMoney;
            }
            set
            {
                if (_mAdvanceMoney == value)
                    return;
                _mAdvanceMoney = value;
                NotifyOfPropertyChange(() => mAdvanceMoney);
            }
        }

        public bool mReCalcHiCmd
        {
            get
            {
                return _mReCalcHiCmd;
            }
            set
            {
                if (_mReCalcHiCmd == value)
                    return;
                _mReCalcHiCmd = value;
                NotifyOfPropertyChange(() => mReCalcHiCmd);
            }
        }
        public bool mNgoaiTru
        {
            get
            {
                return _mNgoaiTru;
            }
            set
            {
                if (_mNgoaiTru == value)
                    return;
                _mNgoaiTru = value;
                NotifyOfPropertyChange(() => mNgoaiTru);
            }
        }

        public bool mNoiTru
        {
            get
            {
                return _mNoiTru;
            }
            set
            {
                if (_mNoiTru == value)
                    return;
                _mNoiTru = value;
                NotifyOfPropertyChange(() => mNoiTru);
            }
        }

        public bool mBaoCao
        {
            get
            {
                return _mBaoCao;
            }
            set
            {
                if (_mBaoCao == value)
                    return;
                _mBaoCao = value;
                NotifyOfPropertyChange(() => mBaoCao);
            }
        }

        private bool _mBaoCaoCLSSo = true;
        public bool mBaoCaoCLSSo
        {
            get
            {
                return _mBaoCaoCLSSo;
            }
            set
            {
                if (_mBaoCaoCLSSo == value)
                    return;
                _mBaoCaoCLSSo = value;
                NotifyOfPropertyChange(() => mBaoCaoCLSSo);
            }
        }

        private bool _mBaoCaoDanhSachDangKyBHYT = true;
        public bool mBaoCaoDanhSachDangKyBHYT
        {
            get
            {
                return _mBaoCaoDanhSachDangKyBHYT;
            }
            set
            {
                if (_mBaoCaoDanhSachDangKyBHYT == value)
                    return;
                _mBaoCaoDanhSachDangKyBHYT = value;
                NotifyOfPropertyChange(() => mBaoCaoDanhSachDangKyBHYT);
            }
        }

        private bool _mBaoCaoNhanhKhuKhamBenh = true;
        public bool mBaoCaoNhanhKhuKhamBenh
        {
            get
            {
                return _mBaoCaoNhanhKhuKhamBenh;
            }
            set
            {
                if (_mBaoCaoNhanhKhuKhamBenh == value)
                    return;
                _mBaoCaoNhanhKhuKhamBenh = value;
                NotifyOfPropertyChange(() => mBaoCaoNhanhKhuKhamBenh);
            }
        }

        private bool _mBaoCaoBangKeThuPhiXN = true;
        public bool mBaoCaoBangKeThuPhiXN
        {
            get
            {
                return _mBaoCaoBangKeThuPhiXN;
            }
            set
            {
                if (_mBaoCaoBangKeThuPhiXN == value)
                    return;
                _mBaoCaoBangKeThuPhiXN = value;
                NotifyOfPropertyChange(() => mBaoCaoBangKeThuPhiXN);
            }
        }

        private bool _mBaoCaomBaoCaoBangKeThuPhiXN_HA = true;
        public bool mBaoCaomBaoCaoBangKeThuPhiXN_HA
        {
            get
            {
                return _mBaoCaomBaoCaoBangKeThuPhiXN_HA;
            }
            set
            {
                if (_mBaoCaomBaoCaoBangKeThuPhiXN_HA == value)
                    return;
                _mBaoCaomBaoCaoBangKeThuPhiXN_HA = value;
                NotifyOfPropertyChange(() => mBaoCaomBaoCaoBangKeThuPhiXN_HA);
            }
        }

        private bool _mBaoCaoPhieuNopTien = true;
        public bool mBaoCaoPhieuNopTien 
        {
            get
            {
                return _mBaoCaoPhieuNopTien;
            }
            set
            {
                if (_mBaoCaoPhieuNopTien == value)
                    return;
                _mBaoCaoPhieuNopTien = value;
                NotifyOfPropertyChange(() => mBaoCaoPhieuNopTien);
            }
        }

        private bool _mBaoCaoTongHopDoanhThuPK = true;
        public bool mBaoCaoTongHopDoanhThuPK
        {
            get
            {
                return _mBaoCaoTongHopDoanhThuPK;
            }
            set
            {
                if (_mBaoCaoTongHopDoanhThuPK == value)
                    return;
                _mBaoCaoTongHopDoanhThuPK = value;
                NotifyOfPropertyChange(() => mBaoCaoTongHopDoanhThuPK);
            }
        }


        private bool _mNhanBenhBH_ThongTin_Sua = true;
        private bool _mNhanBenhBH_TheBH_ThemMoi = true;
        private bool _mNhanBenhBH_TheBH_XacNhan = true;
        private bool _mNhanBenhBH_DangKy = true;
        private bool _mNhanBenhBH_TheBH_Sua = true;

        public bool mNhanBenhBH_ThongTin_Sua
        {
            get
            {
                return _mNhanBenhBH_ThongTin_Sua;
            }
            set
            {
                if (_mNhanBenhBH_ThongTin_Sua == value)
                    return;
                _mNhanBenhBH_ThongTin_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_ThongTin_Sua);
            }
        }

        public bool mNhanBenhBH_TheBH_ThemMoi
        {
            get
            {
                return _mNhanBenhBH_TheBH_ThemMoi;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_ThemMoi == value)
                    return;
                _mNhanBenhBH_TheBH_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_ThemMoi);
            }
        }

        public bool mNhanBenhBH_TheBH_XacNhan
        {
            get
            {
                return _mNhanBenhBH_TheBH_XacNhan;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_XacNhan == value)
                    return;
                _mNhanBenhBH_TheBH_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_XacNhan);
            }
        }

        public bool mNhanBenhBH_TheBH_Sua
        {
            get
            {
                return _mNhanBenhBH_TheBH_Sua;
            }
            set
            {
                if (_mNhanBenhBH_TheBH_Sua == value)
                    return;
                _mNhanBenhBH_TheBH_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_TheBH_Sua);
            }
        }

        public bool mNhanBenhBH_DangKy
        {
            get
            {
                return _mNhanBenhBH_DangKy;
            }
            set
            {
                if (_mNhanBenhBH_DangKy == value)
                    return;
                _mNhanBenhBH_DangKy = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_DangKy);
            }
        }

        private bool _mNhanBenhNoiTru_ThongTin_Sua = true;
        private bool _mNhanBenhNoiTru_TheBH_ThemMoi = true;
        private bool _mNhanBenhNoiTru_TheBH_XacNhan = true;
        private bool _mNhanBenhNoiTru_DangKy = true;
        private bool _mNhanBenhNoiTru_TheBH_Sua = true;

        public bool mNhanBenhNoiTru_ThongTin_Sua
        {
            get
            {
                return _mNhanBenhNoiTru_ThongTin_Sua;
            }
            set
            {
                if (_mNhanBenhNoiTru_ThongTin_Sua == value)
                    return;
                _mNhanBenhNoiTru_ThongTin_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_ThongTin_Sua);
            }
        }

        public bool mNhanBenhNoiTru_TheBH_ThemMoi
        {
            get
            {
                return _mNhanBenhNoiTru_TheBH_ThemMoi;
            }
            set
            {
                if (_mNhanBenhNoiTru_TheBH_ThemMoi == value)
                    return;
                _mNhanBenhNoiTru_TheBH_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_TheBH_ThemMoi);
            }
        }

        public bool mNhanBenhNoiTru_TheBH_XacNhan
        {
            get
            {
                return _mNhanBenhNoiTru_TheBH_XacNhan;
            }
            set
            {
                if (_mNhanBenhNoiTru_TheBH_XacNhan == value)
                    return;
                _mNhanBenhNoiTru_TheBH_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_TheBH_XacNhan);
            }
        }

        public bool mNhanBenhNoiTru_TheBH_Sua
        {
            get
            {
                return _mNhanBenhNoiTru_TheBH_Sua;
            }
            set
            {
                if (_mNhanBenhNoiTru_TheBH_Sua == value)
                    return;
                _mNhanBenhNoiTru_TheBH_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_TheBH_Sua);
            }
        }

        public bool mNhanBenhNoiTru_DangKy
        {
            get
            {
                return _mNhanBenhNoiTru_DangKy;
            }
            set
            {
                if (_mNhanBenhNoiTru_DangKy == value)
                    return;
                _mNhanBenhNoiTru_DangKy = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_DangKy);
            }
        }

        //phan nay nam trong module chung
        private bool _mNhanBenhBH_Patient_TimBN = true;
        private bool _mNhanBenhBH_Patient_ThemBN = true;
        private bool _mNhanBenhBH_Patient_TimDangKy = true;

        private bool _mNhanBenhBH_Info_CapNhatThongTinBN = true;
        private bool _mNhanBenhBH_Info_XacNhan = true;
        private bool _mNhanBenhBH_Info_XoaThe = true;
        private bool _mNhanBenhBH_Info_XemPhongKham = true;

        public bool mNhanBenhBH_Patient_TimBN
        {
            get
            {
                return _mNhanBenhBH_Patient_TimBN;
            }
            set
            {
                if (_mNhanBenhBH_Patient_TimBN == value)
                    return;
                _mNhanBenhBH_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_Patient_TimBN);
            }
        }

        public bool mNhanBenhBH_Patient_ThemBN
        {
            get
            {
                return _mNhanBenhBH_Patient_ThemBN;
            }
            set
            {
                if (_mNhanBenhBH_Patient_ThemBN == value)
                    return;
                _mNhanBenhBH_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_Patient_ThemBN);
            }
        }

        public bool mNhanBenhBH_Patient_TimDangKy
        {
            get
            {
                return _mNhanBenhBH_Patient_TimDangKy;
            }
            set
            {
                if (_mNhanBenhBH_Patient_TimDangKy == value)
                    return;
                _mNhanBenhBH_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_Patient_TimDangKy);
            }
        }

        public bool mNhanBenhBH_Info_CapNhatThongTinBN
        {
            get
            {
                return _mNhanBenhBH_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mNhanBenhBH_Info_CapNhatThongTinBN == value)
                    return;
                _mNhanBenhBH_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_Info_CapNhatThongTinBN);
            }
        }

        public bool mNhanBenhBH_Info_XacNhan
        {
            get
            {
                return _mNhanBenhBH_Info_XacNhan;
            }
            set
            {
                if (_mNhanBenhBH_Info_XacNhan == value)
                    return;
                _mNhanBenhBH_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_Info_XacNhan);
            }
        }

        public bool mNhanBenhBH_Info_XoaThe
        {
            get
            {
                return _mNhanBenhBH_Info_XoaThe;
            }
            set
            {
                if (_mNhanBenhBH_Info_XoaThe == value)
                    return;
                _mNhanBenhBH_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_Info_XoaThe);
            }
        }

        public bool mNhanBenhBH_Info_XemPhongKham
        {
            get
            {
                return _mNhanBenhBH_Info_XemPhongKham;
            }
            set
            {
                if (_mNhanBenhBH_Info_XemPhongKham == value)
                    return;
                _mNhanBenhBH_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mNhanBenhBH_Info_XemPhongKham);
            }
        }

        //phan nay nam trong module chung
        private bool _mNhanBenhNoiTru_Patient_TimBN = true;
        private bool _mNhanBenhNoiTru_Patient_ThemBN = true;
        private bool _mNhanBenhNoiTru_Patient_TimDangKy = true;

        private bool _mNhanBenhNoiTru_Info_CapNhatThongTinBN = true;
        private bool _mNhanBenhNoiTru_Info_XacNhan = true;
        private bool _mNhanBenhNoiTru_Info_XoaThe = true;
        private bool _mNhanBenhNoiTru_Info_XemPhongKham = true;

        public bool mNhanBenhNoiTru_Patient_TimBN
        {
            get
            {
                return _mNhanBenhNoiTru_Patient_TimBN;
            }
            set
            {
                if (_mNhanBenhNoiTru_Patient_TimBN == value)
                    return;
                _mNhanBenhNoiTru_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_Patient_TimBN);
            }
        }

        public bool mNhanBenhNoiTru_Patient_ThemBN
        {
            get
            {
                return _mNhanBenhNoiTru_Patient_ThemBN;
            }
            set
            {
                if (_mNhanBenhNoiTru_Patient_ThemBN == value)
                    return;
                _mNhanBenhNoiTru_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_Patient_ThemBN);
            }
        }

        public bool mNhanBenhNoiTru_Patient_TimDangKy
        {
            get
            {
                return _mNhanBenhNoiTru_Patient_TimDangKy;
            }
            set
            {
                if (_mNhanBenhNoiTru_Patient_TimDangKy == value)
                    return;
                _mNhanBenhNoiTru_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_Patient_TimDangKy);
            }
        }

        public bool mNhanBenhNoiTru_Info_CapNhatThongTinBN
        {
            get
            {
                return _mNhanBenhNoiTru_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mNhanBenhNoiTru_Info_CapNhatThongTinBN == value)
                    return;
                _mNhanBenhNoiTru_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_Info_CapNhatThongTinBN);
            }
        }

        public bool mNhanBenhNoiTru_Info_XacNhan
        {
            get
            {
                return _mNhanBenhNoiTru_Info_XacNhan;
            }
            set
            {
                if (_mNhanBenhNoiTru_Info_XacNhan == value)
                    return;
                _mNhanBenhNoiTru_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_Info_XacNhan);
            }
        }

        public bool mNhanBenhNoiTru_Info_XoaThe
        {
            get
            {
                return _mNhanBenhNoiTru_Info_XoaThe;
            }
            set
            {
                if (_mNhanBenhNoiTru_Info_XoaThe == value)
                    return;
                _mNhanBenhNoiTru_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_Info_XoaThe);
            }
        }

        public bool mNhanBenhNoiTru_Info_XemPhongKham
        {
            get
            {
                return _mNhanBenhNoiTru_Info_XemPhongKham;
            }
            set
            {
                if (_mNhanBenhNoiTru_Info_XemPhongKham == value)
                    return;
                _mNhanBenhNoiTru_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mNhanBenhNoiTru_Info_XemPhongKham);
            }
        }

        #endregion

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
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

        private void RegisterCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.OUT_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNgoaiTru) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IPatientRegistration>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var regVm = Globals.GetViewModel<IPatientRegistration>();

                regVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
                regModule.MainContent = regVm;
                ((Conductor<object>)regModule).ActivateItem(regVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });

            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;

        }
        public void RegisterCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K2863_G1_DKDV;

            // RegisterCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                RegisterCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RegisterCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void InPatientRegisterCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    ||V_DeptTypeOperation == V_DeptTypeOperation.KhoaNgoaiTru) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<IInPatientRegistration>();

            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var regVm = Globals.GetViewModel<IInPatientRegistration>();
                Globals.IsAdmission = true;
                regVm.UsedByTaiVuOffice = false;

                regVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;               
                regModule.MainContent = regVm;

                regVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                ((Conductor<object>)regModule).ActivateItem(regVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPatientRegisterCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T0782_G1_TaoBill;

            //  InPatientRegisterCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPatientRegisterCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPatientRegisterCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void InPatientRegisterCmd_In_TV(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var regVm = Globals.GetViewModel<IInPatientRegistration>();
            Globals.IsAdmission = true;
            regVm.UsedByTaiVuOffice = true;

            regVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

            regModule.MainContent = regVm;

            regVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            ((Conductor<object>)regModule).ActivateItem(regVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPatientRegisterCmd_TV(object source)
        {
            Globals.TitleForm = eHCMSResources.T0782_G1_TaoBill;
            
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPatientRegisterCmd_In_TV(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPatientRegisterCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void InPtRegister_HighTechServiceCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var regVm = Globals.GetViewModel<IInPatientRegistration>();
            Globals.IsAdmission = true;
            regVm.UsedByTaiVuOffice = false;
            regVm.IsHighTechServiceBill = true;
            regVm.ShowInPackageColumn = true;

            regVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            regModule.MainContent = regVm;

            regVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            ((Conductor<object>)regModule).ActivateItem(regVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPtRegister_HighTechServiceCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T0782_G1_TaoBill;

            //  InPatientRegisterCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPtRegister_HighTechServiceCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPtRegister_HighTechServiceCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ProcessPaymentCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.OUT_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNgoaiTru) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IProcessPayment>();

            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var paymentVm = Globals.GetViewModel<IProcessPayment>();

                regModule.MainContent = paymentVm;

                paymentVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
                ((Conductor<object>)regModule).ActivateItem(paymentVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NGOAITRU});
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
        }

        public void ProcessPaymentCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1308_G1_TinhTien;
            //ProcessPaymentCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ProcessPaymentCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ProcessPaymentCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ConfirmHIBenefitCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            LeftMenuByPTType = eLeftMenuByPTType.NONE;
            var mModule = Globals.GetViewModel<IRegistrationModule>();
            var mView = Globals.GetViewModel<IPrescription>();
            mModule.MainContent = mView;
            ((Conductor<object>)mModule).ActivateItem(mView);
        }
        public void ConfirmHIBenefitCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G2370_G1_XNhanBHYT;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ConfirmHIBenefitCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ConfirmHIBenefitCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        public void RegistrationSummaryCmd(object source)
        {
            Globals.TitleForm = "Danh Sách Đăng Ký";
            
            //RegistrationSummaryCmd_In(source);
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                RegistrationSummaryCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RegistrationSummaryCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void RegistrationSummaryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var paymentVm = Globals.GetViewModel<IThongKeDoanhThuDangKy>();

            regModule.MainContent = paymentVm;
            ((Conductor<object>)regModule).ActivateItem(paymentVm);
        }

        public void SearchPatientCmd()
        {
        }

        // Nhan Benh BH Ngoai Tru
        private void ReceivePatientCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var receiveVm = Globals.GetViewModel<IReceivePatient>();
            receiveVm.PageTitle = eHCMSResources.N0183_G1_NhanBenhBaoHiem.ToUpper();
            receiveVm.RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
            // HPT: Thêm giá trị cho thuộc tính V_RegForPatientOfType để đối xử riêng biệt với những đường dẫn dành cho đăng ký Vãng Lai và Tiền Giải Phẫu
            // Cụ thể là các thay đổi điều kiện tìm kiếm và điều khiển một số control
            receiveVm.V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.DKBN_NGTRU;

            receiveVm.mNhanBenh_ThongTin_Sua = mNhanBenhBH_ThongTin_Sua;
            receiveVm.mNhanBenh_TheBH_ThemMoi = mNhanBenhBH_TheBH_ThemMoi;
            receiveVm.mNhanBenh_TheBH_XacNhan = mNhanBenhBH_TheBH_XacNhan;
            receiveVm.mNhanBenh_DangKy = mNhanBenhBH_DangKy;
            receiveVm.mNhanBenh_TheBH_Sua = mNhanBenhBH_TheBH_Sua;


            receiveVm.mPatient_TimBN = mNhanBenhBH_Patient_TimBN;
            receiveVm.mPatient_ThemBN = mNhanBenhBH_Patient_ThemBN;
            receiveVm.mPatient_TimDangKy = mNhanBenhBH_Patient_TimDangKy;

            receiveVm.mInfo_CapNhatThongTinBN = mNhanBenhBH_Info_CapNhatThongTinBN;
            receiveVm.mInfo_XacNhan = mNhanBenhBH_Info_XacNhan;
            receiveVm.mInfo_XoaThe = mNhanBenhBH_Info_XoaThe;
            receiveVm.mInfo_XemPhongKham = mNhanBenhBH_Info_XemPhongKham;

            receiveVm.InitViewContent();

            Globals.IsAdmission = null;
            regModule.MainContent = receiveVm;

            receiveVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            ((Conductor<object>)regModule).ActivateItem(receiveVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NGOAITRU});
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
        }

        public void ReceivePatientCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.N0183_G1_NhanBenhBaoHiem;
            
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                _mInPt_ConfirmHI_Only = false;
                ReceivePatientCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReceivePatientCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }
        //HPT: Thêm biến loại nhận bệnh (vãng lai hoặc tiền giải phẫu)
        private AllLookupValues.V_RegForPatientOfType _V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.Unknown;
        public AllLookupValues.V_RegForPatientOfType V_RegForPatientOfType
        {
            get
            {
                return _V_RegForPatientOfType;
            }
            set
            {
                _V_RegForPatientOfType = value;
                NotifyOfPropertyChange (() => V_RegForPatientOfType);
            }
        }
        // TxD: Common Command Handler for 'Nhan Benh Noi Tru' & 'Nhan benh Cap Cuu' Khong & Co BHYT
        public bool Visibility_CheckboxCasualOrPreOp = false;
        public void Proc_ReceiveInPatient_Cmd(object source)
        {
            Button hyperBtn = (Button)source;
            AllLookupValues.RegistrationType regType = AllLookupValues.RegistrationType.Unknown;
            Visibility_CheckboxCasualOrPreOp = false;
            // Mặc định V_RegForPatientOfType là Unknow, chỉ khi click vào các đường dẫn dành cho đăng ký vãng lai và tiền giải phẫu thì mới gán lại cho đúng
            V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.Unknown;
            string strPageTitle = "";
            bool isEmergency = false;
            bool _isRegForCasualOrPreOp = false;
            Globals.IsAdmission = false;

            if (hyperBtn.Name == "ReConfirmHI_ForInPtCmd")
            {
                Globals.IsAdmission = true;
                Globals.TitleForm = eHCMSResources.G2376_G1_XNhanLaiBHYTe.ToUpper();
                regType = AllLookupValues.RegistrationType.XAC_NHAN_LAI_BHYT;
                mInPt_ConfirmHI_Only = true;
                Visibility_CheckboxCasualOrPreOp = true;
                LeftMenuByPTType = eLeftMenuByPTType.NONE;

                strPageTitle = eHCMSResources.G2376_G1_XNhanLaiBHYTe.ToUpper();
                isEmergency = true;
            }
            else if (hyperBtn.Name == "ReceiveInPatientCmd")
            {
                Globals.TitleForm = eHCMSResources.N0187_G1_NhanBenhNT;
                regType = AllLookupValues.RegistrationType.NOI_TRU;
                _mInPt_ConfirmHI_Only = false;

                LeftMenuByPTType = eLeftMenuByPTType.NONE;

                strPageTitle = string.Format("  {0}", eHCMSResources.N0187_G1_NhanBenhNT.ToUpper());
                isEmergency = false;
            }
            // HPT: Thêm trường hợp click đường dẫn mới
            // Nhận bệnh vãng lai
            else if (hyperBtn.Name == "ReceiveInPatient_Casual_Cmd")
            {
                Globals.TitleForm = eHCMSResources.N0189_G1_NhanBenhVL;
                regType = AllLookupValues.RegistrationType.NOI_TRU;
                _mInPt_ConfirmHI_Only = false;
                LeftMenuByPTType = eLeftMenuByPTType.NONE;

                strPageTitle = string.Format("  {0}", eHCMSResources.N0189_G1_NhanBenhVL.ToUpper());
                isEmergency = false;
                //HPT: Gán giá trị V_RegForPatientOfType để hạn chế một số control (checkbox BN cấp cứu, BN nước ngoài) khi dùng chung ReceivePatientView & ViewModel
                V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI;
                _isRegForCasualOrPreOp = true;

            }
            // Nhận bệnh Tiền giải phẫu
            else if (hyperBtn.Name == "ReceiveInPatient_PreOp_Cmd")
            {
                Globals.TitleForm = eHCMSResources.Z0081_G1_BNGPhauKTC;
                regType = AllLookupValues.RegistrationType.NOI_TRU;
                _mInPt_ConfirmHI_Only = false;
                LeftMenuByPTType = eLeftMenuByPTType.NONE;

                strPageTitle = string.Format("  {0}", eHCMSResources.A1152_G1_NBGiaiPhaiKTC.ToUpper());
                isEmergency = false;
                V_RegForPatientOfType = AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT;
                _isRegForCasualOrPreOp = true;
            }
            else if (hyperBtn.Name == "ReceiveInPatient_WithHI_Cmd")
            {
                Globals.TitleForm = eHCMSResources.A1154_G1_NBNoiTruBHYT;
                regType = AllLookupValues.RegistrationType.NOI_TRU_BHYT;
                _mInPt_ConfirmHI_Only = false;

                LeftMenuByPTType = eLeftMenuByPTType.NONE;

                strPageTitle = string.Format("  {0}", eHCMSResources.A1154_G1_NBNoiTruBHYT.ToUpper());
                isEmergency = false;           
            }
            
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Process_ReceiveInPatient_Common(hyperBtn, regType, strPageTitle, isEmergency, _isRegForCasualOrPreOp);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Process_ReceiveInPatient_Common(hyperBtn, regType, strPageTitle, isEmergency, _isRegForCasualOrPreOp);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        // TxD: Common Process for 'Nhan Benh Noi Tru' & 'Nhan benh Cap Cuu' Khong & Co BHYT
        private void Process_ReceiveInPatient_Common(Button theHyperLinkBtn, AllLookupValues.RegistrationType regType, string strPageTitle, bool isEmergency, bool isRegForCasualOrPreOp)
        {
            string ReceiveInPatient_WithHI_Link = "ReceiveInPatient_WithHI_Cmd";
            string ReConfirmHI_Link = "ReConfirmHI_ForInPtCmd";

            SetHyperlinkSelectedStyle(theHyperLinkBtn);
            Globals.PageName = Globals.TitleForm;

            var receiveVm = Globals.GetViewModel<IReceivePatient>();
            receiveVm.RegistrationType = regType;
            // TxD 13/12/2014 Emergency depends ON confirmation In PatientDetails view model
            //receiveVm.IsEmergency = isEmergency;
            
            receiveVm.PageTitle = strPageTitle;
            receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            //HPT: set giá trị V_RegForPatientOfType cho ViewModel để nhận biết ViewModel được mở từ đường dẫn nào
            //HPT: Tùy theo đường dẫn khác nhau mà ViewModel được mở thêm hoặc giấu đi các control cho phù hợp
            receiveVm.V_RegForPatientOfType = V_RegForPatientOfType;
            receiveVm.Visibility_CheckboxCasualOrPreOp = Visibility_CheckboxCasualOrPreOp;
            receiveVm.IsRegForCasualOrPreOp = isRegForCasualOrPreOp;
            //KMx: Hiện tại Nhận Bệnh Nội Trú BHYT và Xác Nhận Lại BHYT chưa phân quyền, nếu user vào được 1 trong 2 link đó thì cho thấy tất cả chức năng trang đó. Còn Nhận Bệnh Nội Trú thường, cấu hình gì thì thấy cái đó (21/12/2014 09:43).
            receiveVm.mNhanBenh_ThongTin_Sua = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_ThongTin_Sua;
            receiveVm.mNhanBenh_TheBH_ThemMoi = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_TheBH_ThemMoi;
            receiveVm.mNhanBenh_TheBH_XacNhan = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_TheBH_XacNhan;
            receiveVm.mNhanBenh_DangKy = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_DangKy;
            receiveVm.mNhanBenh_TheBH_Sua = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_TheBH_Sua;

            receiveVm.mPatient_TimBN = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_Patient_TimBN;
            receiveVm.mPatient_ThemBN = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_Patient_ThemBN;
            receiveVm.mPatient_TimDangKy = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_Patient_TimDangKy;

            receiveVm.mInfo_CapNhatThongTinBN = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_Info_CapNhatThongTinBN;
            receiveVm.mInfo_XacNhan = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_Info_XacNhan;
            receiveVm.mInfo_XoaThe = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_Info_XoaThe;
            receiveVm.mInfo_XemPhongKham = theHyperLinkBtn.Name == ReceiveInPatient_WithHI_Link || theHyperLinkBtn.Name == ReConfirmHI_Link ? true : mNhanBenhNoiTru_Info_XemPhongKham;

            // TxD 15/12/2014: The following mInPt_ConfirmHI_Only should be SET after mNhanBenh_DangKy
            //                  because it will affect the value of mNhanBenh_DangKy ie. either one of the two should be true
            receiveVm.mInPt_ConfirmHI_Only = mInPt_ConfirmHI_Only;
            receiveVm.InitViewContent();

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            regModule.MainContent = receiveVm;

            receiveVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            ((Conductor<object>)regModule).ActivateItem(receiveVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }


        private void InPatientAdmissionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNoi) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<IInPatientAdmission>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var receiveVm = Globals.GetViewModel<IInPatientAdmission>();
                receiveVm.TitleForm = Globals.TitleForm;
                receiveVm.isAdmision = true;
                Globals.IsAdmission = false;
                receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

                receiveVm.InitViewContent();
                receiveVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                regModule.MainContent = receiveVm;
                ((Conductor<object>)regModule).ActivateItem(receiveVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPatientAdmissionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.N0221_G1_NhapVien;
            
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPatientAdmissionCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPatientAdmissionCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ManageInPatientAdmissionCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var receiveVm = Globals.GetViewModel<IInPatientAdmission>();
                receiveVm.TitleForm = Globals.TitleForm;
                receiveVm.isAdmision = false;
                receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

                receiveVm.InitViewContent();
                receiveVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                Globals.IsAdmission = true;
                regModule.MainContent = receiveVm;
                ((Conductor<object>)regModule).ActivateItem(receiveVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void ManageInPatientAdmissionCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0449_G1_QuanLyBNNoiTru;

            //InPatientAdmissionCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ManageInPatientAdmissionCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ManageInPatientAdmissionCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void Temp02NoiTruCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            
            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var receiveVm = Globals.GetViewModel<ITemp02NoiTru>();

            regModule.MainContent = receiveVm;
            ((Conductor<object>)regModule).ActivateItem(receiveVm);
        }

        public void Temp02NoiTruCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0047_G1_Mau02;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Temp02NoiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp02NoiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void Temp02NoiTruNewCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var receiveVm = Globals.GetViewModel<ITemp02NoiTru>();
            receiveVm.IsTemp02NoiTruNew = true;

            regModule.MainContent = receiveVm;
            ((Conductor<object>)regModule).ActivateItem(receiveVm);
        }

        public void Temp02NoiTruNewCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0007_G1_Mau02New;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                Temp02NoiTruNewCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp02NoiTruNewCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void CreateRptForm02NoiTruCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var receiveVm = Globals.GetViewModel<IInPatientForm02>();
            Globals.IsAdmission = true;
            receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

            receiveVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = receiveVm;            
            ((Conductor<object>)regModule).ActivateItem(receiveVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void CreateRptForm02NoiTruCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T0793_G1_TaoMau02;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                CreateRptForm02NoiTruCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CreateRptForm02NoiTruCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        public void DeptTranferCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K2282_G1_ChKhoa;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            //InPatientAdmissionCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                DeptTranferCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DeptTranferCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void DeptTranferCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNoi) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<IInPatientTransferDeptReq>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var receiveVm = Globals.GetViewModel<IInPatientTransferDeptReq>();
                Globals.IsAdmission = true;
                receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

                receiveVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                regModule.MainContent = receiveVm;
                ((Conductor<object>)regModule).ActivateItem(receiveVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        private void DischargeCmd_BS_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;
            
            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var inPtDischargeVm = Globals.GetViewModel<IDischargeNew>();
            Globals.IsAdmission = true;
            inPtDischargeVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            inPtDischargeVm.EnableSearchAllDepts = true;
            inPtDischargeVm.InitView(false);
            inPtDischargeVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = inPtDischargeVm;
            ((Conductor<object>)regModule).ActivateItem(inPtDischargeVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void DischargeCmd_BS(object source)
        {
            Globals.TitleForm = eHCMSResources.G2900_G1_XV;
            
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                DischargeCmd_BS_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DischargeCmd_BS_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void DischargeCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNoi) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<IDischarge>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var inPtDischargeVm = Globals.GetViewModel<IDischargeNew>();
                Globals.IsAdmission = true;
                inPtDischargeVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
                inPtDischargeVm.IsShowConfirmDischargeBtn = true;
                inPtDischargeVm.InitView(false);

                inPtDischargeVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                regModule.MainContent = inPtDischargeVm;
                ((Conductor<object>)regModule).ActivateItem(inPtDischargeVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void DischargeCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G2900_G1_XV;

            //DischargeCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                DischargeCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DischargeCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void AdvanceMoneyCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNoi) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<IAdvanceMoney>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();

                //KMx: Chuyển từ View IAdvanceMoney -> IInPatientCashAdvance, tách "Tạm ứng" và "Thanh toán" ra riêng để dễ quản lý (14/09/2014 17:41).
                //var vm = Globals.GetViewModel<IAdvanceMoney>();
                var vm = Globals.GetViewModel<IInPatientCashAdvance>();
                Globals.IsAdmission = true;
                vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

                vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                regModule.MainContent = vm;
                ((Conductor<object>)regModule).ActivateItem(vm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void AdvanceMoneyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T0774_G1_TU;

            // AdvanceMoneyCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                AdvanceMoneyCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AdvanceMoneyCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        //KMx: Thêm Link "Thanh Toán", tách ra từ "Tạm ứng" (14/09/2014 17:42).
        private void InPatientPaymentCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            //KMx: Chuyển từ View IAdvanceMoney -> IInPatientPayment, tách "Tạm ứng" và "Thanh toán" ra riêng để dễ quản lý (14/09/2014 17:41).
            //var vm = Globals.GetViewModel<IAdvanceMoney>();
            var vm = Globals.GetViewModel<IInPatientPayment>();
            Globals.IsAdmission = true;
            vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

            vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = vm;
            ((Conductor<object>)regModule).ActivateItem(vm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPatientPaymentCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G0128_G1_TToan;

            // AdvanceMoneyCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPatientPaymentCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPatientPaymentCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void ReportAdvanceMoneyCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            //if (innerVm.aucHoldConsultDoctor != null)
            //{
            //    innerVm.aucHoldConsultDoctor.IsMultiStaffCatType = true;
            //    innerVm.aucHoldConsultDoctor.StaffCatTypeList.Add((long)V_StaffCatType.NhanVienQuayDangKy);
            //}

            innerVm.eItem = ReportName.BAOCAO_THUTIEN_TAMUNG;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;

            //KMx: ReportSwitch = 0: Báo cáo thu tạm ứng, 1: Báo cáo thu tạm ứng Bill, 2: Báo cáo thanh toán (25/01/2015 14:29). 
            innerVm.ReportSwitch = 0;
            innerVm.mRegistrationType = true;
            innerVm.IsShowPaymentMode = true;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportAdvanceMoneyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1077_G1_BCThuTienTU;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportAdvanceMoneyCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportAdvanceMoneyCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ReportAdvanceMoneyForBillCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.BAOCAO_THUTIEN_TAMUNG;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;

            //KMx: ReportSwitch = 0: Báo cáo thu tạm ứng, 1: Báo cáo thu tạm ứng Bill, 2: Báo cáo thanh toán (25/01/2015 14:29). 
            innerVm.ReportSwitch = 1;
            innerVm.mRegistrationType = true;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportAdvanceMoneyForBillCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1078_G1_BCThuTienTUBill;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportAdvanceMoneyForBillCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportAdvanceMoneyForBillCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ReportRepayAdvanceMoneyCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.BAOCAO_THUTIEN_TAMUNG;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;

            //KMx: ReportSwitch = 0: Báo cáo thu tạm ứng, 1: Báo cáo thu tạm ứng Bill, 2: Báo cáo thanh toán (25/01/2015 14:29). 
            innerVm.ReportSwitch = 2;
            innerVm.mRegistrationType = true;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportRepayAdvanceMoneyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1070_G1_BCTToan;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportRepayAdvanceMoneyCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportRepayAdvanceMoneyCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void ReportPatientSettlementCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.REPORT_PATIENT_SETTLEMENT;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportPatientSettlementCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1069_G1_BCQToan;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportPatientSettlementCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportPatientSettlementCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void ReportOutwardMedDeptInflowCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.REPORT_OUTWARD_MEDDEPT_INFLOW;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportOutwardMedDeptInflowCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0649_G1_BCThuTienBanThuoc;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportOutwardMedDeptInflowCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOutwardMedDeptInflowCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        //KMx: Báo cáo nhập xuất khoa (29/12/2014 15:05).
        private void ReportInPatientImportExportDepartmentCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.REPORT_IMPORT_EXPORT_DEPARTMENT;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;
            innerVm.isAllStaff = false;
            innerVm.mDepartment = true;
            innerVm.mInPatientDeptStatus = true;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportInPatientImportExportDepartmentCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1065_G1_BCNXKhoa;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportInPatientImportExportDepartmentCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportInPatientImportExportDepartmentCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        //Báo cáo BN đang điều trị
        private void ReportPatientAreTreatedCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.REPORT_PATIENT_ARE_TREATED;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;
            innerVm.isAllStaff = false;
            innerVm.mDepartment = true;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportPatientAreTreatedCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0651_G1_BCBNDangDTri;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportPatientAreTreatedCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportPatientAreTreatedCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        //KMx: Báo cáo BN chưa đóng tiền tạm ứng (18/01/2015 11:41).
        private void ReportInPatientNotPayCashAdvanceCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();

            innerVm.eItem = ReportName.REPORT_INPT_NOT_PAY_CASH_ADVANCE;
            innerVm.RptParameters.HideFindPatient = false;
            innerVm.mXemChiTiet = false;
            innerVm.mIn = false;
            innerVm.isAllStaff = false;
            innerVm.mDepartment = true;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }        

        public void ReportInPatientNotPayCashAdvanceCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0087_G1_BCBNChuaDongTU;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportInPatientNotPayCashAdvanceCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportInPatientNotPayCashAdvanceCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ReportInPatientDischargeNotPayAllBillCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;
            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.REPORT_INPT_NOT_PAY_ALL_BILL;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ReportInPatientDischargeNotPayAllBillCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0088_G1_BCBNXVConNoVPhi;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportInPatientDischargeNotPayAllBillCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportInPatientDischargeNotPayAllBillCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void SuggestAdvanceMoneyCmd_TV_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

                var regModule = Globals.GetViewModel<IRegistrationModule>();

            //KMx: Chuyển từ View ISuggestAdvanceMoney -> ISuggestCashAdvance (13/09/2014 17:02).
            //var vm = Globals.GetViewModel<ISuggestAdvanceMoney>();
            var vm = Globals.GetViewModel<ISuggestCashAdvance>();
            Globals.IsAdmission = true;
            vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            vm.UsedByTaiVuOffice = true;

            vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = vm;
            ((Conductor<object>)regModule).ActivateItem(vm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void SuggestAdvanceMoneyCmd_TV(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0089_G1_LapPhieuDNTU;

            
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                SuggestAdvanceMoneyCmd_TV_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuggestAdvanceMoneyCmd_TV_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void SuggestAdvanceMoneyCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNoi) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<ISuggestAdvanceMoney>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();

                //KMx: Chuyển từ View ISuggestAdvanceMoney -> ISuggestCashAdvance (13/09/2014 17:02).
                //var vm = Globals.GetViewModel<ISuggestAdvanceMoney>();
                var vm = Globals.GetViewModel<ISuggestCashAdvance>();
                Globals.IsAdmission = true;
                vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

                vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                regModule.MainContent = vm;
                ((Conductor<object>)regModule).ActivateItem(vm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void SuggestAdvanceMoneyCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0089_G1_LapPhieuDNTU;

            //  SuggestAdvanceMoneyCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                SuggestAdvanceMoneyCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuggestAdvanceMoneyCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void SuggestPaymentCmd_TV_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            //KMx: Test
            //var vm = Globals.GetViewModel<ISuggestAdvanceMoney>();
            var vm = Globals.GetViewModel<ISuggestPayment>();
            vm.UsedByTaiVuOffice = true;
            Globals.IsAdmission = true;
            vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

            vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = vm;
            ((Conductor<object>)regModule).ActivateItem(vm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        //KMx: SuggestPaymentCmd được copy từ SuggestAdvanceMoneyCmd. Lý do: Tách ra thành 2 view (12/09/2014 15:52).
        public void SuggestPaymentCmd_TV(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0090_G1_LapPhieuDNTToan;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                SuggestPaymentCmd_TV_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuggestPaymentCmd_TV_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void SuggestPaymentCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();

            //KMx: Test
            //var vm = Globals.GetViewModel<ISuggestAdvanceMoney>();
            var vm = Globals.GetViewModel<ISuggestPayment>();
            Globals.IsAdmission = true;
            vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

            vm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = vm;
            ((Conductor<object>)regModule).ActivateItem(vm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        //KMx: SuggestPaymentCmd được copy từ SuggestAdvanceMoneyCmd. Lý do: Tách ra thành 2 view (12/09/2014 15:52).
        public void SuggestPaymentCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0090_G1_LapPhieuDNTToan;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                SuggestPaymentCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SuggestPaymentCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        public void Handle(LocationSelected message)
        {
            if (message != null && message.DeptLocation != null)
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                if (message.ItemActivated == null)
                {
                    //var regVm = Globals.GetViewModel<IPatientRegistration>();

                    //regModule.MainContent = regVm;
                    //(regModule as Conductor<object>).ActivateItem(regVm); 
                }
                else
                {
                    regModule.MainContent = message.ItemActivated;
                    ((Conductor<object>)regModule).ActivateItem(message.ItemActivated);
                }
            }
        }

        public void TestTranPayCmd(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);

            var loginVm = Globals.GetViewModel<ILogin>();
            if (loginVm.DeptLocation == null) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var receiveVm = Globals.GetViewModel<ITestTransactionPayment>();

                regModule.MainContent = receiveVm;
                ((Conductor<object>)regModule).ActivateItem(receiveVm);
            }
        }
        private void KiemToanCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var receiveVm = Globals.GetViewModel<IKiemToan>();
            Globals.IsAdmission = null;
            regModule.MainContent = receiveVm;
            ((Conductor<object>)regModule).ActivateItem(receiveVm);
        }

        public void KiemToanCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2513_G1_KiemToan;

            // KiemToanCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                KiemToanCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        KiemToanCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void RegSummaryCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNgoaiTru) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNgoaiTru;
            //    locationVm.ItemActivated = Globals.GetViewModel<IRegistrationSummaryListing>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var innerVm = Globals.GetViewModel<IRegistrationSummaryListing>();
                Globals.IsAdmission = null;
                regModule.MainContent = innerVm;
                ((Conductor<object>)regModule).ActivateItem(innerVm);
            }
        }

        public void RegSummaryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1523_G1_TKet;

            //RegSummaryCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                RegSummaryCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RegSummaryCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ReportPaymentReceiptCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<IBaoCaoThuTien>();
            Globals.IsAdmission = null;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportPaymentReceiptCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1071_G1_BCThuTien;

            //ReportPaymentReceiptCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportPaymentReceiptCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportPaymentReceiptCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void InPatientProcessPaymentCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNoi) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<IInPatientProcessPayment>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var innerVm = Globals.GetViewModel<IInPatientProcessPayment>();
                Globals.IsAdmission = true;
                innerVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
                // HPT: nếu click đường dẫn tạo bill cho BN vãng lai và tiền giải phẫu thì kích hoạt tìm kiếm theo V_RegForPatientOfType bằng cách gán SearchByVregForPtOfType = 1
                // Ngược lại thì gán bằng 0
                // Giá trị thuộc tính này sẽ được sử dụng để mở thêm hoặc giấu đi các chức năng chuyên biệt khi viewmodel được gọi đến
                // Có thể truyền dữ liệu và điều khiển các chức năng đó ngay từ leftmenu nhưng anh Tuấn nói để tránh hiệu ứng domino. không nên truyền một biến qua nhiều ViewModel để điều khiển từ xa như vậy

                innerVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

                regModule.MainContent = innerVm;
                ((Conductor<object>)regModule).ActivateItem(innerVm);
            }
            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPatientProcessPaymentCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1317_G1_TinhTienNoiTru;
            
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPatientProcessPaymentCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPatientProcessPaymentCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void InPatientSettlementCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<IInPatientSettlement>();
            Globals.IsAdmission = true;
            innerVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            innerVm.UsedByTaiVuOffice = false;

            innerVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPatientSettlementCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0498_G1_QuyetToan;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPatientSettlementCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPatientSettlementCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void InPatientSettlementCmd_TV_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<IInPatientSettlement>();
            Globals.IsAdmission = true;
            innerVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
            innerVm.UsedByTaiVuOffice = true;

            innerVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void InPatientSettlementCmd_TV(object source)
        {
            Globals.TitleForm = eHCMSResources.Z0091_G1_QToanTVu;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                InPatientSettlementCmd_TV_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        InPatientSettlementCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void ReCalcHiCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;

            //var loginVm = Globals.GetViewModel<ILogin>();
            //if (loginVm.DeptLocation == null
            //    || V_DeptTypeOperation != V_DeptTypeOperation.KhoaNoi) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaNoi;
            //    locationVm.ItemActivated = Globals.GetViewModel<IInPatientRecalcHi>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();
                var innerVm = Globals.GetViewModel<IInPatientRecalcHi>();
                Globals.IsAdmission = true;
                innerVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;

                innerVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            

                regModule.MainContent = innerVm;
                ((Conductor<object>)regModule).ActivateItem(innerVm);
            }

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void ReCalcHiCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G2376_G1_XNhanLaiBHYTe;

            // ReCalcHiCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReCalcHiCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReCalcHiCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void ReCalcHiCmd_NgTr_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<IPatientRecalcHi>();
            Globals.IsAdmission = false;

            innerVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
           
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);

            //Hpt 27/11/2015: Không bắn sự kiện này nữa mà gán trực tiếp giá trị biến ở Globals
            //Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NGOAITRU });
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
        }

        public void ReCalcHiCmd_NgTr(object source)
        {
            Globals.TitleForm = string.Format("{0}: {1}", eHCMSResources.T3719_G1_Mau20NgTru, eHCMSResources.G2377_G1_XNhanLaiBHYT);

            // ReCalcHiCmd_In(source);

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReCalcHiCmd_NgTr_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReCalcHiCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void DanhSachDKBHYTCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.eItem = ReportName.DANHSACH_DANGKY_BHYT;
            innerVm.LoadListStaff((byte)StaffRegistrationType.INSURANCE);
            
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void DanhSachDKBHYTCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K2960_G1_DSDKBHYT;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                DanhSachDKBHYTCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DanhSachDKBHYTCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void ReportQuickConsultationCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<IBaoCaoNhanhKhuKhamBenh>();
            innerVm.eItem = ReportName.BAOCAONHANH_KHUKHAMBENH;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportQuickConsultationCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1062_G1_BCNhanhKhuKB;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportQuickConsultationCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportQuickConsultationCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void BangKeThuPhiXNTheoNgayCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.LoadListStaff((byte)StaffRegistrationType.NORMAL);
            innerVm.eItem = ReportName.BANGKETHUPHI_XN_THEONGAY;
            innerVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void BangKeThuPhiXNTheoNgayCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1044_G1_BKeThuPhiXN;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                BangKeThuPhiXNTheoNgayCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeThuPhiXNTheoNgayCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void BangKeThuPhiKB_CDHATheoNgayCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.mXemChiTiet = true;
            innerVm.LoadListStaff((byte)StaffRegistrationType.NORMAL);
            innerVm.eItem = ReportName.BANGLETHUPHI_KB_CDHA_THEONGAY;
            innerVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void BangKeThuPhiKB_CDHATheoNgayCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1043_G1_BKeThuPhiKBVaCDHA;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                BangKeThuPhiKB_CDHATheoNgayCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BangKeThuPhiKB_CDHATheoNgayCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }


        private void PhieuNopTienCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            innerVm.LoadListStaff((byte)StaffRegistrationType.NORMAL);
            innerVm.eItem = ReportName.PHIEUNOPTIEN;
            innerVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void PhieuNopTienCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.P0375_G1_PhNopTien;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                PhieuNopTienCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PhieuNopTienCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        //TongHopDoanhThuPKCmd

        private void TongHopDoanhThuPKCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<ICommonRptDoanhThuTongHop>();

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void TongHopDoanhThuPKCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G1519_G1_THopDThuPK;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                TongHopDoanhThuPKCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TongHopDoanhThuPKCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        //PCLExamTargetCmd
        private void PCLExamTargetCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.NONE;

            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var innerVm = Globals.GetViewModel<ICommonPCLExamTarget>();

            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void PCLExamTargetCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1059_G1_BCDSCLSSo;
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
        //GenericPaymentCmd
        private void GenericPaymentCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            LeftMenuByPTType = eLeftMenuByPTType.IN_PT;
          
            {
                var regModule = Globals.GetViewModel<IRegistrationModule>();

                var vm = Globals.GetViewModel<IGenericPayment>();
                Globals.IsAdmission = true;
                vm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;
                regModule.MainContent = vm;
                ((Conductor<object>)regModule).ActivateItem(vm);
            }
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void GenericPaymentCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.G0735_G1_ThuTienKhac;
            SetHyperlinkSelectedStyle(source as Button);
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                GenericPaymentCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        GenericPaymentCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ReportGenericPaymentCmd_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            LeftMenuByPTType = eLeftMenuByPTType.NONE;
            var regModule = Globals.GetViewModel<IRegistrationModule>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BAOCAO_THUTIEN_KHAC;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = true;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        public void ReportGenericPaymentCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1076_G1_BCThuTienKhac;

            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportGenericPaymentCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportGenericPaymentCmd_In(source);
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }        
    }
}


//// TxD: Command Nhan benh Cap Cuu Khong BHYT
//public void ReceiveEmergencyPatientCmd(object source)
//{
//    Globals.TitleForm = eHCMSResources.N0184_G1_NhanBenhCC.ToUpper();

//    if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
//    {
//        ProcReceiveEmergencyPatient(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                ProcReceiveEmergencyPatient(source);
//                GlobalsNAV.msgb = null;
//                Globals.HIRegistrationForm = "";
//            }
//        });
//    }

//}

//// TxD: Process Nhan benh Cap Cuu Khong BHYT
//private void ProcReceiveEmergencyPatient(object source)
//{
//    Globals.IsAdmission = null;
//    SetHyperlinkSelectedStyle(source as HyperlinkButton);
//    Globals.PageName = Globals.TitleForm;

//    var receiveVm = Globals.GetViewModel<IReceivePatient>();
//    receiveVm.RegistrationType = AllLookupValues.RegistrationType.CAP_CUU;
//    receiveVm.IsEmergency = true;
//    receiveVm.PageTitle = eHCMSResources.N0184_G1_NhanBenhCC.ToUpper();
//    receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;


//    receiveVm.mNhanBenh_ThongTin_Sua = mNhanBenhNoiTru_ThongTin_Sua;
//    receiveVm.mNhanBenh_TheBH_ThemMoi = mNhanBenhNoiTru_TheBH_ThemMoi;
//    receiveVm.mNhanBenh_TheBH_XacNhan = mNhanBenhNoiTru_TheBH_XacNhan;
//    receiveVm.mNhanBenh_DangKy = mNhanBenhNoiTru_DangKy;
//    receiveVm.mNhanBenh_TheBH_Sua = mNhanBenhNoiTru_TheBH_Sua;

//    receiveVm.mPatient_TimBN = mNhanBenhNoiTru_Patient_TimBN;
//    receiveVm.mPatient_ThemBN = mNhanBenhNoiTru_Patient_ThemBN;
//    receiveVm.mPatient_TimDangKy = mNhanBenhNoiTru_Patient_TimDangKy;

//    receiveVm.mInfo_CapNhatThongTinBN = mNhanBenhNoiTru_Info_CapNhatThongTinBN;
//    receiveVm.mInfo_XacNhan = mNhanBenhNoiTru_Info_XacNhan;
//    receiveVm.mInfo_XoaThe = mNhanBenhNoiTru_Info_XoaThe;
//    receiveVm.mInfo_XemPhongKham = mNhanBenhNoiTru_Info_XemPhongKham;

//    var regModule = Globals.GetViewModel<IRegistrationModule>();
//    regModule.MainContent = receiveVm;
//    ((Conductor<object>)regModule).ActivateItem(receiveVm);

//    Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });

//}

//// TxD: Command Nhan benh Cap Cuu Co BHYT
//public void ReceiveEmergencyPatient_WithHI_Cmd(object source)
//{
//    Globals.TitleForm = eHCMSResources.N0185_G1_NhanBenhCC_BH.ToUpper();

//    if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
//    {
//        ProcReceiveEmergencyPatient_WithHI(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                ProcReceiveEmergencyPatient_WithHI(source);
//                GlobalsNAV.msgb = null;
//                Globals.HIRegistrationForm = "";
//            }
//        });
//    }

//}

//// TxD: Process Nhan benh Cap Cuu Co BHYT
//private void ProcReceiveEmergencyPatient_WithHI(object source)
//{
//    Globals.IsAdmission = null;
//    SetHyperlinkSelectedStyle(source as HyperlinkButton);
//    Globals.PageName = Globals.TitleForm;

//    var receiveVm = Globals.GetViewModel<IReceivePatient>();
//    receiveVm.RegistrationType = AllLookupValues.RegistrationType.CAP_CUU_BHYT;
//    receiveVm.IsEmergency = true;
//    receiveVm.PageTitle = eHCMSResources.N0185_G1_NhanBenhCC_BH.ToUpper();
//    receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;


//    receiveVm.mNhanBenh_ThongTin_Sua = mNhanBenhNoiTru_ThongTin_Sua;
//    receiveVm.mNhanBenh_TheBH_ThemMoi = mNhanBenhNoiTru_TheBH_ThemMoi;
//    receiveVm.mNhanBenh_TheBH_XacNhan = mNhanBenhNoiTru_TheBH_XacNhan;
//    receiveVm.mNhanBenh_DangKy = mNhanBenhNoiTru_DangKy;
//    receiveVm.mNhanBenh_TheBH_Sua = mNhanBenhNoiTru_TheBH_Sua;

//    receiveVm.mPatient_TimBN = mNhanBenhNoiTru_Patient_TimBN;
//    receiveVm.mPatient_ThemBN = mNhanBenhNoiTru_Patient_ThemBN;
//    receiveVm.mPatient_TimDangKy = mNhanBenhNoiTru_Patient_TimDangKy;

//    receiveVm.mInfo_CapNhatThongTinBN = mNhanBenhNoiTru_Info_CapNhatThongTinBN;
//    receiveVm.mInfo_XacNhan = mNhanBenhNoiTru_Info_XacNhan;
//    receiveVm.mInfo_XoaThe = mNhanBenhNoiTru_Info_XoaThe;
//    receiveVm.mInfo_XemPhongKham = mNhanBenhNoiTru_Info_XemPhongKham;

//    var regModule = Globals.GetViewModel<IRegistrationModule>();
//    regModule.MainContent = receiveVm;
//    ((Conductor<object>)regModule).ActivateItem(receiveVm);

//    Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
//}


//// TxD: Command Nhan Benh Noi Tru khong BHYT
//public void ReceiveInPatientCmd(object source)
//{
//    Globals.TitleForm = eHCMSResources.N0187_G1_NhanBenhNT;
            
//    if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
//    {
//        ProcReceiveInPatient(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                ProcReceiveInPatient(source);
//                GlobalsNAV.msgb = null;
//                Globals.HIRegistrationForm = "";
//            }
//        });
//    }
//}

//// TxD: Process Nhan Benh Noi Tru khong BHYT
//private void ProcReceiveInPatient(object source)
//{
//    SetHyperlinkSelectedStyle(source as HyperlinkButton);
//    Globals.PageName = Globals.TitleForm;

//    //var loginVm = Globals.GetViewModel<ILogin>();
//    var receiveVm = Globals.GetViewModel<IReceivePatient>();
//    receiveVm.RegistrationType = AllLookupValues.RegistrationType.NOI_TRU;
//    receiveVm.PageTitle = "  NHẬN BỆNH NỘI TRÚ";
//    receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;


//    receiveVm.mNhanBenh_ThongTin_Sua = mNhanBenhNoiTru_ThongTin_Sua;
//    receiveVm.mNhanBenh_TheBH_ThemMoi = mNhanBenhNoiTru_TheBH_ThemMoi;
//    receiveVm.mNhanBenh_TheBH_XacNhan = mNhanBenhNoiTru_TheBH_XacNhan;
//    receiveVm.mNhanBenh_DangKy = mNhanBenhNoiTru_DangKy;
//    receiveVm.mNhanBenh_TheBH_Sua = mNhanBenhNoiTru_TheBH_Sua;

//    receiveVm.mPatient_TimBN = mNhanBenhNoiTru_Patient_TimBN;
//    receiveVm.mPatient_ThemBN = mNhanBenhNoiTru_Patient_ThemBN;
//    receiveVm.mPatient_TimDangKy = mNhanBenhNoiTru_Patient_TimDangKy;

//    receiveVm.mInfo_CapNhatThongTinBN = mNhanBenhNoiTru_Info_CapNhatThongTinBN;
//    receiveVm.mInfo_XacNhan = mNhanBenhNoiTru_Info_XacNhan;
//    receiveVm.mInfo_XoaThe = mNhanBenhNoiTru_Info_XoaThe;
//    receiveVm.mInfo_XemPhongKham = mNhanBenhNoiTru_Info_XemPhongKham;

//    var regModule = Globals.GetViewModel<IRegistrationModule>();
//    regModule.MainContent = receiveVm;
//    ((Conductor<object>)regModule).ActivateItem(receiveVm);

//    Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
//}

//// TxD: Command Nhan Benh Noi Tru Co BHYT
//public void ReceiveInPatient_WithHI_Cmd(object source)
//{
//    Globals.TitleForm = eHCMSResources.A1154_G1_NBNoiTruBHYT;

//    if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
//    {
//        ProcReceiveInPatient_WithHI(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                ProcReceiveInPatient_WithHI(source);
//                GlobalsNAV.msgb = null;
//                Globals.HIRegistrationForm = "";
//            }
//        });
//    }
//}

//// TxD: Process Nhan Benh Noi Tru Co BHYT
//private void ProcReceiveInPatient_WithHI(object source)
//{
//    SetHyperlinkSelectedStyle(source as HyperlinkButton);
//    Globals.PageName = Globals.TitleForm;

//    //var loginVm = Globals.GetViewModel<ILogin>();
//    var receiveVm = Globals.GetViewModel<IReceivePatient>();
//    receiveVm.RegistrationType = AllLookupValues.RegistrationType.NOI_TRU_BHYT;
//    receiveVm.PageTitle = "  NHẬN BỆNH NỘI TRÚ - BHYT";
//    receiveVm.DeptLocTitle = Globals.ObjRefDepartment.DeptName;


//    receiveVm.mNhanBenh_ThongTin_Sua = mNhanBenhNoiTru_ThongTin_Sua;
//    receiveVm.mNhanBenh_TheBH_ThemMoi = mNhanBenhNoiTru_TheBH_ThemMoi;
//    receiveVm.mNhanBenh_TheBH_XacNhan = mNhanBenhNoiTru_TheBH_XacNhan;
//    receiveVm.mNhanBenh_DangKy = mNhanBenhNoiTru_DangKy;
//    receiveVm.mNhanBenh_TheBH_Sua = mNhanBenhNoiTru_TheBH_Sua;

//    receiveVm.mPatient_TimBN = mNhanBenhNoiTru_Patient_TimBN;
//    receiveVm.mPatient_ThemBN = mNhanBenhNoiTru_Patient_ThemBN;
//    receiveVm.mPatient_TimDangKy = mNhanBenhNoiTru_Patient_TimDangKy;

//    receiveVm.mInfo_CapNhatThongTinBN = mNhanBenhNoiTru_Info_CapNhatThongTinBN;
//    receiveVm.mInfo_XacNhan = mNhanBenhNoiTru_Info_XacNhan;
//    receiveVm.mInfo_XoaThe = mNhanBenhNoiTru_Info_XoaThe;
//    receiveVm.mInfo_XemPhongKham = mNhanBenhNoiTru_Info_XemPhongKham;

//    var regModule = Globals.GetViewModel<IRegistrationModule>();
//    regModule.MainContent = receiveVm;
//    ((Conductor<object>)regModule).ActivateItem(receiveVm);

//    Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
//}

//private void RegisRoomManageCmd_In(object source)
//{
//    SetHyperlinkSelectedStyle(source as HyperlinkButton);
//    Globals.PageName = Globals.TitleForm;

//    {
//        var regModule = Globals.GetViewModel<IRegistrationModule>();
//        var vm = Globals.GetViewModel<IRegisRoomManage>();
//        Globals.IsAdmission = true;
//        regModule.MainContent = vm;
//        ((Conductor<object>)regModule).ActivateItem(vm);
//    }
//    Globals.EventAggregator.Publish(new PatientFindByChange { patientFindBy = AllLookupValues.PatientFindBy.NOITRU });
//}

//public void RegisRoomManageCmd(object source)
//{
//    Globals.TitleForm = "Bảng Kê Phòng Khám";

//    // AdvanceMoneyCmd_In(source);

//    if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
//    {
//        RegisRoomManageCmd_In(source);
//    }
//    else if (Globals.PageName != Globals.TitleForm)
//    {
//        Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
//        {
//            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
//            {
//                RegisRoomManageCmd_In(source);
//            }
//        });
//    }
//}