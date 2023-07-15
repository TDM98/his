using System.ComponentModel.Composition;
using aEMR.CommonTasks;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Infrastructure;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;


namespace aEMR.Common.ViewModels
{
    [Export(typeof(IePrescriptionCalcNotSave)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ePrescriptionCalcNotSaveViewModel : ViewModelBase, IePrescriptionCalcNotSave
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public override bool IsProcessing
        {
            get { return _IsWaitingCalc; }
        }

        public override string StatusText
        {
            get
            {
                if (_IsWaitingCalc)
                {
                    return eHCMSResources.Z0118_G1_DangTinhTien;
                }
                return string.Empty;
            }
        }


        private bool _IsWaitingCalc;
        public bool IsWaitingCalc
        {
            get { return _IsWaitingCalc; }
            set
            {
                if (_IsWaitingCalc != value)
                {
                    _IsWaitingCalc = value;
                    NotifyOfPropertyChange(() => IsWaitingCalc);
                    NotifyWhenBusy();
                }
            }
        }


        private enum DataGridCol
        {
            //ColDelete = 0,
            BH = 0,
            MaThuoc = 1,
            TenThuoc = 2,
            HamLuong = 3,
            DVT = 4,
            CachDung = 5,
            SLYC = 6,
            ThucXuat = 7,
            LoSX = 8,
            DonGiaBan = 9,
            DonGiaBH = 10
        }

        [ImportingConstructor]
        public ePrescriptionCalcNotSaveViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            SelectedOutInvoice = new OutwardDrugInvoice();
            SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();
        }

        public double HIBenefit { get; set; }
        public Prescription ObjPrescription { get; set; }
        public long StoreID { get; set; }
        public Int16 RegistrationType { get; set; }

        private OutwardDrugInvoice _SelectedOutInvoice;
        public OutwardDrugInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
            }
        }


        private Patient _patientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _patientInfo;
            }
            set
            {
                if (_patientInfo != value)
                {
                    _patientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }
        //▼===== 20200722 TTM:  Bổ sung code tính tiền tổng thuốc (tất cả toa bệnh nhân nếu có).
        //                      Loại bỏ code tìm kiếm đăng ký vì tất cả thông tin đã nằm trong Registration_DataStorage.
        //private IEnumerator<IResult> DoGetInfoPatientPrescription(bool IsAllOrOne = false)
        //{
        //    var paymentTypeTask = new LoadPatientInfoByRegistrationTask(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID
        //        , Registration_DataStorage.CurrentPatient.PatientID
        //        , Registration_DataStorage.CurrentPatientRegistration.FindPatient
        //    );
        //    yield return paymentTypeTask;
        //    PatientInfo = paymentTypeTask.CurrentPatient;
        //    HIBenefit = PatientInfo.LatestRegistration.PtInsuranceBenefit.GetValueOrDefault();
        //    if (!IsAllOrOne)
        //    {
        //        spGetInBatchNumberAndPrice_ByPresciption_NotSave();
        //    }
        //    else
        //    {
        //        GetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID();
        //    }
        //}
        public void DoGetInfoPatientPrescription(bool IsAllOrOne = false)
        {
            PatientInfo = Registration_DataStorage.CurrentPatient;
            HIBenefit = Registration_DataStorage.CurrentPatientRegistration.PtInsuranceBenefit.GetValueOrDefault();
            if (!IsAllOrOne)
            {
                spGetInBatchNumberAndPrice_ByPresciption_NotSave();
            }
            else
            {
                GetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID();
            }
        }

        public void Calc()
        {
            //Coroutine.BeginExecute(DoGetInfoPatientPrescription());
            DoGetInfoPatientPrescription();
        }
        public void CalcAll()
        {
            //Coroutine.BeginExecute(DoGetInfoPatientPrescription(true));
            DoGetInfoPatientPrescription(true);
        }
        //▲=====
        public void spGetInBatchNumberAndPrice_ByPresciption_NotSave()
        {
            IsWaitingCalc = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginspGetInBatchNumberAndPrice_ByPresciption_NotSave(ObjPrescription, StoreID, RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndspGetInBatchNumberAndPrice_ByPresciption_NotSave(asyncResult);
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                SumTotalPrice();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsWaitingCalc = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void GetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID()
        {
            IsWaitingCalc = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID((long)ObjPrescription.PtRegistrationID, RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID(asyncResult);
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                SumTotalPrice();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsWaitingCalc = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private decimal _TotalInvoicePrice;
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                if (_TotalInvoicePrice != value)
                {
                    _TotalInvoicePrice = value;
                    NotifyOfPropertyChange(() => TotalInvoicePrice);
                }
            }
        }

        private decimal _TotalHIPayment;
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                if (_TotalHIPayment != value)
                {
                    _TotalHIPayment = value;
                    NotifyOfPropertyChange(() => TotalHIPayment);
                }
            }
        }
        private decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                if (_TotalPatientPayment != value)
                {
                    _TotalPatientPayment = value;
                    NotifyOfPropertyChange(() => TotalPatientPayment);
                }
            }
        }

        private void SumTotalPrice()
        {
            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null)
            {
                if (SelectedOutInvoice.outiID == 0)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit;
                        SelectedOutInvoice.OutwardDrugs[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugs[i].TotalPrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;

                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit;
                    }
                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                else
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;
                    }
                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
            }

        }

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;

        }

        private bool CheckQuantity(object outward)
        {
            try
            {
                OutwardDrug p = outward as OutwardDrug;
                if (p.QtyOffer == p.OutQuantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            if (!CheckQuantity(e.Row.DataContext))
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaidPrescript)
            {
                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {

                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = false;
                }
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
    }
}