using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;
using aEMR.ServiceClient;
using BillingPaymentWcfServiceLibProxy;
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using System.Threading;
using System.Linq;
/*
* 20201012 #001 TNHX: Init Management Banking Payment Card
* 20221005 #002 BLQ: Thêm chức năng thẻ khách hàng
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientPaymentCardManagement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPaymentCardManagementViewModel : ViewModelBase, IPatientPaymentCardManagement
    {
        [ImportingConstructor]
        public PatientPaymentCardManagementViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            authorization();
            eventArg.Subscribe(this);
            //▼====: #002
            LoadPatientClassList();
            //▲====: #002
        }
        public AllLookupValues.RegistrationType RegistrationType { get; set; } = AllLookupValues.RegistrationType.Unknown;

        private IPatientPaymentCardManagementView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);            
            _currentView = view as IPatientPaymentCardManagementView;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                //▼====: #002
                GetCardDetailByPatientID();
                //▲====: #002
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }

        private long _RechargeAmount = 0;
        public long RechargeAmount
        {
            get { return _RechargeAmount; }
            set
            {
                _RechargeAmount = value;
                NotifyOfPropertyChange(() => RechargeAmount);
            }
        }

        private string _DateValidFrom;
        public string DateValidFrom
        {
            get { return _DateValidFrom; }
            set
            {
                _DateValidFrom = value;
                NotifyOfPropertyChange(() => DateValidFrom);
            }
        }

        private string _CardNumber;
        public string CardNumber
        {
            get { return _CardNumber; }
            set
            {
                _CardNumber = value;
                NotifyOfPropertyChange(() => CardNumber);
            }
        }

        private int _TextLength = 16;
        public int TextLength
        {
            get { return _TextLength; }
            set
            {
                _TextLength = value;
                NotifyOfPropertyChange(() => TextLength);
            }
        }

        public void authorization()
        {
            //if (!Globals.isAccountCheck)
            //{
            //    return;
            //}
            mGiaHanThe = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mModulesGen, (int)eModuleGeneral.mGiaHanTheKCB);
        }
        private bool _mGiaHanThe = true;
        public bool mGiaHanThe
        {
            get { return _mGiaHanThe; }
            set
            {
                _mGiaHanThe = value;
                NotifyOfPropertyChange(() => mGiaHanThe);
            }
        }
        public void CancelMappingCard()
        {
            if (CurrentPatient == null) return;
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
				using (var _ServiceFactory = new BillingPaymentWcfServiceLibClient())
				{
					var _Contract = _ServiceFactory.ServiceInstance;
					_Contract.BeginUnmapCard(true, CurrentPatient.PatientID,
						(long) Globals.LoggedUserAccount.StaffID,
						Globals.DispatchCallback((asyncResult) =>
						{
							TransactionResponse _TransactionResponse =
								_Contract.EndUnmapCard(asyncResult);

							this.DlgHideBusyIndicator();
							if (_TransactionResponse.IsSucceed)
							{
								MessageBox.Show("Hủy gán thẻ thành công");
								return;
							}
							MessageBox.Show(_TransactionResponse.ResponseDescription);
							ClientLoggerHelper.LogError("CancelMappingCard: "
								+ _TransactionResponse.ResponseDescription);
					}),
					null);
				}
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError("CancelMappingCard: " + ex.Message);
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                this.DlgHideBusyIndicator();
            }
        }

        public void MappingCard()
        {
            if (CurrentPatient == null) return;
            if (CurrentPatient.IDNumber == null || CurrentPatient.IDNumber.Trim() == "")
            {
                Globals.ShowMessage("Bệnh nhân phải có CMMN. Vui lòng nhập thông tin đầy đủ.",
					eHCMSResources.T0432_G1_Error);
                return;
            }
            if (CardNumber == null || CardNumber.Length != 16)
            {
                Globals.ShowMessage("Mã thẻ không đủ", eHCMSResources.T0432_G1_Error);
                return;
            }
            if (DateValidFrom == null)
            {
                Globals.ShowMessage("Ngày phát hành không bỏ trống", eHCMSResources.T0432_G1_Error);
                return;
            }
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
				using (var _ServiceFactory = new BillingPaymentWcfServiceLibClient())
				{
					var _Contract = _ServiceFactory.ServiceInstance;
					_Contract.BeginMapCard(true, CurrentPatient.PatientID, String.Empty, CardNumber,
						DateValidFrom, (long)Globals.LoggedUserAccount.StaffID,
						Globals.DispatchCallback((asyncResult) => {
							TransactionResponse _Response = _Contract.EndMapCard(asyncResult);

							this.DlgHideBusyIndicator();
							if (_Response.IsSucceed)
							{
								MessageBox.Show("Gán thẻ thành công");
								return;
							}
							MessageBox.Show(_Response.ResponseDescription);
							ClientLoggerHelper.LogError("MappingCard: "
								+ _Response.ResponseDescription);
						}),
						null);
				}
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError("MappingCard: " + ex.Message);
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                this.DlgHideBusyIndicator();
            }
        }

        public void Recharge()
        {
            if (CurrentPatient == null) return;
            if (CardNumber == null || CardNumber.Length != 16)
            {
                Globals.ShowMessage("Mã thẻ không đủ", eHCMSResources.T0432_G1_Error);
                return;
            }
            if (DateValidFrom == null)
            {
                Globals.ShowMessage("Ngày phát hành không bỏ trống", eHCMSResources.T0432_G1_Error);
                return;
            }
            if (RechargeAmount == 0)
            {
                Globals.ShowMessage("Vui lòng nhập số tiền nạp!", eHCMSResources.T0432_G1_Error);
                return;
            }
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
				using (var _ServiceFactory = new BillingPaymentWcfServiceLibClient())
				{
					var _Contract = _ServiceFactory.ServiceInstance;
					_Contract.BeginDeposit(true, CurrentPatient.PatientID, RechargeAmount,
						(long) Globals.LoggedUserAccount.StaffID,
						Globals.DispatchCallback((asyncResult) => {
							TransactionResponse _Response = _Contract.EndDeposit(asyncResult);
							this.DlgHideBusyIndicator();
							if (_Response.IsSucceed)
							{
								MessageBox.Show("Nạp tiền thành công");
								return;
							}
							MessageBox.Show(_Response.ResponseDescription);
							ClientLoggerHelper.LogError("Recharge: " + _Response.ResponseDescription);
						}),
						null);
				}
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError("Recharge: " + ex.Message);
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                this.DlgHideBusyIndicator();
            }
        }

        public void PrintPatientCard()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.CurPatient = CurrentPatient;
                proAlloc.eItem = ReportName.InTheKCB;
                proAlloc.ID = 2;
                proAlloc.ImageUrl = CurrentPatientCardDetail.V_PatientClass == 0
                    ? "" : Globals.AllLookupValueList.Where(x => x.LookupID == CurrentPatientCardDetail.V_PatientClass).FirstOrDefault().ObjectNotes;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        //▼====: #002
        private bool _IsHaveCardDetail = false;
        public bool IsHaveCardDetail
        {
            get { return _IsHaveCardDetail; }
            set
            {
                _IsHaveCardDetail = value;
                NotifyOfPropertyChange(() => IsHaveCardDetail);
            }
        }

        private bool _IsReadonlyExpireDate = false;
        public bool IsReadonlyExpireDate
        {
            get { return _IsReadonlyExpireDate; }
            set
            {
                _IsReadonlyExpireDate = value;
                NotifyOfPropertyChange(() => IsReadonlyExpireDate);
            }
        }

        private bool _IsExtendCard = false;
        public bool IsExtendCard
        {
            get { return _IsExtendCard; }
            set
            {
                _IsExtendCard = value;
                NotifyOfPropertyChange(() => IsExtendCard);
            }
        }

        private bool _IsEnabledPatientClass = true;
        public bool IsEnabledPatientClass
        {
            get { return _IsEnabledPatientClass; }
            set
            {
                _IsEnabledPatientClass = value;
                NotifyOfPropertyChange(() => IsEnabledPatientClass);
            }
        }

        private long _V_PatientClass_Old;
        public long V_PatientClass_Old
        {
            get { return _V_PatientClass_Old; }
            set
            {
                _V_PatientClass_Old = value;
                NotifyOfPropertyChange(() => V_PatientClass_Old);
            }
        }

        private DateTime _ExpireCardDate_Old;
        public DateTime ExpireCardDate_Old
        {
            get { return _ExpireCardDate_Old; }
            set
            {
                _ExpireCardDate_Old = value;
                NotifyOfPropertyChange(() => ExpireCardDate_Old);
            }
        }

        private PatientCardDetail _currentPatientCardDetail;
        public PatientCardDetail CurrentPatientCardDetail
        {
            get { return _currentPatientCardDetail; }
            set
            {
                _currentPatientCardDetail = value;
                NotifyOfPropertyChange(() => CurrentPatientCardDetail);
            }
        }

        private ObservableCollection<Lookup> _ObjV_PatientClass;
        public ObservableCollection<Lookup> ObjV_PatientClass
        {
            get { return _ObjV_PatientClass; }
            set
            {
                _ObjV_PatientClass = value;
                NotifyOfPropertyChange(() => ObjV_PatientClass);
            }
        }

        public void LoadPatientClassList()
        {
            ObjV_PatientClass = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PatientClass).ToObservableCollection();
        }

        public void GetCardDetailByPatientID()
        {
            if (CurrentPatient == null)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetCardDetailByPatientID(CurrentPatient.PatientID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                CurrentPatientCardDetail = contract.EndGetCardDetailByPatientID(asyncResult);
                                if(CurrentPatientCardDetail != null && CurrentPatientCardDetail.PatientCardDetailID > 0)
                                {
                                    IsHaveCardDetail = true;
                                    IsReadonlyExpireDate = true;
                                    IsEnabledPatientClass = true;
                                    V_PatientClass_Old = CurrentPatientCardDetail.V_PatientClass;
                                    ExpireCardDate_Old = (DateTime)CurrentPatientCardDetail.ExpireCardDate;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao);
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
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private bool CheckValidCardDetail()
        {
            if (CurrentPatientCardDetail == null)
            {
                return false;
            }
            if (CurrentPatientCardDetail.CardID == 0)
            {
                MessageBox.Show("Bệnh nhân chưa được gán thẻ. Vui lòng gán thẻ cho bệnh nhân trước khi thêm thông tin", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (string.IsNullOrEmpty(CurrentPatientCardDetail.AccountNumber) || CurrentPatientCardDetail.AccountNumber.Length != 12)
            {
                MessageBox.Show("Định dạng số tài khoản chưa đúng", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (CurrentPatientCardDetail.V_PatientClass == 0)
            {
                MessageBox.Show("Chưa chọn đối tượng khách hàng", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (CurrentPatientCardDetail.OpenCardDate == null)
            {
                MessageBox.Show("Ngày mở thẻ không được trống", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (CurrentPatientCardDetail.ExpireCardDate == null)
            {
                MessageBox.Show("Ngày hết hạn không được trống", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (CurrentPatientCardDetail.OpenCardDate >= CurrentPatientCardDetail.ExpireCardDate)
            {
                MessageBox.Show("Ngày hết hạn không được nhỏ hơn hoặc bằng ngày mở thẻ", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (IsExtendCard && CurrentPatientCardDetail.ExpireCardDate <= ExpireCardDate_Old)
            {
                MessageBox.Show("Ngày hết hạn không được gia hạn nhỏ hơn hoặc bằng với ngày hết hạn cũ", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (!IsExtendCard && CurrentPatientCardDetail.V_PatientClass == V_PatientClass_Old)
            {
                MessageBox.Show("Đối tượng không thể thay đổi giống với đối tượng cũ", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            return true;
        }
        public void btnSaveCardDetail()
        {
            if (!CheckValidCardDetail())
            {
                return;
            }
            CurrentPatientCardDetail.CreatedStaff = Globals.LoggedUserAccount.Staff.StaffID;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveCardDetail(CurrentPatientCardDetail, IsExtendCard, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var result = contract.EndSaveCardDetail(asyncResult);
                                if (result)
                                {
                                    IsHaveCardDetail = true;
                                    IsReadonlyExpireDate = true;
                                    IsExtendCard = false;
                                    IsEnabledPatientClass = true;
                                    GetCardDetailByPatientID();
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0701_G1_Msg_InfoLuuFail, eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao);
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
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnPrintSuggestPaper()
        {
            if (CurrentPatientCardDetail == null || CurrentPatientCardDetail.CardID == 0)
            {
                return;
            }
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.XRpt_GiayDeNghiMoTheKCB;
                proAlloc.ID = CurrentPatientCardDetail.CardID;
                proAlloc.StaffName = Globals.LoggedUserAccount.Staff.FullName;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void btnCardExtend()
        {
            IsExtendCard = true;
            IsReadonlyExpireDate = false;
            IsEnabledPatientClass = false;
        }
        //▲====: #002
    }
}
