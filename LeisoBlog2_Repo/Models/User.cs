using LesioBlog2_Repo.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LesioBlog2_Repo.Models
{
    public class User
    {

        [Key]
        public int User_Id { get; set; }
        public int Gender_Id { get; set; } //fk

        public int Role_Id { get; set; } //fk

        public int Code_Id { get; set; } //fk
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
        [RegularExpression(@"[A-Za-z0-9_]*", ErrorMessage = "No white space or special chars allowed")]
        public string NickName { get; set; }
        [Display(Name = "Full Name:")]
        public string FullName { get; set; }
        [Display(Name = "City:")]
        public string City { get; set; }

        public bool Active { get; set; }


        public virtual ICollection<Post> Post { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Gender Gender { get; set; } //1 gender to one user
        public virtual ICollection<IsPostUpvd> IsPostUpvd {get; set;}

        public virtual Code Code { get; set; } //1 code to one user

        public virtual Role Role { get; set; } //1 role to one user
    }
}