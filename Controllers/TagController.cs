using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Models.ArticleViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lonefire.Controllers
{
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToaster _toaster;

        public TagController(
            ApplicationDbContext context,
            IToaster toaster
            )
        {
            _context = context;
            _toaster = toaster;
        }

        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tag.ToListAsync();

            return View(tags);
        }

        public async Task<IActionResult> List(int? id)
        {
            if (id == null)
            {
                _toaster.ToastError("未找到该标签");
                return NotFound();
            }
                
            var tagToBeFound = await _context.Tag.Where(t => t.TagID == id).FirstOrDefaultAsync();

            if (tagToBeFound == null)
            {
                _toaster.ToastError("未找到该标签");
                return NotFound();
            }
                
            var artcles = await _context.Article.Where(a => a.Tag.Contains(tagToBeFound.TagName) && a.Status == ArticleStatus.Approved ).FirstOrDefaultAsync();

            return View(artcles);
        }
    }
}
