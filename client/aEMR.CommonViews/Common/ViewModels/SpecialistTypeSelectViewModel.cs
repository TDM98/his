using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(ISpecialistTypeSelect)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SpecialistTypeSelectViewModel : ViewModelBase, ISpecialistTypeSelect
    {
        #region Properties
        private bool IsConfirmed = false;
        private Lookup _SelectedSpecialistType;
        private ObservableCollection<Lookup> _SpecialistTypeCollection;
        public Lookup SelectedSpecialistType
        {
            get
            {
                return _SelectedSpecialistType;
            }
            set
            {
                if (_SelectedSpecialistType == value)
                {
                    return;
                }
                _SelectedSpecialistType = value;
                NotifyOfPropertyChange(() => SelectedSpecialistType);
            }
        }
        public ObservableCollection<Lookup> SpecialistTypeCollection
        {
            get
            {
                return _SpecialistTypeCollection;
            }
            set
            {
                if (_SpecialistTypeCollection == value)
                {
                    return;
                }
                _SpecialistTypeCollection = value;
                NotifyOfPropertyChange(() => SpecialistTypeCollection);
            }
        }
        public Lookup ConfirmedSpecialistType
        {
            get
            {
                return !IsConfirmed ? null : SelectedSpecialistType;
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public SpecialistTypeSelectViewModel()
        {
            SpecialistTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_SpecialistType).ToObservableCollection();
            SelectedSpecialistType = SpecialistTypeCollection.FirstOrDefault();
        }
        public void ConfirmCmd()
        {
            IsConfirmed = true;
            TryClose();
        }
        #endregion
    }
}