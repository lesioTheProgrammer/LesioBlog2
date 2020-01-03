using System;
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


        public int UserFrom_Id { get; set; }
        public int UserTo_Id { get; set; }


        public virtual User UserTo { get; set; }
        public virtual User UserFrom { get; set; }


    }
}