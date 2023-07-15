using System;
using System.ComponentModel.Composition;
using System.Threading;
using Ax.ViewContracts.SL;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using eHCMSLanguage;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using System.Windows.Controls;
/*
 * 20200906 #001 TNHX:   BM: [QMS] Thêm tích tự động gọi STT sau khi trả tiền
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof (IRecalQMSTicket)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecalQMSTicketViewModel: Conductor<object>, IRecalQMSTicket
    {
        public RecalQMSTicketViewModel()
        {            
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
        //public void RecalQMSTicketCmd()
        //{
        //    string SerialTicketAfterSub = "";
        //    if (Globals.DeptLocation == null)
        //    {
        //        return;
        //    }
        //    if (!string.IsNullOrEmpty(SerialTicket))
        //    {
        //        int countStr = SerialTicket.Length;
        //        if (countStr < 7)
        //        {
        //            return;
        //        }
        //        SerialTicketAfterSub = SerialTicket.Trim().Substring(3, countStr - 3).ToString();
        //    }
        //    var t = new Thread(() =>
        //    {
        //        using (var mFactory = new QMSService.QMSServiceClient())
        //        {
        //            try
        //            {
        //                var mContract = mFactory.ServiceInstance;
        //                mContract.BeginRecalQMSTicket(Globals.DeptLocation.DeptLocationID, SerialTicketAfterSub, TicketRecalTime, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        var mNextTicketIssue = mContract.EndRecalQMSTicket(asyncResult);
        //                        if (mNextTicketIssue != null && mNextTicketIssue.TicketID > 0)
        //                        {
        //                            Globals.EventAggregator.Publish(new SetTicketIssueForPatientRegistrationView { Item = mNextTicketIssue, IsLoadPatientInfo = true });
        //                            Globals.EventAggregator.Publish(new SetTicketNumnerTextForPatientRegistrationView { Item = mNextTicketIssue });
        //                            this.TryClose();
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("Số thứ tự không hợp lệ.");
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                    }
        //                    finally
        //                    {
        //                    }
        //                }), null);
        //            }
        //            catch (Exception ex)
        //            {
        //                Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
        //            }
        //        }
        //    });
        //    t.Start();
        //}
        //▼===== #001
        public void RecalQMSTicketCmd()
        {
            string SerialTicketAfterSub = "";
            if (Globals.DeptLocation == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(SerialTicket))
            {
                int countStr = SerialTicket.Length;
                if (countStr < 7)
                {
                    return;
                }
                SerialTicketAfterSub = SerialTicket.Trim().Substring(3, countStr - 3).ToString();
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginCallSpecialPriorTicket(Globals.DeptLocation.DeptLocationID, SerialTicketAfterSub, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mNextTicketIssue = mContract.EndCallSpecialPriorTicket(asyncResult);
                                if (mNextTicketIssue != null && mNextTicketIssue.TicketNumberSeq > 0)
                                {
                                    Globals.EventAggregator.Publish(new SetTicketIssueForPatientRegistrationView { Item = mNextTicketIssue, IsLoadPatientInfo = true });
                                    //Globals.EventAggregator.Publish(new SetTicketNumnerTextForPatientRegistrationView { Item = mNextTicketIssue });
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show("Số thứ tự không hợp lệ.");
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

        private bool _AutoCalQMSTicket = true;
        public bool AutoCalQMSTicket
        {
            get { return _AutoCalQMSTicket; }
            set
            {
                _AutoCalQMSTicket = value;
                NotifyOfPropertyChange(() => AutoCalQMSTicket);
            }
        }

        CheckBox chkAutoCalQMSTicket;
        public void chkAutoCalQMSTicket_Loaded(object sender, RoutedEventArgs e)
        {
            chkAutoCalQMSTicket = sender as CheckBox;
        }

        public void chkAutoCalQMSTicket_UnCheck(object sender, RoutedEventArgs e)
        {
            if (chkAutoCalQMSTicket == null)
            {
                return;
            }
            chkAutoCalQMSTicket.IsChecked = true;
        }
        //▲===== #001
    }
}
