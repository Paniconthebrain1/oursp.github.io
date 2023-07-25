using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OurSunday.Data;
using OurSunday.Models;
using OurSunday.ViewModel;
using System.Runtime.InteropServices;

namespace OurSunday.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class PostController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _WebHostenvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notification { get; } 

        public PostController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostenvironment,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notyfService;
            _WebHostenvironment = webHostenvironment;
            _userManager = userManager;
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View(new CreatePostVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }

            //get login id 

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            var post = new Post();

            post.Title = vm.Title; 
            post.Description = vm.Description;
            post.ShortDescription = vm.ShortDescription;
            post.ApplicationUserid = loggedInUser!.Id;

            if(post.Title!=null)
            {
                string slug = vm.Title!.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }


            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            await _context.Posts!.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Published Succesfully.");
            return RedirectToAction("Index");


        }


        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderpath = Path.Combine(_WebHostenvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath= Path.Combine(folderpath, uniqueFileName);
            using(FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }

    }
}
