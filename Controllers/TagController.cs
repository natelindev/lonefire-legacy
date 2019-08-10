using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models.ArticleViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    [Authorize(Roles = Constants.AdministratorsRole)]
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
        public async Task<IActionResult> List(int? id, int page = 1)
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

            PaginatedList<Article> articles = new PaginatedList<Article>();
            try
            {
                IQueryable<Article> articleIQ = _context.Article
                .Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Tag.Contains(tagToBeFound.TagName) && a.Status == ArticleStatus.Approved)
                .OrderByDescending(a => a.AddTime);

                articles = await PaginatedList<Article>.CreateAsync(articleIQ.AsNoTracking(), page, Constants.TagPageCap);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取文章列表失败");
            }

            foreach (var a in articles)
            {
                //a.Author = await _userController.GetNickNameAsync(a.Author);
                if (a.Content != null)
                {
                    a.Content = LF_MarkdownParser.ParseAsPlainText(a.Content);
                    a.Content = a.Content.Substring(0, Math.Min(a.Content.Length, Constants.FrontPageWordCount));
                }
            }

            ViewData["Tag"] = tagToBeFound;
            return View(articles);
        }

        [HttpGet]
        [AllowAnonymous]
        [ExactQueryParam("name")]
        public async Task<IActionResult> List(string name,int page = 1)
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

            PaginatedList<Article> articles = new PaginatedList<Article>();
            try
            {
                IQueryable<Article> articleIQ = _context.Article
                .Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Tag.Contains(name) && a.Status == ArticleStatus.Approved)
                .OrderByDescending(a => a.AddTime);

                articles = await PaginatedList<Article>.CreateAsync(articleIQ.AsNoTracking(), page, Constants.TagPageCap);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取文章列表失败");
            }

            foreach (var a in articles)
            {
                //a.Author = await _userController.GetNickNameAsync(a.Author);
                if (a.Content != null)
                {
                    a.Content = LF_MarkdownParser.ParseAsPlainText(a.Content);
                    a.Content = a.Content.Substring(0, Math.Min(a.Content.Length, Constants.FrontPageWordCount));
                }
            }

            ViewData["Tag"] = tagToBeFound;
            return View(articles);
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
