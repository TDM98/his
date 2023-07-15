using aEMR.Common.BaseModel;
using aEMR.ViewContracts;
using System;
using System.ComponentModel.Composition;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IReportCriteria)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportCriteriaViewModel : ViewModelBase, IReportCriteria
    {
        #region Properties
        private DateTime _FromDate;
        private DateTime _ToDate;
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        public bool IsCompleted { get; set; } = false;
        #endregion
        #region Events
        public void btnOK()
        {
            IsCompleted = true;
            TryClose();
        }
        public void btnCancel()
        {
            TryClose();
        }
        #endregion
    }
}