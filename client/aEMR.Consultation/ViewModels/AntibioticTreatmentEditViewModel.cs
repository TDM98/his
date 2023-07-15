using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
/*
* 20220105 #001 TNHX: Lọc danh sách thuốc/ y cụ theo danh mục COVID
*/
namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IAntibioticTreatmentEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AntibioticTreatmentEditViewModel : ViewModelBase, IAntibioticTreatmentEdit
    {
        #region Properties
        private AntibioticTreatment _CurrentAntibioticTreatment;
        public AntibioticTreatment CurrentAntibioticTreatment
        {
            get
            {
                return _CurrentAntibioticTreatment;
            }
            set
            {
                if (_CurrentAntibioticTreatment == value)
                {
                    return;
                }
                _CurrentAntibioticTreatment = value;
                NotifyOfPropertyChange(() => CurrentAntibioticTreatment);
                NotifyOfPropertyChange(() => EditButtonContent);
                NotifyOfPropertyChange(() => IsEnableEditDetails);
            }
        }
        public long DeptID { get; set; }
        private long StoreID { get; set; }
        public string EditButtonContent
        {
            get
            {
                return CurrentAntibioticTreatment != null && CurrentAntibioticTreatment.AntibioticTreatmentID > 0 ? eHCMSResources.K1599_G1_CNhat : eHCMSResources.T2937_G1_Luu;
            }
        }
        public bool IsUpdatedCompleted { get; set; } = false;
        public long PtRegistrationID { get; set; }
        public bool IsEnableEditDetails
        {
            get
            {
                return CurrentAntibioticTreatment != null && CurrentAntibioticTreatment.InfectionCaseID > 0 && CurrentAntibioticTreatment.V_AntibioticTreatmentType == (long)AllLookupValues.V_AntibioticTreatmentType.InfectionCase;
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public AntibioticTreatmentEditViewModel()
        {
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Coroutine.BeginExecute(DoGetStore_ClinicDeptAll());
            if (CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection == null)
            {
                CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection = new System.Collections.ObjectModel.ObservableCollection<AntibioticTreatmentMedProductDetail>();
            }
            CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Add(new AntibioticTreatmentMedProductDetail());
            if (CurrentAntibioticTreatment.AntibioticTreatmentID > 0)
            {
                GetAntibioticTreatmentMedProductDetails();
            }
        }
        private AntibioticTreatmentMedProductDetail _CurrentAntibioticTreatmentMedProductDetail;
        public AntibioticTreatmentMedProductDetail CurrentAntibioticTreatmentMedProductDetail
        {
            get
            {
                return _CurrentAntibioticTreatmentMedProductDetail;
            }
            set
            {
                if (_CurrentAntibioticTreatmentMedProductDetail == value)
                {
                    return;
                }
                _CurrentAntibioticTreatmentMedProductDetail = value;
                NotifyOfPropertyChange(() => CurrentAntibioticTreatmentMedProductDetail);
            }
        }
        public void DrugCollection_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                return;
            }
            CurrentAntibioticTreatmentMedProductDetail.RefGenMedProductDetail = (sender as AxAutoComplete).SelectedItem as RefGenMedProductDetails;
            CurrentAntibioticTreatmentMedProductDetail.Quantity = 0;
            if (!CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Any(x => x.RefGenMedProductDetail == null || x.RefGenMedProductDetail.GenMedProductID == 0))
            {
                CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Add(new AntibioticTreatmentMedProductDetail());
            }
        }
        public void DrugCollection_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            SearchRefGenMedProductDetails(sender, e.Parameter);
        }
        public void btnSave()
        {
            if ((CurrentAntibioticTreatment.InfectionCaseID > 0 && CurrentAntibioticTreatment.V_AntibioticTreatmentType == (long)AllLookupValues.V_AntibioticTreatmentType.InfectionCase) && !CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Any(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.GenMedProductID > 0))
            {
                Globals.ShowMessage(eHCMSResources.Z2862_G2_ThieuChiTietKhangSinh, eHCMSResources.T0432_G1_Error);
                return;
            }
            if (CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Any(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.GenMedProductID > 0) &&
                CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Where(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.GenMedProductID > 0).GroupBy(x => x.RefGenMedProductDetail.GenMedProductID).Any(x => x.ToList().Count() > 1))
            {
                Globals.ShowMessage(eHCMSResources.Z2877_G1_KhangSinhBiTrungLap, eHCMSResources.T0432_G1_Error);
                return;
            }
            IsUpdatedCompleted = true;
            TryClose();
        }
        public void btnGetOutQuantity()
        {
            foreach (var aItem in CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection)
            {
                aItem.Quantity = 0;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugsInUseOfAntibioticTreatment(CurrentAntibioticTreatment, DeptID, PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetDrugsInUseOfAntibioticTreatment(asyncResult);
                                if (GettedCollection != null && GettedCollection.Count > 0)
                                {
                                    foreach (var aOutItem in GettedCollection)
                                    {
                                        if (CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Any(x => x.RefGenMedProductDetail.GenMedProductID == aOutItem.GenMedProductID))
                                        {
                                            CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.First(x => x.RefGenMedProductDetail.GenMedProductID == aOutItem.GenMedProductID).Quantity = aOutItem.OutQuantity;
                                        }
                                    }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public void DeleteMedProductDetail_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as Button).DataContext as AntibioticTreatmentMedProductDetail).RefGenMedProductDetail == null ||
                ((sender as Button).DataContext as AntibioticTreatmentMedProductDetail).RefGenMedProductDetail.GenMedProductID == 0)
            {
                return;
            }
            CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Remove((sender as Button).DataContext as AntibioticTreatmentMedProductDetail);
        }
        public void gvAntibioticTreatmentCollection_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (!IsEnableEditDetails)
            {
                e.Cancel = true;
                return;
            }
        }
        #endregion
        #region Methods
        private IEnumerator<IResult> DoGetStore_ClinicDeptAll()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            var allRefStorageWarehouseLocation = paymentTypeTask.LookupList;
            if (allRefStorageWarehouseLocation.Any(x => x.IsSubStorage && x.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.THUOC).ToString())))
            {
                StoreID = allRefStorageWarehouseLocation.First(x => x.IsSubStorage && x.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.THUOC).ToString())).StoreID;
            }
            else if (allRefStorageWarehouseLocation.Any(x => x.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.THUOC).ToString())))
            {
                StoreID = allRefStorageWarehouseLocation.First(x => x.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.THUOC).ToString())).StoreID;
            }
            yield break;
        }
        private void SearchRefGenMedProductDetails(object sender, string Name)
        {
            var CurrentThread = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(false, false, Name, StoreID
                        , (long)AllLookupValues.MedProductType.THUOC, null, null, false
                        , null, null, null, true
                        //▼====: #001
                        , false
                        //▲====: #001
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var GettedCollection = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);
                            var RefGenMedProductDetailsList = new List<RefGenMedProductDetails>();
                            if (GettedCollection != null && GettedCollection.Count > 0)
                            {
                                foreach (var aItem in GettedCollection)
                                {
                                    if (RefGenMedProductDetailsList.Any(x => x.GenMedProductID == aItem.GenMedProductID))
                                    {
                                        continue;
                                    }
                                    RefGenMedProductDetailsList.Add(aItem);
                                }
                            }
                            (sender as AxAutoComplete).ItemsSource = RefGenMedProductDetailsList;
                            (sender as AxAutoComplete).PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private void GetAntibioticTreatmentMedProductDetails()
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAntibioticTreatmentMedProductDetails(CurrentAntibioticTreatment.AntibioticTreatmentID, CurrentAntibioticTreatment.V_AntibioticTreatmentType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetAntibioticTreatmentMedProductDetails(asyncResult);
                                if (GettedCollection != null)
                                {
                                    CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection = GettedCollection.ToObservableCollection();
                                }
                                else
                                {
                                    CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection = new System.Collections.ObjectModel.ObservableCollection<AntibioticTreatmentMedProductDetail>();
                                }
                                CurrentAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Add(new AntibioticTreatmentMedProductDetail());
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        #endregion
    }
}