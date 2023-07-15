using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
/*
 * 20191106 #001 TNHX: [BM 0013306]: separate V_MedProductType
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IBidEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BidEditViewModel : Conductor<object>, IBidEdit
    {
        #region Properties
        private Bid _ObjBid = new Bid();
        public Bid ObjBid
        {
            get
            {
                return _ObjBid;
            }
            set
            {
                _ObjBid = value;
                NotifyOfPropertyChange(() => ObjBid);
            }
        }
        #endregion
        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            if (ObjBid == null)
                ObjBid = new Bid();
        }

        public void btnAddBid()
        {
            if (ObjBid == null) return;
            if (string.IsNullOrEmpty(ObjBid.BidName))
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2736_G1_ThongTinThauKoHopLe);
                return;
            }
            if (string.IsNullOrEmpty(ObjBid.PermitNumber))
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2736_G1_SoQDThauKhongHopLe);
                return;
            }
            //▼====: #001
            if (ObjBid.ValidDateFrom > ObjBid.ValidDateTo)
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.A0857_G1_Msg_InfoNgThangKhHopLe2);
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginEditDrugBid(ObjBid, Globals.DispatchCallback((asyncResult) =>
                        {
                            //▲====: #001
                            try
                            {
                                long? BidIDOut;
                                var mBidDetails = contract.EndEditDrugBid(out BidIDOut, asyncResult);
                                if (mBidDetails)
                                {
                                    if (ObjBid.BidID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    ObjBid.BidID = BidIDOut.GetValueOrDefault(0);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        public bool IsShowYear
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.ApplyReport130;
            }
        }
        #endregion
    }
}
