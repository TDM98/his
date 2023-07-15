
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.ServiceClient;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IUpdateDiseaseProgressionView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UpdateDiseaseProgressionViewModel : ViewModelBase, IUpdateDiseaseProgressionView
    {
        public UpdateDiseaseProgressionViewModel()
        {
            Globals.EventAggregator.Subscribe(this);
        }
        private DiagnosisTreatment _DiagTrmtItem;
        public DiagnosisTreatment DiagTrmtItem
        {
            get { return _DiagTrmtItem; }
            set
            {
                if (_DiagTrmtItem != value)
                {
                    _DiagTrmtItem = value;
                    NotifyOfPropertyChange(() => DiagTrmtItem);
                }
            }
        }
        private bool _IsUpdate;
        public bool IsUpdate
        {
            get { return _IsUpdate; }
            set
            {
                if (_IsUpdate != value)
                {
                    _IsUpdate = value;
                    NotifyOfPropertyChange(() => IsUpdate);
                }
            }
        }
        private bool _UpdateOK = false;
        public bool UpdateOK
        {
            get { return _UpdateOK; }
            set
            {
                if (_UpdateOK != value)
                {
                    _UpdateOK = value;
                    NotifyOfPropertyChange(() => UpdateOK);
                }
            }
        }
        public void btnSave()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateDiseaseProgression(DiagTrmtItem, IsUpdate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndUpdateDiseaseProgression(asyncResult))
                            {
                                MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
                                UpdateOK = true;
                                this.TryClose();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
    }
}
