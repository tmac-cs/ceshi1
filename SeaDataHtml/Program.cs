using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToHtml;
using System.Threading;
using System.Data;

namespace SeaDataHtml
{
    public class Program
    {
        static void Main(string[] args)
        {
            //args = new string[] { "大风提示" };
            //args = new string[] { "预警" };
            //args = new string[] { "海上突发天气提示" }; 
            try
            {
                Console.WriteLine("程序开始...");
                if (args == null || args.Length < 1) { return; }
                Console.WriteLine("类别：" + args[0]);
                switch (args[0])
                {
                    case "大风提示":
                        Console.Title = "大风提示";
                        while (true)
                        {
                            {
                                DateTime dt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                                DataTable dTable = null;
                                dTable = TransformHtml.GetTips();
                                if (dTable != null && dTable.Rows.Count > 0)
                                {
                                    Console.WriteLine("当前更新时间" + dt.ToString("yyyy年MM月dd日"));
                                    //List<OBTRealInfo> list = new List<OBTRealInfo>();
                                    //list = TransformHtml.GetObtRealInfoList(dt);
                                    UpLoadFtp("大风提示", dt, dTable,null);
                                }
                                else
                                {
                                    Console.WriteLine("暂没有新的数据，系统休息5分钟");
                                }
                                Thread.Sleep(5 * 60 * 1000); //程序休息五分钟
                            }
                        }

                    case "预警":
                        Console.Title = "预警";
                        while (true)
                        {
                            {
                                DateTime dt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                                DataTable dTable = null;
                                List<WarningEntity> warningEntities = TransformHtml.GetWarningds("");
                                if (warningEntities != null && warningEntities.Count > 0)
                                {
                                    Console.WriteLine("当前更新时间" + dt.ToString("yyyy年MM月dd日"));
                                    //List<OBTRealInfo> list = new List<OBTRealInfo>();
                                    //list = TransformHtml.GetObtRealInfoList(dt);
                                    UpLoadFtp("预警", dt, dTable,warningEntities);
                                }
                                else
                                {
                                    Console.WriteLine("暂没有新的数据，系统休息5分钟");
                                }
                                Thread.Sleep(5 * 60 * 1000);
                            }
                        }

                    case "海上突发天气提示":
                        Console.Title = "海上突发天气提示";
                        while (true)
                        {
                            {
                                DateTime dt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                                DataTable dTable = null;
                                dTable = TransformHtml.GetSevereWeatherAlarm();
                                if (dTable != null && dTable.Rows.Count > 0)
                                {
                                    Console.WriteLine("当前更新时间" + dt.ToString("yyyy年MM月dd日"));
                                    //List<OBTRealInfo> list = new List<OBTRealInfo>();
                                    //list = TransformHtml.GetObtRealInfoList(dt);
                                    UpLoadFtp("海上突发天气提示", dt, dTable,null);
                                }
                                else
                                {
                                    Console.WriteLine("暂没有新的数据，系统休息5分钟");
                                }
                                Thread.Sleep(5 * 60 * 1000);
                            }
                        }

                }



                // Console.Read();
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                CreatLogTxt.ErrWriter(e);
            }
        }

        /// <summary>
        /// 上传文件到FTP目录
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="list"></param>
        private static void UpLoadFtp(string type, DateTime dt, DataTable dtTable, List<WarningEntity> lws)
        {
            if (TransformHtml.WriteFile(type, dtTable,lws,out dt,out string sInfo))
            {
                Console.WriteLine("文件FSE_WA_" + dt.ToString("yyyyMMddHH") + ".html生成成功");

                if (TransformHtml.FtpUpload())
                {
                    Console.WriteLine("文件FSE_WA_" + dt.ToString("yyyyMMddHH") + ".html上传成功");
                    TransformHtml.UpdateNewTime(dt,type, sInfo);
                }
                else
                {
                    Console.WriteLine("上传失败");
                }
            }
        }
    }
}
