using LesioBlog2_Repo.Models;

namespace LesioBlog2_Repo.Models
{
    public class UserMess
    {
        public int Message_Id { get; set; }

        public int User_Id { get; set; }

        public virtual Messages Messages { get; set; }
        public virtual  User Users { get; set; }

    }
}