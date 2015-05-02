using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using Finisar.SQLite;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BaseLib
{
    public class DataBaseHandler
    {
        //public static string CONstr = "Data Source=" + Application.StartupPath + "\\DB_PINDominator.db" + ";Version=3;";
        //public static string CONstr = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Instagram\\DB_Instagram.db" + ";Version=3;";

        public static string CONstr = "Data Source=" + Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\InstagramDB\\Instagram.db" + ";Version=3;";
        public static DataSet SelectQuery(string query, string tablename)
        {
            //try
            //{

                DataSet DS = new DataSet();
            
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    AD.Fill(DS, tablename);
                }
                return DS;
            //}
            //catch
            //{
            //    return new DataSet();
            //}
        }

        public static void InsertQuery(string query, string tablename)
        {
            //try
            //{
            using (SQLiteConnection CON = new SQLiteConnection(CONstr))
            {
               
                SQLiteCommand CMD = new SQLiteCommand(query, CON);
                SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                DataSet DS = new DataSet();
                AD.Fill(DS, tablename);
            }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

        public static void DeleteQuery(string query, string tablename)
        {
            //try
            //{
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    DataSet DS = new DataSet();
                    AD.Fill(DS, tablename);
                }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

        public static void UpdateQuery(string query, string tablename)
        {
            //try
            //{
                using (SQLiteConnection CON = new SQLiteConnection(CONstr))
                {
                    SQLiteCommand CMD = new SQLiteCommand(query, CON);
                    SQLiteDataAdapter AD = new SQLiteDataAdapter(CMD);
                    DataSet DS = new DataSet();
                    AD.Fill(DS, tablename);
                }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //}
        }

    }
}
