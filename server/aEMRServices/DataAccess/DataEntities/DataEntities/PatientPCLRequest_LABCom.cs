using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public partial class PatientPCLRequest_LABCom : NotifyChangedBase
    {   
        #region Primitive Properties

        [DataMemberAttribute()]
         public string SoPhieuChiDinh
        {
            get
            {
                return _SoPhieuChiDinh;
            }
            set
            {
                if (_SoPhieuChiDinh != value)
                {
                    OnSoPhieuChiDinhChanging(value);
                    _SoPhieuChiDinh = value;
                    RaisePropertyChanged("SoPhieuChiDinh");
                    OnSoPhieuChiDinhChanged();
                }
            }
        }
        private string _SoPhieuChiDinh;
        partial void OnSoPhieuChiDinhChanging(string value);
        partial void OnSoPhieuChiDinhChanged();


        [DataMemberAttribute()]
        public string MaDichVu
        {
            get
            {
                return _MaDichVu;
            }
            set
            {
                OnMaDichVuChanging(value);
                _MaDichVu = value;
                RaisePropertyChanged("MaDichVu");
                OnMaDichVuChanged();
            }
        }
        private string _MaDichVu;
        partial void OnMaDichVuChanging(string value);
        partial void OnMaDichVuChanged();


        [DataMemberAttribute()]
        public string KetQua
        {
            get
            {
                return _KetQua;
            }
            set
            {
                OnKetQuaChanging(value);
                _KetQua = value;
                RaisePropertyChanged("KetQua");
                OnKetQuaChanged();
            }
        }
        private string _KetQua;
        partial void OnKetQuaChanging(string value);
        partial void OnKetQuaChanged();
        #endregion
    }
}
