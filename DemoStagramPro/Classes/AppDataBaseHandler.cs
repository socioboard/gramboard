using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.SQLite;
using System.Data;
using BaseLib;

namespace InstagramManager.Classes
{
    public class AppDataBaseHandler
    {
        public DataTable SelectAccoutsForGridView(string table)
        {
            try
            {
                List<string> lstAccount = new List<string>();
                string strQuery = "SELECT * FROM " + table + "";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, table);

                DataTable dt = ds.Tables[table];

                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }
    }
}
