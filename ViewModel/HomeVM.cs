using OurSunday.Models;
using X.PagedList;

namespace OurSunday.ViewModel
{
    public class HomeVM
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IPagedList<Post>? Posts { get; set; }


    }
}
