using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models;
using lonefire.Models.ArticleViewModels;
using lonefire.Models.NoteViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace lonefire.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly CommentController _commentController;
        private readonly UserController _userController;
        private readonly IToaster _toaster;
        private readonly IConfiguration _config;

        public string ImageUploadPath => _config.GetValue<string>("img_upload_path");

        public HomeController(
            ILogger<AccountController> logger,
            ApplicationDbContext context,
            CommentController commentController,
            UserController userController,
            IToaster toaster,
            IConfiguration config
        )
        {
            _commentController = commentController;
            _userController = userController;
            _context = context;
            _logger = logger;
            _toaster = toaster;
            _config = config;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            PaginatedList<Article> articles = new PaginatedList<Article>();
            try
            {
                IQueryable<Article> articleIQ = _context.Article
                .Where(a => !a.Title.Contains("「LONEFIRE」") && a.Status == ArticleStatus.Approved)
                .OrderByDescending(a => a.AddTime);

                articles = await PaginatedList<Article>.CreateAsync(articleIQ.AsNoTracking(), page, Constants.PageCount);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取文章列表失败");
            }
            foreach(var a in articles)
            {
                a.Author = await _userController.GetNickNameAsync(a.Author);
                if(a.Content != null)
                {
                    a.Content = LF_MarkdownParser.ParseAsPlainText(a.Content);
                    a.Content = a.Content.Substring(0, Math.Min(a.Content.Length, 100));
                }
            }
            ViewData["AboutMe"] = await _context.Article.FirstOrDefaultAsync(m => m.Title == "「LONEFIRE」首页关于");
            ViewData["Friends"] = await _context.Article.FirstOrDefaultAsync(m => m.Title == "「LONEFIRE」首页友链");
            ViewData["Tags"] = await _context.Tag.OrderByDescending(t => t.TagCount).Take(6).ToListAsync();
            return View(articles);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Notes(int page = 1)
        {
            PaginatedList<Note> notes = new PaginatedList<Note>();
            try
            {
                IQueryable<Note> noteIQ = _context.Note
                .Where(n => n.Status == NoteStatus.Public)
                .OrderByDescending(a => a.AddTime);

                notes = await PaginatedList<Note>.CreateAsync(noteIQ.AsNoTracking(), page, Constants.PageCount);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取笔记列表失败");
            }

            foreach (var note in notes)
            {
                note.Content = LF_MarkdownParser.Parse(note.Content, ImageUploadPath + note.Title + '/');
            }

            return View(notes);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Portfolio()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Papers()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> About()
        {
            var article = await _context.Article.OrderByDescending(a => a.AddTime)
                .FirstOrDefaultAsync(m => m.Title == "「LONEFIRE」关于");
            if(article == null)
            {
                article = new Article();
                _toaster.ToastWarning("暂时没有 关于我 的内容");
            }
            ViewData["Comments"] = await _commentController.GetAllCommentsAsync(article.ArticleID);
            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> Archives(int page = 1)
        {
            PaginatedList<Article> articles = new PaginatedList<Article>();
            try
            {
                IQueryable<Article> articleIQ = _context.Article
                .Where(a => !a.Title.Contains("「LONEFIRE」") && a.Status == ArticleStatus.Approved)
                .OrderByDescending(a => a.AddTime);

                articles = await PaginatedList<Article>.CreateAsync(articleIQ.AsNoTracking(), page, Constants.PageCount);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取归档文章列表失败");
            }
            return View(articles);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
                if (statusFeature != null)
                {
                    _logger.LogWarning("Handled " + statusCode + " for url: {OriginalPath}", statusFeature.OriginalPath);
                }
                this.HttpContext.Response.StatusCode = statusCode.Value;
            }
            else
            {
                statusCode = HttpContext.Response.StatusCode;
            }

            switch (statusCode)
            {
                case 400:
                    ViewData["Icon"] = "fa fa-ban text-danger";
                    ViewData["Title"] = "错误请求";
                    ViewData["Description"] = "您的浏览器发出了服务器无法识别的格式错误的请求，如果经常看到此页面，请通知系统管理员。";
                    break;
                case 401:
                    ViewData["Icon"] = "fa fa-ban text-danger";
                    ViewData["Title"] = "未授权";
                    ViewData["Description"] = "对不起，该页面需要登录后才可查看，请点击上方登录按钮登录。";
                    break;
                case 403:
                    ViewData["Icon"] = "fa fa-exclamation-circle text-danger";
                    ViewData["Title"] = "禁止访问";
                    ViewData["Description"] = "对不起，该页面禁止访问。";
                    break;
                case 404:
                    ViewData["Icon"] = "fa fa-exclamation-circle text-danger";
                    ViewData["Title"] = "未找到该内容";
                    ViewData["Description"] = "对不起，该内容不存在，请检查您所输入的地址是否正确。";
                    break;
                default:
                    //handle 5xx errors
                    if (statusCode >= 500 && statusCode < 600)
                    {
                        ViewData["Icon"] = "fa fa-exclamation-circle text-danger";
                        ViewData["Title"] = "服务器内部错误";
                        ViewData["Description"] = "服务器在处理您的请求时发生了内部错误，如果经常看到此页面，请通知系统管理员。";
                    }
                    else
                    {
                        ViewData["Icon"] = "fa fa-exclamation-circle text-danger";
                        ViewData["Title"] = "未知错误";
                        ViewData["Description"] = "服务器在处理您的请求时发生了未知错误，如果经常看到此页面，请通知系统管理员。";
                    }
                    break;
            }

            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    StatusCode = (statusCode ?? 500)
                }
            );
        }
    }
}
