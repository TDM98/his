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
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using System.Collections.Generic;
using System.Linq;
using aEMR.Common.ConfigurationManager.Printer;

namespace aEMR.Common.ViewModels
{
    /// <summary>
    /// Quản lý danh sách CLS của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    [Export(typeof(IPatientPayment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPaymentViewModel : Conductor<object>, IPatientPayment
        , IHandle<TranPaymentUpdateEvent>
    {
        public PatientPaymentViewModel()
        {
            _showPrintColumn = false;
            Globals.EventAggregator.Subscribe(this);
        }

        private IPatientPaymentView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IPatientPaymentView;
            if (_currentView != null)
            {
                _currentView.TogglePrintColumnVisibility(_showPrintColumn);
            }
        }

        private ObservableCollection<PatientTransactionPayment> _patientPayments;
        public ObservableCollection<PatientTransactionPayment> PatientPayments
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


        private bool _showPrintColumn;
        public bool ShowPrintColumn
        {
            get { return _showPrintColumn; }
            set
            {
                _showPrintColumn = value;
                if (_currentView != null)
                {
                    _currentView.TogglePrintColumnVisibility(_showPrintColumn);
                }
            }
        }

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

        public void PrintReceiptCmd(object datacontext, object eventArgs)
        {
            if (!CanPrintReceiptCmd)
            {
                MessageBox.Show(eHCMSResources.A0673_G1_Msg_InfoKhDcIn);
                return;
            }
            var payment = datacontext as PatientTransactionPayment;
            if (payment == null)
            {
                MessageBox.Show(eHCMSResources.A0689_G1_Msg_InfoKhTheIn);
                return;
            }

            //ShowReceiptReport(payment);
            ShowReceiptReport_New(payment);

            //var printingMode = Globals.ServerConfigSection.CommonItems.ReceiptPrintingMode;
            //switch (printingMode)
            //{
            //    case 1:
            //        ShowReceiptReport(payment);
            //        break;
            //    case 2:
            //        PrintReceipt(payment);
            //        break;
            //    case 3:
            //        {
            //            ShowReceiptReport(payment);
            //            PrintReceipt(payment);
            //        }
            //        break;
            //}
        }
        //public void ShowReceiptReport(PatientTransactionPayment payment)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<Root>");
        //    sb.Append("<IDList>");
        //    sb.AppendFormat("<ID>{0}</ID>", payment.PtTranPaymtID);
        //    sb.Append("</IDList>");
        //    sb.Append("</Root>");
        //    var reportVm = Globals.GetViewModel<ICommonPreviewView>();
        //    reportVm.Result = sb.ToString();

        //    switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
        //    {
        //        case 1:
        //            {
        //                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML;
        //                break;
        //            }
        //        case 2:
        //            {
        //                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V2;
        //                break;
        //            }
        //        case 4:
        //            {
        //                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4;
        //                break;
        //            }
        //        default:
        //            {
        //                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4;
        //                break;
        //            }
        //    }
        //    Globals.ShowDialog(reportVm as Conductor<object>);
        //}

        public void ShowReceiptReport_New(PatientTransactionPayment payment)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Root>");
            sb.Append("<IDList>");
            sb.AppendFormat("<ID>{0}</ID>", payment.PtTranPaymtID);
            sb.AppendFormat("<OutPtCashAdvanceID>{0}</OutPtCashAdvanceID>", payment.OutPtCashAdvanceID);
            sb.Append("</IDList>");
            sb.Append("</Root>");
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            Action<ICommonPreviewView> onInitDlg = (reportVm) =>
            {
                reportVm.Result = sb.ToString();
                switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
                {
                    case 1:
                        {
                            reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML;
                            break;
                        }
                    case 2:
                        {
                            reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V2;
                            break;
                        }
                    case 4:
                        {
                            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != "")
                            {
                                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4_THERMAL;
                            }
                            else
                            {
                                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4;
                            }
                            break;
                        }
                    default:
                        {
                            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != "")
                            {
                                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4_THERMAL;
                            }
                            else
                            {
                                reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_RECEIPT_XML_V4;
                            }
                            break;
                        }
                }
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
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
                            contract.BeginGetOutPatientReceiptReportXMLInPdfFormat(sb.ToString(),
                                            Globals.DispatchCallback(asyncResult =>
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
            //var typeInfo = Globals.GetViewModel<IPaymentUpdateNote>();
            //typeInfo.CurPaymentInfo = ObjectCopier.DeepCopy(sender as PatientTransactionPayment);

            //var instance = typeInfo as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{

            //    //lam gi do
            //});

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
                            contract.BeginPatientTransactionPayment_Load(TransactionID,
                                            Globals.DispatchCallback(asyncResult =>
                                            {
                                                try
                                                {
                                                    var data = contract.EndPatientTransactionPayment_Load(asyncResult);
                                                    PatientPayments = data.ToObservableCollection();
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
        public void InitViewForPayments(PatientRegistration aRegistration)
        {
            var mPatientPayments = new List<PatientTransactionPayment>();
            if (aRegistration != null && aRegistration.PatientTransaction != null && aRegistration.PatientTransaction.PatientTransactionPayments != null)
            {
                mPatientPayments.AddRange(aRegistration.PatientTransaction.PatientTransactionPayments);
            }
            if (aRegistration != null && aRegistration.PatientTransaction != null && aRegistration.PatientTransaction.PatientCashAdvances != null
                && Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance)
            {
                mPatientPayments = aRegistration.PatientTransaction.PatientCashAdvances.Select(x =>
                    new PatientTransactionPayment { PaymentDate = x.RecCreatedDate, PayAmount = x.PaymentAmount
                        , Currency = new Lookup { LookupID = (long)AllLookupValues.Currency.VND, ObjectValue = AllLookupValues.Currency.VND.GetDescription() }
                        //, PaymentMode = new Lookup{  LookupID = (long)AllLookupValues.PaymentMode.CHUYEN_KHOAN, ObjectValue = AllLookupValues.PaymentMode.CHUYEN_KHOAN.GetDescription() }
                        , PaymentMode = Globals.AllLookupValueList.Where(t => t.ObjectTypeID == (long)LookupValues.PAYMENT_MODE && t.LookupID == x.V_PaymentMode).FirstOrDefault()
                        , PaymentType = new Lookup { LookupID = (long)AllLookupValues.PaymentType.TAM_UNG, ObjectValue = AllLookupValues.PaymentType.TAM_UNG.GetDescription() }
                        , OutPtCashAdvanceID = x.OutPtCashAdvanceID
                        , ReceiptNumber = x.CashAdvReceiptNum
                        , IsDeleted = x.IsDeleted
                        , PtTranPaymtID = x.PtTranPaymtID
                    }).ToList();
            }
            this.PatientPayments = mPatientPayments.ToObservableCollection();
        }
    }
}