using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace aEMR.WPFControls
{
    public class CalendarDayHeader : Control
    {
        private const string ElementDayHeaderLabel = "PART_DayHeaderLabel";
        static CalendarDayHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDayHeader), new FrameworkPropertyMetadata(typeof(CalendarDayHeader)));
        }
        public Calendar Owner { get; set; }

        private BindingBase GetOwnerBinding(string propertyName)
        {
            Binding result = new Binding(propertyName);
            result.Source = this.Owner;
            return result;
        }

        TextBlock _dayHeaderLabel;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dayHeaderLabel = GetTemplateChild(ElementDayHeaderLabel) as TextBlock;

            PopulateHeader();
        }

        void PopulateHeader()
        {
            BindingBase binding = GetOwnerBinding("CurrentDate");
            binding.StringFormat = "{0:D}";
            _dayHeaderLabel.SetBinding(TextBlock.TextProperty, binding);
        }
    }
}