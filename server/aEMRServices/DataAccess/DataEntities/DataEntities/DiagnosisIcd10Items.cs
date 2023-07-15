using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

/*
 * 20180920 #001 TBL:   Chinh sua chuc nang bug mantis ID 0000061, kiem tra tung property trong dataentities 
 *                      neu co su thay doi IsDataChanged = true
 */
namespace DataEntities
{
    public partial class DiagnosisIcd10Items : NotifyChangedBase
    {

        //#region Factory Method


        ///// Create a new DiagnosisIcd10Items object.

        ///// <param name="DiagIcd10ItemID">Initial value of the DiagIcd10ItemID property.</param>
        //public static DiagnosisIcd10Items CreateDiagnosisIcd10Items(long DiagIcd10ItemID)
        //{
        //    DiagnosisIcd10Items DiagnosisIcd10Items = new DiagnosisIcd10Items();
        //    DiagnosisIcd10Items.DiagIcd10ItemID = DiagIcd10ItemID;
        //    return DiagnosisIcd10Items;
        //}

        //#endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long DiagIcd10ItemID
        {
            get
            {
                return _DiagIcd10ItemID;
            }
            set
            {
                if (_DiagIcd10ItemID != value)
                {
                    OnDiagIcd10ItemIDChanging(value);
                    _DiagIcd10ItemID = value;
                    RaisePropertyChanged("DiagIcd10ItemID");
                    OnDiagIcd10ItemIDChanged();
                }
            }
        }
        private long _DiagIcd10ItemID;
        partial void OnDiagIcd10ItemIDChanging(long value);
        partial void OnDiagIcd10ItemIDChanged();

        [DataMemberAttribute()]
        public long DiagnosisIcd10ListID
        {
            get
            {
                return _DiagnosisIcd10ListID;
            }
            set
            {
                OnDiagnosisIcd10ListIDChanging(value);
                _DiagnosisIcd10ListID = value;
                RaisePropertyChanged("DiagnosisIcd10ListID");
                OnDiagnosisIcd10ListIDChanged();
            }
        }
        private long _DiagnosisIcd10ListID;
        partial void OnDiagnosisIcd10ListIDChanging(long value);
        partial void OnDiagnosisIcd10ListIDChanged();

        [DataMemberAttribute()]
        public String ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                OnICD10CodeChanging(value);
                _ICD10Code = value;
                RaisePropertyChanged("ICD10Code");
                OnICD10CodeChanged();
            }
        }
        private String _ICD10Code;
        partial void OnICD10CodeChanging(String value);
        partial void OnICD10CodeChanged();

        [DataMemberAttribute()]
        public long V_DiagIcdStatus
        {
            get
            {
                return _V_DiagIcdStatus;
            }
            set
            {
                _V_DiagIcdStatus = value;
                RaisePropertyChanged("V_DiagIcdStatus");
            }
        }
        private long _V_DiagIcdStatus;


        [DataMemberAttribute()]
        public bool IsMain
        {
            get
            {
                return _IsMain;
            }
            set
            {
                OnIsMainChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _IsMain != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _IsMain = value;
                RaisePropertyChanged("IsMain");
                OnIsMainChanged();
            }
        }
        private bool _IsMain;
        partial void OnIsMainChanging(bool value);
        partial void OnIsMainChanged();

         [DataMemberAttribute()]
        public bool IsCongenital
        {
            get
            {
                return _IsCongenital;
            }
            set
            {
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _IsCongenital != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _IsCongenital = value;
                RaisePropertyChanged("IsCongenital");
            }
        }
        private bool _IsCongenital;

        #endregion

        #region Navigation Properties

        private DiseasesReference _DiseasesReference;
        public DiseasesReference DiseasesReference
        {
            get
            {
                return _DiseasesReference;
            }
            set
            {
                if (_DiseasesReference != value)
                {
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _DiseasesReference != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _DiseasesReference = value;
                    if (_DiseasesReference != null)
                    {
                        _ICD10Code = value.ICD10Code;
                    }
                    RaisePropertyChanged("ICD10Code");
                    RaisePropertyChanged("DiseasesReference");
                }
            }
        }

        private Lookup _LookupStatus;
        public Lookup LookupStatus
        {
            get
            {
                return _LookupStatus;
            }
            set
            {
                if (_LookupStatus != value)
                {
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _LookupStatus != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _LookupStatus = value;
                    RaisePropertyChanged("LookupStatus");
                }
            }
        }

        #endregion

        [DataMemberAttribute()]
        public bool IsInvalid
        {
            get
            {
                return _IsInvalid;
            }
            set
            {
                if (_IsInvalid != value)
                {
                    _IsInvalid = value;
                    RaisePropertyChanged("IsInvalid");
                }
            }
        }
        private bool _IsInvalid;
        /*▼====: #001*/
        public bool IsDataChanged = false;

        private bool _IsObjectBeingUsedByClient = false;
        [DataMemberAttribute()]
        public bool IsObjectBeingUsedByClient
        {
            get
            {
                return _IsObjectBeingUsedByClient;
            }
            set
            {
                if (_IsObjectBeingUsedByClient != value)
                {
                    _IsObjectBeingUsedByClient = value;
                    RaisePropertyChanged("IsObjectBeingUsedByClient");
                }
            }
        }
        /*▲====: #001*/
        public bool IsRequireSubICD { get; set; } = false;
    }
}