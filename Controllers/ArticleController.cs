﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using lonefire.Authorization;
using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models;
using lonefire.Models.ArticleViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToaster _toaster;
        private readonly IAuthorizationService _aus;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CommentController _commentController;
        private readonly UserController _userController;
        private readonly ImageController _imageController;
        private readonly IFileIOHelper _io_Helper;
        private readonly IConfiguration _config;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(
        ApplicationDbContext context,
            IAuthorizationService aus,
            UserManager<ApplicationUser> userManager,
            CommentController commentController,
            UserController userController,
            ImageController imageController,
            IFileIOHelper io_Helper,
            IToaster toaster,
            IConfiguration config,
            ILogger<ArticleController> logger
            )
        {
            _aus = aus;
            _userManager = userManager;
            _context = context;
            _commentController = commentController;
            _userController = userController;
            _io_Helper = io_Helper;
            _toaster = toaster;
            _config = config;
            _logger = logger;
            _imageController = imageController;
        }

        public string ImageUploadPath => _config.GetValue<string>("img_upload_path");

        [HttpGet, ActionName("View")]
        [AllowAnonymous]
        public async Task<IActionResult> ArticleView(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .SingleOrDefaultAsync(m => m.ArticleID == id);

            if (article == null)
            {
                return NotFound();
            }

            //Get Comments
            ViewData["Comments"] = await _commentController.GetAllCommentsAsync(article.ArticleID);

            article.ViewCount++;
            await _context.SaveChangesAsync();

            article.Content = LF_MarkdownParser.Parse(article.Content, ImageUploadPath + article.Title + '/');

            article.Author = await _userController.GetNickNameAsync(article.Author);

            ViewData["HeaderImg"] = ImageUploadPath+ article.Title +'/' + article.HeaderImg;

            try
            {

                List<ArticleIndexVM> articles = await _context.Article
                    .Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Status == ArticleStatus.Approved)
                    .OrderByDescending(a => a.AddTime)
                    .Select(a => new ArticleIndexVM
                    {
                        ArticleID = a.ArticleID,
                        Title = a.Title,
                        Author = a.Author,
                        Tag = a.Tag,
                        AddTime = a.AddTime,
                        Status = a.Status
                    })
                    .AsNoTracking()
                    .ToListAsync();

                int idx = articles.FindIndex(a => a.ArticleID == id);

                if (idx - 1 >= 0)
                {
                    ViewData["Prev"] = articles[idx - 1];
                }
                if (idx + 1 < articles.Count)
                {
                    ViewData["Next"] = articles[idx + 1];
                }
            }
            catch (Exception)
            {
                _toaster.ToastError("读取文章列表失败");
            }
            ViewData["AboutMe"] = await _userController.GetUserInfo(Constants.AdminName);
            ViewData["Related"] = await GetRelatedArticles(article);
            return View(article);
        }

        // GET: Article
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var articles = _context.Article
                    .Select(a => new ArticleIndexVM
                    {
                        ArticleID = a.ArticleID,
                        Title = a.Title,
                        Author = a.Author,
                        Tag = a.Tag,
                        AddTime = a.AddTime,
                        Status = a.Status
                    });

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole);

            var currentUserId = _userManager.GetUserId(User);

            // Only show your own Articles unless you are admin
            if (!isAuthorized)
            {
                articles = articles.Where(a => a.Author == currentUserId);
            }

            articles = articles.OrderByDescending(a => a.ArticleID).Take(Constants.DTPageCap);

            var res = await articles.ToListAsync();

            foreach (var a in res)
            {
                var user = await _userManager.FindByIdAsync(a.Author);
                a.Author = await _userController.GetNickNameAsync(a.Author);
            }

            return View(res);
        }

        // GET: Article
        [HttpGet]
        public async Task<IActionResult> AjaxGetArticles()
        {
            var articles = _context.Article
                    .Select(a => new ArticleIndexVM
                    {
                        ArticleID = a.ArticleID,
                        Title = a.Title,
                        Author = a.Author,
                        Tag = a.Tag,
                        AddTime = a.AddTime,
                        Status = a.Status
                    });

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole);

            var currentUserId = _userManager.GetUserId(User);

            // Only show your own Articles unless you are admin
            if (!isAuthorized)
            {
                articles = articles.Where(a => a.Author == currentUserId);
            }

            articles = articles.OrderByDescending(a => a.ArticleID).Skip(Constants.DTPageCap);

            var res = await articles.ToListAsync();

            foreach (var a in res)
            {
                var user = await _userManager.FindByIdAsync(a.Author);
                a.Author = await _userController.GetNickNameAsync(a.Author);
            }

            var jsonString = JsonConvert.SerializeObject(new { data = res });
            return Content(jsonString, "application/json");
        }

        // GET: Article/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .SingleOrDefaultAsync(m => m.ArticleID == id);

            if (article == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, article,
                                                  ArticleOperations.Read);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            article.Author = await _userController.GetNickNameAsync(article.Author);

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id, ArticleStatus status)
        {

            var articleToUpdate = await _context.Article.SingleOrDefaultAsync(a => a.ArticleID == id);

            if (articleToUpdate == null)
            {
                return NotFound();
            }

            var opeartion = (status == ArticleStatus.Approved ? ArticleOperations.Approve : ArticleOperations.Reject);
            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, articleToUpdate,
                                                  opeartion);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await TryUpdateModelAsync<Article>(articleToUpdate, "",
                         a => a.Status
                    );

                    await _context.SaveChangesAsync();
                    _toaster.ToastSuccess("文章审核成功");
                }
                catch (DbUpdateConcurrencyException)
                {
                    _toaster.ToastError("文章审核失败");

                    if (!ArticleExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(articleToUpdate);
        }


        // GET: Article/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Article/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Author,Tag,Content")]Article article,ArticleStatus? Status, IFormFile headerImg,IList<IFormFile> contentImgs)
        {
            if (ModelState.IsValid)
            {
                var isAuthorized = await _aus.AuthorizeAsync(User, article,
                                                ArticleOperations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }

                var canApprove = await _aus.AuthorizeAsync(User,
                                                article,
                                                ArticleOperations.Approve);

                var uid = _userManager.GetUserId(User);

                if (canApprove.Succeeded)
                {
                    //Only Mod can change Article Author & Does not need Approving
                    article.Author = article.Author ?? uid;
                    article.Status = Status ?? ArticleStatus.Approved;
                }
                else
                {
                    //Use current user as author
                    article.Author = uid;
                }

                //save the Images
                if (headerImg != null || contentImgs.Count > 0)
                {
                    List<string> ArticleImgs = new List<string>();
                    if (headerImg != null)
                    {
                        
                        article.HeaderImg = headerImg.FileName;
                        ArticleImgs.Add(headerImg.FileName);
                        var res = await _imageController.CreateAsync(article.Title, new List<IFormFile> { headerImg });
                        if (res)
                        {
                            _toaster.ToastSuccess("标题图片上传成功");
                        }
                        else
                        {
                            _toaster.ToastError("标题图片上传失败");
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    if(contentImgs != null && contentImgs.Count > 0)
                    {
                        foreach (var img in contentImgs)
                        {
                            ArticleImgs.Add(img.FileName);
                        }
                        var res = await _imageController.CreateAsync(article.Title, contentImgs);
                        if (res)
                        {
                            _toaster.ToastSuccess("内容图片上传成功");
                        }
                        else
                        {
                            _toaster.ToastError("内容图片上传失败");
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    if (ArticleImgs.Count > 0)
                        article.MediaSerialized = JsonConvert.SerializeObject(ArticleImgs);
                }

                //Add the tags
                if (!string.IsNullOrWhiteSpace(article.Tag))
                {
                    var tags = article.Tag.Split(',').ToList();
                    foreach(var tag in tags)
                    {
                        var old_tag = await _context.Tag.Where(t => t.TagName == tag).FirstOrDefaultAsync();
                        if (old_tag != null)
                        {
                            //existing tag
                            old_tag.TagCount++;
                        }
                        else{
                            //new tag
                            _context.Add(new Tag(){ TagName = tag, TagCount = 1 });
                        }
                    }
                }

                _context.Add(article);
                await _context.SaveChangesAsync();
                _toaster.ToastSuccess("文章创建成功");
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: Article/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleID == id);

            if (article == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, article,
                                                  ArticleOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(article);
        }

        // POST: Article/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleToUpdate = await _context.Article.SingleOrDefaultAsync(a => a.ArticleID == id);

            if (articleToUpdate == null)
            {
                return NotFound();
            }

            var isAuthorized = await _aus.AuthorizeAsync(
                                                  User, articleToUpdate,
                                                  ArticleOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Rename the dir when Title Changes
                    //TODO: use regex to validate this Title
                    if(articleToUpdate.Title != HttpContext.Request.Form["Title"])
                    {
                        _io_Helper.MoveImgDir( articleToUpdate.Title, HttpContext.Request.Form["Title"]);
                    }

                    //Only admin can change author
                    if (User.IsInRole(Constants.AdministratorsRole))
                    {
                        await TryUpdateModelAsync(articleToUpdate, "",
                         a => a.Title, a => a.Content, a => a.Tag, a => a.Author, a => a.MediaSerialized
                        );
                    }
                    else
                    {
                        await TryUpdateModelAsync(articleToUpdate, "",
                         a => a.Title, a => a.Content,a => a.Tag, a => a.MediaSerialized
                        );
                    }

                    if (articleToUpdate.Status == ArticleStatus.Approved)
                    {
                        // Reset to submitted status after update(if not mod)
                        var canApprove = await _aus.AuthorizeAsync(User,
                                                articleToUpdate,
                                                ArticleOperations.Approve);

                        if (!canApprove.Succeeded)
                        {
                            articleToUpdate.Status = ArticleStatus.Submitted;
                        }
                    }

                    //prevent empty author
                    articleToUpdate.Author = articleToUpdate.Author ?? _userManager.GetUserId(User);

                    //Tag Update
                    if (articleToUpdate.Tag != HttpContext.Request.Form["Tag"])
                    {
                        List<string> old_tags = new List<string>();
                        List<string> new_tags = new List<string>();
                        if (!string.IsNullOrWhiteSpace(articleToUpdate.Tag))
                        {
                            old_tags = articleToUpdate.Tag.Split(',').ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(HttpContext.Request.Form["Tag"]))
                        {
                            new_tags = ((string)HttpContext.Request.Form["Tag"]).Split(',').ToList();
                        }
                        foreach (var o_tag in old_tags)
                        {
                            //Check if in the new
                            var res = new_tags.FirstOrDefault(t => t == o_tag);
                            if (res == null)
                            {
                                //Not in new
                                //Reduce tagCount
                                var tag = await _context.Tag.Where(t => t.TagName == o_tag).FirstOrDefaultAsync();
                                --tag.TagCount;
                                if (tag.TagCount == 0)
                                {
                                    // Delete on count to zero
                                    _context.Tag.Remove(tag);
                                }
                            }
                            else
                            {
                                //Remove from new
                                new_tags.Remove(res);
                            }
                        }
                        foreach (var n_tag in new_tags)
                        {
                            //Add all new tags
                            _context.Add(new Tag() { TagName = n_tag, TagCount = 1 });
                        }
                    }

                    await _context.SaveChangesAsync();
                    _toaster.ToastSuccess("文章更新成功");
                }
                catch (Exception)
                {
                    _toaster.ToastError("文章更新失败");

                    if (!ArticleExists(id ?? 0))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(articleToUpdate);
        }

        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleID == id);

            var isAuthorized = await _aus.AuthorizeAsync(
                                                 User, article,
                                                 ArticleOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            //delete Image in db
            var res = await _imageController.DeleteByPath(article.Title);
            if (!res)
            {
                _toaster.ToastError("删除数据库图片时失败");
                return RedirectToAction(nameof(Index));
            }

            //delete the Images
            _io_Helper.DeleteImgDir(article.Title);
            
            //Comments were casecade deleted.

            //Reduce Tag Count
            if (!string.IsNullOrWhiteSpace(article.Tag))
            {
                var tags = article.Tag.Split(',').ToList();
                foreach (var tag in tags)
                {
                    var ta = await _context.Tag.Where(t => t.TagName == tag).FirstOrDefaultAsync();
                    ta.TagCount--;
                    if (ta.TagCount == 0)
                    {
                        _context.Tag.Remove(ta);
                    }
                }
            }

            //delete the article
            _context.Article.Remove(article);

            await _context.SaveChangesAsync();
            _toaster.ToastSuccess("文章删除成功");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AjaxLikeArticle(int? articleID)
        {
            if (articleID == null)
            {
                return NotFound();
            }

            var articleToUpdate = await _context.Article.SingleOrDefaultAsync(a => a.ArticleID == articleID);

            if (articleToUpdate == null)
            {
                return NotFound();
            }

            ++articleToUpdate.LikeCount;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogInformation("User Like Article Failed For " + articleID);
                _logger.LogInformation(e.Message);
            }

            return new OkResult();
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ArticleID == id);
        }

        #region Helper

        public async Task<List<Article>> GetRelatedArticles(Article article)
        {
            //Randomly pick 3 articles
            //TODO: Actually implement this.
            var random = new Random();
            return await _context.Article.Where(a => !a.Title.Contains(Constants.ReservedTag) && a.Title != article.Title && a.Status == ArticleStatus.Approved).OrderBy(s => random.Next()).Take(3).ToListAsync();
        }

        #endregion
    }
}
