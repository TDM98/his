using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts.Configuration;
using Castle.Windsor;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ITransactionLeftMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TransactionLeftMenuViewModel : Conductor<object>, ITransactionLeftMenu
    {
        [ImportingConstructor]
        public TransactionLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            authorization();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            else
            {
                #region Authorization of Báo cáo phân tích tài chính

                bTemp20NgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp20NgoaiTru);
                bTemp20NoiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp20NoiTru);
                bTemp21NgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp21NgoaiTru);
                bTemp21NoiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp21NoiTru);
                bTemp25aInsurance = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp25aInsurance);
                bTemp26a = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp26a);
                bTemp19 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mTemp19);
                bTemp20NoiTruNew = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp20NoiTruNew);
                bTemp21New = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp21New);
                bTemp79a = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mTemp79a);
                bTemp79aTraThuoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mTemp79aTraThuoc);
                bTemp80a = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp80a);

                bTemp01_BV_NgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp01_BV_NgoaiTru);
                bTemp02_BV_NoiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp02_BV_NoiTru);
                bTemp02_ChiTietVienPhi_PK = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp02_ChiTietVienPhi_PK);
                bTemp04_ChiTietVienPhi = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp04_ChiTietVienPhi);
                bTemp02_HoatDongKB = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp02_HoatDongKB);
                bTemp06_CanLamSan = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp06_CanLamSan);
                bTemp07_DuocBV = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTemp07_DuocBV);
                bChiDaoTuyen = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mChiDaoTuyen);
                bBC15Day = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mBC15Day);
                bSXN = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mSXN);
                bNghienCuuKhoaHoc = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mNghienCuuKhoaHoc);
                bTempThongKeDoanhThu = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTempThongKeDoanhThu);
                bTempTongHopDoanhThu_PK = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTempTongHopDoanhThu_PK);
                bTempTongHopDoanhThu_NTM = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                  (int)eTransaction_Management.mTempTongHopDoanhThu_NTM);
                bBaoCaoBHYT = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mBaoCaoBHYT);
                bBaoCaoVienPhiTrai = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mBaoCaoVienPhiTrai);
                bBaoCaoVienPhiKTC = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                    (int)eTransaction_Management.mBaoCaoVienPhiKTC);
                bBaoCaoVTYT_KTC = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    (int)eTransaction_Management.mBaoCaoVTYT_KTC);



                //KMx: Chỉ cần 1 Function của Group "Báo cáo" được cấp quyền sử dụng thì Group "Báo cáo" sẽ hiện ra.
                mReport = bTemp20NgoaiTru || bTemp20NoiTru || bTemp19 || bTemp20NoiTruNew || bTemp21NgoaiTru || bTemp21NoiTru || bTemp25aInsurance || bTemp26a
                            || bTemp79a || bTemp79aTraThuoc || bTemp80a || bTemp02_HoatDongKB || bTemp06_CanLamSan || bTemp07_DuocBV
                          || bTemp01_BV_NgoaiTru || bTemp02_BV_NoiTru || bTemp02_ChiTietVienPhi_PK || bTemp04_ChiTietVienPhi
                          || bTempThongKeDoanhThu || bTempTongHopDoanhThu_PK || bTempTongHopDoanhThu_NTM || bBaoCaoBHYT || bBaoCaoVienPhiTrai || bBaoCaoVienPhiKTC || bBaoCaoVTYT_KTC;

                #endregion

                #region Authorization of Price Management

                bMedServiceItemPriceList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                                (int)eTransaction_Management.mBangGiaDichVu);
                bPCLExamTypePriceList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                                (int)eTransaction_Management.mBangGiaCLS);
                bRefDepartmentReqCashAdv_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                                (int)eTransaction_Management.mGiaTamUng);

                mPriceManagement = bMedServiceItemPriceList_Mgnt || bPCLExamTypePriceList_Mgnt || bRefDepartmentReqCashAdv_Mgnt;
                #endregion

                #region Authorization of Y Vụ

                bThongKeNgoaiTru = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                  (int)eYVu_Management.mThongKeNgoaiTru);
                bBaoCaoNhanhKhuKhamBenh = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                  (int)eYVu_Management.mBaoCaoNhanhKhuKhamBenh);
                bReportInPatient = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                  (int)eYVu_Management.mReportInPatient);
                bReportGeneralTemp02 = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mYVu_Management,
                                    (int)eYVu_Management.mReportGeneralTemp02);
                mYVuReport = bThongKeNgoaiTru || bBaoCaoNhanhKhuKhamBenh || bReportInPatient || bReportGeneralTemp02;
                #endregion
            }

        }

        #region Properties of Báo cáo phân tích tài chính

        private bool _mReport = true;
        private bool _bTemp20NgoaiTru = true;
        private bool _bTemp20NoiTru = true;
        private bool _bTemp21NgoaiTru = true;
        private bool _bTemp21NoiTru = true;
        private bool _bTemp25aInsurance = true;
        private bool _bTemp26a = true;
        private bool _bTemp19 = true;
        private bool _bTemp20NoiTruNew = true;
        private bool _bTemp21New = true;
        private bool _bTemp79a = true;
        private bool _bTemp79aTraThuoc = true;
        private bool _bTemp80a = true;
        private bool _bTemp01_BV_NgoaiTru = true;
        private bool _bTemp02_BV_NoiTru = true;
        private bool _bTemp02_ChiTietVienPhi_PK = true;
        private bool _bTemp04_ChiTietVienPhi = true;
        private bool _bTemp02_HoatDongKB = true;
        private bool _bTemp06_CanLamSan = true;
        private bool _bTemp07_DuocBV = true;
        private bool _bChiDaoTuyen = true;
        private bool _bBC15Day = true;
        private bool _bSXN = true;
        private bool _bNghienCuuKhoaHoc = true;
        private bool _bTempThongKeDoanhThu = true;
        private bool _bTempTongHopDoanhThu_PK = true;
        private bool _bTempTongHopDoanhThu_NTM = true;
        private bool _bBaoCaoBHYT = true;
        private bool _bBaoCaoVienPhiTrai = true;
        private bool _bBaoCaoVienPhiKTC = true;
        private bool _bBaoCaoVTYT_KTC = true;

        public bool bTemp02_HoatDongKB
        {
            get
            {
                return _bTemp02_HoatDongKB;
            }
            set
            {
                if (_bTemp02_HoatDongKB == value)
                    return;
                _bTemp02_HoatDongKB = value;
                NotifyOfPropertyChange(() => _bTemp02_HoatDongKB);
            }
        }
        public bool mReport
        {
            get
            {
                return _mReport;
            }
            set
            {
                if (_mReport == value)
                    return;
                _mReport = value;
                NotifyOfPropertyChange(() => mReport);
            }
        }

        public bool bTemp20NgoaiTru
        {
            get
            {
                return _bTemp20NgoaiTru;
            }
            set
            {
                if (_bTemp20NgoaiTru == value)
                    return;
                _bTemp20NgoaiTru = value;
            }
        }

        public bool bTemp20NoiTru
        {
            get
            {
                return _bTemp20NoiTru;
            }
            set
            {
                if (_bTemp20NoiTru == value)
                    return;
                _bTemp20NoiTru = value;
            }
        }

        public bool bTemp21NgoaiTru
        {
            get
            {
                return _bTemp21NgoaiTru;
            }
            set
            {
                if (_bTemp21NgoaiTru == value)
                    return;
                _bTemp21NgoaiTru = value;
            }
        }

        public bool bTemp21NoiTru
        {
            get
            {
                return _bTemp21NoiTru;
            }
            set
            {
                if (_bTemp21NoiTru == value)
                    return;
                _bTemp21NoiTru = value;
            }
        }

        public bool bTemp25aInsurance
        {
            get
            {
                return _bTemp25aInsurance;
            }
            set
            {
                if (_bTemp25aInsurance == value)
                    return;
                _bTemp25aInsurance = value;
            }
        }

        public bool bTemp26a
        {
            get
            {
                return _bTemp26a;
            }
            set
            {
                if (_bTemp26a == value)
                    return;
                _bTemp26a = value;
            }
        }

        public bool bTemp19
        {
            get
            {
                return _bTemp19;
            }
            set
            {
                if (_bTemp19 == value)
                    return;
                _bTemp19 = value;
            }
        }

        public bool bTemp20NoiTruNew
        {
            get
            {
                return _bTemp20NoiTruNew;
            }
            set
            {
                if (_bTemp20NoiTruNew == value)
                    return;
                _bTemp20NoiTruNew = value;
            }
        }

        public bool bTemp21New
        {
            get
            {
                return _bTemp21New;
            }
            set
            {
                if (_bTemp21New == value)
                    return;
                _bTemp21New = value;
            }
        }

        public bool bTemp79a
        {
            get
            {
                return _bTemp79a;
            }
            set
            {
                if (_bTemp79a == value)
                    return;
                _bTemp79a = value;
            }
        }

        public bool bTemp79aTraThuoc
        {
            get
            {
                return _bTemp79aTraThuoc;
            }
            set
            {
                if (_bTemp79aTraThuoc == value)
                    return;
                _bTemp79aTraThuoc = value;
            }
        }

        public bool bTemp80a
        {
            get
            {
                return _bTemp80a;
            }
            set
            {
                if (_bTemp80a == value)
                    return;
                _bTemp80a = value;
            }
        }

        public bool bTemp01_BV_NgoaiTru
        {
            get
            {
                return _bTemp01_BV_NgoaiTru;
            }
            set
            {
                if (_bTemp01_BV_NgoaiTru == value)
                    return;
                _bTemp01_BV_NgoaiTru = value;
            }
        }

        public bool bTemp02_BV_NoiTru
        {
            get
            {
                return _bTemp02_BV_NoiTru;
            }
            set
            {
                if (_bTemp02_BV_NoiTru == value)
                    return;
                _bTemp02_BV_NoiTru = value;
            }
        }

        public bool bTemp02_ChiTietVienPhi_PK
        {
            get
            {
                return _bTemp02_ChiTietVienPhi_PK;
            }
            set
            {
                if (_bTemp02_ChiTietVienPhi_PK == value)
                    return;
                _bTemp02_ChiTietVienPhi_PK = value;
            }
        }

        public bool bTemp04_ChiTietVienPhi
        {
            get
            {
                return _bTemp04_ChiTietVienPhi;
            }
            set
            {
                if (_bTemp04_ChiTietVienPhi == value)
                    return;
                _bTemp04_ChiTietVienPhi = value;
            }
        }
        public bool bTemp06_CanLamSan
        {
            get
            {
                return _bTemp06_CanLamSan;
            }
            set
            {
                if (_bTemp06_CanLamSan == value)
                    return;
                _bTemp06_CanLamSan = value;
            }
        }
        public bool bTemp07_DuocBV
        {
            get
            {
                return _bTemp07_DuocBV;
            }
            set
            {
                if (_bTemp07_DuocBV == value)
                    return;
                _bTemp07_DuocBV = value;
            }
        }
        public bool bBC15Day
        {
            get
            {
                return _bBC15Day;
            }
            set
            {
                if (_bBC15Day == value)
                    return;
                _bBC15Day = value;
            }
        }
        public bool bSXN
        {
            get
            {
                return _bSXN;
            }
            set
            {
                if (_bSXN == value)
                    return;
                _bSXN = value;
            }
        }
        public bool bChiDaoTuyen
        {
            get
            {
                return _bChiDaoTuyen;
            }
            set
            {
                if (_bChiDaoTuyen == value)
                    return;
                _bChiDaoTuyen = value;
            }
        }
        public bool bNghienCuuKhoaHoc
        {
            get
            {
                return _bNghienCuuKhoaHoc;
            }
            set
            {
                if (_bNghienCuuKhoaHoc == value)
                    return;
                _bNghienCuuKhoaHoc = value;
            }
        }
        public bool bTempThongKeDoanhThu
        {
            get
            {
                return _bTempThongKeDoanhThu;
            }
            set
            {
                if (_bTempThongKeDoanhThu == value)
                    return;
                _bTempThongKeDoanhThu = value;
            }
        }

        public bool bTempTongHopDoanhThu_PK
        {
            get
            {
                return _bTempTongHopDoanhThu_PK;
            }
            set
            {
                if (_bTempTongHopDoanhThu_PK == value)
                    return;
                _bTempTongHopDoanhThu_PK = value;
            }
        }

        public bool bTempTongHopDoanhThu_NTM
        {
            get
            {
                return _bTempTongHopDoanhThu_NTM;
            }
            set
            {
                if (_bTempTongHopDoanhThu_NTM == value)
                    return;
                _bTempTongHopDoanhThu_NTM = value;
            }
        }

        public bool bBaoCaoBHYT
        {
            get
            {
                return _bBaoCaoBHYT;
            }
            set
            {
                if (_bBaoCaoBHYT == value)
                    return;
                _bBaoCaoBHYT = value;
            }
        }

        public bool bBaoCaoVienPhiTrai
        {
            get
            {
                return _bBaoCaoVienPhiTrai;
            }
            set
            {
                if (_bBaoCaoVienPhiTrai == value)
                    return;
                _bBaoCaoVienPhiTrai = value;
            }
        }

        public bool bBaoCaoVienPhiKTC
        {
            get
            {
                return _bBaoCaoVienPhiKTC;
            }
            set
            {
                if (_bBaoCaoVienPhiKTC == value)
                    return;
                _bBaoCaoVienPhiKTC = value;
            }
        }

        public bool bBaoCaoVTYT_KTC
        {
            get
            {
                return _bBaoCaoVTYT_KTC;
            }
            set
            {
                if (_bBaoCaoVTYT_KTC == value)
                    return;
                _bBaoCaoVTYT_KTC = value;
            }
        }
        #endregion

        #region Properties of Price Management

        private bool _mPriceManagement = true;
        private bool _bMedServiceItemPriceList_Mgnt = true;
        private bool _bPCLExamTypePriceList_Mgnt = true;
        private bool _bRefDepartmentReqCashAdv_Mgnt = true;

        public bool mPriceManagement
        {
            get
            {
                return _mPriceManagement;
            }
            set
            {
                if (_mPriceManagement == value)
                    return;
                _mPriceManagement = value;
                NotifyOfPropertyChange(() => mPriceManagement);
            }
        }

        public bool bMedServiceItemPriceList_Mgnt
        {
            get
            {
                return _bMedServiceItemPriceList_Mgnt;
            }
            set
            {
                if (_bMedServiceItemPriceList_Mgnt == value)
                    return;
                _bMedServiceItemPriceList_Mgnt = value;
                NotifyOfPropertyChange(() => bMedServiceItemPriceList_Mgnt);
            }
        }

        public bool bPCLExamTypePriceList_Mgnt
        {
            get
            {
                return _bPCLExamTypePriceList_Mgnt;
            }
            set
            {
                if (_bPCLExamTypePriceList_Mgnt == value)
                    return;
                _bPCLExamTypePriceList_Mgnt = value;
                NotifyOfPropertyChange(() => bPCLExamTypePriceList_Mgnt);
            }
        }

        public bool bRefDepartmentReqCashAdv_Mgnt
        {
            get
            {
                return _bRefDepartmentReqCashAdv_Mgnt;
            }
            set
            {
                if (_bRefDepartmentReqCashAdv_Mgnt == value)
                    return;
                _bRefDepartmentReqCashAdv_Mgnt = value;
                NotifyOfPropertyChange(() => bRefDepartmentReqCashAdv_Mgnt);
            }
        }

        #endregion


        #region Properties of Y Vụ

        private bool _mYVuReport = true;
        private bool _bThongKeNgoaiTru = true;
        private bool _bBaoCaoNhanhKhuKhamBenh = true;
        private bool _bReportInPatient = true;
        private bool _bReportGeneralTemp02 = true;

        public bool mYVuReport
        {
            get
            {
                return _mYVuReport;
            }
            set
            {
                if (_mYVuReport == value)
                    return;
                _mYVuReport = value;
                NotifyOfPropertyChange(() => mYVuReport);
            }
        }

        public bool bThongKeNgoaiTru
        {
            get
            {
                return _bThongKeNgoaiTru;
            }
            set
            {
                if (_bThongKeNgoaiTru == value)
                    return;
                _bThongKeNgoaiTru = value;
                NotifyOfPropertyChange(() => bThongKeNgoaiTru);
            }
        }

        public bool bBaoCaoNhanhKhuKhamBenh
        {
            get
            {
                return _bBaoCaoNhanhKhuKhamBenh;
            }
            set
            {
                if (_bBaoCaoNhanhKhuKhamBenh == value)
                    return;
                _bBaoCaoNhanhKhuKhamBenh = value;
                NotifyOfPropertyChange(() => bBaoCaoNhanhKhuKhamBenh);
            }
        }

        public bool bReportInPatient
        {
            get
            {
                return _bReportInPatient;
            }
            set
            {
                if (_bReportInPatient == value)
                    return;
                _bReportInPatient = value;
                NotifyOfPropertyChange(() => bReportInPatient);
            }
        }

        public bool bReportGeneralTemp02
        {
            get
            {
                return _bReportGeneralTemp02;
            }
            set
            {
                if (_bReportGeneralTemp02 == value)
                    return;
                _bReportGeneralTemp02 = value;
                NotifyOfPropertyChange(() => bReportGeneralTemp02);
            }
        }
        #endregion

        private void Temp20NgoaiTruCmd_In()
        {
            resetMenuColor();
            Temp20NgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NGOAITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NgoaiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1217_G1_Mau20NgTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NgoaiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NgoaiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20NgoaiTruTraThuocCmd_In()
        {
            resetMenuColor();
            Temp20NgoaiTruTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NGOAITRU_TRATHUOC;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            //KMX: Nếu có làm phân quyền cho mẫu 20 trả thuốc thì phải sửa lại Enum
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NgoaiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NgoaiTruTraThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1218_G1_Mau20NgTruTraThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NgoaiTruTraThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NgoaiTruTraThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20NoiTruCmd_In()
        {
            resetMenuColor();
            Temp20NoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NOITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NoiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1219_G1_Mau20NoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20VTYTTHCmd_In()
        {
            resetMenuColor();
            Temp20VTYTTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_VTYTTH;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NoiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20VTYTTHCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1220_G1_Mau20VTYTTieuHao.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20VTYTTHCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20VTYTTHCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp21NgoaiTruCmd_In()
        {
            resetMenuColor();
            Temp21NgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP21_NGOAITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp21NgoaiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp21NgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1221_G1_Mau21_NgTru.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp21NgoaiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp21NgoaiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Temp21NoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1222_G1_Mau21NoiTru;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp21NoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp21NoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Temp21NoiTruCmd_In()
        {
            resetMenuColor();
            Temp21NoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP21_NOITRU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp21NoiTru;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void Temp25aCmd_In()
        {
            resetMenuColor();
            Temp25a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP25a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp25aInsurance;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp25aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3728_G1_Mau25A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp25aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp25aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp25aTraThuocCmd_In()
        {
            resetMenuColor();
            Temp25aTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP25aTRATHUOC_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            //KMX: Nếu có làm phân quyền cho mẫu 25 trả thuốc thì phải sửa lại Enum
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp25aInsurance;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp25aTraThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.T3729_G1_Mau25ATraThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp25aTraThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp25aTraThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp26aCmd_In()
        {
            resetMenuColor();
            Temp26a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP26a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp26a;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp26aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3730_G1_Mau26A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp26aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp26aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp19Cmd_In()
        {
            resetMenuColor();
            Temp19.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP19;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp19;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp19Cmd()
        {
            Globals.TitleForm = eHCMSResources.G0023_G1_Mau19;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp19Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp19Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp20NoiTruNewCmd_In()
        {
            resetMenuColor();
            Temp20NoiTruNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP20_NOITRU_NEW;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp20NoiTruNew;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp20NoiTruNewCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1223_G1_Mau20NoiTruNew.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp20NoiTruNewCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp20NoiTruNewCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        public void Temp21NewCmd()
        {
            Globals.TitleForm = eHCMSResources.G0025_G1_Mau21;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp21NewCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp21NewCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void Temp21NewCmd_In()
        {
            resetMenuColor();
            Temp21New.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP21_NEW;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp21New;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }



        private void Temp79aCmd_In()
        {
            resetMenuColor();
            Temp79a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP79a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp79a;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp79aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3733_G1_Mau79A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp79aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp79aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void Temp79aTraThuocCmd_In()
        {
            resetMenuColor();
            Temp79aTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP79aTRATHUOC_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp79aTraThuoc;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp79aTraThuocCmd()
        {
            Globals.TitleForm = eHCMSResources.T3734_G1_Mau79ATraThuoc;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp79aTraThuocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp79aTraThuocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void Temp80aCmd_In()
        {
            resetMenuColor();
            Temp80a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TEMP80a_CHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp26a;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp80aCmd()
        {
            Globals.TitleForm = eHCMSResources.T3736_G1_Mau80A;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp80aCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp80aCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp38aNgoaiTruCmd_In()
        {
            resetMenuColor();
            Temp38aNgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp38NgoaiTru>();
            UnitVM.IsNgoaiTru = true;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp38aNgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1224_G1_Mau01BVNgTru.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp38aNgoaiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp38aNgoaiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Temp38aNoiTruCmd_In()
        {
            resetMenuColor();
            Temp38aNoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp38NoiTru>();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void Temp38aNoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1225_G1_Mau02BVNoiTru.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Temp38aNoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Temp38aNoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ChiTietVienPhiPKCmd_In()
        {
            resetMenuColor();
            ChiTietVienPhiPK.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TRANSACTION_VIENPHICHITIET_PK;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02_ChiTietVienPhi_PK;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ChiTietVienPhiPKCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1216_G1_CTietVPhiPK.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ChiTietVienPhiPKCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ChiTietVienPhiPKCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ChiTietVienPhiCmd_In()
        {
            resetMenuColor();
            ChiTietVienPhi.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TRANSACTION_VIENPHICHITIET;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp04_ChiTietVienPhi;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ChiTietVienPhiCmd()
        {
            Globals.TitleForm = eHCMSResources.T3715_G1_Mau04CTietVPhi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ChiTietVienPhiCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ChiTietVienPhiCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        // mẫu 02 - Hoạt động khám bệnh -TMA
        private void HoatDongKBCmd_In()
        {
            resetMenuColor();
            HoatDongKB.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TRANSACTION_HOATDONGKB;
            UnitVM.RptParameters.ShowFirst = "";
            // UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02_HoatDongKB;
            // UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }


        public void HoatDongKBCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1937_G1_HDongKB;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoatDongKBCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoatDongKBCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        // mẫu 06 - CLS -TMA
        private void CanLamSanCmd_In()
        {
            resetMenuColor();
            CanLamSan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TRANSACTION_CANLAMSAN;
            UnitVM.RptParameters.ShowFirst = "";
            // UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp06_CanLamSan;
            // UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }


        public void CanLamSanCmd()
        {
            Globals.TitleForm = eHCMSResources.T3716_G1_Mau06CanLamSan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                CanLamSanCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        CanLamSanCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //
        // mẫu 07 - Duoc BV -TMA
        private void DuocBVCmd_In()
        {
            resetMenuColor();
            DuocBV.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TRANSACTION_DUOCBV;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp07_DuocBV;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }


        public void DuocBVCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1942_G1_DuocBV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DuocBVCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DuocBVCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //

        // PHỤ LỤC 2B - TransferFormType2Rpt -TMA : 06/10/2017
        private void TransferFormType2RptCmd_In()
        {
            resetMenuColor();
            TransferFormType2Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TransferFormType2Rpt;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02b_TransferFormType2Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

            //
        }


        public void TransferFormType2RptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2131_G1_TransferFormType2Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormType2RptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormType2RptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //

        // PHỤ LỤC 2A - TransferFormType2_1Rpt -TMA : 09/10/2017
        private void TransferFormType2_1RptCmd_In()
        {
            resetMenuColor();
            TransferFormType2_1Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TransferFormType2_1Rpt;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp02a_TransferFormType2_1Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

            //
        }


        public void TransferFormType2_1RptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2132_G1_TransferFormType2_1Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormType2_1RptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormType2_1RptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //

        // PHỤ LỤC 5 - TransferFormType5Rpt -TMA : 12/10/2017
        private void TransferFormType5RptCmd_In()
        {
            resetMenuColor();
            TransferFormType5Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TransferFormType5Rpt;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            //UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

            //
        }


        public void TransferFormType5RptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2133_G1_TransferFormType5Rpt;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormType5RptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormType5RptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //
        //TMA: 18/10/2017 MẪU SỔ KIỂM NHẬP THUỐC/HOÁ CHẤT/VẬT TƯ 
        private void RptDrugMedDeptCmd_In()
        {
            resetMenuColor();
            RptDrugMedDept.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.RptDrugMedDept;
            UnitVM.RptParameters.ShowFirst = "";
            // UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mRptDrugMedDept;
            // UnitVM.IsEnableViewBy = false;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }


        public void RptDrugMedDeptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2135_G1_RptDrugMedDept;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RptDrugMedDeptCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RptDrugMedDeptCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //

        private void ThongKeDoanhThuCmd_In()
        {
            resetMenuColor();
            ThongKeDoanhThu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.THONGKEDOANHTHU;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mTempThongKeDoanhThu;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ThongKeDoanhThuCmd()
        {
            Globals.TitleForm = eHCMSResources.G0454_G1_TKeDThu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ThongKeDoanhThuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ThongKeDoanhThuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TongHopDoanhThuCmd_In()
        {
            resetMenuColor();
            TongHopDoanhThu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRptDoanhThuTongHop>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void TongHopDoanhThuCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1809_G1_TgHopDThu;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TongHopDoanhThuCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TongHopDoanhThuCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void HoatDongQuayDKCmd_In()
        {
            resetMenuColor();
            HoatDongQuayDK.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRptHoatDongQuayDK>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }

        public void HoatDongQuayDKCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1810_G1_BCHoatDongQDK;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoatDongQuayDKCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoatDongQuayDKCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TongHopDoanhThuNoiTruCmd_In()
        {
            resetMenuColor();
            TongHopDoanhThuNoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRptDoanhThuTongHopNoiTru>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }


        public void TongHopDoanhThuNoiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1215_G1_TgHopDThuNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TongHopDoanhThuNoiTruCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TongHopDoanhThuNoiTruCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoBHYTCmd_In()
        {
            resetMenuColor();
            BaoCaoBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<ICreateHIReport>();

            module.MainContent = VM;
            (module as Conductor<object>).ActivateItem(VM);
        }


        public void BaoCaoBHYTCmd()
        {
            Globals.TitleForm = eHCMSResources.K1053_G1_BCBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoBHYTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoBHYTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoVienPhiTraiCmd_In()
        {
            resetMenuColor();
            BaoCaoVienPhiTrai.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HOSPITAL_FEES_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }


        public void BaoCaoVienPhiTraiCmd()
        {
            Globals.TitleForm = eHCMSResources.K1085_G1_BCVPhiTrai;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVienPhiTraiCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVienPhiTraiCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void BaoCaoVPMTreEmCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1811_G1_BCVPhiMoTreEm6Tuoi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVPMTreEmCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVPMTreEmCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoVPMTreEmCmd_In()
        {
            resetMenuColor();
            BaoCaoVPMTreEm.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HIGH_TECH_FEES_CHILD_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void BaoCaoVienPhiKTCCmd_In()
        {
            resetMenuColor();
            BaoCaoVienPhiKTC.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HIGH_TECH_FEES_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }


        public void BaoCaoVienPhiKTCCmd()
        {
            Globals.TitleForm = eHCMSResources.K1083_G1_BCVPhiKTC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVienPhiKTCCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVienPhiKTCCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BaoCaoVTYT_KTCCmd_In()
        {
            resetMenuColor();
            BaoCaoVTYT_KTC.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.OUT_MEDICAL_MATERIAL_REPORT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }


        public void BaoCaoVTYT_KTCCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1812_G1_BCVTYTKTC;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BaoCaoVTYT_KTCCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BaoCaoVTYT_KTCCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ThongKeNgoaiTruCmd_In()
        {
            resetMenuColor();
            ThongKeNgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<ITransactionModule>();
            var ThongKeNgoaiTruVM = Globals.GetViewModel<IThongKeNgoaiTru>();
            ThongKeNgoaiTruVM.LoadRefDept(V_DeptTypeOperation.KhoaNgoaiTru);
            Module.MainContent = ThongKeNgoaiTruVM;
            (Module as Conductor<object>).ActivateItem(ThongKeNgoaiTruVM);

        }

        public void ThongKeNgoaiTruCmd()
        {
            Globals.TitleForm = eHCMSResources.K1035_G1_BKeCTietKB;
            ThongKeNgoaiTruCmd_In();
        }

        private void ReportQuickConsultationCmd_In()
        {
            resetMenuColor();
            ReportQuickConsultation.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ITransactionModule>();
            var innerVm = Globals.GetViewModel<IBaoCaoNhanhKhuKhamBenh>();
            innerVm.eItem = ReportName.BAOCAONHANH_KHUKHAMBENH;
            regModule.MainContent = innerVm;
            ((Conductor<object>)regModule).ActivateItem(innerVm);
        }

        public void ReportQuickConsultationCmd()
        {
            Globals.TitleForm = eHCMSResources.K1062_G1_BCNhanhKhuKB;
            if (string.IsNullOrEmpty(Globals.HIRegistrationForm))
            {
                ReportQuickConsultationCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportQuickConsultationCmd_In();
                        GlobalsNAV.msgb = null;
                        Globals.HIRegistrationForm = "";
                    }
                });
            }
        }

        private void ReportInPatientCmd_In()
        {
            resetMenuColor();
            ReportInPatient.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.REPORT_INPATIENT;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.IsYVu = true;
            UnitVM.EnumOfFunction = (int)eYVu_Management.mReportInPatient;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReportInPatientCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1813_G1_BCBNDTriNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportInPatientCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportInPatientCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void ReportGeneralTemp02Cmd_In()
        {
            resetMenuColor();
            ReportGeneralTemp02.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.REPORT_GENERAL_TEMP02;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ReportModel = null;
            UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.IsYVu = true;
            UnitVM.EnumOfFunction = (int)eYVu_Management.mReportGeneralTemp02;
            //KMx: Sau khi load form phải check lại quyền của user.
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void ReportGeneralTemp02Cmd()
        {
            Globals.TitleForm = eHCMSResources.G1517_G1_THopCPhiDTriNoiTru;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportGeneralTemp02Cmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportGeneralTemp02Cmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void MedServiceItemPriceList_Mgnt_In()
        {
            resetMenuColor();
            BangGiaDichVu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;
            var TransactionModule = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IMedServiceItemPriceList>();

            TransactionModule.MainContent = VM;
            (TransactionModule as Conductor<object>).ActivateItem(VM);
        }

        public void MedServiceItemPriceList_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.K1028_G1_BGiaDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                MedServiceItemPriceList_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        MedServiceItemPriceList_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void PCLExamTypePriceList_Mgnt_In()
        {
            resetMenuColor();
            BangGiaCLS.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;
            var TransactionModule = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IPCLExamTypePriceList>();

            TransactionModule.MainContent = VM;
            (TransactionModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypePriceList_Mgnt()
        {
            Globals.TitleForm = eHCMSResources.K1032_G1_BGiaPCLExamtype;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamTypePriceList_Mgnt_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTypePriceList_Mgnt_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void RefDepartmentReqCashAdv_Mgnt()
        {
            resetMenuColor();
            GiaTamUng.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.TitleForm = eHCMSResources.T1143_G1_GiaTUTungKhoa;
            Globals.PageName = Globals.TitleForm;
            var TransactionModule = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IRefDepartmentReqCashAdv>();

            TransactionModule.MainContent = VM;
            (TransactionModule as Conductor<object>).ActivateItem(VM);
        }

        private void FollowICDCmd_In()
        {
            resetMenuColor();
            FollowICD.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FollowICD;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void FollowICDCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2041_G1_THBTTVTaiBV.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                FollowICDCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        FollowICDCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void EmployeesCmd_In()
        {
            resetMenuColor();
            btnEmployees.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.EmployeesReport;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void EmployeesCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2044_G1_TinhHinhCBCCVC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                EmployeesCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        EmployeesCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PaymentManagementCmd_In()
        {
            resetMenuColor();
            btnPaymentManagement.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IHISPaymentHistory>();
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void PaymentManagementCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2046_G1_QuanLyChiTieu.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PaymentManagementCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PaymentManagementCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        #region Hospital Statistics

        private void HRStatisticsByDeptCmd_In()
        {
            resetMenuColor();
            HRStatisticsByDept.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.HR_STATISTICS_BY_DEPT;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void HRStatisticsByDeptCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1936_G1_TinhHinhCanBo;
            HRStatisticsByDeptCmd_In();
        }

        private void MedExamActivityCmd_In()
        {
            resetMenuColor();
            MedExamActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.MED_EXAM_ACTIVITY;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void MedExamActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1937_G1_HDongKB;
            MedExamActivityCmd_In();
        }

        private void TreatmentActivityCmd_In()
        {
            resetMenuColor();
            TreatmentActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.TREATMENT_ACTIVITY;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void TreatmentActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1938_G1_HDongDTri;
            TreatmentActivityCmd_In();
        }

        private void SpecialistTreatmentActivityCmd_In()
        {
            resetMenuColor();
            SpecialistTreatmentActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.SPECIALIST_TREATMENT_ACTIVITY;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SpecialistTreatmentActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1939_G1_HDongDTriChuyenKhoa;
            SpecialistTreatmentActivityCmd_In();
        }

        private void SurgeryActivityCmd_In()
        {
            resetMenuColor();
            SurgeryActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.SURGERY_ACTIVITY;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void SurgeryActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1940_G1_HDongTThuatPThuat;
            SurgeryActivityCmd_In();
        }

        private void ReproductiveHealthActivityCmd_In()
        {
            resetMenuColor();
            ReproductiveHealthActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.REPRODUCTIVE_HEALTH_ACTIVITY;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void ReproductiveHealthActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1946_G1_HDongSKSS;
            ReproductiveHealthActivityCmd_In();
        }

        private void PCLActivityCmd_In()
        {
            resetMenuColor();
            PCLActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.PCL_ACTIVITY;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void PCLActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1941_G1_HDongCLS;
            PCLActivityCmd_In();
        }

        private void PharmacyDeptStatisticsCmd_In()
        {
            resetMenuColor();
            PharmacyDeptStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.PHARMACY_DEPT_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void PharmacyDeptStatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1942_G1_DuocBV;
            PharmacyDeptStatisticsCmd_In();
        }
        //------ DPT 
        public void HoatdongchidaotuyenCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2118_G1_HDCDT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                HoatdongchidaotuyenCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        HoatdongchidaotuyenCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void HoatdongchidaotuyenCmd_In()
        {
            resetMenuColor();
            HDCDTuyen.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mChiDaoTuyen;
            UnitVM.eItem = ReportName.HoatDongChiDaoTuyen;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

        }

        public void BC15DayCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2134_G1_15Ngay;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BC15DayCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BC15DayCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BC15DayCmd_In()
        {
            resetMenuColor();
            BC15Day.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mBC15Day;
            UnitVM.eItem = ReportName.BaoCao15Ngay;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

        }
        public void ChidaotuyenCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2118_G1_CDT;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ChidaotuyenCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ChidaotuyenCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ChidaotuyenCmd_In()
        {
            resetMenuColor();
            CDTuyen.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITrainingForSubOrg>();
            UnitVM.TitleForm = "Quản lý hoạt động chỉ đạo tuyến";
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);


        }

        public void SoXetNghiemCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2148_G1_SoXetNghiem;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SoXetNghiemCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SoXetNghiemCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void SoXetNghiemCmd_In()
        {
            resetMenuColor();
            SXN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.RptParameters.ShowFirst = "";
            UnitVM.ViDetail = Visibility.Visible;
            UnitVM.ShowDepartment = Visibility.Visible;
            UnitVM.ViTreatedOrNot = Visibility.Visible;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mSXN;
            UnitVM.eItem = ReportName.SoXetNghiem;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

        }

        //---------
        private void MedicalEquipmentStatisticsCmd_In()
        {
            resetMenuColor();
            MedicalEquipmentStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.MEDICAL_EQUIPMENT_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void MedicalEquipmentStatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1947_G1_ThietBiYTe;
            MedicalEquipmentStatisticsCmd_In();
        }
        //-------DPT
        private void ScientificResearchActivityCmd_In()
        {
            resetMenuColor();
            ScientificResearchActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.SCIENTIFIC_RESEARCH_ACTIVITY;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.ViDetail = Visibility.Collapsed;
            UnitVM.EnumOfFunction = (int)eTransaction_Management.mNghienCuuKhoaHoc;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

        }
        private void NghienCuuKhoaHocCmd_In()
        {
            resetMenuColor();
            NCKhoaHoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            Globals.PageName = Globals.TitleForm;
            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IScientificResearchActivity>();
            UnitVM.TitleForm = Globals.TitleForm;
            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);

        }

        public void ScientificResearchActivityCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2119_G1_HDNCKH;
            ScientificResearchActivityCmd_In();
        }
        public void NghienCuuKhoaHocCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2119_G1_HDNCKH;

            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NghienCuuKhoaHocCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NghienCuuKhoaHocCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void FinancialActivityTemp1Cmd_In()
        {
            resetMenuColor();
            FinancialActivityTemp1.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FINANCIAL_ACTIVITY_TEMP1;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        //----------------------
        public void FinancialActivityTemp1Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z1944_G1_HDongTCMau1;
            FinancialActivityTemp1Cmd_In();
        }

        private void FinancialActivityTemp2Cmd_In()
        {
            resetMenuColor();
            FinancialActivityTemp2.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FINANCIAL_ACTIVITY_TEMP2;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void FinancialActivityTemp2Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z1944_G1_HDongTCMau2;
            FinancialActivityTemp2Cmd_In();
        }

        private void FinancialActivityTemp3Cmd_In()
        {
            resetMenuColor();
            FinancialActivityTemp3.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<IRptBaoCaoTheoDot>();
            UnitVM.eItem = ReportName.FINANCIAL_ACTIVITY_TEMP3;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        public void FinancialActivityTemp3Cmd()
        {
            Globals.TitleForm = eHCMSResources.Z1944_G1_HDongTCMau3;
            FinancialActivityTemp3Cmd_In();
        }

        private void ICD10StatisticsCmd_In()
        {
            resetMenuColor();
            ICD10Statistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.ICD10_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }

        public void ICD10StatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z1945_G1_TinhHinhBenhTatTheoICD;
            ICD10StatisticsCmd_In();
        }
        //TransferFormListCmd
        public void TransferFormListCmd()
        {
            Globals.TitleForm = "Danh sách chuyển tuyến";
            TransferFormListCmd_In();
        }
        private void TransferFormListCmd_In()
        {
            resetMenuColor();
            TransferFormList.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITransferFormList>();
            //UnitVM.eItem = ReportName.ICD10_STATISTICS;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }

        private void InPtAdmDisStatisticsCmd_In()
        {
            resetMenuColor();
            btnInPtAdmDisStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.InPtAdmDisStatistics;

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
            UnitVM.LockConditionCombobox();
        }
        public void InPtAdmDisStatisticsCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2132_G1_SoVaoRaChuyenVien;
            InPtAdmDisStatisticsCmd_In();
        }
        #endregion

        #region load

        public Button Temp20NgoaiTru { get; set; }
        public Button Temp20NgoaiTruTraThuoc { get; set; }
        public Button Temp20NoiTru { get; set; }
        public Button Temp20VTYTTH { get; set; }
        public Button Temp21NgoaiTru { get; set; }
        public Button Temp21NoiTru { get; set; }
        public Button Temp25a { get; set; }
        public Button Temp25aTraThuoc { get; set; }
        public Button Temp26a { get; set; }
        public Button Temp19 { get; set; }
        public Button Temp20NoiTruNew { get; set; }
        public Button Temp21New { get; set; }
        public Button Temp79a { get; set; }
        public Button Temp79aTraThuoc { get; set; }
        public Button Temp80a { get; set; }
        public Button Temp38aNgoaiTru { get; set; }
        public Button Temp38aNoiTru { get; set; }
        public Button ChiTietVienPhiPK { get; set; }
        public Button ChiTietVienPhi { get; set; }
        //TMA - Mẫu 02 - HDKB
        public Button HoatDongKB { get; set; }
        //TMA - Mẫu 06 - CLS
        public Button CanLamSan { get; set; }
        //TMA - Mẫu 07 -DuocBV
        public Button DuocBV { get; set; }
        /*TMA - PHỤ LỤC 2B -TransferFormType2Rpt - 06/10/2017*/
        public Button TransferFormType2Rpt { get; set; }
        /*TMA - PHỤ LỤC 2A -TransferFormType2_1Rpt - 09/10/2017*/
        public Button TransferFormType2_1Rpt { get; set; }
        /*TMA - PHỤ LỤC 5 -TransferFormType5Rpt - 12/10/2017*/
        public Button TransferFormType5Rpt { get; set; }
        //*TMA - RptDrugMedDept - 18/10/2017
        public Button RptDrugMedDept { get; set; }
        // DPT -Hoat dong chi dao tuyen
        public Button CDTuyen { get; set; }
        public Button HDCDTuyen { get; set; }
        //---17/10/2017 DPT bao cao 15 day 
        public Button BC15Day { get; set; }
        //---3/11/2017 DPT so xet nghiem
        public Button SXN { get; set; }
        public Button ThongKeDoanhThu { get; set; }
        public Button TongHopDoanhThu { get; set; }
        public Button HoatDongQuayDK { get; set; }

        public Button TongHopDoanhThuNoiTru { get; set; }
        public Button BaoCaoBHYT { get; set; }
        public Button BaoCaoVienPhiTrai { get; set; }
        public Button BaoCaoVienPhiKTC { get; set; }
        public Button BaoCaoVTYT_KTC { get; set; }
        public Button ThongKeNgoaiTru { get; set; }
        public Button ReportQuickConsultation { get; set; }
        public Button ReportInPatient { get; set; }
        public Button ReportGeneralTemp02 { get; set; }
        public Button BangGiaDichVu { get; set; }
        public Button BangGiaCLS { get; set; }
        public Button GiaTamUng { get; set; }
        public Button BaoCaoVPMTreEm { get; set; }
        public Button HRStatisticsByDept { get; set; }
        public Button MedExamActivity { get; set; }
        public Button TreatmentActivity { get; set; }
        public Button SpecialistTreatmentActivity { get; set; }
        public Button SurgeryActivity { get; set; }
        public Button ReproductiveHealthActivity { get; set; }
        public Button PCLActivity { get; set; }
        public Button PharmacyDeptStatistics { get; set; }
        public Button MedicalEquipmentStatistics { get; set; }
        public Button ScientificResearchActivity { get; set; }
        public Button NCKhoaHoc { get; set; }
        public Button FinancialActivityTemp1 { get; set; }
        public Button FinancialActivityTemp2 { get; set; }
        public Button FinancialActivityTemp3 { get; set; }
        public Button ICD10Statistics { get; set; }
        public Button TransferFormList { get; set; }
        public Button InOutValueReport { get; set; }
        public Button FollowICD { get; set; }
        public Button btnEmployees { get; set; }
        public Button btnPaymentManagement { get; set; }
        public Button btnInPtAdmDisStatistics { get; set; }
        public Button ConfirmBHYT { get; set; }


        public void Temp20NgoaiTruCmd_Loaded(object sender)
        {
            Temp20NgoaiTru = sender as Button;
            Temp20NgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp20NgoaiTruTraThuocCmd_Loaded(object sender)
        {
            Temp20NgoaiTruTraThuoc = sender as Button;
            Temp20NgoaiTruTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp20NoiTruCmd_Loaded(object sender)
        {
            Temp20NoiTru = sender as Button;
            Temp20NoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp20VTYTTHCmd_Loaded(object sender)
        {
            Temp20VTYTTH = sender as Button;
            Temp20VTYTTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp21NgoaiTruCmd_Loaded(object sender)
        {
            Temp21NgoaiTru = sender as Button;
            Temp21NgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void Temp21NoiTruCmd_Loaded(object sender)
        {
            Temp21NoiTru = sender as Button;
            Temp21NoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp25aCmd_Loaded(object sender)
        {
            Temp25a = sender as Button;
            Temp25a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp25aTraThuocCmd_Loaded(object sender)
        {
            Temp25aTraThuoc = sender as Button;
            Temp25aTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp26aCmd_Loaded(object sender)
        {
            Temp26a = sender as Button;
            Temp26a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp19Cmd_Loaded(object sender)
        {
            Temp19 = sender as Button;
            Temp19.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp20NoiTruNewCmd_Loaded(object sender)
        {
            Temp20NoiTruNew = sender as Button;
            Temp20NoiTruNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp21NewCmd_Loaded(object sender)
        {
            Temp21New = sender as Button;
            Temp21New.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp79aCmd_Loaded(object sender)
        {
            Temp79a = sender as Button;
            Temp79a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp79aTraThuocCmd_Loaded(object sender)
        {
            Temp79aTraThuoc = sender as Button;
            Temp79aTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp80aCmd_Loaded(object sender)
        {
            Temp80a = sender as Button;
            Temp80a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp38aNgoaiTruCmd_Loaded(object sender)
        {
            Temp38aNgoaiTru = sender as Button;
            Temp38aNgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void Temp38aNoiTruCmd_Loaded(object sender)
        {
            Temp38aNoiTru = sender as Button;
            Temp38aNoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ChiTietVienPhiPKCmd_Loaded(object sender)
        {
            ChiTietVienPhiPK = sender as Button;
            ChiTietVienPhiPK.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ChiTietVienPhiCmd_Loaded(object sender)
        {
            ChiTietVienPhi = sender as Button;
            ChiTietVienPhi.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //tma - mẫu 02 - HDKB
        public void HoatDongKBCmd_Loaded(object sender)
        {
            HoatDongKB = sender as Button;
            HoatDongKB.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //tma - mẫu 06 - CLS
        public void CanLamSanCmd_Loaded(object sender)
        {
            CanLamSan = sender as Button;
            CanLamSan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //tma - mẫu 07 - DuocBV
        public void DuocBVCmd_Loaded(object sender)
        {
            DuocBV = sender as Button;
            DuocBV.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //tma - phụ lục 2b - TransferFormType2Rpt : 06/10/2017
        public void TransferFormType2RptCmd_Loaded(object sender)
        {
            TransferFormType2Rpt = sender as Button;
            TransferFormType2Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //tma - phụ lục 2A - TransferFormType2_1Rpt : 09/10/2017
        public void TransferFormType2_1RptCmd_Loaded(object sender)
        {
            TransferFormType2_1Rpt = sender as Button;
            TransferFormType2_1Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //tma - phụ lục 5 - TransferFormType5Rpt : 12/10/2017
        public void TransferFormType5RptCmd_Loaded(object sender)
        {
            TransferFormType5Rpt = sender as Button;
            TransferFormType5Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //tma - SỔ KIỂM NHẬP THUỐC HÁO CHẤT CẬN LÂM SÀN - 18/10/2017
        public void RptDrugMedDeptCmd_Loaded(object sender)
        {
            RptDrugMedDept = sender as Button;
            RptDrugMedDept.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //------- 17/10/2017 DPT Baocao 15 ngày
        public void BC15Day_Loaded(object sender)
        {
            BC15Day = sender as Button;
            BC15Day.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        // ------------- 3/11/2017 DPT So xet nghiem
        public void SoXetNghiem_Loaded(object sender)
        {
            SXN = sender as Button;
            SXN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        //-------
        //DPT - Hoat dong chi daotuyen
        public void Hoatdongchidaotuyen_Loaded(object sender)
        {
            HDCDTuyen = sender as Button;
            HDCDTuyen.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void Chidaotuyen_Loaded(object sender)
        {
            CDTuyen = sender as Button;
            CDTuyen.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ThongKeDoanhThuCmd_Loaded(object sender)
        {
            ThongKeDoanhThu = sender as Button;
            ThongKeDoanhThu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TongHopDoanhThuCmd_Loaded(object sender)
        {
            TongHopDoanhThu = sender as Button;
            TongHopDoanhThu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void HoatDongQuayDKCmd_Loaded(object sender)
        {
            HoatDongQuayDK = sender as Button;
            HoatDongQuayDK.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void TongHopDoanhThuNoiTruCmd_Loaded(object sender)
        {
            TongHopDoanhThuNoiTru = sender as Button;
            TongHopDoanhThuNoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void BaoCaoBHYTCmd_Loaded(object sender)
        {
            BaoCaoBHYT = sender as Button;
            BaoCaoBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void BaoCaoVienPhiTraiCmd_Loaded(object sender)
        {
            BaoCaoVienPhiTrai = sender as Button;
            BaoCaoVienPhiTrai.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void BaoCaoVPMTreEmCmd_Loaded(object sender)
        {
            BaoCaoVPMTreEm = sender as Button;
            BaoCaoVPMTreEm.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void BaoCaoVienPhiKTCCmd_Loaded(object sender)
        {
            BaoCaoVienPhiKTC = sender as Button;
            BaoCaoVienPhiKTC.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void BaoCaoVTYT_KTCCmd_Loaded(object sender)
        {
            BaoCaoVTYT_KTC = sender as Button;
            BaoCaoVTYT_KTC.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void ThongKeNgoaiTruCmd_Loaded(object sender)
        {
            ThongKeNgoaiTru = sender as Button;
            ThongKeNgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void ReportQuickConsultationCmd_Loaded(object sender)
        {
            ReportQuickConsultation = sender as Button;
            ReportQuickConsultation.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void ReportInPatientCmd_Loaded(object sender)
        {
            ReportInPatient = sender as Button;
            ReportInPatient.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void ReportGeneralTemp02Cmd_Loaded(object sender)
        {
            ReportGeneralTemp02 = sender as Button;
            ReportGeneralTemp02.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void MedServiceItemPriceList_Mgnt_Loaded(object sender)
        {
            BangGiaDichVu = sender as Button;
            BangGiaDichVu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void PCLExamTypePriceList_Mgnt_Loaded(object sender)
        {
            BangGiaCLS = sender as Button;
            BangGiaCLS.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void RefDepartmentReqCashAdv_Mgnt_Loaded(object sender)
        {
            GiaTamUng = sender as Button;
            GiaTamUng.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void HRStatisticsByDeptCmd_Loaded(object sender)
        {
            HRStatisticsByDept = sender as Button;
            HRStatisticsByDept.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void MedExamActivityCmd_Loaded(object sender)
        {
            MedExamActivity = sender as Button;
            MedExamActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void TreatmentActivityCmd_Loaded(object sender)
        {
            TreatmentActivity = sender as Button;
            TreatmentActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void SpecialistTreatmentActivityCmd_Loaded(object sender)
        {
            SpecialistTreatmentActivity = sender as Button;
            SpecialistTreatmentActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void SurgeryActivityCmd_Loaded(object sender)
        {
            SurgeryActivity = sender as Button;
            SurgeryActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void ReproductiveHealthActivityCmd_Loaded(object sender)
        {
            ReproductiveHealthActivity = sender as Button;
            ReproductiveHealthActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void PCLActivityCmd_Loaded(object sender)
        {
            PCLActivity = sender as Button;
            PCLActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void PharmacyDeptStatisticsCmd_Loaded(object sender)
        {
            PharmacyDeptStatistics = sender as Button;
            PharmacyDeptStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void MedicalEquipmentStatisticsCmd_Loaded(object sender)
        {
            MedicalEquipmentStatistics = sender as Button;
            MedicalEquipmentStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void NghienCuuKhoaHocCmd_Loaded(object sender)
        {
            NCKhoaHoc = sender as Button;
            NCKhoaHoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ScientificResearchActivityCmd_Loaded(object sender)
        {
            ScientificResearchActivity = sender as Button;
            ScientificResearchActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void FinancialActivityTemp1Cmd_Loaded(object sender)
        {
            FinancialActivityTemp1 = sender as Button;
            FinancialActivityTemp1.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void FinancialActivityTemp2Cmd_Loaded(object sender)
        {
            FinancialActivityTemp2 = sender as Button;
            FinancialActivityTemp2.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void FinancialActivityTemp3Cmd_Loaded(object sender)
        {
            FinancialActivityTemp3 = sender as Button;
            FinancialActivityTemp3.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }

        public void ICD10StatisticsCmd_Loaded(object sender)
        {
            ICD10Statistics = sender as Button;
            ICD10Statistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void TransferFormList_Loaded(object sender)
        {
            TransferFormList = sender as Button;
            TransferFormList.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void InOutValueReportCmd_Loaded(object sender)
        {
            InOutValueReport = sender as Button;
            InOutValueReport.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void FollowICDCmd_Loaded(object sender)
        {
            FollowICD = sender as Button;
            FollowICD.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void EmployeesCmd_Loaded(object sender)
        {
            btnEmployees = sender as Button;
            btnEmployees.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void PaymentManagement_Loaded(object sender)
        {
            btnPaymentManagement = sender as Button;
            btnPaymentManagement.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void InPtAdmDisStatisticsCmd_Loaded(object sender)
        {
            btnInPtAdmDisStatistics = sender as Button;
            btnInPtAdmDisStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        public void ConfirmBHYTCmd_Loaded(object sender)
        {
            ConfirmBHYT = sender as Button;
            ConfirmBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        #endregion

        public void resetMenuColor()
        {
            if (Temp20NgoaiTru != null)
            {
                Temp20NgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp20NgoaiTruTraThuoc != null)
            {
                Temp20NgoaiTruTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp20NoiTru != null)
            {
                Temp20NoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp20VTYTTH != null)
            {
                Temp20VTYTTH.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp21NgoaiTru != null)
            {
                Temp21NgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp21NoiTru != null)
            {
                Temp21NoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp25a != null)
            {
                Temp25a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp25aTraThuoc != null)
            {
                Temp25aTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp26a != null)
            {
                Temp26a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp19 != null)
            {
                Temp19.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp20NoiTruNew != null)
            {
                Temp20NoiTruNew.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp21New != null)
            {
                Temp21New.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp79a != null)
            {
                Temp79a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp79aTraThuoc != null)
            {
                Temp79aTraThuoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp80a != null)
            {
                Temp80a.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp38aNgoaiTru != null)
            {
                Temp38aNgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (Temp38aNoiTru != null)
            {
                Temp38aNoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ChiTietVienPhiPK != null)
            {
                ChiTietVienPhiPK.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ChiTietVienPhi != null)
            {
                ChiTietVienPhi.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ThongKeDoanhThu != null)
            {
                ThongKeDoanhThu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TongHopDoanhThu != null)
            {
                TongHopDoanhThu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TongHopDoanhThuNoiTru != null)
            {
                TongHopDoanhThuNoiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (BaoCaoBHYT != null)
            {
                BaoCaoBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (BaoCaoVienPhiTrai != null)
            {
                BaoCaoVienPhiTrai.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (BaoCaoVienPhiKTC != null)
            {
                BaoCaoVienPhiKTC.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (BaoCaoVTYT_KTC != null)
            {
                BaoCaoVTYT_KTC.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (HoatDongQuayDK != null)
            {
                HoatDongQuayDK.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ThongKeNgoaiTru != null)
            {
                ThongKeNgoaiTru.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ReportQuickConsultation != null)
            {
                ReportQuickConsultation.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ReportInPatient != null)
            {
                ReportInPatient.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }

            if (ReportGeneralTemp02 != null)
            {
                ReportGeneralTemp02.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }

            if (BangGiaDichVu != null)
            {
                BangGiaDichVu.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (BangGiaCLS != null)
            {
                BangGiaCLS.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (GiaTamUng != null)
            {
                GiaTamUng.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (BaoCaoVPMTreEm != null)
            {
                BaoCaoVPMTreEm.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (HRStatisticsByDept != null)
            {
                HRStatisticsByDept.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (MedExamActivity != null)
            {
                MedExamActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TreatmentActivity != null)
            {
                TreatmentActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (SpecialistTreatmentActivity != null)
            {
                SpecialistTreatmentActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (SurgeryActivity != null)
            {
                SurgeryActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ReproductiveHealthActivity != null)
            {
                ReproductiveHealthActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (PCLActivity != null)
            {
                PCLActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (PharmacyDeptStatistics != null)
            {
                PharmacyDeptStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (MedicalEquipmentStatistics != null)
            {
                MedicalEquipmentStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ScientificResearchActivity != null)
            {
                ScientificResearchActivity.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (NCKhoaHoc != null)
            {
                NCKhoaHoc.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (FinancialActivityTemp1 != null)
            {
                FinancialActivityTemp1.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (FinancialActivityTemp2 != null)
            {
                FinancialActivityTemp2.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (FinancialActivityTemp3 != null)
            {
                FinancialActivityTemp3.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ICD10Statistics != null)
            {
                ICD10Statistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TransferFormList != null)
            {
                TransferFormList.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (InOutValueReport != null)
            {
                InOutValueReport.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            /*TMA*/
            if (HoatDongKB != null)
            {
                HoatDongKB.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (CanLamSan != null)
            {
                CanLamSan.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (DuocBV != null)
            {
                DuocBV.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (RptDrugMedDept != null)
            {
                RptDrugMedDept.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            /*TMA*/
            if (FollowICD != null)
            {
                FollowICD.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (btnEmployees != null)
            {
                btnEmployees.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (btnPaymentManagement != null)
            {
                btnPaymentManagement.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (CDTuyen != null)
            {
                CDTuyen.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (HDCDTuyen != null)
            {
                HDCDTuyen.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            //----17/10/2017 DPT Bao cao 15 Ngày
            if (BC15Day != null)
            {
                BC15Day.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            //----3/11/2017 DPT so xet nghiem
            if (SXN != null)
            {
                SXN.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }

            /*TMA 12/10/2017 : 3 BÁO CÁO CHUYỂN TUYẾN*/
            if (TransferFormType2Rpt != null)
            {
                TransferFormType2Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TransferFormType2_1Rpt != null)
            {
                TransferFormType2_1Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TransferFormType5Rpt != null)
            {
                TransferFormType5Rpt.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            /*TMA 12/10/2017 : 3 BÁO CÁO CHUYỂN TUYẾN*/
            /*TMA 18/10/2017 - SỔ KIỂM NHẬP THUỐC/HOÁ CHẤT/VẬT TƯ Y TẾ TIÊU HAO*/
            if (RptDrugMedDept != null)
            {
                RptDrugMedDept.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            /*TMA 18/10/2017 - SỔ KIỂM NHẬP THUỐC/HOÁ CHẤT/VẬT TƯ Y TẾ TIÊU HAO*/
            if (btnInPtAdmDisStatistics != null)
            {
                btnInPtAdmDisStatistics.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (TransferFormDataExportCmd != null)
            {
                TransferFormDataExportCmd.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
            if (ConfirmBHYT != null)
            {
                ConfirmBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            }
        }
        private void InOutValueReportCmd_In()
        {
            resetMenuColor();
            InOutValueReport.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            Globals.PageName = Globals.TitleForm;

            var Module = Globals.GetViewModel<ITransactionModule>();
            var VM = Globals.GetViewModel<IDrugDeptNhapXuatTon>();
            VM.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
            VM.LoadRefGenericDrugCategory_1();
            VM.strHienThi = Globals.TitleForm;
            VM.IsShowClinic = true;
            //VM.mXemIn = mBaoCaoXuatNhapTon_Thuoc_XemIn;
            //VM.mKetChuyen = mBaoCaoXuatNhapTon_Thuoc_KetChuyen;
            VM.CanSelectedRefGenDrugCatID_1 = true;
            VM.IsGetValue = true;
            Module.MainContent = VM;
            (Module as Conductor<object>).ActivateItem(VM);
        }
        public void InOutValueReportCmd()
        {
            Globals.TitleForm = eHCMSResources.Z2104_G1_BCDoanhThuNXT.ToUpper();
            InOutValueReportCmd_In();
        }
        public Button TransferFormDataExportCmd { get; set; }
        public void TransferFormDataExportCmd_Loaded(object sender)
        {
            TransferFormDataExportCmd = sender as Button;
            TransferFormDataExportCmd.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
        }
        private void TransferFormDataExportCmd_In()
        {
            resetMenuColor();
            TransferFormDataExportCmd.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ITemp20NgoaiTru>();
            UnitVM.eItem = ReportName.TRANSFERFORMDATA;
            UnitVM.RptParameters.ShowFirst = "";
            //UnitVM.EnumOfFunction = (int)eTransaction_Management.mTemp05_TransferFormType5Rpt;
            UnitVM.authorization();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void TransferFormDataExportCmd_Click()
        {
            Globals.TitleForm = eHCMSResources.Z2161_G1_DuLieuChuyenTuyen;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TransferFormDataExportCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TransferFormDataExportCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ConfirmBHYTCmd_In()
        {
            resetMenuColor();
            ConfirmBHYT.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];

            Globals.PageName = Globals.TitleForm;

            var module = Globals.GetViewModel<ITransactionModule>();
            var UnitVM = Globals.GetViewModel<ICreateHIReport_V2>();

            module.MainContent = UnitVM;
            (module as Conductor<object>).ActivateItem(UnitVM);
        }
        public void ConfirmBHYTCmd()
        {
            Globals.TitleForm = eHCMSResources.G2370_G1_XNhanBHYT;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ConfirmBHYTCmd_In();
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ConfirmBHYTCmd_In();
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
    }
}