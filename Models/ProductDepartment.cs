using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class ProductDepartment : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
        #endregion

        public List<ProductCategory> ProductCategories
        {
            get
            {
                return ProductCategory.Methods.GetDepartmentCategories(Key);
            }
        }

        public ProductDepartment(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    Description = "";
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductDepartment > Constructor " + Ex.Message);
            }
        }

        public bool HideUnhide(bool MarkAsHidden)
        {
            try
            {
                int Hide = Convert.ToInt32(MarkAsHidden);
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" update ProductDepartment set Hidden = " + Hide + "");
                Sql.Append(" where ProductDepartment_ID = '" + Key.SanitizeInput() + "'");
                if (ExecuteNonQuery(Sql.ToString()))
                {

                    Sql = new StringBuilder();
                    Sql.Append(" update ProductCategory set Hidden = " + Hide + "");
                    Sql.Append(" where Department_ID = '" + Key.SanitizeInput() + "'");
                    if (ExecuteNonQuery(Sql.ToString()))
                    {
                        Sql = new StringBuilder();
                        List<ProductCategory> DepartmentCategories = ProductCategory.Methods.GetDepartmentCategories(Key);
                        foreach (ProductCategory Category in DepartmentCategories)
                        {
                            Sql.Append("update Product set Hidden = " + Hide + " where Category_ID = '" + Category.Key + "'");
                        }
                        return ExecuteNonQuery(Sql.ToString());
                    }
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

                    Sql.Append(" insert into ProductDepartment (");
                    Sql.Append(" ProductDepartment_ID,");
                    Sql.Append(" Description,");
                    Sql.Append(" Hidden");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + Description.SanitizeInput() + "',");
                    Sql.Append(" " + Convert.ToInt32(Hidden) + "");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update ProductDepartment set");
                    Sql.Append(" Description = '" + Description.SanitizeInput() + "'");
                    Sql.Append(" where ProductDepartment_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductDepartment > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where ProductDepartment_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductDepartment > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["ProductDepartment_ID"].ToString();
                Description = Data["Description"].ToString();
                Hidden = Convert.ToBoolean(Data["Hidden"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductDepartment > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" ProductDepartment_ID,");
                Sql.Append(" Description,");
                Sql.Append(" Hidden");
                Sql.Append(" from ProductDepartment");
                return Sql.ToString();
            }
            public static List<ProductDepartment> GetProductDepartments(bool IncludeHidden)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" Where Hidden in (0" + (IncludeHidden ? ",1" : "") + ") ");

                    DataTable ProductDepartmentDT = ReadDataTable(Sql.ToString());

                    List<ProductDepartment> ProductDepartments = new List<ProductDepartment>();

                    foreach (DataRow ProductDepartmentDR in ProductDepartmentDT.Rows)
                    {
                        ProductDepartment Department = new ProductDepartment();
                        Department.DataRowToClass(ProductDepartmentDR);
                        ProductDepartments.Add(Department);
                    }

                    return ProductDepartments;

                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > ProductDepartment > GetProductDepartments " + Ex.Message);
                }
            }
        }
    }
}

