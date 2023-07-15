using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using System.Text;
using eHCMSLanguage;
using aEMR.ViewContracts;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;

/*
 * 20181010 #001 TBL: BM 0001162: Fix khi kéo scrollview nhanh bằng chuột thì chương trình bị văng
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IRefGenDrugBHYT_Category)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenDrugBHYT_CategoryViewModel : Conductor<object>, IRefGenDrugBHYT_Category                 
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public RefGenDrugBHYT_CategoryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            RefGenDrugBHYT_Category_Load();
            _eventArg = eventArg;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _eventArg.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }
        /*▼====: #001*/
        //private List<RefGenDrugBHYT_Category> _RefGenDrugBHYT_CategoryList;
        //public List<RefGenDrugBHYT_Category> RefGenDrugBHYT_CategoryList
        private ObservableCollection<RefGenDrugBHYT_Category> _RefGenDrugBHYT_CategoryList;
        public ObservableCollection<RefGenDrugBHYT_Category> RefGenDrugBHYT_CategoryList
        /*▲====: #001*/
        {
            get
            {
                return _RefGenDrugBHYT_CategoryList;
            }
            set
            {
                if (_RefGenDrugBHYT_CategoryList != value)
                {
                    _RefGenDrugBHYT_CategoryList = value;
                    NotifyOfPropertyChange(() => RefGenDrugBHYT_CategoryList);
                }
            }
        }

        private List<RefGenDrugBHYT_Category> _RefGenDrugBHYT_CategoryCombineList;
        public List<RefGenDrugBHYT_Category> RefGenDrugBHYT_CategoryCombineList
        {
            get
            {
                return _RefGenDrugBHYT_CategoryCombineList;
            }
            set
            {
                if (_RefGenDrugBHYT_CategoryCombineList != value)
                {
                    _RefGenDrugBHYT_CategoryCombineList = value;
                    NotifyOfPropertyChange(() => RefGenDrugBHYT_CategoryCombineList);
                }
            }
        }

        private RefGenDrugBHYT_Category _SelectedCategoryCombine;
        public RefGenDrugBHYT_Category SelectedCategoryCombine
        {
            get
            {
                return _SelectedCategoryCombine;
            }
            set
            {
                if (_SelectedCategoryCombine != value)
                {
                    _SelectedCategoryCombine = value;
                    NotifyOfPropertyChange(() => SelectedCategoryCombine);
                }
            }
        }

        public void RefGenDrugBHYT_Category_Load()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRefGenDrugBHYT_Category_Load(false, null, 
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                List<RefGenDrugBHYT_Category> allItems = new List<RefGenDrugBHYT_Category>();
                                try
                                {
                                    allItems = contract.EndRefGenDrugBHYT_Category_Load(asyncResult);
                                    SplitRefGenDrugBHYT_CategoryList(allItems);

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void CombineCmd()
        {
            if (RefGenDrugBHYT_CategoryList == null || RefGenDrugBHYT_CategoryList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0657_G1_Msg_InfoKhCoNhomBHYTDeSapNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            List<RefGenDrugBHYT_Category> _categoryCheckedList = RefGenDrugBHYT_CategoryList.Where(x => x.IsChecked).ToList();

            if (_categoryCheckedList == null || _categoryCheckedList.Count <= 1)
            {
                MessageBox.Show(eHCMSResources.A0598_G1_Msg_InfoChon2NhomBHYTSapNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }


            //KMx: Không cần sắp xếp nhóm BHYT theo mã từ nhỏ đến lớn trong code nữa, dưới stored đã làm rồi (21/09/2015 10:30).
            //List<RefGenDrugBHYT_Category> OrderList = new List<RefGenDrugBHYT_Category>();

            //KMx: Sắp xếp mã BHYT từ nhỏ đến lớn (19/09/2015 11:28)
            //while (_categoryCheckedList != null && _categoryCheckedList.Count > 0)
            //{
            //    string MinDrugOrderNo = _categoryCheckedList.Min(x => Convert.ToInt64(x.DrugOrderNo)).ToString();

            //    RefGenDrugBHYT_Category MinCategory = _categoryCheckedList.Where(x => x.DrugOrderNo == MinDrugOrderNo).FirstOrDefault();

            //    OrderList.Add(MinCategory);

            //    _categoryCheckedList.Remove(MinCategory);
            //}

            //string CombineListXml = ConvertCategoryCheckedListToXml(OrderList);

            string CombineListXml = ConvertCategoryCheckedListToXml(_categoryCheckedList);

            Combine_RefGenDrugBHYT_Category(CombineListXml);
        }


        private string ConvertCategoryCheckedListToXml(List<RefGenDrugBHYT_Category> categoryCheckedList)
        {
            StringBuilder sb = new StringBuilder();

            if (categoryCheckedList != null && categoryCheckedList.Count() > 0)
            {
                sb.Append("<CategoryCheckedList>");
                foreach (var item in categoryCheckedList)
                {
                    sb.Append("<RefGenDrugBHYT_CatID>");
                    sb.AppendFormat("<ID>{0}</ID>", item.RefGenDrugBHYT_CatID);
                    sb.Append("</RefGenDrugBHYT_CatID>");
                }
                sb.Append("</CategoryCheckedList>");
            }
            else
            {
                return string.Empty;
            }

            return sb.ToString();
        }


        public void Combine_RefGenDrugBHYT_Category(string categoryCheckedListXml)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCombine_RefGenDrugBHYT_Category(categoryCheckedListXml,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out = new List<RefGenDrugBHYT_Category>();
                                try
                                {
                                    bool result = contract.EndCombine_RefGenDrugBHYT_Category(out RefGenDrugBHYT_Category_Out, asyncResult);
                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.A0964_G1_Msg_InfoSapNhapOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        fcUnCheck();
                                        RefGenDrugBHYT_CategoryCombineList = RefGenDrugBHYT_Category_Out;
                                    }
                                    else
                                    {

                                        MessageBox.Show(eHCMSResources.Z0957_G1_SapNhapNhonBHYTThatBai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                                    }

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void fcUnCheck()
        {
            if (RefGenDrugBHYT_CategoryList == null || RefGenDrugBHYT_CategoryList.Count <= 0)
            {
                return;
            }

            List<RefGenDrugBHYT_Category> _checkList = RefGenDrugBHYT_CategoryList.Where(x => x.IsChecked).ToList();

            if (_checkList == null || _checkList.Count <= 0)
            {
                return;
            }

            foreach (RefGenDrugBHYT_Category item in _checkList)
            {
                item.IsChecked = false;
            }
        }


        public void SplitRefGenDrugBHYT_CategoryList(List<RefGenDrugBHYT_Category> listOrig)
        {
            /*▼====: #001*/
            //RefGenDrugBHYT_CategoryList = new List<RefGenDrugBHYT_Category>();
            RefGenDrugBHYT_CategoryList = new ObservableCollection<RefGenDrugBHYT_Category>();
            /*▲====: #001*/
            RefGenDrugBHYT_CategoryCombineList = new List<RefGenDrugBHYT_Category>();

            if (listOrig == null)
            {
                return;
            }

            foreach (RefGenDrugBHYT_Category item in listOrig)
            {
                if (item.IsCombined)
                {
                    RefGenDrugBHYT_CategoryCombineList.Add(item);
                }
                else
                {
                    RefGenDrugBHYT_CategoryList.Add(item);
                }
            }
        }


        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCategoryCombine == null || SelectedCategoryCombine.RefGenDrugBHYT_CatID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0658_G1_Msg_InfoKhCoNhomBHYTDeXoa, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0167_G1_Msg_ConfXoaNhomBHYT, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteRefGenDrugBHYT_CategoryCombine(SelectedCategoryCombine.RefGenDrugBHYT_CatID);
            }
        }

        private void DeleteRefGenDrugBHYT_CategoryCombine(long RefGenDrugBHYT_CatID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeleteRefGenDrugBHYT_CategoryCombine(RefGenDrugBHYT_CatID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out = new List<RefGenDrugBHYT_Category>();
                                try
                                {
                                    bool result = contract.EndDeleteRefGenDrugBHYT_CategoryCombine(out RefGenDrugBHYT_Category_Out, asyncResult);
                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.K0530_G1_XoaNhomBHYTOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        RefGenDrugBHYT_CategoryCombineList = RefGenDrugBHYT_Category_Out;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.K0531_G1_XoaNhomBHYTFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();


        }
    }
}