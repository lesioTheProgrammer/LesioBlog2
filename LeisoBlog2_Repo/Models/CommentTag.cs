﻿namespace LesioBlog2_Repo.Models
{
    public class CommentTag
    {
        public int Comment_Id { get; set; }
        public int Tag_Id { get; set; }
        //navis
        public Tag Tag { get; set; }
        public Comment Comment { get; set; }
    }
}