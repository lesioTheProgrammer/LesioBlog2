using System.Collections.Generic;

namespace LesioBlog2_Repo.Models
{
    public class Comment : RepeatedFields
    {

        public int CommentID { get; set; }

        public int UserID { get; set; }  //fk

        public int WpisID { get; set; }  //fk


        //comment przypisany do 1 wpisu i do jednego usera
        //navis:
        public virtual User User { get; set; }
        public virtual ICollection<CommentTag> CommentTags { get; set; }

        public Wpis Wpis { get; set; } //1 wpis wiele komentow?



    }
}