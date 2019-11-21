namespace LesioBlog2_Repo.Models
{
    public class PostTag
    {
        public int Post_Id { get; set; }
        public int Tag_Id { get; set; }

        //navis
        public Tag Tag { get; set; }
        public Post Post { get; set; }
    }
}