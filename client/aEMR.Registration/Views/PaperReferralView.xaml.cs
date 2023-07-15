using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.Registration.Views
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
            me.chkActive.IsEnabled = inEditMode;
        }
    }
}
