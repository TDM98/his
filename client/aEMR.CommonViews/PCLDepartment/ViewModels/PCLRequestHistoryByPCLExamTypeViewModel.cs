using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
using eHCMS.CommonUserControls.CommonTasks;
using System.Linq;
using aEMR.Common.BaseModel;
/*
 * 20220523 #001 BLQ: Chỉnh phần lịch sử khi chọn sẽ hiện popup kết quả giống như màn hình khám bệnh 
 */
namespace aEMR.ViewModels
{
    [Export(typeof(IPCLRequestHistoryByPCLExamType)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLRequestHistoryByPCLExamTypeViewModel : ViewModelBase, IPCLRequestHistoryByPCLExamType
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private long _PCLExamTypeID;
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                _PCLExamTypeID = value;
                NotifyOfPropertyChange(() => PCLExamTypeID);
            }
        }

        private long _PatientID;
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                NotifyOfPropertyChange(() => PatientID);
            }
        }


        public PCLRequestHistoryByPCLExamTypeViewModel()
        {
            Globals.EventAggregator.Subscribe(this);

            ObjPatientPCLRequestDetail_SearchPaging_Selected = new PatientPCLRequestDetail();

            ObjPatientPCLRequestDetail_SearchPaging = new PagedSortableCollectionView<PatientPCLRequestDetail>();
            ObjPatientPCLRequestDetail_SearchPaging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPatientPCLRequestDetail_SearchPaging_OnRefresh);
        }

        public void LoadData()
        {
            GetHistoryPCLByPCLExamType(PatientID,PCLExamTypeID,0, ObjPatientPCLRequestDetail_SearchPaging.PageSize, true);
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        

       
        void ObjPatientPCLRequestDetail_SearchPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetHistoryPCLByPCLExamType(PatientID, PCLExamTypeID, ObjPatientPCLRequestDetail_SearchPaging.PageIndex, ObjPatientPCLRequestDetail_SearchPaging.PageSize, false);
        }
        private PatientPCLRequestSearchCriteria _SearchCriteria;
        public PatientPCLRequestSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }



        private PatientPCLRequestDetail _ObjPatientPCLRequestDetail_SearchPaging_Selected;
        public PatientPCLRequestDetail ObjPatientPCLRequestDetail_SearchPaging_Selected
        {
            get { return _ObjPatientPCLRequestDetail_SearchPaging_Selected; }
            set
            {
                _ObjPatientPCLRequestDetail_SearchPaging_Selected = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequestDetail_SearchPaging_Selected);
            }
        }

        private PagedSortableCollectionView<PatientPCLRequestDetail> _ObjPatientPCLRequestDetail_SearchPaging;
        public PagedSortableCollectionView<PatientPCLRequestDetail> ObjPatientPCLRequestDetail_SearchPaging
        {
            get { return _ObjPatientPCLRequestDetail_SearchPaging; }
            set
            {
                _ObjPatientPCLRequestDetail_SearchPaging = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequestDetail_SearchPaging);
            }
        }
        private int _AllCasePCL = 0;
        public int AllCasePCL
        {
            get { return _AllCasePCL; }
            set
            {
                _AllCasePCL = value;
                NotifyOfPropertyChange(() => AllCasePCL);
            }
        }


        
      

        public void btCancel()
        {
            TryClose();
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            PatientPCLRequestDetail p = eventArgs.Value as PatientPCLRequestDetail;
            //Globals.EventAggregator.Publish(new PrintEventActionView());
            //▼====: #001
            //IPatientPCLImagingResult mPopupDialog = Globals.GetViewModel<IPatientPCLImagingResult>();
            //mPopupDialog.IsShowSummaryContent = false;
            //mPopupDialog.IsDialogView = true;
            //mPopupDialog.LoadDataCoroutineEx(new PatientPCLRequest
            //{
            //    PatientPCLReqID = p.PatientPCLReqID,
            //    PatientID = PatientID,
            //    V_PCLRequestType =  AllLookupValues.V_PCLRequestType.NGOAI_TRU,
            //    PCLExamTypeID = p.PCLExamTypeID
            //});
            //GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
            if (p.PatientPCLReqID > 0 && p.PCLExamTypeID > 0)
            {
                IPatientPCLImagingResult_V2 mPopupDialog = Globals.GetViewModel<IPatientPCLImagingResult_V2>();
                mPopupDialog.InitHTML();
                mPopupDialog.CheckTemplatePCLResultByReqID(p.PatientPCLReqID, false);
                GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
            }
            //▲====: #001
        }

        public void btOK()
        {
            if (ObjPatientPCLRequestDetail_SearchPaging_Selected != null && ObjPatientPCLRequestDetail_SearchPaging_Selected.PatientPCLReqID > 0)
            {
                //ChonPhieuDiXetNghiem(ObjPatientPCLRequest_SearchPaging_Selected);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0351_G1_Msg_InfoChonPh);
            }
        }

        private void GetHistoryPCLByPCLExamType(long PatientID, long PCLExamTypeID, int pageIndex, int pageSize, bool countTotal)
        {
            List<PatientPCLRequestDetail> PatientPCLReqList = new List<PatientPCLRequestDetail>();
            int Total = 0;
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bActionCompleteOk = false;

                        contract.BeginGetHistoryPCLByPCLExamType(PatientID, PCLExamTypeID, pageIndex, pageSize, "", (bool)countTotal
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    PatientPCLReqList = contract.EndGetHistoryPCLByPCLExamType(out Total, asyncResult).ToList();
                                    bActionCompleteOk = true;
                                    if (countTotal)
                                    {
                                        ObjPatientPCLRequestDetail_SearchPaging.TotalItemCount = Total;
                                        AllCasePCL = Total;
                                    }
                                    if (PatientPCLReqList != null)
                                    {
                                        foreach (var item in PatientPCLReqList)
                                        {
                                            ObjPatientPCLRequestDetail_SearchPaging.Add(item);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    bActionCompleteOk = false;
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                    if (!bActionCompleteOk)
                                    {
                                        TryClose();
                                    }
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    MessageBox.Show(ex.Message, "");
                }
            });
            t.Start();
        }
    }
}
