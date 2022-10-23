using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class ProductCategory : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string Description { get; set; }
        public string DepartmentID { get; set; }
        public bool Hidden { get; set; }
        #endregion

        public ProductCategory(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    Description = "";
                    DepartmentID = "";
                    Hidden = false;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductCategory > Constructor " + Ex.Message);
            }
        }

        public bool HideUnhide(bool MarkAsHidden)
        {
            try
            {
                int Hide = Convert.ToInt32(MarkAsHidden);
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" update ProductCategory set Hidden = " + Hide + "");
                Sql.Append(" where ProductCategory_ID = '" + Key.SanitizeInput() + "'");
                if (ExecuteNonQuery(Sql.ToString()))
                {
                    Sql = new StringBuilder();
                    List<Product> CategoryProducts = Product.Methods.GetProducts(10000, 0, true, true, false, "", Key);
                    foreach (Product Category in CategoryProducts)
                    {
                        Sql.Append("update Product set Hidden = " + Hide + " where Category_ID = '" + Category.Key + "'");
                    }
                    return ExecuteNonQuery(Sql.ToString());
                }
                return false;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductDepartment > Delete " + Ex.Message);
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

                    Sql.Append(" insert into ProductCategory (");
                    Sql.Append(" ProductCategory_ID,");
                    Sql.Append(" Description,");
                    Sql.Append(" Department_ID,");
                    Sql.Append(" Hidden");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + Description.SanitizeInput() + "',");
                    Sql.Append(" '" + DepartmentID.SanitizeInput() + "',");
                    Sql.Append("" + Convert.ToInt32(Hidden) + "");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update ProductCategory set");
                    Sql.Append(" Description = '" + Description.SanitizeInput() + "'");
                    Sql.Append(" where ProductCategory_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductCategory > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where ProductCategory_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductCategory > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["ProductCategory_ID"].ToString();
                Description = Data["Description"].ToString();
                DepartmentID = Data["Department_ID"].ToString();
                Hidden = Convert.ToBoolean(Data["Hidden"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductCategory > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" ProductCategory_ID,");
                Sql.Append(" Description,");
                Sql.Append(" Department_ID,");
                Sql.Append(" Hidden");
                Sql.Append(" from ProductCategory");
                return Sql.ToString();
            }
            public static List<ProductCategory> GetDepartmentCategories(string DepartmentID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Department_ID = '" + DepartmentID.SanitizeInput() + "'");

                    DataTable ProductCategoryDT = ReadDataTable(Sql.ToString());

                    List<ProductCategory> ProductCategories = new List<ProductCategory>();

                    foreach (DataRow ProductCategoryDR in ProductCategoryDT.Rows)
                    {
                        ProductCategory Category = new ProductCategory();
                        Category.DataRowToClass(ProductCategoryDR);
                        ProductCategories.Add(Category);
                    }

                    return ProductCategories;

                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > ProductCategory > GetDepartmentCategories " + Ex.Message);
                }
            }
        }
    }
}
