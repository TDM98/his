using System;
using System.Collections.Generic;
using System.Data.Common;
using eHCMS.Configurations;
using DataEntities;
using System.Data;
using System.Data.SqlClient;
using eHCMS.DAL;
using AxLogging;
using eHCMSLanguage;

/*
* 20180413 #001 TBLD: Them reader cho HIBedCode
* 20210812 #002 TNHX: 436 Thêm DoctorStaffID cho đặt giường
*/
namespace aEMR.DataAccessLayer.Providers
{
    public class BedAllocations: DataProviderBase
    {
        static private BedAllocations _instance = null;
        static public BedAllocations Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BedAllocations();
                }
                return _instance;
                
            }
        }

        public BedAllocations()
        {
            this.ConnectionString = Globals.Settings.ConfigurationManager.ConnectionString;
        }
        
        protected virtual List<DeptLocation> GetDepartmentCollectionFromReader(IDataReader reader)
        {
            List<DeptLocation> lst = new List<DeptLocation>();
            while (reader.Read())
            {
                lst.Add(GetDepartmentObjFromReader(reader));
            }
            reader.Close();
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
            reader.Close();
            return lst;
        }
        #endregion

        #region BedAllocation
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
            reader.Close();
            return lst;
        }

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
            reader.Close();
            return lst;
        }
        
        #endregion
        #region patient bed allocation

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
            reader.Close();
            return lst;
        }
        #endregion



        public  List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID)
        {
            List<DeptLocation> listRG = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_ByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetDepartmentCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }

        public  bool AddNewRoomPrices(long DeptLocationID
                                                , long StaffID
                                                , DateTime? EffectiveDate
                                                , Decimal NormalPrice
                                                , Decimal PriceForHIPatient
                                                , Decimal HIAllowedPrice
                                                , string Note
                                                , bool IsActive)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRoomPrice_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                    cmd.AddParameter("@EffectiveDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(EffectiveDate));
                    cmd.AddParameter("@NormalPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(NormalPrice));

                    cmd.AddParameter("@PriceForHIPatient", SqlDbType.Decimal, ConvertNullObjectToDBNull(PriceForHIPatient));
                    cmd.AddParameter("@HIAllowedPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(HIAllowedPrice));

                    cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Note));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        public  List<RoomPrices> GetAllRoomPricesByDeptID(long DeptLocationID)
        {
            List<RoomPrices> listRG = new List<RoomPrices>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRoomPrice_GetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetRoomPricesCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public  bool DeleteBedAllocation(long BedAllocationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedAllocation_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedAllocationID));


                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public  bool AddNewBedAllocation(int times, long DeptLocationID
                                                   , string BedNumber
                                                   , long MedServiceID
                                                    , string BAGuid
                                                   , long V_BedLocType
                                                   , bool IsActive)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    cn.Open();
                    // =====▼ #001                                    
                    // transaction = cn.BeginTransaction();
                    // =====▲ #001
                    for (int i = 0; i < times; i++)
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Insert", cn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(BedNumber));
                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_BedLocType));
                        cmd.AddParameter("@BAGuid", SqlDbType.VarChar, ConvertNullObjectToDBNull(BAGuid));
                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                        // =====▼ #001                
                        // cmd.Transaction = transaction;
                        // =====▲ #001
                        lstCmd.Add(cmd);
                    }
                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        // =====▼ #001                                        
                        // transaction.Commit();
                        // =====▲ #001
                    }
                    catch (Exception exObj)
                    {
                        AxLogger.Instance.LogError(exObj);
                        // =====▼ #001                
                        //transaction.Rollback();        
                        // =====▲ #001
                    }
                }
            }
            catch { return false; }
            return true;
        }
        public  bool UpdateBedAllocationList(IList<BedAllocation> LstBedAllocation)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    //SqlTransaction transaction;
                    cn.Open();

                    // =====▼ #001                                                        
                    // transaction = cn.BeginTransaction();
                    // =====▲ #001

                    foreach (BedAllocation ba in LstBedAllocation)
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Update", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.BedAllocationID));
                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ba.BedNumber));

                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.V_BedLocType));

                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(ba.IsActive));
                        cmd.AddParameter("@HIBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.HIBedCode));
                        // =====▼ #001                                                        
                        // cmd.Transaction = transaction;
                        // =====▲ #001

                        lstCmd.Add(cmd);
                    }
                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        // =====▼ #001                                                        
                        // transaction.Commit();                                                                              
                        // =====▲ #001
                    }
                    catch (Exception exObj)
                    {
                        AxLogger.Instance.LogError(exObj);
                        // =====▼ #001                                                        
                        // transaction.Rollback();                                                  
                        // =====▲ #001
                    }
                }
            }
            catch { return false; }
            return true;
        }
        public  bool UpdateBedAllocationMedSer(string BAGuid
                                                           , long MedServiceID
                                                           , long V_BedLocType)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedAllocation_UpdateMedSer", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BAGuid", SqlDbType.VarChar, ConvertNullObjectToDBNull(BAGuid));
                    cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                    cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_BedLocType));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public  bool AddNewBedAllocationList(IList<BedAllocation> LstBedAllocation)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    //SqlTransaction transaction;
                    cn.Open();
                    // =====▼ #001                                                        
                    // transaction = cn.BeginTransaction();                                                      
                    // =====▲ #001
                    foreach (BedAllocation ba in LstBedAllocation)
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Insert", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ba.BedNumber));

                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.VRefMedicalServiceItem.MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.VBedLocType.LookupID));

                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(ba.IsActive));
                        cmd.AddParameter("@BAGuid", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.BAGuid));
                        cmd.AddParameter("@HIBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.HIBedCode));

                        // =====▼ #001                                                        
                        // cmd.Transaction = transaction;
                        // =====▲ #001

                        lstCmd.Add(cmd);
                    }
                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        // =====▼ #001                                                        
                        // transaction.Commit();
                        // =====▲ #001

                    }
                    catch (Exception exObj)
                    {
                        AxLogger.Instance.LogError(exObj);
                        // =====▼ #001                                                        
                        // transaction.Rollback();                                                                              
                        // =====▲ #001
                    }
                }
            }
            catch { return false; }
            return true;
        }
        public  string UpdateBedAllocation(IList<BedAllocation> LstBedAllocation)
        {
            string faultUpdate = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();

                    cn.Open();

                    foreach (BedAllocation ba in LstBedAllocation)
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Update", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.BedAllocationID));
                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ba.BedNumber));

                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.V_BedLocType));

                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(ba.IsActive));
                        cmd.AddParameter("@HIBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.HIBedCode));

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            faultUpdate += "\n" + ba.BedNumber;
                        }
                    }
                }
            }
            catch { return "Error"; }
            return faultUpdate;
        }
        public  List<BedAllocation> GetAllBedAllocationByDeptID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<BedAllocation> listRs = new List<BedAllocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedAllocation_GetAllPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedAllocationCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        //Ham tinh count group by MedServiceID
        public  List<BedAllocation> GetCountBedAllocByDeptID(long DeptLocationID, int Choice)
        {
            List<BedAllocation> listRs = new List<BedAllocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedAllocation_GetCount", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@Choice", SqlDbType.BigInt, (Choice));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedAllocationCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public  List<BedAllocation> GetAllBedAllocByDeptID(long DeptLocationID, int IsActive, out int Total)
        {
            List<BedAllocation> listRs = new List<BedAllocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedAllocation_GetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@IsActive", SqlDbType.BigInt, (IsActive));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedAllocationCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public  List<MedServiceItemPrice> GetAllDeptMSItemsByDeptIDSerTypeID(long DeptID, int MedicalServiceTypeID)
        {
            List<MedServiceItemPrice> listRs = new List<MedServiceItemPrice>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDeptMSItems_ByDeptIDSerTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, (DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.Int, (MedicalServiceTypeID));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetMedServiceItemPriceCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

            }
            return listRs;
        }


        #region
        public  bool UpdateBedLocType(long BedLocTypeID
                                                , string BedLocTypeName
                                                , string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedLocType_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedLocTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedLocTypeID));
                    cmd.AddParameter("@BedLocTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(BedLocTypeName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public  bool AddNewBedLocType(string BedLocTypeName, string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedLocTypeInsert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedLocTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(BedLocTypeName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public  bool DeleteBedLocType(long BedLocTypeID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedLocType_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedLocTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedLocTypeID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }

        public  List<BedAllocType> GetAllBedLocType()
        {
            List<BedAllocType> listRs = new List<BedAllocType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedLocType_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    BedAllocType p = new BedAllocType();
                    p.BedLocTypeID = (long)reader["BedLocTypeID"];
                    p.BedLocTypeName = reader["BedLocTypeName"].ToString();
                    p.Description = reader["Description"].ToString();

                    listRs.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        #endregion

        #region BedPatient Alloc

        public  bool UpdateBedPatientAllocs(long BedPatientID
                                                  , long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedPatientAllocs_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedPatientID));
                    cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedAllocationID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AdmissionDate));
                    cmd.AddParameter("@ExpectedStayingDays", SqlDbType.SmallInt, ConvertNullObjectToDBNull(ExpectedStayingDays));
                    cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DischargeDate));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public  int AddNewBedPatientAllocs(BedPatientAllocs alloc)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return AddNewBedPatientAllocs(alloc, cn, null);
            }
        }

        public  int AddNewBedPatientAllocs(BedPatientAllocs alloc, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spBedPatientAllocs_Insert";
            long bedAllocId = 0;

            if (alloc.VBedAllocation != null)
            {
                bedAllocId = alloc.VBedAllocation.BedAllocationID;
            }
            else
            {
                bedAllocId = alloc.BedAllocationID;
            }

            if (bedAllocId == 0)
            {
                throw new Exception(eHCMSResources.Z1682_G1_ChonGiuong);
            }

            long responsibleDeptID;
            if (alloc.ResponsibleDepartment != null)
            {
                responsibleDeptID = alloc.ResponsibleDepartment.DeptID;
            }
            else
            {
                responsibleDeptID = alloc.ResponsibleDeptID;
            }

            if (responsibleDeptID == 0)
            {
                throw new Exception(eHCMSResources.Z1683_G1_ChonKhoaChiuTrachNhiem);
            }

            cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, bedAllocId);
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, alloc.PtRegistrationID);
            cmd.AddParameter("@ResponsibleDeptID", SqlDbType.BigInt, responsibleDeptID);
            cmd.AddParameter("@CheckInDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(alloc.CheckInDate));
            cmd.AddParameter("@ExpectedStayingDays", SqlDbType.SmallInt, alloc.ExpectedStayingDays);
            cmd.AddParameter("@CheckOutDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(alloc.CheckOutDate));
            cmd.AddParameter("@IsActive", SqlDbType.Bit, alloc.IsActive);
            cmd.AddParameter("@PatientInBed", SqlDbType.Bit, alloc.PatientInBed);
            cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(alloc.InPatientDeptDetailID));
            cmd.AddParameter("@BAMedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(alloc.BAMedServiceID));
            //▼====: #002
            cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(alloc.DoctorStaffID));
            //▲====: #002

            var retVal = cmd.ExecuteNonQuery();
            return retVal;
        }

        public bool EditBedPatientAllocs(BedPatientAllocs alloc, long LoggedStaffID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedPatientAllocs_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(alloc.BedPatientID));
                cmd.AddParameter("@BAMedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(alloc.BAMedServiceID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(alloc.DoctorStaffID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(LoggedStaffID));

                cn.Open();
                var retVal = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public  bool AddNewBedPatientAllocs(long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedPatientAllocs_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedAllocationID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AdmissionDate));
                cmd.AddParameter("@ExpectedStayingDays", SqlDbType.SmallInt, ConvertNullObjectToDBNull(ExpectedStayingDays));
                cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DischargeDate));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                var retVal = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public  bool DeleteBedPatientAllocs(long BedPatientID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedPatientAllocs_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedPatientID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        //IsActive 

        public  List<BedPatientAllocs> GetAllBedPatientAllocByDeptID(long DeptLocationID, bool IsReadOnly, out int Total)
        {
            List<BedPatientAllocs> listRs = new List<BedPatientAllocs>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedPatientAlloc_GetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@IsReadOnly", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsReadOnly));
                cmd.AddParameter("@IsActive", SqlDbType.BigInt, 1);


                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedPatientAllocsCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        #endregion

        public  BedPatientAllocs GetActiveBedAllocationByRegistrationID(long registrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetActiveBedAllocationByRegistrationID(registrationID, cn, null);
            }
        }
        public  BedPatientAllocs GetActiveBedAllocationByRegistrationID(long registrationID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_GetActiveBedAllocationByRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registrationID));

            IDataReader reader = ExecuteReader(cmd);
            BedPatientAllocs retVal = null;
            if (reader != null)
            {
                if (reader.Read())
                {
                    retVal = GetBedPatientAllocsObjFromReader(reader);
                }
                reader.Close();
            }
            return retVal;
        }

        public  List<BedPatientAllocs> GetAllBedAllocationsByRegistrationID(long registrationID, long? DeptID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_GetAllBedAllocationByRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registrationID));
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));

            IDataReader reader = ExecuteReader(cmd);
            List<BedPatientAllocs> retVal = null;
            if (reader != null)
            {
                retVal = GetBedPatientAllocCollectionFromReader(reader);
                reader.Close();
            }
            return retVal;
        }

        public List<BedPatientAllocs> GetBedAllocationsByRegistrationIDForLoadBill(long registrationID, long? DeptID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_GetBedAllocationByRegistrationIDForLoadBill";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registrationID));
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));

            IDataReader reader = ExecuteReader(cmd);
            List<BedPatientAllocs> retVal = null;
            if (reader != null)
            {
                retVal = GetBedPatientAllocCollectionFromReader(reader);
                reader.Close();
            }
            return retVal;
        }
        public BedPatientAllocs GetPreBedAllocationsByRegistrationIDForLoadBill(long registrationID, long BedPatientID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_GetPreBedAllocationsByRegistrationIDForLoadBill";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registrationID));
            cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedPatientID));

            IDataReader reader = ExecuteReader(cmd);
            BedPatientAllocs retVal = null;
            if (reader != null)
            {
                if (reader.Read())
                {
                    retVal = GetBedPatientAllocFromReader(reader);
                }
                reader.Close();
            }
            return retVal;
        }

        public  bool MarkDeleteBedPatientAlloc(long bedPatientID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_BedPatientAllocs_MarkDelete";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, bedPatientID);

            return cmd.ExecuteNonQuery() > 0;
        }
        public  bool MarkDeleteBedPatientAlloc(long bedPatientID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return MarkDeleteBedPatientAlloc(bedPatientID, cn, null);
            }
        }

    }
}
