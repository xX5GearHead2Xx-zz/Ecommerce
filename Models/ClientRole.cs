using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class ClientRole : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string ClientID { get; set; }
        public Enums.ClientRole RoleID { get; set; }
        #endregion
        public ClientRole(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    ClientID = "";
                    RoleID = Enums.ClientRole.NotSet;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ClientRole > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" update ClientRole set");
                Sql.Append(" where ClientRole_ID = '" + Key.SanitizeInput() + "'");
                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ClientRole > Delete " + Ex.Message);
            }
        }

        public bool Save()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Key = Guid.NewGuid().ToString();

                Sql.Append(" insert into ClientRole (");
                Sql.Append(" ClientRole_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" Role_ID");
                Sql.Append(" ) values (");
                Sql.Append(" '" + Key.SanitizeInput() + "',");
                Sql.Append(" '" + ClientID.SanitizeInput() + "',");
                Sql.Append(" '" + (int)RoleID + "'");
                Sql.Append(" )");

                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ClientRole > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where ClientRole_ID = '" + Key + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ClientRole > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["ClientRole_ID"].ToString();
                ClientID = Data["Client_ID"].ToString();
                RoleID = (Enums.ClientRole)Enum.Parse(typeof(Enums.ClientRole), Data["Role_ID"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ClientRole > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" ClientRole_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" Role_ID,");
                Sql.Append(" from ClientRole");
                return Sql.ToString();
            }
            public static List<Enums.ClientRole> GetClientRoles(string ClientID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select");
                    Sql.Append(" Role_ID");
                    Sql.Append(" from ClientRole");
                    Sql.Append(" where Client_ID = '" + ClientID.SanitizeInput() + "'");

                    DataTable ClientRolesDT = ReadDataTable(Sql.ToString());

                    List<Enums.ClientRole> ClientRoles = new List<Enums.ClientRole>();

                    foreach(DataRow ClientRoleDR in ClientRolesDT.Rows)
                    {
                        ClientRoles.Add((Enums.ClientRole)Enum.Parse(typeof(Enums.ClientRole), ClientRoleDR["Role_ID"].ToString()));
                    }

                    return ClientRoles;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > ClientRole > General > GetClientRole" + Ex.Message);
                }
            }
        }
    }
}
