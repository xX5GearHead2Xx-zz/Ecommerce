using System;
using System.Data;
using System.Data.SqlClient;

namespace DataHandler
{
    public class SqlHandler
    {
        private static string GetConnection()
        {
            return Ecommerce.Startup.StaticConfiguration["ConnectionStrings:SqlDBConnection"];
        }

        public static DataTable ReadDataTable(string Query)
        {
            try
            {
                DataTable DataTable = new DataTable();
                SqlConnection Connection = new SqlConnection(GetConnection());
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (Connection)
                    {
                        Connection.Open();
                        using (var DataReader = Command.ExecuteReader())
                        {
                            DataTable.Load(DataReader);
                        }
                    }
                }
                return DataTable;
            }
            catch (Exception Ex)
            {
                throw new Exception("DataHandler > Sqlhandler > Read " + Ex.Message);
            }
        }

        public static DataRow ReadDataRow(string Query)
        {
            try
            {
                DataTable DataTable = new DataTable();
                SqlConnection Connection = new SqlConnection(GetConnection());
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (Connection)
                    {
                        Connection.Open();
                        using (var DataReader = Command.ExecuteReader())
                        {
                            DataTable.Load(DataReader);
                        }
                    }
                }
                return DataTable.Rows[0];
            }
            catch (Exception Ex)
            {
                throw new Exception("DataHandler > Sqlhandler > Read " + Ex.Message);
            }
        }

        public static bool Write(string Query)
        {
            try
            {
                SqlConnection Connection = new SqlConnection(GetConnection());
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (Connection)
                    {
                        Connection.Open();
                        Command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception Ex)
            {
                throw new Exception("DataHandler > Sqlhandler > Write " + Ex.Message);
            }
        }

        public static string ExecuteScalar(string Query)
        {
            try
            {
                SqlConnection Connection = new SqlConnection(GetConnection());
                string ReturnValue = "";
                using (SqlCommand Command = new SqlCommand(Query, Connection))
                {
                    using (Connection)
                    {
                        Connection.Open();
                        var Value = Command.ExecuteScalar();
                        ReturnValue = Value != null ? Value.ToString() : "";
                    }
                }
                return ReturnValue;
            }
            catch (Exception Ex)
            {
                throw new Exception("DataHandler > Sqlhandler > ExecuteScalar " + Ex.Message);
            }
        }

        public static bool ExecuteNonQuery(string Query)
        {
            try
            {
                if (!string.IsNullOrEmpty(Query))
                {
                    SqlConnection Connection = new SqlConnection(GetConnection());
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        using (Connection)
                        {
                            Connection.Open();
                            Command.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                throw new Exception("DataHandler > Sqlhandler > ExecuteNonQuery " + Ex.Message);
            }
        }
    }
}
