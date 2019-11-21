using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LesioBlog2_Repo.Models
{
    public class Comment : RepeatedFields
    {
        [Key]
        public int Comment_Id { get; set; }
        public int User_Id { get; set; }  //fk
        public int Post_Id { get; set; }  //fk

        //comment assign to one wpis and one user
        //navis:
        public virtual User User { get; set; }
        public virtual ICollection<CommentTag> CommentTags { get; set; }
        public Post Post { get; set; } //1 post has many comments
    }
}