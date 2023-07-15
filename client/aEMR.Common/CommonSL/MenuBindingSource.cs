using eHCMSLanguage;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace aEMR.Common.BindingSource
{
    public class MenuBindingSource : INotifyPropertyChanged
    {
        private int _Item = 0;
        public int Item
        {
            get
            {
                return _Item;
            }
        }
        private static int _ModuleCount =12;
        public static int ModuleCount
        {
            get
            {
                return _ModuleCount;
            }
        }
        private eModules _eItem;
        public eModules eItem
        {
            get
            {
                return _eItem;
            }
        }
        private int _eFunction;
        public int eFunction
        {
            get
            {
                return _eFunction;
            }
        }

        private Guid _ID;
        public Guid ID
        {
            get
            {
                return _ID;
            }
        }

        public string ItemName
        {
            get
            {
                switch (_eItem)
                {
                    case eModules.mHome:
                        return "Trang Chủ";
                    case eModules.mPatient:
                        return eHCMSResources.K2848_G1_DK;
                    case eModules.mConsultation:
                        return eHCMSResources.T2119_G1_KB;
                    case eModules.mParaClinical:
                        return eHCMSResources.K1549_G1_CLS;
                    case eModules.mPharmacy:
                        return eHCMSResources.N0181_G1_NhaThuoc;
                    case eModules.mTransaction_Management:
                        return eHCMSResources.K1048_G1_BC;
                    case eModules.mConfiguration_Management:
                        return eHCMSResources.K1691_G1_CHinh;
                    case eModules.mSystem_Management:
                        return eHCMSResources.T1452_G1_HeThong;
                    case eModules.mResources:
                        return eHCMSResources.G2177_G1_VT;
                    case eModules.mResources_Maintenance:
                        return eHCMSResources.K1134_G1_BTriVT;
                    case eModules.mAppointment_System:
                        return eHCMSResources.T1455_G1_HBenh;
                    case eModules.mUserAccount:
                        return "Quản Lý Người Dùng";
                    case eModules.mKhoaDuoc:
                        return eHCMSResources.T2257_G1_KhoaDuoc;
                    default:
                        return "";
                }
            }
        }

        public string FunctionName
        {
            get
            {
                string st = "";
                switch (_eItem)
                {
                    case eModules.mHome:
                        return "Trang Chủ";
                    case eModules.mPatient:

                        switch (_eFunction)
                        {
                            //case (int)ePatient.mPatientRegistration:
                            //    st = eHCMSResources.K2851_G1_DKBN;
                            //    break;
                            //case (int)ePatient.mPatientManagement:
                            //    st = "Danh Sách Bệnh Nhân";
                            //    break;
                            //case (int)ePatient.mRegistrationListing:
                            //    st = "Danh Sách Đăng Ký";
                            //    break;
                            case (int)ePatient.mReceivePatient:
                                st = "Nhận Bệnh";
                                break;
                            //case (int)ePatient.mEmergencyRegistration:
                            //    st = "Khẩn Cấp";
                            //    break;

                            //case (int)ePatient.eInPatientRegistration:
                            //    st = "Đăng Ký Nội Trú";
                            //    break;

                            //case (int)ePatient.mPatientHospitalization:
                            //    st = eHCMSResources.N0221_G1_NhapVien;
                            //    break;
                        }
                        return st;
                    case eModules.mConsultation:

                        switch (_eFunction)
                        {
                            case (int)eConsultation.mPtDashboardSummary:
                                st = eHCMSResources.G0574_G1_TTinChung; break;
                            case (int)eConsultation.mPtDashboardCommonRecs:
                                st = eHCMSResources.G1527_G1_TQuat; break;
                            case (int)eConsultation.mPtPMRConsultationNew:
                                st = eHCMSResources.T2119_G1_KB; break;
                            //case (int)eConsultation.mPtPCLRequest:
                            //    st = eHCMSResources.P0381_G1_PhYeuCauCLS; break;
                            //case (int)eConsultation.mPtPCLResults:
                            //    st = eHCMSResources.T2074_G1_KQuaHA; break;
                            //case (int)eConsultation.mPtPCLLabResults:
                            //    st = eHCMSResources.T2081_G1_KQuaXN; break;
                            case (int)eConsultation.mPtePrescriptionTab:
                                st = eHCMSResources.G1434_G1_ToaThuoc; break;
                            case (int)eConsultation.mPteAppointmentTab:
                                st = eHCMSResources.T1455_G1_HBenh; break;
                                
                        }
                        return st;
                    case eModules.mParaClinical:
                        switch (_eFunction)
                        {
                            case (int)eParaClinical.mPtPCLImport:
                                st = eHCMSResources.T2074_G1_KQuaHA; break;
                            case (int)eParaClinical.mPtPCLLabTest:
                                st = eHCMSResources.T2081_G1_KQuaXN; break;
                        }
                        return st;
                    case eModules.mPharmacy:
                        switch (_eFunction)
                        {
                            case (int)ePharmacy.mSellByPrescription:
                                st = "Xuất bán theo toa"; break;
                            //case (int)ePharmacy.mSellByVisitors:
                            //    st = "Xuất bán lẻ"; break;
                            //case (int)ePharmacy.mReturnOutwardDrug:
                            //    st = "Trả thuốc"; break;
                            //case (int)ePharmacy.mEstimatePharmacy:
                            //    st = "Dự trù thuốc"; break;
                            //case (int)ePharmacy.mInwardDrugSuppliers:
                            //    st = "Từ NCC "; break;
                            //case (int)ePharmacy.mListDrugManagement:
                            //    st = "DM thuốc của Nhà Thuốc "; break;
                            //case (int)ePharmacy.mListDrugMedDeptManagement:
                            //    st = "DM thuốc của Khoa Dược"; break;
                            //case (int)ePharmacy.mFamilyTherapyMgnt:
                            //    st = "Danh mục họ thuốc"; break;
                            //case (int)ePharmacy.mListSuppliersMgnt:
                            //    st = "Danh mục nhà cung cấp"; break;
                            //case (int)ePharmacy.mListWardHouseManagement:
                            //    st = "Danh mục kho"; break;
                            //case (int)ePharmacy.mListUnitMgnt:
                            //    st = "Danh mục ĐVT"; break;
                            //case (int)ePharmacy.mFomularPrice:
                            //    st = "Công thức giá"; break;
                            //case (int)ePharmacy.mContraindicatorDrugEx:
                            //    st = eHCMSResources.K2218_G1_CCD; break;

                            //case (int)ePharmacy.mNhapXuatTonPage:
                            //    st = eHCMSResources.N0223_G1_NXT; break;
                            //case (int)ePharmacy.mCardStoragePage:
                            //    st = "Thẻ kho"; break;
                            //case (int)ePharmacy.mNhapXuatTonTheoTungKho:
                            //    st = "Nhập Xuất Tồn Theo Từng Kho"; break;
                            //case (int)ePharmacy.mDrugExpiryDate:
                            //    st = "Thuốc hết hạn dùng"; break;


                        }
                        return st;
                    //case eModules.mKhoaDuoc:
                        //{
                            //switch (_eFunction)
                            //{
                                //case (int)eKhoaDuoc.mDanhMucThuoc:
                                //    {
                                //        st = eHCMSResources.K2906_G1_DMucThuoc;
                                //        break;
                                //    }
                                //case (int)eKhoaDuoc.mDuTruThuoc:
                                //    st = eHCMSResources.K3922_G1_DuTruThuoc; break;
                                //case (int)eKhoaDuoc.mDuTruYCu:
                                //    st = "Dự Trù Y Cụ"; break;
                                //case (int)eKhoaDuoc.mDuTruHoaChat:
                                //    st = "Dự Trù Hóa Chất"; break;

                                //case (int)eKhoaDuoc.mDatHangThuoc:
                                //    st = "Đặt Hàng Thuốc"; break;
                                //case (int)eKhoaDuoc.mDatHangYCu:
                                //    st = "Đặt Hàng Y Cụ"; break;
                                //case (int)eKhoaDuoc.mDatHangHoaChat:
                                //    st = "Đặt Hàng Hóa Chất"; break;

                                //case (int)eKhoaDuoc.mNhapThuoc:
                                //    st = "Thuốc Khoa Dược"; break;
                                //case (int)eKhoaDuoc.mNhapYCu:
                                //    st = "Y Cụ Khoa Dược"; break;
                                //case (int)eKhoaDuoc.mNhapHoaChat:
                                //    st = "Hóa Chất Khoa Dược"; break;

                                //case (int)eKhoaDuoc.mNhapThuocKP:
                                //    st = "Thuốc Khoa Phòng"; break;
                                //case (int)eKhoaDuoc.mNhapYCuKP:
                                //    st = "Y Cụ Khoa Phòng"; break;
                                //case (int)eKhoaDuoc.mNhapHoaChatKP:
                                //    st = "Hóa Chất Khoa Phòng"; break;

                                //case (int)eKhoaDuoc.mRequestThuoc:
                                //    st = "Yêu Cầu Thuốc"; break;
                                //case (int)eKhoaDuoc.mRequestYCu:
                                //    st = "Yêu Cầu Y Cụ"; break;
                                //case (int)eKhoaDuoc.mRequestHoaChat:
                                //    st = "Yêu Cầu Hóa Chất"; break;

                                //case (int)eKhoaDuoc.mXNBThuoc:
                                //    st = "XNB Thuốc"; break;
                                //case (int)eKhoaDuoc.mXNBYCu:
                                //    st = "XNB Y Cụ"; break;
                                //case (int)eKhoaDuoc.mXNBHoaChat:
                                //    st = "XNB Hóa Chất"; break;
                                //case (int)eKhoaDuoc.mDanhMucHoaChat:
                                //    {
                                //        st = eHCMSResources.K2895_G1_DMucHChat;
                                //        break;
                                //    }
                                //case (int)eKhoaDuoc.mDanhMucYCu:
                                //    {
                                //        st = eHCMSResources.K2917_G1_DMucYCu;
                                //        break;
                                //    }
                                    
                                ////Nhật Xuất Nội Bộ
                                //case (int)eKhoaDuoc.mNhapThuoc_NoiBo:
                                //    st = "Nhập Thuốc Nội Bộ"; break;
                                //case (int)eKhoaDuoc.mNhapYCu_NoiBo:
                                //    st = "Nhập Y Cụ Nội Bộ"; break;
                                //case (int)eKhoaDuoc.mNhapHoaChat_NoiBo:
                                //    st = "Nhập Hóa Chất Nội Bộ"; break;

                                //case (int)eKhoaDuoc.mXuatThuoc_NoiBo:
                                //    st = "Xuất Thuốc Nội Bộ"; break;
                                //case (int)eKhoaDuoc.mXuatYCu_NoiBo:
                                //    st = "Xuất Y Cụ Nội Bộ"; break;
                                //case (int)eKhoaDuoc.mXuatHoaChat_NoiBo:
                                //    st = "Xuất Hóa Chất Nội Bộ"; break;
                                ////Nhật Xuất Nội Bộ
                            //}
                        //}
                        //return st;
                    case eModules.mTransaction_Management:
                        switch (_eFunction)
                        {
                            case (int)eTransaction_Management.mTemp25aInsurance:
                                st = eHCMSResources.T3728_G1_Mau25A; break;
                            case (int)eTransaction_Management.mTemplate38:
                                st = "Mẫu 38 Ngoại Trú"; break;
                            case (int)eTransaction_Management.mThongKeDoanhThu:
                                st = eHCMSResources.G0454_G1_TKeDThu; break;
                        }
                        return st;
                    //case eModules.mConfiguration_Management:
                        //switch (_eFunction)
                        //{
                            //case (int)eConfiguration_Management.mRefDepartmentsMgnt:
                            //    st = eHCMSResources.T2222_G1_Khoa; break;
                            //case (int)eConfiguration_Management.mPCLExamTypeMedServiceDefItemsMgnt:
                            //    st = eHCMSResources.K1699_G1_CHinhGoiDVPCL; break;
                            //case (int)eConfiguration_Management.mPCLItemsSectionsForms:
                            //    st = "PCL-ItemsSectionsForms"; break;
                            //case (int)eConfiguration_Management.mDeptLocMedServices:
                            //    st = "DeptLocMedServices"; break;
                            //case (int)eConfiguration_Management.mRefMedicalServiceTypes:
                            //    st = eHCMSResources.T2707_G1_LoaiDV; break;

                            //case (int)eConfiguration_Management.mRefMedicalServiceItemsMgnt:
                            //    st = "Dịch Vụ - Giá Dịch Vụ"; break;

                            //case (int)eConfiguration_Management.mListLocations1:
                            //    st = eHCMSResources.P0385_G1_Pg; break;
                            //case (int)eConfiguration_Management.mListLocations2:
                            //    st = eHCMSResources.T2808_G1_LoaiPg; break;
                            //case (int)eConfiguration_Management.mListLocations3:
                            //    st = eHCMSResources.G2177_G1_VT; break;
                            //case (int)eConfiguration_Management.mListLocations4:
                            //    st = eHCMSResources.N0254_G1_NhomVatTu; break;
                            //case (int)eConfiguration_Management.mListLocations5:
                            //    st = eHCMSResources.T2855_G1_LoaiVT; break;
                            //case (int)eConfiguration_Management.mPtBedAllocations:
                            //    st = "Giường Bệnh"; break;

                            //case (int)eConfiguration_Management.mPtUserAccount:
                            //    st = "Thêm Người Dùng"; break;
                            //case (int)eConfiguration_Management.mPtModules:
                            //    st = "Quản Lý Modules"; break;
                            //case (int)eConfiguration_Management.mPtModulesTree:
                            //    st = "Quản Lý Modules"; break;

                        //}
                        //return st;
                    case eModules.mSystem_Management:
                        switch (_eFunction)
                        {
                            case (int)eSystem_Management.mPrinterSettings:
                                st = "Thiết Lập In Ấn"; break;
                        }
                        return st;
                    case eModules.mResources:
                        switch (_eFunction)
                        {
                            case (int)eResources.mPtDashboardResource:
                                st = "Danh Mục Vật Tư Y Tế"; break;
                            case (int)eResources.mPtDashboardResource_Office:
                                st = eHCMSResources.K2915_G1_DMucVTVP;
                                break;
                            case (int)eResources.mPtDashboardNewAllocations:
                                st = "Phân Bố Vật Tư Y Tế"; break;
                            case (int)eResources.mPtDashboardNewAllocations__Office:
                                st = "Phân Bố Vật Tư Văn Phòng"; break;
                            case (int)eResources.mPtDashboardNewTranfers:
                                st = eHCMSResources.K3498_G1_DChuyenVT; break;
                            case (int)eResources.mPtDashboardSuppliers:
                                st = "Quản lý NCC Vật Tư Y Tế";
                                break;
                            case (int)eResources.mPtDashboardSuppliers_Office:
                                st = "Quản lý NCC Vật Tư Văn Phòng";
                                break;
                            case (int)eResources.mReports:
                                st = eHCMSResources.K1048_G1_BC;
                                break;
                        }
                        return st;
                    case eModules.mResources_Maintenance:
                        switch (_eFunction)
                        {
                            case (int)eResources_Maintenance.mListRequest:
                                st = eHCMSResources.K2919_G1_DS; break;
                            case (int)eResources_Maintenance.mAddNewRequest:
                                st = eHCMSResources.G0276_G1_ThemMoi; break;
                        }
                        return st;
                    case eModules.mAppointment_System:
                        switch (_eFunction)
                        {
                            case (int)eAppointment_System.mPatientAppointment:
                                st = eHCMSResources.Q0469_G1_QuanLyHenBenh; break;
                            case (int)eAppointment_System.mAppointmentList:
                                st = "Danh Sách Hẹn Bệnh"; break;
                        }
                        return st;
                    case eModules.mUserAccount:

                        return "";
                    default:
                        return "";
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public MenuBindingSource()
        {
            _ID = Guid.NewGuid();
        }
        public MenuBindingSource(eModules eItem, string navigateURL, string targetName)
            : this()
        {
            _eItem = eItem;
            _headerText = ItemName;
            _navigateURL = navigateURL;
            _targetName = targetName;
        }
        public MenuBindingSource(int eFunction, string navigateURL, string targetName, MenuBindingSource parent)
            : this()
        {
            _eItem = parent.eItem;
            _eFunction = eFunction;
            _headerText = FunctionName;
            _navigateURL = navigateURL;
            _targetName = targetName;
            _parent = parent;
        }
        public MenuBindingSource(int eFunction, string navigateURL, string targetName, int parentEnum)
            : this()
        {
            _eItem = (eModules)parentEnum;
            _eFunction = eFunction;
            _headerText = FunctionName;
            _navigateURL = navigateURL;
            _targetName = targetName;
        }
        public MenuBindingSource(string headerText, string navigateURL, string targetName)
            : this()
        {
            _headerText = headerText;
            _navigateURL = navigateURL;
            _targetName = targetName;
        }
        public MenuBindingSource(string headerText, string navigateURL, string targetName, MenuBindingSource parent)
            : this()
        {
            _headerText = headerText;
            _navigateURL = navigateURL;
            _targetName = targetName;
            _parent = parent;
            _eItem = parent._eItem;
        }
        public MenuBindingSource(string headerText, string navigateURL, string targetName, MenuBindingSource parent, bool isDefaultItem)
            : this(headerText, navigateURL, targetName, parent)
        {
            if (isDefaultItem)
            {
                if (parent != null)
                {
                    parent.DefaultItem = this;
                }
            }
        }
        private string _headerText;
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                if (_headerText != value)
                {
                    _headerText = value;
                    OnPropertyChanged("HeaderText");
                }
            }
        }
        private string _navigateURL;
        public string NavigateURL
        {
            get { return _navigateURL; }
            set
            {
                if (_navigateURL != value)
                {
                    _navigateURL = value;
                    OnPropertyChanged("NavigateURL");
                }
            }
        }
        private string _targetName;
        public string TargetName
        {
            get { return _targetName; }
            set
            {
                if (_targetName != value)
                {
                    _targetName = value;
                    OnPropertyChanged("TargetName");
                }
            }
        }
        private ObservableCollection<MenuBindingSource> _items;
        public IEnumerable<MenuBindingSource> Items
        {
            get { return _items; }
        }

        private MenuBindingSource _parent;
        public MenuBindingSource Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged("Parent");
                }
            }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public MenuBindingSource AddItem(MenuBindingSource source)
        {
            if (_items == null)
                _items = new ObservableCollection<MenuBindingSource>();
            _items.Add(source);
            OnPropertyChanged("Items");
            return source;
        }

        public bool RemoveItem(MenuBindingSource source)
        {
            bool hasBeenRemoved = false;
            if (_items != null)
            {
                hasBeenRemoved = _items.Remove(source);
                if (hasBeenRemoved)
                    OnPropertyChanged("Items");
            }
            return hasBeenRemoved;
        }

        public void Clear()
        {
            if (_items != null && _items.Count > 0)
            {
                _items.Clear();
                OnPropertyChanged("Items");
            }
        }

        public void Insert(int index, MenuBindingSource item)
        {
            if (_items == null)
                _items = new ObservableCollection<MenuBindingSource>();
            _items.Insert(index, item);
            OnPropertyChanged("Items");
        }

        public override string ToString()
        {
            return _headerText;
        }
        public override bool Equals(object obj)
        {
            MenuBindingSource cond = obj as MenuBindingSource;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ID == cond.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Kiểm tra item này có chứa navigation url hay không.
        /// </summary>
        /// <param name="navigateURL"></param>
        /// <returns></returns>
        public bool ContainNavigationURL(string navigateURL)
        {
            if (!string.IsNullOrEmpty(navigateURL) &&
                (_navigateURL.ToLower() == navigateURL.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public MenuBindingSource FindItemContainsNavigationURL(string navURL)
        {
            if (_items == null || _items.Count == 0)
            {
                if (this.ContainNavigationURL(navURL))
                {
                    return this;
                }
                else
                {
                    return null;
                }
            }
            MenuBindingSource match = null;
            foreach (MenuBindingSource child in _items)
            {
                if (child.ContainNavigationURL(navURL))
                {
                    return child;
                }
                else
                {
                    match = child.FindItemContainsNavigationURL(navURL);
                    if (match != null)
                    {
                        break;
                    }
                }
            }
            return match;
        }

        private MenuBindingSource _defaultItem;
        /// <summary>
        /// Menu item con mặc định khi menu item này được chọn.
        /// </summary>
        public MenuBindingSource DefaultItem
        {
            get
            {
                if (_defaultItem == null)
                {
                    if (_items != null && _items.Count > 0)
                    {
                        _defaultItem = _items[0];
                    }
                }
                return _defaultItem;
            }
            set
            {
                _defaultItem = value;
                OnPropertyChanged("DefaultItem");
            }
        }
    }
}
