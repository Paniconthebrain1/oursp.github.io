using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OurSunday.Data;
using OurSunday.ViewModel;

namespace OurSunday.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }

        public BlogController(ApplicationDbContext context, INotyfService notyfService)
        {
            _context = context;        
            _notification = notyfService;
        }

        [HttpGet]
        public IActionResult Post(string slug)
        {
            if (slug == "")
            {
                _notification.Error("Posts Not Found!!");
                return View();
            }

            var post = _context.Posts!.Include(x=> x.ApplicationUser).FirstOrDefault(x => x.Slug == slug);
            if (post == null)
            {
                _notification.Error("Posts Not Found!!");
                return View();
            }

            var vm = new BlogPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.ApplicationUser!.FirstName + " " + post.ApplicationUser!.LastName,
                CreatedDate = post.CreateDate,
                ThumbnailUrl = post.ThumbnailUrl,
                Description = post.Description,
                ShortDescription = post.ShortDescription,
            };

            return View(vm);
        }
    }
}
