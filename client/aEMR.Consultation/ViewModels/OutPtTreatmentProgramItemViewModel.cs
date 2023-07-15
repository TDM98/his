using aEMR.Common.BaseModel;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
/*
 * 20220916 #001 DatTB: Thêm điều kiện cảnh báo nếu nhập thiếu Ngày/giờ Ngày kết thúc
 * 20220922 #002 QTD: Set mặc định giờ kết thúc 00:00
 */
namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IOutPtTreatmentProgramItem)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPtTreatmentProgramItemViewModel : ViewModelBase, IOutPtTreatmentProgramItem
    {
        [ImportingConstructor]
        public OutPtTreatmentProgramItemViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            DischargeDateContent = Globals.GetViewModel<IMinHourDateControl>();
            //▼==== #002
            DischargeDateContent.IsEnableMinHourControl = false;
            //▲==== #002
            DischargeDateContent.DateTime = null;

            GetLookupOutDischargeCondition();
            GetLookupOutDischargeType();
            
        }
        #region Properties
        private PatientRegistration _CurrentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get { return _CurrentRegistration; }
            set
            {
                _CurrentRegistration = value;
                if(CurrentRegistration.DischargeDate != null)
                {
                    DischargeDateContent.DateTime = CurrentRegistration.DischargeDate;
                }
                GetSuggestDate();
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }

        private ObservableCollection<Lookup> _OutDischargeCondition;
        public ObservableCollection<Lookup> OutDischargeCondition
        {
            get { return _OutDischargeCondition; }
            set
            {
                _OutDischargeCondition = value;
                NotifyOfPropertyChange(() => OutDischargeCondition);
            }
        }
        private ObservableCollection<Lookup> _OutDischargeType;
        public ObservableCollection<Lookup> OutDischargeType
        {
            get { return _OutDischargeType; }
            set
            {
                _OutDischargeType = value;
                NotifyOfPropertyChange(() => OutDischargeType);
            }
        }

        private IMinHourDateControl _DischargeDateContent;
        public IMinHourDateControl DischargeDateContent
        {
            get { return _DischargeDateContent; }
            set
            {
                _DischargeDateContent = value;
                NotifyOfPropertyChange(() => DischargeDateContent);
            }
        }

        private int _MinNumOfDayMedicine;
        public int MinNumOfDayMedicine
        {
            get { return _MinNumOfDayMedicine; }
            set
            {
                _MinNumOfDayMedicine = value;
                NotifyOfPropertyChange(() => MinNumOfDayMedicine);
            }
        }

        private int _MaxNumOfDayMedicine;
        public int MaxNumOfDayMedicine
        {
            get { return _MaxNumOfDayMedicine; }
            set
            {
                _MaxNumOfDayMedicine = value;
                NotifyOfPropertyChange(() => MaxNumOfDayMedicine);
            }
        }

        private long _OutpatientTreatmentTypeID;
        public long OutpatientTreatmentTypeID
        {
            get { return _OutpatientTreatmentTypeID; }
            set
            {
                _OutpatientTreatmentTypeID = value;
                NotifyOfPropertyChange(() => OutpatientTreatmentTypeID);
            }
        }

        private DateTime _SuggestDate;
        public DateTime SuggestDate
        {
            get { return _SuggestDate; }
            set
            {
                _SuggestDate = value;
                NotifyOfPropertyChange(() => SuggestDate);
            }
        }
        #endregion
        #region Events
        public void SaveButton()
        {
            if (CurrentRegistration == null)
            {
                return;
            }
            if (CurrentRegistration.V_OutDischargeCondition == null || CurrentRegistration.V_OutDischargeCondition.LookupID == 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, "Kết quả điều trị"), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentRegistration.V_OutDischargeType == null || CurrentRegistration.V_OutDischargeType.LookupID == 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, "Tình trạng ra viện"), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (OutpatientTreatmentTypeID > 0 && CurrentRegistration.PrescriptionsAmount > MaxNumOfDayMedicine || CurrentRegistration.PrescriptionsAmount < MinNumOfDayMedicine)
            {
                Globals.ShowMessage("Số ngày cấp toa không được nhập nhỏ hơn " + MinNumOfDayMedicine + " và không lớn hơn " + MaxNumOfDayMedicine, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▼==== #001
            if ((DischargeDateContent.DatePart != null && DischargeDateContent.HourPart == null) || (DischargeDateContent.MinutePart != null && DischargeDateContent.HourPart == null))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "Giờ của Ngày kết thúc"), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if ((DischargeDateContent.DatePart != null && DischargeDateContent.MinutePart == null) || (DischargeDateContent.HourPart != null && DischargeDateContent.MinutePart == null))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "Phút của Ngày kết thúc"), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if ((DischargeDateContent.MinutePart != null && DischargeDateContent.DatePart == null) || (DischargeDateContent.HourPart != null && DischargeDateContent.DatePart == null))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "Ngày kết thúc"), eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲==== #001
            if (!CheckHIValidDate(CurrentRegistration.PrescriptionsAmount))
            {
                return;
            }
            CurrentRegistration.DischargeDate = DischargeDateContent.DateTime;
            CurrentRegistration.OutPtTreatmentProgramStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new CommonUtilsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginSaveOutPtTreatmentProgramItem(CurrentRegistration, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool bOk = CurrentContract.EndSaveOutPtTreatmentProgramItem(asyncResult);
                            if (bOk)
                            {
                                Globals.EventAggregator.Publish(new SaveOutPtTreatmentProgramItem { PrescriptionsAmount = CurrentRegistration.PrescriptionsAmount });
                            }
                            TryClose();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        #endregion

        public void GetLookupOutDischargeCondition()
        {
            OutDischargeCondition = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_OutDischargeCondition))
                {
                    OutDischargeCondition.Add(tmpLookup);
                }
            }
        }
        public void GetLookupOutDischargeType()
        {
            OutDischargeType = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_OutDischargeType))
                {
                    OutDischargeType.Add(tmpLookup);
                }
            }
        }
        public void PrescriptionsAmount_LostFocus()
        {
            GetSuggestDate();
        }
        private void GetSuggestDate()
        {
            if (CurrentRegistration != null && CurrentRegistration.ExamDate != null)
            {
                SuggestDate = CurrentRegistration.ExamDate.AddDays(CurrentRegistration.PrescriptionsAmount - 1);
                //▼==== #002
                if (CurrentRegistration.PrescriptionsAmount != 0)
                {
                    DischargeDateContent.DateTime = CurrentRegistration.ExamDate.AddDays(CurrentRegistration.PrescriptionsAmount - 1).Date;
                }
                //▲==== #002
            }
        }
        public bool CheckHIValidDate(int SelNDay)
        {
            if(CurrentRegistration == null || CurrentRegistration.ExamDate == null)
            {
                return false;
            }
            if (CurrentRegistration.HisID > 0)
            {
                DateTime CheckDate = CurrentRegistration.ExamDate.AddDays(SelNDay);
                DateTime ValidHIDate = (DateTime)CurrentRegistration.HealthInsurance.ValidDateTo;
                TimeSpan AfterEquals = CheckDate.Subtract(ValidHIDate);
                if (AfterEquals.Days > 0)
                {
                    if (MessageBox.Show("Số ngày cấp toa không được vượt quá " + (SelNDay - AfterEquals.Days) +" ngày. Lý do bảo hiểm y tế hết hạn."
                       , eHCMSResources.Z1359_G1_Warning, MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}