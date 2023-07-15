using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using CommonServiceProxy;
using aEMR.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Castle.Windsor;
using eHCMSLanguage;
using aEMR.Controls;
using System.Linq;
using aEMR.Common;
using System.ServiceModel;
using aEMR.DataContracts;
using DataEntities.MedicalInstruction;
using aEMR.Common.BaseModel;
using System.Windows.Data;
using System.ComponentModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure.Events;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IInstructionFromWeb)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InstructionFromWebViewModel : ViewModelBase, IInstructionFromWeb
    {
        ObservableCollection<DiagnosisTreatment> _DiagnosisTreatmentCollection;
        public ObservableCollection<DiagnosisTreatment> DiagnosisTreatmentCollection
        {
            get
            {
                return _DiagnosisTreatmentCollection;
            }
            set
            {
                if (_DiagnosisTreatmentCollection == value)
                {
                    return;
                }
                _DiagnosisTreatmentCollection = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentCollection);
                DiagnosisTreatmentView = CollectionViewSource.GetDefaultView(DiagnosisTreatmentCollection);
                DiagnosisTreatmentView.GroupDescriptions.Add(new PropertyGroupDescription("DeptDetail.GroupText2"));

                //▼===== 20191011 TTM:  Trong DiagnosisTreatment không có InPatientDeptDetailID mà là của DeptDetail (DiagnosisDeptDetail). 
                //                      Nên phải đăt DeptDetail trước InPatientDeptDetailID Mới có thể lấy biding.
                //DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("InPatientDeptDetailID", ListSortDirection.Descending));
                DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("DeptDetail.InPatientDeptDetailID", ListSortDirection.Descending));
                //▲===== 
                //20191128 TBL: Sort theo ngày chứ không theo DTItemID vì y lệnh co thể chỉnh sửa giờ được
                //DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("DTItemID", ListSortDirection.Descending));
                DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("DiagnosisDate", ListSortDirection.Descending));
                if (gTreatmentCollectionOnChanged != null)
                {
                    gTreatmentCollectionOnChanged(DiagnosisTreatmentCollection);
                }
            }
        }
        private ICollectionView _DiagnosisTreatmentView;
        public ICollectionView DiagnosisTreatmentView
        {
            get
            {
                return _DiagnosisTreatmentView;
            }
            set
            {
                if (_DiagnosisTreatmentView == value)
                {
                    return;
                }
                _DiagnosisTreatmentView = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentView);
            }
        }
        private long _PtRegistrationID = 0;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                NotifyOfPropertyChange(() => PtRegistrationID);
            }
        }
        public TreatmentCollectionOnChanged gTreatmentCollectionOnChanged { get; set; }
        [ImportingConstructor]
        public InstructionFromWebViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
        }
        public void GetInstructionList()
        {
            if (PtRegistrationID == 0)
            {
                return;
            }
            LoadData(PtRegistrationID);
        }

        public void LoadData(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagAndDoctorInstruction_InPt_ByPtRegID(PtRegistrationID, null, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mReturnCollection = mContract.EndGetDiagAndDoctorInstruction_InPt_ByPtRegID(asyncResult);
                            if (mReturnCollection == null)
                            {
                                DiagnosisTreatmentCollection = new ObservableCollection<DiagnosisTreatment>();
                            }
                            else
                            {
                                DiagnosisTreatmentCollection = mReturnCollection.ToObservableCollection();
                                //.Where(x => x.IntPtDiagDrInstructionID != 0).ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        public void gvDiagnosis_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (!mChanDoan_tabLanKhamTruoc_HieuChinh && CurrentViewCase != (byte)ViewCase.ConfirmDiagnosisTreatment)
            //{
            //    Globals.ShowMessage(eHCMSResources.Z0508_G1_ChuaDuocQuyenSua, "");
            //    return;
            //}
            //if ((sender as DataGrid).SelectedItem != null)
            //{
            //    TryClose();
            //    var CurrentDiagTrmt = (sender as DataGrid).SelectedItem as DiagnosisTreatment;
            //    //Coroutine.BeginExecute(DoubleClick_Routine(CurrentDiagTrmt));
            //    ITicketCare mDialogView = Globals.GetViewModel<ITicketCare>();
            //    mDialogView.PtRegistrationID = PtRegistrationID;
            //    mDialogView.IntPtDiagDrInstructionID = CurrentDiagTrmt.IntPtDiagDrInstructionID;
            //    //mDialogView.V_LevelCare = CurRegistration.InPatientInstruction.V_LevelCare.LookupID;
            //    mDialogView.DoctorOrientedTreatment = CurrentDiagTrmt.OrientedTreatment;
            //    mDialogView.GetTicketCare(CurrentDiagTrmt.IntPtDiagDrInstructionID);
            //    GlobalsNAV.ShowDialog_V3(mDialogView);

            //}
        }
        public void ConfirmCmd(object datacontext, EventAggregator e)
        {
            DiagnosisTreatment temp = datacontext as DiagnosisTreatment;
            if(temp != null && temp.IntPtDiagDrInstructionID > 0)
            {
                Globals.EventAggregator.Publish(new LoadInstructionFromWebsite { IntPtDiagDrInstructionID = temp.IntPtDiagDrInstructionID });
                this.TryClose();
            }
        }
        public void PreviewCmd(object sender, EventAggregator e)
        {

        }
    }
}
