using System.Collections.Generic;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using System.Linq;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultingDiagnosysGeneral)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConsultingDiagnosysGeneralViewModel : ViewModelBase, IConsultingDiagnosysGeneral
    {
        [ImportingConstructor]
        public ConsultingDiagnosysGeneralViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
        }
        #region Method
        public void InitDataView()
        {
            if (gConsultingDiagnosys.V_ValveType != null && gConsultingDiagnosys.V_ValveType.LookupID > 0)
                gConsultingDiagnosys.V_ValveType = ValveTypeArray.Where(x => x.LookupID == gConsultingDiagnosys.V_ValveType.LookupID).FirstOrDefault();
        }
        #endregion
        #region Properties
        private ConsultingDiagnosys _ConsultingDiagnosys;
        public ConsultingDiagnosys gConsultingDiagnosys
        {
            get
            {
                return _ConsultingDiagnosys;
            }
            set
            {
                _ConsultingDiagnosys = value;
                NotifyOfPropertyChange(() => gConsultingDiagnosys);
            }
        }
        private List<Lookup> _DiagnosticTypeArray;
        public List<Lookup> DiagnosticTypeArray
        {
            get
            {
                return _DiagnosticTypeArray;
            }
            set
            {
                _DiagnosticTypeArray = value;
                NotifyOfPropertyChange(() => DiagnosticTypeArray);
            }
        }
        private List<Lookup> _TreatmentMethodArray;
        public List<Lookup> TreatmentMethodArray
        {
            get
            {
                return _TreatmentMethodArray;
            }
            set
            {
                _TreatmentMethodArray = value;
                NotifyOfPropertyChange(() => TreatmentMethodArray);
            }
        }
        private List<Lookup> _HeartSurgicalTypeArray;
        public List<Lookup> HeartSurgicalTypeArray
        {
            get
            {
                return _HeartSurgicalTypeArray;
            }
            set
            {
                _HeartSurgicalTypeArray = value;
                NotifyOfPropertyChange(() => HeartSurgicalTypeArray);
            }
        }
        private List<Lookup> _ValveTypeArray;
        public List<Lookup> ValveTypeArray
        {
            get
            {
                return _ValveTypeArray;
            }
            set
            {
                _ValveTypeArray = value;
                NotifyOfPropertyChange(() => ValveTypeArray);
            }
        }
        #endregion
    }
}