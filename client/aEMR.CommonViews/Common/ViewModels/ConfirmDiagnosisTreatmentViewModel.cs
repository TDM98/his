using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Linq;

/*
 * 20191115 #001 TBL: BM 0019576: Fix lại chỉ còn xác nhận chẩn đoán xuất khoa và chẩn đoán XV, khi đã có xác nhận XV thì toa thuốc xuất viện và xuất viện BN thì phải chọn đúng chẩn đoán đó
 */

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IConfirmDiagnosisTreatment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConfirmDiagnosisTreatmentViewModel : ViewModelBase, IConfirmDiagnosisTreatment
    {
        #region Properties
        private IDiagnosisTreatmentTree _UCDiagnosisTreatmentTree;
        public IDiagnosisTreatmentTree UCDiagnosisTreatmentTree
        {
            get
            {
                return _UCDiagnosisTreatmentTree;
            }
            set
            {
                if (_UCDiagnosisTreatmentTree == value)
                {
                    return;
                }
                _UCDiagnosisTreatmentTree = value;
                NotifyOfPropertyChange(() => UCDiagnosisTreatmentTree);
            }
        }
        private IConsultationOld_InPt _UCConsultationOld_InPt;
        public IConsultationOld_InPt UCConsultationOldInPt
        {
            get
            {
                return _UCConsultationOld_InPt;
            }
            set
            {
                if (_UCConsultationOld_InPt == value)
                {
                    return;
                }
                _UCConsultationOld_InPt = value;
                NotifyOfPropertyChange(() => UCConsultationOldInPt);
            }
        }
        private IConsultationOld_V3 _UCConsultationOld;
        public IConsultationOld_V3 UCConsultationOld
        {
            get
            {
                return _UCConsultationOld;
            }
            set
            {
                if (_UCConsultationOld == value)
                {
                    return;
                }
                _UCConsultationOld = value;
                NotifyOfPropertyChange(() => UCConsultationOld);
            }
        }
        //CMN: Chẩn đoán được xác nhận
        public DiagnosisTreatment CurrentDiagnosisTreatment
        {
            get
            {
                return IsConfirmed ? (IsInPtView ? UCConsultationOldInPt.DiagTrmtItem : UCConsultationOld.DiagTrmtItem) : null;
            }
        }
        private bool IsConfirmed = false;
        private long _DeptID;
        public long DeptID
        {
            get { return _DeptID; }
            set
            {
                if (_DeptID != value)
                {
                    _DeptID = value;
                    NotifyOfPropertyChange(() => DeptID);
                }
            }
        }
        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType == value)
                {
                    return;
                }
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
                NotifyOfPropertyChange(() => IsInPtView);
            }
        }
        public bool IsInPtView
        {
            get
            {
                return V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            }
        }
        //20191115 TBL: Cờ để nhận biết màn hình xác nhận được gọi từ Toa thuốc XV hoặc Xuất viện BN
        private bool _IsPreAndDischargeView;
        public bool IsPreAndDischargeView
        {
            get { return _IsPreAndDischargeView; }
            set
            {
                if (_IsPreAndDischargeView != value)
                {
                    _IsPreAndDischargeView = value;
                    NotifyOfPropertyChange(() => IsPreAndDischargeView);
                }
            }
        }
        private bool _IsConfirmAgainDiagnosisTreatment;
        public bool IsConfirmAgainDiagnosisTreatment
        {
            get { return _IsConfirmAgainDiagnosisTreatment; }
            set
            {
                if (_IsConfirmAgainDiagnosisTreatment != value)
                {
                    _IsConfirmAgainDiagnosisTreatment = value;
                    NotifyOfPropertyChange(() => IsConfirmAgainDiagnosisTreatment);
                }
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public ConfirmDiagnosisTreatmentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            UCDiagnosisTreatmentTree = Globals.GetViewModel<IDiagnosisTreatmentTree>();
            UCDiagnosisTreatmentTree.CurrentViewCase = 2;
        }
        public void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection)
        {
            UCDiagnosisTreatmentTree.ApplyDiagnosisTreatmentCollection(aDiagnosisTreatmentCollection);
        }
        public void btnConfirm()
        {
            if (UCConsultationOldInPt.DiagTrmtItem == null)
            {
                MessageBox.Show(eHCMSResources.A0405_G1_Msg_InfoChuaCoCD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //20191104 TBL: Kiểm tra khoa bệnh nhân đang nằm với khoa ra chẩn đoán
            if (IsInPtView && DeptID != UCConsultationOldInPt.DiagTrmtItem.Department.DeptID)
            {
                MessageBox.Show(eHCMSResources.Z2902_G1_CDKhongPhaiCuaKhoaDangNam, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            /*▼====: #001*/
            bool IsConfimedForPreAndDischarge = false;
            //20191115 TBL: Kiểm tra xem đã có xác nhận chẩn đoán xuất viện chưa
            //if (IsPreAndDischargeView && UCDiagnosisTreatmentTree.DiagnosisTreatmentCollection != null 
            //    && UCDiagnosisTreatmentTree.DiagnosisTreatmentCollection.Count > 0)
            //{
            //    foreach (var item in UCDiagnosisTreatmentTree.DiagnosisTreatmentCollection)
            //    {
            //        if (item.ConfimedForPreAndDischarge.GetValueOrDefault() > 0)
            //        {
            //            //IsConfimedForPreAndDischarge = true;
            //            break;
            //        }
            //    }
            //}
            //20191115 TBL: Kiểm tra chẩn đoán đang được chọn có phải là chẩn đoán xác nhận xuất viện không
            //if (!IsConfirmAgainDiagnosisTreatment && IsPreAndDischargeView && IsConfimedForPreAndDischarge && UCConsultationOldInPt.DiagTrmtItem.ConfimedForPreAndDischarge.GetValueOrDefault() == 0)
            //{
            //    MessageBox.Show(eHCMSResources.Z2918_G1_CDKhPhaiCDXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            /*▲====: #001*/
            IsConfirmed = true;
            TryClose();
        }
        //CMN: Dời hàm Init các UserControl chẩn đoán vì sử dụng màn hình xác nhận cho chẩn đoán Ngoại lẫn Nội trú
        protected override void OnActivate()
        {
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                UCConsultationOld = Globals.GetViewModel<IConsultationOld_V3>();
                ActivateItem(UCConsultationOld);
            }
            else
            {
                UCConsultationOldInPt = Globals.GetViewModel<IConsultationOld_InPt>();
                ActivateItem(UCConsultationOldInPt);
            }
            base.OnActivate();
        }
        #endregion
    }
}