using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Printing;
using System.Text;
using System.Windows.Media;

namespace aEMR.Common.ViewModels
{
    /// <summary>
    /// Quản lý danh sách CLS của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    [Export(typeof(IPatientPayment_InPt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPayment_InPtViewModel : Conductor<object>, IPatientPayment_InPt
        , IHandle<TranPaymentUpdateEvent>
    {
        public PatientPayment_InPtViewModel()
        {
            Globals.EventAggregator.Subscribe(this);
        }
        #region PROPERTIES
        private PagedCollectionView.PagedCollectionView _patientPayments;
        public PagedCollectionView.PagedCollectionView PatientPayments
        {
            get { return _patientPayments; }
            set
            {
                if (_patientPayments != value)
                {
                    _patientPayments = value;
                    NotifyOfPropertyChange(() => PatientPayments);
                }
            }
        }
        private bool _IsUserAdmin = Globals.IsUserAdmin;
        public bool IsUserAdmin
        {
            get
            {
                return _IsUserAdmin;
            }
            set
            {
                if (_IsUserAdmin == value)
                {
                    return;
                }
                _IsUserAdmin = value;
                NotifyOfPropertyChange(() => IsUserAdmin);
            }
        }
        #endregion
        public bool CanPrintReceiptCmd
        {
            get
            {
                return true;
            }
        }
        public void GridInward_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            PatientTransactionPayment p = e.Row.DataContext as PatientTransactionPayment;
            if (p != null && p.IsDeleted.GetValueOrDefault())
            {
                //e.Row.Background = new SolidColorBrush(Colors.Yellow);
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                e.Row.FontWeight = FontWeights.Bold;
            }
        }
        public void PrintReceiptCmd(PatientTransactionPayment source, object eventArgs)
        {
            if (source == null || source.PtTranPaymtID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0743_G1_Msg_InfoKhTimThayMaHDon), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PaymentID = source.PtTranPaymtID;
                proAlloc.eItem = ReportName.REGISTRATION_IN_PT_REPAYCASHADVANCE;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void DeleteReceiptCmd(PatientTransactionPayment source, object eventArgs)
        {
            if (source == null || source.PtTranPaymtID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0743_G1_Msg_InfoKhTimThayMaHDon), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.LoggedUserAccount == null || !Globals.LoggedUserAccount.StaffID.HasValue || Globals.LoggedUserAccount.StaffID.Value == 0)
            {
                return;
            }
            if (source.PaymentMode.LookupID == 4803)
            {
                StringBuilder _StrBuilder = new StringBuilder();
                _StrBuilder.AppendLine("[Không Thể Hủy Phiếu]");
                _StrBuilder.AppendLine(null);
                _StrBuilder.AppendLine("Phiếu hoàn trả này đã được thanh toán qua [Thẻ Khám Bệnh].");
                Globals.ShowMessage(_StrBuilder.ToString(), eHCMSResources.T0432_G1_Error);
                return;
            }
            var mThread = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteTranacsionPayment_InPt(source.ReceiptNumber, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndDeleteTranacsionPayment_InPt(asyncResult);
                            if (source.PatientTransaction != null
                                && source.PatientTransaction.PatientRegistration != null
                                && source.PatientTransaction.PatientRegistration.PtRegistrationID > 0)
                            {
                                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = source.PatientTransaction.PatientRegistration });
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
        public void PrintReceipt(PatientTransactionPayment payment)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Root>");
            sb.Append("<IDList>");
            sb.AppendFormat("<ID>{0}</ID>", payment.PtTranPaymtID);
            sb.Append("</IDList>");
            sb.Append("</Root>");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        try
                        {
                            contract.BeginGetOutPatientReceiptReportXMLInPdfFormat(sb.ToString(), Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var data = contract.EndGetOutPatientReceiptReportXMLInPdfFormat(asyncResult);

                                    var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_HOA_DON, data, ActiveXPrintType.ByteArray);
                                    Globals.EventAggregator.Publish(printEvt);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                }
                            }), null);
                        }
                        catch (Exception innerEx)
                        {
                            ClientLoggerHelper.LogInfo(innerEx.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
        public void hplUpdateNotes_Click(object sender)
        {
            Action<IPaymentUpdateNote> onInitDlg = (typeInfo) =>
            {
                typeInfo.CurPaymentInfo = ObjectCopier.DeepCopy(sender as PatientTransactionPayment);
            };
            GlobalsNAV.ShowDialog<IPaymentUpdateNote>(onInitDlg);
        }
        public void Handle(TranPaymentUpdateEvent message)
        {
            if (//this.IsActive && 
                message != null && message.TransactionID > 0)
            {
                //load lai danh sach hoa don
                LoadPatientTransactionPayment_ByTransactionID(message.TransactionID);
            }
        }
        public void LoadPatientTransactionPayment_ByTransactionID(long TransactionID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        try
                        {
                            contract.BeginPatientTransactionPayment_Load(TransactionID, Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var data = contract.EndPatientTransactionPayment_Load(asyncResult);
                                    PatientPayments = new PagedCollectionView.PagedCollectionView(data);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                }
                            }), null);
                        }
                        catch (Exception innerEx)
                        {
                            ClientLoggerHelper.LogInfo(innerEx.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
    }
}