using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
namespace DataEntities
{
    public partial class SupplierGenericDrug : NotifyChangedBase
    {
        #region Factory Method

      
        /// Create a new SupplierGenericDrug object.
    
        /// <param name="supGenDrugID">Initial value of the SupGenDrugID property.</param>
        /// <param name="supplierID">Initial value of the SupplierID property.</param>
        /// <param name="drugID">Initial value of the DrugID property.</param>
        /// <param name="recDateCreated">Initial value of the RecDateCreated property.</param>
        public static SupplierGenericDrug CreateSupplierGenericDrug(Int64 supGenDrugID, Int64 supplierID, Int64 drugID, DateTime recDateCreated)
        {
            SupplierGenericDrug supplierGenericDrug = new SupplierGenericDrug();
            supplierGenericDrug.SupGenDrugID = supGenDrugID;
            supplierGenericDrug.SupplierID = supplierID;
            supplierGenericDrug.DrugID = drugID;
            supplierGenericDrug.RecDateCreated = recDateCreated;
            return supplierGenericDrug;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 SupGenDrugID
        {
            get
            {
                return _SupGenDrugID;
            }
            set
            {
                if (_SupGenDrugID != value)
                {
                    OnSupGenDrugIDChanging(value);
                    _SupGenDrugID = value;
                    RaisePropertyChanged("SupGenDrugID");
                    OnSupGenDrugIDChanged();
                }
            }
        }
        private Int64 _SupGenDrugID;
        partial void OnSupGenDrugIDChanging(Int64 value);
        partial void OnSupGenDrugIDChanged();


        [DataMemberAttribute()]
        public Int64 SupplierID
        {
            get
            {
                return _SupplierID;
            }
            set
            {
                OnSupplierIDChanging(value);
                _SupplierID = value;
                RaisePropertyChanged("SupplierID");
                OnSupplierIDChanged();
            }
        }
        private Int64 _SupplierID;
        partial void OnSupplierIDChanging(Int64 value);
        partial void OnSupplierIDChanged();


        [DataMemberAttribute()]
        public Int64 DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private Int64 _DrugID;
        partial void OnDrugIDChanging(Int64 value);
        partial void OnDrugIDChanged();

        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get
            {
                return _RecDateCreated;
            }
            set
            {
                OnRecDateCreatedChanging(value);
                _RecDateCreated = value;
                RaisePropertyChanged("RecDateCreated");
                OnRecDateCreatedChanged();
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();


        [DataMemberAttribute()]
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


        [DataMemberAttribute()]
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                _UnitPrice = value;
                RaisePropertyChanged("UnitPrice");
            }
        }
        private decimal _UnitPrice;

        [DataMemberAttribute()]
        public decimal PackagePrice
        {
            get
            {
                return _PackagePrice;
            }
            set
            {
                _PackagePrice = value;
                RaisePropertyChanged("PackagePrice");
            }
        }
        private decimal _PackagePrice;

        [DataMemberAttribute()]
        public double VAT
        {
            get
            {
                return _VAT;
            }
            set
            {
                _VAT = value;
                RaisePropertyChanged("VAT");
            }
        }
        private double _VAT = 1;

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Supplier SelectedSupplier
        {
            get
            {
                return _SelectedSupplier;
            }
            set
            {
                if (_SelectedSupplier != value)
                {
                    OnSelectedSupplierChanging(value);
                    _SelectedSupplier = value;
                    if (_SelectedSupplier != null)
                    {
                        _SupplierID = _SelectedSupplier.SupplierID;
                    }
                    else
                    {
                        _SupplierID = 0;
                    }
                    RaisePropertyChanged("SupplierID");
                    RaisePropertyChanged("SelectedSupplier");
                    OnSelectedSupplierChanged();
                }
            }
        }
        private Supplier _SelectedSupplier;
        partial void OnSelectedSupplierChanging(Supplier unit);
        partial void OnSelectedSupplierChanged();


        [DataMemberAttribute()]
        public RefGenericDrugDetail SelectedGenericDrug
        {
            get
            {
                return _SelectedGenericDrug;
            }
            set
            {
                if (_SelectedGenericDrug != value)
                {
                    _SelectedGenericDrug = value;
                    if (_SelectedGenericDrug != null)
                    {
                        _DrugID = _SelectedGenericDrug.DrugID;
                    }
                    else
                    {
                        _DrugID = 0;
                    }
                    RaisePropertyChanged("DrugID");
                    RaisePropertyChanged("SelectedGenericDrug");
                }
            }
        }
        private RefGenericDrugDetail _SelectedGenericDrug;

        #endregion

    }
}
