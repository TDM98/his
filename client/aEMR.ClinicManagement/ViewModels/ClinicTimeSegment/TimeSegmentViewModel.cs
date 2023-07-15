using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using System.Windows.Controls;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(ITimeSegment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TimeSegmentViewModel : Conductor<object>, ITimeSegment
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TimeSegmentViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _selectedConsultTimeSeg = new ConsultationTimeSegments();
            GetAllConsultationTimeSegments();
            _curConsultTimeSeg = new ConsultationTimeSegments();
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        #region properties

        private ConsultationTimeSegments _selectedConsultTimeSeg;
        public ConsultationTimeSegments selectedConsultTimeSeg
        {
            get
            {
                return _selectedConsultTimeSeg;
            }
            set
            {
                if (_selectedConsultTimeSeg == value)
                    return;
                _selectedConsultTimeSeg = value;
                NotifyOfPropertyChange(() => selectedConsultTimeSeg);
            }
        }

        private ConsultationTimeSegments _curConsultTimeSeg;
        public ConsultationTimeSegments curConsultTimeSeg
        {
            get
            {
                return _curConsultTimeSeg;
            }
            set
            {
                if (_curConsultTimeSeg == value)
                    return;
                _curConsultTimeSeg = value;
                NotifyOfPropertyChange(() => curConsultTimeSeg);
            }
        }

        private ObservableCollection<ConsultationTimeSegments> _lstConsultationTimeSegments;
        public ObservableCollection<ConsultationTimeSegments> lstConsultationTimeSegments
        {
            get
            {
                return _lstConsultationTimeSegments;
            }
            set
            {
                if (_lstConsultationTimeSegments == value)
                    return;
                _lstConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => lstConsultationTimeSegments);
            }
        }

        private ObservableCollection<ConsultationTimeSegments> _tempConsultationTimeSegments;
        public ObservableCollection<ConsultationTimeSegments> tempConsultationTimeSegments
        {
            get
            {
                return _tempConsultationTimeSegments;
            }
            set
            {
                if (_tempConsultationTimeSegments == value)
                    return;
                _tempConsultationTimeSegments = value;
                NotifyOfPropertyChange(() => tempConsultationTimeSegments);
            }
        }
        #endregion

        public void butSave()
        {
            if (string.IsNullOrEmpty(curConsultTimeSeg.SegmentName))
            {
                MessageBox.Show(eHCMSResources.Z1768_G1_ChuaNhapTenCaKham, eHCMSResources.G0442_G1_TBao);
                return;
            }
            InsertConsultationTimeSegments(curConsultTimeSeg.SegmentName, curConsultTimeSeg.SegmentDescription
                , curConsultTimeSeg.StartTime, curConsultTimeSeg.EndTime, curConsultTimeSeg.StartTime2, curConsultTimeSeg.EndTime2, true);
            curConsultTimeSeg = new ConsultationTimeSegments();
        }

        public void butUpdate()
        {
            UpdateConsultationTimeSegments(lstConsultationTimeSegments);
        }

        public void butReset()
        {
            GetAllConsultationTimeSegments();
        }

        public void lnkDeleteClick(object sender)
        {
            if (MessageBox.Show("Bạn Muốn Xóa Ca Khám này không?\n Sẽ Xóa Các Thông Tin Liên Quan ", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteConsultationTimeSegments(selectedConsultTimeSeg.ConsultationTimeSegmentID);
            }
        }

        #region method
        private void InsertConsultationTimeSegments(string SegmentName, string SegmentDescription, DateTime StartTime,
                                            DateTime EndTime, DateTime? StartTime2, DateTime? EndTime2, bool IsActive)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertConsultationTimeSegments(SegmentName, SegmentDescription, StartTime,
                                EndTime, StartTime2, EndTime2, IsActive, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     var results = contract.EndInsertConsultationTimeSegments(asyncResult);
                                     if (results)
                                     {
                                         GetAllConsultationTimeSegments();
                                         Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
                                     }
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
            });

            t.Start();
        }

        private void DeleteConsultationTimeSegments(long ConsultationTimeSegmentID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteConsultationTimeSegments(ConsultationTimeSegmentID
                    , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteConsultationTimeSegments(asyncResult);
                                if (results)
                                {
                                    GetAllConsultationTimeSegments();
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                                }
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
            });

            t.Start();
        }

        private void UpdateConsultationTimeSegments(ObservableCollection<ConsultationTimeSegments> consultationTimeSegments)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateConsultationTimeSegments(consultationTimeSegments, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndUpdateConsultationTimeSegments(asyncResult);
                                if (results)
                                {
                                    GetAllConsultationTimeSegments();
                                    Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, "");
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, "");
                                }
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
            });
            t.Start();
        }

        private void GetAllConsultationTimeSegments()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllConsultationTimeSegments(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllConsultationTimeSegments(asyncResult);
                            if (results != null)
                            {
                                lstConsultationTimeSegments = new ObservableCollection<ConsultationTimeSegments>();
                                foreach (var consTimeSeg in results)
                                {
                                    lstConsultationTimeSegments.Add(consTimeSeg);
                                }
                            }
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
            });

            t.Start();
        }
        #endregion

        #region authoriztion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mQuanLyCaKham, (int)ePermission.mEdit);
            bAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mQuanLyCaKham, (int)ePermission.mAdd);
            bDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mQuanLyCaKham, (int)ePermission.mDelete);
            bView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mClinicManagement
                                               , (int)eClinicManagement.mQuanLyCaKham,
                                               (int)oClinicManagementEx.mQuanLyCaKham, (int)ePermission.mView);



        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
                NotifyOfPropertyChange(() => bEdit);
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
                NotifyOfPropertyChange(() => bAdd);
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
                NotifyOfPropertyChange(() => bDelete);
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
                NotifyOfPropertyChange(() => bView);
            }
        }


        #endregion
        #region binding visibilty

        public void lnkDeleteLoaded(object sender, RoutedEventArgs e)
        {
            //((HyperlinkButton)sender).Visibility= Globals.convertVisibility(bDelete);
        }
        #endregion
        #endregion

        private bool _TwoTimeVisibility= false;
        public bool TwoTimeVisibility
        {
            get
            {
                return _TwoTimeVisibility;
            }
            set
            {
                if (_TwoTimeVisibility == value)
                    return;
                _TwoTimeVisibility = value;
                NotifyOfPropertyChange(() => TwoTimeVisibility);
            }
        }
        

        public void ckbTwoTime_Click(object sender, EventAggregator e)
        {
            CheckBox ckbTwoTime = sender as CheckBox;
            if (ckbTwoTime.IsChecked== true)
            {
                TwoTimeVisibility = true;
                curConsultTimeSeg.StartTime2 = Globals.GetCurServerDateTime();
                curConsultTimeSeg.EndTime2 = Globals.GetCurServerDateTime();
            }
            else
            {
                TwoTimeVisibility = false;
                curConsultTimeSeg.StartTime2 = null;
                curConsultTimeSeg.EndTime2 = null;
            }
        }
    }
}
