using System;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using eHCMSLanguage;
using Caliburn.Micro;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IDiagnosisTreatmentByDoctorStaffID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiagnosisTreatmentByDoctorStaffIDViewModel : Conductor<object>, IDiagnosisTreatmentByDoctorStaffID
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        [ImportingConstructor]
        public DiagnosisTreatmentByDoctorStaffIDViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            // TxD 02/08/2014: Use Globals Server Date instead
            // GetCurrentDate();
            DateTime date = Globals.GetCurServerDateTime();
            FromDate = date;
            ToDate = date;

            authorization();
        }

        // TxD 02/08/2014: The following method is nolonger required
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
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                //proAlloc.FromDate = FromDate;
                //proAlloc.ToDate = ToDate;
                //proAlloc.eItem = ReportName.DiagnosisTreatmentByDoctorStaffID;

                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.eItem = ReportName.DiagnosisTreatmentByDoctorStaffID;
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

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mThongKe_DSBenhNhanDaKham_Xem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mThongKe_DSBenhNhanDaKham,
                                               (int)oConsultationEx.mThongKe_DSBenhNhanDaKham_Xem, (int)ePermission.mView);


        }

        #region checking account

        private bool _mThongKe_DSBenhNhanDaKham_Xem = true;

        public bool mThongKe_DSBenhNhanDaKham_Xem
        {
            get
            {
                return _mThongKe_DSBenhNhanDaKham_Xem;
            }
            set
            {
                if (_mThongKe_DSBenhNhanDaKham_Xem == value)
                    return;
                _mThongKe_DSBenhNhanDaKham_Xem = value;
                NotifyOfPropertyChange(() => mThongKe_DSBenhNhanDaKham_Xem);
            }
        }



        #endregion
    }
}
