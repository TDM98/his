using aEMR.Common.BaseModel;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
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
using System.ServiceModel;
using System.Threading;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPromoDiscountProgramCollection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PromoDiscountProgramCollectionViewModel : ViewModelBase, IPromoDiscountProgramCollection
    {
        #region Properties
        public long PtRegistrationID { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsChoosed { get; set; }
        public long V_RegistrationType { get; set; }
        private ObservableCollection<PromoDiscountProgram> _DiscountProgramCollection;
        private PromoDiscountProgram _SelectedPromoDiscountProgram;
        public ObservableCollection<PromoDiscountProgram> DiscountProgramCollection
        {
            get
            {
                return _DiscountProgramCollection;
            }
            set
            {
                _DiscountProgramCollection = value;
                NotifyOfPropertyChange(() => DiscountProgramCollection);
            }
        }
        public PromoDiscountProgram SelectedPromoDiscountProgram
        {
            get
            {
                return _SelectedPromoDiscountProgram;
            }
            set
            {
                _SelectedPromoDiscountProgram = value;
                NotifyOfPropertyChange(() => SelectedPromoDiscountProgram);
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public PromoDiscountProgramCollectionViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
        }
        public void btnCreateNew()
        {
            IPromoDiscountProgramEdit mPromoDiscountProgramEdit = Globals.GetViewModel<IPromoDiscountProgramEdit>();
            mPromoDiscountProgramEdit.PromoDiscountProgramObj = new PromoDiscountProgram { RecCreatedDate = Globals.GetCurServerDateTime(), V_RegistrationType = V_RegistrationType };
            mPromoDiscountProgramEdit.PtRegistrationID = PtRegistrationID;
            GlobalsNAV.ShowDialog_V3<IPromoDiscountProgramEdit>(mPromoDiscountProgramEdit);
            if (mPromoDiscountProgramEdit.IsUpdated)
            {
                if (mPromoDiscountProgramEdit.PromoDiscountProgramObj == null)
                {
                    return;
                }
                IsUpdated = true;
                if (PtRegistrationID == 0)
                {
                    DiscountProgramCollection = new ObservableCollection<PromoDiscountProgram> { mPromoDiscountProgramEdit.PromoDiscountProgramObj };
                }
                else
                {
                    DiscountProgramCollection.Add(mPromoDiscountProgramEdit.PromoDiscountProgramObj);
                }
            }
        }
        public void btnClose()
        {
            TryClose();
        }
        public void btnSave()
        {
            IsChoosed = true;
            TryClose();
        }
        public void gvPromoDiscountProgram_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnSave();
        }
        #endregion
        public void GetAllExamptions()
        {
            //DiscountProgramCollection.Clear();
            if (Globals.allExemptions != null && Globals.allExemptions.Count > 0)
            {
                foreach (var item in Globals.allExemptions.Where(x => x.V_RegistrationType == V_RegistrationType || x.V_RegistrationType == 0))
                {
                    DiscountProgramCollection.Add(item);
                }
                IsUpdated = DiscountProgramCollection.Count > 0;
                return;
            }

            this.DlgShowBusyIndicator("Danh sách miễn giảm");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetAllExemptions(Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            List<PromoDiscountProgram> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetAllExemptions(asyncResult);
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


                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    Globals.allExemptions = allItems;
                                    foreach (var item in allItems.Where(x => x.V_RegistrationType == V_RegistrationType || x.V_RegistrationType == 0))
                                    {
                                        DiscountProgramCollection.Add(item);
                                    }
                                    IsUpdated = DiscountProgramCollection.Count > 0;
                                    //foreach (var item in DiscountProgramCollection)
                                    //{
                                    //    GetPromoDiscountItems_ByID(item);
                                    //}
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

        public void GetPromoDiscountItems_ByID(PromoDiscountProgram Obj)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPromoDiscountItems_ByID(Obj.PromoDiscProgID, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            List<PromoDiscountItems> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetPromoDiscountItems_ByID(asyncResult);
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


                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    Obj.PromoDiscountItems = new List<PromoDiscountItems>();
                                    foreach (var item in allItems)
                                    {
                                        Obj.PromoDiscountItems.Add(item);
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
    }
}