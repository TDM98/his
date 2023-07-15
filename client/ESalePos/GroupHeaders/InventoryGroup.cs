using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aEMR.Infrastructure;
using EPos.ViewContracts.Commons;
using EPos.ViewContracts.InventoryViewContracts;
using EPos.ViewContracts.PosViewContracts;
using EPos.ViewContracts.UserManageViewContracts;
using EPos.ViewContracts.SettingViewContracts;
using EPos.ViewContracts;

namespace ESalePos.GroupHeaders
{
    public static class InventoryGroup
    {
        public static PanelItem[] Groups = new[]
                                               {
                                                   new PanelItem
                                                    {
                                                        Text = "Invoices",
                                                        ToolTip = "Invoices",
                                                        ImgSrc =
                                                            "/EPos.Resources;component/Assets/Images/Invoice.png"
                                                        , ViewModelType = typeof(IListInvoiceViewModel)
                                                    }, 
                                                   
                                                       new PanelItem
                                                       {
                                                           Text = "Inward Goods Muti",
                                                           ToolTip = "Inward Goods Muti",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Toolbars/Pos.png",
                                                           ViewModelType = typeof(IInwardGoodMutiViewModel)
                                                       },
                                                           new PanelItem
                                                       {
                                                           Text = "Inward Goods Admin",
                                                           ToolTip = "Inward Goods Admin",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Toolbars/Pos.png",
                                                           ViewModelType = typeof(IListInwardGoodAdminViewModel)
                                                       },
                                                        
                                                   new PanelItem
                                                       {
                                                           Text = "Outward Goods",
                                                           ToolTip = "Outward Goods",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Outward.jpg",
                                                               ViewModelType = typeof(IListOutwardGoodViewModel)
                                                       },
                                                       new PanelItem
                                                       {
                                                           Text = "Admin Outward Goods",
                                                           ToolTip = "Admin Outward Goods",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/OutwardMulti.jpg",
                                                               ViewModelType = typeof(IListOutwardGoodAdminViewModel)
                                                       },
                                                   
                                                       new PanelItem
                                                       {
                                                           Text = "Products",
                                                           ToolTip = "Manage Products",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Clothes.png",
                                                               ViewModelType = typeof(IHomeInventoryViewModel)
                                                       },
                                                        new PanelItem
                                                       {
                                                           Text = "Images Management",
                                                           ToolTip = "Import new photo, assign image to item.",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/img2icns_icon.png",
                                                               ViewModelType = typeof(IHomeImageViewModel)
                                                       }, 

                                                         new PanelItem
                                                       {
                                                           Text = "SaleGroups",
                                                           ToolTip = "SaleGroups",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Toolbars/dollar.png",
                                                               ViewModelType = typeof(IListSaleGroupViewModel)
                                                       },  
                                                           new PanelItem
                                                       {
                                                           Text = "Inventory Admin",
                                                           ToolTip = "Inventory Admin",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Inventory.jpg",
                                                               ViewModelType = typeof(IInventoryAdminLayoutViewModel)
                                                       },  
                                               };


    }

    public static class InwardGroup
    {
        public static PanelItem[] Groups = new[]
                                               {  
                                                   new PanelItem
                                                       {

                                                           Text = "Inward Goods",
                                                           ToolTip = "Inward Goods",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Inward.jpg",
                                                           ViewModelType = typeof(IListInwardGoodViewModel)
                                                       },      
                                               };


    }

    public static class UserManageGroup
    {
        public static PanelItem[] Groups = new[]
                                               {
                                                  new PanelItem
                                                       {
                                                           Text = "User Managerment",
                                                           ToolTip = "User Manage",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/user.jpg",
                                                               ViewModelType = typeof(IHomeUserManageViewModel)
                                                       },                                                       
                                               };


    }
    public static class CommonGroup
    {
        public static PanelItem[] Groups = new[]
                                               {
                                                   new PanelItem
                                                       {
                                                           Text = "Inventory",
                                                           ToolTip = "Inventory",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Inventory.jpg",
                                                           ViewModelType = typeof(IInventoryLayoutViewModel)
                                                       },  
                                                        new PanelItem
                                                       {
                                                           Text = "Search Inventory",
                                                           ToolTip = "Search Inventory",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Inventory.jpg",
                                                           ViewModelType = typeof(ISearchInventoryViewModel)
                                                       },      
                                                       new PanelItem
                                                    {
                                                        Text = "Vouchers/Credit notes",
                                                        ToolTip = "Reports",
                                                        ImgSrc =
                                                            "/EPos.Resources;component/Assets/Images/Voucher.png",
                                                            ViewModelType = typeof(IRefundListViewModel)
                                                    },  
                                                    new PanelItem
                                                       {
                                                           Text = "Clients",
                                                           ToolTip = "Clients",
                                                           ImgSrc =
                                                               "/EPos.Resources;component/Assets/Images/Toolbars/Pos.png"
                                                       },
                                                       new PanelItem
                                                    {
                                                        Text = "User Profile",
                                                        ToolTip = "User Profile",
                                                        ImgSrc =
                                                            "/EPos.Resources;component/Assets/Images/users.png",
                                                            ViewModelType = typeof(IUserInfoViewModel)
                                                    },  
                                               };
                                              


    }
}
