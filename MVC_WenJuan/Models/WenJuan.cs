using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MVC_WenJuan.Models
{
    public class WenJuan
    {
        [Key]
        public Guid ID { get; set; }
        public Guid SubmitID { get; set; }
        public string BanShiChu { get; set; }
        public string YingXiang { get; set; }
        public string YingXiang_Other { get; set; }
        public string YiShiGuanNian { get; set; }
        public string YiShiGuanNian_Other { get; set; }
        public string ZhongDian { get; set; }
        public string LiuCheng { get; set; }
        public string XinXi { get; set; }
        public string BaoHe { get; set; }
        public string PeiYang { get; set; }
        public string TiSheng { get; set; }
        public string TiSheng_Other { get; set; }
        public string GangWeiWenTi { get; set; }
        public string FenPeiFnagAn { get; set; }
        public string JieYue { get; set; }
        public string JieYue_Other { get; set; }
        public string XuQiu { get; set; }
        public string BuTongChang { get; set; }
        public string GangWeiHeLi { get; set; }
        public string LiuChengJianYi { get; set; }
        public string KeFuQueDian { get; set; }
        public string JiangBen { get; set; }
        public string JiaQiang { get; set; }
        public string KaoHe { get; set; }
        public string PeiYangBuHeLi { get; set; }
        public string KaoHeGaiJin { get; set; }
        public string GouTong { get; set; }
        public string RenLiGaiJin { get; set; }
        public string PeiXunWenTi { get; set; }
        public string JianYiA { get; set; }
        public string JianYiB { get; set; }
        public string JianYiC { get; set; }
        public string JianYiD { get; set; }

        public WenJuan()
        {
            this.ID = Guid.NewGuid();
        }
    }

    public class WenJuanContext : DbContext
    {
        public IDbSet<WenJuan> WenJuans { get; set; }

    }

    //public class WenJuan
    //{
    //    [Key]
    //    public Guid ID { get; set; }
    //    public string WenJuanName { get; set; }
    //    public string XuYan { get; set; }
    //    public bool Enable { get; set; }
    //    public List<Subject> Subjects { get; set; }
    //    public WenJuan()
    //    {
    //        this.ID = Guid.NewGuid();
    //        this.Subjects = new List<Subject>();
    //        this.Enable = false;
    //    }
    //}

    //public class Subject
    //{
    //    [Key]
    //    public Guid ID { get; set; }
    //    public string SubjectName { get; set; }
    //    public List<Question> Questions { get; set; }
    //    public Subject()
    //    {
    //        this.ID = Guid.NewGuid();
    //        this.Questions = new List<Question>();
    //    }
    //}

    //public class Question
    //{
    //    [Key]
    //    public Guid ID { get; set; }
    //    public string QuestionContent { get; set; }
    //    public List<Option> Options { get; set; }

    //    public Question()
    //    {
    //        this.ID = Guid.NewGuid();
    //        this.Options = new List<Option>();
    //    }
    //}

    //public class Option
    //{
    //    [Key]
    //    public Guid ID { get; set; }
    //    public string OptionContent { get; set; }
    //    public string OptionClass { get; set; }
    //    public string OptionData { get; set; }
    //    public int Max { get; set; }
    //    public int Min { get; set; }
    //    public Option()
    //    {
    //        this.ID = Guid.NewGuid();
    //    }
    //}

    //public class Result
    //{
    //    [Key]
    //    public Guid ID { get; set; }
    //    public WenJuan WenJuan { get; set; }
    //    public Subject Subject { get; set; }
    //    public Question Question { get; set; }
    //    public User User { get; set; }
    //    public Result()
    //    {
    //        this.ID = Guid.NewGuid();
    //    }
    //}

    //public class OptionResult
    //{
    //    [Key]
    //    public Guid ID { get; set; }
    //    public Result Result { get; set; }
    //    public Option Option { get; set; }
    //    public string OptionResultData { get; set; }
    //    public OptionResult()
    //    {
    //        this.ID = Guid.NewGuid();
    //    }
    //}

    //public class User
    //{
    //    [Key]
    //    public Guid ID { get; set; }
    //    public WenJuan WenJuan { get; set; }
    //    public string Organization { get; set; }
    //    public string Name { get; set; }
    //    public string LoginWord { get; set; }
    //    public bool Submited { get; set; }
    //    public User()
    //    {
    //        this.ID = Guid.NewGuid();
    //    }
    //}

    //public class WenJuanContext : DbContext
    //{
    //    public IDbSet<WenJuan> WenJuans { get; set; }
    //    public IDbSet<Subject> Subjects { get; set; }
    //    public IDbSet<Question> Questions { get; set; }
    //    public IDbSet<Option> Options { get; set; }
    //    public IDbSet<Result> Results { get; set; }
    //    public IDbSet<OptionResult> OptionResults { get; set; }
    //    public IDbSet<User> Users { get; set; }

    //}
}