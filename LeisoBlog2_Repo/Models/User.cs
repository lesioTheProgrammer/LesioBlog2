using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LesioBlog2_Repo.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string NickName { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }

        public virtual ICollection<Wpis> Wpis { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}