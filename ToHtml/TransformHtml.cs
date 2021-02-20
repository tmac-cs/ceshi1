using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Net;

namespace ToHtml
{
    public class TransformHtml
    {
        // static string wordPath = @"\\10.153.16.178\EastWestSea\";
        static List<string> PlatformName = new List<string> { "分海区预报", "南澳海域预报", "东西部海域预报" };
        //static string wordPath = @"\\10.153.16.178\EastWestSea\";
        static string ftpServerIP = @"ftp://172.22.1.17:21/";
        static string ftpUserID = "gmcrsz";
        static string ftpPassword = "123456";
        //static string ftpFilepPath = "/nas/begz/rcv/ocean/";
        static string ftpFilepPath = "/begz/rcv/ocean/";
        static string htmlfilename = "";
        static bool bThunder = false; //是否发布雷电预警
        static bool bWind = false; //是否发布大风预警
        static bool bRain = false; //是否发布暴雨预警
        static bool bFog = false; //是否发布大雾预警
        public TransformHtml()
        { }
        static Db_Class dbClass = new Db_Class();
        public static bool WriteFile(string type, DataTable dtTable, List<WarningEntity> lsw ,out DateTime dt,out string sInfo)
        {
            sInfo = "";
            dt = DateTime.Now;
            try
            {
                //if (dtTable == null || dtTable.Rows.Count < 1)
                //{
                //    return false;
                //}

                //   string path = HttpContext.Current.Server.MapPath("news/");
                Encoding code = Encoding.GetEncoding("gb2312");
                // 读取模板文件
                string temp = System.AppDomain.CurrentDomain.BaseDirectory + "/Wrod/HTMLPage1.html";
                StreamReader sr = null;
                StreamWriter sw = null;
                string str = "";

                sr = new StreamReader(temp, code);
                str = sr.ReadToEnd(); // 读取文件

                //  预报员：$foreastor$
                //签发人：$issuer$
                //  $date$
                //  $info$
                //   防御指引：1、$showOne$<br />2、$showTwo$

                string sShow = "";
               
                string sInfoTemp = "";
               

                if (type == "大风提示")
                {
                    dt = Convert.ToDateTime(dtTable.Rows[0]["DDateTime"]);
                    //sShow = "1．做好防风准备，注意了解大风最新消息；<br />2．请涉水旅游项目、水上交通的船舶和人员采取必要防御措施，确保安全。";
                    sInfo = dtTable.Rows[0]["contents"].ToString();
                    sInfoTemp = sInfo.Split('。')[0].ToString();
                    if (sInfoTemp != "")
                    {
                        sInfo = sInfoTemp;
                    }
                    sInfo += "。";
                }
                else if (type == "海上突发天气提示")
                {
                    if (dtTable.Rows[0]["DDateTime"].ToString() != "")
                    {
                        dt = Convert.ToDateTime(dtTable.Rows[0]["DDateTime"]);

                    }
                    //sShow = "1．请涉水旅游项目、水上交通的船舶和人员采取必要防御措施，确保安全。<br />2．请密切留意我台最新预报警报信息。";
                    sInfo = dtTable.Rows[0]["contentstest"].ToString();
                    //sInfoTemp = sInfo.Split('。')[0].ToString();
                    //if (sInfoTemp != "")
                    //{
                    //    sInfo = sInfoTemp;
                    //}
                    //sInfo += "。";
                }
                else if (type == "预警")
                {
                    string sWindArea = "";
                    string sRainArea = "";
                    string sThunderArea = "";
                    string sFogArea = "";
                    string sWindColor = "";
                    bThunder = false;
                    bWind = false;
                    bFog = false;
                    bRain = false;
                    {
                        //List<WarningEntity> lsw = TransformHtml.GetWarningds("");

                        // List<WarningEntity> lWarning = lWarning.Where(a=>a.Area.Contains("海区")).ToList();
                        //foreach (var strs in str)
                        //{
                        //判断预警信息类型是否包含暴雨、台风信息
                        //if (lsw[i].Type.Equals(strs) && yjtext_title_content.Contains(strs))
                        for (int i = 0; i < lsw.Count(); i++)
                        {
                            if (lsw[i].Area.Contains("海区"))
                            {
                                dt = lsw[0].Time;
                                if (lsw[i].Type.Trim().Contains("雷电"))
                                {
                                    bThunder = true;
                                    sThunderArea = GetWaringArea(lsw[i].Area);
                                }
                                else if (lsw[i].Type.Trim().Contains("大风"))
                                {
                                    bWind = true;
                                    sWindArea = GetWaringArea(lsw[i].Area);
                                    sWindColor = lsw[i].Type + lsw[i].Level;
                                }
                                else if (lsw[i].Type.Trim().Contains("大雾"))
                                {
                                    bFog = true;
                                    sFogArea = GetWaringArea(lsw[i].Area);
                                }
                                else if (lsw[i].Type.Trim().Contains("暴雨"))
                                {
                                    bRain = true;
                                    sRainArea = GetWaringArea(lsw[i].Area);
                                }
                            }
                        }
                    }
                    if (bRain)
                    {
                        sInfo = sRainArea + "有暴雨";
                        if (bWind)
                        {
                            sInfo += "，且风力较大，" + DefenseShow(sWindColor);
                            if (bThunder)
                            {
                                sInfo += "，并伴有雷暴";
                            }
                        }
                        else if (bThunder)
                        {
                            sInfo += "，并伴有雷暴";
                        }
                    }
                    else if (bWind)
                    {
                        sInfo = sWindArea + "风力较大，" + DefenseShow(sWindColor);
                        if (bThunder)
                        {
                            sInfo += "，并伴有雷暴";
                        }
                    }
                    else if (bThunder)
                    {
                        sInfo = sThunderArea + "有雷暴";
                    }
                    else if (bFog)
                    {
                        sInfo = sFogArea + "能见度较低";
                    }
                    else
                    {
                        return false;
                    }
                    sInfo += "。";

                }
                //else if (type == "大风预警")
                //{
                //    dt = Convert.ToDateTime(dtTable.Rows[0]["recdate"]);
                //    sInfo = "我市" + dtTable.Rows[0]["area"] + "风力较大，" + DefenseShow(dtTable.Rows[0]["colortype"].ToString());
                //    if (bThunder)
                //    {
                //        sInfo += "，并伴有雷暴。";
                //    }
                //    else
                //    {
                //        sInfo += "。";
                //    }
                //}
                else
                {

                }
                string sForeastor = "王蕊";
                string sIssuer = "王明洁";
                sForeastor = GetDutyNamePHONE(dt);
                sIssuer = GetSXPHONE(dt);
                sShow = "1．请涉水旅游项目、水上交通的船舶和人员采取必要防御措施，确保安全。<br />2．请密切留意我台最新预报警报信息。";
                htmlfilename = "FSE_WA_" + dt.ToString("yyyyMMddHH") + ".HTML";
                str = str.Replace("$foreastor$", sForeastor);
                str = str.Replace("$issuer$", sIssuer);
                str = str.Replace("$date$", dt.ToString("yyyy") + "年" + dt.ToString("MM") + "月" + dt.ToString("dd") + "日" + dt.ToString("HH") + "时");
                str = str.Replace("$info$", sInfo);
                str = str.Replace("$show$", sShow);
                // 写文件
                try
                {
                    //if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "/Wrod/" + htmlfilename))
                    if(SelectFinishTime(dt, type, sInfo))
                    {
                        Console.WriteLine("类别：" + type + "已生成！时间：" + dt + "内容：" + sInfo + "名称：" + htmlfilename);
                        return false;
                    }
                    sw = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "/Wrod/" + htmlfilename, false, code);
                    sw.Write(str);
                    sw.Flush();
                    sw.Close();
                    //  File.Copy(System.AppDomain.CurrentDomain.BaseDirectory + htmlfilename, wordPath + htmlfilename, true);
                }
                catch (Exception e)
                {
                    CreatLogTxt.ErrWriter(e);
                }
                finally
                {

                }
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }

