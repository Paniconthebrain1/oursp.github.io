using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OurSunday.Data;
using OurSunday.Models;
using OurSunday.ViewModel;

namespace OurSunday.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        private readonly IWebHostEnvironment _WebHostenvironment;

        public SettingController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notification = notyfService;
            _WebHostenvironment = webHostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = _context.Settings!.ToList();
            if (settings.Count > 0)
            {
                var vm = new SettingVM()
                {
                    id = settings[0].id,
                    SiteName = settings[0].SiteName,
                    Title = settings[0].Title,
                    ShortDescription = settings[0].ShortDescription,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    FacebookUrl = settings[0].FacebookUrl,
                    GithubUrl = settings[0].GithubUrl,
                    TwitterUrl = settings[0].TwitterUrl,
                };
                return View(vm);
            }
            var setting = new Setting()
            {
                SiteName = "Demo Name",
            };
            await _context.Settings!.AddAsync(setting);
            await _context.SaveChangesAsync();
            var createdSettings = _context.Settings!.ToList();
            var createdVm = new SettingVM()
            {
                id = createdSettings[0].id,
                SiteName = createdSettings[0].SiteName,
                Title = createdSettings[0].Title,
                ShortDescription = createdSettings[0].ShortDescription,
                ThumbnailUrl = createdSettings[0].ThumbnailUrl,
                FacebookUrl = createdSettings[0].FacebookUrl,
                GithubUrl = createdSettings[0].GithubUrl,
                TwitterUrl = createdSettings[0].TwitterUrl,
            };
            return View(createdVm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SettingVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var setting = await _context.Settings!.FirstOrDefaultAsync(x => x.id == vm.id);
            if (setting == null)
            {
                _notification.Error("Something went wrong");
                return View(vm);
            }
            setting.SiteName = vm.SiteName;
            setting.Title = vm.Title;
            setting.ShortDescription = vm.ShortDescription;
            setting.FacebookUrl = vm.FacebookUrl;
            setting.GithubUrl = vm.GithubUrl;
            setting.TwitterUrl = vm.TwitterUrl;

            if (vm.Thumbnail != null)
            {
                setting.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Setting updated succesfully");
            return RedirectToAction("Index", "Setting", new { area = "Admin" });
        }

        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_WebHostenvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }

}
