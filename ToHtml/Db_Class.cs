using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;

namespace ToHtml
{
    public class Db_Class
    {

        private OracleConnection Conn;
        private OracleCommand cmd;

        //构造函数
        public Db_Class()
        {
            try
            {

                string connEJETDB247OCEAN = ConfigurationManager.AppSettings["EJETDB247OCEAN"];
                Conn = new OracleConnection(connEJETDB247OCEAN); //连接数据库
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
        }


        public Db_Class(string connName)
        {
            try
            {
                string strConn = ConfigurationManager.AppSettings[connName].ToString();
                Conn = new OracleConnection(strConn); //连接数据库
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
        }
        //打开数据源链接
        public OracleConnection Db_Conn()
        {
            Conn.Open();
            return Conn;
        }


        //返回数据DataSet数据集
        public DataTable Db_CreateDataTable(string SQL)
        {

            try
            {
                Db_Conn();
                cmd = new OracleCommand(SQL, Conn);
                OracleDataAdapter Adpt = new OracleDataAdapter(cmd);
                DataSet Ds = new DataSet();
                Adpt.Fill(Ds, "NewTable");
                this.close();
                return Ds.Tables[0];

            }
            catch (Exception e)
            {
                this.close();
                CreatLogTxt.ErrWriter(e);
            }
            return null;
        }

        //返回数据DataReader数据集，不需要返回数据的修改，删除可以使用本函数
        public bool Db_ExecuteNonquery(string SQL)
        {
            try
            {
                Db_Conn();
                cmd = new OracleCommand(SQL, Conn);
                try
                {
                    cmd.ExecuteNonQuery();

                    return true;
                }
                catch (Exception e)
                {
                    CreatLogTxt.ErrWriter(e);
                    return false;
                }
            }
            finally
            {
                this.close();
            }

        }
        //返回数据DataReader数据集，返回数据
        public String Db_Executequery(string SQL)
        {
            try
            {
                Db_Conn();
                cmd = new OracleCommand(SQL, Conn);
                try
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        return obj.ToString();
                    }
                }
                catch (Exception e)
                {
                    CreatLogTxt.ErrWriter(e);

                }
            }
            finally
            {
                this.close();
            }
            return "";
        }

        //关闭数据链接
        public void close()
        {
            Conn.Close();
        }

    }
}
