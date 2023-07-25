using Microsoft.Build.Framework;
using OurSunday.Models;

namespace OurSunday.ViewModel
{
    public class CreatePostVM
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        
        public string? ShortDescription { get; set; }
        public string? ApplicationUserid { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }

    }
}
