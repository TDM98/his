using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using eHCMS.DAL;
using eHCMS.Configurations;

namespace aEMR.DataAccessLayer.Providers
{
    public class StaffProvider : DataProviderBase
    {
        static private StaffProvider _instance = null;
        static public StaffProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StaffProvider();
                }
                return _instance;
            }
        }

        public StaffProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }

        public  Staff GetStaffByID(Int64 ID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaff_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ID);
                cn.Open();
                Staff staff = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    staff = GetStaffFromReader(reader);
                }

                if (reader != null)
                {
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return staff;
            }
        }
        public  List<Staff> GetAllStaffContain()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaff_AllRoleANDCatg", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<Staff> staffs = null;
                IDataReader reader = ExecuteReader(cmd);
                staffs = GetStaffCollectionFromReader(reader);

                if (reader != null)
                {
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return staffs;
            }
        }

        public  List<StaffPosition> GetAllStaffPosition()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffPosition_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<StaffPosition> staffs = null;
                IDataReader reader = ExecuteReader(cmd);
                staffs = GetStaffPositionCollectionFromReader(reader);

                if (reader != null)
                {
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return staffs;
            }
        }

    }

}
