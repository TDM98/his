using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using Service.Core.Common;

namespace DataEntities
{
    public class DrugDeptEstimationForPODetailExt:EntityBase
    {
        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }

        [DataMemberAttribute()]
        public DrugDeptEstimationForPoDetail DrugDeptEstimationForPoDetail
        {
            get
            {
                return _DrugDeptEstimationForPoDetail;
            }
            set
            {
                _DrugDeptEstimationForPoDetail = value;
                RaisePropertyChanged("DrugDeptEstimationForPoDetail");
            }
        }
        private DrugDeptEstimationForPoDetail _DrugDeptEstimationForPoDetail;

        [DataMemberAttribute()]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                if (_Code != value)
                {
                    _Code = value;
                    RaisePropertyChanged("Code");
                }
            }
        }
        private string _Code;
    }
}
