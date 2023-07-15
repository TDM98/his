/*
20161223 #001 CMN: Add file manager
*/
using eHCMSLanguage;
using System.Windows;
using aEMR.Infrastructure;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.ComponentModel.Composition;
using DataEntities;
using System.Windows.Controls;
/*
 * 20220810 #001 QTD: Dời Menu Quản lý hồ sơ từ sang BC Tổng hợp
 */

namespace aEMR.ClinicManagement.ViewModels.Menu
{
    [Export(typeof(IClinicTopMenu))]
    public class ClinicTopMenuViewModel : Conductor<object>, IClinicTopMenu
    {
        public bool EnableMedicalFileManagement
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.EnableMedicalFileManagement;
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            
            bResourcesMedListCmd =Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        ,(int)eResources.mPtDashboardResource);

            bResourcesOffListCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardResource_Office);

            bAllocResourcesMedCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardNewAllocations);

            bAllocResourcesOffCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardNewAllocations__Office);

            bTranfResourcesCmd = Globals.CheckFunction(Globals.listRefModule,(int)eModules.mResources
                        , (int)eResources.mPtDashboardNewTranfers);

            bResourceMaintenanceLog_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mResources_Maintenance
                        , (int)eResources_Maintenance.mListRequest);

            bResourceMaintenanceLog_AddNewMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mResources_Maintenance
                        , (int)eResources_Maintenance.mAddNewRequest);

            bClinicManager = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mClinicManagement, (int)eClinicManagement.mQuanLyPhongKham);
            //▼====: #001
            //bFileManager = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mClinicManagement, (int)eClinicManagement.mFileManagement);
            //▲====: #001
            bOvertimeClinicManager = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mClinicManagement, (int)eClinicManagement.mLichLamViecNgoaiGio);
            bParentClinicManager = bResourcesMedListCmd || bResourcesOffListCmd || bAllocResourcesMedCmd || bAllocResourcesOffCmd || bTranfResourcesCmd
                || bResourceMaintenanceLog_Mgnt || bResourceMaintenanceLog_AddNewMgnt || bClinicManager || bOvertimeClinicManager;
        }
        #region bool properties
        private bool _bResourcesMedListCmd = true;
        private bool _bResourcesOffListCmd = true;
        private bool _bAllocResourcesMedCmd = true;
        private bool _bAllocResourcesOffCmd = true;
        private bool _bTranfResourcesCmd = true;
        private bool _bResourceMaintenanceLog_Mgnt = true;
        private bool _bResourceMaintenanceLog_AddNewMgnt = true;
        private bool _bClinicManager = true;
        private bool _bParentClinicManager = true;
        private bool _bFileManager = true;

        public bool bResourcesMedListCmd
        {
            get
            {
                return _bResourcesMedListCmd;
            }
            set
            {
                if (_bResourcesMedListCmd == value)
                    return;
                _bResourcesMedListCmd = value;
            }
        }
        public bool bResourcesOffListCmd
        {
            get
            {
                return _bResourcesOffListCmd;
            }
            set
            {
                if (_bResourcesOffListCmd == value)
                    return;
                _bResourcesOffListCmd = value;
            }
        }
        public bool bAllocResourcesMedCmd
        {
            get
            {
                return _bAllocResourcesMedCmd;
            }
            set
            {
                if (_bAllocResourcesMedCmd == value)
                    return;
                _bAllocResourcesMedCmd = value;
            }
        }
        public bool bAllocResourcesOffCmd
        {
            get
            {
                return _bAllocResourcesOffCmd;
            }
            set
            {
                if (_bAllocResourcesOffCmd == value)
                    return;
                _bAllocResourcesOffCmd = value;
            }
        }
        public bool bTranfResourcesCmd
        {
            get
            {
                return _bTranfResourcesCmd;
            }
            set
            {
                if (_bTranfResourcesCmd == value)
                    return;
                _bTranfResourcesCmd = value;
            }
        }
        public bool bResourceMaintenanceLog_Mgnt
        {
            get
            {
                return _bResourceMaintenanceLog_Mgnt;
            }
            set
            {
                if (_bResourceMaintenanceLog_Mgnt == value)
                    return;
                _bResourceMaintenanceLog_Mgnt = value;
            }
        }
        public bool bResourceMaintenanceLog_AddNewMgnt
        {
            get
            {
                return _bResourceMaintenanceLog_AddNewMgnt;
            }
            set
            {
                if (_bResourceMaintenanceLog_AddNewMgnt == value)
                    return;
                _bResourceMaintenanceLog_AddNewMgnt = value;
            }
        }
        public bool bClinicManager
        {
            get
            {
                return _bClinicManager;
            }
            set
            {
                _bClinicManager = value;
            }
        }
        public bool bParentClinicManager
        {
            get
            {
                return _bParentClinicManager;
            }
            set
            {
                _bParentClinicManager = value;
            }
        }
        public bool bFileManager
        {
            get
            {
                return _bFileManager;
            }
            set
            {
                _bFileManager = value;
            }
        }
        #endregion
        private void ClinicCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IClinicManagement>();
            var regVm = Globals.GetViewModel<IConsultationRoomStaff>();
            regModule.mainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void ClinicCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0476_G1_QuanLyPgKham.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ClinicTargetCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IClinicManagement>();
            var regVm = Globals.GetViewModel<IConsultationTargetEx>();
            regModule.mainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void ClinicTargetCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1814_G1_QLyTieuChiPK;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicTargetCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicTargetCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ClinicTimeCmd_In(object source)
        {

            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IClinicManagement>();
            var regVm = Globals.GetViewModel<ITimeSegment>();
            regModule.mainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void ClinicTimeCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0451_G1_QuanLyCaKham.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicTimeCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicTimeCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ClinicReportCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IClinicManagement>();
            var regVm = Globals.GetViewModel<IClinicReport>();
            regModule.mainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void ClinicReportCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1048_G1_BC.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ClinicReportCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ClinicReportCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ConsultCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IClinicManagement>();
            var regVm = Globals.GetViewModel<IConsultation>();
            regModule.mainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void ConsultCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0470_G1_QuanLyHoSoPK.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ConsultCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ConsultCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void PCLCmd_In(object source)
        {
            //SetHyperlinkSelectedStyle(source as HyperlinkButton);
            //Globals.PageName = Globals.TitleForm;
            MessageBox.Show(eHCMSResources.A0416_G1_Msg_InfoChuaLam);
        }
        public void PCLCmd(object source)
        {
            MessageBox.Show(eHCMSResources.A0416_G1_Msg_InfoChuaLam);

            //Globals.TitleForm = "CHƯA LÀM";
            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    PCLCmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(Globals.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (Globals.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            PCLCmd_In(source);
            //            Globals.msgb = null;
            //        }
            //    });
            //}
        }
        //▼====: #001
        //==== #001
        //private void ShelfManCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IRefShelf>();
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void ShelfManCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z1987_QLyKhoKe.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        ShelfManCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                ShelfManCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //private void FileInportCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IRefShelfImportFile>();
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void FileInportCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z1999_G1_DatHSoVaoKe.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        FileInportCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                FileInportCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //private void FileCheckInCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IRefMedicalFileCheckIn>();
        //    regVm.IsCheckIn = true;
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void FileCheckInCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z1994_G1_NhapHSo.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        FileCheckInCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                FileCheckInCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //private void FileCheckOutCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IRefMedicalFileCheckIn>();
        //    regVm.IsCheckIn = false;
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void FileCheckOutCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z1995_G1_XuatHSo.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        FileCheckOutCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                FileCheckOutCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //private void FileCodePrintCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IRefMedicalFileCodePrint>();
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void FileCodePrintCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z1988_InMaHS.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        FileCodePrintCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                FileCodePrintCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //private void FileCheckInFromRegCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IGetMedicalFileFromRegistration>();
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void FileCheckInFromRegCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z1989_XuatHSTuDK.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        FileCheckInFromRegCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                FileCheckInFromRegCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //private void FileCodeHistoryCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IMedicalFileCheckInHistory>();
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void FileCodeHistoryCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z2042_G1_LSGNHoSo.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        FileCodeHistoryCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                FileCodeHistoryCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //private void FileManCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IMedicalFileManagement>();
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void FileManCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z1968_G1_QLyHS.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        FileManCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                FileManCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //
        //==== #001
        //private void OutFileManCmd_In(object source)
        //{
        //    Globals.PageName = Globals.TitleForm;
        //    var regModule = Globals.GetViewModel<IClinicManagement>();
        //    var regVm = Globals.GetViewModel<IOutMedicalFileManagement>();
        //    regModule.mainContent = regVm;
        //    (regModule as Conductor<object>).ActivateItem(regVm);
        //}
        //public void OutFileManCmd(object source)
        //{
        //    Globals.TitleForm = eHCMSResources.Z3114_G1_OutFileManager.ToUpper();
        //    if (string.IsNullOrEmpty(Globals.PageName))
        //    {
        //        OutFileManCmd_In(source);
        //    }
        //    else if (Globals.PageName != Globals.TitleForm)
        //    {
        //        Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
        //        {
        //            if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
        //            {
        //                OutFileManCmd_In(source);
        //                GlobalsNAV.msgb = null;
        //            }
        //        });
        //    }
        //}
        //▲====: #001
        private void WorkingScheduleCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IClinicManagement>();
            var regVm = Globals.GetViewModel<IConsultationRoomStaff_V2>();
            regModule.mainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void WorkingScheduleCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2803_G1_LichLamViec.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                WorkingScheduleCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        WorkingScheduleCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private bool _bOvertimeClinicManager = true;
        public bool bOvertimeClinicManager
        {
            get
            {
                return _bOvertimeClinicManager;
            }
            set
            {
                if (_bOvertimeClinicManager == value)
                    return;
                _bOvertimeClinicManager = value;
            }
        }
        private void OvertimeWorkingScheduleCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IClinicManagement>();
            var regVm = Globals.GetViewModel<IOvertimeWorkingSchedule>();
            regModule.mainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
        }
        public void OvertimeWorkingScheduleCmd(object source)
        {
            Globals.TitleForm = "LỊCH LÀM VIỆC NGOÀI GIỜ";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OvertimeWorkingScheduleCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OvertimeWorkingScheduleCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void ReportOvertimeWorkingScheduleCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IClinicManagement>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.XRpt_GioLamThemBacSi;
            reportVm.strHienThi = Globals.TitleForm;
            reportVm.mXemChiTiet = false;
            reportVm.mXemIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            regModule.mainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        public void ReportOvertimeWorkingScheduleCmd(object source)
        {
            Globals.TitleForm = "Báo cáo giờ làm thêm của Bác sĩ";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ReportOvertimeWorkingScheduleCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ReportOvertimeWorkingScheduleCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
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
        #endregion
    }
}