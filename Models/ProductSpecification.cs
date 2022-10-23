using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class ProductSpecification : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string ProductID { get; set; }
        #endregion
        public ProductSpecification(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    Description = "";
                    Value = "";
                    ProductID = "";
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductSpecification > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from ProductSpecification");
                Sql.Append(" where ProductSpecification_ID = '" + Key.SanitizeInput() + "'");

                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductSpecification > Delete " + Ex.Message);
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

                    Sql.Append(" insert into ProductSpecification (");
                    Sql.Append(" ProductSpecification_ID,");
                    Sql.Append(" Description,");
                    Sql.Append(" Value,");
                    Sql.Append(" Product_ID");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + Description.SanitizeInput() + "',");
                    Sql.Append(" '" + Value.SanitizeInput() + "',");
                    Sql.Append(" '" + ProductID.SanitizeInput() + "'");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update ProductSpecification set");
                    Sql.Append(" Description = '" + Description.SanitizeInput() + "'");
                    Sql.Append(" where ProductSpecification_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductSpecification > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where ProductSpecification_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductSpecification > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["ProductSpecification_ID"].ToString();
                Description = Data["Description"].ToString();
                Value = Data["Value"].ToString();
                ProductID = Data["Product_ID"].ToString();
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductSpecification > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" ProductSpecification_ID,");
                Sql.Append(" Description,");
                Sql.Append(" Value,");
                Sql.Append(" Product_ID");
                Sql.Append(" from ProductSpecification");
                return Sql.ToString();
            }
            public static List<ProductSpecification> GetProductSpecifications(string ProductId)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Product_ID = '" + ProductId.SanitizeInput() + "'");

                    DataTable ProductSpecDT = ReadDataTable(Sql.ToString());

                    List<ProductSpecification> ProductSpecs = new List<ProductSpecification>();

                    foreach (DataRow ProductDR in ProductSpecDT.Rows)
                    {
                        ProductSpecification ProductSpec = new ProductSpecification();
                        ProductSpec.DataRowToClass(ProductDR);
                        ProductSpecs.Add(ProductSpec);
                    }

                    return ProductSpecs;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > ProductSpecification > General > GetProductSpecifications " + Ex.Message);
                }
            }
        }
    }
}
