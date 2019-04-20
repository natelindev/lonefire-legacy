using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using lonefire.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using lonefire.Data;
using lonefire.Models.ArticleViewModels;
using lonefire.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using lonefire.Authorization;

namespace lonefire.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserController _userController;

        public HomeController(
            ILogger<AccountController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            UserController userController
        )
        {
            _userManager = userManager;
            _userController = userController;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<Article> articles = new List<Article>();
            var article = await _context.Article.OrderByDescending(a => a.AddTime)
                .FirstOrDefaultAsync(m => m.Title.Contains("公告"));
            try
            {
                articles = await _context.Article.FromSql("SELECT TOP 6 * FROM Articles WHERE Title NOT LIKE N'%公告%' AND STATUS = '1' ORDER BY ArticleID DESC").AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                TempData.PutString(Constants.ToastMessage, "读取文章列表失败");
            }
            foreach(var a in articles)
            {
                a.Author = _userController.GetNickNameAsync(a.Author).Result.Value;
            }
            articles.Add(article);

            return View(articles);
        }

        public IActionResult Privacy()
        {
            return View();
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
