namespace LesioBlog2_Repo.Models
{
    public class WpisTag
    {
        public int WpisID { get; set; }
        public int TagID { get; set; }


        //navis
        public Tag Tag { get; set; }
        public Wpis Wpis { get; set; }
    }
}