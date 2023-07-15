using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class AbdominalUltrasound : NotifyChangedBase
    {
        private long abdominalUltrasoundID;
        [DataMemberAttribute()]
        public long AbdominalUltrasoundID
        {
            get
            { 
                return abdominalUltrasoundID; 
            }
            set 
            {
                abdominalUltrasoundID = value;
                RaisePropertyChanged("AbdominalUltrasoundID");
            }
        }

        private long patientPCLReqID;
        [DataMemberAttribute()]
        public long PatientPCLReqID
        {
            get
            {
                return patientPCLReqID;
            }
            set
            {
                patientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
            }
        }

        private long doctorStaffID;
        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return doctorStaffID;
            }
            set
            {
                doctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }

        //Gan
        private string liver;
        [DataMemberAttribute()]
        public string Liver
        {
            get 
            {
                return liver;
            }
            set 
            {
                liver = value;
                RaisePropertyChanged("Liver");
            }
        }

        //Mật
        private string gallbladder;
        [DataMemberAttribute()]
        public string Gallbladder
        {
            get
            { 
                return gallbladder; 
            }
            set
            { 
                gallbladder = value;
                RaisePropertyChanged("Gallbladder");
            }
        }

        //Tụy
        private string pancreas;
        [DataMemberAttribute()]
        public string Pancreas
        {
            get 
            { 
                return pancreas; 
            }
            set
            { 
                pancreas = value;
                RaisePropertyChanged("Pancreas");
            }
        }

        //Lách
        private string spleen;
        [DataMemberAttribute()]
        public string Spleen
        {
            get
            {
                return spleen; 
            }
            set
            { 
                spleen = value;
                RaisePropertyChanged("Spleen");
            }
        }

        //Thận trái
        private string leftKidney;
        [DataMemberAttribute()]
        public string LeftKidney
        {
            get
            { 
                return leftKidney; 
            }
            set
            { 
                leftKidney = value;
                RaisePropertyChanged("LeftKidney");
            }
        }

        //Thận phải
        private string rightKidney;
        [DataMemberAttribute()]
        public string RightKidney
        {
            get 
            { 
                return rightKidney;
            }
            set 
            { 
                rightKidney = value;
                RaisePropertyChanged("RightKidney");
            }
        }

        //Bàng quang
        private string bladder;
        [DataMemberAttribute()]
        public string Bladder
        {
            get
            { 
                return bladder;
            }
            set 
            { 
                bladder = value;
                RaisePropertyChanged("Bladder");
            }
        }

        //Tuyến tiền liệt
        private string prostate;
        [DataMemberAttribute()]
        public string Prostate
        {
            get 
            {
                return prostate;
            }
            set
            { 
                prostate = value;
                RaisePropertyChanged("Prostate");
            }
        }

        //Tử cung
        private string uterus;
        [DataMemberAttribute()]
        public string Uterus
        {
            get
            {
                return uterus;
            }
            set
            {
                uterus = value;
                RaisePropertyChanged("Uterus");
            }
        }

        //Phần phụ phải (buồng trứng phải)
        private string rightOvary;
        [DataMemberAttribute()]
        public string RightOvary
        {
            get
            {
                return rightOvary;
            }
            set
            {
                rightOvary = value;
                RaisePropertyChanged("RightOvary");
            }
        }

        //Phần phụ trái (buồng trứng trái)
        private string leftOvary;
        [DataMemberAttribute()]
        public string LeftOvary
        {
            get
            {
                return leftOvary;
            }
            set
            {
                leftOvary = value;
                RaisePropertyChanged("LeftOvary");
            }
        }

        //Dịch ổ bụng
        private string peritonealFluid;
        [DataMemberAttribute()]
        public string PeritonealFluid
        {
            get
            { 
                return peritonealFluid; 
            }
            set 
            {
                peritonealFluid = value;
                RaisePropertyChanged("PeritonealFluid");
            }
        }

        //Dịch màng phổi
        private string pleuralFluid;
        [DataMemberAttribute()]
        public string PleuralFluid
        {
            get 
            {
                return pleuralFluid;
            }
            set 
            { 
                pleuralFluid = value;
                RaisePropertyChanged("PleuralFluid");
            }
        }

        //Động mạch chủ bụng
        private string abdominalAortic;
        [DataMemberAttribute()]
        public string AbdominalAortic
        {
            get
            { 
                return abdominalAortic; 
            }
            set
            { 
                abdominalAortic = value;
                RaisePropertyChanged("AbdominalAortic");
            }
        }

        private string conclusion;
        [DataMemberAttribute()]
        public string Conclusion
        {
            get
            {
                return conclusion;
            }
            set
            {
                conclusion = value;
                RaisePropertyChanged("Conclusion");
            }
        }
        
    }
}
