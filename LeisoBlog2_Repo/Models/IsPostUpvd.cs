namespace LesioBlog2_Repo.Models
{
    public class IsPostUpvd
    {
        public int User_Id { get; set; } //fk
        public int Post_Id { get; set; } //fk

        public bool  IsPostUpvoted { get; set; } //prop

        public virtual User User { get; set; }  //navi
        public virtual Post Post { get; set; } //navi

    }
}