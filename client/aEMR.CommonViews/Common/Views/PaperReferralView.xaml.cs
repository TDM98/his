using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace aEMR.Common.Views
{
    public partial class PaperReferralView : UserControl
    {
        public PaperReferralView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PaperReferralView_Loaded);
        }

        void PaperReferralView_Loaded(object sender, RoutedEventArgs e)
        {
            Binding IsAddNewModeBinding = new Binding();
            IsAddNewModeBinding.Mode = BindingMode.OneWay;
            IsAddNewModeBinding.Path = new PropertyPath("IsAddNewMode");
            this.SetBinding(PaperReferralView.IsAddNewModeProperty, IsAddNewModeBinding);

            Binding editChronicModeBinding = new Binding();
            editChronicModeBinding.Mode = BindingMode.OneWay;
            editChronicModeBinding.Path = new PropertyPath("IsChronicEditMode");
            this.SetBinding(PaperReferralView.IsChronicEditModeProperty, editChronicModeBinding);

            Binding editModeBinding = new Binding();
            editModeBinding.Mode = BindingMode.OneWay;
            editModeBinding.Path = new PropertyPath("IsEditMode");
            this.SetBinding(PaperReferralView.IsEditModeProperty, editModeBinding);
        }

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set
            {
                SetValue(IsEditModeProperty, value);
            }
        }

        public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(
            "IsEditMode",
            typeof(bool),
            typeof(PaperReferralView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnIsEditModeChanged)));

        private static void OnIsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool inEditMode = (bool)e.NewValue;
            var me = ((PaperReferralView)d);

            me.HospitalAutoCompleteContent.IsEnabled = inEditMode;
            me.txtCreatedDate.IsEnabled = inEditMode;
            me.txtAcceptDate.IsEnabled = inEditMode;
            me.chkSearchAll.IsEnabled = inEditMode;
            //me.chkActive.IsEnabled = inEditMode;
            me.chkChronic.IsEnabled = false;
            me.txtNotes.IsReadOnly = !inEditMode ;
        }



        public bool IsChronicEditMode
        {
            get { return (bool)GetValue(IsChronicEditModeProperty); }
            set
            {
                SetValue(IsChronicEditModeProperty, value);
            }
        }
        public static readonly DependencyProperty IsChronicEditModeProperty = DependencyProperty.Register(
            "IsChronicEditMode",
            typeof(bool),
            typeof(PaperReferralView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnIsChronicEditModeChanged)));

        private static void OnIsChronicEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool inEditMode = (bool)e.NewValue;
            var me = ((PaperReferralView)d);

            me.HospitalAutoCompleteContent.IsEnabled = false;
            me.chkSearchAll.IsEnabled = false;
            me.txtCreatedDate.IsEnabled = false;
            me.txtAcceptDate.IsEnabled = inEditMode;
            me.chkChronic.IsEnabled = inEditMode;
            me.txtNotes.IsReadOnly = !inEditMode ;
        }


        public bool IsAddNewMode
        {
            get { return (bool)GetValue(IsAddNewModeProperty); }
            set
            {
                SetValue(IsAddNewModeProperty, value);
            }
        }
        public static readonly DependencyProperty IsAddNewModeProperty = DependencyProperty.Register(
            "IsAddNewMode",
            typeof(bool),
            typeof(PaperReferralView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnIsAddNewModeChanged)));

        private static void OnIsAddNewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool inEditMode = (bool)e.NewValue;
            var me = ((PaperReferralView)d);

            me.HospitalAutoCompleteContent.IsEnabled = inEditMode;
            me.chkSearchAll.IsEnabled = inEditMode;
            me.txtCreatedDate.IsEnabled = inEditMode;
            me.txtAcceptDate.IsEnabled = inEditMode;
            me.chkChronic.IsEnabled = false;
            me.txtNotes.IsReadOnly = !inEditMode;
        }
    }
}