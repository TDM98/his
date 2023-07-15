using System;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IAllDiagnosisGroupByDoctorStaffIDDeptLocationID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AllDiagnosisGroupByDoctorStaffIDDeptLocationIDViewModel : Conductor<object>, IAllDiagnosisGroupByDoctorStaffIDDeptLocationID
    {
        [ImportingConstructor]
        public AllDiagnosisGroupByDoctorStaffIDDeptLocationIDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            // TxD 02/08/2014: Use Globals Server Date instead
            //GetCurrentDate();
            DateTime todayDate = Globals.GetCurServerDateTime();
            FromDate = todayDate;
            ToDate = todayDate;

            authorization();
        }

        // TxD 02/08/2014 The following method is nolonger required
        //public void GetCurrentDate()
        //{
        //    var t = new Thread(() =>
        //    {
        //        IsLoading = true;

        //        try
        //        {
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
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
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

        public void btView()
        {
            if (CheckDateValid())
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.DoctorStaffID = 0;
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.eItem = ReportName.AllDiagnosisTreatmentGroupByDoctorStaffID;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
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
                    MessageBox.Show(eHCMSResources.A0884_G1_Msg_InfoNhapTuNgDenNg, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
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

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mThongKe_DSBacSiKham_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mThongKe_DSBacSiKham,
                                               (int)oConsultationEx.mThongKe_DSBacSiKham_Xem, (int)ePermission.mView);
        }

        #region checking account

        private bool _mThongKe_DSBacSiKham_Xem = true;

        public bool mThongKe_DSBacSiKham_Xem
        {
            get
            {
                return _mThongKe_DSBacSiKham_Xem;
            }
            set
            {
                if (_mThongKe_DSBacSiKham_Xem == value)
                    return;
                _mThongKe_DSBacSiKham_Xem = value;
                NotifyOfPropertyChange(() => mThongKe_DSBacSiKham_Xem);
            }
        }

        #endregion
    }
}
