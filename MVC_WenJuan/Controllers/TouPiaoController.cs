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
    public class TouPiaoController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.FeebackMessage = string.Empty;

            Dictionary<string, object> dataList = new Dictionary<string, object>();

            TouPiaoContext tc = new TouPiaoContext();
            string userName = Request["UserName"];
            string companyName = Request["CompanyName"];
            string checkID = Request["checkID"];
            string remoteAddr = Request["REMOTE_ADDR"];
            string remoteHost = Request["REMOTE_HOST"];

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(companyName) && !string.IsNullOrEmpty(checkID))
            {
                User submitUser = tc.Users.Where(u => u.Name == userName && u.Company == companyName).FirstOrDefault();

                if (submitUser != null)
                {
                    int submitCount = tc.TouPiaos.Where(t => t.User.ID == submitUser.ID).Count();
                    if (submitCount != 0)
                    {

                        SystemName targetName = tc.TouPiaos.Where(t => t.User.ID == submitUser.ID).Select(t => t.SystemName).FirstOrDefault();
                        ViewBag.FeebackMessage = "本次投票失败，您之前已投票给【" + targetName.NameCN + "/" + targetName.NameEN + "】如有疑问，请联系管理员。";
                    }
                    else
                    {
                        Guid guid = Guid.Parse(checkID);

                        TouPiao tempTouPiao = new TouPiao();
                        tempTouPiao.User = submitUser;
                        tempTouPiao.SystemName = tc.SystemNames.Where(s => s.ID == guid).First();
                        tempTouPiao.SubmitIP = remoteAddr;
                        tempTouPiao.SubmitMAC = remoteHost;
                        tempTouPiao.SubmitTime = DateTime.Now;
                        tc.TouPiaos.Add(tempTouPiao);
                        tc.SaveChanges();

                        ViewBag.FeebackMessage = "投票成功";
                    }
                }
                else
                {
                    ViewBag.FeebackMessage = "投票名单不存在：" + userName + " 联系管理员处理。";
                }
            }

            dataList["NameList"] = tc.Users.OrderByDescending(k => k.Name).Select(u => u.Name).Distinct().ToList();
            ((List<string>)dataList["NameList"]).Sort();
            dataList["CompanyList"] = tc.Users.OrderByDescending(u => u.Company).Select(u => u.Company).Distinct().ToList();
            ((List<string>)dataList["CompanyList"]).Sort();
            dataList["SystemName"] = tc.SystemNames.OrderBy(u => u.SubmitTime).ToList();

            return View(dataList);
        }

        public string GetUserNames()
        {
            string strForReturn = string.Empty;

            string cn = Request["CompanyName"];
            if (!string.IsNullOrEmpty(cn))
            {
                TouPiaoContext tc = new TouPiaoContext();
                var users = tc.Users.Where(u => u.Company == cn).OrderBy(k=>k.Name).ToList();
                foreach (var u in users)
                {
                    if (strForReturn == string.Empty)
                    {
                        strForReturn = "{'name':" + "'" + u.Name + "'}";
                    }
                    else
                    {
                        strForReturn = strForReturn + "," + "{'name':" + "'" + u.Name + "'}";
                    }
                }
                if (strForReturn != string.Empty)
                {
                    strForReturn = "[" + strForReturn + "]";
                }
            }
            return strForReturn;
        }

        public ActionResult ImportSystemName()
        {
            string exportDataList = Request["exportDataList"];

            if (!string.IsNullOrEmpty(exportDataList))
            {
                string[] rowList = exportDataList.Split('%');
                TouPiaoContext tc = new TouPiaoContext();

                var tps = tc.TouPiaos.ToList();
                foreach (var tp in tps)
                {
                    tc.TouPiaos.Remove(tp);
                }
                tc.SaveChanges();

                var sns = tc.SystemNames.ToList();
                foreach (var sn in sns)
                {
                    tc.SystemNames.Remove(sn);
                }
                tc.SaveChanges();

                foreach (string rowstr in rowList)
                {
                    string[] colList = rowstr.Split('&');
                    if (colList.Length == 4)
                    {
                        SystemName ed = new SystemName();

                        ed.NameCN = colList[0] == "." ? string.Empty : colList[0];
                        ed.NameEN = colList[1] == "." ? string.Empty : colList[1];
                        ed.Remark = colList[2] == "." ? string.Empty : colList[2];
                        ed.SubmitTime = Convert.ToDateTime(colList[3]);

                        tc.SystemNames.Add(ed);
                        tc.SaveChanges();
                    }
                }
            }

            return View();
        }

        public ActionResult ImportUser()
        {
            string exportDataList = Request["exportDataList"];

            if (!string.IsNullOrEmpty(exportDataList))
            {
                string[] rowList = exportDataList.Split('%');
                TouPiaoContext tc = new TouPiaoContext();

                var tps = tc.TouPiaos.ToList();
                foreach (var tp in tps)
                {
                    tc.TouPiaos.Remove(tp);
                }
                tc.SaveChanges();

                var sns = tc.Users.ToList();
                foreach (var sn in sns)
                {
                    tc.Users.Remove(sn);
                }
                tc.SaveChanges();

                foreach (string rowstr in rowList)
                {
                    string[] colList = rowstr.Split('&');
                    if (colList.Length == 3)
                    {
                        User ed = new User();

                        ed.Company = colList[0];
                        ed.Name = colList[1];
                        ed.Contact = colList[2];

                        tc.Users.Add(ed);
                        tc.SaveChanges();
                    }
                }
            }

            return View();
        }

        public ActionResult TouPiaoResult()
        {
            if (string.IsNullOrEmpty(Request["XingShi"]))
            {
                ViewBag.XingShi = "BG";
                if (string.IsNullOrEmpty(Request["ResultCount"]))
                {
                    ViewBag.BGData = GetResultBG("30");
                }
                else
                {
                    ViewBag.BGData = GetResultBG(Request["ResultCount"]);
                }
            }
            else if (Request["XingShi"] == "TX")
            {
                ViewBag.XingShi = "TX";
                if (string.IsNullOrEmpty(Request["ResultCount"]))
                {
                    ViewBag.TongJiData = GetResultTX("30");
                }
                else
                {
                    ViewBag.TongJiData = GetResultTX(Request["ResultCount"]);
                }
            }
            else
            {
                ViewBag.XingShi = "BG";
                if (string.IsNullOrEmpty(Request["ResultCount"]))
                {
                    ViewBag.BGData = GetResultBG("30");
                }
                else
                {
                    ViewBag.BGData = GetResultBG(Request["ResultCount"]);
                }
            }
            
            return View();
        }

        private string[,] GetResultBG(string resultCount)
        {
            string[,] bgData = new string[Convert.ToInt32(resultCount), 3];

            TouPiaoContext tc = new TouPiaoContext();
            TouPiaoContext tc2 = new TouPiaoContext();

            MySqlConnection dbcon = (MySqlConnection)tc.Database.Connection;
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = dbcon;
            cm.Connection.Open();

            cm.CommandText = "SELECT SystemName_ID,COUNT(User_ID) AS piaos FROM toupiaos GROUP BY SystemName_ID ORDER BY piaos DESC LIMIT " + resultCount + ";";
            MySqlDataReader rd = cm.ExecuteReader();

            int rowIndex = 0;
            while (rd.Read())
            {
                Guid snGuid = Guid.Parse(rd["SystemName_ID"].ToString());
                SystemName systemName = tc2.SystemNames.Where(s => s.ID == snGuid).First();

                bgData[rowIndex, 0] = systemName.NameCN;
                bgData[rowIndex, 1] = systemName.NameEN;
                bgData[rowIndex, 2] = rd["piaos"].ToString();

                rowIndex = rowIndex + 1;
            }

            string[,] bgDataReturn = new string[rowIndex, 3];
            bgDataReturn = bgData;

            rd.Close();
            cm.Connection.Close();

            return bgDataReturn;
        }

        private string GetResultTX(string resultCount)
        {
            TouPiaoContext tc = new TouPiaoContext();
            TouPiaoContext tc2 = new TouPiaoContext();

            MySqlConnection dbcon = (MySqlConnection)tc.Database.Connection;
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = dbcon;
            cm.Connection.Open();

            cm.CommandText = "SELECT SystemName_ID,COUNT(User_ID) AS piaos FROM toupiaos GROUP BY SystemName_ID ORDER BY piaos DESC LIMIT " + resultCount + ";";
            MySqlDataReader rd = cm.ExecuteReader();

            string tempStr = string.Empty;
            while (rd.Read())
            {
                Guid snGuid=Guid.Parse(rd["SystemName_ID"].ToString());
                SystemName systemName = tc2.SystemNames.Where(s => s.ID == snGuid).First();

                string name = string.Empty;
                if (systemName.NameCN != string.Empty)
                {
                    name = systemName.NameCN;
                    if (systemName.NameEN != string.Empty)
                    {
                        name = name + "/" + systemName.NameEN;
                    }
                }
                else
                {
                    if (systemName.NameEN != string.Empty)
                    {
                        name = systemName.NameEN;
                    }
                }

                string val = rd["piaos"].ToString();

                if (tempStr == string.Empty)
                {
                    tempStr = "{ 'name': '" + name + "', 'val': " + val + " }";
                }
                else
                {
                    tempStr = tempStr + ",{ 'name': '" + name + "', 'val': " + val + " }";
                }
            }

            rd.Close();
            cm.Connection.Close();

            //tongJiList["YingXiang"] = "[{ 'name': '中文', 'val': 20 }, { 'name': '英文', 'val': 20 }, { 'name': '泰文', 'val': 20 }, { 'name': '添文', 'val': 20 }, { 'name': '草书', 'val': 20 }]";

            return "[" + tempStr + "]";
        }

        public string Update(string Name, string Company, string Flag, string ID)
        {
            TouPiaoContext tc = new TouPiaoContext();
            Guid UID = Guid.Parse(ID);
            string feebackMessage = string.Empty;
            switch (Flag)
            {
                case "DU":
                    User du = tc.Users.Find(UID);
                    tc.Users.Remove(du);
                    tc.SaveChanges();
                    feebackMessage = "DELTE USER ID:" + ID;
                    break;
                case "AU":
                    User au = tc.Users.Find(UID);
                    au.Company = Company;
                    au.Name = Name;
                    tc.SaveChanges();
                    feebackMessage = "UPDATE USER ID:" + ID + " COMPANY TO :" + Company + " NAME TO:" + Name;
                    break;
                case "DT":
                    TouPiao dt = tc.TouPiaos.Find(UID);
                    tc.TouPiaos.Remove(dt);
                    tc.SaveChanges();
                    feebackMessage = "DELTE TOUPIAO ID:" + ID;
                    break;
                default:
                    break;
            }
            return feebackMessage;
        }
        public ActionResult CheckSubmit()
        {
            Dictionary<string,object> dataDic=new Dictionary<string,object>();
            TouPiaoContext tc = new TouPiaoContext();

            dataDic.Add("UserList", null);
            dataDic.Add("TouPiaos", null);
            dataDic.Add("CompanyNameList", null);
            string companyName = Request["CompanyName"];
            if (!string.IsNullOrEmpty(companyName))
            {
                string isSubmit = Request["IsSubmit"];

                if (companyName == "ALL")
                {
                    switch (isSubmit)
                    {
                        case "ALL":
                            dataDic["UserList"] = tc.Users.ToList();
                            dataDic["TouPiaos"] = tc.TouPiaos.Include(u=>u.User).Include(u=>u.SystemName).ToList();
                            break;
                        case "Y":
                            var qu = from tp in tc.TouPiaos
                                     from us in tc.Users
                                     where tp.User.ID == us.ID
                                     select us;
                            dataDic["UserList"] = qu.ToList();
                            dataDic["TouPiaos"] = tc.TouPiaos.Include(u => u.User).Include(u => u.SystemName).ToList();
                            break;
                        default:
                            var c = tc.TouPiaos.Select(t => t.User.ID).ToList();
                            var b = tc.Users.Select(u => u.ID).ToList();
                            var d = b.Except(c);
                            var qu2 = tc.Users.Where(u => d.Contains(u.ID));
                            dataDic["UserList"] = qu2.ToList();
                            break;
                    }
                    
                }
                else
                {
                    switch (isSubmit)
                    {
                        case "ALL":
                            dataDic["UserList"] = tc.Users.Where(u => u.Company == companyName).ToList();
                            var qt = from tp in tc.TouPiaos
                                     from us in tc.Users
                                     where tp.User.ID == us.ID && us.Company==companyName
                                     select tp;
                            dataDic["TouPiaos"] = qt.Include(u => u.User).Include(u => u.SystemName).ToList();
                            break;
                        case "Y":
                            var qu = from tp in tc.TouPiaos
                                     from us in tc.Users
                                     where tp.User.ID == us.ID && us.Company==companyName
                                     select us;
                            dataDic["UserList"] = qu.ToList();
                            var qt2 = from tp in tc.TouPiaos
                                     from us in tc.Users
                                     where tp.User.ID == us.ID && us.Company == companyName
                                     select tp;
                            dataDic["TouPiaos"] = qt2.Include(u=>u.User).Include(u=>u.SystemName).ToList();
                            break;
                        default:
                            var c = tc.TouPiaos.Select(t => t.User.ID).ToList();
                            var b = tc.Users.Where(u => u.Company == companyName).Select(u => u.ID).ToList();
                            var d = b.Except(c);
                            var qu2 = tc.Users.Where(u => d.Contains(u.ID));
                            dataDic["UserList"] = qu2.ToList();
                            break;
                    }
                }
            }
            
            dataDic["CompanyNameList"] = tc.Users.Select(u => u.Company).Distinct().ToList();
            
            return View(dataDic);
        }
	}
}