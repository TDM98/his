using System.Windows;
using System.Windows.Controls;
namespace aEMR.WPFControls
{
    //[System.Windows.TemplateVisualState(Name = CalendarAppointmentItem.StateNormal, GroupName = CalendarAppointmentItem.GroupCommon)]
    //[System.Windows.TemplateVisualState(Name = CalendarAppointmentItem.StateMouseOver, GroupName = CalendarAppointmentItem.GroupCommon)]
    //[System.Windows.TemplateVisualState(Name = CalendarAppointmentItem.StateDisabled, GroupName = CalendarAppointmentItem.GroupCommon)]
    //[ToolboxBrowsable(false)]
    public class CalendarAppointmentItem : ContentControl
    {
        public const string StateNormal = "Normal";
        public const string StateMouseOver = "MouseOver";
        public const string StateDisabled = "Disabled";
        public const string GroupCommon = "CommonStates";
        static CalendarAppointmentItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarAppointmentItem), new FrameworkPropertyMetadata(typeof(CalendarAppointmentItem)));
        }
        #region StartTime/EndTime
        public static readonly DependencyProperty StartTimeProperty =
            TimeslotPanel.StartTimeProperty.AddOwner(typeof(CalendarAppointmentItem));

        public bool StartTime
        {
            get { return (bool)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        public static readonly DependencyProperty EndTimeProperty =
            TimeslotPanel.EndTimeProperty.AddOwner(typeof(CalendarAppointmentItem));

        public bool EndTime
        {
            get { return (bool)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }
        #endregion
    }
}