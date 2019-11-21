namespace LesioBlog2_Repo.Models
{
    public class IsCommUpvoted
    {
        public int User_Id { get; set; } //fk
        public int Comment_Id { get; set; } //fk

        public bool IsCommentUpvoted { get; set; } //prop

        public virtual User User { get; set; }  //navi
        public virtual Comment Comment { get; set; } //navi

    }
}