using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;

/*
 * 20220531 #001 DatTB: Thêm biến IsWithOutBill Hủy quyết toán bỏ qua HDDT và phân quyền
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IReviewSettlement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReviewSettlementViewModel : Conductor<object>, IReviewSettlement
    {
        #region Properties
        private ObservableCollection<TransactionFinalization> _patientSettlementList;
        public ObservableCollection<TransactionFinalization> PatientSettlementList
        {
            get { return _patientSettlementList; }
            set
            {
                if (_patientSettlementList != value)
                {
                    _patientSettlementList = value;
                    NotifyOfPropertyChange(() => PatientSettlementList);
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
        #region Events
        [ImportingConstructor]
        public ReviewSettlementViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
        }
        #endregion
        #region Methods
        public void GetPatientSettlement(long PtRegistrationID, long V_RegistrationType)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientSettlement(PtRegistrationID, V_RegistrationType,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndGetPatientSettlement(asyncResult);

                                    if (regItem != null)
                                    {
                                        PatientSettlementList = regItem.ToObservableCollection();
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void PrintSettlementCmd(TransactionFinalization source, object eventArgs)
        {
            //KMx: Dùng chung cho In Quyết Toán Nội Trú và In Hóa Đơn Bán Lẻ Của Khoa Dược (26/12/2014 14:47)
            if (source == null || source.TranFinalizationID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0743_G1_Msg_InfoKhTimThayMaQToan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.ID = source.TranFinalizationID;

            //switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
            //{
            //    case 1:
            //        {
            //            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT;
            //            break;
            //        }
            //    case 2:
            //        {
            //            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V2;
            //            break;
            //        }
            //    case 4:
            //        {
            //            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V4;
            //            proAlloc.ReceiptType = ReceiptType.FINAL_SETTLEMENT;
            //            break;
            //        }
            //    default:
            //        {
            //            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V4;
            //            proAlloc.ReceiptType = ReceiptType.FINAL_SETTLEMENT;
            //            break;
            //        }
            //}

            //proAlloc.flag = 0; //0: In Quyết Toán, 1: In Hóa Đơn Bán Lẻ Của Khoa Dược.

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            GlobalsNAV.ShowDialog<ICommonPreviewView>((ICommonPreviewView aView) => {
                aView.RegistrationID = source.PtRegistrationID;
                aView.eItem = ReportName.RptOutPtTransactionFinalization;
                aView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
                aView.ID = source.TranFinalizationID;
            });
            return;

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = source.TranFinalizationID;

                switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
                {
                    case 1:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT;
                            break;
                        }
                    case 2:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V2;
                            break;
                        }
                    case 4:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V4;
                            proAlloc.ReceiptType = ReceiptType.FINAL_SETTLEMENT;
                            break;
                        }
                    default:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V4;
                            proAlloc.ReceiptType = ReceiptType.FINAL_SETTLEMENT;
                            break;
                        }
                }

                proAlloc.flag = 0;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

        }

        //▼==== #001
        public void DeleteSettlementCmd(TransactionFinalization source, object eventArgs, bool IsWithOutBill = false)
        {
            if (source == null || source.TranFinalizationID <= 0 || source.PtRegistrationID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0743_G1_Msg_InfoKhTimThayMaQToan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (MessageBox.Show(string.Format(eHCMSResources.Z2778_G1_CoChacMuonHuyQT, source.FinalizedReceiptNum), eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            var mThread = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteTransactionFinalization(source.FinalizedReceiptNum, Globals.LoggedUserAccount.StaffID.Value, (long)AllLookupValues.RegistrationType.NOI_TRU, null, IsWithOutBill, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string mOutMessage = contract.EndDeleteTransactionFinalization(asyncResult);
                            if (!string.IsNullOrEmpty(mOutMessage))
                            {
                                MessageBox.Show(mOutMessage, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            GetPatientSettlement(source.PtRegistrationID, (long)source.V_RegistrationType);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
        //▲==== #001
        #endregion
    }
}