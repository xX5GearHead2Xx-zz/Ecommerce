using System;
using System.Text;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class Client : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        #endregion

        public List<Order> Orders
        {
            get
            {
                return Order.General.GetClientOrders(Key);
            }
        }

        public List<Address> Addresses
        {
            get
            {
                return Address.Methods.GetClientAddresses(Key);
            }
        }

        public List<Enums.ClientRole> Roles
        {
            get
            {
                return ClientRole.Methods.GetClientRoles(Key);
            }
        }

        public List<Wishlist> Wishlists
        {
            get
            {
                return Wishlist.Methods.GetClientWishlists(Key);
            }
        }

        public Client(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    Email = "";
                    Password = "";
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Client > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from Client");
                Sql.Append(" where Client_ID = '" + Key.SanitizeInput() + "'");
                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Client > Delete " + Ex.Message);
            }
        }

        public bool Save(bool UpdatePassword = false)
        {
            try
            {
                if (string.IsNullOrEmpty(Key))
                {
                    StringBuilder Sql = new StringBuilder();
                    Key = Guid.NewGuid().ToString();

                    Sql.Append(" insert into Client (");
                    Sql.Append(" Client_ID,");
                    Sql.Append(" Email,");
                    Sql.Append(" Password");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + Email.SanitizeInput() + "',");
                    Sql.Append(" '" + Security.SHA512(Password) + "'");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update Client set");
                    if (UpdatePassword)
                    {
                        Sql.Append(" Password = '" + Security.SHA512(Password) + "',");
                    }
                    Sql.Append(" Email = '" + Email.SanitizeInput() + "'");
                    Sql.Append(" where Client_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }


            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Client > Save " + Ex.Message);
            }
        }

        public DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where Client_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Client > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["Client_ID"].ToString();
                Email = Data["Email"].ToString();
                Password = Data["Password"].ToString();
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Client > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" Client_ID,");
                Sql.Append(" Email,");
                Sql.Append(" Password");
                Sql.Append(" from Client");
                return Sql.ToString();
            }

            public static bool EmailExists(string Email)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select count(*) from Client");
                    Sql.Append(" where Email = '" + Email.SanitizeInput() + "'");

                    return Convert.ToInt32(ExecuteScalar(Sql.ToString())) > 0;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Client > General > EmailExists " + Ex.Message);
                }
            }

            public static string GetClientIDFromEmail(string Email)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select Client_ID from Client");
                    Sql.Append(" where Email = '" + Email.SanitizeInput() + "'");

                    return ExecuteScalar(Sql.ToString());
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Client > General > EmailExists " + Ex.Message);
                }
            }

            public static bool PasswordValid(string Password)
            {
                try
                {
                    Regex regex = new Regex(@"^(?=.*[a-zA-Z0-9]).{6,}$");
                    return regex.IsMatch(Password);
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Client > General > PasswordValidation " + Ex.Message);
                }
            }

            public static bool EmailValid(string Email)
            {
                try
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    return regex.IsMatch(Email);
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Client > General > EmailValid " + Ex.Message);
                }
            }

            public static Client AuthenticateClient(string Email, string Password)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select Client_ID from Client");
                    Sql.Append(" where Email = '" + Email.SanitizeInput() + "'");
                    Sql.Append(" and Password = '" + Security.SHA512(Password) + "'");
                    string ClientID = ExecuteScalar(Sql.ToString());

                    if (!string.IsNullOrEmpty(ClientID))
                    {
                        return new Client(ClientID);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Client > General > AuthenticateClient " + Ex.Message);
                }
            }

            public static int GetTotalClients()
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select Count(*) from Client");
                    return Convert.ToInt32(ExecuteScalar(Sql.ToString()));
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Client > General > GetTotalClients " + Ex.Message);
                }
            }

            public static List<Client> GetClients(int Fetch, int Skip, string Email = null)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();

                    Sql.Append(GetReadSql());
                    if (!string.IsNullOrEmpty(Email))
                    {
                        Sql.Append(" where Email like '%" + Email.SanitizeInput() + "%'");
                    }
                    Sql.Append(" order by Email");
                    Sql.Append(" OFFSET " + Skip + " ROWS");
                    Sql.Append(" FETCH NEXT " + Fetch + " ROWS ONLY");


                    DataTable ClientDT = ReadDataTable(Sql.ToString());
                    List<Client> Clients = new List<Client>();
                    foreach (DataRow ClientDR in ClientDT.Rows)
                    {
                        Client client = new Client();
                        client.DataRowToClass(ClientDR);
                        Clients.Add(client);
                    }

                    return Clients;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Client > General > SearchClient " + Ex.Message);
                }
            }
        }
    }
}
