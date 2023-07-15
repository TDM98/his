using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;
using aEMR.Controls;
using aEMR.Common;

namespace aEMR.Common.Views
{
    public partial class cwdBedPatientView : UserControl
    {
        public cwdBedPatientView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(cwdBedPatientView_Loaded);
            this.Unloaded += new RoutedEventHandler(cwdBedPatientView_Unloaded);
        }

        void cwdBedPatientView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void cwdBedPatientView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            //if (BedPatientAllocVM.selectedBedPatientAllocs.AdmissionDate == null)
            //{
            //    BedPatientAllocVM.selectedBedPatientAllocs.AdmissionDate = DateTime.Now;
            //}
            //DateTime dateTime = (DateTime)BedPatientAllocVM.selectedBedPatientAllocs.AdmissionDate;
            
            //DateTime dT = new System.DateTime(dateTime.Year
            //                            , dateTime.Month
            //                            , dateTime.Day + BedPatientAllocVM.selectedBedPatientAllocs.ExpectedStayingDays
            //                            );
            //if (BedPatientAllocVM.selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID < 1
            //    || BedPatientAllocVM.selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID == null)
            //{
            //    BedPatientAllocVM.selectedBedPatientAllocs.PtRegistrationID = 1;
            //}
            //else 
            //{
            //    BedPatientAllocVM.selectedBedPatientAllocs.PtRegistrationID = BedPatientAllocVM.selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID;            
            //}
            //BedPatientAllocVM.AddNewBedPatientAllocs(BedPatientAllocVM.selectedBedPatientAllocs.BedAllocationID
            //    , BedPatientAllocVM.selectedBedPatientAllocs.PtRegistrationID
            //    , BedPatientAllocVM.selectedBedPatientAllocs.AdmissionDate
            //    , BedPatientAllocVM.selectedBedPatientAllocs.ExpectedStayingDays
            //    , dT
            //    , true);
            //this.DialogResult = true;
        }

        private void butExit_Click(object sender, RoutedEventArgs e)
        {
            
        }
                
        private void butDelete_Click(object sender, RoutedEventArgs e)
        {
            //DeleteBedPatientAllocs(BedPatientAllocVM.selectedBedPatientAllocs.BedPatientID);
            
        }

        
    }
}

