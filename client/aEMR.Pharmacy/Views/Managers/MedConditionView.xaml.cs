using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.Controls;
using System.ComponentModel;
using DataEntities;
namespace aEMR.Pharmacy.Views
{
    public partial class MedConditionView : AxUserControl
    {
        public string pharmaceautical = ""; 
        public decimal drugID;
        public MedConditionView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MedConditionView_Loaded);
            this.Unloaded += new RoutedEventHandler(MedConditionView_Unloaded);
        }

        void MedConditionView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void MedConditionView_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!DesignerProperties.IsInDesignTool)
            //{
            //    //drugmodel = this.DataContext as DrugViewModel;
            //    //if (drugmodel != null)
            //    //{
            //    //    DrugContraindicatorVM.allContrainName.Clear();
            //    //    foreach(string st in drugmodel.allContrainName)
            //    //    {
            //    //        DrugContraindicatorVM.allContrainName.Add(st) ;
            //    //    }
            //    //}
            //}
        }
        
        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            //viet ham add contrain o day
            //foreach (RefMedicalConditionType mc in DrugContraindicatorVM.allNewRefMedicalCondition)
            //{
            //    drugmodel.allNewRefMedicalCondition.Add(mc);                
            //    ContraIndicatorDrugsRelToMedCond cdt = new ContraIndicatorDrugsRelToMedCond();
            //    cdt.RefMedicalConditionType = new RefMedicalConditionType();
            //    cdt.RefMedicalConditionType = mc;
            //    drugmodel.allContraIndicatorDrugsRelToMedCond.Add(cdt);
                
            //}
            //drugmodel.allContrainName.Clear();
            //foreach (string st in DrugContraindicatorVM.allContrainName)
            //{
            //    drugmodel.allContrainName.Add(st);
            //}
            //this.DialogResult = false;            
        }

        private void butExit_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void UCMedConditionGrid_DoubleClick(object sender, EventArgs e)
        {
            //if (!DrugContraindicatorVM.allContrainName.Contains(DrugContraindicatorVM.selectedRefMedicalCondition.MedConditionType))
            //{
            //    DrugContraindicatorVM.allContrainName.Add(DrugContraindicatorVM.selectedRefMedicalCondition.MedConditionType);
            //    DrugContraindicatorVM.allNewRefMedicalCondition.Add(DrugContraindicatorVM.selectedRefMedicalCondition);
            //}
            //else 
            //{
            //    MessageBox.Show("Điều kiện bệnh này đã có trong danh sách chống chỉ định rồi!");
            //}
            
        }

        private void btAddChoose_Click(object sender, RoutedEventArgs e)
        {
            //if (!DrugContraindicatorVM.allContrainName.Contains(DrugContraindicatorVM.selectedRefMedicalCondition.MedConditionType))
            //{
            //    DrugContraindicatorVM.allContrainName.Add(DrugContraindicatorVM.selectedRefMedicalCondition.MedConditionType);
            //    DrugContraindicatorVM.allNewRefMedicalCondition.Add(DrugContraindicatorVM.selectedRefMedicalCondition);
            //}
            //else
            //{
            //    MessageBox.Show("Điều kiện bệnh này đã có trong danh sách chống chỉ định rồi!");
            //}
            
        }

        
    }
}

