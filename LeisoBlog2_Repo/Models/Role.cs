using LesioBlog2_Repo.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LesioBlog2_Repo.Models
{
    public class Role
    {
        [Key]
        public int Role_Id { get; set; }
        public string RoleName { get; set; }

        public virtual  ICollection<User> User { get; set; }
    }
}