using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PCLForExternalRefDetails : NotifyChangedBase
    {
      
            #region Factory Method
            public static PCLForExternalRefDetails CreatePCLForExternalRefDetails(long pCLForExtRefDetailsID, long pCLExtRefID, long pCLItemID)
            {
                PCLForExternalRefDetails pCLForExternalRefDetails = new PCLForExternalRefDetails();
                pCLForExternalRefDetails.PCLForExtRefDetailsID = pCLForExtRefDetailsID;
                pCLForExternalRefDetails.PCLExtRefID = pCLExtRefID;
                pCLForExternalRefDetails.PCLItemID = pCLItemID;
                return pCLForExternalRefDetails;
            }

            #endregion
            #region Primitive Properties

            [DataMemberAttribute()]
            public long PCLForExtRefDetailsID
            {
                get
                {
                    return _PCLForExtRefDetailsID;
                }
                set
                {
                    if (_PCLForExtRefDetailsID != value)
                    {
                        OnPCLForExtRefDetailsIDChanging(value);
                        _PCLForExtRefDetailsID = value;
                        RaisePropertyChanged("PCLForExtRefDetailsID");
                        OnPCLForExtRefDetailsIDChanged();
                    }
                }
            }
            private long _PCLForExtRefDetailsID;
            partial void OnPCLForExtRefDetailsIDChanging(long value);
            partial void OnPCLForExtRefDetailsIDChanged();

            [DataMemberAttribute()]
            public long PCLExtRefID
            {
                get
                {
                    return _PCLExtRefID;
                }
                set
                {
                    OnPCLExtRefIDChanging(value);
                    _PCLExtRefID = value;
                    RaisePropertyChanged("PCLExtRefID");
                    OnPCLExtRefIDChanged();
                }
            }
            private long _PCLExtRefID;
            partial void OnPCLExtRefIDChanging(long value);
            partial void OnPCLExtRefIDChanged();

         
            [DataMemberAttribute()]
            public long PCLItemID
            {
                get
                {
                    return _PCLItemID;
                }
                set
                {
                    OnPCLItemIDChanging(value);
                    _PCLItemID = value;
                    RaisePropertyChanged("PCLItemID");
                    OnPCLItemIDChanged();
                }
            }
            private long _PCLItemID;
            partial void OnPCLItemIDChanging(long value);
            partial void OnPCLItemIDChanged();


            #endregion

            #region Navigation Properties


            [DataMemberAttribute()]
            public PCLItem PCLItem
            {
                get;
                set;
            }
           
            [DataMemberAttribute()]
            public PCLForExternalRef PCLForExternalRef
            {
                get;
                set;
            }
            #endregion
        }
}
