/*
20170217 #001 CMN: Add TotalInPtRevenue Report
*/
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.ExportExcel;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using Microsoft.Win32;
/*
 * 20190622 #001 TNHX: [BM0011874] Create report RptTongHopDoanhThuTheoKhoa 
 * 20191211 #002 TNHX: Add checkbox doen't count Patient did discharge
 */
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IRptDoanhThuTongHopNoiTru)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RptDoanhThuTongHopNoiTruViewModel : Conductor<object>, IRptDoanhThuTongHopNoiTru
        , IHandle<RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event>
    {
        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RptDoanhThuTongHopNoiTruViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            Authorization();
            //Globals.EventAggregator.Subscribe(this);

            DeptID = 0;
            DeptName = eHCMSResources.Z0044_G1_TatCaKhoa;

            //Load UC
            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = false;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            leftContent = UCRefDepartments_BystrV_DeptTypeViewModel;
            (this as Conductor<object>).ActivateItem(leftContent);
            //Load UC

            // TxD 02/08/2014: Use Globals Server Date instead
            //GetCurrentDate();
            DateTime todayDate = Globals.GetCurServerDateTime();
            FromDate = todayDate;
            ToDate = todayDate;
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            else
            {
                mExportToExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mTempTongHopDoanhThu_NTM, (int)oTransaction_ManagementEx.mExportToExcel, (int)ePermission.mView);
                mViewAndPrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                                 (int)eTransaction_Management.mTempTongHopDoanhThu_NTM, (int)oTransaction_ManagementEx.mViewAndPrint, (int)ePermission.mView);
            }
        }

        private bool _mExportToExcel = true;
        private bool _mViewAndPrint = true;

        public bool mExportToExcel
        {
            get
            {
                return _mExportToExcel;
            }
            set
            {
                if (_mExportToExcel == value)
                    return;
                _mExportToExcel = value;
                NotifyOfPropertyChange(() => mExportToExcel);
            }
        }

        public bool mViewAndPrint
        {
            get
            {
                return _mViewAndPrint;
            }
            set
            {
                if (_mViewAndPrint == value)
                    return;
                _mViewAndPrint = value;
                NotifyOfPropertyChange(() => mViewAndPrint);
            }
        }

        //public void GetCurrentDate()
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            IsLoading = true;

        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        DateTime date = contract.EndGetDate(asyncResult);
        //                        FromDate = date;
        //                        ToDate = date;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                    finally
        //                    {
        //                        IsLoading = false;
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

        private DateTime? _FromDate;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }

        private DateTime? _ToDate;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }

        private long _DeptID;
        public long DeptID
        {
            get { return _DeptID; }
            set
            {
                if (_DeptID != value)
                {
                    _DeptID = value;
                    NotifyOfPropertyChange(() => DeptID);
                }
            }
        }

        private bool _HasDischarge = false;
        public bool HasDischarge
        {
            get { return _HasDischarge; }
            set
            {
                if (_HasDischarge != value)
                {
                    _HasDischarge = value;
                    NotifyOfPropertyChange(() => HasDischarge);
                }
            }
        }

        public void btView(bool isDetail = false)
        {
            //==== #001
            /*
            if (CheckDateValid())
            {
                var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                proAlloc.TieuDeRpt = string.Format("Báo Cáo Tổng Hợp Doanh Thu NTM Từ Ngày {0} Đến Ngày {1}", FromDate.Value.ToString("dd/MM/yyyy"), ToDate.Value.ToString("dd/MM/yyyy"));
                proAlloc.TieuDeRpt1 = DeptName;
                proAlloc.FromDate = FromDate;
                proAlloc.ToDate = ToDate;
                proAlloc.DeptID = DeptID;
                proAlloc.eItem = ReportName.RptTongHopDoanhThuNoiTru;

                var instance = proAlloc as Conductor<object>;
                Globals.ShowDialog(instance, (o) => { });
            }
            */
            if (CheckDateValid())
            {
                //▼====: #001
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.FromDate = FromDate;
                //proAlloc.ToDate = ToDate;
                //proAlloc.DeptID = DeptID;
                //proAlloc.eItem = ReportName.RptTongHopDoanhThuNoiTru;
                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.DeptID = DeptID;
                    proAlloc.DeptName = DeptName;
                    proAlloc.IsDetails = isDetail;
                    proAlloc.HasDischarge = !HasDischarge;
                    proAlloc.eItem = ReportName.RptTongHopDoanhThuNoiTru;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                //▲====: #001
            }
            //==== #001
        }

        public void btViewDetail()
        {
            btView(true);
        }

        private bool CheckDateValid()
        {
            if (FromDate != null && ToDate != null)
            {
                DateTime F = DateTime.Now;
                DateTime.TryParse(FromDate.ToString(), out F);

                DateTime T = DateTime.Now;
                DateTime.TryParse(ToDate.ToString(), out T);

                if (F > T)
                {
                    MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0884_G1_Msg_InfoNhapTuNgDenNg);
                return false;
            }
        }

        public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        {
            if (message != null)
            {
                RefDepartmentsTree NodeTree = message.ObjRefDepartments_Current as RefDepartmentsTree;

                if (NodeTree.ParentID != null)
                {
                    DeptID = NodeTree.NodeID;
                    DeptName = NodeTree.NodeText.Trim();
                }
                else
                {
                    DeptID = 0;
                    DeptName = eHCMSResources.Z0044_G1_TatCaKhoa;
                }
            }
        }

        private string _DeptName;
        public string DeptName
        {
            get { return _DeptName; }
            set
            {
                if (_DeptName != value)
                {
                    _DeptName = value;
                    NotifyOfPropertyChange(() => DeptName);
                }
            }
        }

        #region Export excel from database

        public void btnExportExcel()
        {
            //==== #001
            //if (CheckDateValid())
            //{
            //    //ExportToExcellAll_DoanhThuNoiTruTongHop();
            //    SaveFileDialog objSFD = new SaveFileDialog()
            //    {
            //        DefaultExt = ".xls",
            //        Filter = "Excel xls (*.xls)|*.xls",
            //        FilterIndex = 1
            //    };
            //    if (objSFD.ShowDialog() != true)
            //    {
            //        return;
            //    }
            //    IsLoading = true;
            //    ReportParameters RptParameters = new ReportParameters { FromDate = FromDate, ToDate = ToDate, DeptID = DeptID };
            //    RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
            //    RptParameters.Show = "TongHopDoanhThuNoiTru";
            //    RptParameters.reportName = ReportName.RptTongHopDoanhThuNoiTru;

            //    ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            //    //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
            //    //ExportToExcellAllGeneric(objSFD, RptParameters);
            //}
            if (CheckDateValid())
            {
                SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = ".xls", Filter = "Excel xls (*.xls)|*.xls", FilterIndex = 1 };
                if (objSFD.ShowDialog() != true)
                    return;
                this.ShowBusyIndicator();
                ReportParameters RptParameters = new ReportParameters { FromDate = FromDate, ToDate = ToDate, DeptID = DeptID, HasDischarge = !HasDischarge };
                //▼====: #001
                //RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                //RptParameters.Show = "TongHopDoanhThuNoiTru";
                //RptParameters.reportName = ReportName.RptTongHopDoanhThuNoiTru;                
                RptParameters.ReportType = ReportType.BAOCAO_TONGHOP_KT;
                RptParameters.Show = DeptName;
                RptParameters.reportName = ReportName.RptTongHopDoanhThuTheoKhoa;
                //▲====: #001
                ExportToExcelGeneric.Action(RptParameters, objSFD, this);
                this.HideBusyIndicator();
            }
            //==== #001
        }
        //private IEnumerator<IResult> DoSaveExcel(ReportParameters rptParameters, SaveFileDialog objSFD)
        //{
        //    var res = new ExportToExcellAllGenericTask(rptParameters, objSFD);
        //    yield return res;
        //    IsLoading = false;
        //    yield break;
        //}
        public void ExportToExcellAll_DoanhThuNoiTruTongHop()
        {
            ReportParameters RptParameters = new ReportParameters { FromDate = FromDate, ToDate = ToDate, DeptID = DeptID };
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_DoanhThuNoiTruTongHop(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_DoanhThuNoiTruTongHop(asyncResult);
                            ExportToExcelFileAllData.Export(results, "DoanhThuNoiTruTongHop");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion
    }
}
