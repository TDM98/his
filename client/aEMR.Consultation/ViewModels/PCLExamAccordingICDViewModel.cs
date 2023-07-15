using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using aEMR.CommonTasks;
using eHCMSLanguage;
using System.Linq;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Common.BaseModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using aEMR.Common.HotKeyManagement;
using aEMR.Common.ViewModels;
using aEMR.ServiceClient.Consultation_PCLs;
using System.ServiceModel;
using aEMR.DataContracts;
using aEMR.Common.Converters;
using System.Text;
using aEMR.ViewContracts.Consultation_ePrescription;
/*
* 
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPCLExamAccordingICD)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamAccordingICDViewModel : ViewModelBase, IPCLExamAccordingICD
    {
        [ImportingConstructor]
        public PCLExamAccordingICDViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            Authorization();
        }

        #region Properties
        public List<PCLExamAccordingICD> ListPCLExamAccordingICD { get; set; }
        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    SetAllAppointment();
                }
            }
        }
        #endregion

        #region Events
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
        #endregion
        #region Methods
        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        public void SetAllAppointment()
        {
            if (ListPCLExamAccordingICD == null || ListPCLExamAccordingICD.Count <= 0)
            {
                return;
            }

            foreach (var item in ListPCLExamAccordingICD)
            {
                if (AllChecked && !item.IsChecked && item.PCLExamType.IsUsed && (bool)item.PCLExamType.IsActive)
                {
                    item.IsChecked = true;
                }
                else
                {
                    item.IsChecked = false;
                }
            }
        }
        public void btChoCLS()
        {
            if (ListPCLExamAccordingICD.Where(x => x.IsChecked == true).ToList().Count == 0)
            {
                MessageBox.Show("Chưa chọn cận lâm sàng");
                return;
            }
            List<PCLExamType> examTypes = new List<PCLExamType>();
            foreach (var item in ListPCLExamAccordingICD.Where(x => x.IsChecked == true))
            {
                examTypes.Add(item.PCLExamType);
            }
            Globals.EventAggregator.Publish(new PCLExamAccordingICD_Event { ListPCLExamAccordingICD = examTypes });
            this.TryClose();
        }
        #endregion

        public void btCancel()
        {
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }
    }
}
