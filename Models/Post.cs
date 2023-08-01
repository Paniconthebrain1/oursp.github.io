﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OurSunday.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        //relation
        public string? ApplicationUserid { get; set; }
        public ApplicationUser? ApplicationUser { get; set;}
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? Slug { get; set;}
        public string? ThumbnailUrl { get; set; }

    }
}
