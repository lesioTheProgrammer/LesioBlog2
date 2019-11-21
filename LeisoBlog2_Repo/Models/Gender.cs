﻿using LesioBlog2_Repo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LesioBlog2_Repo.Models
{
    public class Gender
    {
        [Key]
        public int Gender_Id { get; set; }
        public string GenderName { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}