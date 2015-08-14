using MVC_WenJuan.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MVC_WenJuan.Controllers
{
    public class WenJuanController : Controller
    {
        //
        // GET: /WenJuan/
        public ActionResult Index()
        {
            ViewBag.Message = string.Empty;

            if (!string.IsNullOrEmpty(Request["BanShiChu"]))
            {
                UpdateWenJuanData(Request);
                ViewBag.Message = "提交成功";
                return View();
            }
            else
            {
                return View();
            }
        }

        public ActionResult WenJuanResult()
        {
            string keyWords = string.Empty;
            ViewBag.ErrorMsg = string.Empty;

            Dictionary<string, string> tongJiList = new Dictionary<string, string>();
            tongJiList.Add("YingXiang",string.Empty);
            tongJiList.Add("YiShiGuanNian",string.Empty);
            tongJiList.Add("ZhongDian",string.Empty);
            tongJiList.Add("LiuCheng",string.Empty);
            tongJiList.Add("XinXi",string.Empty);
            tongJiList.Add("BaoHe",string.Empty);
            tongJiList.Add("PeiYang",string.Empty);
            tongJiList.Add("TiSheng",string.Empty);
            tongJiList.Add("GangWeiWenTi",string.Empty);
            tongJiList.Add("FenPeiFnagAn",string.Empty);
            tongJiList.Add("JieYue",string.Empty);
            tongJiList.Add("XuQiu",string.Empty);
            tongJiList.Add("BuTongChang",string.Empty);
            tongJiList.Add("GangWeiHeLi",string.Empty);
            tongJiList.Add("LiuChengJianYi",string.Empty);
            tongJiList.Add("KeFuQueDian",string.Empty);
            tongJiList.Add("JiangBen",string.Empty);
            tongJiList.Add("JiaQiang",string.Empty);
            tongJiList.Add("KaoHe",string.Empty);
            tongJiList.Add("PeiYangBuHeLi",string.Empty);
            tongJiList.Add("KaoHeGaiJin",string.Empty);
            tongJiList.Add("GouTong",string.Empty);
            tongJiList.Add("RenLiGaiJin",string.Empty);
            tongJiList.Add("PeiXunWenTi",string.Empty);

            tongJiList.Add("YingXiang_Other",string.Empty);
            tongJiList.Add("YiShiGuanNian_Other",string.Empty);
            tongJiList.Add("TiSheng_Other",string.Empty);
            tongJiList.Add("JieYue_Other",string.Empty);
            tongJiList.Add("JianYiA",string.Empty);
            tongJiList.Add("JianYiB",string.Empty);
            tongJiList.Add("JianYiC",string.Empty);
            tongJiList.Add("JianYiD",string.Empty);

            ViewBag.TongJiData = tongJiList;
            ViewBag.TongJiList = "YingXiang$YiShiGuanNian$ZhongDian$LiuCheng$XinXi$BaoHe$PeiYang$TiSheng$GangWeiWenTi$FenPeiFnagAn$JieYue$XuQiu$BuTongChang$GangWeiHeLi"+
                                 "$LiuChengJianYi$KeFuQueDian$JiangBen$JiaQiang$KaoHe$PeiYangBuHeLi$KaoHeGaiJin$GouTong$RenLiGaiJin$PeiXunWenTi";

            if (!string.IsNullOrEmpty(Request["BanShiChu"]))
            {
                keyWords = Request["BanShiChu"];
            }
            else if (!string.IsNullOrEmpty(Request["Zone"]))
            {
                keyWords = Request["Zone"];
            }
            else
            {
                ViewBag.ErrorMsg = "请选择统计的区域或者办事处、分公司。";
                return View();
            }

            WenJuanContext wc = new WenJuanContext();
            int totalUser = wc.WenJuans.Where(u => keyWords.IndexOf("'" + u.BanShiChu + "'") != -1).Select(u => u.SubmitID).Distinct().Count();

            if (totalUser != 0)
            {
                GetTongJiData(totalUser, wc, tongJiList, keyWords);
            }
            else
            {
                ViewBag.ErrorMsg = "暂无符合的数据。";
                return View();
            }

            return View();
        }

        private void GetTongJiData(int totalUser, WenJuanContext wc, Dictionary<string, string> tongJiList, string keyWords)
        {
            string tjTotal = "YingXiang$YiShiGuanNian$ZhongDian$LiuCheng$XinXi$BaoHe$PeiYang$TiSheng$GangWeiWenTi$FenPeiFnagAn$JieYue$XuQiu$BuTongChang$GangWeiHeLi" +
                                "$LiuChengJianYi$KeFuQueDian$JiangBen$JiaQiang$KaoHe$PeiYangBuHeLi$KaoHeGaiJin$GouTong$RenLiGaiJin$PeiXunWenTi";

            string jyTotal = "YingXiang_Other$YiShiGuanNian_Other$TiSheng_Other$JieYue_Other$JianYiA$JianYiB$JianYiC$JianYiD";

            string[] jyList = jyTotal.Split('$');
            string[] tjList = tjTotal.Split('$');

            MySqlConnection dbcon = (MySqlConnection)wc.Database.Connection;
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = dbcon;
            cm.Connection.Open();

            foreach (string tj in tjList)
            {
                cm.CommandText = "select count(distinct SubmitID) from wenjuans where " + tj + " is not null and BanShiChu in (" + keyWords + ");";
                int userCheckCount = Convert.ToInt32(cm.ExecuteScalar());

                cm.CommandText = "select " + tj + ",count(BanShiChu) as checkCount from wenjuans where " + tj + " is not null and BanShiChu in (" + keyWords + ") group by " + tj + ";";
                MySqlDataReader rd = cm.ExecuteReader();
                
                string tempStr = string.Empty;
                while (rd.Read())
                {
                    string name = rd[tj].ToString().Substring(0, 1);
                    string val = rd["checkCount"].ToString();

                    double bl = Math.Round(Convert.ToDouble(val) / userCheckCount * 100, 2);

                    name += " " + bl.ToString() + "%";

                    if (tempStr == string.Empty)
                    {
                        tempStr = "{ 'name': '" + name + "', 'val': " + val + " }";
                    }
                    else
                    {
                        tempStr = tempStr + ",{ 'name': '" + name + "', 'val': " + val + " }";
                    }
                }
                if (tempStr != string.Empty)
                {
                    tempStr = "[" + tempStr + "]";
                    tongJiList[tj] = tempStr + "$" + "选择总数/问卷总数：" + userCheckCount.ToString() + "/" + totalUser.ToString() + "   选择率：" + Math.Round(Convert.ToDouble(userCheckCount) / totalUser * 100, 2).ToString() + "%";
                }

                rd.Close();
            }

            foreach (string jy in jyList)
            {
                cm.CommandText = "select count(BanShiChu) from wenjuans where " + jy + " is not null and " + jy + " <>'' and BanShiChu in (" + keyWords + ");";
                int userCheckCount = Convert.ToInt32(cm.ExecuteScalar());

                cm.CommandText = "select distinct " + jy + " from wenjuans where " + jy + " is not null and " + jy + " <>'' and BanShiChu in (" + keyWords + ");";
                MySqlDataReader rd = cm.ExecuteReader();

                string tempStr = string.Empty;
                while (rd.Read())
                {
                    if (tempStr == string.Empty)
                    {
                        tempStr = rd[jy].ToString();
                    }
                    else
                    {
                        tempStr = tempStr + "$" + rd[jy].ToString();
                    }
                }
                if (tempStr != string.Empty)
                {
                    tongJiList[jy] = tempStr + "&" + "填写总数/问卷总数：" + userCheckCount.ToString() + "/" + totalUser.ToString() + "   填写率：" + Math.Round(Convert.ToDouble(userCheckCount) / totalUser * 100, 2).ToString() + "%";
                }

                rd.Close();
            }

            cm.Connection.Close();

            //tongJiList["YingXiang"] = "[{ 'name': '中文', 'val': 20 }, { 'name': '英文', 'val': 20 }, { 'name': '泰文', 'val': 20 }, { 'name': '添文', 'val': 20 }, { 'name': '草书', 'val': 20 }]";
        }

        private void UpdateWenJuanData(HttpRequestBase Request)
        {
            Guid submitID = Guid.NewGuid(); 

            WenJuanContext wc = new WenJuanContext();

            WenJuan wj = new WenJuan();
            wj.SubmitID = submitID;
            wj.BanShiChu = Request["BanShiChu"];
            wj.YingXiang = Request["YingXiang"];
            wj.YiShiGuanNian = Request["YiShiGuanNian"];
            wj.YingXiang_Other = Request["YingXiang_Other"];
            wj.YiShiGuanNian = Request["YiShiGuanNian"];
            wj.YiShiGuanNian_Other = Request["YiShiGuanNian_Other"];
            wj.ZhongDian = Request["ZhongDian"];
            wj.LiuCheng = Request["LiuCheng"];
            wj.XinXi = Request["XinXi"];
            wj.BaoHe = Request["BaoHe"];
            wj.PeiYang = Request["PeiYang"];
            wj.TiSheng = Request["TiSheng"];
            wj.TiSheng_Other = Request["TiSheng_Other"];
            wj.GangWeiWenTi = Request["GangWeiWenTi"];
            wj.FenPeiFnagAn = Request["FenPeiFnagAn"];
            wj.JieYue = Request["JieYue"];
            wj.JieYue_Other = Request["JieYue_Other"];

            if (!string.IsNullOrEmpty(Request["XuQiu"]) && Request["XuQiu"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["XuQiu"].Split(',');
                wj.XuQiu = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.XuQiu = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.XuQiu = Request["XuQiu"];
            }

            if (!string.IsNullOrEmpty(Request["BuTongChang"]) && Request["BuTongChang"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["BuTongChang"].Split(',');
                wj.BuTongChang = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.BuTongChang = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.BuTongChang = Request["BuTongChang"];
            }

            if (!string.IsNullOrEmpty(Request["GangWeiHeLi"]) && Request["GangWeiHeLi"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["GangWeiHeLi"].Split(',');
                wj.GangWeiHeLi = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.GangWeiHeLi = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.GangWeiHeLi = Request["GangWeiHeLi"];
            }

            if (!string.IsNullOrEmpty(Request["LiuChengJianYi"]) && Request["LiuChengJianYi"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["LiuChengJianYi"].Split(',');
                wj.LiuChengJianYi = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.LiuChengJianYi = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.LiuChengJianYi = Request["LiuChengJianYi"];
            }

            if (!string.IsNullOrEmpty(Request["KeFuQueDian"]) && Request["KeFuQueDian"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["KeFuQueDian"].Split(',');
                wj.KeFuQueDian = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.KeFuQueDian = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.KeFuQueDian = Request["KeFuQueDian"];
            }

            if (!string.IsNullOrEmpty(Request["JiangBen"]) && Request["JiangBen"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["JiangBen"].Split(',');
                wj.JiangBen = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.JiangBen = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.JiangBen = Request["JiangBen"];
            }

            if (!string.IsNullOrEmpty(Request["JiaQiang"]) && Request["JiaQiang"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["JiaQiang"].Split(',');
                wj.JiaQiang = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.JiaQiang = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.JiaQiang = Request["JiaQiang"];
            }

            if (!string.IsNullOrEmpty(Request["KaoHe"]) && Request["KaoHe"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["KaoHe"].Split(',');
                wj.KaoHe = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.KaoHe = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.KaoHe = Request["KaoHe"];
            }

            if (!string.IsNullOrEmpty(Request["PeiYangBuHeLi"]) && Request["PeiYangBuHeLi"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["PeiYangBuHeLi"].Split(',');
                wj.PeiYangBuHeLi = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.PeiYangBuHeLi = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.PeiYangBuHeLi = Request["PeiYangBuHeLi"];
            }

            if (!string.IsNullOrEmpty(Request["KaoHeGaiJin"]) && Request["KaoHeGaiJin"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["KaoHeGaiJin"].Split(',');
                wj.KaoHeGaiJin = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.KaoHeGaiJin = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.KaoHeGaiJin = Request["KaoHeGaiJin"];
            }

            if (!string.IsNullOrEmpty(Request["GouTong"]) && Request["GouTong"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["GouTong"].Split(',');
                wj.GouTong = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.GouTong = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.GouTong = Request["GouTong"];
            }

            if (!string.IsNullOrEmpty(Request["RenLiGaiJin"]) && Request["RenLiGaiJin"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["RenLiGaiJin"].Split(',');
                wj.RenLiGaiJin = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.RenLiGaiJin = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.RenLiGaiJin = Request["RenLiGaiJin"];
            }

            if (!string.IsNullOrEmpty(Request["PeiXunWenTi"]) && Request["PeiXunWenTi"].IndexOf(',') != -1)
            {
                string[] cList1 = Request["PeiXunWenTi"].Split(',');
                wj.PeiXunWenTi = cList1[0];

                for (int i = 1; i < cList1.Length; i++)
                {
                    WenJuan wjTemp = new WenJuan();
                    wjTemp.SubmitID = submitID;
                    wjTemp.BanShiChu = Request["BanShiChu"];
                    wjTemp.PeiXunWenTi = cList1[i];
                    wc.WenJuans.Add(wjTemp);
                    wc.SaveChanges();
                }
            }
            else
            {
                wj.PeiXunWenTi = Request["PeiXunWenTi"];
            }

            wj.JianYiA = Request["JianYiA"];
            wj.JianYiB = Request["JianYiB"];
            wj.JianYiC = Request["JianYiC"];
            wj.JianYiD = Request["JianYiD"];

            wc.WenJuans.Add(wj);
            wc.SaveChanges();
        }

        //public ActionResult Login()
        //{
        //    string userName = Request["UserName"];
        //    string passWord = Request["PassWord"];

        //    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
        //    {
        //        ViewBag.IsLoginOk = string.Empty;
        //        return View();
        //    }
        //    else
        //    {
        //        WenJuanContext wc = new WenJuanContext();
                
        //        var userCount = wc.Users.Count(e => (e.Name.ToUpper() == userName.ToUpper() && e.LoginWord == passWord));

        //        if (userCount != 0)
        //        {
        //            Session["IsLogin"] = "OK";
        //            Session["UserName"] = userName;
        //            Session["PassWord"] = passWord;

        //            User user = wc.Users.Include("WenJuan.Subjects.Questions.Options").Where(e => (e.Name.ToUpper() == userName.ToUpper() && e.LoginWord == passWord)).First();
                    
        //            var wenjuan = user.WenJuan;
                   

        //            return View("WenJuan", wenjuan);
        //        }
        //        else
        //        {
        //            ViewBag.IsLoginOk = "NOT";
        //            return View();
        //        }
        //    }
        //}

        //public string CreateWenJuan(string[] dataList)
        //{
        //    WenJuanContext wc = new WenJuanContext();

        //    WenJuan wj = new WenJuan();
        //    Subject tempSJ = new Subject();
        //    Question tempQS = new Question();
        //    foreach (string dataLine in dataList)
        //    {
        //        string[] itemList = dataLine.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
        //        string firstItem = itemList[0].Trim();
        //        bool isFirstItemOk = true;

        //        if (!string.IsNullOrEmpty(firstItem))
        //        {
        //            switch (firstItem)
        //            {
        //                case "【START】":
        //                    break;
        //                case "【问卷标识】":
        //                    wj.WenJuanName = itemList[1].Trim();
        //                    break;
        //                case "【序言】":
        //                    wj.XuYan = itemList[1];
        //                    break;
        //                case "【主题】":
        //                    Subject sj = new Subject();
        //                    tempSJ = sj;
        //                    sj.SubjectName = itemList[1].Trim();
        //                    wj.Subjects.Add(sj);
        //                    break;
        //                case "【问题】":
        //                    Question qs = new Question();
        //                    tempQS = qs;
        //                    qs.QuestionContent = itemList[1].Trim();
        //                    tempSJ.Questions.Add(qs);
        //                    break;
        //                case "【选项】":
        //                    for (int i = 1; i < itemList.Count();i++)
        //                    {
        //                        string[] opList = itemList[i].Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
        //                        Option op = new Option();
        //                        op.OptionContent = opList[0];
        //                        op.OptionClass = opList[1];
        //                        if (opList[1] == "NUMBER")
        //                        {
        //                            op.Min = Convert.ToInt32(opList[2]);
        //                            op.Max = Convert.ToInt32(opList[3]);
        //                        }
        //                        tempQS.Options.Add(op);
        //                    }
        //                        break;
        //                case "【END】":
        //                    break;
        //                default:
        //                    isFirstItemOk = false;
        //                    break;
        //            }
        //        }

        //        if (!isFirstItemOk)
        //        {
        //            return "格式错误，请重新提交！！";
        //        }
        //    }

        //    wc.WenJuans.Add(wj);
        //    wc.SaveChanges();

        //    return "OK";
        //}
        //public ActionResult SubmitWenJuanTemplate()
        //{
        //    string wenJuanTemplateData = Request["wjdata"];
        //    if (!string.IsNullOrEmpty(wenJuanTemplateData))
        //    {
        //        if (wenJuanTemplateData.IndexOf("【START】") == -1 || wenJuanTemplateData.IndexOf("【问卷标识】") == -1 || wenJuanTemplateData.IndexOf("【序言】") == -1
        //            || wenJuanTemplateData.IndexOf("【主题】") == -1 || wenJuanTemplateData.IndexOf("【问题】") == -1 || wenJuanTemplateData.IndexOf("【选项】") == -1
        //            || wenJuanTemplateData.IndexOf("【END】") == -1)
        //        {
        //            ViewBag.SubmitResult = "格式错误，请重新提交！！";
        //        }
        //        else
        //        {
        //            string[] dataList = wenJuanTemplateData.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        //            ViewBag.SubmitResult = CreateWenJuan(dataList);
        //        }

        //        //FileStream fs = new FileStream("E:\\output.txt", FileMode.OpenOrCreate);
        //        //StreamWriter sw = new StreamWriter(fs);
        //        //foreach (var data in dataList)
        //        //{
        //        //    sw.Write(data + "\r\n");
        //        //}
        //        //sw.Flush();
        //        //sw.Close();
        //        //fs.Close();
                
        //    }
        //    else
        //    {
        //        ViewBag.SubmitResult = "请提交数据！！";
        //    }

        //    return View();
        //}

    }
}
