using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using ResourcesManagementProxy;
using aEMR.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
/*
 * #001: 20180904 TTM:  Thay đổi cách lấy danh sách Staff theo Category từ chạy đi chạy về database -> lấy từ Globals.
 *                      Vì danh sách Staff đã được lấy lên lúc log-in không có lý do gì phải lấy lại từng lần theo Category như vậy.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof (ISieuAmTT_SauSinh_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_SauSinhViewModel : Conductor<object>, ISieuAmTT_SauSinh_Consultation
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTT_SauSinhViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            //Globals.EventAggregator.Subscribe(this);

            if (Globals.PatientPCLReqID_Imaging > 0)
            {
                PatientPCLReqID = Globals.PatientPCLReqID_Imaging;
                LoadInfo();
            }
        }


        private long _PatientPCLReqID;
        public long PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    _PatientPCLReqID = value;
                    NotifyOfPropertyChange(() => PatientPCLReqID);
                }
            }
        }


        #region properties

        private ObservableCollection<Staff> _allStaff;
        private UltraResParams_FetalEchocardiographyPostpartum _curURP_FEPostpartum;

        public UltraResParams_FetalEchocardiographyPostpartum curURP_FEPostpartum
        {
            get { return _curURP_FEPostpartum; }
            set
            {
                if (_curURP_FEPostpartum == value)
                    return;
                _curURP_FEPostpartum = value;
                NotifyOfPropertyChange(() => curURP_FEPostpartum);
            }
        }

        public ObservableCollection<Staff> allStaff
        {
            get { return _allStaff; }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
            }
        }

        #endregion

        #region method

        private void GetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi});
            var t = new Thread(() =>
                                   {
                                       using (var serviceFactory = new PCLsClient())
                                       {
                                           IPCLs contract = serviceFactory.ServiceInstance;
                                           contract.BeginGetUltraResParams_FetalEchocardiographyPostpartumByID(UltraResParams_FetalEchocardiographyPostpartumID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                                                                                              {
                                                                                                  try
                                                                                                  {
                                                                                                      var item=contract.EndGetUltraResParams_FetalEchocardiographyPostpartumByID(asyncResult);
                                                                                                      if(item!=null)
                                                                                                      {
                                                                                                          curURP_FEPostpartum=item;
                                                                                                      }
                                                                                                  }
                                                                                                  catch (Exception ex)
                                                                                                  {
                                                                                                      MessageBox.Show(ex.Message);
                                                                                                  }
                                                                                                  finally
                                                                                                  {
                                                                                                      Globals.IsBusy =false;
                                                                                                  }
                                                                                              }), null);
                                       }
                                   });

            t.Start();
        }



        


        #endregion

        public void LoadInfo()
        {
            //▼====== #001
            GetStaffCategoriesByType();
            //▲====== #001
            //GetStaffCategoriesByType((long) AllLookupValues.StaffCatType.BAC_SI);
            //CheckHasPCLImageID();
            //CheckSave();
            //_tempURP_FEPostpartum = new UltraResParams_FetalEchocardiographyPostpartum();
            curURP_FEPostpartum = new UltraResParams_FetalEchocardiographyPostpartum();
            GetUltraResParams_FetalEchocardiographyPostpartum(0,PatientPCLReqID);
          //  GetPCLExamResultTemplateListByTypeID(3, (int) AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI);
        }
        //▼====== #001
        private void GetStaffCategoriesByType()
        {
            allStaff = new ObservableCollection<Staff>();
            foreach (var item in Globals.AllStaffs)
            {
                if (item.StaffCatgID == (long)StaffCatg.Bs ||
                    item.StaffCatgID == (long)StaffCatg.BsDongY ||
                    item.StaffCatgID == (long)StaffCatg.BsRHM ||
                    item.StaffCatgID == (long)StaffCatg.BsSan ||
                    item.StaffCatgID == (long)StaffCatg.BsTMH ||
                    item.StaffCatgID == (long)StaffCatg.ThS_BS ||
                    item.StaffCatgID == (long)StaffCatg.TS_BS)
                {
                    allStaff.Add(item);
                }
            }
            NotifyOfPropertyChange(() => allStaff);
        }
    }
    //private void GetStaffCategoriesByType(long V_StaffCatType)
    //{
    //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
    //    var t = new Thread(() =>
    //    {
    //        using (var serviceFactory = new ResourcesManagementServiceClient())
    //        {
    //            IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //            contract.BeginGetRefStaffCategoriesByType(V_StaffCatType,
    //                                                      Globals.DispatchCallback(
    //                                                          (asyncResult) =>
    //                                                          {
    //                                                              try
    //                                                              {
    //                                                                  List
    //                                                                                                     <
    //                                                                                                         RefStaffCategory
    //                                                                                                         > results =
    //                                                                                                             contract
    //                                                                                                                 .
    //                                                                                                                 EndGetRefStaffCategoriesByType
    //                                                                                                                 (asyncResult);
    //                                                                  if (results != null &&
    //                                                                                                     results.Count >
    //                                                                                                     0)
    //                                                                  {
    //                                                                      allStaff =
    //                                                                                                         new ObservableCollection
    //                                                                                                             <Staff>
    //                                                                                                             ();
    //                                                                      foreach (
    //                                                                                                         RefStaffCategory
    //                                                                                                             p in
    //                                                                                                             results
    //                                                                                                         )
    //                                                                      {
    //                                                                          GetAllStaff
    //                                                                                                             (p.
    //                                                                                                                  StaffCatgID);
    //                                                                      }
    //                                                                      NotifyOfPropertyChange
    //                                                                                                         (() =>
    //                                                                                                          allStaff);
    //                                                                  }
    //                                                              }
    //                                                              catch (Exception ex)
    //                                                              {
    //                                                                  MessageBox.Show(ex.Message);
    //                                                              }
    //                                                              finally
    //                                                              {
    //                                                                  Globals.IsBusy = false;
    //                                                              }
    //                                                          }), null);
    //        }
    //    });

    //    t.Start();
    //}

    //private void GetAllStaff(long StaffCatgID)
    //{
    //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
    //    var t = new Thread(() =>
    //    {
    //        using (var serviceFactory = new ResourcesManagementServiceClient())
    //        {
    //            IResourcesManagementService contract = serviceFactory.ServiceInstance;
    //            contract.BeginGetAllStaff(StaffCatgID, Globals.DispatchCallback((asyncResult) =>
    //            {
    //                try
    //                {
    //                    List<Staff> results = contract.EndGetAllStaff(asyncResult);
    //                    if (results != null && results.Count > 0)
    //                    {
    //                        foreach (Staff p in results)
    //                        {
    //                            allStaff.Add(p);
    //                        }
    //                        NotifyOfPropertyChange(() => allStaff);
    //                    }
    //                }
    //                catch (Exception ex)
    //                {
    //                    MessageBox.Show(ex.Message);
    //                }
    //                finally
    //                {
    //                    Globals.IsBusy = false;
    //                }
    //            }), null);
    //        }
    //    });

    //    t.Start();
    //}
    //▲====== #001
}
