/*
 * 20182905 TTM #001: Fix bug hien thong bao ben duoi popup.
 */
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using aEMR.Controls;
using System.Windows.Input;
using aEMR.Common;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResTypeListSelector)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResTypeListSelectorViewModel : Conductor<object>, IResTypeListSelector
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResTypeListSelectorViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            InitData();
        }

        private ObservableCollection<RefMedicalServiceType> _refResourceTypes;
        public ObservableCollection<RefMedicalServiceType> refResourceTypes
        {
            get
            {
                return _refResourceTypes;
            }
            set
            {
                if (_refResourceTypes == value)
                    return;
                _refResourceTypes = value;
                if (_refResourceTypes != null)
                {
                    refResourceType = refResourceTypes.FirstOrDefault();
                }
                NotifyOfPropertyChange(() => refResourceTypes);
                LoadSelectedResourceTypes();
            }
        }

        private ObservableCollection<RefMedicalServiceType> _selectedResourceTypes;
        public ObservableCollection<RefMedicalServiceType> selectedResourceTypes
        {
            get
            {
                return _selectedResourceTypes;
            }
            set
            {
                if (_selectedResourceTypes == value)
                    return;
                _selectedResourceTypes = value;
                NotifyOfPropertyChange(() => selectedResourceTypes);
            }
        }

        private ObservableCollection<ResourceTypeLists> _RscrTypeListsOld;
        public ObservableCollection<ResourceTypeLists> RscrTypeListsOld
        {
            get
            {
                return _RscrTypeListsOld;
            }
            set
            {
                if (_RscrTypeListsOld == value)
                    return;
                _RscrTypeListsOld = value;
                NotifyOfPropertyChange(() => RscrTypeListsOld);
                LoadSelectedResourceTypes();
            }
        }
        
        private RefMedicalServiceType _refResourceType;
        public RefMedicalServiceType refResourceType
        {
            get
            {
                return _refResourceType;
            }
            set
            {
                if (_refResourceType == value)
                    return;
                _refResourceType = value;
                NotifyOfPropertyChange(() => refResourceType);
            }
        }

        private List<long> _MServiceTypeListID;
        public List<long> MServiceTypeListID
        {
            get
            {
                return _MServiceTypeListID;
            }
            set
            {
                if (_MServiceTypeListID == value)
                    return;
                _MServiceTypeListID = value;
                NotifyOfPropertyChange(() => MServiceTypeListID);
            }
        }

        private string _MServiceTypeListIDStr;
        public string MServiceTypeListIDStr
        {
            get
            {
                return _MServiceTypeListIDStr;
            }
            set
            {
                if (_MServiceTypeListIDStr == value)
                    return;
                _MServiceTypeListIDStr = value;
                NotifyOfPropertyChange(() => MServiceTypeListIDStr);
            }
        }


        private void InitData()
        {
            if (refResourceTypes == null)
            {
                refResourceTypes = new ObservableCollection<RefMedicalServiceType>();
            }
            
            if (selectedResourceTypes == null)
            {
                selectedResourceTypes = new ObservableCollection<RefMedicalServiceType>();
            }
                
            if (refResourceType == null)
            {
                refResourceType = new RefMedicalServiceType();
            }
                
            if (MServiceTypeListID == null)
            {
                MServiceTypeListID = new List<long>();
            }
        }

        public void CancelButton()
        {
            TryClose();
        }
        public void OKButton()
        {
            if (MServiceTypeListIDStr != null)
            {
                Globals.EventAggregator.Publish(new OnChangedSelectionResourceTypes() { selectedResourceTypes = selectedResourceTypes, MServiceTypeListIDStr = MServiceTypeListIDStr });
            }
            else
            {
                MessageBox.Show("Chưa chọn loại vật tư!");
                return;
            }

            TryClose();
        }
                
        public void LoadSelectedResourceTypes()
        {
            selectedResourceTypes = new ObservableCollection<RefMedicalServiceType>();

            if (RscrTypeListsOld != null && selectedResourceTypes != null && refResourceTypes != null)
            {
                try
                {
                    foreach (var item in RscrTypeListsOld)
                    {
                        var findResourceType = refResourceTypes.Where(x => x.MedicalServiceTypeID == item.MedicalServiceTypeID).ToList().FirstOrDefault();
                        if (findResourceType != null)
                        {
                            selectedResourceTypes.Add(findResourceType);
                        }
                        MServiceTypeListID.Add(item.MedicalServiceTypeID);
                    }
                    MServiceTypeListIDStr = String.Join(";", MServiceTypeListID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
        }

        public void AddResourceType()
        {
            if (CheckNotExists(refResourceType))
            {
                GetRscrTypeNameFinal(refResourceType);
            }
        }

        public void GetRscrTypeNameFinal(RefMedicalServiceType resourceType)
        {
            if (resourceType != null)
            {
                selectedResourceTypes.Add(resourceType);
                MServiceTypeListID.Add(resourceType.MedicalServiceTypeID);
                MServiceTypeListIDStr = String.Join(";", MServiceTypeListID);
            }
        }

        public void RemoveRscrTypeName(RefMedicalServiceType resourceType)
        {
            if (resourceType != null)
            {
                selectedResourceTypes.Remove(resourceType);
                MServiceTypeListID.Remove(resourceType.MedicalServiceTypeID);
                MServiceTypeListIDStr = String.Join(";", MServiceTypeListID);
            }
        }

        private bool CheckNotExists(RefMedicalServiceType resourceType, bool HasMessage = true)
        {
            int i = 0;
            if (resourceType == null)
            {
                return true;
            }
            foreach (RefMedicalServiceType p in selectedResourceTypes)
            {
                if (p != null)
                {
                    if (resourceType.MedicalServiceTypeID == p.MedicalServiceTypeID)
                    {
                        i++;
                    }
                }
            }
            if (i > 0)
            {
                resourceType = null;
                if (HasMessage)
                {
                    MessageBox.Show("Loại đã tồn tại!");
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RemoveRscrTypeName(elem.DataContext as RefMedicalServiceType);
            }
        }
        
    }
}
