using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

namespace eHCMS.DAL
{
   public class SqlStaffProvider:StaffProvider
    {
       public SqlStaffProvider()
            : base()
        {

        }
       public override Staff GetStaffByID(Int64 ID)
       {
           using (SqlConnection cn = new SqlConnection(this.ConnectionString))
           {
               SqlCommand cmd = new SqlCommand("spStaff_ByID", cn);
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.AddParameter("@StaffID",SqlDbType.BigInt,ID);
               cn.Open();
               Staff staff = null;
               IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    staff = GetStaffFromReader(reader);
                }
               return staff;
           }
       }
       public override List<Staff> GetAllStaffContain()
       {
           using (SqlConnection cn = new SqlConnection(this.ConnectionString))
           {
               SqlCommand cmd = new SqlCommand("spStaff_AllRoleANDCatg", cn);
               cmd.CommandType = CommandType.StoredProcedure;
               cn.Open();
               List<Staff> staffs = null;
               IDataReader reader = ExecuteReader(cmd);
               staffs = GetStaffCollectionFromReader(reader);
               return staffs;
           }
       }

       public override List<StaffPosition> GetAllStaffPosition()
       {
           using (SqlConnection cn = new SqlConnection(this.ConnectionString))
           {
               SqlCommand cmd = new SqlCommand("spStaffPosition_All", cn);
               cmd.CommandType = CommandType.StoredProcedure;
               cn.Open();
               List<StaffPosition> staffs = null;
               IDataReader reader = ExecuteReader(cmd);
               staffs = GetStaffPositionCollectionFromReader(reader);
               return staffs;
           }
       }
    }
}

