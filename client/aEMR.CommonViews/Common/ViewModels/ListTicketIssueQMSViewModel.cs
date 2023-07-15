using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using eHCMSLanguage;
using System.Windows;
using Ax.ViewContracts.SL;
using aEMR.CommonTasks;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IListTicketIssueQMS)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ListTicketIssueQMSViewModel: Conductor<object>, IListTicketIssueQMS
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ListTicketIssueQMSViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            LoadAllTicketQueueByDeptLocID();
            LoadCounterStatusByDeptLocID();
        }
        private string _LabelbtnChangeCounterStatus = "";
        public string LabelbtnChangeCounterStatus
        {
            get
            {
                return _LabelbtnChangeCounterStatus;
            }
            set
            {
                _LabelbtnChangeCounterStatus = value;
                NotifyOfPropertyChange(() => LabelbtnChangeCounterStatus);
            }
        }
        private string _SerialTicket = "";
        public string SerialTicket
        {
            get
            {
                return _SerialTicket;
            }
            set
            {
                _SerialTicket = value;
                NotifyOfPropertyChange(() => SerialTicket);
            }
        }
        private DateTime _TicketRecalTime = Globals.GetCurServerDateTime();
        public DateTime TicketRecalTime
        {
            get
            {
                return _TicketRecalTime;
            }
            set
            {
                _TicketRecalTime = value;
                NotifyOfPropertyChange(() => TicketRecalTime);
            }
        }
        private List<TicketIssue> _ObjTicketIssueList;
        public List<TicketIssue> ObjTicketIssueList
        {
            get { return _ObjTicketIssueList; }
            set
            {
                if (_ObjTicketIssueList != value)
                {
                    _ObjTicketIssueList = value;
                    NotifyOfPropertyChange(() => ObjTicketIssueList);
                }
            }
        }
        private bool IsClosed;
        public void LoadAllTicketQueueByDeptLocID()
        {
            if (Globals.DeptLocation == null)
            {
                return;
            }
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetAllTicketIssueByCounter(Globals.DeptLocation.DeptLocationID, TicketRecalTime, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {                                
                                ObjTicketIssueList = mContract.EndGetAllTicketIssueByCounter(asyncResult);
                                List<TicketIssue> ObjTicketIssueList_UT = ObjTicketIssueList.Where(x => x.TicketTypeID == (long)V_TicketType_Enum.UT).OrderBy(y => y.TicketNumberSeq).ToList();
                                List<TicketIssue> ObjTicketIssueList_Rest = ObjTicketIssueList.Where(x => x.TicketTypeID != (long)V_TicketType_Enum.UT).OrderBy(y => y.TicketNumberSeq).ToList();
                                var ObjTicketIssueList_tmp = new List<TicketIssue>();
                                ObjTicketIssueList_tmp = ObjTicketIssueList_UT;
                                if (ObjTicketIssueList_Rest != null && ObjTicketIssueList_Rest.Count > 0)
                                {
                                    foreach (var item in ObjTicketIssueList_Rest)
                                    {
                                        ObjTicketIssueList_tmp.Add(item);
                                    }
                                }
                                ObjTicketIssueList = ObjTicketIssueList_tmp;
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
        public void LoadCounterStatusByDeptLocID()
        {
            if (Globals.DeptLocation == null)
            {
                return;
            }
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetWorkingCounterStatus(Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                IsClosed = mContract.EndGetWorkingCounterStatus(asyncResult);
                                if (IsClosed)
                                {

                                    LabelbtnChangeCounterStatus = eHCMSResources.Z2964_G1_MoQuay;
                                }
                                else
                                {
                                    LabelbtnChangeCounterStatus = eHCMSResources.Z2965_G1_DongQuay;
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
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }
        public void gridQMSList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            TicketIssue ObjRow = e.Row.DataContext as TicketIssue;

            if (ObjRow !=null && ObjRow.TicketTypeID == (long)V_TicketType_Enum.UT)
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
                NotifyOfPropertyChange(() => e.Row.Background);
            }
        }
        public void btnChangeCounterStatus_Click(System.Windows.RoutedEventArgs e)
        {
            if (IsClosed == true)
            {
                if (MessageBox.Show(eHCMSResources.Z2966_G1_XacNhanMoQuay, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Coroutine.BeginExecute(btSaveCoroutine());
                }
            }
            else
            {
                if (MessageBox.Show(eHCMSResources.Z2967_G1_XacNhanDongQuay, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Coroutine.BeginExecute(btSaveCoroutine());
                }
            }

        }
        public IEnumerator<IResult> btSaveCoroutine()
        {

            yield return GenericCoRoutineTask.StartTask(ChangeWorkingCounterStatus);
            LoadCounterStatusByDeptLocID();

        }
        private void ChangeWorkingCounterStatus(GenericCoRoutineTask genTask)
        {
            if (Globals.DeptLocation == null || Globals.LoggedUserAccount.Staff == null)
            {
                if (genTask != null)
                {
                    genTask.ActionComplete(true);
                }
                return;
            }
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        bool LoadCompleted = false;
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginChangeWorkingCounterStatus(Globals.DeptLocation.DeptLocationID, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                LoadCompleted = mContract.EndChangeWorkingCounterStatus(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(LoadCompleted);
                                }
                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                        this.HideBusyIndicator();
                        if (genTask != null)
                        {
                            genTask.ActionComplete(false);
                        }
                    }
                }
            });
            t.Start();

        }
    }
}
