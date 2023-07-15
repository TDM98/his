using System;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure;
using eHCMSLanguage;
/*
 * 20220922 #001 QTD: Set mặc định giờ kết thúc 00:00
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IMinHourDateControl)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MinHourDateControlViewModel : Conductor<object>, IMinHourDateControl
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public MinHourDateControlViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        public AxTextBoxFilter.TextBoxFilterType IntNumberFilter
        {
            get
            {
                return AxTextBoxFilter.TextBoxFilterType.Integer;
            }
        }

        private DateTime? _dateTime = Globals.GetCurServerDateTime();
        public DateTime? DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    NotifyOfPropertyChange(() => DateTime);

                    if (!_dateTime.HasValue)
                    {
                        _datePart = null;
                        _hourPart = null;
                        _minutePart = null;
                        //▼==== #001
                        if (!_IsEnableMinHourControl)
                        {
                            _hourPart = 0;
                            _minutePart = 0;
                        }
                        //▲==== #001
                    }
                    else
                    {
                        _datePart = _dateTime.Value.Date;
                        _hourPart = _dateTime.Value.Hour;
                        _minutePart = _dateTime.Value.Minute;
                    }

                    NotifyOfPropertyChange(() => DatePart);
                    NotifyOfPropertyChange(() => HourPart);
                    NotifyOfPropertyChange(() => MinutePart);
                }
            }
        }

        private DateTime? _datePart = Globals.GetCurServerDateTime();
        public DateTime? DatePart
        {
            get { return _datePart; }
            set
            {
                if (_datePart != value && value != null)
                {
                    _datePart = value;
                    NotifyOfPropertyChange(() => DatePart);
                    CalcDateFromParts();
                }
            }
        }

        private int? _hourPart = Globals.GetCurServerDateTime().Hour;
        public int? HourPart
        {
            get { return _hourPart; }
            set
            {
                if (_hourPart != value)
                {
                    _hourPart = value;
                    NotifyOfPropertyChange(() => HourPart);

                    CalcDateFromParts();
                }
            }
        }

        private int? _minutePart = Globals.GetCurServerDateTime().Minute;
        public int? MinutePart
        {
            get { return _minutePart; }
            set
            {
                if (_minutePart != value)
                {
                    _minutePart = value;
                    NotifyOfPropertyChange(() => MinutePart);
                    CalcDateFromParts();
                }
            }
        }

        private bool _bShowButton;
        public bool bShowButton
        {
            get { return _bShowButton; }
            set
            {
                if (_bShowButton != value)
                {
                    _bShowButton = value;
                    NotifyOfPropertyChange(() => bShowButton);
                }
            }
        }

        private bool _bOK;
        public bool bOK
        {
            get { return _bOK; }
            set
            {
                if (_bOK != value)
                {
                    _bOK = value;
                    NotifyOfPropertyChange(() => bOK);
                }
            }
        }

        private void CalcDateFromParts()
        {
            if (!DatePart.HasValue)
            {
                DateTime = null;
                return;
            }
            if (MinutePart.Value > 59 || MinutePart.Value < 0)
            {
                //throw new Exception("Phút không hợp lệ. Trong khoảng 0 - 59");
                Globals.ShowMessage(eHCMSResources.Z2924_G1_PhutKhongHopLe, eHCMSResources.T0432_G1_Error);
                return;
            }
            if (HourPart.Value > 23 || HourPart.Value < 0)
            {
                Globals.ShowMessage(eHCMSResources.Z2924_G1_GioKhongHopLe, eHCMSResources.T0432_G1_Error);
                return;
            }
            int h = 0, m = 0;
            if (HourPart != null)
            {
                h = HourPart.Value;
            }
            if (MinutePart != null)
            {
                m = MinutePart.Value;
            }
            var date = DatePart.Value;
            _dateTime = new DateTime(date.Year, date.Month, date.Day, h, m, 0, 0);
            NotifyOfPropertyChange(() => DateTime);
            NotifyOfPropertyChange(() => DateTimeToString);
        }

        #region Event Handlers
        //public void DatePart_DateChanged(object source, DateTimeSelectedEventArgs eventArgs)
        //{
        //    if (eventArgs.NewValue != null)
        //    {
        //        DatePart = eventArgs.NewValue;
        //    }
        //}
        ////public void HourPart_KeyUp(AxTextBoxFilter source, KeyEventArgs eventArgs)
        ////{
        ////    int hour;
        ////    if (int.TryParse(source.Text, out hour))
        ////    {
        ////        HourPart = hour;
        ////    }
        ////}

        //public void HourPart_LostFocus(AxTextBoxFilter source, object eventArgs)
        //{
        //    int.TryParse(source.Text, out int hour);
        //    if (hour > 24 || hour < 0)
        //    {
        //        throw new Exception("Giờ không hợp lệ. Trong khoảng 0 - 24");
        //    }
        //    if (hour > 0)
        //    {
        //        HourPart = hour;
        //    }
        //    else
        //    {
        //        HourPart = null;
        //    }
        //}

        //public void MinutePart_LostFocus(AxTextBoxFilter source, object eventArgs)
        //{
        //    int.TryParse(source.Text, out int minute);
        //    if (minute >= 60 || minute < 0)
        //    {
        //        throw new Exception("Phút không hợp lệ. Trong khoảng 0 - 59");
        //    }
        //    if (minute > 0)
        //    {
        //        MinutePart = minute;
        //    }
        //    else
        //    {
        //        MinutePart = null;
        //    }
        //}

        public void btnOK()
        {
            bOK = true;
            TryClose();
        }
        #endregion
        //▼==== #001
        private bool _IsEnableMinHourControl = true;
        public bool IsEnableMinHourControl
        {
            get { return _IsEnableMinHourControl; }
            set
            {
                if (_IsEnableMinHourControl != value)
                {
                    _IsEnableMinHourControl = value;
                    NotifyOfPropertyChange(() => IsEnableMinHourControl);
                }
            }
        }
        //▲==== #001

        public string DateTimeToString
        {
            get  { 
                return string.Format("{0} giờ {1} phút ngày {2}", HourPart.ToString(), MinutePart.ToString(), DateTime.GetValueOrDefault().ToString("dd/MM/yyyy")); 
            }
        }
    }
}
