using System;
using System.Text;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class PasswordRecovery : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string ClientID { get; set; }
        public DateTime DateRequested { get; set; }
        public DateTime ExpiryDate { get; set; }
        #endregion

        public PasswordRecovery(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    ClientID = "";
                    DateRequested = DateTime.Now;
                    ExpiryDate = DateTime.Now.AddHours(1);
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from PasswordRecovery");
                Sql.Append(" where PasswordRecovery_ID = '" + Key.SanitizeInput() + "'");
                if (ExecuteNonQuery(Sql.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Delete " + Ex.Message);
            }
        }

        public bool Save()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Key = Guid.NewGuid().ToString();

                Sql.Append(" insert into PasswordRecovery (");
                Sql.Append(" PasswordRecovery_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" DateRequested,");
                Sql.Append(" ExpiryDate");
                Sql.Append(" ) values (");
                Sql.Append(" '" + Key.SanitizeInput() + "',");
                Sql.Append(" '" + ClientID.SanitizeInput() + "',");
                Sql.Append(" '" + DateRequested.ToDBDate() + "',");
                Sql.Append(" '" + ExpiryDate.ToDBDate() + "'");
                Sql.Append(" )");

                if (ExecuteNonQuery(Sql.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where ExpiryDate > '" + DateTime.Now.ToDBDate() + "'");
                Sql.Append(" and PasswordRecovery_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["PasswordRecovery_ID"].ToString();
                ClientID = Data["Client_ID"].ToString();
                DateRequested = Convert.ToDateTime(Data["DateRequested"].ToString());
                ExpiryDate = Convert.ToDateTime(Data["ExpiryDate"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > PasswordRecovery > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" PasswordRecovery_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" DateRequested,");
                Sql.Append(" ExpiryDate");
                Sql.Append(" from PasswordRecovery");
                return Sql.ToString();
            }

            public static List<PasswordRecovery> GetClientRecoveryRequests(string ClientID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Client_ID = '" + ClientID.SanitizeInput() + "'");

                    DataTable RecoveryDT = ReadDataTable(Sql.ToString());

                    List<PasswordRecovery> passwordRecoveries = new List<PasswordRecovery>();

                    foreach (DataRow RecoveryDR in RecoveryDT.Rows)
                    {
                        PasswordRecovery Recovery = new PasswordRecovery();
                        Recovery.DataRowToClass(RecoveryDR);
                        passwordRecoveries.Add(Recovery);
                    }

                    return passwordRecoveries;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > PasswordRecovery > GetClientRecoveryRequests" + Ex.Message);
                }
            }
        }
    }
}
