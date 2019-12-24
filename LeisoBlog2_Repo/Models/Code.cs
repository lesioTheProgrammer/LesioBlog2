using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LesioBlog2_Repo.Models
{
    public class Code
    {
        [Key]
        [ForeignKey("User")]
        public int User_Id { get; set; } //fk pk

        public int CodeValue { get; set; }

        public virtual User User { get; set; } //navi
    }
}