using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Common;

namespace aEMR.Registration.Views
{
    //[Export(typeof(IPatientHiManagementView)),PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PatientHiManagementView : UserControl, IPatientHiManagementView
    {
        public PatientHiManagementView()
        {
            InitializeComponent();
            IsEditMode = false;
            //gridHIItems.RemoveEmptyRow += gridHIItems_RemoveEmptyRow;
            this.Loaded += new RoutedEventHandler(PatientHiManagementView_Loaded);
        }

        void PatientHiManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            Binding editModeBinding = new Binding();
            editModeBinding.Mode = BindingMode.OneWay;
            editModeBinding.Path = new PropertyPath("IsEditMode");

            this.SetBinding(PatientHiManagementView.IsEditModeProperty, editModeBinding);
        }

        //void gridHIItems_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var width = e.NewSize.Width - e.PreviousSize.Width;
        //    if (gridHIItems.Columns != null && gridHIItems.Columns.Count > 0)
        //    {
        //        var lastColumn = gridHIItems.Columns[gridHIItems.Columns.Count - 1];
        //        lastColumn.Width = new DataGridLength(lastColumn.Width.Value+width);
        //    }
        //}
        public event EventHandler<EventArgs<object>> RemoveHiItemAtIndex;
        private void gridHIItems_RemoveEmptyRow(int idx)
        {
            if(RemoveHiItemAtIndex != null)
            {
                RemoveHiItemAtIndex(this, new EventArgs<object>(idx));
            }
            //var hiManagementVm = Globals.GetViewModel<IPatientHiManagement>();
            //hiManagementVm.HealthInsurances.RemoveAt(idx);
        }
        
        public void CommitEditingHiGridIfValid()
        {
            //if (gridHIItems.IsEditing)
            //{
            //    gridHIItems.CommitEdit();
            //    if (!gridHIItems.IsValid)
            //    {
            //        bool bOK = gridHIItems.CancelEdit();
            //        gridHIItems.Cancel();
            //    }
            //}
        }


        public void BeginEditingOnGrid()
        {
            int editingIdx = gridHIItems.EditingIndex;
            if (editingIdx != gridHIItems.SelectedIndex)
            {
                gridHIItems.CancelEdit();
            }
            gridHIItems.EnterEdit(gridHIItems.SelectedIndex, true);
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
            typeof(PatientHiManagementView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnIsEditModeChanged)));

        private static void OnIsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool inEditMode = (bool)e.NewValue;
            var me = ((PatientHiManagementView)d);
            Visibility editCtrlVisibility, viewCtrlVisibility;
            if (inEditMode)
            {
                editCtrlVisibility = Visibility.Visible;
                viewCtrlVisibility = Visibility.Collapsed;
            }
            else
            {
                editCtrlVisibility = Visibility.Collapsed;
                viewCtrlVisibility = Visibility.Visible;
            }

            me.lblCardType.Visibility = viewCtrlVisibility;
            me.lblHICardNo.Visibility = viewCtrlVisibility;
            me.lblRegistrationCode.Visibility = viewCtrlVisibility;
            me.lblRegistrationLocation.Visibility = viewCtrlVisibility;
            me.lblValidDateFrom.Visibility = viewCtrlVisibility;
            me.lblValidDateTo.Visibility = viewCtrlVisibility;
            //me.lblHIPatientBenefit.Visibility = viewCtrlVisibility;
            me.rdoActiveView.Visibility = viewCtrlVisibility;

            me.cboCardType.Visibility = editCtrlVisibility;
            me.txtHICardNo.Visibility = editCtrlVisibility;
            me.txtRegistrationCode.Visibility = editCtrlVisibility;
            me.HospitalAutoCompleteContent.Visibility = editCtrlVisibility;
            //me.txtRegistrationLocation.Visibility = editCtrlVisibility;
            me.txtValidDateFrom.Visibility = editCtrlVisibility;
            //me.dtpValidDateTo.Visibility = editCtrlVisibility;
            me.txtValidDateTo.Visibility = editCtrlVisibility;
            //me.txtHIPatientBenefit.Visibility = editCtrlVisibility;
            me.rdoActiveEdit.Visibility = editCtrlVisibility;
        }

    }
}
