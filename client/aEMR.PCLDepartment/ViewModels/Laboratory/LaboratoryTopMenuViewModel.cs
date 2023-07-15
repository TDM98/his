using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
/*
* 20190419 #001 TNHX: [BM0006757] create report "BCThoiGianBNChoXN"
* 20220517 #002 DatTB: Báo cáo danh sách dịch vụ kỹ thuật xét nghiệm
* 20220518 #003 DatTB: Báo cáo lượt xét nghiệm
* 20220824 #004 BLQ: Thêm màn hình quản lý giao dịch chữ ký số. Thêm phân màn hình
*/
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ILaboratoryTopMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LaboratoryTopMenuViewModel : Conductor<object>, ILaboratoryTopMenu
         , IHandle<LocationSelected>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LaboratoryTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            
            Globals.PageName = "";
            Globals.TitleForm = "";
            eventAggregator.Subscribe(this);
            authorization();
        }
        
        //ILeftMenuView _currentView;
        //protected override void OnViewLoaded(object view)
        //{
        //    base.OnViewLoaded(view);
        //    _currentView = view as ILeftMenuView;
        //    if (_currentView != null)
        //    {
        //        _currentView.ResetMenuColor();
        //    }
        //}

        private void PCLRequest_Cmd_In(object source)
        {
            //31072018 TTM TopMenu không xài hàm này 
            //SetHyperlinkSelectedStyle(source as Button);

            Globals.PageName = Globals.TitleForm;

           // var loginVm = Globals.GetViewModel<ILogin>();
            //khong can vi khi dang nhap da chon phong roi,neu tim ko dc thi nguoi dung chon lai phong la dc
            //if (loginVm.DeptLocation == null
            //    ||Globals.V_DeptTypeOperation != V_DeptTypeOperation.KhoaCanLamSang) //Chưa chọn phòng => Yêu cầu Chọn phòng.
            //{
            //    var locationVm = Globals.GetViewModel<ISelectLocation>();
            //    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
            //    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
            //    locationVm.ItemActivated = Globals.GetViewModel<ILaboratoryResultHome>();
            //    Globals.ShowDialog(locationVm as Conductor<object>);
            //}
            //else
            {
                var regModule = Globals.GetViewModel<ILaboratoryHome>();
                var regVm = Globals.GetViewModel<ILaboratoryResultHome>();
                regModule.MainContent = regVm;
                (regModule as Conductor<object>).ActivateItem(regVm);

                KhoiTaoPCLDepartmentLAB();

                //Load OutStandingstask
                var shell = Globals.GetViewModel<IHome>();
                //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();
                var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<ILaboratoryResultList>();
                

                if(Globals.PatientFindBy_ForLab == null)
                    Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;

                //UCPCLDepartmentOutstandingTaskView.PatientFindBy = Globals.PatientFindBy_ForLab.Value;

                shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
                shell.IsExpandOST = true;
                //(shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
            }
        }

        public void PCLRequest_Cmd(object source)
        {
            //do chi co mot link nen cho no refresh lai
            Globals.TitleForm = eHCMSResources.G2600_G1_XN;
            PCLRequest_Cmd_In(source);
            //nhieu hon se unmark lại chỗ này

            //if (string.IsNullOrEmpty(Globals.PageName))
            //{
            //    PCLRequest_Cmd_In(source);
            //}
            //else if (Globals.PageName != Globals.TitleForm)
            //{
            //    Coroutine.BeginExecute(Globals.DoMessageBox(), null, (o, e) =>
            //    {
            //        if (Globals.msgb.Result == AxMessageBoxResult.Ok)
            //        {
            //            PCLRequest_Cmd_In(source);
            //            Globals.msgb = null;
            //        }
            //    });
            //}
        }

        public void ListPatientRunTestLaboratory_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2426_G1_DsachBNChiDinhThucHienXN;
            ListPatientRunTestLaboratory_Cmd_In(source);
        }

        private void ListPatientRunTestLaboratory_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.DsachBNChiDinhThucHienXN;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;   
            reportVm.ShowPCLSection = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        //▼==== #002
        public void BCDsachDichVuKyThuatXN_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3243_G1_BCDsachDichVuKyThuatXN;
            BCDsachDichVuKyThuatXN_Cmd_In(source);
        }

        private void BCDsachDichVuKyThuatXN_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCDsachDichVuKyThuatXN;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.RptParameters.HidePCLStatus = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.ShowPCLSection = false;
            reportVm.ShowPCLExamType = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #002

        //▼==== #003
        public void BaoCaoLuotXetNghiem_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3244_G1_BaoCaoLuotXetNghiem;
            BaoCaoLuotXetNghiem_Cmd_In(source);
        }

        private void BaoCaoLuotXetNghiem_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BaoCaoLuotXetNghiem;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.RptParameters.HidePCLStatus = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.ShowPCLSection = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲==== #003

        public void TinhHinhHoatDongCLS_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2427_G1_TinhHinhHoatDongCLS;
            TinhHinhHoatDongCLS_Cmd_In(source);
        }

        private void TinhHinhHoatDongCLS_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.TinhHinhHoatDongCLS_XN;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = false;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.ShowPCLSection = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }

        
        //▼======: #001
        public void BCThoiGianBNChoXN_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z2427_G1_TinhHinhHoatDongCLS;
            BCThoiGianBNChoXN_Cmd_In(source);
        }

        private void BCThoiGianBNChoXN_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BCThoiGianBNChoXN;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mIn = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▼======: #001

        public void Handle(LocationSelected message)
        {
            if (message != null && message.DeptLocation != null)
            {
                KhoiTaoPCLDepartmentLAB();

                var regModule = Globals.GetViewModel<ILaboratoryHome>();
                if (message.ItemActivated == null)
                {
                    //không làm gì hết vì chưa có load Item nào lên, khi đó dựa vào chọn phòng ở Top sẽ load lại
                    var locationVm = Globals.GetViewModel<ISelectLocation>();
                    locationVm.ItemActivated = Globals.GetViewModel<ILaboratoryResultHome>();
                    locationVm.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
                    Globals.V_DeptTypeOperation = V_DeptTypeOperation.KhoaCanLamSang;
                    var Module = Globals.GetViewModel<ILaboratoryHome>();
                    var VM = Globals.GetViewModel<ILaboratoryResultHome>();

                    Module.MainContent = VM;
                    (Module as Conductor<object>).ActivateItem(VM);


                    //Load OutStandingstask
                    var shell = Globals.GetViewModel<IHome>();
                    //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();
                    var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<ILaboratoryResultList>();


                    
                    if (Globals.PatientFindBy_ForLab==null)
                        Globals.PatientFindBy_ForLab= AllLookupValues.PatientFindBy.NGOAITRU;
                    
                    //UCPCLDepartmentOutstandingTaskView.PatientFindBy = Globals.PatientFindBy_ForLab.Value;

                    shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
                    (shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
                    shell.IsExpandOST = true;
                }
                else
                {

                    regModule.MainContent = message.ItemActivated;
                    (regModule as Conductor<object>).ActivateItem(message.ItemActivated);

                    //Load OutStandingstask
                    var shell = Globals.GetViewModel<IHome>();
                    //var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<IPCLDepartmentOutstandingTask>();
                    var UCPCLDepartmentOutstandingTaskView = Globals.GetViewModel<ILaboratoryResultList>();
                    shell.OutstandingTaskContent = UCPCLDepartmentOutstandingTaskView;
                    shell.IsExpandOST = true;

                    if (Globals.PatientFindBy_ForLab == null)
                        Globals.PatientFindBy_ForLab = AllLookupValues.PatientFindBy.NGOAITRU;

                    //UCPCLDepartmentOutstandingTaskView.PatientFindBy =Globals.PatientFindBy_ForLab.Value;

                    (shell as Conductor<object>).ActivateItem(UCPCLDepartmentOutstandingTaskView);
                }
            }
        }

        private void KhoiTaoPCLDepartmentLAB()
        {
            Globals.PCLDepartment.ObjPCLExamTypeLocationsDeptLocationID = Globals.DeptLocation;
            Globals.PCLDepartment.ObjV_PCLMainCategory = new Lookup();
            Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID = new PCLExamTypeSubCategory();
            Globals.PCLDepartment.ObjPCLResultParamImpID = new PCLResultParamImplementations();

            //Globals.PatientAllDetails.PatientInfo = null;

            ////Show Info xóa thông tin BN trước đó
            //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = null, PtReg = null, PtRegDetail = null });
            ////Show Info xóa thông tin BN trước đó
        }

        public void SoXetNghiem_Cmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3089_G1_SoXetNghiem;
            SoXetNghiem_Cmd_In(source);
        }

        private void SoXetNghiem_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
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
        //▼====: #004
        private void DigitalSignTransaction_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var regVm = Globals.GetViewModel<IDigitalSignTransaction>();
            regModule.MainContent = regVm;
            (regModule as Conductor<object>).ActivateItem(regVm);
            var shell = Globals.GetViewModel<IHome>();
            shell.OutstandingTaskContent = null;
            shell.IsExpandOST = false;
        }

        public void DigitalSignTransaction_Cmd(object source)
        {
            Globals.TitleForm = "QUẢN LÝ GIAO DỊCH CHỮ KÝ SỐ".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DigitalSignTransaction_Cmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DigitalSignTransaction_Cmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        public void BaoCaoDSBenhNhanCapNhatKetQua_Cmd(object source)
        {
            Globals.TitleForm = "Báo cáo Danh sách BN cập nhật KQ".ToUpper();
            BaoCaoDSBenhNhanCapNhatKetQua_Cmd_In(source);
        }

        private void BaoCaoDSBenhNhanCapNhatKetQua_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BaoCaoDSBenhNhanCapNhatKetQua;
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
        public void BaoCaoXNChuaNhapKetQua_Cmd(object source)
        {
            Globals.TitleForm = "Báo cáo Xét nghiệm chưa nhập Kết quả".ToUpper();
            BaoCaoXNChuaNhapKetQua_Cmd_In(source);
        }

        private void BaoCaoXNChuaNhapKetQua_Cmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<ILaboratoryHome>();
            var reportVm = Globals.GetViewModel<ICommonReportByDDMMYYYY>();
            reportVm.eItem = ReportName.BaoCaoXNChuaNhapKetQua;
            reportVm.mDepartment = false;
            reportVm.mXemChiTiet = false;
            reportVm.mInPatientDeptStatus = false;
            reportVm.mRegistrationType = false;
            reportVm.isAllStaff = false;
            reportVm.RptParameters.HideFindPatient = true;
            reportVm.IsEnabledToDatePicker = true;
            reportVm.mXemIn = false;
            regModule.MainContent = reportVm;
            ((Conductor<object>)regModule).ActivateItem(reportVm);
        }
        //▲====: #004
        #region menu
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //if (_currentView != null)
            //{
            //    _currentView.ResetMenuColor();
            //}
        }

        //31072018 TTM TopMenu không cần xài hàm này
        //private void SetHyperlinkSelectedStyle(Button lnk)
        //{
        //    if (_currentView != null)
        //    {
        //        _currentView.ResetMenuColor();
        //    }
        //    if (lnk != null)
        //    {
        //        lnk.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
        //    }
        //}
        #endregion
        //▼====: #004
        #region authorization
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mGiaoDichKySo = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mCLSLaboratory,(int)eCLSLaboratory.mGiaoDich_KySo);
        }
        private bool _mGiaoDichKySo = true;
        public bool mGiaoDichKySo
        {
            get
            {
                return _mGiaoDichKySo;
            }
            set
            {
                if (_mGiaoDichKySo == value)
                    return;
                _mGiaoDichKySo = value;
                NotifyOfPropertyChange(() => mGiaoDichKySo);
            }
        }
        #endregion
        //▲====: #004
    }
}