using eHCMSLanguage;
using aEMR.ServiceClient;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using System.Windows.Controls;
using aEMR.ServiceClient.Consultation_PCLs;
using System.Windows;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IRiskFactors)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RiskFactorsViewModel : Conductor<object>, IRiskFactors
    {

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
        private bool _isEdit;
        public bool isEdit
        {
            get { return _isEdit; }
            set
            {
                if (_isEdit != value)
                {
                    _isEdit = value;
                    NotifyOfPropertyChange(() => isEdit);                    
                }
            }
        }

        
              
        public RiskFactorsViewModel()
        {
            curRiskFactors = new RiskFactors(); 
        }
        #region Properties member


        private long? _PatientID;
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(()=>PatientID);
                }
            }
        }

        private RiskFactors _curRiskFactors;
        public RiskFactors curRiskFactors
        {
            get
            {
                return _curRiskFactors;
            }
            set
            {
                if (_curRiskFactors != value)
                {
                    _curRiskFactors = value;
                    NotifyOfPropertyChange(() => curRiskFactors);
                }
            }
        }

      
       
        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private bool CheckValidPhysiscalExam(object temp)
        {
            RiskFactors u = temp as RiskFactors;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }

     
        public void OKButton(object sender, RoutedEventArgs e)
        {
            if (CheckValidPhysiscalExam(curRiskFactors))
            {
                curRiskFactors.CommonMedicalRecord = new CommonMedicalRecord();
                curRiskFactors.CommonMedicalRecord.PatientID = PatientID;
                //AddNewPhyExam();                
            }
        }

        //public void CancelButton(object sender, RoutedEventArgs e)
        //{
        //    TryClose();
        //}

        
        public void OKSave(object sender, RoutedEventArgs e)
        {
            //add new here             
            if (PatientID == null || PatientID<1)
            {
                return;
            }
            if (CheckValidPhysiscalExam(curRiskFactors))
            {
                curRiskFactors.CommonMedicalRecord = new CommonMedicalRecord();
                curRiskFactors.CommonMedicalRecord.PatientID = PatientID;
                curRiskFactors.PatientID = PatientID.Value;
                curRiskFactors.StaffID = Globals.LoggedUserAccount.StaffID.Value;

                //AddNewRiskFactor();

                AddNewRiskFactor_New();
            }
        }

        public void CloseCmd() 
        {
            TryClose();
        }
        #region service method
        //private void AddNewRiskFactor()
        //{
        //    var theDisp = Deployment.Current.Dispatcher;

        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        IsLoading = true;

        //        using (var serviceFactory = new ComRecordsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginRiskFactorInsert(curRiskFactors, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    var results = contract.EndRiskFactorInsert(asyncResult);
        //                    //Globals.ShowMessage(eHCMSResources.A1027_G1_Msg_InfoThemOK,"");
        //                    //phat ra su kien de load lai Physical sau cung
        //                    Globals.EventAggregator.Publish(new RiskFactorSaveCompleteEvent { });
        //                    theDisp.BeginInvoke(() => { TryClose(); });
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;
        //                    IsLoading = false;
        //                }

        //            }), null);

        //        }

        //    });
        //    t.Start();
        //}

        private void AddNewRiskFactor_New()
        {
            
        //var theDisp = System.Windows.Application.Current.Dispatcher;

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRiskFactorInsert(curRiskFactors, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndRiskFactorInsert(asyncResult);
                            //phat ra su kien de load lai Physical sau cung
                            Globals.EventAggregator.Publish(new RiskFactorSaveCompleteEvent { });
                            //theDisp.BeginInvoke(() =>
                            System.Windows.Threading.Dispatcher theDisp = System.Windows.Application.Current.Dispatcher;
                            theDisp.Invoke(() => { TryClose(); });
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        #endregion
    }
}
