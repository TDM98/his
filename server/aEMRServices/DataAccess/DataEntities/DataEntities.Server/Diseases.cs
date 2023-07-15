using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class Diseases : EntityBase, IEditableObject
    {
        public static Diseases CreateDiseaseChapters(Int64 DiseaseID,String DiseaseNameVN, String ICDXCode,Int32 DiseaseChapterID)
        {
            Diseases Diseases = new Diseases();
            Diseases.DiseaseID = DiseaseID;
            Diseases.DiseaseNameVN = DiseaseNameVN;
            Diseases.ICDXCode = ICDXCode;
            Diseases.DiseaseChapterID = DiseaseChapterID;
            return Diseases;
        }
        #region Primitive Properties


        [DataMemberAttribute()]
        public Int64 DiseaseID
        {
            get
            {
                return _DiseaseID;
            }
            set
            {
                if (_DiseaseID != value)
                {
                    OnDiseaseIDChanging(value);
                    _DiseaseID = value;
                    RaisePropertyChanged("DiseaseID");
                    OnDiseaseIDChanged();
                }
            }
        }
        private Int64 _DiseaseID;
        partial void OnDiseaseIDChanging(Int64 value);
        partial void OnDiseaseIDChanged();

        

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
        public String DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                OnDiseaseNameVNChanging(value);
                _DiseaseNameVN = value;
                RaisePropertyChanged("DiseaseNameVN");
                OnDiseaseNameVNChanged();
            }
        }
        private String _DiseaseNameVN;
        partial void OnDiseaseNameVNChanging(String value);
        partial void OnDiseaseNameVNChanged();

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
