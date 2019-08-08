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
using System.Text.RegularExpressions;
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
                .Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Status == ArticleStatus.Approved)
                .OrderByDescending(a => a.AddTime);

                articles = await PaginatedList<Article>.CreateAsync(articleIQ.AsNoTracking(), page, Constants.PageCap);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取文章列表失败");
            }
            foreach(var a in articles)
            {
                //a.Author = await _userController.GetNickNameAsync(a.Author);
                if(a.Content != null)
                {
                    a.Content = LF_MarkdownParser.ParseAsPlainText(a.Content);
                    a.Content = a.Content.Substring(0, Math.Min(a.Content.Length, Constants.FrontPageWordCount));
                }
            }
            ViewData["AboutMe"] = await _userController.GetUserInfo(Constants.AdminName);
            ViewData["Friends"] = await _context.Friend.OrderBy(f => f.FriendID).ToListAsync();
            ViewData["Tags"] = await _context.Tag.OrderByDescending(t => t.TagCount).Take(6).ToListAsync();
            return View(articles);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Notes(int page = 1)
        {
            var isAuthorized = User.IsInRole(Constants.AdministratorsRole);

            PaginatedList<Note> notes = new PaginatedList<Note>();
            try
            {
                IQueryable<Note> noteIQ = _context.Note
                .OrderByDescending(a => a.AddTime);

                if (!isAuthorized)
                {
                    noteIQ = noteIQ.Where(n => n.Status == NoteStatus.Public);
                }
                notes = await PaginatedList<Note>.CreateAsync(noteIQ.AsNoTracking(), page, Constants.PageCap);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取动态列表失败");
            }

            foreach (var note in notes)
            {
                note.Content = LF_MarkdownParser.Parse(note.Content, ImageUploadPath + note.Title + '/');
            }

            return View(notes);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string keyword, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                keyword = TempData.Peek<string>(Constants.LastKeyword);
            }
            else
            {
                TempData.Put(Constants.LastKeyword, keyword);
            }
            //Keyword shouldn't be null now
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return NotFound();
            }
            var keywords = keyword.Split(" ");
            var L_keywords = keywords.Select(k => (k.Length > 1 ? k.Substring(1, k.Length - 1) : k));
            var R_keywords = keywords.Select(k => (k.Length > 1 ? k.Substring(0, k.Length - 2) : k));
            var words = keywords.Union(L_keywords).Union(R_keywords).ToArray();
            string pattern = '(' + string.Join('|', words) + ')';
            var articlesIQ = _context.Article
                .Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Status == ArticleStatus.Approved)
                .OrderByDescending(a => a.AddTime);
            var resultIQ = articlesIQ
                .OrderByDescending(a =>
                Regex.Matches(a.Title, pattern, RegexOptions.IgnoreCase).Count * 3 //Title weight 3
                + Regex.Matches(a.Tag, pattern, RegexOptions.IgnoreCase).Count * 2 //Tag weight 2
                + Regex.Matches(a.Content, pattern, RegexOptions.IgnoreCase).Count) //Content weight 1
                .Take(Constants.SearchPageCap * 3);
            var results = await PaginatedList<Article>.CreateAsync(resultIQ.AsNoTracking(), page, Constants.SearchPageCap);
            foreach (var a in results)
            {
                //a.Author = await _userController.GetNickNameAsync(a.Author);
                if (a.Content != null)
                {
                    a.Content = LF_MarkdownParser.ParseAsPlainText(a.Content);
                    a.Content = a.Content.Substring(0, Math.Min(a.Content.Length, Constants.FrontPageWordCount));
                }
            }
            ViewData["Keyword"] = "\"" + keyword +"\""+ " 的搜索结果";
            return View(results);
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
            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Title == Constants.ReservedTag + "关于");
            if(article == null)
            {
                article = new Article();
                _toaster.ToastWarning("暂时没有 关于 的内容");
            }
            ViewData["AboutMe"] = await _userController.GetUserInfo(Constants.AdminName);
            ViewData["Comments"] = await _commentController.GetAllCommentsAsync(article.ArticleID);
            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> MessageBoard()
        {
            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Title == Constants.ReservedTag + "留言板");
            if (article == null)
            {
                article = new Article();
                _toaster.ToastWarning("暂时没有 留言板 的内容");
            }
            ViewData["Comments"] = await _commentController.GetAllCommentsAsync(article.ArticleID);
            return View(article);
        }

        [HttpGet]
        public IActionResult Images()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Friends(int page = 1)
        {
            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Title == Constants.ReservedTag + "友链");
            if (article == null)
            {
                article = new Article();
                _toaster.ToastWarning("暂时没有 友链 的内容");
            }
            ViewData["Comments"] = await _commentController.GetAllCommentsAsync(article.ArticleID);
            ViewData["Friend"] = article;

            PaginatedList<Friend> friends = new PaginatedList<Friend>();
            try
            {
                IQueryable<Friend> friendIQ = _context.Friend.OrderBy(f => f.FriendID);

                friends = await PaginatedList<Friend>.CreateAsync(friendIQ.AsNoTracking(), page, Constants.FriendPageCap);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取友链列表失败");
            }
            return View(friends);
        }

        [HttpGet]
        public async Task<IActionResult> Timeline(int page = 1)
        {
            PaginatedList<Article> articles = new PaginatedList<Article>();
            try
            {
                IQueryable<Article> articleIQ = _context.Article
                .Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Status == ArticleStatus.Approved)
                .OrderByDescending(a => a.AddTime);

                articles = await PaginatedList<Article>.CreateAsync(articleIQ.AsNoTracking(), page, Constants.PageCap);
            }
            catch (Exception)
            {
                _toaster.ToastError("读取 时间线 文章列表失败");
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
