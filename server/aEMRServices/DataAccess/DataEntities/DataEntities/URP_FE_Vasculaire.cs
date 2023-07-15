using System;
using System.Net;
using System.Windows;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class URP_FE_Vasculaire : NotifyChangedBase
    {
        public URP_FE_Vasculaire()
        {
            curURP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();
            curURP_FE_VasculaireExam = new URP_FE_VasculaireExam();
            curURP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();
            curURP_FE_StressDobutamineImages = new URP_FE_StressDobutamineImages();
            curURP_FE_VasculaireAnother = new URP_FE_VasculaireAnother();
        }
        private URP_FE_VasculaireCarotid _curURP_FE_VasculaireCarotid;
        [DataMemberAttribute()]
        public URP_FE_VasculaireCarotid curURP_FE_VasculaireCarotid
        {
            get { return _curURP_FE_VasculaireCarotid; }
            set
            {
                if (_curURP_FE_VasculaireCarotid != value)
                {
                    OnURP_FE_VasculaireCarotidChanging(value);
                    _curURP_FE_VasculaireCarotid = value;
                    RaisePropertyChanged("curURP_FE_VasculaireCarotid");
                    OnURP_FE_VasculaireCarotidChanged();
                }
            }
        }
        partial void OnURP_FE_VasculaireCarotidChanging(URP_FE_VasculaireCarotid value);
        partial void OnURP_FE_VasculaireCarotidChanged();

        private URP_FE_VasculaireExam _curURP_FE_VasculaireExam;
        [DataMemberAttribute()]
        public URP_FE_VasculaireExam curURP_FE_VasculaireExam
        {
            get { return _curURP_FE_VasculaireExam; }
            set
            {
                if (_curURP_FE_VasculaireExam != value)
                {
                    OnURP_FE_VasculaireExamChanging(value);
                    _curURP_FE_VasculaireExam = value;
                    RaisePropertyChanged("curURP_FE_VasculaireExam");
                    OnURP_FE_VasculaireExamChanged();
                }
            }
        }
        partial void OnURP_FE_VasculaireExamChanging(URP_FE_VasculaireExam value);
        partial void OnURP_FE_VasculaireExamChanged();

        private URP_FE_VasculaireAorta _curURP_FE_VasculaireAorta;
        [DataMemberAttribute()]
        public URP_FE_VasculaireAorta curURP_FE_VasculaireAorta
        {
            get { return _curURP_FE_VasculaireAorta; }
            set
            {
                if (_curURP_FE_VasculaireAorta != value)
                {
                    OnURP_FE_VasculaireAortaChanging(value);
                    _curURP_FE_VasculaireAorta = value;
                    RaisePropertyChanged("curURP_FE_VasculaireAorta");
                    OnURP_FE_VasculaireAortaChanged();
                }
            }
        }
        partial void OnURP_FE_VasculaireAortaChanging(URP_FE_VasculaireAorta value);
        partial void OnURP_FE_VasculaireAortaChanged();

        private URP_FE_StressDobutamineImages _curURP_FE_StressDobutamineImages;
        [DataMemberAttribute()]
        public URP_FE_StressDobutamineImages curURP_FE_StressDobutamineImages
        {
            get { return _curURP_FE_StressDobutamineImages; }
            set
            {
                if (_curURP_FE_StressDobutamineImages != value)
                {
                    OnURP_FE_StressDobutamineImagesChanging(value);
                    _curURP_FE_StressDobutamineImages = value;
                    RaisePropertyChanged("curURP_FE_StressDobutamineImages");
                    OnURP_FE_StressDobutamineImagesChanged();
                }
            }
        }
        partial void OnURP_FE_StressDobutamineImagesChanging(URP_FE_StressDobutamineImages value);
        partial void OnURP_FE_StressDobutamineImagesChanged();

        private URP_FE_VasculaireAnother _curURP_FE_VasculaireAnother;
        [DataMemberAttribute()]
        public URP_FE_VasculaireAnother curURP_FE_VasculaireAnother
        {
            get { return _curURP_FE_VasculaireAnother; }
            set
            {
                if (_curURP_FE_VasculaireAnother != value)
                {
                    OnURP_FE_VasculaireAnotherChanging(value);
                    _curURP_FE_VasculaireAnother = value;
                    RaisePropertyChanged("curURP_FE_VasculaireAnother");
                    OnURP_FE_VasculaireAnotherChanged();
                }
            }
        }
        partial void OnURP_FE_VasculaireAnotherChanging(URP_FE_VasculaireAnother value);
        partial void OnURP_FE_VasculaireAnotherChanged();

        [DataMemberAttribute()]
        public PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        [DataMemberAttribute()]
        public List<PCLResultFileStorageDetail> FileForStore { get; set; }
        [DataMemberAttribute()]
        public List<PCLResultFileStorageDetail> FileForDelete { get; set; }
        
        [DataMemberAttribute()]
        public long PCLRequestID
        {
            get
            {
                return _PCLRequestID;
            }
            set
            {
                if (_PCLRequestID != value)
                {
                    OnPCLRequestIDChanging(value);
                    _PCLRequestID = value;
                    RaisePropertyChanged("PCLRequestID");
                    OnPCLRequestIDChanged();
                }
            }
        }
        private long _PCLRequestID;
        partial void OnPCLRequestIDChanging(long value);
        partial void OnPCLRequestIDChanged();
    }
}
