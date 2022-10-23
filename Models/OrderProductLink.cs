using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class OrderProductLink : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string OrderID { get; set; }
        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public int ItemPrice { get; set; }
        #endregion

        public Product Product
        {
            get
            {
                return new Product(ProductID);
            }
        }

        public OrderProductLink(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    OrderID = "";
                    ProductID = "";
                    Quantity = 0;
                    ItemPrice = 0;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > OrderProductLink > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from OrderProductLink");
                Sql.Append(" where OrderProductLink_ID = '" + Key + "'");

                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > OrderProductLink > Delete " + Ex.Message);
            }
        }

        public bool Save()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Key = Guid.NewGuid().ToString();

                Sql.Append(" insert into OrderProductLink (");
                Sql.Append(" OrderProductLink_ID,");
                Sql.Append(" Order_ID,");
                Sql.Append(" Product_ID,");
                Sql.Append(" Quantity,");
                Sql.Append(" ItemPrice");
                Sql.Append(" ) values (");
                Sql.Append(" '" + Key + "',");
                Sql.Append(" '" + OrderID.SanitizeInput() + "',");
                Sql.Append(" '" + ProductID.SanitizeInput() + "',");
                Sql.Append(" " + Quantity + ",");
                Sql.Append(" " + ItemPrice + "");
                Sql.Append(" )");

                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > OrderProductLink > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where OrderProductLink_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > OrderProductLink > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["OrderProductLink_ID"].ToString();
                OrderID = Data["Order_ID"].ToString();
                ProductID = Data["Product_ID"].ToString();
                Quantity = Convert.ToInt32(Data["Quantity"].ToString());
                ItemPrice = Convert.ToInt32(Data["ItemPrice"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > OrderProductLink > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" OrderProductLink_ID,");
                Sql.Append(" Order_ID,");
                Sql.Append(" Product_ID,");
                Sql.Append(" Quantity,");
                Sql.Append(" ItemPrice");
                Sql.Append(" from OrderProductLink");
                return Sql.ToString();

            }
            public static List<OrderProductLink> GetProductsLinkedToOrder(string OrderID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Order_ID = '" + OrderID.SanitizeInput() + "'");

                    DataTable OrderproductLinkDT = ReadDataTable(Sql.ToString());

                    List<OrderProductLink> OrderproductLinks = new List<OrderProductLink>();

                    foreach (DataRow OrderProductLinkDR in OrderproductLinkDT.Rows)
                    {
                        OrderProductLink OrderproductLink = new OrderProductLink();
                        OrderproductLink.DataRowToClass(OrderProductLinkDR);
                        OrderproductLinks.Add(OrderproductLink);
                    }

                    return OrderproductLinks;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > orderproductLink > GetProductsLinkedToOrder " + Ex.Message);
                }
            }
        }
    }
}
