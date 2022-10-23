using System;
using System.Collections.Generic;
using static Ecommerce.Models.Enums;
using static DataHandler.SqlHandler;
using System.Text;
using System.Data;

namespace Ecommerce.Models
{
    public class IndexImage : BaseModel
    {
        #region "Properties"
        public string Key { get; set; }
        public string URL { get; set; }

        private byte[] _data;
        public byte[] Data
        {
            get
            {
                if(_data != null)
                {
                    if(_data.Length > 0)
                    {
                        return _data;
                    }
                }
                _data = new AzureStorage().Download(Key.SanitizeInput());
                return _data;
            }
            set
            {
                _data = value;
            } 
        }

        #endregion

        public IndexImage(string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    Key = "";
                    URL = "";
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
                Sql.Append(" delete from IndexImage");
                Sql.Append(" where IndexImage_ID = '" + Key.SanitizeInput() + "'");
                if (new AzureStorage().Delete(Key.SanitizeInput()))
                {
                    return ExecuteNonQuery(Sql.ToString());
                }

                return false;
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > IndexImage > Delete " + Ex.Message);
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

                    Sql.Append(" insert into IndexImage (");
                    Sql.Append(" IndexImage_ID,");
                    Sql.Append(" URL");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + URL.SanitizeInput() + "'");
                    Sql.Append(" )");

                    AzureStorage Storage = new AzureStorage();
                    if (Storage.Upload(Key.SanitizeInput(), Data))
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
                throw new Exception("Models > IndexImage > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSql());
                Sql.Append(" where IndexImage_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > IndexImage > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Datarow)
        {
            try
            {
                Key = Datarow["IndexImage_ID"].ToString();
                URL = Datarow["URL"].ToString();
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > IndexImage > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSql()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" IndexImage_ID,");
                Sql.Append(" URL");
                Sql.Append(" from IndexImage");
                return Sql.ToString();
            }
            public static List<IndexImage> GetIndexImages()
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSql());
                    DataTable IndexImageDT = ReadDataTable(Sql.ToString());

                    List<IndexImage> IndexImages = new List<IndexImage>();

                    foreach (DataRow IndexImageDR in IndexImageDT.Rows)
                    {
                        IndexImage IndexImage = new IndexImage();
                        IndexImage.DataRowToClass(IndexImageDR);
                        IndexImages.Add(IndexImage);
                    }

                    return IndexImages;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > IndexImage > GetIndexImages " + Ex.Message);
                }
            }
        }
    }
}