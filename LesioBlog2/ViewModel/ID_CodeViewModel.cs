using System.ComponentModel.DataAnnotations;

namespace LesioBlog2.ViewModel
{
    public class ID_CodeViewModel
    {
        public int ID { get; set; }
        public int Code { get; set; }

      
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 6)]

        [Display(Name = "Password: ")]


        public string Password { get; set; }


    }
}