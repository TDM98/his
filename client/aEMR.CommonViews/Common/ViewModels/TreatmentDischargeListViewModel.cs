using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using DataEntities;
using System.Linq;
using eHCMSLanguage;
using aEMR.Common.Collections;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ITreatmentDischargeList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TreatmentDischargeListViewModel : Conductor<object>, ITreatmentDischargeList
    {

        private string _TitleForm = "";
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        IEventAggregator _eventArg;
        [ImportingConstructor]
        public TreatmentDischargeListViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            LoadTreatmentDischargeList();
            LoadTreatmentDischargeDetailList();
        }

        protected override void OnActivate()
        {
            _eventArg.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }

        public void LoadTreatmentDischargeList()
        {
            DiseaseProgression U2W = new DiseaseProgression {
                DiseaseProgressionID = (long)LookupValues.V_U22WPregnancy,
                DiseaseProgressionName = "Thai dưới 22 tuần tuổi"
            };
            DiseaseProgression.Add(U2W);
            DiseaseProgression O2W = new DiseaseProgression
            {
                DiseaseProgressionID = (long)LookupValues.V_O22WPregnancy,
                DiseaseProgressionName = "Thai trên 22 tuần tuổi"
            };
            DiseaseProgression.Add(O2W);
        }
        public void LoadTreatmentDischargeDetailList()
        {
            ObservableCollection<Lookup>  V_U22WPregnancy = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_U22WPregnancy).ToObservableCollection();
            ObservableCollection<Lookup> V_O22WPregnancy = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_O22WPregnancy).ToObservableCollection();
            if(V_U22WPregnancy != null && V_U22WPregnancy.Count > 0)
            {
                foreach (Lookup LK in V_U22WPregnancy)
                {
                    DiseaseProgressionDetails ob = new DiseaseProgressionDetails
                    {
                        DiseaseProgressionID = LK.ObjectTypeID,
                        DiseaseProgressionDetailName = LK.ObjectValue,
                        DiseaseProgressionDetailID = LK.LookupID
                    };
                    DiseaseProgressionDetails.Add(ob);
                }
            }
            if (V_O22WPregnancy != null && V_O22WPregnancy.Count > 0)
            {
                foreach (Lookup LK in V_O22WPregnancy)
                {
                    DiseaseProgressionDetails ob = new DiseaseProgressionDetails
                    {
                        DiseaseProgressionID = LK.ObjectTypeID,
                        DiseaseProgressionDetailName = LK.ObjectValue,
                        DiseaseProgressionDetailID = LK.LookupID
                    };
                    DiseaseProgressionDetails.Add(ob);
                }
            }
            if (DiseaseProgression != null && DiseaseProgressionDetails != null)
            {
                foreach (var item in DiseaseProgression)
                {
                    item.DiseaseProgressionDetails = DiseaseProgressionDetails.Where(x => x.DiseaseProgressionID == item.DiseaseProgressionID).ToList();
                }
            }
        }
        public void btnOK()
        {
            string SelectDiseaseProgression = "";
            foreach (var item in DiseaseProgression)
            {
                foreach (var detail in item.DiseaseProgressionDetails)
                {
                    if (detail.IsChecked)
                    {
                        SelectDiseaseProgression += "; " + detail.DiseaseProgressionDetailName;
                    }
                }
            }
            
            Globals.EventAggregator.Publish(new SelectedTreatmentDischarge_Event() { SelectedTreatmentDischarge = SelectDiseaseProgression });
            TryClose();
        }
        public void btnCancel()
        {
            TryClose();
        }

        private ObservableCollection<DiseaseProgression> _DiseaseProgression = new ObservableCollection<DiseaseProgression>();
        public ObservableCollection<DiseaseProgression> DiseaseProgression
        {
            get { return _DiseaseProgression; }
            set
            {
                _DiseaseProgression = value;
                NotifyOfPropertyChange(() => DiseaseProgression);
            }
        }
        private ObservableCollection<DiseaseProgressionDetails> _DiseaseProgressionDetails = new ObservableCollection<DiseaseProgressionDetails>();
        public ObservableCollection<DiseaseProgressionDetails> DiseaseProgressionDetails
        {
            get { return _DiseaseProgressionDetails; }
            set
            {
                _DiseaseProgressionDetails = value;
                NotifyOfPropertyChange(() => DiseaseProgressionDetails);
            }
        }
    }
}