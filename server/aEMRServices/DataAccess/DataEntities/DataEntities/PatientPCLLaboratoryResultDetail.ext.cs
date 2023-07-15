using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;
/*
 * 20210527 #001 TNHX: 317 Thêm mã máy xét nghiệm con
 */
namespace DataEntities
{
    public partial class PatientPCLLaboratoryResultDetail : NotifyChangedBase
    {
        #region Extended Properties

        [DataMemberAttribute()]
        public String PCLExamTypeCode
        {
            get
            {
                return _PCLExamTypeCode;
            }
            set
            {
                OnPCLExamTypeCodeChanging(value);
                _PCLExamTypeCode = value;
                RaisePropertyChanged("PCLExamTypeCode");
                OnPCLExamTypeCodeChanged();
            }
        }
        private String _PCLExamTypeCode;
        partial void OnPCLExamTypeCodeChanging(String value);
        partial void OnPCLExamTypeCodeChanged();

        [DataMemberAttribute()]
        public long? PCLExamGroupID
        {
            get
            {
                return _PCLExamGroupID;
            }
            set
            {
                _PCLExamGroupID = value;
                RaisePropertyChanged("PCLExamGroupID");
            }
        }
        private long? _PCLExamGroupID;


        [DataMemberAttribute()]
        public Nullable<Double> PCLIndMinValue
        {
            get
            {
                return _PCLIndMinValue;
            }
            set
            {
                OnPCLIndMinValueChanging(value);
                _PCLIndMinValue = value;
                RaisePropertyChanged("PCLIndMinValue");
                OnPCLIndMinValueChanged();
            }
        }
        private Nullable<Double> _PCLIndMinValue;
        partial void OnPCLIndMinValueChanging(Nullable<Double> value);
        partial void OnPCLIndMinValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndMaxValue
        {
            get
            {
                return _PCLIndMaxValue;
            }
            set
            {
                OnPCLIndMaxValueChanging(value);
                _PCLIndMaxValue = value;
                RaisePropertyChanged("PCLIndMaxValue");
                OnPCLIndMaxValueChanged();
            }
        }
        private Nullable<Double> _PCLIndMaxValue;
        partial void OnPCLIndMaxValueChanging(Nullable<Double> value);
        partial void OnPCLIndMaxValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndAVGValue
        {
            get
            {
                return _PCLIndAVGValue;
            }
            set
            {
                OnPCLIndAVGValueChanging(value);
                _PCLIndAVGValue = value;
                RaisePropertyChanged("PCLIndAVGValue");
                OnPCLIndAVGValueChanged();
            }
        }
        private Nullable<Double> _PCLIndAVGValue;
        partial void OnPCLIndAVGValueChanging(Nullable<Double> value);
        partial void OnPCLIndAVGValueChanged();

        [DataMemberAttribute()]
        public Nullable<Double> PCLIndDeclinationValue
        {
            get
            {
                return _PCLIndDeclinationValue;
            }
            set
            {
                OnPCLIndDeclinationValueChanging(value);
                _PCLIndDeclinationValue = value;
                RaisePropertyChanged("PCLIndDeclinationValue");
                OnPCLIndDeclinationValueChanged();
            }
        }
        private Nullable<Double> _PCLIndDeclinationValue;
        partial void OnPCLIndDeclinationValueChanging(Nullable<Double> value);
        partial void OnPCLIndDeclinationValueChanged();

        [DataMemberAttribute()]
        public Nullable<Boolean> ClosedInterval
        {
            get
            {
                return _ClosedInterval;
            }
            set
            {
                OnClosedIntervalChanging(value);
                _ClosedInterval = value;
                RaisePropertyChanged("ClosedInterval");
                OnClosedIntervalChanged();
            }
        }
        private Nullable<Boolean> _ClosedInterval;
        partial void OnClosedIntervalChanging(Nullable<Boolean> value);
        partial void OnClosedIntervalChanged();


