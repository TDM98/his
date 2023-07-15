using System.Windows.Controls;

namespace aEMR.Controls
{
    public class AxDataPager : DataPager
    {
        private TextBox _currentPageTextBox;
        public TextBox CurrentPageTextBox
        {
            get
            {
                return _currentPageTextBox;
            }
        }
      
        public AxDataPager()
            : base()
        {
           
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _currentPageTextBox = GetTemplateChild("CurrentPageTextBox") as TextBox;
        }

       

    }
}