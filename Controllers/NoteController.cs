using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models.NoteViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class NoteController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IToaster _toaster;
        private readonly IFileIOHelper _io_Helper;
        private readonly IConfiguration _config;

        public string ImageUploadPath => _config.GetValue<string>("img_upload_path");

        public NoteController(
            ApplicationDbContext context,
            IToaster toaster,
            IFileIOHelper io_Helper,
            IConfiguration config
            )
        {
            _context = context;
            _toaster = toaster;
            _io_Helper = io_Helper;
            _config = config;
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Note.ToListAsync());
        }

        // POST: Note/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")]Note note, IList<IFormFile> contentImgs)
        {
            if (ModelState.IsValid)
            {
                //save the Images
                if (contentImgs.Count > 0)
                {
                    List<string> ArticleImgs = new List<string>();

                    //If your note need imgs, it probably need a title.
                    if (!string.IsNullOrEmpty(note.Title))
                    {
                        foreach (var img in contentImgs)
                        {
                            var res = await _io_Helper.SaveImgAsync(img, note.Title, 256, img.FileName);
                            ArticleImgs.Add(res);
                        }
                    }

                    if (ArticleImgs.Count > 0)
                        note.MediaSerialized = JsonConvert.SerializeObject(ArticleImgs);
                }

                _context.Add(note);
                await _context.SaveChangesAsync();
                _toaster.ToastSuccess("笔记创建成功");
                return RedirectToAction(nameof(HomeController.Notes));
            }
            return View(note);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.Where(n => n.NoteID == id).FirstOrDefaultAsync();
            if (note == null)
            {
                _toaster.ToastError("获取笔记失败");
                return NotFound();
            }

            return View(note);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.Where(n => n.NoteID == id).FirstOrDefaultAsync();
            if (note == null)
            {
                _toaster.ToastError("获取笔记失败");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await TryUpdateModelAsync(note, "",
                             n => n.Title, n => n.Content, n => n.MediaSerialized
                        );
                    await _context.SaveChangesAsync();

                    _toaster.ToastSuccess("编辑笔记成功");
                }
                catch (DbUpdateException)
                {
                    _toaster.ToastError("编辑笔记失败");
                }
            }

            return View(note);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.Where(n => n.NoteID == id).FirstOrDefaultAsync();
            if (note == null)
            {
                _toaster.ToastError("获取笔记失败");
                return NotFound();
            }

            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Note.Where(n => n.NoteID == id).FirstOrDefaultAsync();

            if(!string.IsNullOrEmpty(note.Title))
            {
                _io_Helper.DeleteImgDir(note.Title);
            }

            if (note == null)
            {
                _toaster.ToastError("获取笔记失败");
                return NotFound();
            }

            try
            {
                _context.Note.Remove(note);
                await _context.SaveChangesAsync();
                _toaster.ToastSuccess("删除笔记成功");
            }
            catch (DbUpdateException)
            {
                _toaster.ToastError("删除笔记失败");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}