using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MVC_WenJuan.Models
{
    public class User
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Contact { get; set; }
        public User()
        {
            this.ID = Guid.NewGuid();
        }
    }

    public class SystemName
    {
        public Guid ID { get; set; }
        public string NameEN { get; set; }
        public string NameCN { get; set; }
        public string Remark { get; set; }
        public DateTime SubmitTime { get; set; }
        public SystemName()
        {
            this.ID = Guid.NewGuid();
        }
    }
    public class TouPiao
    {
        [Key]
        public Guid ID { get; set; }
        public User User { get; set; }
        public SystemName SystemName { get; set; }
        public DateTime SubmitTime { get; set; }
        public string SubmitMAC { get; set; }
        public string SubmitIP { get; set; }

        public TouPiao()
        {
            this.ID = Guid.NewGuid();
        }
    }

    public class TouPiaoContext : DbContext
    {
        public IDbSet<User> Users { get; set; }
        public IDbSet<SystemName> SystemNames { get; set; }
        public IDbSet<TouPiao> TouPiaos { get; set; }
    }
    
}