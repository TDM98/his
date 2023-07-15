using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities.MedicalInstruction
{
    public partial class ExecuteDrug: NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long ExecuteDrugID
        {
            get
            {
                return _ExecuteDrugID;
            }
            set
            {
                if (_ExecuteDrugID != value)
                {
                    _ExecuteDrugID = value;
                    RaisePropertyChanged("ExecuteDrugID");
                }
            }
        }
        private long _ExecuteDrugID;
        [DataMemberAttribute()]
        public bool MarkAsDeleted
        {
            get
            {
                return _MarkAsDeleted;
            }
            set
            {
                _MarkAsDeleted = value;
                RaisePropertyChanged("MarkAsDeleted");
            }
        }
        private bool _MarkAsDeleted;
        [DataMemberAttribute()]
        public List<ExecuteDrugDetail> ExecuteDrugDetailList
        {
            get
            {
                return _ExecuteDrugDetailList;
            }
            set
            {
                _ExecuteDrugDetailList = value;
                RaisePropertyChanged("ExecuteDrugDetailList");
            }
        }
        private List<ExecuteDrugDetail> _ExecuteDrugDetailList;
        [DataMemberAttribute()]
        public ExecuteDrugDetail ExecuteDrugDetail
        {
            get
            {
                return _ExecuteDrugDetail;
            }
            set
            {
                _ExecuteDrugDetail = value;
                RaisePropertyChanged("ExecuteDrugDetail");
            }
        }
        private ExecuteDrugDetail _ExecuteDrugDetail;
        [DataMemberAttribute()]
        public RefGenMedProductDetails DrugInfo
        {
            get
            {
                return _DrugInfo;
            }
            set
            {
                _DrugInfo = value;
                RaisePropertyChanged("DrugInfo");
            }
        }
        private RefGenMedProductDetails _DrugInfo;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }
        private long _IntPtDiagDrInstructionID;
        [DataMemberAttribute()]
        public long OutClinicDeptReqID
        {
            get
            {
                return _OutClinicDeptReqID;
            }
            set
            {
                _OutClinicDeptReqID = value;
                RaisePropertyChanged("OutClinicDeptReqID");
            }
        }
        private long _OutClinicDeptReqID;
        [DataMemberAttribute()]
        public decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
        private decimal _Qty;
        [DataMemberAttribute()]
        public int NumberOfUse
        {
            get
            {
                return _NumberOfUse;
            }
            set
            {
                _NumberOfUse = value;
                RaisePropertyChanged("NumberOfUse");
            }
        }
        private int _NumberOfUse;
        [DataMemberAttribute()]
        public long V_LevelCare
        {
            get
            {
                return _V_LevelCare;
            }
            set
            {
                _V_LevelCare = value;
                RaisePropertyChanged("V_LevelCare");
            }
        }
        private long _V_LevelCare;
        #endregion

        public static ExecuteDrug CreateExecuteDrug()
        {
            ExecuteDrug ExecuteDrug = new ExecuteDrug();
            ExecuteDrug.ExecuteDrugDetailList = new List<ExecuteDrugDetail>();
            return ExecuteDrug;
        }
    }
}
