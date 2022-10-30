using System;
using System.Collections.Generic;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Text;
using System.Data;

namespace Ecommerce.Models
{
    public class Address : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string ClientID { get; set; }
        public string AddressLine { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public Province Province { get; set; }
        public string ProvinceDescription
        {
            get
            {
                return Methods.GetProvinceDescription(Province);
            }
        }
        public int ZipCode { get; set; }
        public string DeliveryInstructions { get; set; }
        public string ContactNumber { get; set; }
        public PropertyType PropertyType { get; set; }
        #endregion
        public Address(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    AddressLine = "";
                    Suburb = "";
                    City = "";
                    Province = Province.Unknown;
                    ZipCode = 0;
                    DeliveryInstructions = "";
                    ContactNumber = "";
                    PropertyType = PropertyType.Unknown;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Address > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from Address");
                Sql.Append(" where Address_ID = '" + Key.SanitizeInput() + "'");

                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Address > Delete " + Ex.Message);
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

                    Sql.Append(" insert into Address (");
                    Sql.Append(" Address_ID,");
                    Sql.Append(" Client_ID,");
                    Sql.Append(" AddressLine,");
                    Sql.Append(" Suburb,");
                    Sql.Append(" City,");
                    Sql.Append(" Province,");
                    Sql.Append(" ZipCode,");
                    Sql.Append(" DeliveryInstructions,");
                    Sql.Append(" ContactNumber,");
                    Sql.Append(" PropertyType");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + ClientID.SanitizeInput() + "',");
                    Sql.Append(" '" + AddressLine.SanitizeInput() + "',");
                    Sql.Append(" '" + Suburb.SanitizeInput() + "',");
                    Sql.Append(" '" + City.SanitizeInput() + "',");
                    Sql.Append(" '" + (int)Province + "',");
                    Sql.Append(" " + ZipCode + ",");
                    Sql.Append("'" + DeliveryInstructions + "',");
                    Sql.Append("'" + ContactNumber + "',");
                    Sql.Append("" + (int)PropertyType + "");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update Address set");
                    Sql.Append(" Client_ID = '" + ClientID.SanitizeInput() + "',");
                    Sql.Append(" AddressLine = '" + AddressLine.SanitizeInput() + "',");
                    Sql.Append(" Suburb = '" + Suburb.SanitizeInput() + "',");
                    Sql.Append(" City = '" + City.SanitizeInput() + "',");
                    Sql.Append(" Province = '" + (int)Province + "',");
                    Sql.Append(" ZipCode = " + ZipCode + ",");
                    Sql.Append(" DeliveryInstructions = " + DeliveryInstructions.SanitizeInput() + ",");
                    Sql.Append(" ContactNumber = " + ContactNumber.SanitizeInput() + ",");
                    Sql.Append(" PropertyType = " + (int)PropertyType + "");
                    Sql.Append(" where Address_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }


            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Address > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where Address_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Event > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["Address_ID"].ToString();
                ClientID = Data["Client_ID"].ToString();
                AddressLine = Data["AddressLine"].ToString();
                Suburb = Data["Suburb"].ToString();
                City = Data["City"].ToString();
                Province = (Province)Convert.ToInt32(Data["Province"].ToString());
                ZipCode = Convert.ToInt32(Data["ZipCode"].ToString());
                DeliveryInstructions = Data["DeliveryInstructions"].ToString();
                ContactNumber = Data["ContactNumber"].ToString();
                PropertyType = (PropertyType)Convert.ToInt32(Data["PropertyType"]);
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Address > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" Address_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" AddressLine,");
                Sql.Append(" Suburb,");
                Sql.Append(" City,");
                Sql.Append(" Province,");
                Sql.Append(" ZipCode,");
                Sql.Append(" DeliveryInstructions,");
                Sql.Append(" ContactNumber,");
                Sql.Append(" PropertyType");
                Sql.Append(" from Address");
                return Sql.ToString();
            }
            public static List<Address> GetClientAddresses(string ClientID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Client_ID = '" + ClientID.SanitizeInput() + "'");

                    DataTable AddressDT = ReadDataTable(Sql.ToString());

                    List<Address> Addresses = new List<Address>();

                    foreach (DataRow AddressDR in AddressDT.Rows)
                    {
                        Address Address = new Address();
                        Address.DataRowToClass(AddressDR);
                        Addresses.Add(Address);
                    }

                    return Addresses;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Address > GetClientAddresses " + Ex.Message);
                }
            }

            public static string GetProvinceDescription(Province ProvinceID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append("select Description from ReferenceProvince where Province_ID = '" + (int)ProvinceID + "'");
                    return ExecuteScalar(Sql.ToString());
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Address > GetProvinceDescription " + Ex.Message);
                }
            }
        }
    }
}