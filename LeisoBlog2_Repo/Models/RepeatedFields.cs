using System;
using System.ComponentModel.DataAnnotations;

namespace LesioBlog2_Repo.Models
{
    public class RepeatedFields
    {
        [Required(ErrorMessage = "You can not send empty wpis/comment")]
        public string Content { get; set; }
        public DateTime AddingDate { get; set; }

        public int Votes { get; set; }


        public DateTime? EditingDate { get; set; }

        public bool Active { get; set; }
    }
}