        [DataMemberAttribute()]
        public String PCLIndOtherValue
        {
            get
            {
                return _PCLIndOtherValue;
            }
            set
            {
                OnPCLIndOtherValueChanging(value);
                _PCLIndOtherValue = value;
                RaisePropertyChanged("PCLIndOtherValue");
                OnPCLIndOtherValueChanged();
            }
        }
        private String _PCLIndOtherValue;
        partial void OnPCLIndOtherValueChanging(String value);
        partial void OnPCLIndOtherValueChanged();

        [DataMemberAttribute()]
        public String SlidingScale
        {
            get
            {
                return _SlidingScale;
            }
            set
            {
                OnSlidingScaleChanging(value);
                _SlidingScale = value;
                RaisePropertyChanged("SlidingScale");
                OnSlidingScaleChanged();
            }
        }
        private String _SlidingScale;
        partial void OnSlidingScaleChanging(String value);
        partial void OnSlidingScaleChanged();


        [DataMemberAttribute()]
        public String MeasurementUnit
        {
            get
            {
                return _MeasurementUnit;
            }
            set
            {
                OnMeasurementUnitChanging(value);
                _MeasurementUnit = value;
                RaisePropertyChanged("MeasurementUnit");
                OnMeasurementUnitChanged();
            }
        }
        private String _MeasurementUnit;
        partial void OnMeasurementUnitChanging(String value);
        partial void OnMeasurementUnitChanged();


        [DataMemberAttribute()]
        public byte GenderType
        {
            get
            {
                return _GenderType;
            }
            set
            {
                OnGenderTypeChanging(value);
                _GenderType = value;
                RaisePropertyChanged("GenderType");
                OnGenderTypeChanged();
            }
        }
        private byte _GenderType;
        partial void OnGenderTypeChanging(byte value);
        partial void OnGenderTypeChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> RequestedDate
        {
            get
            {
                return _RequestedDate;
            }
            set
            {
                OnRequestedDateChanging(value);
                _RequestedDate = value;
                RaisePropertyChanged("RequestedDate");
                OnRequestedDateChanged();
            }
        }
        private Nullable<DateTime> _RequestedDate;
        partial void OnRequestedDateChanging(Nullable<DateTime> value);
        partial void OnRequestedDateChanged();

        [DataMemberAttribute()]
        public Boolean PathologicalSigns
        {
            get
            {
                return _PathologicalSigns;
            }
            set
            {
                OnPathologicalSignsChanging(value);
                _PathologicalSigns = value;
                RaisePropertyChanged("PathologicalSigns");
                OnPathologicalSignsChanged();
            }
        }
        private Boolean _PathologicalSigns;
        partial void OnPathologicalSignsChanging(Boolean value);
        partial void OnPathologicalSignsChanged();

        //▼====: #001
        [DataMemberAttribute()]
        public string HIRepResourceCode
        {
            get
            {
                return _HIRepResourceCode;
            }
            set
            {
                _HIRepResourceCode = value;
                RaisePropertyChanged("HIRepResourceCode");
            }
        }
        private string _HIRepResourceCode;
        [DataMemberAttribute()]
        public bool IsTemporaryBlock
        {
            get
            {
                return _IsTemporaryBlock;
            }
            set
            {
                _IsTemporaryBlock = value;
                RaisePropertyChanged("IsTemporaryBlock");
            }
        }
        private bool _IsTemporaryBlock;
        //▲====: #001
        #endregion

        #region XMLparsing GetInfo for PatientPClAppointmentIndicator
        public static string[] Nodes = new string[5] { "PCLExamTypeID", "Value", "IsNormal", "NumberOfTest", "Comments" };
        public static string[] Fields = new string[5] { "PCLExamTypeID", "Value", "IsNormal", "NumberOfTest", "Comments" };
        public static byte IsNormalIdx = 2;
        public static bool IsContainsField(string fieldName)
        {
            try
            {
                return Array.IndexOf(Fields, fieldName) >= 0 ? true : false;
            }
            catch
            {
                return false;
            }
        }

        private static string BuildXmlString(string xmlRootName, string values)
        {
            StringBuilder xmlString = new StringBuilder();
            xmlString.AppendFormat("<" + xmlRootName + ">{0}</" + xmlRootName + ">", values);
            return xmlString.ToString();
        }

