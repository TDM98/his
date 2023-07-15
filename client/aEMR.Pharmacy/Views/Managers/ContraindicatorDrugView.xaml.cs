using System;
using System.Windows;
using System.Windows.Controls;
using aEMR.Controls;
using System.ComponentModel;

namespace aEMR.Pharmacy.Views
{
    
    public partial class ContraindicatorDrugView : UserControl
    {

        public ContraindicatorDrugView()
        {
            InitializeComponent();
            
        }

        void ContraindicatorDrugView_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!DesignerProperties.IsInDesignTool)
            //{
            //    //DrugContrainVM.GetContraIndicatorDrugsRelToMedCondPaging(mctypeID);
            //    //DrugContrainVM.GetRefMedCondType();
                
            //}
        }

       public void DrugExVM_InsertConIndicatorDrugsRelToMedCondExCompleted(object sender, EventArgs e)
        {
            //DrugContrainVM.PageIndex = 0;
            //DrugContrainVM.GetContraIndicatorDrugsRelToMedCondPaging(mctypeID);
        }

        public void DrugContrainVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "pageId")
            //{
            //    //DrugContrainVM.GetContraIndicatorDrugsRelToMedCondPaging(mctypeID);
            //}
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            
            //if (DrugContrainVM.selectedRefMedicalConditionType != null)
            //{
            //    if (DrugExVM.MCTypeID > 0)
            //    {
            //        var cw = new AddDrugContrain();
            //        DrugExVM.allNewRefGenericDrugDetail.Clear();
            //        cw.DataContext = DrugExVM;
            //        cw.Show();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Chưa chọn điều kiện bệnh!");
            //        return;

            //    }
            //}
            //else 
            //{
            //    MessageBox.Show("Chưa chọn điều kiện bệnh!");
            //    return;
            
            //}
            
        }

        private void cborefMedType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (((DataEntities.RefMedicalConditionType)(cborefMedType.SelectedItemEx)).MCTypeID >0)
            //{
            //    mctypeID =(int)((DataEntities.RefMedicalConditionType)(cborefMedType.SelectedItemEx)).MCTypeID;
            //    DrugExVM.MCTypeID = mctypeID;
            //    DrugContrainVM.PageIndex = 0;
            //    DrugContrainVM.GetContraIndicatorDrugsRelToMedCondPaging(mctypeID);
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //var cw = new eHCMSApp.Views.Pharmacy.ChildWindows.cwdMedCondition();
            //cw.Show();

        }
    }
}
