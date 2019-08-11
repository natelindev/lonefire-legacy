using lonefire.Data;
using lonefire.Models;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace lonefire.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileIOHelper _io_helper;
        private readonly IConfiguration _config;
        private readonly IToaster _toaster;

        public ImageController(
            ILogger<AccountController> logger,
            ApplicationDbContext context,
            IFileIOHelper ioHelper,
            IConfiguration config,
            IToaster toaster
           )
        {
            _io_helper = ioHelper;
            _logger = logger;
            _context = context;
            _config = config;
            _toaster = toaster;
        }

        public string ImageUploadPath => _config.GetValue<string>("img_upload_path");

        [HttpPost]
        public async Task<IActionResult> AjaxImgUpload(IFormFile upload)
        {

            string jsonString = "";

            string img_name = await _io_helper.SaveImgAsync(upload, upload.FileName, 256);

            if (string.IsNullOrEmpty(img_name))
            {
                jsonString = JsonConvert.SerializeObject(new { uploaded = false, error = new { message = "图片上传失败" } });
            }
            else
            {
                jsonString = JsonConvert.SerializeObject(new { uploaded = true, url = ImageUploadPath + img_name });
            }

            return Content(jsonString, "application/json");

        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Image.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Path, IList<IFormFile> uploads)
        {
            if (ModelState.IsValid)
            {
                foreach(var upload in uploads)
                {
                    var new_image = new Image
                    {
                        Path = Path,
                        Name = upload.FileName
                    };

                    string img_name = await _io_helper.SaveImgAsync(upload, Path, 256, upload.FileName);
                    _context.Add(new_image);
                }
                try
                {
                    await _context.SaveChangesAsync();
                    _toaster.ToastSuccess("图片上传成功");
                    return RedirectToAction(nameof(HomeController.Images), "Home");
                }
                catch (Exception)
                {
                    _toaster.ToastError("图片上传失败");
                }
            }
            return RedirectToAction(nameof(HomeController.Images), "Home");
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.Where(n => n.ImageID == id).FirstOrDefaultAsync();
            if (image == null)
            {
                _toaster.ToastError("获取图片失败");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await TryUpdateModelAsync(image, "",
                             i => i.Name, i => i.Path
                        );
                    await _context.SaveChangesAsync();

                    _toaster.ToastSuccess("编辑图片成功");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    _toaster.ToastError("编辑图片失败");
                }
            }

            return View(image);
        }


        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Image.FindAsync(id);
            _context.Image.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Image.Any(e => e.ImageID == id);
        }
    }
}
