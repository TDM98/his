using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
/*
 * 20221203 #001 BLQ: Create entity
 */

namespace DataEntities
{
    public partial class DTDT_don_thuoc_chi_tiet : EntityBase
    {
        #region Primitive Properties
        public long id_don_thuoc_chi_tiet
        {
            get
            {
                return _id_don_thuoc_chi_tiet;
            }
            set
            {
                _id_don_thuoc_chi_tiet = value;
                RaisePropertyChanged("id_don_thuoc_chi_tiet");
            }
        }
        private long _id_don_thuoc_chi_tiet;

        
        public long id_don_thuoc
        {
            get
            {
                return _id_don_thuoc;
            }
            set
            {
                _id_don_thuoc = value;
                RaisePropertyChanged("id_don_thuoc");
            }
        }
        private long _id_don_thuoc;

        [DataMemberAttribute()]
        public string ma_thuoc
        {
            get
            {
                return _ma_thuoc;
            }
            set
            {
                _ma_thuoc = value;
                RaisePropertyChanged("ma_thuoc");
            }
        }
        private string _ma_thuoc;

        [DataMemberAttribute()]
        public string ten_thuoc
        {
            get
            {
                return _ten_thuoc;
            }
            set
            {
                _ten_thuoc = value;
                RaisePropertyChanged("ten_thuoc");
            }
        }
        private string _ten_thuoc;

        [DataMemberAttribute()]
        public string biet_duoc
        {
            get
            {
                return _biet_duoc;
            }
            set
            {
                _biet_duoc = value;
                RaisePropertyChanged("biet_duoc");
            }
        }
        private string _biet_duoc;
        [DataMemberAttribute()]
        public string don_vi_tinh
        {
            get
            {
                return _don_vi_tinh;
            }
            set
            {
                _don_vi_tinh = value;
                RaisePropertyChanged("don_vi_tinh");
            }
        }
        private string _don_vi_tinh;

        [DataMemberAttribute()]
        public string cach_dung
        {
            get
            {
                return _cach_dung;
            }
            set
            {
                _cach_dung = value;
                RaisePropertyChanged("cach_dung");
            }
        }
        private string _cach_dung;

        public string ham_luong
        {
            get
            {
                return _ham_luong;
            }
            set
            {
                _ham_luong = value;
                RaisePropertyChanged("ham_luong");
            }
        }
        private string _ham_luong;

        public string duong_dung
        {
            get
            {
                return _duong_dung;
            }
            set
            {
                _duong_dung = value;
                RaisePropertyChanged("duong_dung");
            }
        }
        private string _duong_dung;

        public string lieu_dung
        {
            get
            {
                return _lieu_dung;
            }
            set
            {
                _lieu_dung = value;
                RaisePropertyChanged("lieu_dung");
            }
        }
        private string _lieu_dung;

        public string so_dang_ky
        {
            get
            {
                return _so_dang_ky;
            }
            set
            {
                _so_dang_ky = value;
                RaisePropertyChanged("so_dang_ky");
            }
        }
        private string _so_dang_ky;

        [DataMemberAttribute()]
        public int so_luong
        {
            get
            {
                return _so_luong;
            }
            set
            {
                _so_luong = value;
                RaisePropertyChanged("so_luong");
            }
        }
        private int _so_luong;
    
        #endregion
    }
}
