using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BaseLib
{
    public class clsSettingDB
    {
        string Upmodule = string.Empty;
        string Upfiletype = string.Empty;
        string Upfilepath = string.Empty;
        string DeathByCaptcha = string.Empty;

        /// <summary>
        /// Inserts Settings in DataBase
        /// Updates if Settings already present
        /// </summary>
        /// <param name="module"></param>
        /// <param name="filetype"></param>
        /// <param name="filepath"></param>
        public void InsertOrUpdateSetting(string module,string filetype,string filepath)
        {
            try
            {
                this.Upmodule=module;
                this.Upfiletype=filetype;
                this.Upfilepath=filepath;

                string Upmodule=module;
                string UPfiletype=filetype;
                string strQuery = "INSERT INTO tb_Setting VALUES ('" + module + "','" + filetype + "','" + filepath  + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
            }
            catch (Exception)
            {
                UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
            }

        }
        public DataTable SelectSettingData()
        {
            try
            {

                string strQuery = "SELECT * FROM tb_Setting";

                DataSet ds = DataBaseHandler.SelectQuery(strQuery, "tb_Setting");

                DataTable dt = ds.Tables["tb_Setting"];
                return dt;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }
        public void UpdateSettingData(string module, string filetype, string filepath)
        {
            try
            {
                string strTable = "tb_Setting";
                string strQuery = "UPDATE tb_Setting SET Module='" + module + "', FilePath='" + filepath + "' WHERE FileType='" + filetype + "'";

                DataBaseHandler.UpdateQuery(strQuery, strTable);
            }
            catch (Exception)
            {

            }
        }
        public void InsertDBCData(string username, string DeathByCaptcha, string password)
        {
            try
            {
                
                string strQuery = "INSERT INTO tb_Setting VALUES ('" + username + "','" + DeathByCaptcha + "','" + password + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
            }
            catch (Exception)
            {
                UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
            }
        }
        public void DeleteDBCDecaptcherData(string strDeathByCaptcha)
        {
            try
            {

                string strTable = "tb_Setting";
                string strQuery = "DELETE FROM tb_Setting WHERE FileType='" + strDeathByCaptcha + "'";

                DataBaseHandler.DeleteQuery(strQuery, strTable);
            }
            catch (Exception)
            {
            }
        }

        public void InsertDecaptcherData(string server, string port, string username, string password, string Decaptcher)
        {
            try
            {

                string strQuery = "INSERT INTO tb_Setting VALUES ('" + server + "<:>" + port + "','" + Decaptcher + "','" + username + "<:>" + password + "') ";

                DataBaseHandler.InsertQuery(strQuery, "tb_Setting");
            }
            catch (Exception)
            {
                UpdateSettingData(Upmodule, Upfiletype, Upfilepath);
            }
        }
        //public void DeleteDecaptcherData(string strDecaptcher)
        //{
        //    try
        //    {

        //        string strTable = "tb_Setting";
        //        string strQuery = "DELETE FROM tb_Setting WHERE FileType=" + strDecaptcher;

        //        DataBaseHandler.DeleteQuery(strQuery, strTable);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}
