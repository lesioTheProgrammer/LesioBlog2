namespace LesioBlog2_Repo.Models
{
    public class IfPlusowalWpis
    {
        public int UserID { get; set; } //fk
        public int WpisID { get; set; } //fk

        public bool  IfPlusWpis { get; set; } //prop

        public virtual User User { get; set; }  //navi
        public virtual Wpis Wpis { get; set; } //navi

    }
}