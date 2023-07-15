using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace aEMR.WPFControls
{
    public partial class CalendarTimeslotItem : ButtonBase
    {
        public const string StateNormal = "Normal";
        public const string StateMouseOver = "MouseOver";
        public const string StateDisabled = "Disabled";
        public const string GroupCommon = "CommonStates";
        static CalendarTimeslotItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarTimeslotItem), new FrameworkPropertyMetadata(typeof(CalendarTimeslotItem)));
        }
        #region AddAppointment

        private void RaiseAddAppointmentEvent()
        {
            RoutedEventArgs e = new RoutedEventArgs();
            e.RoutedEvent = AddAppointmentEvent;
            e.Source = this;

            OnAddAppointment(e);
        }

        public static readonly RoutedEvent AddAppointmentEvent =
            EventManager.RegisterRoutedEvent("AddAppointment", RoutingStrategy.Bubble,
            typeof(RoutedEventArgs), typeof(CalendarTimeslotItem));

        public event RoutedEventHandler AddAppointment
        {
            add
            {
                AddHandler(AddAppointmentEvent, value);
            }
            remove
            {
                RemoveHandler(AddAppointmentEvent, value);
            }
        }

        protected virtual void OnAddAppointment(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion
        protected override void OnClick()
        {
            base.OnClick();
            if (!this.IsEnabled)
            {
                return;
            }
            RaiseAddAppointmentEvent();
        }
        #region StartTime

        /// <summary>
        /// StartTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(CalendarTimeslotItem),
                new FrameworkPropertyMetadata((DateTime)DateTime.Now));

        /// <summary>
        /// Gets or sets the StartTime property.  This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        #endregion
        #region EndTime
        /// <summary>
        /// StartTime Dependency Property
        /// </summary>
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(CalendarTimeslotItem),
                new FrameworkPropertyMetadata((DateTime)DateTime.Now));

        /// <summary>
        /// Gets or sets the StartTime property.  This dependency property 
        /// indicates ....
        /// </summary>
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }
        #endregion
    }
}