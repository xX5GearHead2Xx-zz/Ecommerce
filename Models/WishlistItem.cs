using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static DataHandler.SqlHandler;

namespace Ecommerce.Models
{
    public class WishlistItem
    {
        #region "Properties"
        public string Key { get; set; }
        public string WishlistID { get; set; }
        public string ProductID { get; set; }
        #endregion

        public Product Product
        {
            get
            {
                return new Product(ProductID);
            }
        }

        public WishlistItem(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    WishlistID = "";
                    ProductID = "";
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > WishlistItem > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from WishlistItem ");
                Sql.Append(" where WishlistItem_ID = '" + Key.SanitizeInput() + "'");
                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > WishlistItem > Delete " + Ex.Message);
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

                    Sql.Append(" insert into WishlistItem (");
                    Sql.Append(" WishlistItem_ID,");
                    Sql.Append(" Wishlist_ID,");
                    Sql.Append(" Product_ID");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + WishlistID.SanitizeInput() + "',");
                    Sql.Append(" '" + ProductID.SanitizeInput() + "'");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    throw new NotImplementedException("Wishlist items can only be created and deleted (not updated)");
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > WishlistItem > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where WishlistItem_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > WishlistItem > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["WishlistItem_ID"].ToString();
                WishlistID = Data["Wishlist_ID"].ToString();
                ProductID = Data["Product_ID"].ToString();
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > WishlistItem > DataRowToClass " + Ex.Message);
            }
        }

        public bool Exists()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select count(*) from WishlistItem");
                Sql.Append(" where Product_ID = '" + ProductID + "'");
                Sql.Append(" and Wishlist_ID = '" + WishlistID + "'");

                return !(Convert.ToInt32(ExecuteScalar(Sql.ToString())) == 0);
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > WishlistItem > Exists " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" WishlistItem_ID,");
                Sql.Append(" Wishlist_ID,");
                Sql.Append(" Product_ID");
                Sql.Append(" from WishlistItem");
                return Sql.ToString();
            }
            public static List<WishlistItem> GetWishlistItems(string WishlistID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Wishlist_ID = '" + WishlistID.SanitizeInput() + "'");

                    DataTable WishlistDT = ReadDataTable(Sql.ToString());

                    List<WishlistItem> WishlistItems = new List<WishlistItem>();

                    foreach (DataRow WishlistDR in WishlistDT.Rows)
                    {
                        WishlistItem WishlistItem = new WishlistItem();
                        WishlistItem.DataRowToClass(WishlistDR);
                        WishlistItems.Add(WishlistItem);
                    }

                    return WishlistItems;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > WishlistItem > GetWishlistItems " + Ex.Message);
                }
            }
        }
    }
}
