using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(ICheckedValidHICard)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CheckedValidHICardViewModel : ViewModelBase, ICheckedValidHICard
    {
        [ImportingConstructor]
        public CheckedValidHICardViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            if ((GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.APIKey == null || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token)))
            {
                GlobalsNAV.LoginHIAPI();
            }
        }
        #region Properties
        private string HIAPICheckHICardAddress = "http://egw.baohiemxahoi.gov.vn/api/egw/KQNhanLichSuKCB2019?token={0}&id_token={1}&username={2}&password={3}";
        private HealthInsurance _gHealthInsurance;
        private HIAPICheckedHICard _HIAPICheckedHICard;
        public HealthInsurance gHealthInsurance
        {
            get => _gHealthInsurance; set
            {
                _gHealthInsurance = value;
                NotifyOfPropertyChange(() => gHealthInsurance);
            }
        }
        public HIAPICheckedHICard HIAPICheckedHICard
        {
            get => _HIAPICheckedHICard; set
            {
                _HIAPICheckedHICard = value;
                NotifyOfPropertyChange(() => HIAPICheckedHICard);
            }
        }
        #endregion
        #region Methods
        private void GetHICardInfo()
        {
            string mHIAPICheckHICardAddress = string.Format(HIAPICheckHICardAddress, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password);
            string mHIData = string.Format("{{\"maThe\":\"{0}\",\"hoTen\":\"{1}\",\"ngaySinh\":\"{2}\"}}", gHealthInsurance.HICardNo.ToUpper(), gHealthInsurance.Patient.FullName.ToUpper(), gHealthInsurance.Patient.DOB.HasValue && gHealthInsurance.Patient.DOB != null && gHealthInsurance.Patient.DOB.Value.Day == 1 && gHealthInsurance.Patient.DOB.Value.Month == 1 ? gHealthInsurance.Patient.DOB.Value.Year.ToString() : gHealthInsurance.Patient.DOBText);
            string mRestJson = GlobalsNAV.GetRESTServiceJSon(mHIAPICheckHICardAddress, mHIData);
            HIAPICheckedHICard = GlobalsNAV.ConvertJsonToObject<HIAPICheckedHICard>(mRestJson);
        }
        #endregion
        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            if (GlobalsNAV.gLoggedHIAPIUser.maKetQua != 200) return;
            GetHICardInfo();
            if (HIAPICheckedHICard.maKetQua == 401)
            {
                GlobalsNAV.LoginHIAPI();
                GetHICardInfo();
            }
        }
        public void btnSave()
        {
            if (HIAPICheckedHICard.maKetQua != 0) return;
            Globals.EventAggregator.Publish(new ItemSelected<HIAPICheckedHICard> { Source = this, Item = HIAPICheckedHICard });
            TryClose();
        }
        #endregion
    }
}