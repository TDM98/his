using eHCMSLanguage;
using aEMR.ServiceClient;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using System;
using System.Windows;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPrescriptionNoteTemplates_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionNoteTemplates_AddEditViewModel : ViewModelBase, IPrescriptionNoteTemplates_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PrescriptionNoteTemplates_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        public override bool IsProcessing
        {
            get
            {
                return _IsWaitingSaving;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_IsWaitingSaving)
                {
                    return eHCMSResources.Z0343_G1_DangLuu;
                }
                return string.Empty;
            }
        }

        private bool _IsWaitingSaving;
        public bool IsWaitingSaving
        {
            get { return _IsWaitingSaving; }
            set
            {
                if (_IsWaitingSaving != value)
                {
                    _IsWaitingSaving = value;
                    NotifyOfPropertyChange(() => IsWaitingSaving);
                    NotifyWhenBusy();
                }
            }
        }

       
        private PrescriptionNoteTemplates _ObjPrescriptionNoteTemplates_Current;
        public PrescriptionNoteTemplates ObjPrescriptionNoteTemplates_Current
        {
            get { return _ObjPrescriptionNoteTemplates_Current; }
            set
            {
                _ObjPrescriptionNoteTemplates_Current = value;
                NotifyOfPropertyChange(() => _ObjPrescriptionNoteTemplates_Current);
            }
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        public void btSave()
        {
            if(CheckValid()==false)
                return; 

            PrescriptionNoteTemplates_Save();
        }

        private bool CheckValid()
        {
            if(string.IsNullOrEmpty(ObjPrescriptionNoteTemplates_Current.NoteDetails))
            {
                MessageBox.Show(eHCMSResources.A0871_G1_Msg_InfoNhapLoiDan);
                return false;
            }
            return true;
        }

        private void PrescriptionNoteTemplates_Save()
        {
            int Action = 1;
            if(ObjPrescriptionNoteTemplates_Current.PrescriptNoteTemplateID<=0)
            {
                Action = 0;
            }

            var t = new Thread(() =>
            {
                IsWaitingSaving = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPrescriptionNoteTemplates_Save(ObjPrescriptionNoteTemplates_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            string Result = "";
                            contract.EndPrescriptionNoteTemplates_Save(out Result, asyncResult);

                            long IDNew=0;

                            long.TryParse(Result, out IDNew);

                            if(IDNew>0)
                            {
                                if (Action == 0)
                                {
                                    Globals.EventAggregator.Publish(new ReLoadDataAfterCUD());
                                    MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                    InitializeNewItem();
                                }
                                else
                                {
                                    Globals.EventAggregator.Publish(new ReLoadDataAfterCUD());
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                }
                            }
                            else
                            {
                                MessageBox.Show(Result);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsWaitingSaving = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void InitializeNewItem()
        {
            ObjPrescriptionNoteTemplates_Current=new PrescriptionNoteTemplates();
            ObjPrescriptionNoteTemplates_Current.PrescriptNoteTemplateID = 0;
            ObjPrescriptionNoteTemplates_Current.NoteDetails = "";
            ObjPrescriptionNoteTemplates_Current.IsActive = true;
        }

        public void btClose()
        {
            TryClose();
        }
    }
}
