using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
/*
 *  20200724 #001 TTM:  BM 0022848: Bổ sung phác đồ cho dịch vụ thủ thuật
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ITreatmentRegimenEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TreatmentRegimenEditViewModel : Conductor<object>, ITreatmentRegimenEdit
        , IHandle<DoubleClick>
        , IHandle<DoubleClickAddReqLAB>
    {
        [ImportingConstructor]
        public TreatmentRegimenEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            V_TreatmentPeriodicCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_TreatmentPeriodic).ToObservableCollection();
            Icd10ListingView = Globals.GetViewModel<Iicd10Listing>();

            var selectPclVm = Globals.GetViewModel<IInPatientSelectPcl>();
            SelectPCLContent = selectPclVm;
            SelectPCLContent.ShowLocationSelection = false;
            ActivateItem(selectPclVm);

            var selectPclVmLAB = Globals.GetViewModel<IInPatientSelectPclLAB>();
            SelectPCLContentLAB = selectPclVmLAB;
            SelectPCLContentLAB.ShowLocationSelection = false;
            ActivateItem(selectPclVmLAB);

            //▼===== #001
            var selectServiceVm = Globals.GetViewModel<IInPatientSelectPcl>();
            SelectPCLContent = selectPclVm;
            SelectPCLContent.ShowLocationSelection = false;
            ActivateItem(selectPclVm);

            GetServiceTypes();
            //▲===== #001
        }

        #region Properties
        private RefTreatmentRegimen _gRefTreatmentRegimen;
        private RefTreatmentRegimen _gOrgRefTreatmentRegimen;
        private ObservableCollection<Lookup> _V_TreatmentPeriodicCollection;
        private long _SelectedTreatmentPeriodic = (long)AllLookupValues.V_TreatmentPeriodic.TreatmentPeriodic1;
        private CollectionViewSource CV_RefTreatmentRegimenDrugDetailsSource;
        private CollectionViewSource CV_RefTreatmentRegimenPCLDetailsSource;
        private CollectionViewSource CV_RefTreatmentRegimenServiceDetailsSource;
        private CollectionViewSource CV_GenericClassesSource;
        private Iicd10Listing _Icd10ListingView;
        private ObservableCollection<DrugClass> _GenericClasses;
        private bool _DefaultIsExpanded = false;
        public CollectionView CV_RefTreatmentRegimenDrugDetails { get; set; }

        public CollectionView CV_RefTreatmentRegimenPCLDetails { get; set; }

        public CollectionView CV_RefTreatmentRegimenServiceDetails { get; set; }

        public CollectionView CV_GenericClasses { get; set; }

        public RefTreatmentRegimen gRefTreatmentRegimen
        {
            get => _gRefTreatmentRegimen; set
            {
                _gRefTreatmentRegimen = value;
                NotifyOfPropertyChange(() => gRefTreatmentRegimen);
                CV_RefTreatmentRegimenDrugDetailsSource = new CollectionViewSource { Source = gRefTreatmentRegimen.RefTreatmentRegimenDrugDetails };
                CV_RefTreatmentRegimenDrugDetails = (CollectionView)CV_RefTreatmentRegimenDrugDetailsSource.View;
                CV_RefTreatmentRegimenDrugDetails.Filter = mItem =>
                {
                    RefTreatmentRegimenDrugDetail SItem = mItem as RefTreatmentRegimenDrugDetail;
                    return SItem != null && !SItem.IsDeleted && SItem.V_TreatmentPeriodic == SelectedTreatmentPeriodic;
                };
                NotifyOfPropertyChange(() => CV_RefTreatmentRegimenDrugDetails);

                CV_RefTreatmentRegimenPCLDetailsSource = new CollectionViewSource { Source = gRefTreatmentRegimen.RefTreatmentRegimenPCLDetails };
                CV_RefTreatmentRegimenPCLDetails = (CollectionView)CV_RefTreatmentRegimenPCLDetailsSource.View;
                CV_RefTreatmentRegimenPCLDetails.Filter = mItem =>
                {
                    RefTreatmentRegimenPCLDetail SItem = mItem as RefTreatmentRegimenPCLDetail;
                    return SItem != null && !SItem.IsDeleted && SItem.V_TreatmentPeriodic == SelectedTreatmentPeriodic;
                };
                NotifyOfPropertyChange(() => CV_RefTreatmentRegimenPCLDetails);

                //▼===== #001
                CV_RefTreatmentRegimenServiceDetailsSource = new CollectionViewSource { Source = gRefTreatmentRegimen.RefTreatmentRegimenServiceDetails };
                CV_RefTreatmentRegimenServiceDetails = (CollectionView)CV_RefTreatmentRegimenServiceDetailsSource.View;
                CV_RefTreatmentRegimenServiceDetails.Filter = mItem =>
                {
                    RefTreatmentRegimenServiceDetail SItem = mItem as RefTreatmentRegimenServiceDetail;
                    return SItem != null && !SItem.IsDeleted && SItem.V_TreatmentPeriodic == SelectedTreatmentPeriodic;
                };
                NotifyOfPropertyChange(() => CV_RefTreatmentRegimenServiceDetails);
                //▲===== #001
            }
        }

        public ObservableCollection<Lookup> V_TreatmentPeriodicCollection
        {
            get => _V_TreatmentPeriodicCollection; set
            {
                _V_TreatmentPeriodicCollection = value;
                NotifyOfPropertyChange(() => V_TreatmentPeriodicCollection);
            }
        }

        public long SelectedTreatmentPeriodic
        {
            get => _SelectedTreatmentPeriodic; set
            {
                _SelectedTreatmentPeriodic = value;
                NotifyOfPropertyChange(() => SelectedTreatmentPeriodic);
            }
        }

        public Iicd10Listing Icd10ListingView
        {
            get { return _Icd10ListingView; }
            set
            {
                _Icd10ListingView = value;
                NotifyOfPropertyChange(() => Icd10ListingView);
            }
        }

        public ObservableCollection<DrugClass> GenericClasses
        {
            get => _GenericClasses; set
            {
                _GenericClasses = value;
                NotifyOfPropertyChange(() => GenericClasses);
            }
        }

        public RefTreatmentRegimen gOrgRefTreatmentRegimen
        {
            get => _gOrgRefTreatmentRegimen; set
            {
                _gOrgRefTreatmentRegimen = value;
                NotifyOfPropertyChange(() => gOrgRefTreatmentRegimen);
            }
        }

        public bool DefaultIsExpanded
        {
            get => _DefaultIsExpanded; set
            {
                _DefaultIsExpanded = value;
                NotifyOfPropertyChange(() => DefaultIsExpanded);
            }
        }

        private IInPatientSelectPcl _selectPCLContent;
        public IInPatientSelectPcl SelectPCLContent
        {
            get { return _selectPCLContent; }
            set
            {
                _selectPCLContent = value;
                NotifyOfPropertyChange(() => SelectPCLContent);
            }
        }

        private IInPatientSelectPclLAB _selectPCLContentLAB;
        public IInPatientSelectPclLAB SelectPCLContentLAB
        {
            get { return _selectPCLContentLAB; }
            set
            {
                _selectPCLContentLAB = value;
                NotifyOfPropertyChange(() => SelectPCLContentLAB);
            }
        }

        //▼===== #001
        private ObservableCollection<RefMedicalServiceType> _serviceTypes;
        public ObservableCollection<RefMedicalServiceType> ServiceTypes
        {
            get { return _serviceTypes; }
            set
            {
                _serviceTypes = value;
                NotifyOfPropertyChange(() => ServiceTypes);
            }
        }
        private RefMedicalServiceType _medServiceType;
        public RefMedicalServiceType MedServiceType
        {
            get
            {
                return _medServiceType;
            }
            set
            {
                if (_medServiceType != value)
                {
                    _medServiceType = value;
                    NotifyOfPropertyChange(() => MedServiceType);
                    MedicalServiceItems = null;
                    GetAllMedicalServiceItemsByType(_medServiceType);
                }
            }
        }
        private ObservableCollection<RefMedicalServiceItem> _medicalServiceItems;
        public ObservableCollection<RefMedicalServiceItem> MedicalServiceItems
        {
            get { return _medicalServiceItems; }
            set
            {
                _medicalServiceItems = value;
                NotifyOfPropertyChange(() => MedicalServiceItems);
            }
        }
        private RefMedicalServiceItem _medServiceItem;
        public RefMedicalServiceItem MedServiceItem
        {
            get
            {
                return _medServiceItem;
            }
            set
            {
                if (_medServiceItem != value)
                {
                    _medServiceItem = value;
                    NotifyOfPropertyChange(() => MedServiceItem);
                }
            }
        }
        //▲===== #001

        #endregion

        #region Events
        public void TreatmentPeriodic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CV_RefTreatmentRegimenDrugDetails == null)
            {
                return;
            }
            CV_RefTreatmentRegimenDrugDetails.Refresh();
            CV_RefTreatmentRegimenPCLDetails.Refresh();
            CV_RefTreatmentRegimenServiceDetails.Refresh();
        }

        public void btnSave()
        {
            gRefTreatmentRegimen.ICD10Code = Icd10ListingView.SelectedItem != null ? Icd10ListingView.SelectedItem.ICD10Code : null;
            gRefTreatmentRegimen.LastUpdatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0;
            if (!CheckBeforeSave()) return;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditRefTreatmentRegimen(gRefTreatmentRegimen, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RefTreatmentRegimen OutRefTreatmentRegimen;
                                if (contract.EndEditRefTreatmentRegimen(out OutRefTreatmentRegimen, asyncResult))
                                {
                                    gRefTreatmentRegimen = OutRefTreatmentRegimen;
                                    CV_RefTreatmentRegimenDrugDetails.Refresh();
                                    CV_RefTreatmentRegimenPCLDetails.Refresh();
                                    CV_RefTreatmentRegimenServiceDetails.Refresh();
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                gRefTreatmentRegimen.ICD10Code = gOrgRefTreatmentRegimen.ICD10Code;
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
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            if (gRefTreatmentRegimen != null)
            {
                Icd10ListingView.SetText(gRefTreatmentRegimen.ICD10Code);
                gOrgRefTreatmentRegimen = gRefTreatmentRegimen.DeepCopy();
            }
            if (GenericClasses != null)
            {
                CV_GenericClassesSource = new CollectionViewSource { Source = GenericClasses };
                CV_GenericClasses = (CollectionView)CV_GenericClassesSource.View;
                CV_GenericClasses.GroupDescriptions.Add(new PropertyGroupDescription("ParentText"));
                NotifyOfPropertyChange(() => CV_GenericClasses);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        public void gvGenericClasses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DrugClass mDrugClass = (e.OriginalSource as FrameworkElement).DataContext as DrugClass;
            if (mDrugClass == null) return;
            if (gRefTreatmentRegimen.RefTreatmentRegimenDrugDetails.Any(x => x.V_TreatmentPeriodic == SelectedTreatmentPeriodic && x.GenericID == mDrugClass.DrugClassID && !x.IsDeleted))
            {
                MessageBox.Show(eHCMSResources.G0859_G1_ThuocDaDung, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            gRefTreatmentRegimen.RefTreatmentRegimenDrugDetails.Add(new RefTreatmentRegimenDrugDetail { TreatmentRegimenID = gRefTreatmentRegimen.TreatmentRegimenID, V_TreatmentPeriodic = SelectedTreatmentPeriodic, GenericID = mDrugClass.DrugClassID, IsDeleted = false, CreatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0, LastUpdatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0, GenericCode = mDrugClass.DrugClassCode, GenericName = mDrugClass.FaName, TreatmentPeriodic = V_TreatmentPeriodicCollection.FirstOrDefault(x => x.LookupID == SelectedTreatmentPeriodic).ObjectValue });
            CV_RefTreatmentRegimenDrugDetails.Refresh();
        }

        public void clDelete(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as RefTreatmentRegimenDrugDetail).IsDeleted = true;
            CV_RefTreatmentRegimenDrugDetails.Refresh();
        }

        public void clDeletePCL(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as RefTreatmentRegimenPCLDetail).IsDeleted = true;
            CV_RefTreatmentRegimenPCLDetails.Refresh();
        }
        public void clDeleteService(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as RefTreatmentRegimenServiceDetail).IsDeleted = true;
            CV_RefTreatmentRegimenServiceDetails.Refresh();
        }
        public void GenericClasses_Populating(object sender, PopulatingEventArgs e)
        {
            AxAutoComplete cboContext = sender as AxAutoComplete;
            e.Cancel = true;
            if (string.IsNullOrEmpty(cboContext.SearchText))
            {
                DefaultIsExpanded = false;
                CV_GenericClasses.Filter = null;
            }
            else
            {
                DefaultIsExpanded = true;
                CV_GenericClasses.Filter = mItem =>
                {
                    DrugClass SItem = mItem as DrugClass;
                    return SItem != null && ((!string.IsNullOrEmpty(SItem.DrugClassCode) && SItem.DrugClassCode.ToUpper().Contains(cboContext.SearchText.ToUpper())) || (!string.IsNullOrEmpty(SItem.FaName) && Globals.RemoveVietnameseString(SItem.FaName).ToUpper().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToUpper())));
                };
            }
            CV_GenericClasses.Refresh();
        }

        public void AddPclExamTypeCmd()
        {
            if (gRefTreatmentRegimen.RefTreatmentRegimenPCLDetails.Any(x => x.V_TreatmentPeriodic == SelectedTreatmentPeriodic && x.PCLExamTypeID == SelectPCLContent.SelectedPCLExamType.PCLExamTypeID && !x.IsDeleted))
            {
                MessageBox.Show(eHCMSResources.A0510_G1_Msg_InfoDaTonTaiDV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (SelectPCLContent == null || SelectPCLContent.SelectedPCLExamType == null)
            {
                return;
            }
            gRefTreatmentRegimen.RefTreatmentRegimenPCLDetails.Add(new RefTreatmentRegimenPCLDetail
            {
                TreatmentRegimenID = gRefTreatmentRegimen.TreatmentRegimenID,
                PCLExamTypeID = SelectPCLContent.SelectedPCLExamType.PCLExamTypeID,
                PCLExamTypeCode = SelectPCLContent.SelectedPCLExamType.PCLExamTypeCode,
                PCLExamTypeName = SelectPCLContent.SelectedPCLExamType.PCLExamTypeName,
                V_PCLMainCategory = SelectPCLContent.SelectedPCLExamType.V_PCLMainCategory,
                V_PCLMainCategoryValue = SelectPCLContent.SelectedPCLExamType.ObjV_PCLMainCategory.ObjectValue,
                V_TreatmentPeriodic = SelectedTreatmentPeriodic,
                IsDeleted = false,
                CreatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0,
                LastUpdatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0,
                TreatmentPeriodic = V_TreatmentPeriodicCollection.FirstOrDefault(x => x.LookupID == SelectedTreatmentPeriodic).ObjectValue
            });
            CV_RefTreatmentRegimenPCLDetails.Refresh();
        }

        public void AddPclExamTypeCmd_LAB()
        {
            if (gRefTreatmentRegimen.RefTreatmentRegimenPCLDetails.Any(x => x.V_TreatmentPeriodic == SelectedTreatmentPeriodic && x.PCLExamTypeID == SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeID && !x.IsDeleted))
            {
                MessageBox.Show(eHCMSResources.A0510_G1_Msg_InfoDaTonTaiDV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (SelectPCLContentLAB == null || SelectPCLContentLAB.SelectedPCLExamType == null)
            {
                return;
            }
            gRefTreatmentRegimen.RefTreatmentRegimenPCLDetails.Add(new RefTreatmentRegimenPCLDetail
            {
                TreatmentRegimenID = gRefTreatmentRegimen.TreatmentRegimenID,
                PCLExamTypeID = SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeID,
                PCLExamTypeCode = SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeCode,
                PCLExamTypeName = SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeName,
                V_PCLMainCategory = SelectPCLContentLAB.SelectedPCLExamType.V_PCLMainCategory,
                V_PCLMainCategoryValue = SelectPCLContentLAB.SelectedPCLExamType.ObjV_PCLMainCategory.ObjectValue,
                V_TreatmentPeriodic = SelectedTreatmentPeriodic,
                IsDeleted = false,
                CreatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0,
                LastUpdatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0,
                TreatmentPeriodic = V_TreatmentPeriodicCollection.FirstOrDefault(x => x.LookupID == SelectedTreatmentPeriodic).ObjectValue
            });
            CV_RefTreatmentRegimenPCLDetails.Refresh();
        }

        public void Handle(DoubleClick message)
        {
            if (message.Source != SelectPCLContent)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContent.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmd();
        }

        public void Handle(DoubleClickAddReqLAB message)
        {
            if (message.Source != SelectPCLContentLAB)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContentLAB.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmd_LAB();
        }

        //▼===== #001
        public void GetAllMedicalServiceItemsByType(RefMedicalServiceType serviceType)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        long? serviceTypeID = null;
                        if (serviceType != null)
                        {
                            serviceTypeID = serviceType.MedicalServiceTypeID;
                        }

                        contract.BeginGetAllMedicalServiceItemsByType(serviceTypeID, null, null,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<RefMedicalServiceItem> allItem = new ObservableCollection<RefMedicalServiceItem>();
                                try
                                {
                                    allItem = contract.EndGetAllMedicalServiceItemsByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                if (allItem == null)
                                {
                                    MedicalServiceItems = null;
                                }
                                else
                                {
                                    var sType = (RefMedicalServiceType)asyncResult.AsyncState;
                                    var col = new ObservableCollection<RefMedicalServiceItem>();
                                    foreach (var item in allItem)
                                    {
                                        item.RefMedicalServiceType = sType;
                                        col.Add(item);
                                    }
                                    MedicalServiceItems = col;
                                }
                            }), serviceType);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                }
            });
            t.Start();
        }
        public void GetServiceTypes()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var outTypes = new List<long>
                                           {
                                               (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NGOAITRU,
                                               (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU,
                                           };
                        contract.BeginGetMedicalServiceTypesByInOutType(outTypes,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<RefMedicalServiceType> allItem = new ObservableCollection<RefMedicalServiceType>();
                                try
                                {
                                    allItem = contract.EndGetMedicalServiceTypesByInOutType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                ObservableCollection<RefMedicalServiceType> TypeList = new ObservableCollection<RefMedicalServiceType>();
                                foreach (var item in allItem)
                                {
                                    if (item.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT)
                                    {
                                        TypeList.Add(item);
                                    }
                                }
                                ServiceTypes = new ObservableCollection<RefMedicalServiceType>(TypeList);

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                }
            });

            t.Start();
        }

        public void AddServiceCmd()
        {
            if (gRefTreatmentRegimen.RefTreatmentRegimenServiceDetails.Any(x => x.V_TreatmentPeriodic == SelectedTreatmentPeriodic && x.MedServiceID == MedServiceItem.MedServiceID && !x.IsDeleted))
            {
                MessageBox.Show(eHCMSResources.A0510_G1_Msg_InfoDaTonTaiDV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (MedServiceItem == null)
            {
                return;
            }
            gRefTreatmentRegimen.RefTreatmentRegimenServiceDetails.Add(new RefTreatmentRegimenServiceDetail
            {
                TreatmentRegimenID = gRefTreatmentRegimen.TreatmentRegimenID,
                MedServiceID = MedServiceItem.MedServiceID,
                MedicalServiceTypeID = (long)MedServiceItem.MedicalServiceTypeID,
                MedicalServiceTypeName = MedServiceItem.RefMedicalServiceType != null ? MedServiceItem.RefMedicalServiceType.MedicalServiceTypeName : "",
                MedServiceCode = MedServiceItem.MedServiceCode,
                MedServiceName = MedServiceItem.MedServiceName,
                V_TreatmentPeriodic = SelectedTreatmentPeriodic,
                IsDeleted = false,
                CreatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0,
                LastUpdatedStaffID = Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : 0,
                TreatmentPeriodic = V_TreatmentPeriodicCollection.FirstOrDefault(x => x.LookupID == SelectedTreatmentPeriodic).ObjectValue
            });
            CV_RefTreatmentRegimenServiceDetails.Refresh();
        }
        //▲===== #001

        #endregion

        #region Methods
        private bool CheckBeforeSave()
        {
            if (string.IsNullOrEmpty(gRefTreatmentRegimen.TreatmentRegimenCode))
            {
                MessageBox.Show(eHCMSResources.Z2269_G1_MaPhacDoKhongDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            else if (string.IsNullOrEmpty(gRefTreatmentRegimen.TreatmentRegimenName))
            {
                MessageBox.Show(eHCMSResources.Z2270_G1_TenPhacDoKhongDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            else if (string.IsNullOrEmpty(gRefTreatmentRegimen.ICD10Code))
            {
                MessageBox.Show(eHCMSResources.A0199_G1_Msg_YCNhapICD10, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            if (gRefTreatmentRegimen.LastUpdatedStaffID == 0)
            {
                MessageBox.Show(eHCMSResources.Z0126_G1_Chon1NVien, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            return true;
        }
        #endregion
    }
}