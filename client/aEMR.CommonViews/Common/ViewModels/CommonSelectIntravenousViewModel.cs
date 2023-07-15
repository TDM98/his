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
using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using System.Threading;
using System.Collections.Generic;
using eHCMSLanguage;
using System.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Service.Core.Common;

using CommonService_V2Proxy;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICommonSelectIntravenous)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommonSelectIntravenousViewModel : Conductor<object>, ICommonSelectIntravenous
    {
        #region Properties
        private IMinHourDateControl _StartDateContent;
        public IMinHourDateControl StartDateContent
        {
            get { return _StartDateContent; }
            set
            {
                _StartDateContent = value;
                NotifyOfPropertyChange(() => StartDateContent);
            }
        }

        private IMinHourDateControl _StopDateContent;
        public IMinHourDateControl StopDateContent
        {
            get { return _StopDateContent; }
            set
            {
                _StopDateContent = value;
                NotifyOfPropertyChange(() => StopDateContent);
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _ReqOutwardDrugClinicDeptPatientList;
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDrugClinicDeptPatientList
        {
            get
            {
                return _ReqOutwardDrugClinicDeptPatientList;
            }
            set
            {
                if (_ReqOutwardDrugClinicDeptPatientList != value)
                {
                    _ReqOutwardDrugClinicDeptPatientList = value;
                    NotifyOfPropertyChange(() => ReqOutwardDrugClinicDeptPatientList);
                }
            }
        }
        private ReqOutwardDrugClinicDeptPatient _SelectedReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient SelectedReqOutwardDrugClinicDeptPatient
        {
            get
            {
                return _SelectedReqOutwardDrugClinicDeptPatient;
            }
            set
            {
                if (_SelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _SelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => SelectedReqOutwardDrugClinicDeptPatient);
                }
            }
        }
        private ReqOutwardDrugClinicDeptPatient _EditReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient EditReqOutwardDrugClinicDeptPatient
        {
            get
            {
                return _EditReqOutwardDrugClinicDeptPatient;
            }
            set
            {
                if (_EditReqOutwardDrugClinicDeptPatient != value)
                {
                    _EditReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => EditReqOutwardDrugClinicDeptPatient);
                }
            }
        }
        private ObservableCollection<Lookup> _InfusionProcessTypeList;
        public ObservableCollection<Lookup> InfusionProcessTypeList
        {
            get { return _InfusionProcessTypeList; }
            set
            {
                if (_InfusionProcessTypeList == value)
                    return;
                _InfusionProcessTypeList = value;
                NotifyOfPropertyChange(() => InfusionProcessTypeList);
            }
        }
        private ObservableCollection<Lookup> _InfusionTypeList;
        public ObservableCollection<Lookup> InfusionTypeList
        {
            get { return _InfusionTypeList; }
            set
            {
                if (_InfusionTypeList == value)
                    return;
                _InfusionTypeList = value;
                NotifyOfPropertyChange(() => InfusionTypeList);
            }
        }
        private ObservableCollection<Lookup> _TimeIntervalUnitList;
        public ObservableCollection<Lookup> TimeIntervalUnitList
        {
            get { return _TimeIntervalUnitList; }
            set
            {
                if (_TimeIntervalUnitList == value)
                    return;
                _TimeIntervalUnitList = value;
                NotifyOfPropertyChange(() => TimeIntervalUnitList);
            }
        }
        public DateTime MedicalInstructionDate { get; set; }
        private Intravenous _ObjIntravenous;
        public Intravenous ObjIntravenous
        {
            get
            {
                return _ObjIntravenous;
            }
            set
            {
                if (_ObjIntravenous != value)
                {
                    _ObjIntravenous = value;
                    NotifyOfPropertyChange(() => ObjIntravenous);
                }
            }
        }
        private bool _IsPercentInwardDrug;
        public bool IsPercentInwardDrug
        {
            get
            {
                return _IsPercentInwardDrug;
            }
            set
            {
                _IsPercentInwardDrug = value;
                NotifyOfPropertyChange(() => IsPercentInwardDrug);
            }
        }
        private int IntravenousPlan_InPtID;
        #endregion
        #region Events

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public CommonSelectIntravenousViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {

            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            MedicalInstructionDate = Globals.GetCurServerDateTime();
            StartDateContent = Globals.GetViewModel<IMinHourDateControl>();
            StartDateContent.DateTime = null;
            StopDateContent = Globals.GetViewModel<IMinHourDateControl>();
            StopDateContent.DateTime = null;

            IntravenousPlan_InPtID = -1;
            ObjIntravenous = new Intravenous { StartDateTime = Globals.GetCurServerDateTime(), StopDateTime = Globals.GetCurServerDateTime() };
            AddBlankRow();
            GetInfusionProcessType();
            GetInfusionType();
            GetTimeIntervalUnit();
        }
        public void txtDrugCode_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtDrugCode = sender as TextBox;
            if (!string.IsNullOrEmpty(txtDrugCode.Text))
            {
                string mDrugCode = txtDrugCode.Text;
                mDrugCode = Globals.FormatCode((long)AllLookupValues.MedProductType.THUOC, mDrugCode);
                GetRefGenMedProductDetails_Auto(mDrugCode, true, 0, 1000);
            }
        }
        public void txtQty_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtQty = sender as TextBox;
            if (!string.IsNullOrEmpty(txtQty.Text))
            {
                try
                {
                    int mQty = Convert.ToInt32(txtQty.Text);
                    EditReqOutwardDrugClinicDeptPatient.PrescribedQty = mQty;
                }
                catch
                {
                    EditReqOutwardDrugClinicDeptPatient.PrescribedQty = 0;
                }
            }
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReqOutwardDrugClinicDeptPatient == null || SelectedReqOutwardDrugClinicDeptPatient.GenMedProductID == 0)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }
            ReqOutwardDrugClinicDeptPatientList.Remove(SelectedReqOutwardDrugClinicDeptPatient);
        }
        public void grdReqOutwardDetails_BeginningEdit(object sender, RoutedEventArgs e)
        {
            EditReqOutwardDrugClinicDeptPatient = SelectedReqOutwardDrugClinicDeptPatient;
        }
        public void btnAddIntravenous()
        {
            try
            {
                if (ObjIntravenous.V_InfusionType == null)
                {
                    MessageBox.Show("Vui lòng chọn loại truyền!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                if (ObjIntravenous.V_TimeIntervalUnit == null)
                {
                    MessageBox.Show("Vui lòng chọn định lượng!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                if (ObjIntravenous.V_InfusionProcessType == null)
                {
                    MessageBox.Show("Vui lòng chọn số lần!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                if (ReqOutwardDrugClinicDeptPatientList.Count == 1)
                {
                    MessageBox.Show("Vui lòng thêm đầy đủ thành phần thuốc!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                ObjIntravenous.StartDateTime = StartDateContent.DateTime;
                ObjIntravenous.StopDateTime = StopDateContent.DateTime;
                ObjIntravenous.IntravenousID = IntravenousPlan_InPtID;
                foreach (var mIntravenousDetail in ReqOutwardDrugClinicDeptPatientList)
                {
                    mIntravenousDetail.IntravenousPlan_InPtID = IntravenousPlan_InPtID;
                    mIntravenousDetail.IDAndInfusionProcessType = IntravenousPlan_InPtID + string.Format(" : {0} {1}", (ObjIntravenous.V_InfusionType as Lookup).ObjectValue, ObjIntravenous.V_InfusionProcessType != null ? (ObjIntravenous.V_InfusionProcessType as Lookup).ObjectValue : "");
                }
                ObjIntravenous.IntravenousDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>(ReqOutwardDrugClinicDeptPatientList.Where(x => x.GenMedProductID > 0));
                IntravenousPlan_InPtID--;
                Globals.EventAggregator.Publish(new ReqOutwardDrugClinicDeptPatient_Add { AddedReqOutwardDrugClinicDeptPatient = ObjIntravenous.DeepCopy() });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void cboDrugName_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboDrugName = sender as AutoCompleteBox;
            GetRefGenMedProductDetails_Auto(e.Parameter, false, 0, 1000, cboDrugName);
        }
        public void cboDrugName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (EditReqOutwardDrugClinicDeptPatient != null)
            {
                RefGenMedProductDetails obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductDetails;
                EditReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = obj;
                AddBlankRow();
            }
        }
        #endregion
        #region Function
        private void AddBlankRow()
        {
            if (CheckBlankRow())
            {
                if (ReqOutwardDrugClinicDeptPatientList == null)
                {
                    ReqOutwardDrugClinicDeptPatientList = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                ReqOutwardDrugClinicDeptPatient p = new ReqOutwardDrugClinicDeptPatient { GenMedProductID = 0, PrescribedQty = 1, ReqQty = 1, ReqDrugInClinicDeptID = 1 };
                Lookup item = new Lookup();
                item.LookupID = (long)AllLookupValues.V_GoodsType.HANGMUA;
                item.ObjectValue = eHCMSResources.Z0595_G1_HangMua;
                ReqOutwardDrugClinicDeptPatientList.Add(p);
            }
        }
        private bool CheckBlankRow()
        {
            if (ReqOutwardDrugClinicDeptPatientList != null && ReqOutwardDrugClinicDeptPatientList.Count > 0)
            {
                for (int i = 0; i < ReqOutwardDrugClinicDeptPatientList.Count; i++)
                {
                    if (ReqOutwardDrugClinicDeptPatientList[i].GenMedProductID == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void GetRefGenMedProductDetails_Auto(string BrandName, bool IsCode, int PageIndex, int PageSize, AutoCompleteBox cboDrugName = null)
        {
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }
            long? RefGenDrugCatID_1 = null;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging_V2(IsCode, BrandName, null, (long)AllLookupValues.MedProductType.THUOC, RefGenDrugCatID_1, PageSize, PageIndex, Globals.ServerConfigSection.MedDeptElements.IntravenousCatID, null, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total;
                            var results = contract.EndRefGenMedProductDetails_SearchAutoPaging_V2(out Total, asyncResult);
                            if (IsCode)
                            {
                                if (results != null && results.Count > 0)
                                {
                                    EditReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = results.FirstOrDefault();
                                    AddBlankRow();
                                }
                            }
                            else if (!IsCode && cboDrugName != null)
                            {
                                cboDrugName.ItemsSource = results;
                                cboDrugName.PopulateComplete();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void GetInfusionProcessType()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    ICommonService_V2 contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_InfusionProcessType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<Lookup> results = contract.EndGetAllLookupValuesByType(asyncResult);
                            if (results != null)
                            {
                                if (InfusionProcessTypeList == null)
                                {
                                    InfusionProcessTypeList = new ObservableCollection<Lookup>();
                                }
                                else
                                {
                                    InfusionProcessTypeList.Clear();
                                }
                                foreach (Lookup p in results)
                                {
                                    InfusionProcessTypeList.Add(p);
                                }
                                NotifyOfPropertyChange(() => InfusionProcessTypeList);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }),
                    null);
                }
            });
            t.Start();
        }
        private void GetInfusionType()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    ICommonService_V2 contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_InfusionType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<Lookup> results = contract.EndGetAllLookupValuesByType(asyncResult);
                            if (results != null)
                            {
                                if (InfusionTypeList == null)
                                {
                                    InfusionTypeList = new ObservableCollection<Lookup>();
                                }
                                else
                                {
                                    InfusionTypeList.Clear();
                                }
                                foreach (Lookup p in results)
                                {
                                    InfusionTypeList.Add(p);
                                }
                                NotifyOfPropertyChange(() => InfusionTypeList);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }),
                    null);
                }
            });
            t.Start();
        }
        private void GetTimeIntervalUnit()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    ICommonService_V2 contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_TimeIntervalUnit, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<Lookup> results = contract.EndGetAllLookupValuesByType(asyncResult);
                            if (results != null)
                            {
                                if (TimeIntervalUnitList == null)
                                {
                                    TimeIntervalUnitList = new ObservableCollection<Lookup>();
                                }
                                else
                                {
                                    TimeIntervalUnitList.Clear();
                                }
                                foreach (Lookup p in results)
                                {
                                    TimeIntervalUnitList.Add(p);
                                }
                                NotifyOfPropertyChange(() => TimeIntervalUnitList);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }),
                    null);
                }
            });
            t.Start();
        }
        #endregion
    }
}
