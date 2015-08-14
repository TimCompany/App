using MVC_WenJuan.Models.QandA;
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
    public class QandAController : Controller
    {
        //
        // GET: /QandA/
        public ActionResult Index()
        {
            Dictionary<string, object> dataDic = new Dictionary<string, object>();

            QandAContext qc =new QandAContext();

            if (qc.QuestionClasss.Count() == 0)
            {
                QuestionClass c1 = new QuestionClass();
                c1.ClassName = "费用信息维护";
                c1.IndexNo = 1;

                QuestionClass c2 = new QuestionClass();
                c2.ClassName = "协议相关问题";
                c2.IndexNo = 2;

                QuestionClass c3 = new QuestionClass();
                c3.ClassName = "操作流程指引";
                c3.IndexNo = 3;

                QuestionClass c4 = new QuestionClass();
                c4.ClassName = "流程改进完善";
                c4.IndexNo = 4;

                QuestionClass c5 = new QuestionClass();
                c5.ClassName = "需求完善建议";
                c5.IndexNo = 5;

                QuestionClass c6 = new QuestionClass();
                c6.ClassName = "其他问题建议";
                c6.IndexNo = 6;

                qc.QuestionClasss.Add(c1);
                qc.QuestionClasss.Add(c2);
                qc.QuestionClasss.Add(c3);
                qc.QuestionClasss.Add(c4);
                qc.QuestionClasss.Add(c5);
                qc.QuestionClasss.Add(c6);

                qc.SaveChanges();

            }

            if (qc.States.Count() == 0)
            {
                State c1 = new State();
                c1.Name = "待解决";
                qc.States.Add(c1);

                State c2 = new State();
                c2.Name = "已解决";
                qc.States.Add(c2);

                State c3 = new State();
                c3.Name = "待确认";
                qc.States.Add(c3);
 
                State c4 = new State();
                c4.Name = "已关闭";
                qc.States.Add(c4);

                State c5 = new State();
                c5.Name = "加急";
                qc.States.Add(c5);
                qc.SaveChanges();
               
            }

            dataDic.Add("TabList", qc.QuestionClasss.OrderBy(t=>t.IndexNo).ToList());

            string tabID = Request["TabID"];

            dataDic.Add("QuestionTable", null);
            if (string.IsNullOrEmpty(tabID))
            {
                tabID = qc.QuestionClasss.FirstOrDefault().ID.ToString();
            }

            ViewBag.TabID = tabID;
            
            Guid tid = Guid.Parse(tabID);
            if (qc.Qusetions.Where(q => q.QuestionClass.ID == tid).Count() != 0)
            {
                var tabs = qc.Qusetions.Include(q => q.Answers).Include(q => q.SubmitUser).Include(q => q.State).Where(q => q.QuestionClass.ID == tid).ToList();
                foreach (var qu in tabs)
                {
                    foreach (var an in qu.Answers)
                    {
                        an.SupportUsers.ToList();
                        an.SubmitUser = qc.Answers.Where(a => a.ID == an.ID).Select(a => a.SubmitUser).First();
                    }
                }
                dataDic["QuestionTable"] = tabs;
            }

            return View(dataDic);
        }

        public string CreateQuestion()
        {
            if (!string.IsNullOrEmpty(Request["UserName"]) && !string.IsNullOrEmpty(Request["questionBody"]))
            {
                QandAContext qc = new QandAContext();

                string userName = Request["UserName"];
                string userContact = Request["UserContact"];
                string questionClass = Request["QuestionClass"];
                string questionSubject = Request["questionSubject"];
                string questionBody = Request["questionBody"];

                User u;
                int uCount = qc.Users.Where(us => us.Name == userName).Count();
                if (uCount == 0)
                {
                    u = new User();
                    u.Name = userName;
                    u.Contact = userContact;
                    qc.Users.Add(u);
                    qc.SaveChanges();
                }
                else
                {
                    u = qc.Users.Where(us => us.Name == userName).First();
                    u.Contact = userContact;
                }

                Qusetion qu = new Qusetion();
                qu.QuestionClass = qc.QuestionClasss.Find(Guid.Parse(questionClass));
                qu.Subject = questionSubject;
                qu.Body = questionBody;
                qu.SubmitUser = u;
                qu.SubmitTime = DateTime.Now;
                qu.State = qc.States.Where(s => s.Name == "待解决").First();

                qc.Qusetions.Add(qu);
                qc.SaveChanges();

                return "QuestionOK";
            }

            return "False";
        }

        public string AnswerQuestion()
        {
            if (!string.IsNullOrEmpty(Request["UserName"]) && !string.IsNullOrEmpty(Request["AnswerBody"]))
            {
                QandAContext qc = new QandAContext();

                string userName = Request["UserName"];
                string userContact = Request["UserContact"];
                string questionID = Request["QuestionID"];
                string answerBody = Request["AnswerBody"];

                User u;
                int uCount = qc.Users.Where(us => us.Name == userName).Count();
                if (uCount == 0)
                {
                    u = new User();
                    u.Name = userName;
                    u.Contact = userContact;
                    qc.Users.Add(u);
                    qc.SaveChanges();
                }
                else
                {
                    u = qc.Users.Where(us => us.Name == userName).First();
                    u.Contact = userContact;
                }

                Qusetion qu = qc.Qusetions.Find(Guid.Parse(questionID));

                Answer an = new Answer();
                an.Body = answerBody;
                an.SubmitUser = u;
                an.SubmitTime = DateTime.Now;
                an.Qusetion = qu;

                qc.Answers.Add(an);

                qc.SaveChanges();

                return "AnswerOK";
            }

            return "False";
        }
	}
}