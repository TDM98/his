using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;
using aEMR.Controls;
using aEMR.Common.Collections;
using aEMR.Common.Utilities;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities.MedicalInstruction;
/*
* 20211112 #001 TNHX: Tạo màn hình mới cho chỉ định gói DVKT
*/
namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IInPatientSelectPackage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientSelectPackageViewModel : Conductor<object>, IInPatientSelectPackage
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientSelectPackageViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            PackageTechnicalServiceCombo_Search();
        }

        private PCLFormsSearchCriteria _searchCriteria;
        public PCLFormsSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

            }
        }

        public Dictionary<long, PCLExamType> _dicPCLExamType;
        public Dictionary<long, PCLExamType> dicPCLExamType
        {
            get
            {
                return _dicPCLExamType;
            }
            set
            {
                _dicPCLExamType = value;
                NotifyOfPropertyChange(() => dicPCLExamType);

            }
        }

        private PackageTechnicalService _selectedCombo;
        public PackageTechnicalService SelectedCombo
        {
            get { return _selectedCombo; }
            set
            {
                _selectedCombo = value;
                NotifyOfPropertyChange(() => SelectedCombo);
                if (_selectedCombo != null && _selectedCombo.PackageTechnicalServiceID > 0)
                {
                    if (CurrentInPatientAdmDisDetail != null && CurrentInPatientAdmDisDetail.PCLExamTypePriceListID > 0)
                    {
                        LoadAllPCLExamTypesByComboIDAction(_selectedCombo.PackageTechnicalServiceID);
                    }
                    else
                    {
                        PackageTechnicalServiceDetail_ByID(_selectedCombo.PackageTechnicalServiceID, Globals.ListPackageTechnicalServiceDetailAll);
                    }
                }
            }
        }

        private PCLExamTypeLocation _selectedPclExamTypeLocation;
        public PCLExamTypeLocation SelectedPclExamTypeLocation
        {
            get { return _selectedPclExamTypeLocation; }
            set
            {
                _selectedPclExamTypeLocation = value;
                NotifyOfPropertyChange(() => SelectedPclExamTypeLocation);
            }
        }

        private ObservableCollection<PCLItem> _pclItemList;

        public ObservableCollection<PCLItem> PCLItemList
        {
            get { return _pclItemList; }
            set
            {
                _pclItemList = value;
                NotifyOfPropertyChange(() => PCLItemList);
            }
        }

        public long? DeptID { get; set; }

        private InPatientAdmDisDetails _CurrentInPatientAdmDisDetail;
        public InPatientAdmDisDetails CurrentInPatientAdmDisDetail
        {
            get
            {
                return _CurrentInPatientAdmDisDetail;
            }
            set
            {
                if (_CurrentInPatientAdmDisDetail == value)
                {
                    return;
                }
                if (_CurrentInPatientAdmDisDetail != null && SelectedCombo != null && PackageTechnicalServiceCombos != null)
                {
                    if (value == null || value.PCLExamTypePriceListID != _CurrentInPatientAdmDisDetail.PCLExamTypePriceListID)
                    {
                        SelectedCombo = PackageTechnicalServiceCombos.FirstOrDefault();
                    }
                }
                _CurrentInPatientAdmDisDetail = value;
            }
        }

        private ObservableCollection<PackageTechnicalService> _PackageTechnicalServiceCombos;
        public ObservableCollection<PackageTechnicalService> PackageTechnicalServiceCombos
        {
            get { return _PackageTechnicalServiceCombos; }
            set
            {
                _PackageTechnicalServiceCombos = value;
                NotifyOfPropertyChange(() => PackageTechnicalServiceCombos);
            }
        }

        private void PackageTechnicalServiceCombo_Search()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPackageTechnicalService_Search(new GeneralSearchCriteria(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndPackageTechnicalService_Search(asyncResult);
                                PackageTechnicalServiceCombos = allItems.ToObservableCollection();
                                PackageTechnicalService ItemDefault = new PackageTechnicalService();
                                ItemDefault.PackageTechnicalServiceID = -1;
                                ItemDefault.Title = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0186_G1_ChonComboXN);
                                PackageTechnicalServiceCombos.Insert(0, ItemDefault);
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void PackageTechnicalServiceDetail_ByID(long ID, IList<PackageTechnicalServiceDetail> ListPackageTechnicalServiceDetailAll)
        {
            IList<PackageTechnicalServiceDetail> listpcl = new List<PackageTechnicalServiceDetail>();
            ObservableCollection<PackageTechnicalServiceDetail> listpclCombo = new ObservableCollection<PackageTechnicalServiceDetail>();
            if (IsRegimenChecked && ListRegiment != null)
            {
                PackageTechnicalServiceDetail TreatmentRegimenPCLDetail = new PackageTechnicalServiceDetail();
                foreach (var item in ListRegiment)
                {
                    if (item.RefTreatmentRegimenPCLDetails != null && item.RefTreatmentRegimenPCLDetails.Count > 0)
                    {
                        foreach (var detail in item.RefTreatmentRegimenPCLDetails)
                        {
                            TreatmentRegimenPCLDetail = ListPackageTechnicalServiceDetailAll.Where(x => x.PCLExamTypeID == detail.PCLExamTypeID).FirstOrDefault();
                            if (TreatmentRegimenPCLDetail != null && !listpcl.Any(x => x.PCLExamTypeID == detail.PCLExamTypeID))
                            {
                                listpclCombo.Add(TreatmentRegimenPCLDetail);
                            }
                        }
                    }
                }
            }
            else
            {
                listpclCombo = ListPackageTechnicalServiceDetailAll.ToObservableCollection();
            }
            // tự động add qua danh sách bên phải luôn
            if (listpclCombo != null && listpclCombo.Count > 0)
            {
                Globals.EventAggregator.Publish(new AddAllPackageTechnicalService { ListpclCombo = listpclCombo });
            }
        }

        private void LoadAllPCLExamTypesByComboIDAction(long PCLExamTypeComboID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPackageTechnicalServiceDetail_All(CurrentInPatientAdmDisDetail.PCLExamTypePriceListID
                            , Globals.DispatchCallback(delegate (IAsyncResult asyncResult)
                        {
                            try
                            {
                                IList<PackageTechnicalServiceDetail> allItems = client.EndPackageTechnicalServiceDetail_All(asyncResult);
                                if (allItems == null)
                                {
                                    allItems = new List<PackageTechnicalServiceDetail>();
                                }
                                PackageTechnicalServiceDetail_ByID(_selectedCombo.PackageTechnicalServiceID, allItems);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                }
            });

            t.Start();
        }

        private bool _IsRegimenChecked;
        public bool IsRegimenChecked
        {
            get
            {
                return _IsRegimenChecked;
            }
            set
            {
                _IsRegimenChecked = value;
                NotifyOfPropertyChange(() => IsRegimenChecked);
            }
        }

        private List<RefTreatmentRegimen> _ListRegiment = new List<RefTreatmentRegimen>();
        public List<RefTreatmentRegimen> ListRegiment
        {
            get { return _ListRegiment; }
            set
            {
                if (_ListRegiment != value)
                {
                    _ListRegiment = value;
                    NotifyOfPropertyChange(() => ListRegiment);
                }
            }
        }
    }
}