using System.Collections.Generic;

namespace LesioBlog2_Repo.Models
{
    public class Tag
    {

        public int TagID { get; set; }

        public string TagName { get; set; }

        //wiele tagow wiele wpisow

        public virtual ICollection<WpisTag> WpisTag { get; set; }

        //wiele tagoe wiele komentow
        public virtual ICollection<CommentTag> CommentTags { get; set; }
    }
}