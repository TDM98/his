using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
using aEMR.Common;
using System.Windows.Input;
using aEMR.Common.HotKeyManagement;
using aEMR.Controls;
using System.Windows.Controls;
using aEMR.Common.Collections;
using System.Linq;
using System.Windows.Media;
/*
* 20230607 #001 DatTB: 
* + Thêm các trường lưu bệnh phẩm xét nghiệm
* + Thêm chức năng song ngữ mẫu kết quả xét nghiệm
*/ 
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IPatientPCLImagingItemResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLImagingItemResultViewModel : ViewModelBase, IPatientPCLImagingItemResult
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<DbClickPatientPCLRequest<PatientPCLRequest, string>>
        , IHandle<LocationSelected>
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
        , IHandle<PCLDeptImagingResultLoadEvent>
        , IHandle<ItemSelected<PatientPCLImagingResult, ObservableCollection<PCLResultFileStorageDetail>>>
        , IHandle<PCLResultReloadEvent>
    {
        [ImportingConstructor]
        public PatientPCLImagingItemResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            if (Globals.PCLDepartment.ObjPCLResultParamImpID != null)
            {
                GetResourcesForMedicalServicesListByTypeID(Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID);
            }
            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            aucDoctorResult = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucDoctorResult.StaffCatType = (long)V_StaffCatType.BacSi;
            ObjPatientPCLImagingResult_General = new PatientPCLImagingResult();
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            Content = SAVE;
            //▼==== #001
            GetAllSpecimen();
            //▲==== #001
        }

        protected override void OnActivate()
        {
            Globals.EventAggregator.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        #region Properties
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        private long PatientPCLReqID = 0;
        private long PatientID = 0;
        private string StaffName = "";
        private bool _isEnable = true;
        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    NotifyOfPropertyChange(() => IsEnable);
                }
            }
        }
        //▼====: #001
        private bool _IsWaitResultEnabled = false;
        public bool IsWaitResultEnabled
        {
            get
            {
                return _IsWaitResultEnabled;
            }
            set
            {
                if (_IsWaitResultEnabled != value)
                {
                    _IsWaitResultEnabled = value;
                    NotifyOfPropertyChange(() => IsWaitResultEnabled);
                }
            }
        }
        private bool _IsWaitResultVisibility = false;
        public bool IsWaitResultVisibility
        {
            get
            {
                return _IsWaitResultVisibility;
            }
            set
            {
                if (_IsWaitResultVisibility != value)
                {
                    _IsWaitResultVisibility = value;
                    NotifyOfPropertyChange(() => IsWaitResultVisibility);
                }
            }
        }
        private bool _IsDoneVisibility = false;
        public bool IsDoneVisibility
        {
            get
            {
                return _IsDoneVisibility;
            }
            set
            {
                if (_IsDoneVisibility != value)
                {
                    _IsDoneVisibility = value;
                    NotifyOfPropertyChange(() => IsDoneVisibility);
                }
            }
        }
        private bool _MoveInDataGridWithArrow = false;
        public bool MoveInDataGridWithArrow
        {
            get
            {
                return _MoveInDataGridWithArrow;
            }
            set
            {
                if (_MoveInDataGridWithArrow != value)
                {
                    _MoveInDataGridWithArrow = value;
                    NotifyOfPropertyChange(() => MoveInDataGridWithArrow);
                }
            }
        }
        //▲====: #001

        private PatientPCLRequest _CurPatientPCLRequest;
        public PatientPCLRequest CurPatientPCLRequest
        {
            get
            {
                return _CurPatientPCLRequest;
            }
            set
            {
                if (_CurPatientPCLRequest == value)
                    return;
                _CurPatientPCLRequest = value;
                IsWaitResultVisibility = _CurPatientPCLRequest.IsHaveWaitResult;
                if (_CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
                {
                    IsWaitResultEnabled = false;
                    if (_CurPatientPCLRequest.IsWaitResult)
                    {
                        IsDoneVisibility = true;
                    }
                    else
                    {
                        IsDoneVisibility = false;
                    }
                }
                else
                {
                    IsWaitResultEnabled = true;
                    IsDoneVisibility = false;
                }
                NotifyOfPropertyChange(() => CurPatientPCLRequest);
            }
        }
        public long PCLRequestTypeID
        {
            get
            {
                return CurPatientPCLRequest != null && (long)CurPatientPCLRequest.V_PCLRequestType > 0 ? (long)CurPatientPCLRequest.V_PCLRequestType : 0;
            }
        }

        private ObservableCollection<PatientPCLImagingResultDetail> _allPatientPCLImagingResultDetail;
        public ObservableCollection<PatientPCLImagingResultDetail> allPatientPCLImagingResultDetail
        {
            get
            {
                return _allPatientPCLImagingResultDetail;
            }
            set
            {
                if (_allPatientPCLImagingResultDetail == value)
                    return;
                _allPatientPCLImagingResultDetail = value;
                NotifyOfPropertyChange(() => allPatientPCLImagingResultDetail);
            }
        }

        private PatientPCLImagingResultDetail _CurPatientPCLImagingResultDetail;
        public PatientPCLImagingResultDetail CurPatientPCLImagingResultDetail
        {
            get
            {
                return _CurPatientPCLImagingResultDetail;
            }
            set
            {
                //if (!string.IsNullOrEmpty(allPatientPCLImagingResultDetail.Where(x => x.PCLExamTestItemID == 3152).FirstOrDefault().Value) && CurPatientPCLImagingResultDetail.PCLExamTestItemID == 3152)
                //{
                //    allPatientPCLImagingResultDetail.Where(x => x.PCLExamTestItemID == 3159).FirstOrDefault().Value = allPatientPCLImagingResultDetail.Where(x => x.PCLExamTestItemID == 3152).FirstOrDefault().Value;
                //}
                //if (!string.IsNullOrEmpty(allPatientPCLImagingResultDetail.Where(x => x.PCLExamTestItemID == 3161).FirstOrDefault().Value))
                //{
                //    ObjPatientPCLImagingResult_General.TemplateResult = allPatientPCLImagingResultDetail.Where(x => x.PCLExamTestItemID == 3161).FirstOrDefault().Value;
                //}
                if (_CurPatientPCLImagingResultDetail == value)
                    return;
                _CurPatientPCLImagingResultDetail = value;
                NotifyOfPropertyChange(() => CurPatientPCLImagingResultDetail);
            }
        }

        private string _Content;
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    NotifyOfPropertyChange(() => Content);
                }
            }
        }
        private string SAVE = eHCMSResources.T2937_G1_Luu;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private IAucHoldConsultDoctor _aucDoctorResult;
        public IAucHoldConsultDoctor aucDoctorResult
        {
            get
            {
                return _aucDoctorResult;
            }
            set
            {
                if (_aucDoctorResult != value)
                {
                    _aucDoctorResult = value;
                    NotifyOfPropertyChange(() => aucDoctorResult);
                }
            }
        }
        private PatientPCLImagingResult _ObjPatientPCLImagingResult_General;
        public PatientPCLImagingResult ObjPatientPCLImagingResult_General
        {
            get { return _ObjPatientPCLImagingResult_General; }
            set
            {
                if (_ObjPatientPCLImagingResult_General != value)
                {
                    _ObjPatientPCLImagingResult_General = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult_General);
                }
            }
        }
        private ObservableCollection<Resources> _HIRepResourceCode;
        public ObservableCollection<Resources> HIRepResourceCode
        {
            get { return _HIRepResourceCode; }
            set
            {
                if (HIRepResourceCode != value)
                {
                    _HIRepResourceCode = value;
                }
                NotifyOfPropertyChange(() => HIRepResourceCode);
            }
        }
        #endregion

        #region Methods

        //▼====: #001
        public void ckbIsWaitResult_Click(object source, object sender)
        {
            if (CurPatientPCLRequest == null)
            {
                return;
            }
            CheckBox ckbIsChecked = source as CheckBox;
            if (!ckbIsChecked.IsChecked.GetValueOrDefault(false))
            {
                CurPatientPCLRequest.IsDone = false;
            }
        }
        //▲====: #001
        public IEnumerator<IResult> ShowMessage(string message)
        {
            var dialog = new MessageWarningShowDialogTask(message, "", false);
            yield return dialog;
            yield break;
        }

        AxComboBox Cbo;
        public void cboHiRepResourceCode_Loaded(object sender, SelectionChangedEventArgs e)
        {
            Cbo = sender as AxComboBox;
        }
        public void cboHiRepResourceCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cbo.SelectedValue != null && !string.IsNullOrEmpty(Cbo.SelectedValue.ToString()))
            {
                ObjPatientPCLImagingResult_General.HIRepResourceCode = Cbo.SelectedValue.ToString();
            }
        }

        public void ckbIsChecked_Click(object source, object sender)
        {
            CheckBox ckbIsChecked = source as CheckBox;
            if (!(ckbIsChecked.DataContext is PatientPCLLaboratoryResultDetail))
            {
                return;
            }
            PatientPCLLaboratoryResultDetail item = (ckbIsChecked.DataContext as PatientPCLLaboratoryResultDetail);
            item.IsTemporaryBlock = ckbIsChecked.IsChecked.GetValueOrDefault(true);
        }
        //▲====: #003
        #endregion

        #region Handles
        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                if (Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    PatientPCLReqID = message.Result.PatientPCLReqID;
                    PCLImagingResults_With_ResultOld(message.Result.PatientID, message.Result.PatientPCLReqID, (long)message.Result.V_PCLRequestType);
                    CurPatientPCLRequest = message.Result;
                    if (ObjPatientPCLImagingResult_General != null && ObjPatientPCLImagingResult_General.PatientPCLRequest == null)
                    {
                        ObjPatientPCLImagingResult_General.PatientPCLRequest = CurPatientPCLRequest;
                    }
                    if (CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
                    {
                        Content = UPDATE;
                    }
                    else
                    {
                        Content = SAVE;
                    }
                    PatientID = message.Result.PatientID;
                }
                IsEnable = !message.IsReadOnly;
            }
        }
        public void Handle(DbClickPatientPCLRequest<PatientPCLRequest, string> message)
        {
            if (message != null)
            {
                if (message.ObjB == eHCMSResources.Z0055_G1_Edit)
                {
                    PatientPCLReqID = message.ObjA.PatientPCLReqID;
                    PatientID = message.ObjA.PatientID;
                    PCLImagingResults_With_ResultOld(message.ObjA.PatientID, message.ObjA.PatientPCLReqID, (long)message.ObjA.V_PCLRequestType);
                    CurPatientPCLRequest = message.ObjA;
                    if (ObjPatientPCLImagingResult_General != null && ObjPatientPCLImagingResult_General.PatientPCLRequest == null)
                    {
                        ObjPatientPCLImagingResult_General.PatientPCLRequest = CurPatientPCLRequest;
                    }
                }
                IsEnable = message.IsReadOnly;
            }
        }
        public void Handle(LocationSelected message)
        {
            //if (message != null && message.DeptLocation != null)
            //{
            //    PatientPCLReqID = 0;
            //    allPatientPCLImagingResultDetail = new ObservableCollection<PatientPCLLaboratoryResultDetail>();
            //}
        }
        public void Handle(PCLDeptImagingResultLoadEvent message)
        {
            //if (message != null
            //    && message.PatientPCLRequest_Imaging != null
            //    && message.PatientPCLRequest_Imaging.PatientPCLReqID > 0)
            //{
            //    allPatientPCLImagingResultDetail = new ObservableCollection<PatientPCLImagingResultDetail>();
            //    PatientPCLReqID = message.PatientPCLRequest_Imaging.PatientPCLReqID;
            //        PCLImagingResults_With_ResultOld(message.PatientPCLRequest_Imaging.PatientID, message.PatientPCLRequest_Imaging.PatientPCLReqID, (long)message.PatientPCLRequest_Imaging.V_PCLRequestType);
            //        CurPatientPCLRequest = message.PatientPCLRequest_Imaging;
            //        if (ObjPatientPCLImagingResult_General != null && ObjPatientPCLImagingResult_General.PatientPCLRequest == null)
            //        {
            //            ObjPatientPCLImagingResult_General.PatientPCLRequest = CurPatientPCLRequest;
            //        }
            //        if (CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
            //        {
            //            Content = UPDATE;
            //        }
            //        else
            //        {
            //            Content = SAVE;
            //        }
            //        PatientID = message.PatientPCLRequest_Imaging.PatientID;
                
            //    //IsEnable = !message.IsReadOnly;
                
            //}
        }
        public void Handle(PCLResultReloadEvent message)
        {
            if (Globals.ServerConfigSection.CommonItems.IsApplyPCRDual)
            {
                if (message != null
                && message.PatientPCLRequest_Imaging != null
                && message.PatientPCLRequest_Imaging.PatientPCLReqID > 0)
                {
                    allPatientPCLImagingResultDetail = new ObservableCollection<PatientPCLImagingResultDetail>();
                    PatientPCLReqID = message.PatientPCLRequest_Imaging.PatientPCLReqID;
                    PCLImagingResults_With_ResultOld(message.PatientPCLRequest_Imaging.PatientID, message.PatientPCLRequest_Imaging.PatientPCLReqID, (long)message.PatientPCLRequest_Imaging.V_PCLRequestType);
                    CurPatientPCLRequest = message.PatientPCLRequest_Imaging;
                    if (ObjPatientPCLImagingResult_General != null && ObjPatientPCLImagingResult_General.PatientPCLRequest == null)
                    {
                        ObjPatientPCLImagingResult_General.PatientPCLRequest = CurPatientPCLRequest;
                    }
                    if (CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
                    {
                        Content = UPDATE;
                    }
                    else
                    {
                        Content = SAVE;
                    }
                    PatientID = message.PatientPCLRequest_Imaging.PatientID;

                    //IsEnable = !message.IsReadOnly;

                }
            }
        }
        public void Handle(ItemSelected<PatientPCLImagingResult, ObservableCollection<PCLResultFileStorageDetail>> message)
        {
            if (message != null
                && message.Sender.PatientPCLRequest != null
                && message.Sender.PatientPCLRequest.PatientPCLReqID > 0)
            {
                allPatientPCLImagingResultDetail = new ObservableCollection<PatientPCLImagingResultDetail>();
                PatientPCLReqID = message.Sender.PatientPCLRequest.PatientPCLReqID;
                PCLImagingResults_With_ResultOld(message.Sender.PatientPCLRequest.PatientID, message.Sender.PatientPCLRequest.PatientPCLReqID, (long)message.Sender.PatientPCLRequest.V_PCLRequestType);
                CurPatientPCLRequest = message.Sender.PatientPCLRequest;
                if (ObjPatientPCLImagingResult_General != null && ObjPatientPCLImagingResult_General.PatientPCLRequest == null)
                {
                    ObjPatientPCLImagingResult_General.PatientPCLRequest = CurPatientPCLRequest;
                }
                if (CurPatientPCLRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
                {
                    Content = UPDATE;
                }
                else
                {
                    Content = SAVE;
                }
                PatientID = message.Sender.PatientPCLRequest.PatientID;

                //IsEnable = !message.IsReadOnly;

            }
        }
        private void GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetResourcesForMedicalServicesListByTypeID(PCLResultParamImpID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Resources> results = contract.EndGetResourcesForMedicalServicesListByTypeID(asyncResult);
                                if (results != null)
                                {
                                    if (HIRepResourceCode == null)
                                    {
                                        HIRepResourceCode = new ObservableCollection<Resources>();
                                    }
                                    else
                                    {
                                        HIRepResourceCode.Clear();
                                    }
                                    HIRepResourceCode = results.ToObservableCollection();
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
                        }), null);
                }
            });

            t.Start();
        }

        #endregion
        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                CurPatientPCLRequest = message.PCLRequest;
                if (ObjPatientPCLImagingResult_General != null && ObjPatientPCLImagingResult_General.PatientPCLRequest == null)
                {
                    ObjPatientPCLImagingResult_General.PatientPCLRequest = CurPatientPCLRequest;
                }
                allPatientPCLImagingResultDetail = new ObservableCollection<PatientPCLImagingResultDetail>();
                PCLImagingResults_With_ResultOld(message.PCLRequest.PatientID, message.PCLRequest.PatientPCLReqID, (long)message.PCLRequest.V_PCLRequestType);
            }
        }
        private void PCLImagingResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        // HPT 21/03/2017: Hiện tại đường dẫn xem kết quả xét nghiệm chỉ có trong ngoại trú nên tạm để const = ngoại trú
                        contract.BeginPCLImagingResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                IList<PatientPCLImagingResultDetail> results = contract.EndPCLImagingResults_With_ResultOld(asyncResult);
                                if (CurPatientPCLRequest.Patient != null)
                                {
                                    if (CurPatientPCLRequest.Patient.Gender == "M")
                                    {
                                        allPatientPCLImagingResultDetail = results.Where(x => x.IsForMen == true || x.IsForMen == null).ToObservableCollection();
                                    }
                                    else
                                    {
                                        allPatientPCLImagingResultDetail = results.Where(x => x.IsForMen == false || x.IsForMen == null).ToObservableCollection();
                                    }
                                }
                                else
                                {
                                    allPatientPCLImagingResultDetail = results.ToObservableCollection();
                                }
                                //▼====== #002
                                //CanBtnViewPrint = true;
                                //NotifyOfPropertyChange(() => CanBtnViewPrint);
                                //▲====== #002
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        public void ItemResult_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!MoveInDataGridWithArrow)
            {
                return;
            }
            TextBox tempTB = sender as TextBox;
           
            if (e.Key == Key.Down)
            {
                if (tempTB.Text.Length > 0 && (tempTB.Text.Length - tempTB.GetLineLength(tempTB.GetLastVisibleLineIndex())) > tempTB.CaretIndex)
                {
                    return;
                }
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                tempTB.MoveFocus(request);
                
            }
            if (e.Key == Key.Up)
            {
                if (tempTB.Text.Length > 0 && tempTB.GetLineLength(tempTB.GetFirstVisibleLineIndex()) < tempTB.CaretIndex )
                {
                    return;
                }
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                request.Wrapped = true;
                tempTB.MoveFocus(request);
               
            }
        }
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (Globals.ServerConfigSection.CommonItems.IsApplyPCRDual)
            {
                PatientPCLImagingResultDetail item = e.Row.DataContext as PatientPCLImagingResultDetail;
                if (item == null)
                {
                    return;
                }
                if (CurPatientPCLRequest.V_ReportForm == 86707)
                {
                    if (item.PrintIdx == 4 || item.PrintIdx == 5 || item.PrintIdx == 10 || item.PrintIdx == 11 || item.PrintIdx == 16)
                    {
                        e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                    }
                }
                if ( CurPatientPCLRequest.V_ReportForm == 86708)
                {
                    if ( item.PrintIdx == 7 || item.PrintIdx == 5)
                    {
                        e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                    }
                }
            }
        }
        //▼==== #001
        public string _ParamName;
        public string ParamName
        {
            get
            {
                return _ParamName;
            }
            set
            {
                if (_ParamName != value)
                {
                    _ParamName = value;
                    NotifyOfPropertyChange(() => ParamName);
                    if (ParamName.Contains("Xét Nghiệm"))
                    {
                        IsLaboratory = true;
                    }
                    else
                    {
                        IsLaboratory = false;
                    }
                }
            }
        }
        
        public bool _IsLaboratory;
        public bool IsLaboratory
        {
            get
            {
                return _IsLaboratory;
            }
            set
            {
                if (_IsLaboratory != value)
                {
                    _IsLaboratory = value;
                    NotifyOfPropertyChange(() => IsLaboratory);
                }
            }
        }

        private ObservableCollection<Specimen> _AllSpecimen;
        public ObservableCollection<Specimen> AllSpecimen
        {
            get { return _AllSpecimen; }
            set
            {
                if (_AllSpecimen != value)
                {
                    _AllSpecimen = value;
                    NotifyOfPropertyChange(() => AllSpecimen);
                }
            }
        }

        private void GetAllSpecimen()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSpecimen(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Specimen> results = contract.EndGetAllSpecimen(asyncResult);
                                if (results != null)
                                {
                                    if (AllSpecimen == null)
                                    {
                                        AllSpecimen = new ObservableCollection<Specimen>();
                                    }
                                    else
                                    {
                                        AllSpecimen.Clear();
                                    }
                                    AllSpecimen = results.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //▲==== #001
    }
}
