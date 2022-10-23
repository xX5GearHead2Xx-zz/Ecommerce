using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static DataHandler.SqlHandler;

namespace Ecommerce.Models
{
    public class Wishlist
    {
        #region "Properties"
        public string Key { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
        #endregion

        public List<WishlistItem> Items
        {
            get
            {
                return WishlistItem.Methods.GetWishlistItems(Key);
            }
        }

        public Wishlist(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    ClientID = "";
                    Name = "";
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Wishlist > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from Wishlist ");
                Sql.Append(" where Wishlist_ID = '" + Key.SanitizeInput() + "'");
                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Wishlist > Delete " + Ex.Message);
            }
        }

        public bool Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Key))
                {
                    StringBuilder Sql = new StringBuilder();
                    Key = Guid.NewGuid().ToString();

                    Sql.Append(" insert into Wishlist (");
                    Sql.Append(" Wishlist_ID,");
                    Sql.Append(" Client_ID,");
                    Sql.Append(" Name");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + ClientID.SanitizeInput() + "',");
                    Sql.Append(" '" + Name.SanitizeInput() + "'");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update Wishlist set");
                    Sql.Append(" Name = '" + Name.SanitizeInput() + "'");
                    Sql.Append(" where Wishlist_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Wishlist > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where Wishlist_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Wishlist > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["Wishlist_ID"].ToString();
                ClientID = Data["Client_ID"].ToString();
                Name = Data["Name"].ToString();
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Wishlist > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" Wishlist_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" Name");
                Sql.Append(" from Wishlist");
                return Sql.ToString();
            }
            public static List<Wishlist> GetClientWishlists(string ClientID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Client_ID = '" + ClientID.SanitizeInput() + "'");

                    DataTable WishlistDT = ReadDataTable(Sql.ToString());

                    List<Wishlist> Wishlists = new List<Wishlist>();

                    foreach (DataRow WishlistDR in WishlistDT.Rows)
                    {
                        Wishlist Wishlist = new Wishlist();
                        Wishlist.DataRowToClass(WishlistDR);
                        Wishlists.Add(Wishlist);
                    }

                    return Wishlists;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Wishlist > GetClientWishlists " + Ex.Message);
                }
            }
        }

    }
}
