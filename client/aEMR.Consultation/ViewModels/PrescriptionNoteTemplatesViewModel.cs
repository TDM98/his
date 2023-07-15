using System.Collections.Generic;
using aEMR.ServiceClient;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using System.Windows;
using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPrescriptionNoteTemplates)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionNoteTemplatesViewModel : ViewModelBase, IPrescriptionNoteTemplates
        , IHandle<ReLoadDataAfterCUD>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public override bool IsProcessing
        {
            get
            {
                return _IsWaitingPrescriptionNoteTemplates_GetAll
                    ||_IsWaitingDelete;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_IsWaitingPrescriptionNoteTemplates_GetAll)
                {
                    return eHCMSResources.Z0491_G1_LoadDanhSach;
                }
                if(_IsWaitingDelete)
                {
                    return eHCMSResources.Z0492_G1_DangXoa;
                }
                return string.Empty;
            }
        }

        private bool _IsWaitingPrescriptionNoteTemplates_GetAll;
        public bool IsWaitingPrescriptionNoteTemplates_GetAll
        {
            get { return _IsWaitingPrescriptionNoteTemplates_GetAll; }
            set
            {
                if (_IsWaitingPrescriptionNoteTemplates_GetAll != value)
                {
                    _IsWaitingPrescriptionNoteTemplates_GetAll = value;
                    NotifyOfPropertyChange(() => IsWaitingPrescriptionNoteTemplates_GetAll);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingDelete;
        public bool IsWaitingDelete
        {
            get { return _IsWaitingDelete; }
            set
            {
                if (_IsWaitingDelete != value)
                {
                    _IsWaitingDelete = value;
                    NotifyOfPropertyChange(() => IsWaitingDelete);
                    NotifyWhenBusy();
                }
            }
        }

        [ImportingConstructor]
        public PrescriptionNoteTemplatesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            //Globals.EventAggregator.Subscribe(this);

            PrescriptionNoteTemplates_GetAll();
        }

        private ObservableCollection<PrescriptionNoteTemplates> _ObjPrescriptionNoteTemplates_GetAll;
        public ObservableCollection<PrescriptionNoteTemplates> ObjPrescriptionNoteTemplates_GetAll
        {
            get { return _ObjPrescriptionNoteTemplates_GetAll; }
            set
            {
                _ObjPrescriptionNoteTemplates_GetAll = value;
                NotifyOfPropertyChange(() => ObjPrescriptionNoteTemplates_GetAll);
            }
        }

        public void PrescriptionNoteTemplates_GetAll()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPrescriptionNoteTemplates_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAll(asyncResult);

                                ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);
                                
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }


        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<IPrescriptionNoteTemplates_AddEdit>();
                //typeInfo.ObjPrescriptionNoteTemplates_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PrescriptionNoteTemplates));

                //typeInfo.TitleForm = string.Format("{0} ",eHCMSResources.Z0480_G1_HieuChinhMau) + typeInfo.ObjPrescriptionNoteTemplates_Current.PrescriptNoteTemplateID.ToString(); 
                
                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<IPrescriptionNoteTemplates_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjPrescriptionNoteTemplates_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.PrescriptionNoteTemplates));

                    typeInfo.TitleForm = string.Format("{0} ", eHCMSResources.Z0480_G1_HieuChinhMau) + typeInfo.ObjPrescriptionNoteTemplates_Current.PrescriptNoteTemplateID.ToString();
                };
                GlobalsNAV.ShowDialog<IPrescriptionNoteTemplates_AddEdit>(onInitDlg);
            }
        }


        public void hplAddNew_Click()
        {
            //var typeInfo = Globals.GetViewModel<IPrescriptionNoteTemplates_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.Z0481_G1_ThemMoiMau;
            //typeInfo.InitializeNewItem();

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPrescriptionNoteTemplates_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.Z0481_G1_ThemMoiMau;
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog<IPrescriptionNoteTemplates_AddEdit>(onInitDlg);
        }



        public void hplDelete_Click(object selectedItem)
        {
            PrescriptionNoteTemplates p = (selectedItem as PrescriptionNoteTemplates);
            if (p != null && p.PrescriptNoteTemplateID > 0)
            {
                if (MessageBox.Show(string.Format("{0}: ",eHCMSResources.A0139_G1_Msg_ConfNgungHoatDongMau) + p.PrescriptNoteTemplateID.ToString() + string.Format(", {0}",eHCMSResources.Z0355_G1_NayKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    p.IsActive = false;
                    PrescriptionNoteTemplates_Delete(p);
                }
            }
        }


        private void PrescriptionNoteTemplates_Delete(PrescriptionNoteTemplates p)
        {
            var t = new Thread(() =>
            {
                IsWaitingDelete = true;

                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPrescriptionNoteTemplates_Save(p, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            string Result = "";
                            contract.EndPrescriptionNoteTemplates_Save(out Result, asyncResult);

                            long IDNew = 0;

                            long.TryParse(Result, out IDNew);

                            if (IDNew > 0)
                            {
                                PrescriptionNoteTemplates_GetAll();
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
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
                            IsWaitingDelete = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void Handle(ReLoadDataAfterCUD message)
        {
            if(message!=null && this.GetView()!=null)
            {
                PrescriptionNoteTemplates_GetAll();
            }
        }
    }
}
