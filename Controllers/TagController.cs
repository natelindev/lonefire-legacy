using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Models.ArticleViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    [Authorize(Roles = "Administrator")]
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tag.ToListAsync();
            ViewData["RawData"] = JsonConvert.SerializeObject(tags);
            return View(tags);
        }

        [HttpGet]
        [AllowAnonymous]
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
                
            var artcles = await _context.Article.Where(a => a.Tag.Contains(tagToBeFound.TagName) && a.Status == ArticleStatus.Approved ).ToListAsync();

            ViewData["Tag"] = tagToBeFound;
            return View(artcles);
        }

        [HttpGet]
        [AllowAnonymous]
        [ExactQueryParam("name")]
        public async Task<IActionResult> List(string name)
        {
            if (name == null)
            {
                _toaster.ToastError("未找到该标签");
                return NotFound();
            }

            var tagToBeFound = await _context.Tag.Where(t => t.TagName == name).FirstOrDefaultAsync();

            if (tagToBeFound == null)
            {
                _toaster.ToastError("未找到该标签");
                return NotFound();
            }

            var artcles = await _context.Article.Where(a => a.Tag.Contains(tagToBeFound.TagName) && a.Status == ArticleStatus.Approved).ToListAsync();

            ViewData["Tag"] = tagToBeFound;

            return View(artcles);
        }

        [HttpGet]
        public IActionResult Manage()
        {
            return View();
        }

        //Get Function Overloading Helper
        public class ExactQueryParamAttribute : Attribute, IActionConstraint
        {
            private readonly string[] keys;

            public ExactQueryParamAttribute(params string[] keys)
            {
                this.keys = keys;
            }

            public int Order => 0;

            public bool Accept(ActionConstraintContext context)
            {
                var query = context.RouteContext.HttpContext.Request.Query;
                return query.Count == keys.Length && keys.All(query.ContainsKey);
            }
        }
    }
}
