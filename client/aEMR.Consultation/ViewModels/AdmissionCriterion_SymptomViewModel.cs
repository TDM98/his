using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IAdmissionCriterion_Symptom)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdmissionCriterion_SymptomViewModel : ViewModelBase, IAdmissionCriterion_Symptom
    {
        [ImportingConstructor]
        public AdmissionCriterion_SymptomViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        private long _AdmissionCriterionID;
        public long AdmissionCriterionID
        {
            get { return _AdmissionCriterionID; }
            set
            {
                if (_AdmissionCriterionID != value)
                {
                    _AdmissionCriterionID = value;
                    NotifyOfPropertyChange(() => AdmissionCriterionID);
                }
            }
        }
        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get { return _PtRegistrationID; }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    NotifyOfPropertyChange(() => PtRegistrationID);
                }
            }
        }
        private string _PatientCode;
        public string PatientCode
        {
            get { return _PatientCode; }
            set
            {
                if (_PatientCode != value)
                {
                    _PatientCode = value;
                    NotifyOfPropertyChange(() => PatientCode);
                }
            }
        }
        private ObservableCollection<SymptomCategory> _allSymptomCategory;
        public ObservableCollection<SymptomCategory> allSymptomCategory
        {
            get { return _allSymptomCategory; }
            set
            {
                _allSymptomCategory = value;
                NotifyOfPropertyChange(() => allSymptomCategory);
            }
        }
        private ObservableCollection<AdmissionCriterionAttachSymptom> _RequiredSymptomList;
        public ObservableCollection<AdmissionCriterionAttachSymptom> RequiredSymptomList
        {
            get { return _RequiredSymptomList; }
            set
            {
                _RequiredSymptomList = value;
                NotifyOfPropertyChange(() => RequiredSymptomList);
            }
        }
        private ObservableCollection<AdmissionCriterionAttachSymptom> _NonRequiredSymptomList;
        public ObservableCollection<AdmissionCriterionAttachSymptom> NonRequiredSymptomList
        {
            get { return _NonRequiredSymptomList; }
            set
            {
                _NonRequiredSymptomList = value;
                NotifyOfPropertyChange(() => NonRequiredSymptomList);
            }
        }
        private ObservableCollection<GroupPCLs> _allGroupPCLs;
        public ObservableCollection<GroupPCLs> allGroupPCLs
        {
            get
            {
                return _allGroupPCLs;
            }
            set
            {
                if (_allGroupPCLs == value)
                    return;
                _allGroupPCLs = value;
                NotifyOfPropertyChange(() => allGroupPCLs);
            }
        }
      
        public void InitInfo()
        {
            GetSymptomListByAdmissionCriterionID(AdmissionCriterionID);
            GetGroupPCLListByAdmissionCriterionID(AdmissionCriterionID,PtRegistrationID);
            GetAdmissionCriterionDetailByPtRegistrationID(PtRegistrationID);
        }
        private void GetSymptomListByAdmissionCriterionID(long CriterionID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetSymptomListByAdmissionCriterionID(CriterionID, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<AdmissionCriterionAttachSymptom> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetSymptomListByAdmissionCriterionID(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                            RequiredSymptomList = new ObservableCollection<AdmissionCriterionAttachSymptom>();
                            NonRequiredSymptomList = new ObservableCollection<AdmissionCriterionAttachSymptom>();
                            if (bOK)
                            {
                                if (allItems != null)
                                {

                                    foreach (var item in allItems.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Bat_Buoc))
                                    {
                                        RequiredSymptomList.Add(item);
                                    }
                                    foreach (var item in allItems.Where(x => x.V_SymptomType == (long)AllLookupValues.V_SymptomType.Khong_BatBuoc))
                                    {
                                        NonRequiredSymptomList.Add(item);
                                    }
                                    ReCheckSymptomList();
                                }
                                if(RequiredSymptomList.Count == 0 && NonRequiredSymptomList.Count == 0)
                                {
                                    TryClose();
                                    Globals.EventAggregator.Publish(new AdmissionCriterion_Symptom_Select_Event { Result = true });
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetGroupPCLListByAdmissionCriterionID(long CriterionID,long PtRegistrationID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGroupPCL_PCLExamType_ByAdmissionCriterionID(CriterionID, PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {

                            IList<GroupPCLs> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGroupPCL_PCLExamType_ByAdmissionCriterionID(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            //allAdmissionCriterionAttachGroupPCL.Clear();
                            allGroupPCLs = new ObservableCollection<GroupPCLs>();
                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        allGroupPCLs.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void ckbRequiredChecked_Click(object source, object sender)
        {

        }
        public void ckbNonRequiredChecked_Click(object source, object sender)
        {

        }
        public void btnOK()
        {
            if(CurrentAdmissionCriterionDetail == null)
            {
                MessageBox.Show("Chưa lưu thông tin triệu chứng. Vui lòng chọn triệu chứng và lưu lại trước khi đề nghị nhập viện");
                return;
            }
            if (RequiredSymptomList.ToList().Count > 0 && RequiredSymptomList.Where(x => x.IsChecked == true).ToList().Count == 0)
            {
                MessageBox.Show("Phải chọn ít nhất 1 triệu chứng bắt buộc");
                return;
            }
            //else if(NonRequiredSymptomList.ToList().Count > 0 && NonRequiredSymptomList.Where(x => x.IsChecked == true).ToList().Count == 0)
            //{
            //    MessageBox.Show("Phải chọn ít nhất 1 triệu chứng không bắt buộc");
            //    return;
            //}
           
            else
            {
                if (allGroupPCLs != null && allGroupPCLs.Count > 0 && allGroupPCLs.Where(x=>x.PCLExamTypeItem!=null && x.PCLExamTypeItem.Count>0).Count()>0)
                {
                    CheckGroupPCLAndResult();
                    if(tempGroupPCLHaveRequest.Count==0)
                    {
                        //MessageBox.Show("Chưa có nhóm cls nào đủ chỉ định");
                        MessageBox.Show("Chưa có cls nào có chỉ định");
                        return;
                    }
                    else if (tempGroupPCLIsDone.Count == 0)
                    {
                        //MessageBox.Show("Chưa có nhóm cls nào đủ kết quả");
                        MessageBox.Show("Chưa có cls nào có kết quả");
                        return;
                    }
                    else
                    {
                        TryClose();
                        Globals.EventAggregator.Publish(new AdmissionCriterion_Symptom_Select_Event { Result = true, Diagnosis = GetSymptomListValue() });
                    }
                }
                else
                {
                    TryClose();
                    Globals.EventAggregator.Publish(new AdmissionCriterion_Symptom_Select_Event { Result = true, Diagnosis = GetSymptomListValue() });
                }
            }
        }
        private ObservableCollection<GroupPCLs> tempGroupPCLHaveRequest = new ObservableCollection<GroupPCLs>();
        private ObservableCollection<GroupPCLs> tempGroupPCLIsDone = new ObservableCollection<GroupPCLs>();
        private void CheckGroupPCLAndResult()
        {
            foreach (var item in allGroupPCLs)
            {
                if (item != null && item.PCLExamTypeItem != null && item.PCLExamTypeItem.Count() > 0)
                {
                    //if (item.PCLExamTypeItem.Count == item.PCLExamTypeItem.Where(x => x.IsHaveRequest).Count())
                    //{
                    //    tempGroupPCLHaveRequest.Add(item);
                    //}
                    //if (item.PCLExamTypeItem.Count == item.PCLExamTypeItem.Where(x => x.IsSelected == true).Count())
                    //{
                    //    tempGroupPCLIsDone.Add(item);
                    //}
                    if (item.PCLExamTypeItem.Where(x => x.IsHaveRequest).Count() > 0)
                    {
                        tempGroupPCLHaveRequest.Add(item);
                    }
                    if (item.PCLExamTypeItem.Where(x => x.IsSelected == true).Count() > 0)
                    {
                        tempGroupPCLIsDone.Add(item);
                    }
                }
            }
        }
        private AdmissionCriterionDetail _CurrentAdmissionCriterionDetail;
        public AdmissionCriterionDetail CurrentAdmissionCriterionDetail
        {
            get
            {
                return _CurrentAdmissionCriterionDetail;
            }
            set
            {
                if (_CurrentAdmissionCriterionDetail == value)
                    return;
                _CurrentAdmissionCriterionDetail = value;
                NotifyOfPropertyChange(() => CurrentAdmissionCriterionDetail);
            }
        }
        private bool _IsBtnSaveEnable = true;
        public bool IsBtnSaveEnable
        {
            get
            {
                return _IsBtnSaveEnable;
            }
            set
            {
                if (_IsBtnSaveEnable == value)
                    return;
                _IsBtnSaveEnable = value;
                NotifyOfPropertyChange(() => IsBtnSaveEnable);
            }
        }
        private void GetAdmissionCriterionDetailByPtRegistrationID(long PtRegistrationID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        AdmissionCriterionDetail result = null;
                        client.BeginGetAdmissionCriterionDetailByPtRegistrationID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                result = client.EndGetAdmissionCriterionDetailByPtRegistrationID(asyncResult);
                                if(result == null)
                                {
                                    CurrentAdmissionCriterionDetail = new AdmissionCriterionDetail { PtRegistrationID = this.PtRegistrationID};
                                    IsBtnSaveEnable = true;
                                }
                                else
                                {
                                    CurrentAdmissionCriterionDetail = result;
                                    IsBtnSaveEnable = false;
                                    ReCheckSymptomList();
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnSave()
        {
            if(RequiredSymptomList.Where(x => x.IsChecked).Count() == 0 && NonRequiredSymptomList.Where(x => x.IsChecked).Count() == 0)
            {
                MessageBox.Show("Chưa chọn triệu chứng");
                return;
            }
            SaveAdmissionCriterionDetail(true);
        }
        public void btnEdit()
        {
            if (RequiredSymptomList.Where(x => x.IsChecked).Count() == 0 && NonRequiredSymptomList.Where(x => x.IsChecked).Count() == 0)
            {
                MessageBox.Show("Chưa chọn triệu chứng");
                return;
            }
            SaveAdmissionCriterionDetail(false);
        }
        private string GetSymptomListID()
        {
            string CheckedSymptom = "|";
            foreach (var item in RequiredSymptomList.Where(x => x.IsChecked))
            {
                CheckedSymptom += item.SymptomCategoryID + "|";
            }
            foreach (var item in NonRequiredSymptomList.Where(x => x.IsChecked))
            {
                CheckedSymptom += item.SymptomCategoryID + "|";
            }
            return CheckedSymptom;
        }
        private string GetSymptomListValue()
        {
            string CheckedSymptom = "";
            string SymptomNotUseForAdmission = Globals.ServerConfigSection.CommonItems.SymptomNotUseForAdmission;
            foreach (var item in RequiredSymptomList.Where(x => x.IsChecked))
            {
                if (!SymptomNotUseForAdmission.Contains("|" + item.SymptomCategoryID + "|"))
                {
                    CheckedSymptom += item.SymptomCategoryName + ", ";
                }
            }
            foreach (var item in NonRequiredSymptomList.Where(x => x.IsChecked))
            {
                if (!SymptomNotUseForAdmission.Contains("|" + item.SymptomCategoryID + "|"))
                {
                    CheckedSymptom += item.SymptomCategoryName + ", ";
                }
            }
            return CheckedSymptom.Substring(0, CheckedSymptom.Length - 2);
        }
        private void ReCheckSymptomList()
        {
            if(CurrentAdmissionCriterionDetail == null || CurrentAdmissionCriterionDetail.SymptomList == null)
            {
                return;
            }
            foreach (var item in RequiredSymptomList)
            {
                if(CurrentAdmissionCriterionDetail.SymptomList.Contains("|" + item.SymptomCategoryID + "|"))
                {
                    item.IsChecked = true;
                }
            }
            foreach (var item in NonRequiredSymptomList)
            {
                if (CurrentAdmissionCriterionDetail.SymptomList.Contains("|" + item.SymptomCategoryID + "|"))
                {
                    item.IsChecked = true;
                }
            }
        }
        private void SaveAdmissionCriterionDetail(bool IsInsert)
        {
            if (IsInsert)
            {
                CurrentAdmissionCriterionDetail.CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentAdmissionCriterionDetail.CreatedDate = Globals.GetCurServerDateTime();
            }
            else
            {
                CurrentAdmissionCriterionDetail.LastUpdateStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentAdmissionCriterionDetail.LastUpdateDate = Globals.GetCurServerDateTime();
            }
            CurrentAdmissionCriterionDetail.SymptomList = GetSymptomListID();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        bool result;
                        client.BeginSaveAdmissionCriterionDetail(CurrentAdmissionCriterionDetail, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                result = client.EndSaveAdmissionCriterionDetail(asyncResult);
                                if (result)
                                {
                                    GetAdmissionCriterionDetailByPtRegistrationID(PtRegistrationID);
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnAddPCLResult()
        {
            GlobalsNAV.ShowDialog<IAdmissionCriterion_PCLResult>((VM) =>
            {
                VM.AdmissionCriterionID = AdmissionCriterionID;
                VM.PtRegistrationID = PtRegistrationID;
                VM.PatientCode = PatientCode;
                VM.InitInfo();
            }, null, false, true, new Size(1200, 500));
            GetGroupPCLListByAdmissionCriterionID(AdmissionCriterionID, PtRegistrationID);
        }
    }
}
