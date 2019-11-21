using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LesioBlog2_Repo.Models
{
    public class Post : RepeatedFields
    {
        //many to many with tags
        //1 post has many comments
        [Key]
        public int Post_Id { get; set; } //pk

        public int User_Id { get; set; } //fk usera, 1 user 1 wpis


        //navis
        public virtual ICollection<PostTag> PostTags { get; set; }

        public virtual User User { get; set; }

        public virtual  ICollection<Comment> Comments { get; set; }
    }
}