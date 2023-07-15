using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
/*
 * 20200318 #001 TTM:   BM ?: Bổ sung màu cho những đăng ký chưa hoàn tất và đã chờ quá lâu.
                        Từ 60 phút đến 119 phút là màu cam, từ 120 phút trở đi là màu đỏ.
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IConsultationOutstandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationOutstandingTaskViewModel : Conductor<object>, IConsultationOutstandingTask
        , IHandle<LocationSelected>
        , IHandle<LoadMedicalInstructionEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public ConsultationOutstandingTaskViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            Globals.EventAggregator.Subscribe(this);
            IsSearchGoToKhamBenh = true;
            Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            // TxD 28/10/2018 Commented out the following query because it takes 12secs TBR
            //  After screen comes up you can press Refresh button
            //SearchRegistrationCmd();
        }
        public void RegistrationDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (Globals.DeptLocation != null)
            {
                SearchRegistrationListForOST();
            }
        }

        public void SearchRegistrationCmd()
        {
            SearchRegistrationListForOST();
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }
        private bool _IsSearchGoToKhamBenh;
        public bool IsSearchGoToKhamBenh
        {
            get { return _IsSearchGoToKhamBenh; }
            set
            {
                _IsSearchGoToKhamBenh = value;
                NotifyOfPropertyChange(() => IsSearchGoToKhamBenh);
            }
        }
        //Danh sách hoàn tất
        private ObservableCollection<PatientRegistrationDetail> _CompleteRegList;
        public ObservableCollection<PatientRegistrationDetail> CompleteRegList
        {
            get { return _CompleteRegList; }
            private set
            {
                _CompleteRegList = value;
                NotifyOfPropertyChange(() => CompleteRegList);
            }
        }

        private PatientRegistrationDetail _SelectedCompleteReg;
        public PatientRegistrationDetail SelectedCompleteReg
        {
            get { return _SelectedCompleteReg; }
            set
            {
                _SelectedCompleteReg = value;
                NotifyOfPropertyChange(() => SelectedCompleteReg);
            }
        }
        //Danh sách chờ CLS
        private ObservableCollection<PatientRegistrationDetail> _WaitingForPCLList;
        public ObservableCollection<PatientRegistrationDetail> WaitingForPCLList
        {
            get { return _WaitingForPCLList; }
            private set
            {
                _WaitingForPCLList = value;
                NotifyOfPropertyChange(() => WaitingForPCLList);
            }
        }
        //Danh sách chờ CLS
        private ObservableCollection<PatientRegistrationDetail> _WaitingForSPList;
        public ObservableCollection<PatientRegistrationDetail> WaitingForSPList
        {
            get { return _WaitingForSPList; }
            private set
            {
                _WaitingForSPList = value;
                NotifyOfPropertyChange(() => WaitingForSPList);
            }
        }
        private PatientRegistrationDetail _SelectedWaitingForPCLReg;
        public PatientRegistrationDetail SelectedWaitingForPCLReg
        {
            get { return _SelectedWaitingForPCLReg; }
            set
            {
                _SelectedWaitingForPCLReg = value;
                NotifyOfPropertyChange(() => SelectedWaitingForPCLReg);
            }
        }
        //Danh sách chờ khám
        private ObservableCollection<PatientRegistrationDetail> _WaitingForExamList;
        public ObservableCollection<PatientRegistrationDetail> WaitingForExamList
        {
            get { return _WaitingForExamList; }
            private set
            {
                _WaitingForExamList = value;
                NotifyOfPropertyChange(() => WaitingForExamList);
            }
        }

        private PatientRegistrationDetail _SelectedWaitingForExamReg;
        public PatientRegistrationDetail SelectedWaitingForExamReg
        {
            get { return _SelectedWaitingForExamReg; }
            set
            {
                _SelectedWaitingForExamReg = value;
                NotifyOfPropertyChange(() => SelectedWaitingForExamReg);
            }
        }

        public int TotalCompleteReg
        {
            get
            {
                return CompleteRegList != null ? CompleteRegList.Count : 0;
            }
        }

        public int TotalWaitingExamReg
        {
            get
            {
                return WaitingForExamList != null ? WaitingForExamList.Count : 0;
            }
        }

        public int TotalWaitingPCLReg
        {
            get
            {
                return WaitingForPCLList != null ? WaitingForPCLList.Count : 0;
            }
        }
        public int TotalWaitingSPReg
        {
            get
            {
                return WaitingForSPList != null ? WaitingForSPList.Count : 0;
            }
        }
        public void SearchRegistrationListForOST(long ExamRegStatus = (long)V_ExamRegStatus.mDangKyKham, long OutPtEntStatus = (long)V_OutPtEntStatus.mChoKham)
        {
            if (Globals.DeptLocation == null || Globals.DeptLocation.DeptID <= 0)
            {
                return;
            }
            this.ShowBusyIndicator();
            if (Globals.DeptLocation == null)
            {
                this.HideBusyIndicator();
                return;
            }
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    IsLoading = true;
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationListForOST(Globals.DeptLocation.DeptID, Globals.DeptLocation.DeptLocationID
                            , Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), ExamRegStatus, OutPtEntStatus
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PatientRegistrationDetail> allItems = null;
                            try
                            {
                                allItems = client.EndSearchRegistrationListForOST(asyncResult);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            //if (allItems != null && allItems.Count > 0)
                            //{
                            //    WaitingForExamList = new PagedSortableCollectionView<PatientRegistrationDetail>();
                            //    WaitingForPCLList = new PagedSortableCollectionView<PatientRegistrationDetail>();
                            //    CompleteRegList = new PagedSortableCollectionView<PatientRegistrationDetail>();
                            //    foreach (var item in allItems)
                            //    {
                            //        if (item.V_ExamRegStatus == (long)V_ExamRegStatus.mDangKyKham)
                            //        {
                            //            WaitingForExamList.Add(item);
                            //        }
                            //        else if (item.V_ExamRegStatus == (long)V_ExamRegStatus.mChoKham) // Chờ kết quả cận lâm sàng
                            //        {
                            //            WaitingForPCLList.Add(item);
                            //        }
                            //        else if (item.V_ExamRegStatus == (long)V_ExamRegStatus.mHoanTat)
                            //        {
                            //            CompleteRegList.Add(item);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    WaitingForExamList = new PagedSortableCollectionView<PatientRegistrationDetail>();
                            //    WaitingForPCLList = new PagedSortableCollectionView<PatientRegistrationDetail>();
                            //    CompleteRegList = new PagedSortableCollectionView<PatientRegistrationDetail>();
                            //}
                            //NotifyOfPropertyChange(() => TotalCompleteReg);
                            //NotifyOfPropertyChange(() => TotalWaitingExamReg);
                            //NotifyOfPropertyChange(() => TotalWaitingPCLReg);
                            if (allItems != null && allItems.Count > 0)
                            {
                                WaitingForExamList = new ObservableCollection<PatientRegistrationDetail>();
                                WaitingForPCLList = new ObservableCollection<PatientRegistrationDetail>();
                                CompleteRegList = new ObservableCollection<PatientRegistrationDetail>();
                                WaitingForSPList = new ObservableCollection<PatientRegistrationDetail>();
                                if (OutPtEntStatus ==(long)V_OutPtEntStatus.mChoThuThuat)
                                {
                                    WaitingForSPList = allItems.ToObservableCollection();
                                }
                                else if (ExamRegStatus == (long)V_ExamRegStatus.mDangKyKham)
                                {
                                    WaitingForExamList = allItems.ToObservableCollection();
                                }
                                else if (ExamRegStatus == (long)V_ExamRegStatus.mChoKham)
                                {
                                    WaitingForPCLList = allItems.ToObservableCollection();
                                }
                                else if (ExamRegStatus == (long)V_ExamRegStatus.mHoanTat)
                                {
                                    CompleteRegList = allItems.ToObservableCollection();
                                }
                            }
                            else
                            {
                                WaitingForSPList = new ObservableCollection<PatientRegistrationDetail>();
                                WaitingForExamList = new ObservableCollection<PatientRegistrationDetail>();
                                WaitingForPCLList = new ObservableCollection<PatientRegistrationDetail>();
                                CompleteRegList = new ObservableCollection<PatientRegistrationDetail>();
                            }
                            NotifyOfPropertyChange(() => TotalCompleteReg);
                            NotifyOfPropertyChange(() => TotalWaitingExamReg);
                            NotifyOfPropertyChange(() => TotalWaitingPCLReg);
                            NotifyOfPropertyChange(() => TotalWaitingSPReg);
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            PatientRegistrationDetail SelectedItem = eventArgs.Value as PatientRegistrationDetail;

            Globals.EventAggregator.Publish(new RegDetailFromOutStandingTask() { Source = SelectedItem });
        }

        public void DoubleClick1(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedWaitingForPCLReg = eventArgs.Value as PatientRegistrationDetail;

            Globals.EventAggregator.Publish(new RegDetailFromOutStandingTask() { Source = SelectedWaitingForPCLReg });
        }

        public void DoubleClick2(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedCompleteReg = eventArgs.Value as PatientRegistrationDetail;

            Globals.EventAggregator.Publish(new RegDetailFromOutStandingTask() { Source = SelectedCompleteReg });
        }

        public void hplRefresh_Waiting()
        {
            SearchRegistrationListForOST((long)V_ExamRegStatus.mDangKyKham);
        }
        public void hplRefresh_WaitingPCL()
        {
            SearchRegistrationListForOST((long)V_ExamRegStatus.mChoKham);
        }
        public void hplRefresh_Complete()
        {
            SearchRegistrationListForOST((long)V_ExamRegStatus.mHoanTat);
        }
        public void hplRefresh_WaitingSP()
        {
            SearchRegistrationListForOST(0,(long)V_OutPtEntStatus.mChoThuThuat);
        }
        public void Handle(LocationSelected message)
        {
            if (message != null && message.DeptLocation != null)
            {
                if (this.GetView() != null)
                {
                    SearchRegistrationListForOST();
                }
            }
        }
        public void Handle(LoadMedicalInstructionEvent message)
        {
            if (message != null && this.GetView() != null && message.gRegistration != null)
            {
                LoadInPatientInstruction(message.gRegistration);
            }
        }
        private void LoadInPatientInstruction(PatientRegistration aRegistration)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientInstructionCollection(aRegistration,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    gPatientInstructionCollection = contract.EndGetInPatientInstructionCollection(asyncResult).ToObservableCollection();
                                    PatientInstructionTreeCollection = new ObservableCollection<InPatientInstructionTree>();
                                    foreach (var item in gPatientInstructionCollection.Select(x => x.Department).ToList())
                                    {
                                        if (!PatientInstructionTreeCollection.Select(x => x.Department.DeptID).Contains(item.DeptID))
                                            PatientInstructionTreeCollection.Add(new InPatientInstructionTree(item));
                                    }
                                    foreach(var item in PatientInstructionTreeCollection)
                                    {
                                        item.Children = gPatientInstructionCollection.Where(x => x.Department.DeptID == item.Department.DeptID).Select(x => new InPatientInstructionTree(x)).ToList();
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
        private ObservableCollection<InPatientInstruction> _gPatientInstructionCollection;
        public ObservableCollection<InPatientInstruction> gPatientInstructionCollection
        {
            get
            {
                return _gPatientInstructionCollection;
            }
            set
            {
                _gPatientInstructionCollection = value;
                NotifyOfPropertyChange(() => gPatientInstructionCollection);
            }
        }
        private ObservableCollection<InPatientInstructionTree> _PatientInstructionTreeCollection;
        public ObservableCollection<InPatientInstructionTree> PatientInstructionTreeCollection
        {
            get
            {
                return _PatientInstructionTreeCollection;
            }
            set
            {
                _PatientInstructionTreeCollection = value;
                NotifyOfPropertyChange(() => PatientInstructionTreeCollection);
            }
        }
        public void InstructionDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TreeView).SelectedItem == null) return;
            InPatientInstructionTree SelectedItem = (sender as TreeView).SelectedItem as InPatientInstructionTree;
            if (SelectedItem.Instruction != null)
                Globals.EventAggregator.Publish(new ReloadLoadMedicalInstructionEvent() { gInPatientInstruction = SelectedItem.Instruction });
        }
        private bool _IsMedicalInstruction = false;
        public bool IsMedicalInstruction
        {
            get
            {
                return _IsMedicalInstruction;
            }
            set
            {
                _IsMedicalInstruction = value;
                NotifyOfPropertyChange(() => IsMedicalInstruction);
                if (IsMedicalInstruction && gTabCommon != null)
                    gTabCommon.SelectedIndex = 3;
                else
                    gTabCommon.SelectedIndex = 0;
            }
        }
        private TabControl gTabCommon { get; set; }
        public void TabCommon_Loaded(object sender, RoutedEventArgs e)
        {
            gTabCommon = (TabControl)sender;
        }
        private int currentTabIndex = 0;
        public void TabCommon_Changed(object source, object eventArgs)
        {
            var tabCtrl = source as TabControl;
            int destTabIndex = tabCtrl.SelectedIndex;
            if (destTabIndex != currentTabIndex)
            {
                if (destTabIndex == 0)
                {
                    SearchRegistrationListForOST((long)V_ExamRegStatus.mDangKyKham);
                }
                else if (destTabIndex == 1)
                {
                    SearchRegistrationListForOST((long)V_ExamRegStatus.mChoKham);
                }
                else if (destTabIndex == 2)
                {
                    SearchRegistrationListForOST((long)V_ExamRegStatus.mHoanTat);
                }
                else if (destTabIndex == 4)
                {
                    SearchRegistrationListForOST(0,85102);
                }
                currentTabIndex = destTabIndex;
            }
        }
        //▼===== #001
        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PatientRegistrationDetail item = e.Row.DataContext as PatientRegistrationDetail;
            if (item == null)
            {
                return;
            }
            //if (item.PatientWaitStatus == 1)
            //{
            //    e.Row.Foreground = new SolidColorBrush(Colors.Orange);
            //}
            //else if (item.PatientWaitStatus == 2)
            //{
            //    e.Row.Foreground = new SolidColorBrush(Colors.Red);
            //}
            if (item.IsDigitalSigned == 1)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Blue);
            }
            else if (item.IsDigitalSigned == 2)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
    }
    //▲===== #001
}