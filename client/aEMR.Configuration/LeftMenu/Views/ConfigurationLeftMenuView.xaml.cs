using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ViewContracts;

namespace aEMR.Configuration.LeftMenu.Views
{
     [Export(typeof(ConfigurationLeftMenuView))]
    public partial class ConfigurationLeftMenuView : ILeftMenuView
    {
        public ConfigurationLeftMenuView()
        {
            InitializeComponent();
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            RefDepartments_Mgnt.Style = defaultStyle;
            RefMedicalServiceItems_Mgnt.Style = defaultStyle;
            RefMedicalServiceTypes_Mgnt.Style = defaultStyle;
            Locations_Mgnt.Style = defaultStyle;
            RoomType_Mgnt.Style = defaultStyle;
            DeptLocMedServices_Mgnt.Style = defaultStyle;
            PCLForms_Mgnt.Style = defaultStyle;

            PCLExamTypeCombo_Mgnt.Style = defaultStyle;
            PCLExamTypeExamTestPrintIndexMgnt.Style = defaultStyle;
            PCLExamTypeServiceTarget_Mgnt.Style = defaultStyle;

            RefMedicalServiceItems_IsPCL_Mgnt.Style = defaultStyle;
            PCLExamType_Mgnt.Style = defaultStyle;
            PCLExamTypeLocationsCmd.Style = defaultStyle;
            PCLSections_Mgnt.Style = defaultStyle;
            PCLExamTypeExamTestPrintMgnt.Style = defaultStyle;
            BedAlloc_Mgnt.Style = defaultStyle;

            Encrypt_Mgnt.Style = defaultStyle;
            //PatientApptLocTargetsClick.Style = defaultStyle;
            MedServiceItemPriceList_Mgnt.Style = defaultStyle;
            PCLExamTypePriceList_Mgnt.Style = defaultStyle;
            NoteTemplate_Mgnt.Style = defaultStyle;
            PCLAgency_Mgnt.Style = defaultStyle;

        }
    }
}
