using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using aEMR.Controls;
using aEMR.Common;
using aEMR.Common.Utilities;
using Castle.Windsor;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(ISieuAmResultTemplate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmResultTemplateViewModel : Conductor<object>, ISieuAmResultTemplate
    {
        //private ObservableCollection<AllLookupValues.PCLResultParamImpID> _allPCLResultParamImpID;
        #region properties

        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        private PCLExamParamResult _curPCLExamParamResult;
        private PCLExamResultTemplate _curPCLExamResultTemplate;
        private bool _isSave;
        private bool _isUpdate;

        private ObservableCollection<PCLExamParamResult> _lstPCLExamParamResult;

        private ObservableCollection<PCLExamResultTemplate> _lstPCLExamResultTemplate;
        private bool isHasPatient;

        public PCLExamParamResult curPCLExamParamResult
        {
            get { return _curPCLExamParamResult; }
            set
            {
                if (_curPCLExamParamResult == value)
                    return;
                _curPCLExamParamResult = value;
                NotifyOfPropertyChange(() => curPCLExamParamResult);
            }
        }

        public ObservableCollection<PCLExamParamResult> lstPCLExamParamResult
        {
            get { return _lstPCLExamParamResult; }
            set
            {
                if (_lstPCLExamParamResult == value)
                    return;
                _lstPCLExamParamResult = value;
                NotifyOfPropertyChange(() => lstPCLExamParamResult);
            }
        }

        public ObservableCollection<PCLExamResultTemplate> lstPCLExamResultTemplate
        {
            get { return _lstPCLExamResultTemplate; }
            set
            {
                if (_lstPCLExamResultTemplate == value)
                    return;
                _lstPCLExamResultTemplate = value;
                NotifyOfPropertyChange(() => lstPCLExamResultTemplate);
            }
        }

        public PCLExamResultTemplate curPCLExamResultTemplate
        {
            get { return _curPCLExamResultTemplate; }
            set
            {
                if (_curPCLExamResultTemplate == value)
                    return;
                _curPCLExamResultTemplate = value;
                NotifyOfPropertyChange(() => curPCLExamResultTemplate);
            }
        }

        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get { return _allRefStaffCategory; }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }


        public bool IsHasPatient
        {
            get { return isHasPatient; }
            set
            {
                if (isHasPatient == value)
                    return;
                isHasPatient = value;
                NotifyOfPropertyChange(() => isHasPatient);
            }
        }

        public bool isSave
        {
            get { return _isSave; }
            set
            {
                if (_isSave == value)
                    return;
                _isSave = value;
                NotifyOfPropertyChange(() => isSave);
                //_isUpdate= !isSave;
            }
        }

        public bool isUpdate
        {
            get { return _isUpdate; }
            set
            {
                if (_isUpdate == value)
                    return;
                _isUpdate = value;
                NotifyOfPropertyChange(() => isUpdate);
                //_isSave = !isUpdate;
            }
        }

        public string GroupName { get; set; }

        public string PCLExamTemplateName { get; set; }

        private EnumDescription selectPCLResultParamImpID;

        public EnumDescription SelectPCLResultParamImpID
        {
            get
            {
                return selectPCLResultParamImpID;
            }
            set
            {
                selectPCLResultParamImpID = value;
                NotifyOfPropertyChange(() => SelectPCLResultParamImpID);
            }
        }

        private ObservableCollection<EnumDescription> allPCLResultParamImpID;

        public ObservableCollection<EnumDescription> AllPCLResultParamImpID
        {
            get
            {
                return allPCLResultParamImpID;
            }
            set
            {
                allPCLResultParamImpID = value;
                NotifyOfPropertyChange(() => AllPCLResultParamImpID);
            }
        }

        //private enum _enumType = AllLookupValues.PCLResultParamImpID;
        //public enum EnumType
        //{
        //    get { return _enumType; }
        //    set
        //    {
        //        _enumType = value;
        //        NotifyOfPropertyChange(() => EnumType);
        //    }
        //}
        #endregion

        #region fuction

        public void GetPclExamType()
        {
            //allPCLResultParamImpID = new ObservableCollection<AllLookupValues.PCLResultParamImpID>();
            //for (int i = 1; i < (int)AllLookupValues.PCLResultParamImpID.count; i++)
            //{
            //    allPCLResultParamImpID.Add((AllLookupValues.PCLResultParamImpID)i);
            //}


            //KMx: The following code is copied from class EnumListingViewModel, function LoadData(). When I have time, I will combine them (13/11/2016 14:19).
            AllPCLResultParamImpID = new ObservableCollection<EnumDescription>();

            Array enumValues = Enum.GetValues(typeof(AllLookupValues.PCLResultParamImpID));

            if (enumValues != null)
            {
                foreach (Enum enumValue in enumValues)
                {
                    var obj = new EnumDescription();
                    obj.Description = Helpers.GetEnumDescription(enumValue);
                    obj.EnumItem = enumValue;
                    obj.EnumValue = enumValue.ToString();
                    obj.EnumIntValue = Convert.ToInt32(enumValue);
                    AllPCLResultParamImpID.Add(obj);
                }
            }
        }

        public void cboVType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //curPCLExamParamResult.ParamEnum = (int)((AxComboBox)sender).SelectedItem;
            curPCLExamParamResult.ParamEnum = SelectPCLResultParamImpID.EnumIntValue;
            GetPCLExamParamResultList(0, curPCLExamParamResult.ParamEnum);
        }

        public void cboVGroup_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((AxComboBox)sender).SelectedItem == null)
            {
                return;
            }
            curPCLExamResultTemplate.PCLExamGroupTemplateResultID =
                Convert.ToInt32(((PCLExamParamResult)((AxComboBox)sender).SelectedItem).PCLExamResultID);
            //GetPCLExamParamResultList(0, curPCLExamParamResult.ParamEnum);
            GetPCLExamResultTemplateList(curPCLExamResultTemplate.PCLExamGroupTemplateResultID);
        }

        public void cboVTemplate_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (((AxComboBox)sender).SelectedItem == null)
            {
                return;
            }
            curPCLExamResultTemplate.PCLExamResultTemplateID =
                ((PCLExamResultTemplate)((AxComboBox)sender).SelectedItem).PCLExamResultTemplateID;
            GetPCLExamResultTemplate(curPCLExamResultTemplate.PCLExamResultTemplateID);
        }

        public void butSaveGroup()
        {
            if (curPCLExamParamResult.ParamEnum < 1
                || GroupName == "")
            {
                Globals.ShowMessage("Dữ liệu không hợp lệ", "");
                return;
            }
            curPCLExamParamResult.GroupName = GroupName;
            AddPCLExamParamResult(curPCLExamParamResult);
        }

        public void butSaveTemp()
        {
            if (curPCLExamResultTemplate.PCLExamGroupTemplateResultID < 1
                || PCLExamTemplateName == "")
            {
                Globals.ShowMessage("Dữ liệu không hợp lệ", "");
                return;
            }
            curPCLExamResultTemplate.PCLExamTemplateName = PCLExamTemplateName;
            AddPCLExamResultTemplate(curPCLExamResultTemplate);
        }

        public void butUpdate()
        {
            if (curPCLExamResultTemplate.PCLExamGroupTemplateResultID < 1
                || curPCLExamResultTemplate.ResultContent == "")
            {
                Globals.ShowMessage("Dữ liệu không hợp lệ", "");
                return;
            }
            UpdatePCLExamResultTemplate(curPCLExamResultTemplate);
        }

        public void CheckHasPCLImageID()
        {
            isHasPatient = true;
            //if()
            //{
            //    isHasPatient = true;
            //}else
            //{
            //    isHasPatient = false;
            //}
        }

        public void CheckSave()
        {
            isSave = true;
            isUpdate = false;
        }

        public void CheckUpdate()
        {
            isSave = false;
            isUpdate = true;
        }

        public void butReset()
        {
            curPCLExamParamResult = new PCLExamParamResult();
            //curPCLExamParamResult = tempPCLExamParamResult;
            NotifyOfPropertyChange(() => curPCLExamParamResult);
        }

        public void butSave()
        {
            try
            {
                if (curPCLExamParamResult != null)
                {
                    AddPCLExamParamResult(curPCLExamParamResult);
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }

        public void butUpdateNameGroup()
        {
            if (curPCLExamParamResult != null &&
                curPCLExamParamResult.GroupName != "")
            {
                UpdatePCLExamParamResult(curPCLExamParamResult);
            }
        }

        public void butUpdateTemplateName()
        {
            if (curPCLExamResultTemplate != null &&
                curPCLExamResultTemplate.PCLExamTemplateName != "")
            {
                UpdatePCLExamResultTemplate(curPCLExamResultTemplate);
            }
        }

        #endregion

        #region method

        private void AddPCLExamParamResult(PCLExamParamResult entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginAddPCLExamParamResult(entity,
                                                        Globals.DispatchCallback((asyncResult) =>
                                                        {
                                                            try
                                                            {
                                                                bool
                                                                                           res
                                                                                               =
                                                                                               contract
                                                                                                   .
                                                                                                   EndAddPCLExamParamResult
                                                                                                   (asyncResult);
                                                                if (
                                                                                           res)
                                                                {
                                                                    Globals
                                                                                               .
                                                                                               ShowMessage
                                                                                               ("Đã lưu!",
                                                                                                "");
                                                                }
                                                                else
                                                                {
                                                                    Globals
                                                                                               .
                                                                                               ShowMessage
                                                                                               ("Lưu không thành công!",
                                                                                                "");
                                                                }
                                                            }
                                                            catch (
                                                                                       Exception
                                                                                           ex
                                                                                       )
                                                            {
                                                                Globals
                                                                                           .
                                                                                           ShowMessage
                                                                                           (ex
                                                                                                .
                                                                                                Message,
                                                                                            eHCMSResources.T0432_G1_Error);
                                                            }
                                                            finally
                                                            {
                                                                Globals
                                                                                           .
                                                                                           IsBusy
                                                                                           =
                                                                                           false;
                                                            }
                                                        }), null);
                }
            });

            t.Start();
        }

        private void AddPCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginAddPCLExamResultTemplate(entity,
                                                           Globals.DispatchCallback(
                                                               (asyncResult) =>
                                                               {
                                                                   try
                                                                   {
                                                                       bool res =
                                                                                                  contract.
                                                                                                      EndAddPCLExamResultTemplate
                                                                                                      (asyncResult);
                                                                       if (res)
                                                                       {
                                                                           Globals.
                                                                                                      ShowMessage(
                                                                                                          "Đã lưu!",
                                                                                                          "");
                                                                       }
                                                                       else
                                                                       {
                                                                           Globals.
                                                                                                      ShowMessage(
                                                                                                          "Lưu không thành công!",
                                                                                                          "");
                                                                       }
                                                                   }
                                                                   catch (Exception ex)
                                                                   {
                                                                       Globals.ShowMessage(
                                                                                                  ex.Message,
                                                                                                  eHCMSResources.T0432_G1_Error);
                                                                   }
                                                                   finally
                                                                   {
                                                                       Globals.IsBusy = false;
                                                                   }
                                                               }), null);
                }
            });

            t.Start();
        }

        private void UpdatePCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdatePCLExamResultTemplate(entity,
                                                              Globals.DispatchCallback(
                                                                  (asyncResult) =>
                                                                  {
                                                                      try
                                                                      {
                                                                          bool res =
                                                                                                     contract.
                                                                                                         EndUpdatePCLExamResultTemplate
                                                                                                         (asyncResult);
                                                                          if (res)
                                                                          {
                                                                              Globals.
                                                                                                         ShowMessage
                                                                                                         ("Đã lưu!",
                                                                                                          "");
                                                                          }
                                                                          else
                                                                          {
                                                                              Globals.
                                                                                                         ShowMessage
                                                                                                         ("Lưu không thành công!",
                                                                                                          "");
                                                                          }
                                                                      }
                                                                      catch (Exception ex)
                                                                      {
                                                                          Globals.ShowMessage
                                                                                                     (ex.Message,
                                                                                                      eHCMSResources.T0432_G1_Error);
                                                                      }
                                                                      finally
                                                                      {
                                                                          Globals.IsBusy =
                                                                                                     false;
                                                                      }
                                                                  }), null);
                }
            });

            t.Start();
        }

        private void UpdatePCLExamParamResult(PCLExamParamResult entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdatePCLExamParamResult(entity,
                                                           Globals.DispatchCallback(
                                                               (asyncResult) =>
                                                               {
                                                                   try
                                                                   {
                                                                       bool res =
                                                                                                  contract.
                                                                                                      EndUpdatePCLExamParamResult
                                                                                                      (asyncResult);
                                                                       if (res)
                                                                       {
                                                                           Globals.
                                                                                                      ShowMessage(
                                                                                                          "Đã lưu!",
                                                                                                          "");
                                                                       }
                                                                       else
                                                                       {
                                                                           Globals.
                                                                                                      ShowMessage(
                                                                                                          "Lưu không thành công!",
                                                                                                          "");
                                                                       }
                                                                   }
                                                                   catch (Exception ex)
                                                                   {
                                                                       Globals.ShowMessage(
                                                                                                  ex.Message,
                                                                                                  eHCMSResources.T0432_G1_Error);
                                                                   }
                                                                   finally
                                                                   {
                                                                       Globals.IsBusy = false;
                                                                   }
                                                               }), null);
                }
            });

            t.Start();
        }

        private void GetPCLExamParamResultList(long PCLExamParamResultID, long ParamEnum)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPCLExamParamResultList(PCLExamParamResultID, ParamEnum
                                                            ,
                                                            Globals.DispatchCallback(
                                                                (asyncResult) =>
                                                                {
                                                                    try
                                                                    {
                                                                        List
                                                                                                   <
                                                                                                       PCLExamParamResult
                                                                                                       > res =
                                                                                                           contract.
                                                                                                               EndGetPCLExamParamResultList
                                                                                                               (asyncResult);
                                                                        lstPCLExamParamResult
                                                                                                   =
                                                                                                   new ObservableCollection
                                                                                                       <
                                                                                                           PCLExamParamResult
                                                                                                           >();
                                                                        foreach (
                                                                                                   PCLExamParamResult
                                                                                                       pclExamParamResult
                                                                                                       in res)
                                                                        {
                                                                            lstPCLExamParamResult
                                                                                                       .Add(
                                                                                                           pclExamParamResult);
                                                                        }
                                                                        NotifyOfPropertyChange
                                                                                                   (() =>
                                                                                                    lstPCLExamParamResult);
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        Globals.ShowMessage(
                                                                                                   ex.Message,
                                                                                                   eHCMSResources.T0432_G1_Error);
                                                                    }
                                                                    finally
                                                                    {
                                                                        Globals.IsBusy =
                                                                                                   false;
                                                                    }
                                                                }), null);
                }
            });

            t.Start();
        }

        private void GetPCLExamResultTemplateList(long PCLExamGroupTemplateResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPCLExamResultTemplateList(PCLExamGroupTemplateResultID
                                                               ,
                                                               Globals.DispatchCallback(
                                                                   (asyncResult) =>
                                                                   {
                                                                       try
                                                                       {
                                                                           List
                                                                                                      <
                                                                                                          PCLExamResultTemplate
                                                                                                          > res =
                                                                                                              contract
                                                                                                                  .
                                                                                                                  EndGetPCLExamResultTemplateList
                                                                                                                  (asyncResult);
                                                                           lstPCLExamResultTemplate
                                                                                                      =
                                                                                                      new ObservableCollection
                                                                                                          <
                                                                                                              PCLExamResultTemplate
                                                                                                              >();
                                                                           foreach (
                                                                                                      PCLExamResultTemplate
                                                                                                          pclExamParamResult
                                                                                                          in res)
                                                                           {
                                                                               lstPCLExamResultTemplate
                                                                                                          .Add(
                                                                                                              pclExamParamResult);
                                                                           }
                                                                           NotifyOfPropertyChange
                                                                                                      (() =>
                                                                                                       lstPCLExamResultTemplate);
                                                                       }
                                                                       catch (Exception ex)
                                                                       {
                                                                           Globals.
                                                                                                      ShowMessage(
                                                                                                          ex.Message,
                                                                                                          eHCMSResources.T0432_G1_Error);
                                                                       }
                                                                       finally
                                                                       {
                                                                           Globals.IsBusy =
                                                                                                      false;
                                                                       }
                                                                   }), null);
                }
            });

            t.Start();
        }

        private void GetPCLExamResultTemplate(long PCLExamGroupTemplateResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPCLExamResultTemplate(PCLExamGroupTemplateResultID
                                                           ,
                                                           Globals.DispatchCallback(
                                                               (asyncResult) =>
                                                               {
                                                                   try
                                                                   {
                                                                       curPCLExamResultTemplate
                                                                                                  =
                                                                                                  contract.
                                                                                                      EndGetPCLExamResultTemplate
                                                                                                      (asyncResult);

                                                                       NotifyOfPropertyChange
                                                                                                  (() =>
                                                                                                   lstPCLExamResultTemplate);
                                                                   }
                                                                   catch (Exception ex)
                                                                   {
                                                                       Globals.ShowMessage(
                                                                                                  ex.Message,
                                                                                                  eHCMSResources.T0432_G1_Error);
                                                                   }
                                                                   finally
                                                                   {
                                                                       Globals.IsBusy = false;
                                                                   }
                                                               }), null);
                }
            });

            t.Start();
        }

        #endregion
        [ImportingConstructor]
        public SieuAmResultTemplateViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _curPCLExamResultTemplate = new PCLExamResultTemplate();
            _curPCLExamParamResult = new PCLExamParamResult();

            GetPclExamType();
        }

        //public ObservableCollection<AllLookupValues.PCLResultParamImpID> allPCLResultParamImpID
        //{
        //    get { return _allPCLResultParamImpID; }
        //    set
        //    {
        //        if (_allPCLResultParamImpID == value)
        //            return;
        //        _allPCLResultParamImpID = value;
        //        NotifyOfPropertyChange(() => allPCLResultParamImpID);
        //    }
        //}
    }
}