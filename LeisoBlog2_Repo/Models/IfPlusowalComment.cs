namespace LesioBlog2_Repo.Models
{
    public class IfPlusowalComment
    {
        public int UserID { get; set; } //fk
        public int CommentID { get; set; } //fk

        public bool IfPlusWpis { get; set; } //prop

        public virtual User User { get; set; }  //navi
        public virtual Comment Comment { get; set; } //navi

    }
}