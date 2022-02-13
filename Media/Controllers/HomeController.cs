using Media.Data;
using Media.Models;
using Media.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Media.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
            // string x = $"{nameof(ApplicationDbContext)}";
        }

        public IActionResult Index()
        {
            var folderList = _db.Folders.Where(x => x.ParentId == null).ToList();
            return View(folderList);
        }

        [HttpGet]
        public IActionResult AddNewFolder(int? parentId)
        {
            ViewBag.ParentId = parentId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNewFolder(Folder model)
        {
            if (ModelState.IsValid)
            {
                if (_db.Folders.Any(x => x.ParentId == model.ParentId && x.Title == model.Title))
                {
                    var list = _db.Folders.Where(x => x.ParentId == model.ParentId && x.Title == model.Title).ToList();
                    foreach (var item in list)
                    {
                        if (item.Title == model.Title)
                        {
                            ModelState.AddModelError("entity error", "Title and Parent Folder Is not unique");
                            return View(model);
                        }
                    }

                }

                _db.Folders.Add(model);
                var saveResult = _db.SaveChanges();
                if (saveResult > 0)
                    if (model.ParentId == null)
                        return RedirectToAction(nameof(Index));
                    else
                        return Redirect($"/home/folder?folderId={model.ParentId}");

            }
            return View(model);
        }



        //[Route("{folderId:int}")]
        // [HttpGet("{folderId:int}/{parentid:int}")]
        public IActionResult Folder(int folderId)
        {
            var folder = _db.Folders
                .Include(x => x.Files)
                .Include(x => x.Childs)
                .FirstOrDefault(x => x.Id == folderId);
            return View(folder);
        }


        [HttpPost]
        public IActionResult RenameFolder(string folderTitle, int folderId)
        {
            var folders = _db.Folders.Where(x => x.Title == folderTitle).ToList();
            foreach (var item in folders)
            {
                if (item.Title == folderTitle)
                {
                    return Json(new { isSuccess = false, message = "Title used by another folder!" });
                }
            }
            var folder = _db.Folders.FirstOrDefault(x => x.Id == folderId);
            folder.Title = folderTitle;
            var saveResult = _db.SaveChanges();
            if (saveResult > 0)
                return Json(new { isSuccess = true, message = "Successfully changed" });
            return Json(new { isSuccess = false, message = "Error in saveing change happend!" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}