            return true;
        }

        private static string GetWaringArea(string sArea)
        {
            string sWaringArea = "";
            if (sArea.Contains("东部"))
            {
                sWaringArea = "我市东部海区";
                if (sArea.Contains("西部"))
                {
                    sWaringArea = "我市东、西部海区";
                }
            }
            else if (sArea.Contains("西部"))
            {
                sWaringArea = "我市西部海区";
            }

            return sWaringArea;
        }

        private static string DefenseShow(string level)
        {
            try
            {
                string str = "";
                //if (type =="大风预警")
                //{
                if (level == "大风蓝色")
                {
                    str = "阵风达7-8级";
                }
                else if (level == "大风黄色")
                {
                    str = "阵风达9-10级";
                }
                else if (level == "大风橙色")
                {
                    str = "阵风达11-12级";
                }
                else if (level == "大风红色")
                {
                    str = "阵风达12级以上";
                }
                //}
                return str;
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
            return "";


        }
        private static string DefenseShow_old(string level)
        {
            try
            {
                /*
                 大风蓝色
大风黄色
大风橙色
大风红色

                 */
                string str = "";
                //if (type =="大风预警")
                //{
                if (level == "大风蓝色")
                {
                    str = "1．做好防风准备，注意了解大风最新消息；<br />2．请涉水旅游项目、水上交通的船舶和人员采取必要防御措施，确保安全。";
                }
                else if (level == "大风黄色")
                {
                    str = "1．停止水上户外作业，危险地带人员撤离；<br />2．船舶采取有效措施避风。";
                }
                else if (level == "大风橙色")
                {
                    str = "1．港口、码头等单位应当采取措施，注意防风；<br />2．请密切留意我台预报预警信息";
                }
                else if (level == "大风红色")
                {
                    str = "1．进入特别紧急防御状态，涉水旅游项目关闭、水上交通的船舶和人员回港；<br />2．各单位准备启动抢险应急方案。";
                }
                //}
                return str;
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
            return "";


        }
        public static List<OBTRealInfo> GetObtRealInfoList(DateTime dt)
        {
            List<OBTRealInfo> newList = new List<OBTRealInfo>();
            try
            {
                //查询未来某五天的预报数据
                //string strSQL = "select * from t_ocean_eastwestsea where ddatetime = to_date('" + dt.Date.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss')"
                //     + " and ISBUILDWORD = 0 order by ForeCastTime";
                //测试
                //strSQL = "select * from t_ocean_eastwestsea where ddatetime = to_date('" + dt.Date.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss')"
                //     + " and ISBUILDWORD = 1 order by ForeCastTime";


                string strSQL = @"select contents,DDATETIME from LWS_TRIPSMESSAGE t 
where DDATETIME  in (select max(DDATETIME)
from LWS_TRIPSMESSAGE where typename like '%大风%'and ddatetime >= sysdate - numtodsinterval(20000, 'minute'))";
                DataTable dTable = dbClass.Db_CreateDataTable(strSQL);
                if (dTable != null && dTable.Rows.Count > 0)
                {
                    #region  预报数据

                    DateTime dtF = dt;
                    double d0 = 0;
                    OBTRealInfo info;

                    for (int i = 0; i < dTable.Rows.Count; i++)
                    {
                        info = new OBTRealInfo();
                        info.ForecastDate = dTable.Rows[i]["ForeCastTime"] != DBNull.Value ? Convert.ToDateTime(dTable.Rows[i]["ForeCastTime"]).Date : dt.Date;

                        dtF = dt;
                        DateTime.TryParse(dTable.Rows[i]["ddatetime"].ToString(), out dtF);
                        info.DDateTime = dtF;
                        info.AreaName = dTable.Rows[i]["AREANAME"].ToString();

                        info.WindName = dTable.Rows[i]["WINDSPEED"].ToString();
                        info.WindGust = dTable.Rows[i]["WINDGUST"].ToString();

                        info.WeatherStatus = dTable.Rows[i]["WEATHERSTATUS"].ToString();
                        info.WindDirectName = dTable.Rows[i]["WINDDIRECT"].ToString();
                        //////////////////////////////////
                        //d0 = 0;
                        //double.TryParse(dTable.Rows[i]["WAVEHEIGHT"].ToString(), out d0);
                        //info.WaveHeight = d0;

                        d0 = 0;
                        double.TryParse(dTable.Rows[i]["VISI"].ToString(), out d0);
                        info.Visi = d0;
                        d0 = 0;
                        double.TryParse(dTable.Rows[i]["MINIVISI"].ToString(), out d0);
                        info.MiniVisi = d0;

                        //info.WaveLevel = dTable.Rows[i]["WAVELEVEL"].ToString();
                        newList.Add(info);
                    }

                }
                #endregion
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
            return newList;
        }

        /// <summary>
        /// 获取预报员信息
        /// </summary>
        /// <returns></returns>
        public static string GetDutyNamePHONE(DateTime dt)
        {
            //string strSQL = "select b.u_mobile from ybzx.t_waclasslist a,ybzx.t_USERS b where a.bstr3 = b.u_cname and a.ddatetime = to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-mm-dd')";
            string strSQL = @"select distinct t7.cname
from (select * from jnyb.schd_schedule t where t.expired = '0' and to_char(t.first_date, 'yyyy-MM') = to_char(sysdate,'yyyy-MM')) t1 
left join jnyb.schd_daily_schedule t2 on t1.id = t2.schedule_id
left join jnyb.schd_schedule_detail t3 on t2.id = t3.daily_schedule_id
left join jnyb.schd_detail_staff t4 on t4.detail_id = t3.id
left join jnyb.schd_shift t5 on t3.shift_id = t5.id
left join jnyb.schd_staff t6 on t4.staff_id = t6.id
left join jnyb.l_users t7 on t6.user_id = t7.luid 
where t2.schedule_date =  to_date('" + dt.ToString("yyyy-MM-dd") + @"','yyyy-mm-dd') and
t5.name = '夜班'";
            DateTime dt1 = new DateTime(dt.Year, dt.Month, dt.Day, 8, 00, 0);
            DateTime dt2 = new DateTime(dt.Year, dt.Month, dt.Day, 16, 30, 0);
            //夜间领班
            if (dt < dt1)
            {
                strSQL = @"select distinct t7.cname
from (select * from jnyb.schd_schedule t where t.expired = '0' and to_char(t.first_date, 'yyyy-MM') = to_char(sysdate,'yyyy-MM')) t1 
left join jnyb.schd_daily_schedule t2 on t1.id = t2.schedule_id
left join jnyb.schd_schedule_detail t3 on t2.id = t3.daily_schedule_id
left join jnyb.schd_detail_staff t4 on t4.detail_id = t3.id
left join jnyb.schd_shift t5 on t3.shift_id = t5.id
left join jnyb.schd_staff t6 on t4.staff_id = t6.id
left join jnyb.l_users t7 on t6.user_id = t7.luid 
where t2.schedule_date =  to_date('" + dt.AddDays(-1).ToString("yyyy-MM-dd") + @"','yyyy-mm-dd') and 
t5.name = '夜班'";
            }
            //白天领班
            if (dt >= dt1 && dt < dt2)
            {
                strSQL = @"select distinct t7.cname
from (select * from jnyb.schd_schedule t where t.expired = '0' and to_char(t.first_date, 'yyyy-MM') = to_char(sysdate,'yyyy-MM')) t1 
left join jnyb.schd_daily_schedule t2 on t1.id = t2.schedule_id
left join jnyb.schd_schedule_detail t3 on t2.id = t3.daily_schedule_id
left join jnyb.schd_detail_staff t4 on t4.detail_id = t3.id
left join jnyb.schd_shift t5 on t3.shift_id = t5.id
left join jnyb.schd_staff t6 on t4.staff_id = t6.id
left join jnyb.l_users t7 on t6.user_id = t7.luid 
where t2.schedule_date =  to_date('" + dt.ToString("yyyy-MM-dd") + @"','yyyy-mm-dd') and
t5.name = '领班'";
            }
            try
            {

                //Db_Class db = new Db_Class("SZQX1szym");
                Db_Class db = new Db_Class("EJETDB247IDCTY");
                return db.Db_Executequery(strSQL);

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                CreatLogTxt.ErrWriter(e);
            }
            return null;
        }

        /// <summary>
        /// 获取首席信息
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetSXPHONE(DateTime dt)
        {
            string strSQL = "";


            //    strSQL = "select b.u_mobile from ybzx.t_waclasslist a,ybzx.t_USERS b where a.bstr3 = b.u_cname and a.ddatetime = to_date('" + dt.AddDays(-1).ToString("yyyy-MM-dd") + "','yyyy-mm-dd')";
            //当天首席

            strSQL = @"select distinct t7.cname
from (select * from jnyb.schd_schedule t where t.expired = '0' and to_char(t.first_date, 'yyyy-MM') = to_char(sysdate,'yyyy-MM')) t1 
left join jnyb.schd_daily_schedule t2 on t1.id = t2.schedule_id
left join jnyb.schd_schedule_detail t3 on t2.id = t3.daily_schedule_id
left join jnyb.schd_detail_staff t4 on t4.detail_id = t3.id
left join jnyb.schd_shift t5 on t3.shift_id = t5.id
left join jnyb.schd_staff t6 on t4.staff_id = t6.id
left join jnyb.l_users t7 on t6.user_id = t7.luid 
where t2.schedule_date =  to_date('" + dt.ToString("yyyy-MM-dd") + @"','yyyy-mm-dd') and
t5.name = '首席'";

            DateTime dt1 = new DateTime(dt.Year, dt.Month, dt.Day, 8, 00, 0);
            //DateTime dt2 = new DateTime(dt.Year, dt.Month, dt.Day, 17, 00, 0);
            //前一天首席
            if (dt < dt1)
            {
                strSQL = @"select distinct t7.cname
from (select * from jnyb.schd_schedule t where t.expired = '0' and to_char(t.first_date, 'yyyy-MM') = to_char(sysdate,'yyyy-MM')) t1 
left join jnyb.schd_daily_schedule t2 on t1.id = t2.schedule_id
left join jnyb.schd_schedule_detail t3 on t2.id = t3.daily_schedule_id
left join jnyb.schd_detail_staff t4 on t4.detail_id = t3.id
left join jnyb.schd_shift t5 on t3.shift_id = t5.id
left join jnyb.schd_staff t6 on t4.staff_id = t6.id
left join jnyb.l_users t7 on t6.user_id = t7.luid 
where t2.schedule_date =  to_date('" + dt.AddDays(-1).ToString("yyyy-MM-dd") + @"','yyyy-mm-dd') and
t5.name = '首席'";

            }

            try
            {

                //Db_Class db = new Db_Class("SZQX1szym");
                Db_Class db = new Db_Class("EJETDB247IDCTY");
                return db.Db_Executequery(strSQL);

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                CreatLogTxt.ErrWriter(ex);
            }
            return null;
        }

        public static DataTable GetForester()
        {

            DataTable dTable = null;
            try
            {
                string strSQL = @"select distinct t7.cname,t5.name
from (select * from jnyb.schd_schedule t where t.expired = '0' and to_char(t.first_date, 'yyyy-MM') = to_char(sysdate,'yyyy-MM')) t1 
left join jnyb.schd_daily_schedule t2 on t1.id = t2.schedule_id
left join jnyb.schd_schedule_detail t3 on t2.id = t3.daily_schedule_id
left join jnyb.schd_detail_staff t4 on t4.detail_id = t3.id
left join jnyb.schd_shift t5 on t3.shift_id = t5.id
left join jnyb.schd_staff t6 on t4.staff_id = t6.id
left join jnyb.l_users t7 on t6.user_id = t7.luid 
where t2.schedule_date = to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + @"','yyyy-mm-dd') and
(t5.name = '服务' or t5.name = '首席')  order by t5.name";
                //测试
                //strSQL = "select max(ddatetime) from t_ocean_eastwestsea where ISBUILDWORD =1 and ddatetime > sysdate-2";
                Db_Class dbClass = new Db_Class("EJETDB247IDCTY");
                dTable = dbClass.Db_CreateDataTable(strSQL);
                return dTable;

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                CreatLogTxt.ErrWriter(e);
            }
            return null;
        }

        public static DataTable GetTips()
        {
            DateTime dt = DateTime.Parse("2000-01-01 00:00:00");
            DataTable dTable = null;
            try
            {
                string strSQL = @"select contents,DDATETIME from LWS_TRIPSMESSAGE t 
where DDATETIME  in (select max(DDATETIME)
from LWS_TRIPSMESSAGE where typename like '%大风%'and ddatetime >= sysdate - numtodsinterval(20, 'minute'))";
                //测试
                //strSQL = "select max(ddatetime) from t_ocean_eastwestsea where ISBUILDWORD =1 and ddatetime > sysdate-2";
                Db_Class dbClass = new Db_Class("EJETDB247Nowgis2010");
                dTable = dbClass.Db_CreateDataTable(strSQL);
                return dTable;
                //if (dTable != null && dTable.Rows.Count > 0)
                //{
                //    dt = DateTime.Parse(dTable);
                //}
                //else
                //{
                //    dt = DateTime.Parse("2000-01-01 00:00:00");
                //}
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                CreatLogTxt.ErrWriter(e);
            }
            return null;
        }


        public static List<WarningEntity> GetWarningds(string signalName)
        {
            DateTime dt = DateTime.Parse("2000-01-01 00:00:00");
            DataTable dTable = null;
            bThunder = false;
            List<WarningEntity> lsw = new List<WarningEntity>();
            //string signalName = "大风";
            try
            {
                string strSQL = @"select SIGNALTXT,recdate,forecast colortype,INGNALNUM,underwrite area,canceltxt signalLevel from
(select SIGNALTXT,PICTURE,INGNALNUM,recdate,RECID,forecast,canceltxt,underwrite from gdfpicture t 
where recdate > sysdate -numtodsinterval(20,'minute') order by t.recid desc ) where rownum<2";

                //测试  82145（大凤黄色、雷电），82205（暴雨黄色、雷电）
                //strSQL = "select SIGNALTXT,recdate,forecast colortype,INGNALNUM,underwrite area,canceltxt signalLevel from gdfpicture where recid = 82125";
                Db_Class dbClass = new Db_Class("EJETDB247IDCTY");
                dTable = dbClass.Db_CreateDataTable(strSQL);
                bool flag = false;
                if (dTable == null) return null;
                Console.WriteLine("dTable,获取到了预警为空");
                if (dTable.Rows.Count > 0)
                {
                    int wflag = 0;
                    int.TryParse(dTable.Rows[0]["INGNALNUM"].ToString(), out wflag);

                    string yjtext_title_content = string.Empty;
                    lsw = GetwarningInfo(out yjtext_title_content, dTable.Rows[0]["SIGNALTXT"].ToString());

                    //当前正在生效的预警
                    //if (!wflag.Equals(0))
                    //{
                    //    //foreach (var item in lsw)
                    //    for (int i = 0; i < lsw.Count(); i++)
                    //    {
                    //        //foreach (var strs in str)
                    //        //{
                    //        //判断预警信息类型是否包含暴雨、台风信息
                    //        //if (lsw[i].Type.Equals(strs) && yjtext_title_content.Contains(strs))
                    //        if (lsw[i].Area.Contains("海区"))
                    //        {
                    //            if (lsw[i].Type.Trim().Contains("雷电"))
                    //            {
                    //                bThunder = true;
                    //            }
                    //            else if (lsw[i].Type.Trim().Contains("大风"))
                    //            {
                    //                bWind = true;
                    //            }
                    //            else if (lsw[i].Type.Trim().Contains("大雾"))
                    //            {
                    //                bFog = true;
                    //            }
                    //            else if (lsw[i].Type.Trim().Contains("暴雨"))
                    //            {
                    //                bRain = true;
                    //            }
                    //        }
                    //        if ((lsw[i].Type.Trim() + lsw[i].Level.Trim()).Contains(signalName.Trim()))
                    //        {
                    //            if (lsw[i].Area.Contains("海区"))
                    //            {
                    //                string sArea = "";
                    //                if (lsw[i].Area.Contains("东部"))
                    //                {
                    //                    sArea = "我市东部海区";
                    //                    if (lsw[i].Area.Contains("西部"))
                    //                    {
                    //                        sArea = "我市东、西部海区";
                    //                    }
                    //                }
                    //                else if (lsw[i].Area.Contains("西部"))
                    //                {
                    //                    sArea = "我市西部海区";
                    //                }
                    //                dTable.Rows[0]["colortype"] = lsw[i].Type + lsw[i].Level;
                    //                dTable.Rows[0]["area"] = sArea;
                    //                dTable.Rows[0]["signalLevel"] = yjtext_title_content;
                    //                //如果当前暴雨、台风预警信号正在生效，则不需要外呼大雨预警
                    //                flag = true;
                    //                //return lsw[i];
                    //                //break;
                    //            }
                    //        }
                    //        //}
                    //    }
                    //}
                }
                if (flag == false)
                {
                    dTable = null;
                }

                return lsw;
                //if (dTable != null && dTable.Rows.Count > 0)
                //{
                //    dt = DateTime.Parse(dTable);
                //}
                //else
                //{
                //    dt = DateTime.Parse("2000-01-01 00:00:00");
                //}
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                CreatLogTxt.ErrWriter(e);
            }
            return null;
        }

        public static DataTable GetSevereWeatherAlarm()
        {
            DateTime dt = DateTime.Parse("2000-01-01 00:00:00");
            DataTable dTable = null;
            //string signalName = "大风";
            try
            {
                string strSQL = @"select * from (select * from LWS_SevereWeatherAlarm t 
where ddatetime > sysdate -numtodsinterval(20,'minute') and isSend = 0  order by writetime desc) where rownum<2";
                Db_Class dbClass = new Db_Class("EJETDB247Nowgis2010");
                dTable = dbClass.Db_CreateDataTable(strSQL);
                return dTable;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                CreatLogTxt.ErrWriter(e);
            }
            return null;
        }

        public static bool UpdateNewTime(DateTime dt,string typeName,string sInfo)
        {
            try
            {
                //string strSQL = "update t_ocean_eastwestsea set ISBUILDWORD = 1 where ddatetime = to_date('" + dt.ToString("yyyy-MM-dd") + "','yyyy-mm-dd') ";
                //bool nRestul = dbClass.Db_ExecuteNonquery(strSQL);
                //string recid = "";
                //string sql = "select max(recid) from t_ocean_allstatus where ddatetime=to_date('" + dt.ToString("yyyy-MM-dd 00:00:00") + "','yyyy-mm-dd hh24:mi:ss') and areaname = '" + PlatformName[2] + "'";
                //recid = dbClass.Db_Executequery(sql);
                //if (!string.IsNullOrEmpty(recid))
                //{
                //    sql = "update t_ocean_allstatus set sentFlag= 1, UPDATETIME =to_date('" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "', 'dd-mm-yyyy hh24:mi:ss')  where recid=" + recid;
                //    dbClass.Db_ExecuteNonquery(sql);
                //}
                //return nRestul;
                string strSQL = "select recid from  LWS_SEVEREWEATHERALARM where ddatetime = to_date('" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss') ";
                Db_Class dbClass = new Db_Class("EJETDB247Nowgis2010");
                string sRestul = dbClass.Db_Executequery(strSQL);
                bool nRestul = false;
                if (sRestul.Trim() == "")
                {
                    strSQL = "insert into LWS_SevereWeatherAlarm (RECID,TYPENAME,CONTENTStest,FORECASTER,DDATETIME,WRITETIME,issend"
                   + " ) values (SEQ_LWS_SevereWeatherAlarm.Nextval ,'" + typeName + "'"
                      + ",'" + sInfo + "'"
                      + ",''"
                      + ",to_date('" + dt.ToString("yyyy-MM-dd HH:mm") + "','yyyy-mm-dd hh24:mi')"
                      + ",to_date('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss')"
                      + ",1) ";
                    dbClass = new Db_Class("EJETDB247Nowgis2010");
                    nRestul = dbClass.Db_ExecuteNonquery(strSQL);
                }
                else
                {
                    strSQL = "update LWS_SEVEREWEATHERALARM set issend = 1,TYPENAME ='" + typeName + "',CONTENTStest='" + sInfo + "' where ddatetime = to_date('" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss') ";
                    dbClass = new Db_Class("EJETDB247Nowgis2010");
                    nRestul = dbClass.Db_ExecuteNonquery(strSQL);
                }
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
            return false;
        }

        /// <summary>
        /// 查询是否已经制作过
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="typeName"></param>
        /// <param name="sInfo"></param>
        /// <returns></returns>
        public static bool SelectFinishTime(DateTime dt, string typeName, string sInfo)
        {
            try
            {
                string strSQL = "select recid from  LWS_SEVEREWEATHERALARM where ddatetime = to_date('" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-mm-dd hh24:mi:ss') " +
                    " and typename = '"+ typeName+ "' and CONTENTStest ='" + sInfo + "'  and issend = 1";
                Db_Class dbClass = new Db_Class("EJETDB247Nowgis2010");
                string sRestul = dbClass.Db_Executequery(strSQL);

                if (sRestul.Trim() != "")
                {
                    return true;
                }
                   
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
            return false;
        }
        //截取预警信息
        public static List<WarningEntity> GetwarningInfo(out string strinfo, string signalstrs)
        {
            List<WarningEntity> ls = new List<WarningEntity>();
            strinfo = "";
            try
            {

                string signalstr = signalstrs;
                WarningEntity info = new WarningEntity();
                //0:信号 ,1:短信
                string[] signaltxt = null;
                //各信号
                string[] strtext = null;
                //信号内容
                string[] strType = null;

                if (signalstr.IndexOf(";.") != -1)
                {
                    //将短信与信号分离
                    signaltxt = signalstr.Split(new string[1] { ";." }, StringSplitOptions.RemoveEmptyEntries);

                    if (signaltxt[0].IndexOf(";") != -1)
                    {
                        strtext = signaltxt[0].Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                        if (strtext != null && strtext.Length > 0)
                        {
                            foreach (string strT in strtext)
                            {
                                strType = strT.Split(',');
                                if (strType != null && strType.Length > 3)
                                {
                                    info = new WarningEntity();
                                    info.Area = strType[0];
                                    info.Type = strType[1];
                                    info.Level = strType[2].Trim();
                                    info.Time = DateTime.Parse(strType[3]);
                                    ls.Add(info);
                                }
                            }
                        }
                    }
                    else
                    {
                        strType = signaltxt[0].Split(',');
                        if (strType != null && strType.Length > 3)
                        {
                            info = new WarningEntity();
                            info.Area = strType[0];
                            info.Type = strType[1];
                            info.Level = strType[2].Trim();
                            info.Time = DateTime.Parse(strType[3]);
                            ls.Add(info);
                        }
                    }

                }
                else
                {
                    signaltxt = new string[1];
                    signaltxt[0] = signalstr;
                }


                if (signaltxt.Length > 1)
                {

                    strinfo = signaltxt[signaltxt.Length - 1].ToString();
                }
                else
                {

                    strinfo = signaltxt[signaltxt.Length - 1].ToString();
                }



            }
            catch (Exception e)
            {

                CreatLogTxt.ErrWriter(e);
                string em = e.Message;
            }
            finally
            {

            }
            return ls;

        }

        //上传文件
        public static Boolean FtpUpload()
        {
            try
            {
                //检查目录是否存在，不存在创建
                //   FtpCheckDirectoryExist(ftpFilepPath);
                FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + "/Wrod/" + htmlfilename);
                FileStream fs = fi.OpenRead();
                long length = fs.Length;
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpServerIP + ftpFilepPath + fi.Name);
                req.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.ContentLength = length;
                req.Timeout = 10 * 1000;
                try
                {
                    Stream stream = req.GetRequestStream();
                    int BufferLength = 2048; //2K   
                    byte[] b = new byte[BufferLength];
                    int i;
                    while ((i = fs.Read(b, 0, BufferLength)) > 0)
                    {
                        stream.Write(b, 0, i);
                    }
                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception e)
                {
                    CreatLogTxt.ErrWriter(e);
                    return false;
                }
                finally
                {
                    fs.Close();
                    req.Abort();
                }
                req.Abort();
                return true;
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
            return false;
        }

        //判断文件的目录是否存,不存则创建
        public static void FtpCheckDirectoryExist(string destFilePath)
        {
            string fullDir = FtpParseDirectory(destFilePath);
            string[] dirs = fullDir.Split('/');
            string curDir = "/";
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空  
                if (dir != null && dir.Length > 0)
                {
                    try
                    {
                        curDir += dir + "/";
                        FtpMakeDir(curDir);
                    }
                    catch (Exception e)
                    {
                        CreatLogTxt.ErrWriter(e);
                    }
                }
            }
        }

        public static string FtpParseDirectory(string destFilePath)
        {
            return destFilePath.Substring(0, destFilePath.LastIndexOf("/"));
        }

        //创建目录
        public static Boolean FtpMakeDir(string localFile)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpServerIP + localFile);
            req.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception)
            {
                req.Abort();
                return false;
            }
            req.Abort();
            return true;
        }

        public static void Download(string fileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                FileStream outputStream = new FileStream("F:\\test\\ftp\\" + fileName, FileMode.Create);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpServerIP + ftpFilepPath + fileName));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                CreatLogTxt.ErrWriter(e);
            }
        }
    }

    public class OBTRealInfo
    {
        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string areaname;
        public string AreaName
        {
            get { return areaname; }
            set { areaname = value; }
        }

        private double maxtemp;

        public double MaxTemp
        {
            get { return maxtemp; }
            set { maxtemp = value; }
        }
        private double mintemp;

        public double MinTemp
        {
            get { return mintemp; }
            set { mintemp = value; }
        }
        private double temp;

        public double Temp
        {
            get { return temp; }
            set { temp = value; }
        }

        private double wind;

        public double Wind
        {
            get { return wind; }
            set { wind = value; }
        }

        private double wind1s;

        public double Wind1S
        {
            get { return wind1s; }
            set { wind1s = value; }
        }

        //风向
        private double wind1sdirect;

        public double Wind1SDirect
        {
            get { return wind1sdirect; }
            set { wind1sdirect = value; }
        }

        private double wind10m;

        public double Wind10M
        {
            get { return wind10m; }
            set { wind10m = value; }
        }

        //风向
        private double wind10mdirect;

        public double Wind10MDirect
        {
            get { return wind10mdirect; }
            set { wind10mdirect = value; }
        }

        private double wind1smax;

        public double Wind1SMax
        {
            get { return wind1smax; }
            set { wind1smax = value; }
        }

        //风向
        private double wind1smaxdirect;

        public double Wind1SMaxDirect
        {
            get { return wind1smaxdirect; }
            set { wind1smaxdirect = value; }
        }


        private double maxwinddirect;

        public double MaxWindDirect
        {
            get { return maxwinddirect; }
            set { maxwinddirect = value; }
        }

        private double maxwind;

        public double MaxWind
        {
            get { return maxwind; }
            set { maxwind = value; }
        }

        private double visi;
        /// <summary>
        /// 能见度
        /// </summary>

        public double Visi
        {
            get { return visi; }
            set { visi = value; }
        }

        private double minivisi;
        /// <summary>
        /// 能见度
        /// </summary>

        public double MiniVisi
        {
            get { return minivisi; }
            set { minivisi = value; }
        }

        //风向
        private string windGustDirectName;

        public string WindGustDirectName
        {
            get { return windGustDirectName; }
            set { windGustDirectName = value; }
        }
        //风向
        private double winddirect;

        public double WindDirect
        {
            get { return winddirect; }
            set { winddirect = value; }
        }
        //风向名称
        private string winddirectname;

        public string WindDirectName
        {
            get { return winddirectname; }
            set { winddirectname = value; }
        }
        //风级
        private string windname;

        public string WindName
        {
            get { return windname; }
            set { windname = value; }
        }
        //阵风
        private string windgust;

        public string WindGust
        {
            get { return windgust; }
            set { windgust = value; }
        }

        //湿度
        private double humidity;

        public double Humidity
        {
            get { return humidity; }
            set { humidity = value; }
        }
        //湿度
        private double maxhumidity;

        public double MaxHumidity
        {
            get { return maxhumidity; }
            set { maxhumidity = value; }
        }


        private DateTime ddatetime;

        public DateTime DDateTime
        {
            get { return ddatetime; }
            set { ddatetime = value; }
        }

        private DateTime forecastdate;

        public DateTime ForecastDate
        {
            get { return forecastdate; }
            set { forecastdate = value; }
        }
        private string obttablename;

        public string ObtTableName
        {
            get { return obttablename; }
            set { obttablename = value; }
        }


        string weatherPic;

        public string WeatherPic
        {
            get { return weatherPic; }
            set { weatherPic = value; }
        }

        string weatherStatus;

        public string WeatherStatus
        {
            get { return weatherStatus; }
            set { weatherStatus = value; }
        }

        double waveHeight;
        /// <summary>
        /// 浪高
        /// </summary>
        public double WaveHeight
        {
            get { return waveHeight; }
            set { waveHeight = value; }
        }

        private string wavelevel;
        public string WaveLevel
        {
            get { return wavelevel; }
            set { wavelevel = value; }
        }

        public OBTRealInfo()
        {
        }


    }

    public class WarningEntity
    {
        private DateTime _time;
        //发布时间
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }
        private string _area;
        //发布区域
        public string Area
        {
            get { return _area; }
            set { _area = value; }
        }
        private string _level;
        //发布级别
        public string Level
        {
            get { return _level; }
            set { _level = value; }
        }
        private string _type;
        //发布的类型
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}


