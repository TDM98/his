using aEMR.Common;
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
/*
 * 20190612 #001 TTM:   BM 0011807: Tách tạo và chuyển dữ liệu ra thành 2 bước để kế toán có thể xem trước khi chuyển dữ liệu.
 * 20190628 #002 TTM:   BM 0011903: Thêm chức năng xoá báo cáo nếu như chưa chuyển đổi dữ liệu sang cho FAST.
 */
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ICreateFastReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateFastReportViewModel : ViewModelBase, ICreateFastReport
    {
        [ImportingConstructor]
        public CreateFastReportViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            gReport.Staff = new Staff();
            gReport.Staff = Globals.LoggedUserAccount.Staff;
            gReport.FromDate = Globals.GetCurServerDateTime().Date;
        }
        #region Properties
        private HealthInsuranceReport _gReport = new HealthInsuranceReport();
        private List<HealthInsuranceReport> _gReportCollection;
        public HealthInsuranceReport gReport
        {
            get
            {
                return _gReport;
            }
            set
            {
                _gReport = value;
                NotifyOfPropertyChange(() => gReport);
            }
        }
        public List<HealthInsuranceReport> gReportCollection
        {
            get
            {
                return _gReportCollection;
            }
            set
            {
                if (_gReportCollection != value)
                {
                    _gReportCollection = value;
                    NotifyOfPropertyChange(() => gReportCollection);
                }
            }
        }
        private long _V_FastReportType;
        public long V_FastReportType
        {
            get
            {
                return _V_FastReportType;
            }
            set
            {
                _V_FastReportType = value;
                NotifyOfPropertyChange(() => V_FastReportType);
            }
        }
        private string _TitleForm;
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        #endregion
        #region Events
        public void CreateReportCmd()
        {
            if (gReport == null || string.IsNullOrEmpty(gReport.Title))
            {
                return;
            }
            if (gReportCollection != null && gReportCollection.Any(x => x.FromDate == gReport.FromDate))
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
                        contract.BeginCreateFastReport(gReport, V_FastReportType,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    long FastReportID;
                                    var result = contract.EndCreateFastReport(out FastReportID, asyncResult);
                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.A0998_G1_Msg_InfoTaoBCOK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0999_G1_Msg_InfoTaoBCFail);
                                    }
                                    GetFastReports();
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.Message);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void SearchCmd()
        {
            GetFastReports();
        }
        public void gvReports_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void gvReports_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is null || !(sender is DataGrid) || (sender as DataGrid).SelectedItem == null || !((sender as DataGrid).SelectedItem is HealthInsuranceReport))
            {
                return;
            }
            GetFastReportDetails(((sender as DataGrid).SelectedItem as HealthInsuranceReport).HIReportID);
        }
        #endregion
        #region Methods
        private void GetFastReports()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetFastReports(V_FastReportType,Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                gReportCollection = contract.EndGetFastReports(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetFastReportDetails(long FastReportID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetFastReportDetails(FastReportID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var mFastReportDetails = contract.EndGetFastReportDetails(asyncResult);
                                this.HideBusyIndicator();
                                if (mFastReportDetails != null && mFastReportDetails.Tables.Count > 0)
                                {
                                    GlobalsNAV.ShowDialog<IPreviewFastReport>((mView) =>
                                    {
                                        mView.V_FastReportType = V_FastReportType;
                                        mView.FastReportID = FastReportID;
                                        mView.FastReportDetails = mFastReportDetails;
                                        mView.ViewTable();
                                    }, null, false, true);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
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
        //▼===== #001
        public void TransferDataToFAST_Click(object source)
        {
            if (source == null || !(source is HealthInsuranceReport))
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
                        contract.BeginTransferFastReport((source as HealthInsuranceReport).HIReportID,
                            new AsyncCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool result = contract.EndTransferFastReport(asyncResult);
                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.Z2651_G1_ChuyenDuLieuHoanTat);
                                        SearchCmd();
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
        //▲===== #001
        //▼===== #002
        public void DeleteReportBeforeTransfer_Click(object source)
        {
            if (source == null || !(source is HealthInsuranceReport))
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
                        contract.BeginDeleteFastReport((source as HealthInsuranceReport).HIReportID,
                            new AsyncCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool result = contract.EndDeleteFastReport(asyncResult);
                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                        SearchCmd();
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
        //▲===== #002
        #endregion
    }
}