using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Models
{
    public class Order : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string ClientID { get; set; }
        public DateTime DateTime { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string DeliveryAddressID { get; set; }
        public int Number { get; set; }
        public PaymentOption PaymentOption { get; set; }
        #endregion

        public List<OrderProductLink> OrderProductLinks
        {
            get
            {
                return OrderProductLink.Methods.GetProductsLinkedToOrder(Key);
            }
        }

        public Address DeliveryAddress
        {
            get
            {
                return new Address(DeliveryAddressID);
            }
        }


        public Order(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    ClientID = "";
                    DateTime = DateTime.Now;
                    OrderStatus = OrderStatus.NotSet;
                    Number = 0;
                    DeliveryAddressID = "";
                    PaymentOption = PaymentOption.NotSet;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Order > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from [ORDER] ");
                Sql.Append(" where Order_ID = '" + Key.SanitizeInput() + "'");
                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Order > Delete " + Ex.Message);
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

                    Sql.Append(" insert into [ORDER] (");
                    Sql.Append(" Order_ID,");
                    Sql.Append(" Client_ID,");
                    Sql.Append(" Date,");
                    Sql.Append(" Status,");
                    Sql.Append(" DeliveryAddress_ID,");
                    Sql.Append(" PaymentOption");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + ClientID.SanitizeInput() + "',");
                    Sql.Append(" '" + DateTime.ToDBDate() + "',");
                    Sql.Append(" " + (int)OrderStatus + ",");
                    Sql.Append(" '" + DeliveryAddressID.SanitizeInput() + "',");
                    Sql.Append(" " + (int)PaymentOption + "");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update [ORDER] set");
                    Sql.Append(" Status = " + (int)OrderStatus + "");
                    Sql.Append(" where Order_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Order > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(General.GetReadSql());
                Sql.Append(" where Order_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Order > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["Order_ID"].ToString();
                ClientID = Data["Client_ID"].ToString();
                DateTime = Convert.ToDateTime(Data["Date"].ToString());
                OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), Data["Status"].ToString());
                DeliveryAddressID = Data["DeliveryAddress_ID"].ToString();
                Number = Convert.ToInt32(Data["Number"].ToString());
                PaymentOption = (PaymentOption)Enum.Parse(typeof(PaymentOption), Data["PaymentOption"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Order > DataRowToClass " + Ex.Message);
            }
        }

        public class General
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" Order_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" Date,");
                Sql.Append(" Status,");
                Sql.Append(" DeliveryAddress_ID,");
                Sql.Append(" Number,");
                Sql.Append(" PaymentOption");
                Sql.Append(" from [ORDER]");
                return Sql.ToString();
            }

            public static int GetTotalOrders(OrderStatus? OrderStatus = null)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select Count(*) from [ORDER]");
                    if (OrderStatus != null)
                    {
                        Sql.Append(" where Status = " + (int)OrderStatus + "");
                    }
                    return Convert.ToInt32(ExecuteScalar(Sql.ToString()));
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Order > General > GetTotalOrders " + Ex.Message);
                }
            }
            public static List<Order> GetClientOrders(string ClientID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Client_ID = '" + ClientID.SanitizeInput() + "'");

                    DataTable OrderDT = ReadDataTable(Sql.ToString());

                    List<Order> Orders = new List<Order>();

                    foreach (DataRow OrderDR in OrderDT.Rows)
                    {
                        Order Order = new Order()
                        {
                            Key = OrderDR["Order_ID"].ToString(),
                            ClientID = OrderDR["Client_ID"].ToString(),
                            DateTime = Convert.ToDateTime(OrderDR["Date"].ToString()),
                            OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), OrderDR["Status"].ToString()),
                            DeliveryAddressID = OrderDR["DeliveryAddress_ID"].ToString(),
                            Number = Convert.ToInt32(OrderDR["Number"].ToString()),
                            PaymentOption = (PaymentOption)Enum.Parse(typeof(PaymentOption), OrderDR["PaymentOption"].ToString()),
                        };
                        Orders.Add(Order);
                    }

                    return Orders;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Order > GetClientOrders " + Ex.Message);
                }
            }
            public static List<Order> GetOrders(int Fetch, int Skip, OrderStatus OrderStatus, string OrderNumber = "")
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Status = " + (int)OrderStatus + "");
                    if (!string.IsNullOrEmpty(OrderNumber))
                    {
                        Sql.Append(" and Number like '%" + OrderNumber + "%'");
                    }
                    Sql.Append(" order by Date DESC");
                    Sql.Append(" OFFSET " + Skip + " ROWS");
                    Sql.Append(" FETCH NEXT " + Fetch + " ROWS ONLY");

                    DataTable OrderDT = ReadDataTable(Sql.ToString());

                    List<Order> Orders = new List<Order>();

                    foreach (DataRow OrderDR in OrderDT.Rows)
                    {
                        Order Order = new Order();
                        Order.DataRowToClass(OrderDR);
                        Orders.Add(Order);
                    }

                    return Orders;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Order > GetOrders " + Ex.Message);
                }
            }

            public static List<SelectListItem> GetOrderStatusOptions(int SelectedStatusID)
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" OrderStatus_ID,");
                Sql.Append(" Description");
                Sql.Append(" from ReferenceOrderStatus");

                DataTable StatusOptions = ReadDataTable(Sql.ToString());
                List<SelectListItem> Options = new List<SelectListItem>();
                foreach (DataRow Option in StatusOptions.Rows)
                {
                    Options.Add(new SelectListItem() { Value = Option["OrderStatus_ID"].ToString(), Text = Option["Description"].ToString(), Selected = SelectedStatusID.ToString() == Option["OrderStatus_ID"].ToString() ? true : false });
                }

                return Options;
            }


        }
    }
}
