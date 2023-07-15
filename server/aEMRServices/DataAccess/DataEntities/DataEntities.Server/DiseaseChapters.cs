using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class DiseaseChapters : EntityBase, IEditableObject
    {
        public static DiseaseChapters CreateDiseaseChapters(Int32 DiseaseChapterID,String DiseaseChapterNameVN, String ICDXCode)
        {
            DiseaseChapters DiseaseChapters = new DiseaseChapters();
            DiseaseChapters.DiseaseChapterID = DiseaseChapterID;
            DiseaseChapters.DiseaseChapterNameVN = DiseaseChapterNameVN;
            DiseaseChapters.ICDXCode = ICDXCode;
            return DiseaseChapters;
        }
        #region Primitive Properties


        [DataMemberAttribute()]
        public Int32 DiseaseChapterID
        {
            get
            {
                return _DiseaseChapterID;
            }
            set
            {
                if (_DiseaseChapterID != value)
                {
                    OnDiseaseChapterIDChanging(value);
                    _DiseaseChapterID = value;
                    RaisePropertyChanged("DiseaseChapterID");
                    OnDiseaseChapterIDChanged();
                }
            }
        }
        private Int32 _DiseaseChapterID;
        partial void OnDiseaseChapterIDChanging(Int32 value);
        partial void OnDiseaseChapterIDChanged();

        

        [Required(ErrorMessage = "Nhập chẩn đoán!")]
        [DataMemberAttribute()]
        public String ICDXCode
        {
            get
            {
                return _ICDXCode;
            }
            set
            {
                OnICDXCodeChanging(value);
                ValidateProperty("ICDXCode", value);
                _ICDXCode = value;
                RaisePropertyChanged("ICDXCode");
                OnICDXCodeChanged();
            }
        }
        private String _ICDXCode;
        partial void OnICDXCodeChanging(String value);
        partial void OnICDXCodeChanged();
        


        [DataMemberAttribute()]
        public String DiseaseChapterNameVN
        {
            get
            {
                return _DiseaseChapterNameVN;
            }
            set
            {
                OnDiseaseChapterNameVNChanging(value);
                _DiseaseChapterNameVN = value;
                RaisePropertyChanged("DiseaseChapterNameVN");
                OnDiseaseChapterNameVNChanged();
            }
        }
        private String _DiseaseChapterNameVN;
        partial void OnDiseaseChapterNameVNChanging(String value);
        partial void OnDiseaseChapterNameVNChanged();


        [DataMemberAttribute()]
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                OnDateModifiedChanging(value);
                _DateModified = value;
                RaisePropertyChanged("DateModified");
                OnDateModifiedChanged();
            }
        }
        private DateTime _DateModified;
        partial void OnDateModifiedChanging(DateTime value);
        partial void OnDateModifiedChanged();

        [DataMemberAttribute()]
        public String ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                OnModifiedLogChanging(value);
                _ModifiedLog = value;
                RaisePropertyChanged("ModifiedLog");
                OnModifiedLogChanged();
            }
        }
        private String _ModifiedLog;
        partial void OnModifiedLogChanging(String value);
        partial void OnModifiedLogChanged();

        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }


        #endregion
        #region Navigation Properties
        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_LOCATION_REL_RM29_ROOMTYPE", "Locations")]
        public ObservableCollection<DiseaseChapters> DiseaseChapterss
        {
            get;
            set;
        }
        #endregion
    }
}
