using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class Product : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string CategoryID { get; set; }
        public string Brand { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
        public int CartQuantity { get; set; }
        public bool Featured { get; set; }
        public bool Hidden { get; set; }
        #endregion

        public ProductCategory Category
        {
            get
            {
                return new ProductCategory(CategoryID);
            }
        }

        public ProductImage FeaturedImage
        {
            get
            {
                return ProductImage.Methods.GetFirstProductImage(Key);
            }
        }

        public List<ProductImage> Images
        {
            get
            {
                return ProductImage.Methods.GetProductImages(Key);
            }
        }

        public List<ProductSpecification> Specifications
        {
            get
            {
                return ProductSpecification.Methods.GetProductSpecifications(Key);
            }
        }

        public Product(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    Name = "";
                    Description = "";
                    Price = 0;
                    Quantity = 0;
                    CategoryID = "";
                    Brand = "";
                    Height = 0;
                    Width = 0;
                    Depth = 0;
                    Weight = 0;
                    Color = "";
                    Featured = false;
                    Hidden = false;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Product > Constructor " + Ex.Message);
            }
        }

        public bool HideUnhide(bool MarkAsHidden)
        {
            try
            {
                int Hide = Convert.ToInt32(MarkAsHidden);
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" update Product set Hidden = " + Hide + " ");
                Sql.Append(" where Product_ID = '" + Key.SanitizeInput() + "'");

                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Product > Delete " + Ex.Message);
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

                    Sql.Append(" insert into Product (");
                    Sql.Append(" Product_ID,");
                    Sql.Append(" Name,");
                    Sql.Append(" Description,");
                    Sql.Append(" Price,");
                    Sql.Append(" Quantity,");
                    Sql.Append(" Category_ID,");
                    Sql.Append(" Brand,");
                    Sql.Append(" WidthInMillimeters ,");
                    Sql.Append(" HeightInMillimeters,");
                    Sql.Append(" DepthInMillimeters,");
                    Sql.Append(" WeightInGrams,");
                    Sql.Append(" Color,");
                    Sql.Append(" Featured,");
                    Sql.Append(" Hidden");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + Name.SanitizeInput() + "',");
                    Sql.Append(" '" + Description.SanitizeInput() + "',");
                    Sql.Append(" " + Price + ",");
                    Sql.Append(" " + Quantity + ",");
                    Sql.Append(" '" + CategoryID.SanitizeInput() + "',");
                    Sql.Append(" '" + Brand.SanitizeInput() + "',");
                    Sql.Append(" " + Width + ",");
                    Sql.Append(" " + Height + ",");
                    Sql.Append(" " + Depth + ",");
                    Sql.Append(" " + Weight + ",");
                    Sql.Append(" '" + Color.SanitizeInput() + "',");
                    Sql.Append(" " + Convert.ToInt32(Featured) + ",");
                    Sql.Append(" " + Convert.ToInt32(Hidden) + "");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update Product set");
                    Sql.Append(" Name = '" + Name.SanitizeInput() + "',");
                    Sql.Append(" Description = '" + Description.SanitizeInput() + "',");
                    Sql.Append(" Price = " + Price + ",");
                    Sql.Append(" Quantity = " + Quantity + ",");
                    Sql.Append(" Category_ID = '" + CategoryID.SanitizeInput() + "',");
                    Sql.Append(" Brand = '" + Brand.SanitizeInput() + "',");
                    Sql.Append(" WidthInMillimeters = " + Width + ",");
                    Sql.Append(" HeightInMillimeters = " + Height + ",");
                    Sql.Append(" DepthInMillimeters = " + Depth + ",");
                    Sql.Append(" WeightInGrams = " + Weight + ",");
                    Sql.Append(" Color = '" + Color.SanitizeInput() + "',");
                    Sql.Append(" Featured = " + Convert.ToInt32(Featured) + "");
                    Sql.Append(" where Product_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }

            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Product > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where Product_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Product > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["Product_ID"].ToString();
                Name = Data["Name"].ToString();
                Description = Data["Description"].ToString();
                Price = Convert.ToInt32(Data["Price"].ToString());
                Quantity = Convert.ToInt32(Data["Quantity"].ToString());
                CategoryID = Data["Category_ID"].ToString();
                Brand = Data["Brand"].ToString();
                Width = Convert.ToInt32(Data["WidthInMillimeters"].ToString());
                Height = Convert.ToInt32(Data["HeightInMillimeters"].ToString());
                Depth = Convert.ToInt32(Data["DepthInMillimeters"].ToString());
                Weight = Convert.ToInt32(Data["WeightInGrams"].ToString());
                Color = Data["Color"].ToString();
                Featured = Convert.ToBoolean(Data["Featured"].ToString());
                Hidden = Convert.ToBoolean(Data["Hidden"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Product > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" Product_ID,");
                Sql.Append(" Name,");
                Sql.Append(" Description,");
                Sql.Append(" Price,");
                Sql.Append(" Quantity,");
                Sql.Append(" Category_ID,");
                Sql.Append(" Brand,");
                Sql.Append(" WidthInMillimeters ,");
                Sql.Append(" HeightInMillimeters,");
                Sql.Append(" DepthInMillimeters,");
                Sql.Append(" WeightInGrams,");
                Sql.Append(" Color,");
                Sql.Append(" Featured,");
                Sql.Append(" Hidden");
                Sql.Append(" from Product");
                return Sql.ToString();
            }
            public static int GetTotalProducts(bool IncludeHidden)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select COUNT(*) from Product where Hidden in (0" + (IncludeHidden ? ",1" : "") + ")");
                    return Convert.ToInt32(ExecuteScalar(Sql.ToString()));
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > ProductDepartment > GetProductDepartments " + Ex.Message);
                }
            }

            public static List<Product> GetProducts(int Fetch, int Skip, bool IncludeOutOfStock, bool IncludeHidden, bool FeaturedOnly, string ProductName = "", string ProductCategoryID = "")
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" Where Hidden in (0" + (IncludeHidden ? ",1" : "") + ") ");
                    if (FeaturedOnly)
                    {
                        Sql.Append(" and Featured = 1");
                    }
                    if (!IncludeOutOfStock)
                    {
                        Sql.Append(" and Quantity > 0");
                    }
                    if (!string.IsNullOrEmpty(ProductName))
                    {
                        Sql.Append(" and Name like '%" + ProductName.SanitizeInput() + "%'");
                    }
                    if (!string.IsNullOrEmpty(ProductCategoryID))
                    {
                        Sql.Append(" and Category_ID = '" + ProductCategoryID.SanitizeInput() + "'");
                    }
                    Sql.Append(" order by Price");
                    Sql.Append(" OFFSET " + Skip + " ROWS");
                    Sql.Append(" FETCH NEXT " + Fetch + " ROWS ONLY");
                    DataTable ProductDT = ReadDataTable(Sql.ToString());

                    List<Product> Products = new List<Product>();

                    foreach (DataRow ProductDR in ProductDT.Rows)
                    {
                        Product Product = new Product();
                        Product.DataRowToClass(ProductDR);
                        Products.Add(Product);
                    }

                    return Products;

                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Product > GetProducts " + Ex.Message);
                }
            }
        }
    }
}
