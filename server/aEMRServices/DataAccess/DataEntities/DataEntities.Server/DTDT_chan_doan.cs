using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
/*
* 20221203 #001 BLQ: Create entity
*/

namespace DataEntities
{
    public partial class DTDT_chan_doan : EntityBase
    {
        #region Primitive Properties
        public long don_thuoc_id
        {
            get
            {
                return _don_thuoc_id;
            }
            set
            {
                _don_thuoc_id = value;
                RaisePropertyChanged("don_thuoc_id");
            }
        }
        private long _don_thuoc_id;
        public long chan_doan_id
        {
            get
            {
                return _chan_doan_id;
            }
            set
            {
                _chan_doan_id = value;
                RaisePropertyChanged("chan_doan_id");
            }
        }
        private long _chan_doan_id;

        [DataMemberAttribute()]
        public string ma_chan_doan
        {
            get
            {
                return _ma_chan_doan;
            }
            set
            {
                _ma_chan_doan = value;
                RaisePropertyChanged("ma_chan_doan");
            }
        }
        private string _ma_chan_doan;

        [DataMemberAttribute()]
        public string ten_chan_doan
        {
            get
            {
                return _ten_chan_doan;
            }
            set
            {
                _ten_chan_doan = value;
                RaisePropertyChanged("ten_chan_doan");
            }
        }
        private string _ten_chan_doan;

        [DataMemberAttribute()]
        public string ket_luan
        {
            get
            {
                return _ket_luan;
            }
            set
            {
                _ket_luan = value;
                RaisePropertyChanged("ket_luan");
            }
        }
        private string _ket_luan;

        public bool IsMain
        {
            get
            {
                return _IsMain;
            }
            set
            {
                _IsMain = value;
                RaisePropertyChanged("IsMain");
            }
        }
        private bool _IsMain;
        #endregion
    }
    public partial class DTDT_dot_dung_thuoc : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public int dot
        {
            get
            {
                return _dot;
            }
            set
            {
                _dot = value;
                RaisePropertyChanged("dot");
            }
        }
        private int _dot;

        [DataMemberAttribute()]
        public string tu_ngay
        {
            get
            {
                return _tu_ngay;
            }
            set
            {
                _tu_ngay = value;
                RaisePropertyChanged("tu_ngay");
            }
        }
        private string _tu_ngay;

        [DataMemberAttribute()]
        public string den_ngay
        {
            get
            {
                return _den_ngay;
            }
            set
            {
                _den_ngay = value;
                RaisePropertyChanged("den_ngay");
            }
        }
        private string _den_ngay;

        [DataMemberAttribute()]
        public string so_thang_thuoc
        {
            get
            {
                return _so_thang_thuoc;
            }
            set
            {
                _so_thang_thuoc = value;
                RaisePropertyChanged("so_thang_thuoc");
            }
        }
        private string _so_thang_thuoc;
        #endregion
    }
}
