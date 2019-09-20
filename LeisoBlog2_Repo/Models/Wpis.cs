using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LesioBlog2_Repo.Models
{
    public class Wpis : RepeatedFields
    {
        //wiele do wielu z tagami
        //1 wpis ma wiele comentow
        public int WpisID { get; set; } //pk

        public int UserID { get; set; } //fk usera, 1 user 1 wpis


        //navis
        public virtual ICollection<WpisTag> WpisTags { get; set; }

        public virtual User User { get; set; }

        public virtual  ICollection<Comment> Comments { get; set; }



    }
}