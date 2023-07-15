using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Globalization;

namespace DataEntities
{
    public class PrescriptionDetailSchedulesLieuDung : NotifyChangedBase
    {
        private Single _ID;
        public Single ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    RaisePropertyChanged("ID");
                }
            }
        }

        private String _Name;
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public PrescriptionDetailSchedulesLieuDung()
        {
        }

        public PrescriptionDetailSchedulesLieuDung(string _Name)
        {
            Name = _Name;
            change();
        }

        public PrescriptionDetailSchedulesLieuDung(PrescriptionDetailSchedulesLieuDung objTmp)
        {
            ID = objTmp.ID;
            Name = objTmp.Name;
            change();
        }

        public override bool Equals(object obj)
        {
            PrescriptionDetailSchedulesLieuDung seletedStore = obj as PrescriptionDetailSchedulesLieuDung;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ID == seletedStore.ID;
        }
        public void change()
        {
            try
            {
                ID = 0;
                if (Name.Contains("/"))
                {

                    try
                    {
                        string[] arr = Name.Split('/');
                        ID = 0;
                        if (arr.Length == 2)
                        {
                            //KMx: Không được Round số lượng. Nếu không sẽ bị sai khi gặp số lẻ (10/06/2014 16:34).
                            //ID = Convert.ToSingle(Math.Round(Convert.ToSingle(arr[0]) / Convert.ToSingle(arr[1]), 3));
                            Single firstNum = Convert.ToSingle(arr[0]);
                            Single secondNum = Convert.ToSingle(arr[1]);
                            if (secondNum > 0)
                            {
                                ID = Convert.ToSingle(firstNum / secondNum);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    if (Name.Contains(","))
                    {
                        Name.Replace(",", ".");
                    }
                    if (Name.Contains("-"))
                    {
                        Name.Replace("-", "");
                    }
                    try
                    {
                        ID = Convert.ToSingle(Name, new CultureInfo("en-US"));
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
                Name = "";
                ID = 0;
            }

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}