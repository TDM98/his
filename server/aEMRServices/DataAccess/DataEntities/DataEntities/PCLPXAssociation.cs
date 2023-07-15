using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PCLPXAssociation : NotifyChangedBase
    {
            #region Factory Method
            public static PCLPXAssociation CreatePCLPXAssociation(long pCLPXID, long prescriptID, long pLCExamID)
            {
                PCLPXAssociation pCLPXAssociation = new PCLPXAssociation();
                pCLPXAssociation.PCLPXID = pCLPXID;
                pCLPXAssociation.PrescriptID = prescriptID;
                pCLPXAssociation.PLCExamID = pLCExamID;
                return pCLPXAssociation;
            }

            #endregion
            #region Primitive Properties

          
            [DataMemberAttribute()]
            public long PCLPXID
            {
                get
                {
                    return _PCLPXID;
                }
                set
                {
                    if (_PCLPXID != value)
                    {
                        OnPCLPXIDChanging(value);
                        _PCLPXID = value;
                        RaisePropertyChanged("PCLPXID");
                        OnPCLPXIDChanged();
                        
                    }
                }
            }
            private long _PCLPXID;
            partial void OnPCLPXIDChanging(long value);
            partial void OnPCLPXIDChanged();

          
            [DataMemberAttribute()]
            public long PrescriptID
            {
                get
                {
                    return _PrescriptID;
                }
                set
                {
                    OnPrescriptIDChanging(value);
                    _PrescriptID = value;
                    RaisePropertyChanged("PrescriptID");
                    OnPrescriptIDChanged();
                }
            }
            private long _PrescriptID;
            partial void OnPrescriptIDChanging(long value);
            partial void OnPrescriptIDChanged();

            [DataMemberAttribute()]
            public long PLCExamID
            {
                get
                {
                    return _PLCExamID;
                }
                set
                {
                    OnPLCExamIDChanging(value);
                    _PLCExamID = value;
                    RaisePropertyChanged("PLCExamID");
                    OnPLCExamIDChanged();
                }
            }
            private long _PLCExamID;
            partial void OnPLCExamIDChanging(long value);
            partial void OnPLCExamIDChanged();

            #endregion

            #region Navigation Properties


            [DataMemberAttribute()]

            public PatientPCLImagingResult PatientPCLExamResult
            {
                get;
                set;
            }


            [DataMemberAttribute()]
            public Prescription Prescription
            {
                get;
                set;
            }
            #endregion
    }
}
