using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Collections;

namespace MVC_WenJuan.Models.QandA
{
    public class Answer
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public Qusetion Qusetion { get; set; }
        [Required]
        public string Body { get; set; }
        public DateTime SubmitTime { get; set; }
        [Required]
        public User SubmitUser { get; set; }
        public virtual ICollection<User> SupportUsers { get; set; }
        public Answer()
        {
            this.ID = Guid.NewGuid();
        }
    }

    public class State
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Remark { get; set; }
        public State()
        {
            this.ID = Guid.NewGuid();
        }
    }
    public class Qusetion
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public QuestionClass QuestionClass { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public State State { get; set; }
        public DateTime SubmitTime { get; set; }
        [Required]
        public User SubmitUser { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public Qusetion()
        {
            this.ID = Guid.NewGuid();
        }
    }

    public class QuestionClass
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string ClassName { get; set; }
        public string Remark { get; set; }
        public int IndexNo { get; set; }
        public QuestionClass()
        {
            this.ID = Guid.NewGuid();
        }
    }

    public class User
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Regional { get; set; }
        public string Contact { get; set; }
        public DateTime Email { get; set; }
        public User()
        {
            this.ID = Guid.NewGuid();
        }
    }

    public class QandAContext : DbContext
    {
        public IDbSet<Answer> Answers { get; set; }
        public IDbSet<State> States { get; set; }
        public IDbSet<Qusetion> Qusetions { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<QuestionClass> QuestionClasss { get; set; }
    }
    
}