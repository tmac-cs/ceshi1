using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ToHtml
{
    /// <summary>
    /// 错误日志的输出。
    /// </summary>
    public class CreatLogTxt
    {

        /// <summary>
        /// 写入错误日志文件。2提交，3提交，4提交一
        /// </summary>
        /// <param name="ex">错误对象。</param>
        static public void ErrWriter(Exception ex)
        {
            try
            {
                string AppPath = AppDomain.CurrentDomain.BaseDirectory + @"logs";
                string FileName = DateTime.Now.ToString("yyyyMMdd");
                StreamWriter StrW = new StreamWriter(AppPath + @"/" + FileName + ".log", true);
                string str = string.Empty;
                str = string.Format("时间：{0} Message:{1} \r\n Source:{2} \r\n StackTrace:{3}  \r\n TargetSite{4}", DateTime.Now.ToString(), ex.Message, ex.Source, ex.StackTrace, ex.TargetSite);
                StrW.WriteLine(str);
                StrW.Flush();
                StrW.Close();
            }
            catch { }
        }
        static public void ErrWriter(string strError)
        {
            try
            {
                string AppPath = AppDomain.CurrentDomain.BaseDirectory + @"logs";
                string FileName = DateTime.Now.ToString("yyyyMMdd");
                StreamWriter StrW = new StreamWriter(AppPath + @"/" + FileName + ".log", true);
                string str = string.Empty;
                str = string.Format("时间：{0} Message:{1} ", DateTime.Now.ToString(), strError);
                StrW.WriteLine(str);
                StrW.Flush();
                StrW.Close();
            }
            catch { }
        }
    }
}
