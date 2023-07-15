using aEMR.Common.BaseModel;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ICreateDQGReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateDQGReportViewModel : ViewModelBase, ICreateDQGReport
    {
        [ImportingConstructor]
        public CreateDQGReportViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            gDQGReport = new DQGReport();
            gDQGReport.FromDate = Globals.GetCurServerDateTime();
            gDQGReport.ToDate = gDQGReport.FromDate;
        }
        #region Events
        public void CreateReportCmd()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCreateDQGReport(gDQGReport,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    DQGReport mDQGReport = contract.EndCreateDQGReport(asyncResult);
                                    if (mDQGReport == null || mDQGReport.DQGReportID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    }
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void SearchCmd()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDQGReports(Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    DQGReportCollection = contract.EndGetDQGReports(asyncResult);
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void clTransferDataToDQG_Click(object source)
        {
            if (source == null || !(source is DQGReport))
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDQGReportWithDetails((source as DQGReport).DQGReportID,
                            new AsyncCallback((asyncResult) =>
                            {
                                try
                                {
                                    DQGReport mDQGReport = contract.EndGetDQGReportWithDetails(asyncResult);
                                    if (mDQGReport == null || mDQGReport.DQGReportID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    }
                                    this.HideBusyIndicator();
                                    Coroutine.BeginExecute(TransferAllDataToDQG_Routine(mDQGReport));
                                }
                                catch (Exception ex)
                                {
                                    this.HideBusyIndicator();
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void gvReports_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is null || !(sender is DataGrid) || (sender as DataGrid).SelectedItem == null || !((sender as DataGrid).SelectedItem is DQGReport))
            {
                return;
            }
            GetReportDetails(((sender as DataGrid).SelectedItem as DQGReport).DQGReportID);
        }
        #endregion
        #region Methods
        private IEnumerator<IResult> TransferAllDataToDQG_Routine(DQGReport aDQGReport)
        {
            ILoggerDialog mLogView = Globals.GetViewModel<ILoggerDialog>();
            var mThread = new Thread(() =>
            {
                GlobalsNAV.ShowDialog_V4(mLogView, null, null, false, true);
            });
            mThread.Start();
            foreach (var item in aDQGReport.phieu_nhap.Where(x => string.IsNullOrEmpty(x.ma_phieu_nhap_quoc_gia)))
            {
                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_ChuyenPhieuNhap, item.ma_phieu_nhap));
                GlobalDrugsSystemAPIResultCode mResult = GlobalsNAV.PharmacyAPIImportInwardDrugInvoice(item);
                if (mResult == null || mResult.code != "200")
                {
                    mLogView.AppendLogMessage(mResult.mess);
                }
                else
                {
                    item.ma_phieu_nhap_quoc_gia = mResult.ma_phieu_nhap_quoc_gia;
                    yield return GenericCoRoutineTask.StartTask(UpdateCodeDQG_phieu_nhap, item, mLogView);
                }
            }
            foreach (var item in aDQGReport.don_thuoc.Where(x => string.IsNullOrEmpty(x.ma_don_thuoc_quoc_gia)))
            {
                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_ChuyenDonThuoc, item.ma_don_thuoc_co_so_kcb));
                GlobalDrugsSystemAPIResultCode mResult = GlobalsNAV.PharmacyAPIImportPrescription(item);
                if (mResult == null || mResult.code != "200")
                {
                    mLogView.AppendLogMessage(mResult.mess);
                }
                else
                {
                    item.ma_don_thuoc_quoc_gia = mResult.ma_don_thuoc_quoc_gia;
                    yield return GenericCoRoutineTask.StartTask(UpdateCodeDQG_don_thuoc, item, mLogView);
                }
            }
            foreach (var item in aDQGReport.hoa_don.Where(x => string.IsNullOrEmpty(x.ma_hoa_don_quoc_gia)))
            {
                if (aDQGReport.don_thuoc.Any(x => x.IssueID == item.IssueID && !string.IsNullOrEmpty(x.ma_don_thuoc_quoc_gia)))
                {
                    item.ma_don_thuoc_quoc_gia = aDQGReport.don_thuoc.First(x => x.IssueID == item.IssueID).ma_don_thuoc_quoc_gia;
                }
                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_ChuyenHoaDon, item.ma_hoa_don));
                GlobalDrugsSystemAPIResultCode mResult = GlobalsNAV.PharmacyAPIImportOutwardPrescription(item);
                if (mResult == null || mResult.code != "200")
                {
                    mLogView.AppendLogMessage(mResult.mess);
                }
                else
                {
                    item.ma_hoa_don_quoc_gia = mResult.ma_hoa_don_quoc_gia;
                    yield return GenericCoRoutineTask.StartTask(UpdateCodeDQG_hoa_don, item, mLogView);
                }
            }
            foreach (var item in aDQGReport.phieu_xuat.Where(x => string.IsNullOrEmpty(x.ma_phieu_xuat_quoc_gia)))
            {
                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_ChuyenPhieuXuat, item.ma_phieu_xuat));
                GlobalDrugsSystemAPIResultCode mResult = GlobalsNAV.PharmacyAPIImportOutwardInvoice(item);
                if (mResult == null || mResult.code != "200")
                {
                    mLogView.AppendLogMessage(mResult.mess);
                }
                else
                {
                    item.ma_phieu_xuat_quoc_gia = mResult.ma_phieu_xuat_quoc_gia;
                    yield return GenericCoRoutineTask.StartTask(UpdateCodeDQG_phieu_xuat, item, mLogView);
                }
            }
            yield return GenericCoRoutineTask.StartTask(UpdateCodeDQG_DQGReport, aDQGReport.DQGReportID, mLogView);
            SearchCmd();
            mLogView.IsFinished = true;
        }
        private void UpdateCodeDQG_phieu_nhap(GenericCoRoutineTask aGenTask, object aDQG_phieu_nhap, object aLogView)
        {
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            DQG_phieu_nhap mDQG_phieu_nhap = aDQG_phieu_nhap as DQG_phieu_nhap;
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new TransactionServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateCodeDQGReport(new DQGReport { phieu_nhap = new List<DQG_phieu_nhap> { mDQG_phieu_nhap } }, (byte)UpdateCodeDQGReportCase.phieu_nhap, Globals.DispatchCallback((asyncResult) => {
                            try
                            {
                                if (!mContract.EndUpdateCodeDQGReport(asyncResult))
                                {
                                    mLogView.AppendLogMessage(eHCMSResources.T0074_G1_I);
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogView.AppendLogMessage(ex.Message);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                                mFactory.Dispose();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    mLogView.AppendLogMessage(ex.Message);
                    aGenTask.ActionComplete(true);
                }
            });
            t.Start();
        }
        private void UpdateCodeDQG_don_thuoc(GenericCoRoutineTask aGenTask, object aDQG_don_thuoc, object aLogView)
        {
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            DQG_don_thuoc mDQG_don_thuoc = aDQG_don_thuoc as DQG_don_thuoc;
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new TransactionServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateCodeDQGReport(new DQGReport { don_thuoc = new List<DQG_don_thuoc> { mDQG_don_thuoc } }, (byte)UpdateCodeDQGReportCase.don_thuoc, Globals.DispatchCallback((asyncResult) => {
                            try
                            {
                                if (!mContract.EndUpdateCodeDQGReport(asyncResult))
                                {
                                    mLogView.AppendLogMessage(eHCMSResources.T0074_G1_I);
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogView.AppendLogMessage(ex.Message);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                                mFactory.Dispose();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    mLogView.AppendLogMessage(ex.Message);
                    aGenTask.ActionComplete(true);
                }
            });
            t.Start();
        }
        private void UpdateCodeDQG_hoa_don(GenericCoRoutineTask aGenTask, object aDQG_hoa_don, object aLogView)
        {
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            DQG_hoa_don mDQG_hoa_don = aDQG_hoa_don as DQG_hoa_don;
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new TransactionServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateCodeDQGReport(new DQGReport { hoa_don = new List<DQG_hoa_don> { mDQG_hoa_don } }, (byte)UpdateCodeDQGReportCase.hoa_don, Globals.DispatchCallback((asyncResult) => {
                            try
                            {
                                if (!mContract.EndUpdateCodeDQGReport(asyncResult))
                                {
                                    mLogView.AppendLogMessage(eHCMSResources.T0074_G1_I);
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogView.AppendLogMessage(ex.Message);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                                mFactory.Dispose();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    mLogView.AppendLogMessage(ex.Message);
                    aGenTask.ActionComplete(true);
                }
            });
            t.Start();
        }
        private void UpdateCodeDQG_phieu_xuat(GenericCoRoutineTask aGenTask, object aDQG_phieu_xuat, object aLogView)
        {
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            DQG_phieu_xuat mDQG_phieu_xuat = aDQG_phieu_xuat as DQG_phieu_xuat;
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new TransactionServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateCodeDQGReport(new DQGReport { phieu_xuat = new List<DQG_phieu_xuat> { mDQG_phieu_xuat } }, (byte)UpdateCodeDQGReportCase.phieu_xuat, Globals.DispatchCallback((asyncResult) => {
                            try
                            {
                                if (!mContract.EndUpdateCodeDQGReport(asyncResult))
                                {
                                    mLogView.AppendLogMessage(eHCMSResources.T0074_G1_I);
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogView.AppendLogMessage(ex.Message);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                                mFactory.Dispose();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    mLogView.AppendLogMessage(ex.Message);
                    aGenTask.ActionComplete(true);
                }
            });
            t.Start();
        }
        private void UpdateCodeDQG_DQGReport(GenericCoRoutineTask aGenTask, object aDQGReportID, object aLogView)
        {
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            long mDQGReportID = (long)aDQGReportID;
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new TransactionServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateCodeDQGReport(new DQGReport { DQGReportID = mDQGReportID }, (byte)UpdateCodeDQGReportCase.DQGReport, Globals.DispatchCallback((asyncResult) => {
                            try
                            {
                                if (!mContract.EndUpdateCodeDQGReport(asyncResult))
                                {
                                    mLogView.AppendLogMessage(eHCMSResources.Z2651_G1_ChuyenDuLieuChuaHoanTat);
                                }
                                else
                                {
                                    mLogView.AppendLogMessage(eHCMSResources.Z2651_G1_ChuyenDuLieuHoanTat);
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogView.AppendLogMessage(ex.Message);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                                mFactory.Dispose();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    mLogView.AppendLogMessage(ex.Message);
                    aGenTask.ActionComplete(true);
                }
            });
            t.Start();
        }
        private void GetReportDetails(long aReportID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDQGReportAllDetails(aReportID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var mReportDetails = contract.EndGetDQGReportAllDetails(asyncResult);
                                this.HideBusyIndicator();
                                if (mReportDetails != null && mReportDetails.Tables.Count > 0)
                                {
                                    GlobalsNAV.ShowDialog<IPreviewDQGReport>((mView) =>
                                    {
                                        mView.gReportDetails = mReportDetails;
                                        mView.ViewTable();
                                    }, null, false, true);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
        #region Properties
        private DQGReport _gDQGReport;
        private List<DQGReport> _DQGReportCollection;
        public DQGReport gDQGReport
        {
            get
            {
                return _gDQGReport;
            }
            set
            {
                _gDQGReport = value;
                NotifyOfPropertyChange(() => gDQGReport);
            }
        }
        public List<DQGReport> DQGReportCollection
        {
            get
            {
                return _DQGReportCollection;
            }
            set
            {
                _DQGReportCollection = value;
                NotifyOfPropertyChange(() => DQGReportCollection);
            }
        }
        private enum UpdateCodeDQGReportCase : byte
        {
            phieu_nhap = 0,
            don_thuoc = 1,
            hoa_don = 2,
            phieu_xuat = 3,
            DQGReport = 4
        }
        #endregion
    }
}