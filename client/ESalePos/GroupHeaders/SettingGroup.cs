using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aEMR.Infrastructure;
using EPos.ViewContracts.PosViewContracts;
using EPos.ViewContracts.SeedDataViewContracts;
using EPos.ViewContracts.SettingViewContracts;
using EPos.ViewContracts.Reports;
using EPos.ViewContracts;

namespace ESalePos.GroupHeaders
{
    public static class SettingGroup
    {
        public static PanelItem[] Groups = new[]
                                               {
                                                   
                                                   //new PanelItem
                                                   //    {
                                                   //        Text = "Suppliers",
                                                   //        ToolTip = "Suppliers",
                                                   //        ImgSrc =
                                                   //            "/EPos.Resources;component/Assets/Images/Toolbars/dollar.png",
                                                   //            ViewModelType = typeof(IListSupplierViewModel)
                                                   //    },  
                                                   //    new PanelItem
                                                   //    {
                                                   //        Text = "Categories",
                                                   //        ToolTip = "Categories",
                                                   //        ImgSrc =
                                                   //            "/EPos.Resources;component/Assets/Images/Toolbars/dollar.png",
                                                   //            ViewModelType = typeof(IListCategoryViewModel)
                                                   //    },  
                                                   //    new PanelItem
                                                   //    {
                                                   //        Text = "Warehouses",
                                                   //        ToolTip = "Warehouses",
                                                   //        ImgSrc =
                                                   //            "/EPos.Resources;component/Assets/Images/Toolbars/dollar.png",
                                                   //            ViewModelType = typeof(IListWarehouseViewModel)
                                                   //    }, 
                                                       
                                                       new PanelItem
                                                       {
                                                           Text = "Settings",
                                                           ToolTip = "Settings",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Settings.png",
                                                               ViewModelType = typeof(IHomeSettingViewModel)
                                                       },  

                                                       
                                               };


    }

    public static class ReportGroup
    {
        public static PanelItem[] Groups = new[]
                                               {       new PanelItem
                                                       {
                                                           Text = "Reports",
                                                           ToolTip = "Reports",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/report_hot.png",
                                                               ViewModelType = typeof(IReportLayoutViewModel)
                                                       },  

                                                       
                                               };


    }
}
