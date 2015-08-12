using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DemoStagramPro.Classes
{
    class QueryExecuter
    {
        private const string tableAccountInfo = "AccountInfo";
        private const string tableFollowInfo = "FollowInfo";
        private const string tableLikeInfo = "LikeInfo";

        public static void insertAccount(string username, string password, string addr, string port, string user, string pass, string path)
        {
            try
            {
                string insertQuery = "Insert into " + tableAccountInfo + " (Username, Password,IPAddress,IPPort,IPUsername,IPPassword,Path) values('" + username + "','" + password + "','" + addr + "','" + port + "','" + user + "','" + pass + "','" + path + "')";
                DBHandler.InsertQuery(insertQuery, tableAccountInfo);
            }
            catch { }
        }

        public static void deleteQuery()
        {
            try
            {
                string deleteQuery = "Delete from " + tableAccountInfo;
                DBHandler.DeleteQuery(deleteQuery, tableAccountInfo);
            }
            catch { }
        }

        public DataSet getAccount()
        {
            DataSet ds = new DataSet();
            try
            {
                string selectQuery = "Select Username,Password,IPAddress,IPPort,IPUsername,IPPassword,Path from " + tableAccountInfo;
                ds = DBHandler.SelectQuery(selectQuery, tableAccountInfo);
            }
            catch { }
            return ds;
        }

        public static void insertFollowInfo(string account, string user, string status)
        {
            string dt = String.Format("{0:MM/dd/yyyy}", DateTime.Now);
            try
            {
                string insertQuery = "insert into " + tableFollowInfo + "(AccountHolder,FollowingUser,FollowTime,FollowStatus) values('" + account + "','" + user + "','" + dt + "','" + status + "')";
                DBHandler.InsertQuery(insertQuery, tableFollowInfo);
            }
            catch { }
        }

        public static DataSet getFollowInfo(string accountHolder)
        {
            DataSet ds = new DataSet();
            try
            {
                string selectQuery = "Select FollowingUser,FollowTime from " + tableFollowInfo + " where AccountHolder='" + accountHolder + "'";
                ds = DBHandler.SelectQuery(selectQuery, tableFollowInfo);
            }
            catch { }
            return ds;
        }

        /// <summary>
        /// LikeInfo Insert,Update,Select
        /// </summary>
        /// <returns></returns>

        public DataSet getLikeInfo()
        {
            string SelectQuery = "select LikePhotoId from LikeInfo where Status=0";
            DataSet ds = DBHandler.SelectQuery(SelectQuery, "LikeInfo");
            return ds;
        }

        public static void insertPhotoId(string UseName, string LikePhotoId)
        {

            try
            {
                string insertQuery = "insert into " + tableLikeInfo + "(UseName,Status,LikePhotoId) values('" + UseName + "','" + "1" + "','" + LikePhotoId + "')";
                DBHandler.InsertQuery(insertQuery, tableLikeInfo);
            }
            catch { }
        }

        public static void UpdateStatusPhotoId(string UseName, string LikePhotoId)
        {
            try
            {
                string UpdateQuery = "UPDATE " + tableLikeInfo + " SET Status =1 Where LikePhotoId ='" + LikePhotoId + "'";
                DBHandler.UpdateQuery(UpdateQuery, "tableLikeInfo");
            }
            catch { };
        }

        public static string likeStatus(string id, string username)
        {
            string status = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                string selectQuery = "select Status from " + tableLikeInfo + " where UseName='" + username + "' and PhotoId='" + id + "'";
                ds = DBHandler.SelectQuery(selectQuery, tableLikeInfo);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int cnt = Convert.ToInt32(ds.Tables[0].Rows[0].ItemArray[0]);
                    if (cnt == 1)
                    {
                        status = "Liked";
                    }

            
                    else if (cnt == 2)
                    {
                        status = "Unliked";
                    }
                }
            }
            catch { }
            return status;
        }

        public static void insertLikeStatus(string id, string username, int status)
        {
            try
            {
                string insertQuery = "Insert into " + tableLikeInfo + " (UseName,Status,PhotoId) values('" + username + "'," + status + ",'" + id + "')";
                DBHandler.InsertQuery(insertQuery, tableLikeInfo);
            }
            catch { }
        }

        public static string getFollowStatus(string user, string followingUser)
        {
            string status = string.Empty;
            DataSet ds = new DataSet();
            try
            {
               
                string selectQuery = "Select FollowStatus from " + tableFollowInfo + " where AccountHolder='" + user + "' and FollowingUser='" + followingUser + "'";
                ds = DBHandler.SelectQuery(selectQuery, tableFollowInfo);
            }
            catch { }
            if (ds.Tables[0].Rows.Count > 0)
            {
                try
                {
                    status = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                }
                catch { }
            }
            return status;
        }

        public static void updateFollowStatus(string user, string followingUser, string status)
        {
            try
            {
                string perfomQuery = "Update " + tableFollowInfo + " set FollowStatus='" + status + "' where AccountHolder='" + user + "' and FollowingUser='" + followingUser + "'";
                DBHandler.PerformQuery(perfomQuery, tableFollowInfo);
            }
            catch { }
        }
    }
}
        
