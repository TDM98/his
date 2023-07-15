using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(ISearchMedicalServiceGroups)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchMedicalServiceGroupsViewModel : ViewModelBase, ISearchMedicalServiceGroups
    {
        [ImportingConstructor]
        public SearchMedicalServiceGroupsViewModel(IWindsorContainer aContainer, INavigationService aNavigationService, IEventAggregator aEventAggregator)
        {
        }

        #region Properties
        private long _V_RegistrationType = 0;
        public long V_RegistrationType
        {
            get { return _V_RegistrationType; }
            set
            {
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
        }

        private string _SearchMedicalServiceGroupCode;
        private List<RefMedicalServiceGroups> _MedicalServiceGroupCollection;
        private ObservableCollection<RefMedicalServiceGroups> _SearchMedicalServiceGroupCollection;
        private RefMedicalServiceGroups _SelectedRefMedicalServiceGroup;
        public string SearchMedicalServiceGroupCode
        {
            get => _SearchMedicalServiceGroupCode; set
            {
                _SearchMedicalServiceGroupCode = value;
                NotifyOfPropertyChange(() => SearchMedicalServiceGroupCode);
            }
        }
        public List<RefMedicalServiceGroups> MedicalServiceGroupCollection
        {
            get => _MedicalServiceGroupCollection; set
            {
                _MedicalServiceGroupCollection = value;
                NotifyOfPropertyChange(() => MedicalServiceGroupCollection);
            }
        }
        public ObservableCollection<RefMedicalServiceGroups> SearchMedicalServiceGroupCollection
        {
            get => _SearchMedicalServiceGroupCollection; set
            {
                _SearchMedicalServiceGroupCollection = value;
                NotifyOfPropertyChange(() => SearchMedicalServiceGroupCollection);
            }
        }
        public RefMedicalServiceGroups SelectedRefMedicalServiceGroup
        {
            get => _SelectedRefMedicalServiceGroup; set
            {
                _SelectedRefMedicalServiceGroup = value;
                NotifyOfPropertyChange(() => SelectedRefMedicalServiceGroup);
            }
        }
        private string _SearchMedicalServiceGroupName;
        public string SearchMedicalServiceGroupName
        {
            get => _SearchMedicalServiceGroupName; set
            {
                _SearchMedicalServiceGroupName = value;
                NotifyOfPropertyChange(() => SearchMedicalServiceGroupName);
            }
        }
        private long _V_MedicalServiceGroupType;
        public long V_MedicalServiceGroupType
        {
            get => _V_MedicalServiceGroupType; set
            {
                _V_MedicalServiceGroupType = value;
                NotifyOfPropertyChange(() => V_MedicalServiceGroupType);
            }
        }
        private ObservableCollection<Lookup> _ListV_MedicalServiceGroupType;
        public ObservableCollection<Lookup> ListV_MedicalServiceGroupType
        {
            get { return _ListV_MedicalServiceGroupType; }
            set
            {
                if (_ListV_MedicalServiceGroupType != value)
                    _ListV_MedicalServiceGroupType = value;
                NotifyOfPropertyChange(() => ListV_MedicalServiceGroupType);
            }
        }
        #endregion

        #region Events
        public void gvSearchContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid) || (sender as DataGrid).SelectedItem == null || !((sender as DataGrid).SelectedItem is RefMedicalServiceGroups))
            {
                return;
            }
            SelectedRefMedicalServiceGroup = ((sender as DataGrid).SelectedItem as RefMedicalServiceGroups);
            TryClose();
        }
        #endregion

        #region Methods
        public void ApplySearchContent(List<RefMedicalServiceGroups> aMedicalServiceGroupCollection, string aSearchMedicalServiceGroupCode, List<RefMedicalServiceGroups> aSearchMedicalServiceGroupCollection)
        {
            MedicalServiceGroupCollection = aMedicalServiceGroupCollection.Where(x => x.V_RegistrationType == V_RegistrationType || x.V_RegistrationType == 0 || V_RegistrationType == 0).ToList();
            SearchMedicalServiceGroupCode = aSearchMedicalServiceGroupCode;
            SearchMedicalServiceGroupCollection = aSearchMedicalServiceGroupCollection != null ? aSearchMedicalServiceGroupCollection.ToObservableCollection() : MedicalServiceGroupCollection.ToObservableCollection();
            ListV_MedicalServiceGroupType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedicalServiceGroupType).ToObservableCollection();
        }

        public void btnSearchMedServiceGroups()
        {
            if (MedicalServiceGroupCollection == null || MedicalServiceGroupCollection.Count() == 0)
            {
                return;
            }
            if (SearchMedicalServiceGroupCode == null)
            {
                SearchMedicalServiceGroupCode = "";
            }
            if (SearchMedicalServiceGroupName == null)
            {
                SearchMedicalServiceGroupName = "";
            }
            var SearchCode = Globals.RemoveVietnameseString(SearchMedicalServiceGroupCode.ToLower());
            var SearchName = Globals.RemoveVietnameseString(SearchMedicalServiceGroupName.ToLower());
            var SearchMedicalServiceGroups = MedicalServiceGroupCollection.Where(x => 
                (!string.IsNullOrEmpty(x.MedicalServiceGroupCode) && Globals.RemoveVietnameseString(x.MedicalServiceGroupCode.ToLower()).Contains(SearchCode)) && 
                (!string.IsNullOrWhiteSpace(x.MedicalServiceGroupName) && Globals.RemoveVietnameseString(x.MedicalServiceGroupName.ToLower()).Contains(SearchName)) && 
                x.V_MedicalServiceGroupType == V_MedicalServiceGroupType //&& (x.V_RegistrationType == V_RegistrationType || x.V_RegistrationType == 0)
                ).ToList();
            SearchMedicalServiceGroupCollection = new ObservableCollection<RefMedicalServiceGroups>();
            if (SearchMedicalServiceGroups != null)
            {
                SearchMedicalServiceGroupCollection = new ObservableCollection<RefMedicalServiceGroups>(SearchMedicalServiceGroups);
            }
        }
        #endregion
    }
}
