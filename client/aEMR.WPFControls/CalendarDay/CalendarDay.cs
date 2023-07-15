using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace aEMR.WPFControls
{
    public class CalendarDay : ItemsControl
    {
        private const string ElementTimeslotItems = "PART_TimeslotItems";
        StackPanel _dayItems;
        static CalendarDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDay), new FrameworkPropertyMetadata(typeof(CalendarDay)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _dayItems = GetTemplateChild(ElementTimeslotItems) as StackPanel;

            PopulateDay();
        }

        public void PopulateDay()
        {
            if (_dayItems != null)
            {
                _dayItems.Children.Clear();
                DateTime? ValidDateFrom = null;
                DateTime? ValidDateTo = null;
                if (Owner != null && Owner.ValidDateTime != null && Owner.ValidDateTime.Length == 2)
                {
                    ValidDateFrom = new DateTime(Owner.CurrentDate.Year, Owner.CurrentDate.Month, Owner.CurrentDate.Day, 0, 0, 0).AddHours(Owner.ValidDateTime[0].Hour).AddMinutes(Owner.ValidDateTime[0].Minute);
                    ValidDateTo = new DateTime(Owner.CurrentDate.Year, Owner.CurrentDate.Month, Owner.CurrentDate.Day, 0, 0, 0).AddHours(Owner.ValidDateTime[1].Hour).AddMinutes(Owner.ValidDateTime[1].Minute);
                }
                DateTime startTime = new DateTime(Owner.CurrentDate.Year, Owner.CurrentDate.Month, Owner.CurrentDate.Day, 0, 0, 0);
                for (int i = 0; i < 96; i++)
                {
                    CalendarTimeslotItem timeslot = new CalendarTimeslotItem();
                    timeslot.StartTime = startTime;
                    timeslot.EndTime = startTime + TimeSpan.FromMinutes(15);
                    if (ValidDateFrom != null && ValidDateTo != null && (timeslot.StartTime < ValidDateFrom || timeslot.EndTime > ValidDateTo))
                    {
                        timeslot.IsEnabled = false;
                    }
                    if (!timeslot.IsEnabled)
                    {
                        timeslot.Background = System.Windows.Media.Brushes.Lavender;
                    }
                    //else if (startTime.Hour >= 8 && startTime.Hour <= 17)
                    //{
                    //    timeslot.SetBinding(Calendar.BackgroundProperty, GetOwnerBinding("PeakTimeslotBackground"));
                    //}
                    else
                    {
                        //timeslot.SetBinding(Calendar.BackgroundProperty, GetOwnerBinding("OffPeakTimeslotBackground"));
                        timeslot.SetBinding(Calendar.BackgroundProperty, GetOwnerBinding("PeakTimeslotBackground"));
                    }
                    timeslot.SetBinding(CalendarTimeslotItem.StyleProperty, GetOwnerBinding("CalendarTimeslotItemStyle"));
                    _dayItems.Children.Add(timeslot);
                    startTime = startTime + TimeSpan.FromMinutes(15);
                }
            }
            if (Owner != null)
            {
                Owner.ScrollToHome();
            }
        }

        #region ItemsControl Container Override

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CalendarAppointmentItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is CalendarAppointmentItem);
        }

        #endregion

        public Calendar Owner { get; set; }

        private BindingBase GetOwnerBinding(string propertyName)
        {
            Binding result = new Binding(propertyName);
            result.Source = this.Owner;
            return result;
        }
    }
}