using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class EntitiesEdit<T> : NotifyChangedBase, IEditableObject
    {

        public EntitiesEdit()
        {
            CurObject=new ObservableCollection<T>();
            NewObject=new ObservableCollection<T>();
            TempObject=new ObservableCollection<T>();
            DeleteObject=new ObservableCollection<T>();
            UpdateObject=new ObservableCollection<T>();
        }
        public EntitiesEdit(bool IsCheckByToString)
        {
            CurObject = new ObservableCollection<T>();
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
            _IsCheckByToString = IsCheckByToString;
        }

        public EntitiesEdit(ObservableCollection<T> _CurObject)
        {
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
            CurObject = _CurObject;
            BackupObject = _CurObject;
        }

        public EntitiesEdit(ObservableCollection<T> _CurObject, bool IsCheckByToString)
        {
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
            CurObject = _CurObject;
            BackupObject = _CurObject;
            _IsCheckByToString = IsCheckByToString;
        }

        private EntitiesEdit<T> _tempEntitiesEdit;
        private bool _IsCheckByToString=false;
        public void BeginEdit()
        {
            _tempEntitiesEdit = (EntitiesEdit<T>)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempEntitiesEdit)
                CopyFrom(_tempEntitiesEdit);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(EntitiesEdit<T> p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        public void Clear()
        {
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
            BackupObject = new ObservableCollection<T>();
        }

        #region properties
        private ObservableCollection<T> _CurObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> CurObject
        {
            get { return _CurObject; }
            set
            {
                if (_CurObject != value)
                {
                    _CurObject = value;
                    _TempObject = DeepCopier.EntityDeepCopy(_CurObject);
                }
                RaisePropertyChanged("CurObject");
                
            }
        }

        private ObservableCollection<T> _NewObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> NewObject
        {
            get { return _NewObject; }
            set
            {
                if (_NewObject != value)
                    _NewObject = value;
                RaisePropertyChanged("NewObject");
            }
        }

        private ObservableCollection<T> _DeleteObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> DeleteObject
        {
            get { return _DeleteObject; }
            set
            {
                if (_DeleteObject != value)
                    _DeleteObject = value;
                RaisePropertyChanged("DeleteObject");
            }
        }

        private ObservableCollection<T> _UpdateObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> UpdateObject
        {
            get { return _UpdateObject; }
            set
            {
                if (_UpdateObject != value)
                    _UpdateObject = value;
                RaisePropertyChanged("UpdateObject");
            }
        }

        private ObservableCollection<T> _TempObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> TempObject
        {
            get { return _TempObject; }
            set
            {
                if (_TempObject != value)
                    _TempObject = value;
                RaisePropertyChanged("TempObject");
            }
        }

        private ObservableCollection<T> _BackupObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> BackupObject
        {
            get { return _BackupObject; }
            set
            {
                if (_BackupObject != value)
                    _BackupObject = value;
                RaisePropertyChanged("BackupObject");
            }
        }
        #endregion

        public bool Remove(T obj)
        {
            if (CheckExist(obj, TempObject))
            {

                NewObject.Remove(obj);
                TempObject.Remove(obj);
                if (CheckExist(obj, CurObject))
                {
                    DeleteObject.Add(obj);
                    CurObject.Remove(obj);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(T obj)
        {
            UpdateObject.Add(obj);
        }

        public bool Add(T obj)
        {
            if(!CheckExist(obj,TempObject))
            {
                NewObject.Add(obj);
                TempObject.Add(obj);
                CurObject.Add(obj);
                return true;
            }else
            {
                return false;    
            }
            
        }
        public bool isExist(T compareObj)
        {
            return CheckExist(compareObj,TempObject);
        }
        public bool CheckExist(T compareObj,ObservableCollection<T> lstObj)
        {
            foreach (var item in lstObj)
            {
                if(CheckEquals(item,compareObj))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckEquals(T sourceObject,T comparisonObject)
        {
            if (_IsCheckByToString)
            {
                return sourceObject.ToString().Equals(comparisonObject.ToString(), StringComparison.Ordinal);
            }
            Type sourceType = sourceObject.GetType();
            Type destinationType = comparisonObject.GetType();
            if(sourceType==destinationType)
            {
                PropertyInfo[] sourceProperties = sourceType.GetProperties();
                foreach (var item in sourceProperties)
                {
                    if (item.ToString() == "Int32 MyID"
                        ||item.ToString()=="Boolean HasErrors")
                    {
                        continue;
                    }
                    if (sourceType.GetProperty(item.Name).GetValue(sourceObject, null) != null
                        && destinationType.GetProperty(item.Name).GetValue(comparisonObject, null) != null)
                    {
                        //2022-01-13 BLQ: 893 Bỏ so sánh này đi vì thấy không cần thiết, điều kiện này thì chỉ sử dụng được cho chống chỉ định
                        if (//(sourceObject.ToString() == "DataEntities.RefMedContraIndicationTypes" && sourceType.GetProperty(item.Name).ToString() != "Boolean IsWarning") &&
                            sourceType.GetProperty(item.Name).GetValue(sourceObject, null).ToString() != destinationType.GetProperty(item.Name).GetValue(comparisonObject, null).ToString())
                        {
                            return false;
                        }
                    }
                } 
            }
            return true;
        }

        public void Reset()
        {
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
            CurObject = DeepCopier.EntityDeepCopy(BackupObject);
        }
    }

    public partial class ObjectEdit<T> : NotifyChangedBase, IEditableObject
    {

        public ObjectEdit( string Property1,string Property2,string Property3)
        {
            _Property1 = Property1;
            _Property2 = Property2;
            _Property3 = Property3;
            CurObject = new ObservableCollection<T>();
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
        }
        public ObjectEdit(ObservableCollection<T> _CurObject, string Property1, string Property2, string Property3)
        {
            _Property1 = Property1;
            _Property2 = Property2;
            _Property3 = Property3;
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
            CurObject = _CurObject;
            BackupObject = DeepCopier.EntityDeepCopy(CurObject);
        }
        private EntitiesEdit<T> _tempEntitiesEdit;
        private string _Property1;
        private string _Property2;
        private string _Property3;
        public void BeginEdit()
        {
            _tempEntitiesEdit = (EntitiesEdit<T>)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempEntitiesEdit)
                CopyFrom(_tempEntitiesEdit);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(EntitiesEdit<T> p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        public void Clear()
        {
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
        }

        #region properties
        private ObservableCollection<T> _CurObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> CurObject
        {
            get { return _CurObject; }
            set
            {
                if (_CurObject != value)
                {
                    _CurObject = value;
                    _TempObject = DeepCopier.EntityDeepCopy(_CurObject);
                }
                RaisePropertyChanged("CurObject");

            }
        }

        private ObservableCollection<T> _NewObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> NewObject
        {
            get { return _NewObject; }
            set
            {
                if (_NewObject != value)
                    _NewObject = value;
                RaisePropertyChanged("NewObject");
            }
        }

        private ObservableCollection<T> _DeleteObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> DeleteObject
        {
            get { return _DeleteObject; }
            set
            {
                if (_DeleteObject != value)
                    _DeleteObject = value;
                RaisePropertyChanged("DeleteObject");
            }
        }

        private ObservableCollection<T> _UpdateObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> UpdateObject
        {
            get { return _UpdateObject; }
            set
            {
                if (_UpdateObject != value)
                    _UpdateObject = value;
                RaisePropertyChanged("UpdateObject");
            }
        }

        private ObservableCollection<T> _TempObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> TempObject
        {
            get { return _TempObject; }
            set
            {
                if (_TempObject != value)
                    _TempObject = value;
                RaisePropertyChanged("TempObject");
            }
        }

        private ObservableCollection<T> _BackupObject;
        [DataMemberAttribute()]
        public ObservableCollection<T> BackupObject
        {
            get { return _BackupObject; }
            set
            {
                if (_BackupObject != value)
                    _BackupObject = value;
                RaisePropertyChanged("BackupObject");
            }
        }
        #endregion
        public void Reset()
        {
            NewObject = new ObservableCollection<T>();
            TempObject = new ObservableCollection<T>();
            DeleteObject = new ObservableCollection<T>();
            UpdateObject = new ObservableCollection<T>();
            CurObject = DeepCopier.EntityDeepCopy(BackupObject);
        }

        public bool Remove(T obj)
        {
            if (CheckExist(obj, TempObject))
            {
                TempObject.Remove(obj);
                if (CheckExist(obj, CurObject))
                {
                    DeleteObject.Add(obj);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(T obj)
        {
            UpdateObject.Add(obj);
        }

        public bool Add(T obj)
        {
            if (!CheckExist(obj, TempObject))
            {
                NewObject.Add(obj);
                TempObject.Add(obj);
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool isExist(T compareObj)
        {
            return CheckExist(compareObj, TempObject);
        }
        public bool CheckExist(T compareObj, ObservableCollection<T> lstObj)
        {
            foreach (var item in lstObj)
            {
                if (CheckEquals(item, compareObj))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckEquals(T sourceObject, T comparisonObject)
        {
            Type sourceType = sourceObject.GetType();
            Type destinationType = comparisonObject.GetType();
            if (sourceType == destinationType)
            {
                PropertyInfo[] sourceProperties = sourceType.GetProperties();
                foreach (var item in sourceProperties)
                {
                    if ((_Property1 != "" && (item.ToString() + " ").Contains(" " + _Property1 + " "))
                        || (_Property2 != "" && (item.ToString() + " ").Contains(" " + _Property2 + " "))
                        || (_Property3 != "" && (item.ToString() + " ").Contains(" " + _Property3 + " ")))
                    {
                        if (sourceType.GetProperty(item.Name).GetValue(sourceObject, null) != null
                            && destinationType.GetProperty(item.Name).GetValue(comparisonObject, null) != null)
                        {
                            if (sourceType.GetProperty(item.Name).GetValue(sourceObject, null).ToString() !=
                                 destinationType.GetProperty(item.Name).GetValue(comparisonObject, null).ToString())
                            {
                                return false;
                            }
                        }    
                    }
                }
            }
            return true;
        }
    }

    public static class DeepCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T EntityDeepCopy<T>(this T ObjectToCopy)
        {
            T Copy;
            DataContractSerializer dCS = new DataContractSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                dCS.WriteObject(ms, ObjectToCopy);
                ms.Position = 0;
                Copy = (T)dCS.ReadObject(ms);
            }
            return Copy;
        }
    }
}

