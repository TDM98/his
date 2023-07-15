using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using System.ComponentModel.Composition;
using System.Data;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IPreviewHIReport130)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PreviewHIReport130ViewModel : ViewModelBase, IPreviewHIReport130
    {
        [ImportingConstructor]
        public PreviewHIReport130ViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
        }
        private DataSet _PreviewHIReportSet;
        private DataTable _DataTable1;
        private DataTable _DataTable2;
        private DataTable _DataTable3;
        private DataTable _DataTable4;
        private DataTable _DataTable5;
        private DataTable _DataTable6;
        private DataTable _DataTable7;
        private DataTable _DataTable8;
        private DataTable _DataTable9;
        private DataTable _DataTable10;
        private DataTable _DataTable11;
        private DataTable _DataTable12;
        private DataTable _DataTable13;
        private string _ErrText;
        public DataSet PreviewHIReportSet
        {
            get => _PreviewHIReportSet; set
            {
                _PreviewHIReportSet = value;
                NotifyOfPropertyChange(() => PreviewHIReportSet);
            }
        }
        public DataTable DataTable1
        {
            get => _DataTable1; set
            {
                _DataTable1 = value;
                NotifyOfPropertyChange(() => DataTable1);
            }
        }
        public DataTable DataTable2
        {
            get => _DataTable2; set
            {
                _DataTable2 = value;
                NotifyOfPropertyChange(() => DataTable2);
            }
        }
        public DataTable DataTable3
        {
            get => _DataTable3; set
            {
                _DataTable3 = value;
                NotifyOfPropertyChange(() => DataTable3);
            }
        }
        public DataTable DataTable4
        {
            get => _DataTable4; set
            {
                _DataTable4 = value;
                NotifyOfPropertyChange(() => DataTable4);
            }
        }
        public DataTable DataTable5
        {
            get => _DataTable5; set
            {
                _DataTable5 = value;
                NotifyOfPropertyChange(() => DataTable5);
            }
        }
        public DataTable DataTable6
        {
            get => _DataTable6; set
            {
                _DataTable6 = value;
                NotifyOfPropertyChange(() => DataTable6);
            }
        }
        public DataTable DataTable7
        {
            get => _DataTable7; set
            {
                _DataTable7 = value;
                NotifyOfPropertyChange(() => DataTable7);
            }
        }
        public DataTable DataTable8
        {
            get => _DataTable8; set
            {
                _DataTable8 = value;
                NotifyOfPropertyChange(() => DataTable8);
            }
        }
        public DataTable DataTable9
        {
            get => _DataTable9; set
            {
                _DataTable9 = value;
                NotifyOfPropertyChange(() => DataTable9);
            }
        }
        public DataTable DataTable10
        {
            get => _DataTable10; set
            {
                _DataTable10 = value;
                NotifyOfPropertyChange(() => DataTable10);
            }
        }
        public DataTable DataTable11
        {
            get => _DataTable11; set
            {
                _DataTable11 = value;
                NotifyOfPropertyChange(() => DataTable11);
            }
        }
        public DataTable DataTable12
        {
            get => _DataTable12; set
            {
                _DataTable12 = value;
                NotifyOfPropertyChange(() => DataTable12);
            }
        }
        public DataTable DataTable13
        {
            get => _DataTable13; set
            {
                _DataTable13 = value;
                NotifyOfPropertyChange(() => DataTable13);
            }
        }
        public string ErrText
        {
            get => _ErrText; set
            {
                _ErrText = value;
                NotifyOfPropertyChange(() => ErrText);
            }
        }
        public void ApplyPreviewHIReportSet(DataSet aPreviewHIReportSet, string aErrText)
        {
            if (aPreviewHIReportSet == null || aPreviewHIReportSet.Tables == null || aPreviewHIReportSet.Tables.Count != 3)
            {
                if (DataTable1 != null)
                {
                    DataTable1.Rows.Clear();
                    DataTable2.Rows.Clear();
                    DataTable3.Rows.Clear();
                    DataTable4.Rows.Clear();
                    DataTable5.Rows.Clear();
                    DataTable6.Rows.Clear();
                    DataTable7.Rows.Clear();
                    DataTable8.Rows.Clear();
                    DataTable9.Rows.Clear();
                    DataTable10.Rows.Clear();
                    DataTable11.Rows.Clear();
                    DataTable12.Rows.Clear();
                    DataTable13.Rows.Clear();
                }
            }
            DataTable1 = aPreviewHIReportSet.Tables[0];
            DataTable2 = aPreviewHIReportSet.Tables[1];
            DataTable3 = aPreviewHIReportSet.Tables[2];
            DataTable4 = aPreviewHIReportSet.Tables[3];
            DataTable5 = aPreviewHIReportSet.Tables[4];
            if (aPreviewHIReportSet.Tables.Count > 5)
            {
                DataTable6 = aPreviewHIReportSet.Tables[5];
            }
            if (aPreviewHIReportSet.Tables.Count > 6)
            {
                DataTable7 = aPreviewHIReportSet.Tables[6];
            }
            if (aPreviewHIReportSet.Tables.Count > 7)
            {
                DataTable8 = aPreviewHIReportSet.Tables[7];
            }
            if (aPreviewHIReportSet.Tables.Count > 8)
            {
                DataTable9 = aPreviewHIReportSet.Tables[8];
            }
            if (aPreviewHIReportSet.Tables.Count > 9)
            {
                DataTable10 = aPreviewHIReportSet.Tables[9];
            }
            if (aPreviewHIReportSet.Tables.Count > 10)
            {
                DataTable11 = aPreviewHIReportSet.Tables[10];
            }
            //DataTable7 = aPreviewHIReportSet.Tables[6];
            //DataTable8 = aPreviewHIReportSet.Tables[7];
            //DataTable9 = aPreviewHIReportSet.Tables[8];
            //DataTable10 = aPreviewHIReportSet.Tables[9];
            //DataTable11 = aPreviewHIReportSet.Tables[10];
            //DataTable12 = aPreviewHIReportSet.Tables[11];
            //DataTable13 = aPreviewHIReportSet.Tables[12];
            ErrText = aErrText;
        }
    }
}