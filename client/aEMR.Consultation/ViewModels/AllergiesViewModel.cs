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
using System.Windows;
using aEMR.ServiceClient.Consultation_PCLs;
using Castle.Windsor;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IAllergies)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AllergiesViewModel : Conductor<object>, IAllergies
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

        [ImportingConstructor]
        public AllergiesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Allergy = new MDAllergy();
            Warning=new MDWarning();
        }

        #region Properties member
        private ObservableCollection<RefGenericDrugDetail> _drugNameList;
        public ObservableCollection<RefGenericDrugDetail> DrugNameList
        {
            get
            {
                return _drugNameList;
            }
            set
            {
                if (_drugNameList != value)
                {
                    _drugNameList = value;
                    NotifyOfPropertyChange(() => DrugNameList);
                }
            }
        }

        private long _PatientID;
        public long PatientID
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
                    NotifyOfPropertyChange(() => PatientID);
                }
            }
        }

        private MDAllergy _allergy;
        public MDAllergy Allergy
        {
            get
            {
                return _allergy;
            }
            set
            {
                if (_allergy != value)
                {
                    _allergy = value;
                    NotifyOfPropertyChange(()=>Allergy);
                }
            }
        }

        private MDWarning _Warning;
        public MDWarning Warning
        {
            get
            {
                return _Warning;
            }
            set
            {
                if (_Warning != value)
                {
                    _Warning = value;
                    NotifyOfPropertyChange(() => Warning);
                }
            }
        }



        //public void SetValueWarning(string str)
        //{
        //    Allergy.WarningItems = str;
        //}

        #endregion
        enum AllergyType
        {
            Drug = 0,
            DrugClass= 1,
            Others =2
        };




        private long _CaseOfAllergyType=(long)AllergyType.Drug;
        public long CaseOfAllergyType
        {
            get { return _CaseOfAllergyType; }
            set
            {
                if (_CaseOfAllergyType != value)
                {
                    _CaseOfAllergyType = value;
                    NotifyOfPropertyChange(()=>CaseOfAllergyType);
                }
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }
   

        private void LoadDrugNames(string Name, byte type)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugNames(Name, 0, Globals.PageSize, type, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchRefDrugNames(asyncResult);
                            if (results != null)
                            {
                                if (DrugNameList == null)
                                {
                                    DrugNameList = new ObservableCollection<RefGenericDrugDetail>();
                                }
                                else
                                {
                                    DrugNameList.Clear();
                                }
                                foreach (RefGenericDrugDetail p in results)
                                {
                                    DrugNameList.Add(p);
                                }
                                NotifyOfPropertyChange(() => DrugNameList);
                            }
                            au.ItemsSource = DrugNameList;
                            au.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }
        AutoCompleteBox au;
        public void aucAllergies_Populating(object sender, PopulatingEventArgs e)
        {
            au = sender as AutoCompleteBox;
            switch (CaseOfAllergyType)
            {
                case (long)AllergyType.Drug:
                    LoadDrugNames(e.Parameter, 0);
                    break;
                case (long)AllergyType.DrugClass:
                    LoadDrugNames(e.Parameter, 1);
                    break;
                default:
                    break;
            }
        }

        public void gAlleryType0_Checked(object sender, RoutedEventArgs e)
        {
            CaseOfAllergyType = (long)AllergyType.Drug;
        }

        public void gAlleryType1_Checked(object sender, RoutedEventArgs e)
        {
            CaseOfAllergyType =(long)AllergyType.DrugClass;
        }

        public void gAlleryType2_Checked(object sender, RoutedEventArgs e)
        {
            CaseOfAllergyType = (long)AllergyType.Others;
        }

        public void btnAdd()
        {
            PreparingSaveAllergies(Allergy);
            TryClose();
        }

        public void btnAddWarning()
        {
            PreparingSaveWarning();
        }
        
        public void aucAllergies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Allergy.AllergiesItems = au.Text;
            }
            catch
            {
                Allergy.AllergiesItems = string.Empty;
            }
        }

        private void PreparingSaveAllergies(object newAllergy)
        {
            MDAllergy objA = (MDAllergy)newAllergy;

            switch (CaseOfAllergyType)
            {
                case 0:
                    {
                        objA.V_AItemType = (long)AllLookupValues.V_AItemType.DRUG;
                        break;
                    }
                case 1:
                    {
                        objA.V_AItemType = (long)AllLookupValues.V_AItemType.DRUG_CLASS;
                        break;
                    }
                case 2:
                    {
                        objA.V_AItemType = (long)AllLookupValues.V_AItemType.LY_DO_KHAC;
                        break;
                    }
            }

            try
            {
                objA.AllergiesItems = au.Text;
            }
            catch
            {
                objA.AllergiesItems = string.Empty;
            }

            if (string.IsNullOrEmpty(objA.AllergiesItems))
            {
                MessageBox.Show(eHCMSResources.A0868_G1_Msg_InfoNhapDiUng);
                return;
            }

            MDAllergies_Save(objA);
            
        }

        private void PreparingSaveWarning()
        {
            //Add warnings
            if (string.IsNullOrEmpty(Warning.WarningItems))
            {
                MessageBox.Show(eHCMSResources.A0866_G1_Msg_InfoNhapCBao);
                return;
            }

            MDWarnings_Save(Warning);
        }


        private void MDAllergies_Save(MDAllergy ObjA)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDAllergies_Save(ObjA, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string Result = "";
                            contract.EndMDAllergies_Save(out Result, asyncResult);
                            if (Result=="1")
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                //phat ra su kien load lai di ung
                                Globals.EventAggregator.Publish(new Re_ReadAllergiesEvent() { });
                            }
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

        private void MDWarnings_Save(MDWarning ObjW)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new SummaryServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginMDWarnings_Save(ObjW, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long Result = -1;
                            contract.EndMDWarnings_Save(out Result, asyncResult);
                            if (Result >0)
                            {
                                ObjW.WItemID = Result;
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                //phat ra su kien load lai canh bao
                                Globals.EventAggregator.Publish(new Re_ReadWarningEvent() { });
                                TryClose();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0801_G1_Msg_InfoLuuCBaoFail);
                            }
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


    }
}
