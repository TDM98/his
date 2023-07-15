using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Reflection;
using eHCMS.Configurations;
using DataEntities;
using System.Data;
/*
 * 20180413 #001 TBLD: Them reader cho HIBedCode
*/
namespace eHCMS.DAL
{
    public abstract class BedAllocations: DataProviderBase
    {
        static private BedAllocations _instance = null;
        static public BedAllocations Instance
        {
            get
            {
                if (_instance == null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.ConfigurationManager.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type[] types = assem.GetExportedTypes();
                    Type t = assem.GetType(Globals.Settings.ConfigurationManager.bedlocation.ProviderType);
                    _instance = (BedAllocations)Activator.CreateInstance(t);
                }
                return _instance;
                
            }
        }

        public BedAllocations()
        {
            this.ConnectionString = Globals.Settings.ConfigurationManager.ConnectionString;
        }
        public abstract List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID);
        protected virtual List<DeptLocation> GetDepartmentCollectionFromReader(IDataReader reader)
        {
            List<DeptLocation> lst = new List<DeptLocation>();
            while (reader.Read())
            {
                lst.Add(GetDepartmentObjFromReader(reader));
            }
            return lst;
        }
        protected virtual DeptLocation GetDepartmentObjFromReader(IDataReader reader)
        {
            DeptLocation p = new DeptLocation();
            try
            {                
                if (reader.HasColumn("LID"))
                {
                    p.LID = (long)reader["LID"];                    
                }
                if (reader.HasColumn("DeptID"))
                {
                    p.DeptID = (long)reader["DeptID"];
                }
                if (reader.HasColumn("DeptLocationID"))
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                }
                
                p.RefDepartment = new RefDepartment();
                try
                {
                    if (reader.HasColumn("DeptName"))
                    {
                        p.RefDepartment.DeptName = reader["DeptName"].ToString();
                    }
                    if (reader.HasColumn("DeptDescription"))
                    {
                        p.RefDepartment.DeptDescription = reader["DeptDescription"].ToString();
                    }
                    if (reader.HasColumn("V_DeptType"))
                    {
                        p.RefDepartment.V_DeptType = (long)reader["V_DeptType"];
                    }
                }
                catch { }

                p.Location = new Location();
                try
                {
                    if (reader.HasColumn("LocationName"))
                    {
                        p.Location.LocationName = reader["LocationName"].ToString();
                    }
                    if (reader.HasColumn("LocationDescription"))
                    {
                        p.Location.LocationDescription = reader["LocationDescription"].ToString();
                    }
                }
                catch { }
            }
            catch 
            { return null; }
            return p;
        }

        #region room price
        public abstract bool AddNewRoomPrices( long DeptLocationID
                                                , long StaffID
                                                , DateTime? EffectiveDate
                                                , Decimal NormalPrice
                                                , Decimal PriceForHIPatient
                                                , Decimal HIAllowedPrice
                                                , string Note
                                                , bool IsActive);
        public abstract List<RoomPrices> GetAllRoomPricesByDeptID(long DeptLocationID);
        protected virtual RoomPrices GetRoomPricesObjFromReader(IDataReader reader)
        {
            RoomPrices p = new RoomPrices();
            try
            {
                if (reader.HasColumn("RoomPriceID"))
                {
                    p.RoomPriceID = (long)reader["RoomPriceID"];
                }

                if (reader.HasColumn("RecDateCreated"))
                {
                    p.RecDateCreated = (DateTime)reader["RecDateCreated"];
                }

                if (reader.HasColumn("DeptLocationID"))
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                }

                if (reader.HasColumn("StaffID"))
                {
                    p.StaffID = (long)reader["StaffID"];
                }

                if (reader.HasColumn("EffectiveDate"))
                {
                    p.EffectiveDate = (DateTime?)reader["EffectiveDate"];
                }

                if (reader.HasColumn("NormalPrice"))
                {
                    p.NormalPrice = (Decimal)reader["NormalPrice"];
                }

                if (reader.HasColumn("PriceForHIPatient"))
                {
                    p.PriceForHIPatient = (Decimal)reader["PriceForHIPatient"];
                }

                if (reader.HasColumn("HIAllowedPrice"))
                {
                    p.HIAllowedPrice = (Decimal)reader["HIAllowedPrice"];
                }

                if (reader.HasColumn("Note"))
                {
                    p.Note = reader["Note"].ToString();
                }

                if (reader.HasColumn("IsActive"))
                {
                    p.IsActive = (bool)reader["IsActive"];
                }

                p.VStaff = new Staff();
                try
                {
                    if (reader.HasColumn("StaffID"))
                    {
                        p.VStaff.StaffID = (long)reader["StaffID"];
                    }
                    if (reader.HasColumn("FullName"))
                    {
                        p.VStaff.FullName = reader["FullName"].ToString();
                    }
                }
                catch { }
            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<RoomPrices> GetRoomPricesCollectionFromReader(IDataReader reader)
        {
            List<RoomPrices> lst = new List<RoomPrices>();
            while (reader.Read())
            {
                lst.Add(GetRoomPricesObjFromReader(reader));
            }
            return lst;
        }
        #endregion

        #region BedAllocation
        public abstract bool AddNewBedAllocation(int times,long DeptLocationID
                                                   , string BedNumber
                                                   , long MedServiceID
                                                    ,string BAGuid
                                                   , long V_BedLocType
                                                   , bool IsActive);
        public abstract List<BedAllocation> GetAllBedAllocationByDeptID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        protected virtual BedAllocation GetBedAllocationObjFromReader(IDataReader reader)
        {
            BedAllocation p = new BedAllocation();
            try//
            {
                p.Status = 0;
                try 
                {
                    if (reader.HasColumn("CountTotal"))
                    {
                        p.TotalRecord = Convert.ToInt32(reader["CountTotal"]);
                    }
                    if (reader.HasColumn("BAGuid"))
                    {
                        p.BAGuid = reader["BAGuid"].ToString();
                    }
                }
                catch 
                { }
                try 
                {
                    if (reader.HasColumn("MedServiceID"))
                    {
                        p.MedServiceID = (long)reader["MedServiceID"];
                    }
                    if (reader.HasColumn("BedAllocationID"))
                    {
                        p.BedAllocationID = (long)reader["BedAllocationID"];
                    }
                    if (reader.HasColumn("DeptLocationID"))
                    {
                        p.DeptLocationID = (long)reader["DeptLocationID"];
                    }
                    if (reader.HasColumn("BedNumber"))
                    {
                        p.BedNumber = reader["BedNumber"].ToString();
                    }
                    if (reader.HasColumn("V_BedLocType"))
                    {
                        p.V_BedLocType = (long)reader["V_BedLocType"];
                    }
                    if (reader.HasColumn("IsActive"))
                    {
                        p.IsActive = (bool)reader["IsActive"];
                    }
                    /*▼====: #001*/
                    if (reader.HasColumn("HIBedCode") && reader["HIBedCode"] != DBNull.Value)
                    {
                        p.HIBedCode = reader["HIBedCode"].ToString();
                    }
                    /*▲====: #001*/
                    p.Status = 0;
                    p.VDeptLocation = new DeptLocation();
                    try
                    {
                        if (reader.HasColumn("DeptID"))
                        {
                            p.VDeptLocation.DeptID = (long)reader["DeptID"];
                        }

                        if (reader.HasColumn("LID"))
                        {
                            p.VDeptLocation.LID = (long)reader["LID"];
                        }
                    }
                    catch { }                
                }
                catch { }
                
                p.VRefMedicalServiceItem = new RefMedicalServiceItem();
                try
                {
                    if (reader.HasColumn("MedServiceID"))
                    {
                        p.VRefMedicalServiceItem.MedServiceID = (long)reader["MedServiceID"];
                    }
                    if (reader.HasColumn("MedicalServiceTypeID"))
                    {
                        p.VRefMedicalServiceItem.MedicalServiceTypeID = (long)reader["MedicalServiceTypeID"];
                    }
                    //p.VRefMedicalServiceItem.PartnerShipID = (long)reader["PartnerShipID"];

                    if (reader.HasColumn("MedServiceCode"))
                    {
                        p.VRefMedicalServiceItem.MedServiceCode = reader["MedServiceCode"].ToString();
                    }
                    if (reader.HasColumn("MedServiceName"))
                    {
                        p.VRefMedicalServiceItem.MedServiceName = reader["MedServiceName"].ToString();
                    }

                    if (reader.HasColumn("RNormalPrice"))
                    {
                        p.VRefMedicalServiceItem.NormalPrice = (Decimal)reader["RNormalPrice"];
                    }

                    if (reader.HasColumn("EffectiveDate"))
                    {
                        p.VRefMedicalServiceItem.ExpiryDate = (DateTime?)reader["EffectiveDate"];
                    }

                    if (reader.HasColumn("V_RefMedServiceItemsUnit"))
                    {
                        p.VRefMedicalServiceItem.V_RefMedServiceItemsUnit = (long)reader["V_RefMedServiceItemsUnit"];
                    }

                    if (reader.HasColumn("VATRate"))
                    {
                        p.VRefMedicalServiceItem.VATRate = (long)reader["VATRate"];
                    }
                    if (reader.HasColumn("PriceForHIPatient"))
                    {
                        p.VRefMedicalServiceItem.PriceForHIPatient = (Decimal)reader["PriceForHIPatient"];
                    }

                    if (reader.HasColumn("PriceDifference"))
                    {
                        p.VRefMedicalServiceItem.PriceDifference = (Decimal)reader["PriceDifference"];
                    }

                    if (reader.HasColumn("ChildrenUnderSixPrice"))
                    {
                        p.VRefMedicalServiceItem.ChildrenUnderSixPrice = (Decimal)reader["ChildrenUnderSixPrice"];
                    }

                    if (reader.HasColumn("HIAllowedPrice"))
                    {
                        p.VRefMedicalServiceItem.HIAllowedPrice = (Decimal)reader["HIAllowedPrice"];
                    }

                    if (reader.HasColumn("IsExpiredDate"))
                    {
                        p.VRefMedicalServiceItem.IsExpiredDate = (bool?)reader["IsExpiredDate"];
                    }

                    if (reader.HasColumn("ByRequest"))
                    {
                        p.VRefMedicalServiceItem.ByRequest = (bool?)reader["ByRequest"];
                    }

                    if (reader.HasColumn("ServiceMainTime"))
                    {
                        p.VRefMedicalServiceItem.ServiceMainTime = (byte?)reader["ServiceMainTime"];
                    }
                }
                catch { }
            }
            catch 
            { return null; }
            return p;
        }
        protected virtual List<BedAllocation> GetBedAllocationCollectionFromReader(IDataReader reader)
        {
            List<BedAllocation> lst = new List<BedAllocation>();
            while (reader.Read())
            {
                lst.Add(GetBedAllocationObjFromReader(reader));
            }
            return lst;
        }
        public abstract bool DeleteBedAllocation(long BedAllocationID);
        public abstract string UpdateBedAllocation(IList<BedAllocation> LstBedAllocation);
        public abstract List<BedAllocation> GetAllBedAllocByDeptID(long DeptLocationID, int IsActive,out int Total);
        public abstract bool AddNewBedAllocationList(IList<BedAllocation> LstBedAllocation);
        public abstract bool UpdateBedAllocationList(IList<BedAllocation> LstBedAllocation);
        public abstract bool UpdateBedAllocationMedSer(string BAGuid
                                                           , long MedServiceID
                                                           , long V_BedLocType);

        protected new DeptMedServiceItems GetDeptMedServiceItemsObjFromReader(IDataReader reader)
        {
            DeptMedServiceItems p = new DeptMedServiceItems();
            try//
            {
                if (reader.HasColumn("DeptMedServItemID"))
                {
                    p.DeptMedServItemID = (long)reader["DeptMedServItemID"];
                }

                if (reader.HasColumn("DeptID"))
                {
                    p.DeptID = (long)reader["DeptID"];
                }

                if (reader.HasColumn("MedServiceID"))
                {
                    p.MedServiceID = (long)reader["MedServiceID"];
                }
                p.ObjDeptID = new RefDepartments();
                try
                {                    

                }
                catch { }

                p.ObjRefMedicalServiceItem = new RefMedicalServiceItem();
                try
                {
                    if (reader.HasColumn("MedServiceID"))
                    {
                        p.ObjRefMedicalServiceItem.MedServiceID = (long)reader["MedServiceID"];
                    }

                    if (reader.HasColumn("MedicalServiceTypeID"))
                    {
                        p.ObjRefMedicalServiceItem.MedicalServiceTypeID = (long?)reader["MedicalServiceTypeID"];
                    }

                    if (reader.HasColumn("MedServiceName"))
                    {
                        p.ObjRefMedicalServiceItem.MedServiceName = reader["MedServiceName"].ToString();
                    }

                    if (reader.HasColumn("RNormalPrice"))
                    {
                        p.ObjRefMedicalServiceItem.NormalPrice = (Decimal)reader["RNormalPrice"];
                    }
                    p.ObjRefMedicalServiceItem.Description = p.ObjRefMedicalServiceItem.MedServiceName
                                +" - Normal price: "+p.ObjRefMedicalServiceItem.NormalPrice.ToString();

                    if (reader.HasColumn("EffectiveDate"))
                    {
                        p.ObjRefMedicalServiceItem.ExpiryDate = (DateTime?)reader["EffectiveDate"];
                    }
                    //p.ObjRefMedicalServiceItem.PartnerShipID = (long?)reader["PartnerShipID"];

                    if (reader.HasColumn("MedServiceCode"))
                    {
                        p.ObjRefMedicalServiceItem.MedServiceCode = reader["MedServiceCode"].ToString();
                    }

                    if (reader.HasColumn("V_RefMedServiceItemsUnit"))
                    {
                        p.ObjRefMedicalServiceItem.V_RefMedServiceItemsUnit =(long)reader["V_RefMedServiceItemsUnit"];
                    }

                    if (reader.HasColumn("VATRate"))
                    {
                        p.ObjRefMedicalServiceItem.VATRate = (Double?)reader["VATRate"];
                    }

                    if (reader.HasColumn("IsExpiredDate"))
                    {
                        p.ObjRefMedicalServiceItem.IsExpiredDate = (bool?)reader["IsExpiredDate"];
                    }

                    if (reader.HasColumn("ByRequest"))
                    {
                        p.ObjRefMedicalServiceItem.ByRequest = (bool?)reader["ByRequest"];
                    }

                    if (reader.HasColumn("ServiceMainTime"))
                    {
                        p.ObjRefMedicalServiceItem.ServiceMainTime = (byte?)reader["ServiceMainTime"];
                    }
                }
                catch { }
            }
            catch 
            { return null; }
            return p;
        }
        protected new List<DeptMedServiceItems> GetDeptMedServiceItemsCollectionFromReader(IDataReader reader)
        {
            List<DeptMedServiceItems> lst = new List<DeptMedServiceItems>();
            while (reader.Read())
            {
                lst.Add(GetDeptMedServiceItemsObjFromReader(reader));
            }
            return lst;
        }
        public abstract List<MedServiceItemPrice> GetAllDeptMSItemsByDeptIDSerTypeID(long DeptID, int MedicalServiceTypeID);
        public abstract List<BedAllocation> GetCountBedAllocByDeptID(long DeptLocationID, int Choice);
        #endregion

        #region
        public abstract bool UpdateBedLocType(long BedLocTypeID
                                                , string BedLocTypeName
                                                , string Description);
        
        public abstract bool AddNewBedLocType(string BedLocTypeName, string Description);
        
        public abstract bool DeleteBedLocType(long BedLocTypeID);

        public abstract List<BedAllocType> GetAllBedLocType();
        
        #endregion
        #region patient bed allocation
        public abstract bool UpdateBedPatientAllocs(long BedPatientID
                                                  , long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive);

        public abstract bool AddNewBedPatientAllocs(long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive);

        public abstract bool DeleteBedPatientAllocs(long BedPatientID);

        public abstract List<BedPatientAllocs> GetAllBedPatientAllocByDeptID(long DeptLocationID, out int Total);
        protected virtual BedPatientAllocs GetBedPatientAllocsObjFromReader(IDataReader reader)
        {
            BedPatientAllocs p = new BedPatientAllocs();

            if (reader["BedPatientID"] != DBNull.Value)
            {
                p.BedPatientID = (long)reader["BedPatientID"];

                if (reader["ResponsibleDeptID"] != DBNull.Value)
                {
                    p.ResponsibleDeptID = (long)reader["ResponsibleDeptID"];
                }

                if (reader.HasColumn("PatientIsActive"))
                {
                    p.IsActive = (bool)reader["PatientIsActive"];
                }

                if (reader.HasColumn("PatientInBed"))
                {
                    p.PatientInBed = (bool) reader["PatientInBed"];
                }

                if (p.IsActive == false)
                {
                    p.PStatus = "Giường Trống.";
                    p.CheckInDate = null;
                    p.CheckOutDate = null;
                    p.ExpectedStayingDays = 1;                    
                }
                else
                {
                    p.PStatus = "Giường Có Bệnh Nhân.";
                    try
                    {
                        if (reader.HasColumn("CheckInDate"))
                        {
                            p.CheckInDate = (DateTime?)reader["CheckInDate"];
                        }
                    }
                    catch { }
                    try
                    {
                        if (reader.HasColumn("ExpectedStayingDays"))
                        {
                            p.ExpectedStayingDays = Convert.ToInt32(reader["ExpectedStayingDays"]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (reader.HasColumn("CheckOutDate"))
                        {
                            p.CheckOutDate = (DateTime?)reader["CheckOutDate"];
                        }
                    }
                    catch { }
                }

                try
                {
                    if (reader.HasColumn("BedAllocationID"))
                    {
                        p.BedAllocationID = (long)reader["BedAllocationID"];
                    }
                }
                catch { }
                try
                {
                    if (reader.HasColumn("PtRegistrationID"))
                    {
                        p.PtRegistrationID = (long)reader["PtRegistrationID"];
                    }
                }
                catch { }

                if(reader.HasColumn("Res_DeptID") && reader["Res_DeptID"] != DBNull.Value)
                {
                    p.ResponsibleDepartment = GetResposibleDepartmentFromReader(reader, "Res_");
                }
                else
                {
                    p.ResponsibleDepartment = null;
                }
            }

                
                
                
                
                p.VBedAllocation = new BedAllocation();
                
                try
                {
                    if (reader.HasColumn("BAGuid"))
                    {
                        p.VBedAllocation.BAGuid = reader["BAGuid"].ToString();
                    }

                    if (reader.HasColumn("MedServiceID"))
                    {
                        p.VBedAllocation.MedServiceID = (long)reader["MedServiceID"];
                    }

                    if (reader.HasColumn("BedAllocationID"))
                    {
                        p.VBedAllocation.BedAllocationID = (long)reader["BedAllocationID"];
                        p.BedAllocationID = (long)reader["BedAllocationID"];
                    }

                    if (reader.HasColumn("DeptLocationID"))
                    {
                        p.VBedAllocation.DeptLocationID = (long)reader["DeptLocationID"];
                    }

                    if (reader.HasColumn("BedNumber"))
                    {
                        p.VBedAllocation.BedNumber = reader["BedNumber"].ToString();
                    }

                    if (reader.HasColumn("V_BedLocType"))
                    {
                        p.VBedAllocation.V_BedLocType = (long)reader["V_BedLocType"];
                    }

                    if (reader.HasColumn("IsActive"))
                    {
                        p.VBedAllocation.IsActive = (bool)reader["IsActive"];
                    }
                    p.VBedAllocation.Status = 0;

                    p.VBedAllocation.VDeptLocation = GetDeptLocationFromReader(reader);
                    //p.VBedAllocation.VDeptLocation = new DeptLocation();
                    //try
                    //{
                    //    if (reader.HasColumn("DeptID"))
                    //    {
                    //        p.VBedAllocation.VDeptLocation.DeptID = (long)reader["DeptID"];
                    //    }
                    //    if (reader.HasColumn("LID"))
                    //    {
                    //        p.VBedAllocation.VDeptLocation.LID = (long)reader["LID"];
                    //    }
                    //}
                    //catch { }
                }
                catch { }

                p.VBedAllocation.VRefMedicalServiceItem = new RefMedicalServiceItem();
                try
                {
                    if (reader.HasColumn("MedServiceID"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.MedServiceID = (long)reader["MedServiceID"];
                    }

                    if (reader.HasColumn("MedicalServiceTypeID"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.MedicalServiceTypeID = (long)reader["MedicalServiceTypeID"];
                    }

                    if (reader.HasColumn("MedServiceCode"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.MedServiceCode = reader["MedServiceCode"].ToString();
                    }

                    if (reader.HasColumn("MedServiceName"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.MedServiceName = reader["MedServiceName"].ToString();
                    }

                    if (reader.HasColumn("RNormalPrice"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.NormalPrice = (Decimal)reader["RNormalPrice"];
                    }

                    if (reader.HasColumn("EffectiveDate"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.ExpiryDate = (DateTime?)reader["EffectiveDate"];
                    }

                    if (reader.HasColumn("V_RefMedServiceItemsUnit"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.V_RefMedServiceItemsUnit = (long)reader["V_RefMedServiceItemsUnit"];
                    }

                    if (reader.HasColumn("VATRate"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.VATRate = (long)reader["VATRate"];
                    }

                    if (reader.HasColumn("PriceForHIPatient"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.PriceForHIPatient = (Decimal)reader["PriceForHIPatient"];
                    }

                    if (reader.HasColumn("PriceDifference"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.PriceDifference = (Decimal)reader["PriceDifference"];
                    }

                    if (reader.HasColumn("ChildrenUnderSixPrice"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.ChildrenUnderSixPrice = (Decimal)reader["ChildrenUnderSixPrice"];
                    }

                    if (reader.HasColumn("HIAllowedPrice"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.HIAllowedPrice = (Decimal)reader["HIAllowedPrice"];
                    }

                    if (reader.HasColumn("IsExpiredDate"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.IsExpiredDate = (bool?)reader["IsExpiredDate"];
                    }

                    if (reader.HasColumn("ByRequest"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.ByRequest = (bool?)reader["ByRequest"];
                    }

                    if (reader.HasColumn("ServiceMainTime"))
                    {
                        p.VBedAllocation.VRefMedicalServiceItem.ServiceMainTime = (byte?)reader["ServiceMainTime"];
                    }
                }
                catch { }
            
            return p;
        }
        protected virtual List<BedPatientAllocs> GetBedPatientAllocsCollectionFromReader(IDataReader reader)
        {
            List<BedPatientAllocs> lst = new List<BedPatientAllocs>();
            while (reader.Read())
            {
                lst.Add(GetBedPatientAllocsObjFromReader(reader));
            }
            return lst;
        }
        #endregion
        public abstract BedPatientAllocs GetActiveBedAllocationByRegistrationID(long registrationID);
        public abstract BedPatientAllocs GetActiveBedAllocationByRegistrationID(long registrationID, DbConnection conn, DbTransaction tran);
        public abstract int AddNewBedPatientAllocs(BedPatientAllocs alloc, DbConnection connection, DbTransaction tran);
        public abstract int AddNewBedPatientAllocs(BedPatientAllocs alloc);
        public abstract List<BedPatientAllocs> GetAllBedAllocationsByRegistrationID(long registrationID,long? DeptID, DbConnection conn, DbTransaction tran);
        public abstract bool MarkDeleteBedPatientAlloc(long bedPatientID, DbConnection conn, DbTransaction tran);
        public abstract bool MarkDeleteBedPatientAlloc(long BedPatientID);
    }
}
