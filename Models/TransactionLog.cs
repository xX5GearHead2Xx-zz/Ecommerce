using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataHandler.SqlHandler;
using static Ecommerce.Models.Enums;

namespace Ecommerce.Models
{
    public class TransactionLog : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string OrderID { get; set; }
        public int Amount { get; set; }
        public string ClientID { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public string StatusCode { get; set; }
        public string Description { get; set; }
        public string PayRequestID { get; set; }
        public string Checksum { get; set; }

        #endregion
        public TransactionLog(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    OrderID = "";
                    Amount = 0;
                    ClientID = "";
                    RequestDate = DateTime.Now;
                    CompleteDate = DateTime.Now;
                    StatusCode = "";
                    Description = "";
                    PayRequestID = "";
                    Checksum = "";
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > TransactionLog > Constructor " + Ex.Message);
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

                    Sql.Append(" insert into TransactionLog (");
                    Sql.Append(" Transaction_ID,");
                    Sql.Append(" Order_ID,");
                    Sql.Append(" Amount,");
                    Sql.Append(" Client_ID,");
                    Sql.Append(" RequestDate,");
                    Sql.Append(" CompleteDate,");
                    Sql.Append(" StatusCode,");
                    Sql.Append(" Description,");
                    Sql.Append(" PayRequestID,");
                    Sql.Append(" Checksum");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + OrderID.SanitizeInput() + "',");
                    Sql.Append(" '" + Amount + "',");
                    Sql.Append(" '" + ClientID.SanitizeInput() + "',");
                    Sql.Append(" '" + RequestDate.ToDBDate() + "',");
                    Sql.Append(" NULL,");
                    Sql.Append(" '" + StatusCode.SanitizeInput() + "',");
                    Sql.Append(" '" + Description.SanitizeInput() + "',");
                    Sql.Append(" '" + PayRequestID.SanitizeInput() + "',");
                    Sql.Append(" '" + Checksum.SanitizeInput() + "'");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update TransactionLog set");
                    Sql.Append(" CompleteDate = '" + CompleteDate.ToDBDate() + "',");
                    Sql.Append(" StatusCode = '" + StatusCode.SanitizeInput() + "',");
                    Sql.Append(" Description = '" + Description.SanitizeInput() + "'");
                    Sql.Append(" where Transaction_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }

            }
            catch (Exception Ex)
            {
                throw new Exception("Models > TransactionLog > Save " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["Transaction_ID"].ToString();
                OrderID = Data["Order_ID"].ToString();
                Amount = Convert.ToInt32(Data["Amount"].ToString());
                ClientID = Data["Client_ID"].ToString();
                RequestDate = Convert.ToDateTime(Data["RequestDate"]);
                if (!string.IsNullOrEmpty(Data["CompleteDate"].ToString()))
                {
                    CompleteDate = Convert.ToDateTime(Data["CompleteDate"]);
                }
                else
                {
                    CompleteDate = DateTime.Now;
                }
                StatusCode = Data["StatusCode"].ToString();
                Description = Data["Description"].ToString();
                PayRequestID = Data["PayRequestID"].ToString();
                Checksum = Data["Checksum"].ToString();
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > TransactionLog > DataRowToClass " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where Transaction_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > TransactionLog > Read " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" Transaction_ID,");
                Sql.Append(" Order_ID,");
                Sql.Append(" Amount,");
                Sql.Append(" Client_ID,");
                Sql.Append(" RequestDate,");
                Sql.Append(" CompleteDate,");
                Sql.Append(" StatusCode,");
                Sql.Append(" Description,");
                Sql.Append(" PayRequestID,");
                Sql.Append(" Checksum");
                Sql.Append(" from TransactionLog");
                return Sql.ToString();
            }
            public static List<TransactionLog> GetOrderTransactions(string OrderID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Order_ID = '" + OrderID.SanitizeInput() + "'");

                    DataTable TransactionDT = ReadDataTable(Sql.ToString());

                    List<TransactionLog> TransactionList = new List<TransactionLog>();
                    foreach (DataRow Data in TransactionDT.Rows)
                    {

                        TransactionLog Transaction = new TransactionLog();
                        Transaction.DataRowToClass(Data);
                        TransactionList.Add(Transaction);
                    }

                    return TransactionList;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > TransactionLog > Methods > GetOrderTransactions " + Ex.Message);
                }
            }

            public static int GetTotalTransactions()
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select Count(*) from TransactionLog");
                    return Convert.ToInt32(ExecuteScalar(Sql.ToString()));
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > TransactionLog > General > GetTotalTransactions" + Ex.Message);
                }
            }

            public static TransactionLog GetTransactionLogByPayRequestID(string PaymentRequestID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select");
                    Sql.Append(" Transaction_ID");
                    Sql.Append(" from TransactionLog");
                    Sql.Append(" where PayRequestID = '" + PaymentRequestID.SanitizeInput() + "'");

                    string TransactionLogID = ExecuteScalar(Sql.ToString());
                    return new TransactionLog(TransactionLogID);
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > TransactionLog > General > GetTransactionLogByPaymentReference" + Ex.Message);
                }
            }
        }
    }
}
