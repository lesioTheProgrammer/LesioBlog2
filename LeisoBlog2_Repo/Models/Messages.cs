using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LesioBlog2_Repo.Models
{
    public class Messages
    {
        [Key]
        public int Message_Id { get; set; }
        public string MessageTitle { get; set; } ////
        public DateTime MessSendDate { get; set; }
        public string Content2 { get; set; } //rep
        public bool Active { get; set; }


        //many mess to many users
        //moge usera po fk pobrac przeciez
        public virtual ICollection<UserMess> UserMess { get; set; }

    }
}