        private static string FormatFieldXML(string data, string xNode)
        {
            if (data != null)
                return String.Format(" " + xNode + "='{0}'", data);
            else
                return "";
        }

        private static void BuildXMLStringOfRow(int rowIdx, StringBuilder strBuilder, List<string> lstFields)
        {
            //strBuilder.Append("<?xml version=''1.0'' encoding=''UTF-8''?>");
            strBuilder.Append("<Row");
            strBuilder.Append(FormatFieldXML(rowIdx.ToString().Trim(), "RowID"));
            strBuilder.Append(String.Join("", lstFields.ToArray()));
            strBuilder.Append(" />");
        }

        public static string ConvertToXML(List<PatientPCLLaboratoryResultDetail> PtPCLLabResultDetailsObj)
        {
            StringBuilder strBuilder = new StringBuilder();
            List<string> lstData = new List<string>();
            int RowIdx = 0;
            foreach (PatientPCLLaboratoryResultDetail p in PtPCLLabResultDetailsObj)
            {
                RowIdx++;
                Type oType = p.GetType();
                lstData.Clear();
                foreach (System.Reflection.PropertyInfo r in oType.GetProperties())
                {
                    if (PatientPCLLaboratoryResultDetail.IsContainsField(r.Name))
                    {
                        string propertyName = r.Name.Replace(" ", "");
                        string propertyValue = "";
                        object objectValue = null;
                        if (r.CanRead)
                        {
                            objectValue = r.GetValue(p, null);
                            try
                            {
                                propertyValue = objectValue.ToString();
                            }
                            catch
                            {
                                propertyValue = null;
                            }
                        }
                        if (objectValue != null)
                            lstData.Add(FormatFieldXML(propertyValue, Nodes[Array.IndexOf(Fields, propertyName)]));
                    }
                }
                BuildXMLStringOfRow(RowIdx, strBuilder, lstData);
            }
            string sResult = BuildXmlString("Tbl", strBuilder.ToString());
            return sResult;
        }

        public bool GetIsNormalValue()
        {
            double? PCLIndvalue = null;
            try
            {
                PCLIndvalue = double.Parse(Value);
            }
            catch { }

            if (PCLIndvalue != null)// PCL Ind value is numeric
            {
                if (PCLIndMinValue != null && PCLIndMaxValue != null)
                {
                    return (((double)PCLIndvalue >= (double)PCLIndMinValue) && ((double)PCLIndvalue <= (double)PCLIndMaxValue));
                }
                else
                {
                    if (PCLIndMinValue != null)
                    {
                        if (ClosedInterval != null && ClosedInterval == true)
                        {
                            return ((double)PCLIndvalue >= (double)PCLIndMinValue);
                        }
                        else
                        {
                            return ((double)PCLIndvalue > (double)PCLIndMinValue);
                        }
                    }
                    else
                    {
                        if (PCLIndMaxValue != null)
                        {
                            if (ClosedInterval != null && ClosedInterval == true)
                            {
                                return ((double)PCLIndvalue <= (double)PCLIndMaxValue);
                            }
                            else
                            {
                                return ((double)PCLIndvalue < (double)PCLIndMaxValue);
                            }
                        }
                        else//PCLIndMinValue and PCLIndMaxValue are null
                        {
                            if (PCLIndAVGValue != null)
                            {
                                if (PCLIndDeclinationValue != null)
                                {
                                    //Caculate range of value
                                    double LeftValue = (double)PCLIndAVGValue - (double)PCLIndDeclinationValue;
                                    double RightValue = (double)PCLIndAVGValue + (double)PCLIndDeclinationValue;
                                    return (((double)PCLIndvalue >= LeftValue) && ((double)PCLIndvalue <= RightValue));
                                }
                                else
                                {
                                    return ((double)PCLIndvalue == (double)PCLIndAVGValue);
                                }
                            }
                            return false;
                        }
                    }
                }
            }
            else//Other value PCLIndValue is text
            {
                string IndOtherValue = PCLIndOtherValue.Replace("(", "").Replace(")", "").TrimEnd();
                string IndValue = Value.Replace("(", "").Replace(")", "").TrimEnd();
                return IndOtherValue.Contains(IndValue);
            }
        }
        #endregion

    }
}
