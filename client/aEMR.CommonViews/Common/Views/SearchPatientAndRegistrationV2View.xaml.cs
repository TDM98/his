using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using System.Windows;
using System.Windows.Controls;
namespace aEMR.Common.Views
{
    public partial class SearchPatientAndRegistrationV2View : UserControl, ISearchPatientAndRegistrationView
    {
        public SearchPatientAndRegistrationV2View()
        {
            InitializeComponent();
            authorization();
        }
        public void SetDefaultButton(Button btn)
        {
            var invoker = (TextBoxEnterButtonInvoke)myTrigger.Actions[0];
            invoker.TargetObject = btn;
            //ChangedWPF-CMN: Added TargetName for invoker cause of TargetObject not working
            invoker.TargetName = btn.Name;
        }

        public Button SearchRegistrationButton 
        {
            get { return this.SearchRegistrationCmd; }
        }
        public Button SearchPatientButton
        {
            get { return this.SearchPatientCmd; }
        }
        public Button NewPatientButton
        {
            get { return this.CreateNewPatientCmd; }
        }
        public Button SearchAppointmentsButton
        {
            get { return this.SearchAppointmentCmd; }
        }

        public void InitButtonVisibility(SearchRegButtonsVisibility values)
        {
            NewPatientButton.Visibility = BoolToVisibility((values & SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN) == SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
            SearchPatientButton.Visibility = BoolToVisibility((values & SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN) == SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN);
            SearchRegistrationButton.Visibility = BoolToVisibility((values & SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN) == SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            SearchAppointmentsButton.Visibility = BoolToVisibility((values & SearchRegButtonsVisibility.SHOW_SEARCH_APPOINTMENT) == SearchRegButtonsVisibility.SHOW_SEARCH_APPOINTMENT);
        }

        private Visibility BoolToVisibility(bool visible)
        {
            if (visible)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _BenhNhan_View = true;
        private bool _BenhNhan_Add = true;
        private bool _DangKi_View = true;

        public bool BenhNhan_View
        {
            get
            {
                return _BenhNhan_View;
            }
            set
            {
                if (_BenhNhan_View == value)
                    return;
                _BenhNhan_View = value;
            }
        }
        public bool BenhNhan_Add
        {
            get
            {
                return _BenhNhan_Add;
            }
            set
            {
                if (_BenhNhan_Add == value)
                    return;
                _BenhNhan_Add = value;
            }
        }
        public bool DangKi_View
        {
            get
            {
                return _DangKi_View;
            }
            set
            {
                if (_DangKi_View == value)
                    return;
                _DangKi_View = value;
            }
        }

        #endregion
    }
}