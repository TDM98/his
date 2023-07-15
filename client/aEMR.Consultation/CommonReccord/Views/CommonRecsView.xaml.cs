using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ConsultantEPrescription.Views;
using aEMR.Controls;


namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{
    [Export(typeof(CommonRecsView))]
	public partial class CommonRecsView : AxUserControl
	{
        private bool _allowChangeTabs;
        public CommonRecsView()
		{
			// Required to initialize variables
			InitializeComponent();
            InitControls();
		}

        private void InitControls()
        {
            AllowChangeTabs = true;
            tabCommon.IsEnabled = true;
        }

        public bool AllowChangeTabs
        {
            get
            {
                return _allowChangeTabs;
            }
            set
            {
                _allowChangeTabs = value;
            }
        }
        private void tabCommon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

	}
}