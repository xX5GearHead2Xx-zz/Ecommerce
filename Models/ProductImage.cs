using System;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace Ecommerce.Models
{
    public class ProductImage : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string ProductID { get; set; }
        private byte[] _data;
        public byte[] Data
        {
            get
            {
                if (_data != null)
                {
                    if (_data.Length > 0)
                    {
                        return _data;
                    }
                }
                _data =  new AzureStorage().Download(Key.SanitizeInput());
                return _data;
            }
            set
            {
                _data = value;
            }
        }
        public int Order { get; set; }
        #endregion
        public ProductImage(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    ProductID = "";
                    Data = new byte[0];
                    Order = 0;
                }
                else
                {
                    DataRowToClass(Read(key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductImage > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from ProductImage");
                Sql.Append(" where ProductImage_ID = '" + Key.SanitizeInput() + "'");

                if (ExecuteNonQuery(Sql.ToString()))
                {
                    return new AzureStorage().Delete(Key.SanitizeInput());
                }
                return false;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductImage > Delete " + Ex.Message);
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

                    Sql.Append(" insert into ProductImage (");
                    Sql.Append(" ProductImage_ID,");
                    Sql.Append(" Product_ID,");
                    Sql.Append(" [Order]");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + ProductID.SanitizeInput() + "',");
                    Sql.Append(" " + Order + "");
                    Sql.Append(" )");

                    if (new AzureStorage().Upload(Key.SanitizeInput(), Data))
                    {
                        return ExecuteNonQuery(Sql.ToString());
                    }
                    return false;
                }
                else
                {
                    //If the image entry already exists, we only need to update the storage bytes
                    AzureStorage Storage = new AzureStorage();
                    return Storage.Upload(Key.SanitizeInput(), Data);
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
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where ProductImage_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductImage > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Datarow)
        {
            try
            {
                Key = Datarow["ProductImage_ID"].ToString();
                ProductID = Datarow["Product_ID"].ToString();
                Order = Convert.ToInt32(Datarow["Order"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > ProductImage > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" ProductImage_ID,");
                Sql.Append(" Product_ID,");
                Sql.Append(" [Order]");
                Sql.Append(" from ProductImage");
                return Sql.ToString();
            }
            public static List<ProductImage> GetProductImages(string ProductID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Product_ID = '" + ProductID.SanitizeInput() + "'");

                    DataTable ProductImageDT = ReadDataTable(Sql.ToString());

                    List<ProductImage> ProductImages = new List<ProductImage>();

                    foreach (DataRow ProductDR in ProductImageDT.Rows)
                    {
                        ProductImage ProductImage = new ProductImage();
                        ProductImage.DataRowToClass(ProductDR);
                        ProductImages.Add(ProductImage);
                    }

                    return ProductImages;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > ProductImage > General > GetProductImages " + Ex.Message);
                }
            }

            public static ProductImage GetFirstProductImage(string ProductID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    Sql.Append(" where Product_ID = '" + ProductID + "'");
                    Sql.Append(" and [Order] = 1");

                    DataRow ProductDR = ReadDataRow(Sql.ToString());
                    ProductImage ProductImage = new ProductImage();
                    ProductImage.DataRowToClass(ProductDR);
                    return ProductImage;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > ProductImage > General > GetFirstProductImage " + Ex.Message);
                }
            }
        }
    }
}
