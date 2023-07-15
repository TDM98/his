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
using aEMR.ViewContracts;

namespace aEMR.DrugDept.Views
{
    public partial class DrugLeftMenuView : ILeftMenuView
    {
        public DrugLeftMenuView()
        {
            InitializeComponent();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            RequestNewThuocCmd.Style = defaultStyle;
            RequestNewYCuCmd.Style = defaultStyle;
            RequestNewHoaChatCmd.Style = defaultStyle;

            //RequestThuocCmd.Style = defaultStyle;            
            //RequestYCuCmd.Style = defaultStyle;
            //RequestHoaChatCmd.Style = defaultStyle;

            XNBThuocCmd.Style = defaultStyle;
            XNBYCuCmd.Style = defaultStyle;
            XNBHoaChatCmd.Style = defaultStyle;
            XuatThuocTheoToaCmd.Style = defaultStyle;
            XNBThuocHangKyGoiCmd.Style = defaultStyle;
            XNBYCuHangKyGoiCmd.Style = defaultStyle;
            XNBHoaChatHangKyGoiCmd.Style = defaultStyle;
            JoinThuocHangKyGoiCmd.Style = defaultStyle;
            JoinYCuHangKyGoiCmd.Style = defaultStyle;
            JoinHoaChatHangKyGoiCmd.Style = defaultStyle;
            ReturnThuocCmd.Style = defaultStyle;
            ReturnYCuCmd.Style = defaultStyle;
            ReturnHoaChatCmd.Style = defaultStyle;
            //DemageThuoc.Style = defaultStyle;
            //DemageYCu.Style = defaultStyle;
            //DemageHoaChat.Style = defaultStyle;
            EstimationDrugDeptCmd.Style = defaultStyle;
            EstimationYCuCmd.Style = defaultStyle;
            EstimationChemicalCmd.Style = defaultStyle;
            OrderThuocCmd.Style = defaultStyle;
            OrderYCuCmd.Style = defaultStyle;
            OrderHoaChatCmd.Style = defaultStyle;
            InwardDrugFromSupplierDrugCmd.Style = defaultStyle;
            InwardDrugFromSupplierMedicalDeviceClinicCmd.Style = defaultStyle;
            InwardDrugFromSupplierChemicalCmd.Style = defaultStyle;
            InwardDrugFromSupplierDrugClinicCmd.Style = defaultStyle;
            InwardDrugFromSupplierMedicalDeviceClinicCmd.Style = defaultStyle;
            InwardDrugFromSupplierChemicalClinicCmd.Style = defaultStyle;
            CostThuocCmd.Style = defaultStyle;
            CostYCuCmd.Style = defaultStyle;
            CostHoaChatCmd.Style = defaultStyle;
            SuggestThuocCmd.Style = defaultStyle;
            SuggestYCuCmd.Style = defaultStyle;
            SuggestHoaChatCmd.Style = defaultStyle;
            KKThuocCmd.Style = defaultStyle;
            KKYCuCmd.Style = defaultStyle;
            KKHoaChatCmd.Style = defaultStyle;
            XuatThuocCmd.Style = defaultStyle;
            XuatYCuCmd.Style = defaultStyle;
            XuatHoaChatCmd.Style = defaultStyle;
            WatchMedCmd.Style = defaultStyle;
            WatchMatCmd.Style = defaultStyle;
            WatchLabCmd.Style = defaultStyle;
            NhapXuatTonThuocCmd.Style = defaultStyle;
            NhapXuatTonYCuCmd.Style = defaultStyle;
            NhapXuatTonHoaChatCmd.Style = defaultStyle;
            TheKhoThuocCmd.Style = defaultStyle;
            TheKhoYCuCmd.Style = defaultStyle;
            TheKhoHoaChatCmd.Style = defaultStyle;
            BangKeNhapHangThangTheoSoPhieuNhapKhoThuocCmd.Style = defaultStyle;
            BangKeNhapHangThangTheoSoPhieuNhapKhoYCuCmd.Style = defaultStyle;
            BangKeNhapHangThangTheoSoPhieuNhapKhoHoaChatCmd.Style = defaultStyle;
            SuDungThuocCmd.Style = defaultStyle;
            ThuocXuatDenKhoaCmd.Style = defaultStyle;
            YCuXuatDenKhoaCmd.Style = defaultStyle;
            HoaChatXuatDenKhoaCmd.Style = defaultStyle;
            TheoDoiCongNoThuocCmd.Style = defaultStyle;
            TheoDoiCongNoYCuCmd.Style = defaultStyle;
            TheoDoiCongNoHoaChatCmd.Style = defaultStyle;
            SupplierProductCmd.Style = defaultStyle;
            SupplierAndProductCmd.Style = defaultStyle;
            DrugDeptPharmaceulCompanyCmd.Style = defaultStyle;
            DrugDeptPharmaceulCompanySupplierCmd.Style = defaultStyle;
            RefGenMedProductDetails_DrugMgnt.Style = defaultStyle;
            RefGenMedProductDetails_MedicalDevicesMgnt.Style = defaultStyle;
            RefGenMedProductDetails_ChemicalMgnt.Style = defaultStyle;
            SupplierGenMedProductsPrice_Mgnt.Style = defaultStyle;
            SupplierGenMedProductsPrice_Medical_Mgnt.Style = defaultStyle;
            SupplierGenMedProductsPrice_Chemical_Mgnt.Style = defaultStyle;
            DrugDeptSellPriceProfitScale_Mgnt.Style = defaultStyle;
            DrugDeptSellPriceProfitScale_Medical_Mgnt.Style = defaultStyle;
            DrugDeptSellPriceProfitScale_Chemical_Mgnt.Style = defaultStyle;
            DrugDeptSellingItemPrices_Mgnt.Style = defaultStyle;
            DrugDeptSellingItemPrices_Medical_Mgnt.Style = defaultStyle;
            DrugDeptSellingItemPrices_Chemical_Mgnt.Style = defaultStyle;
            DrugDeptSellingPriceList_Mgnt.Style = defaultStyle;
            DrugDeptSellingPriceList_Medical_Mgnt.Style = defaultStyle;
            DrugDeptSellingPriceList_Chemical_Mgnt.Style = defaultStyle;

            UnitCmd.Style = defaultStyle;
            DrugClass_DeptCmd.Style = defaultStyle;
            DrugClass_MedicalCmd.Style = defaultStyle;
            DrugClass_ChemicalCmd.Style = defaultStyle;
            InStock_MedCmd.Style = defaultStyle;
            InStock_MatCmd.Style = defaultStyle;
            InStock_LabCmd.Style = defaultStyle;

            AdjustOutPrice_MedCmd.Style = defaultStyle;
            AdjustOutPrice_MatCmd.Style = defaultStyle;
            AdjustOutPrice_LabCmd.Style = defaultStyle;

            DTNhapXuatTonThuocCmd.Style = defaultStyle;
            DTNhapXuatTonYCuCmd.Style = defaultStyle;
            DTNhapXuatTonHoaChatCmd.Style = defaultStyle;

            MedBidDetailCmd.Style = defaultStyle;
            MatBidDetailCmd.Style = defaultStyle;
            LabBidDetailCmd.Style = defaultStyle;

            Clinic_AdjustOutPrice_MedCmd.Style = defaultStyle;
            Clinic_AdjustOutPrice_MatCmd.Style = defaultStyle;
            Clinic_AdjustOutPrice_LabCmd.Style = defaultStyle;

            /*TMA 24/10/2017*/
            NhapThuocCmd.Style = defaultStyle;
            NhapYCuCmd.Style = defaultStyle;
            NhapHoaChatCmd.Style = defaultStyle;
            /*TMA 24/10/2017*/

            /*▼====: #003*/
            TempInwardMedReportCmd.Style = defaultStyle;
            TempInwardMatReportCmd.Style = defaultStyle;
            TempInwardLabReportCmd.Style = defaultStyle;
            /*▲====: #003*/
        }
    }
}
