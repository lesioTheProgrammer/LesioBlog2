using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LesioBlog2_Repo.Models
{
    public class User
    {
        public int UserID { get; set; }
        public int GenderID { get; set; }



        [Required]
        [EmailAddress]
        [Display(Name = "Email adress: ")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 6)]

        [Display(Name = "Password: ")]


        public string Password { get; set; }

        public string PasswordSalt { get; set; }  //prevent reverse engineering to get password

        [Required]
        [Display(Name = "Nickname:")]
        public string NickName { get; set; }
        [Display(Name = "Full Name:")]

        public string FullName { get; set; }
        [Display(Name = "City:")]
        public string City { get; set; }

        public int Code { get; set; }




        public virtual ICollection<Wpis> Wpis { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual Gender Gender { get; set; } //1 gender to one user
        public virtual ICollection<IfPlusowalWpis> IfPlusowalWpis {get; set;}
    }
}