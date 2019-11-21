using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LesioBlog2_Repo.Models
{
    public class Tag
    {
        [Key]
        public int Tag_Id { get; set; }
        public string TagName { get; set; }
        //many tags many post
        public virtual ICollection<PostTag> PostTag { get; set; }
        //many tags many comme
        public virtual ICollection<CommentTag> CommentTags { get; set; }
    }
}