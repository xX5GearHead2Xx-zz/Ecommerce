using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static DataHandler.SqlHandler;
using static Ecommerce.Models.Enums;

namespace Ecommerce.Models
{
    public class Review
    {
        #region "Properties"
        public string Key { get; set; }
        public string ClientID { get; set; }
        public string ProductID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
        public Enums.ReviewStatus Status { get; set; }
        #endregion

        public Product Product
        {
            get
            {
                return new Product(ProductID);
            }
        }

        public Client Client
        {
            get
            {
                return new Client(ClientID);
            }
        }

        public Review(string Key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(Key))
                {
                    Key = "";
                    ClientID = "";
                    ProductID = "";
                    Title = "";
                    Description = "";
                    Rating = 0;
                    Date = DateTime.Now;
                    Status = Enums.ReviewStatus.NotSet;
                }
                else
                {
                    DataRowToClass(Read(Key));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Review > Constructor " + Ex.Message);
            }
        }

        public bool Delete()
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" delete from Review ");
                Sql.Append(" where Review_ID = '" + Key.SanitizeInput() + "'");
                return ExecuteNonQuery(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Review > Delete " + Ex.Message);
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

                    Sql.Append(" insert into Review (");
                    Sql.Append(" Review_ID,");
                    Sql.Append(" Client_ID,");
                    Sql.Append(" Product_ID,");
                    Sql.Append(" Title,");
                    Sql.Append(" Description,");
                    Sql.Append(" Rating,");
                    Sql.Append(" Date,");
                    Sql.Append(" Status");
                    Sql.Append(" ) values (");
                    Sql.Append(" '" + Key.SanitizeInput() + "',");
                    Sql.Append(" '" + ClientID.SanitizeInput() + "',");
                    Sql.Append(" '" + ProductID + "',");
                    Sql.Append(" '" + Title.SanitizeInput() + "',");
                    Sql.Append(" '" + Description.SanitizeInput() + "',");
                    Sql.Append(" " + Rating + ",");
                    Sql.Append(" '" + Date + "',");
                    Sql.Append(" " + (int)Status + "");
                    Sql.Append(" )");

                    return ExecuteNonQuery(Sql.ToString());
                }
                else
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" update Review set");
                    Sql.Append(" Title = '" + Title.SanitizeInput() + "',");
                    Sql.Append(" Description = '" + Description.SanitizeInput() + "',");
                    Sql.Append(" Rating = " + Rating + ",");
                    Sql.Append(" Date = '" + Date + "',");
                    Sql.Append(" Approved = " + (int)Status + "");
                    Sql.Append(" where Review_ID = '" + Key.SanitizeInput() + "'");

                    return ExecuteNonQuery(Sql.ToString());
                }
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Review > Save " + Ex.Message);
            }
        }

        public static DataRow Read(string Key)
        {
            try
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(Methods.GetReadSQL());
                Sql.Append(" where Review_ID = '" + Key.SanitizeInput() + "'");
                return ReadDataRow(Sql.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Review > Read " + Ex.Message);
            }
        }

        public void DataRowToClass(DataRow Data)
        {
            try
            {
                Key = Data["Review_ID"].ToString();
                ClientID = Data["Client_ID"].ToString();
                ProductID = Data["Product_ID"].ToString();
                Title = Data["Title"].ToString();
                Description = Data["Description"].ToString();
                Rating = Convert.ToInt32(Data["Rating"].ToString());
                Date = Convert.ToDateTime(Data["Date"].ToString());
                Status = (Enums.ReviewStatus)Convert.ToInt32(Data["Status"].ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Models > Review > DataRowToClass " + Ex.Message);
            }
        }

        public class Methods
        {
            public static string GetReadSQL()
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" Review_ID,");
                Sql.Append(" Client_ID,");
                Sql.Append(" Product_ID,");
                Sql.Append(" Title,");
                Sql.Append(" Description,");
                Sql.Append(" Rating,");
                Sql.Append(" Date,");
                Sql.Append(" Status");
                Sql.Append(" from Review");
                return Sql.ToString();
            }

            public static int GetTotalReviews(ReviewStatus? ReviewStatus = null)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(" select Count(*) from Review");
                    if (ReviewStatus != null)
                    {
                        Sql.Append(" where Status = " + (int)ReviewStatus + "");
                    }
                    return Convert.ToInt32(ExecuteScalar(Sql.ToString()));
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Review > General > GetTotalReviews " + Ex.Message);
                }
            }

            public static List<Review> SearchReviews(int Fetch, int Skip, Enums.ReviewStatus Status, string ClientID = "", string ProductID = "")
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSQL());
                    Sql.Append(" where Status = " + (int)Status + "");

                    if (ClientID != "")
                        Sql.Append(" and Client_ID = '" + ClientID.SanitizeInput() + "'");

                    if (ProductID != "")
                        Sql.Append(" and Product_ID = '" + ProductID.SanitizeInput() + "'");

                    Sql.Append(" order by Date DESC");
                    Sql.Append(" OFFSET " + Skip + " ROWS");
                    Sql.Append(" FETCH NEXT " + Fetch + " ROWS ONLY");

                    DataTable ReviewDT = ReadDataTable(Sql.ToString());

                    List<Review> Reviews = new List<Review>();

                    foreach (DataRow ReviewDR in ReviewDT.Rows)
                    {
                        Review Review = new Review();
                        Review.DataRowToClass(ReviewDR);
                        Reviews.Add(Review);
                    }

                    return Reviews;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Review > GetClientReviews " + Ex.Message);
                }
            }

            public static List<Review> GetClientReviews(string ClientID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSQL());
                    Sql.Append(" where Client_ID = '" + ClientID + "'");
                    Sql.Append(" order by Date DESC");

                    DataTable ReviewDT = ReadDataTable(Sql.ToString());

                    List<Review> Reviews = new List<Review>();

                    foreach (DataRow ReviewDR in ReviewDT.Rows)
                    {
                        Review Review = new Review();
                        Review.DataRowToClass(ReviewDR);
                        Reviews.Add(Review);
                    }

                    return Reviews;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Review > GetClientReviews " + Ex.Message);
                }
            }

            public static List<Review> GetProductReviews(string ProductID)
            {
                try
                {
                    StringBuilder Sql = new StringBuilder();
                    Sql.Append(GetReadSQL());
                    Sql.Append(" where Client_ID = '" + ProductID + "'");
                    Sql.Append(" order by Date DESC");

                    DataTable ReviewDT = ReadDataTable(Sql.ToString());

                    List<Review> Reviews = new List<Review>();

                    foreach (DataRow ReviewDR in ReviewDT.Rows)
                    {
                        Review Review = new Review();
                        Review.DataRowToClass(ReviewDR);
                        Reviews.Add(Review);
                    }

                    return Reviews;
                }
                catch (Exception Ex)
                {
                    throw new Exception("Models > Review > GetClientReviews " + Ex.Message);
                }
            }

            public static List<SelectListItem> GetReviewStatusOptions(int SelectedStatusID)
            {
                StringBuilder Sql = new StringBuilder();
                Sql.Append(" select");
                Sql.Append(" ReviewStatus_ID,");
                Sql.Append(" Description");
                Sql.Append(" from ReferenceReviewStatus");

                DataTable StatusOptions = ReadDataTable(Sql.ToString());
                List<SelectListItem> Options = new List<SelectListItem>();
                foreach (DataRow Option in StatusOptions.Rows)
                {
                    Options.Add(new SelectListItem() { Value = Option["ReviewStatus_ID"].ToString(), Text = Option["Description"].ToString(), Selected = SelectedStatusID.ToString() == Option["ReviewStatus_ID"].ToString() ? true : false });
                }

                return Options;
            }
        }

    }
}
