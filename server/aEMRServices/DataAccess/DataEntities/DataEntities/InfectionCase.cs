using eHCMS.Services.Core.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace DataEntities
{
    [DataContract]
    public partial class InfectionCase : NotifyChangedBase
    {
        private long _InfectionCaseID;
        private long _V_InfectionType;
        private string _InfectionTypeNote;
        private long _InfectedByVirusID;
        private long _V_BloodSmearResult;
        private long _V_BloodSmearMethod;
        private DateTime _StartDate;
        private DateTime? _EndDate;
        private string _Notes;
        private long _PtRegistrationID;
        private long _PatientID;
        private long _DeptID;
        private long _InfectionICD10ListID1;
        private long? _InfectionICD10ListID2;
        private ObservableCollection<InfectionICD10Item> _InfectionICD10ListID1Items;
        private ObservableCollection<InfectionICD10Item> _InfectionICD10ListID2Items;
        private string _DiagnosisFinal1;
        private string _DiagnosisFinal2;
        private ObservableCollection<AntibioticTreatment> _AntibioticTreatmentCollection;
        private PatientRegistration _CurrentRegistration;
        [DataMemberAttribute]
        public long InfectionCaseID
        {
            get
            {
                return _InfectionCaseID;
            }
            set
            {
                if (_InfectionCaseID == value)
                {
                    return;
                }
                _InfectionCaseID = value;
                RaisePropertyChanged("InfectionCaseID");
            }
        }
        [DataMemberAttribute]
        public long V_InfectionType
        {
            get
            {
                return _V_InfectionType;
            }
            set
            {
                if (_V_InfectionType == value)
                {
                    return;
                }
                _V_InfectionType = value;
                RaisePropertyChanged("V_InfectionType");
            }
        }
        [DataMemberAttribute]
        public string InfectionTypeNote
        {
            get
            {
                return _InfectionTypeNote;
            }
            set
            {
                if (_InfectionTypeNote == value)
                {
                    return;
                }
                _InfectionTypeNote = value;
                RaisePropertyChanged("InfectionTypeNote");
            }
        }
        [DataMemberAttribute]
        public long InfectedByVirusID
        {
            get
            {
                return _InfectedByVirusID;
            }
            set
            {
                if (_InfectedByVirusID == value)
                {
                    return;
                }
                _InfectedByVirusID = value;
                RaisePropertyChanged("InfectedByVirusID");
            }
        }
        [DataMemberAttribute]
        public long V_BloodSmearResult
        {
            get
            {
                return _V_BloodSmearResult;
            }
            set
            {
                if (_V_BloodSmearResult == value)
                {
                    return;
                }
                _V_BloodSmearResult = value;
                RaisePropertyChanged("V_BloodSmearResult");
            }
        }
        [DataMemberAttribute]
        public long V_BloodSmearMethod
        {
            get
            {
                return _V_BloodSmearMethod;
            }
            set
            {
                if (_V_BloodSmearMethod == value)
                {
                    return;
                }
                _V_BloodSmearMethod = value;
                RaisePropertyChanged("V_BloodSmearMethod");
            }
        }
        [DataMemberAttribute]
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                if (_StartDate == value)
                {
                    return;
                }
                _StartDate = value;
                RaisePropertyChanged("StartDate");
            }
        }
        [DataMemberAttribute]
        public DateTime? EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                if (_EndDate == value)
                {
                    return;
                }
                _EndDate = value;
                RaisePropertyChanged("EndDate");
            }
        }
        [DataMemberAttribute]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes == value)
                {
                    return;
                }
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
        [DataMemberAttribute]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        [DataMemberAttribute]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID == value)
                {
                    return;
                }
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        [DataMemberAttribute]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                if (_DeptID == value)
                {
                    return;
                }
                _DeptID = value;
                RaisePropertyChanged("DeptID");
            }
        }
        [DataMemberAttribute]
        public long InfectionICD10ListID1
        {
            get
            {
                return _InfectionICD10ListID1;
            }
            set
            {
                if (_InfectionICD10ListID1 == value)
                {
                    return;
                }
                _InfectionICD10ListID1 = value;
                RaisePropertyChanged("InfectionICD10ListID1");
            }
        }
        [DataMemberAttribute]
        public long? InfectionICD10ListID2
        {
            get
            {
                return _InfectionICD10ListID2;
            }
            set
            {
                if (_InfectionICD10ListID2 == value)
                {
                    return;
                }
                _InfectionICD10ListID2 = value;
                RaisePropertyChanged("InfectionICD10ListID2");
            }
        }
        [DataMemberAttribute]
        public ObservableCollection<InfectionICD10Item> InfectionICD10ListID1Items
        {
            get
            {
                return _InfectionICD10ListID1Items;
            }
            set
            {
                if (_InfectionICD10ListID1Items == value)
                {
                    return;
                }
                _InfectionICD10ListID1Items = value;
                RaisePropertyChanged("InfectionICD10ListID1Items");
            }
        }
        [DataMemberAttribute]
        public ObservableCollection<InfectionICD10Item> InfectionICD10ListID2Items
        {
            get
            {
                return _InfectionICD10ListID2Items;
            }
            set
            {
                if (_InfectionICD10ListID2Items == value)
                {
                    return;
                }
                _InfectionICD10ListID2Items = value;
                RaisePropertyChanged("InfectionICD10ListID2Items");
            }
        }
        [DataMemberAttribute]
        public string DiagnosisFinal1
        {
            get
            {
                return _DiagnosisFinal1;
            }
            set
            {
                if (_DiagnosisFinal1 == value)
                {
                    return;
                }
                _DiagnosisFinal1 = value;
                RaisePropertyChanged("DiagnosisFinal1");
            }
        }
        [DataMemberAttribute]
        public string DiagnosisFinal2
        {
            get
            {
                return _DiagnosisFinal2;
            }
            set
            {
                if (_DiagnosisFinal2 == value)
                {
                    return;
                }
                _DiagnosisFinal2 = value;
                RaisePropertyChanged("DiagnosisFinal2");
            }
        }
        [DataMemberAttribute]
        public ObservableCollection<AntibioticTreatment> AntibioticTreatmentCollection
        {
            get
            {
                return _AntibioticTreatmentCollection;
            }
            set
            {
                if (_AntibioticTreatmentCollection == value)
                {
                    return;
                }
                _AntibioticTreatmentCollection = value;
                RaisePropertyChanged("AntibioticTreatmentCollection");
            }
        }
        [DataMemberAttribute]
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _CurrentRegistration;
            }
            set
            {
                if (_CurrentRegistration == value)
                {
                    return;
                }
                _CurrentRegistration = value;
                RaisePropertyChanged("CurrentRegistration");
            }
        }
        public string ConvertToXml()
        {
            var xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("InfectionCase", new XElement[] {
                    new XElement("InfectionCaseID", InfectionCaseID),
                    new XElement("V_InfectionType", V_InfectionType),
                    new XElement("InfectionTypeNote", InfectionTypeNote),
                    new XElement("InfectedByVirusID", InfectedByVirusID),
                    new XElement("V_BloodSmearResult", V_BloodSmearResult),
                    new XElement("V_BloodSmearMethod", V_BloodSmearMethod),
                    new XElement("StartDate", StartDate.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    new XElement("EndDate", EndDate.HasValue && EndDate.Value > new DateTime(2010,01,01) ? EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null),
                    new XElement("Notes", Notes),
                    new XElement("PtRegistrationID", PtRegistrationID),
                    new XElement("PatientID", PatientID),
                    new XElement("DeptID", DeptID),
                    new XElement("InfectionICD10ListID1", InfectionICD10ListID1),
                    new XElement("InfectionICD10ListID2", InfectionICD10ListID2),
                    new XElement("DiagnosisFinal1", DiagnosisFinal1),
                    new XElement("DiagnosisFinal2", DiagnosisFinal2),
                    new XElement("InfectionICD10ListID1Items", InfectionICD10ListID1Items == null || InfectionICD10ListID1Items.Count == 0 ? null :
                        InfectionICD10ListID1Items.Select(x => new XElement("item", new XElement[] {
                            new XElement("InfectionICD10ID", x.InfectionICD10ID),
                            new XElement("InfectionICD10ListID", x.InfectionICD10ListID),
                            new XElement("ICD10Code", x.ICD10Code),
                            new XElement("ICD10Name", x.ICD10Name),
                            new XElement("IsMain", x.IsMain)
                        }))),
                    new XElement("InfectionICD10ListID2Items", InfectionICD10ListID2Items == null || InfectionICD10ListID2Items.Count == 0 ? null :
                        InfectionICD10ListID2Items.Select(x => new XElement("item", new XElement[] {
                            new XElement("InfectionICD10ID", x.InfectionICD10ID),
                            new XElement("InfectionICD10ListID", x.InfectionICD10ListID),
                            new XElement("ICD10Code", x.ICD10Code),
                            new XElement("ICD10Name", x.ICD10Name),
                            new XElement("IsMain", x.IsMain)
                        })))
                }));
            return xDocument.ToString();
        }
    }
    public class InfectionICD10Item : NotifyChangedBase
    {
        private long _InfectionICD10ID;
        private long _InfectionICD10ListID;
        private string _ICD10Code;
        private string _ICD10Name;
        private bool _IsMain;
        [DataMemberAttribute]
        public long InfectionICD10ID
        {
            get
            {
                return _InfectionICD10ID;
            }
            set
            {
                if (_InfectionICD10ID == value)
                {
                    return;
                }
                _InfectionICD10ID = value;
                RaisePropertyChanged("InfectionICD10ID");
            }
        }
        [DataMemberAttribute]
        public long InfectionICD10ListID
        {
            get
            {
                return _InfectionICD10ListID;
            }
            set
            {
                if (_InfectionICD10ListID == value)
                {
                    return;
                }
                _InfectionICD10ListID = value;
                RaisePropertyChanged("InfectionICD10ListID");
            }
        }
        [DataMemberAttribute]
        public string ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                if (_ICD10Code == value)
                {
                    return;
                }
                _ICD10Code = value;
                RaisePropertyChanged("ICD10Code");
            }
        }
        [DataMemberAttribute]
        public string ICD10Name
        {
            get
            {
                return _ICD10Name;
            }
            set
            {
                if (_ICD10Name == value)
                {
                    return;
                }
                _ICD10Name = value;
                RaisePropertyChanged("ICD10Name");
            }
        }
        [DataMemberAttribute]
        public bool IsMain
        {
            get
            {
                return _IsMain;
            }
            set
            {
                if (_IsMain == value)
                {
                    return;
                }
                _IsMain = value;
                RaisePropertyChanged("IsMain");
            }
        }
    }
    public class InfectionVirus : NotifyChangedBase
    {
        private long _InfectionVirusID;
        private string _InfectionVirusName;
        private string _Notes;
        [DataMemberAttribute]
        public long InfectionVirusID
        {
            get
            {
                return _InfectionVirusID;
            }
            set
            {
                if (_InfectionVirusID == value)
                {
                    return;
                }
                _InfectionVirusID = value;
                RaisePropertyChanged("InfectionVirusID");
            }
        }
        [DataMemberAttribute]
        public string InfectionVirusName
        {
            get
            {
                return
                  _InfectionVirusName;
            }
            set
            {
                if (_InfectionVirusName == value)
                {
                    return;
                }
                _InfectionVirusName = value;
                RaisePropertyChanged("InfectionVirusName");
            }
        }
        [DataMemberAttribute]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes == value)
                {
                    return;
                }
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
    }
    public class AntibioticTreatment : NotifyChangedBase
    {
        private long _AntibioticTreatmentID;
        private long _InfectionCaseID;
        private DateTime _StartDate;
        private DateTime? _EndDate;
        private ObservableCollection<AntibioticTreatmentMedProductDetail> _AntibioticTreatmentMedProductDetailCollection;
        private string _AntibioticTreatmentTitle;
        private long? _PtRegistrationID;
        private long? _IntPtDiagDrInstructionID;
        private long _V_AntibioticTreatmentType = (long)AllLookupValues.V_AntibioticTreatmentType.InfectionCase;
        [DataMemberAttribute]
        public long AntibioticTreatmentID
        {
            get
            {
                return _AntibioticTreatmentID;
            }
            set
            {
                if (_AntibioticTreatmentID == value)
                {
                    return;
                }
                _AntibioticTreatmentID = value;
                RaisePropertyChanged("AntibioticTreatmentID");
            }
        }
        [DataMemberAttribute]
        public long InfectionCaseID
        {
            get
            {
                return _InfectionCaseID;
            }
            set
            {
                if (_InfectionCaseID == value)
                {
                    return;
                }
                _InfectionCaseID = value;
                RaisePropertyChanged("InfectionCaseID");
            }
        }
        [DataMemberAttribute]
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                if (_StartDate == value)
                {
                    return;
                }
                _StartDate = value;
                RaisePropertyChanged("StartDate");
            }
        }
        [DataMemberAttribute]
        public DateTime? EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                if (_EndDate == value)
                {
                    return;
                }
                _EndDate = value;
                RaisePropertyChanged("EndDate");
            }
        }
        [DataMemberAttribute]
        public ObservableCollection<AntibioticTreatmentMedProductDetail> AntibioticTreatmentMedProductDetailCollection
        {
            get
            {
                return _AntibioticTreatmentMedProductDetailCollection;
            }
            set
            {
                if (_AntibioticTreatmentMedProductDetailCollection == value)
                {
                    return;
                }
                _AntibioticTreatmentMedProductDetailCollection = value;
                RaisePropertyChanged("AntibioticTreatmentMedProductDetailCollection");
            }
        }
        [DataMemberAttribute]
        public string AntibioticTreatmentTitle
        {
            get
            {
                return _AntibioticTreatmentTitle;
            }
            set
            {
                if (_AntibioticTreatmentTitle == value)
                {
                    return;
                }
                _AntibioticTreatmentTitle = value;
                RaisePropertyChanged("AntibioticTreatmentTitle");
            }
        }
        [DataMemberAttribute]
        public long? PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        [DataMemberAttribute]
        public long? IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                if (_IntPtDiagDrInstructionID == value)
                {
                    return;
                }
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }
        [DataMemberAttribute]
        public long V_AntibioticTreatmentType
        {
            get
            {
                return _V_AntibioticTreatmentType;
            }
            set
            {
                if (_V_AntibioticTreatmentType == value)
                {
                    return;
                }
                _V_AntibioticTreatmentType = value;
                RaisePropertyChanged("V_AntibioticTreatmentType");
            }
        }
        public string ConvertToXml()
        {
            var xDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("AntibioticTreatment", new XElement[] {
                    new XElement("AntibioticTreatmentID", AntibioticTreatmentID),
                    new XElement("InfectionCaseID", InfectionCaseID),
                    new XElement("StartDate", StartDate.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                    new XElement("EndDate", EndDate.HasValue && EndDate.Value > new DateTime(2010,01,01) ? EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null),
                    new XElement("AntibioticTreatmentTitle", AntibioticTreatmentTitle),
                    new XElement("PtRegistrationID", PtRegistrationID),
                    new XElement("IntPtDiagDrInstructionID", IntPtDiagDrInstructionID),
                    new XElement("V_AntibioticTreatmentType", V_AntibioticTreatmentType),
                    new XElement("AntibioticTreatmentMedProductDetailCollection", AntibioticTreatmentMedProductDetailCollection == null || AntibioticTreatmentMedProductDetailCollection.Count == 0 ? null :
                        AntibioticTreatmentMedProductDetailCollection.Select(x => new XElement("item", new XElement[] {
                            new XElement("AntibioticMedProductID", x.AntibioticMedProductID),
                            new XElement("GenMedProductID", x.RefGenMedProductDetail == null ? null : (long?)x.RefGenMedProductDetail.GenMedProductID),
                            new XElement("Quantity", x.Quantity),
                            new XElement("Notes", x.Notes)
                        })))
                }));
            return xDocument.ToString();
        }
    }
    public class AntibioticTreatmentMedProductDetail : NotifyChangedBase
    {
        private long _AntibioticMedProductID;
        private long _AntibioticTreatmentID;
        private RefGenMedProductDetails _RefGenMedProductDetail;
        private decimal _Quantity;
        private string _Notes;
        public long AntibioticMedProductID
        {
            get
            {
                return _AntibioticMedProductID;
            }
            set
            {
                if (_AntibioticMedProductID == value)
                {
                    return;
                }
                _AntibioticMedProductID = value;
                RaisePropertyChanged("AntibioticMedProductID");
            }
        }
        [DataMemberAttribute]
        public long AntibioticTreatmentID
        {
            get
            {
                return _AntibioticTreatmentID;
            }
            set
            {
                if (_AntibioticTreatmentID == value)
                {
                    return;
                }
                _AntibioticTreatmentID = value;
                RaisePropertyChanged("AntibioticTreatmentID");
            }
        }
        [DataMemberAttribute]
        public RefGenMedProductDetails RefGenMedProductDetail
        {
            get
            {
                return _RefGenMedProductDetail;
            }
            set
            {
                if (_RefGenMedProductDetail == value)
                {
                    return;
                }
                _RefGenMedProductDetail = value;
                RaisePropertyChanged("RefGenMedProductDetail");
            }
        }
        [DataMemberAttribute]
        public decimal Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                if (_Quantity == value)
                {
                    return;
                }
                _Quantity = value;
                RaisePropertyChanged("Quantity");
            }
        }
        [DataMemberAttribute]
        public string Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                if (_Notes == value)
                {
                    return;
                }
                _Notes = value;
                RaisePropertyChanged("Notes");
            }
        }
    }
    public class AntibioticTreatmentMedProductDetailSummaryContent : AntibioticTreatmentMedProductDetail
    {
        private InfectionCase _CurrentInfectionCase;
        private AntibioticTreatment _CurrentAntibioticTreatment;
        [DataMemberAttribute]
        public InfectionCase CurrentInfectionCase
        {
            get
            {
                return _CurrentInfectionCase;
            }
            set
            {
                if (_CurrentInfectionCase == value)
                {
                    return;
                }
                _CurrentInfectionCase = value;
                RaisePropertyChanged("CurrentInfectionCase");
            }
        }
        [DataMemberAttribute]
        public AntibioticTreatment CurrentAntibioticTreatment
        {
            get
            {
                return _CurrentAntibioticTreatment;
            }
            set
            {
                if (_CurrentAntibioticTreatment == value)
                {
                    return;
                }
                _CurrentAntibioticTreatment = value;
                RaisePropertyChanged("CurrentAntibioticTreatment");
            }
        }
    }
}