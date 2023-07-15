using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows.Controls;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IPreviewDQGReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PreviewDQGReportViewModel : ViewModelBase, IPreviewDQGReport
    {
        [ImportingConstructor]
        public PreviewDQGReportViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
        }
        #region Properties
        private ComboBox cboTableTypes { get; set; }
        private DataSet _gReportDetails;
        private DataTable _gDataTable;
        public DataSet gReportDetails
        {
            get => _gReportDetails; set
            {
                _gReportDetails = value;
                NotifyOfPropertyChange(() => gReportDetails);
            }
        }
        public DataTable gDataTable
        {
            get => _gDataTable; set
            {
                _gDataTable = value;
                NotifyOfPropertyChange(() => gDataTable);
            }
        }
        #endregion
        #region Events
        public void cboTableTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTableTypes == null)
            {
                return;
            }
            ViewTable();
        }
        public void cboTableTypes_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender == null || !(sender is ComboBox))
            {
                return;
            }
            cboTableTypes = sender as ComboBox;
            cboTableTypes.SelectedIndex = 0;
            ViewTable();
        }
        #endregion
        #region Methods
        public void ViewTable()
        {
            if (cboTableTypes == null)
            {
                return;
            }
            int mTableIndex = cboTableTypes.SelectedIndex;
            if (gReportDetails == null || gReportDetails.Tables.Count <= mTableIndex || mTableIndex < 0)
            {
                return;
            }
            gDataTable = gReportDetails.Tables[mTableIndex];
        }
        #endregion
    }
}