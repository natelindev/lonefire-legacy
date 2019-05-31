using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Models.NoteViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class NoteController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IToaster _toaster;
        private readonly IFileIOHelper _io_Helper;

        public NoteController(
            ApplicationDbContext context,
            IToaster toaster,
            IFileIOHelper io_Helper
            )
        {
            _context = context;
            _toaster = toaster;
            _io_Helper = io_Helper;
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Note> notes = await _context.Note.ToListAsync();
            return View(notes);
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            List<Note> notes = await _context.Note.ToListAsync();
            return View(notes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")]Note note, IFormFile headerImg, IList<IFormFile> contentImgs)
        {
            if (ModelState.IsValid)
            {
                //save the Images
                if (headerImg != null || contentImgs.Count > 0)
                {
                    List<string> ArticleImgs = new List<string>();
                    if (headerImg != null)
                    {
                        var headerImgName = await _io_Helper.SaveImgAsync(headerImg, note.Title, 256, headerImg.FileName);
                        note.HeaderImg = headerImgName;
                        ArticleImgs.Add(headerImgName);
                    }

                    foreach (var img in contentImgs)
                    {
                        var res = await _io_Helper.SaveImgAsync(img, note.Title, 256, img.FileName);
                        ArticleImgs.Add(res);
                    }

                    if (ArticleImgs.Count > 0)
                        note.MediaSerialized = JsonConvert.SerializeObject(ArticleImgs);
                }

                _context.Add(note);
                await _context.SaveChangesAsync();
                _toaster.ToastSuccess("笔记创建成功");
                return RedirectToAction(nameof(Index));
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
                             n => n.Title, n => n.Content, n => n.HeaderImg, n => n.MediaSerialized
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