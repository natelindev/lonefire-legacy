using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Authorization;
using lonefire.Data;
using lonefire.Models;
using lonefire.Models.ArticleViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;

namespace lonefire.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AjaxController _ajax;
        private readonly IAuthorizationService _aus;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CommentController _commentController;
        private readonly UserController _userController;
        private readonly IToaster _toaster;

        public ArticleController(
        ApplicationDbContext context,
            AjaxController ajax,
            IAuthorizationService aus,
            UserManager<ApplicationUser> userManager,
            CommentController commentController,
            UserController userController,
            IToaster toaster
            )
        {
            _aus = aus;
            _userManager = userManager;
            _context = context;
            _commentController = commentController;
            _userController = userController;
            _ajax = ajax;
            _toaster = toaster;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Billboard()
        {
            var article = await _context.Article.OrderByDescending(a => a.AddTime)
                .FirstOrDefaultAsync(m => m.Title.Contains("公告"));
            return View(article);
        }

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

            article.Author = _userController.GetNickNameAsync(article.Author).Result.Value;

            //Get Comments
            ViewData["Comments"] = _commentController.GetAllCommentsAsync(article.ArticleID).Result.Value;

            return View(article);
        }

        [HttpGet]
        [AllowAnonymous]
        [ExactQueryParam("Tag")]
        public async Task<IActionResult> List(int? Tag)
        {
            if (Tag == null)
            {
                return NotFound();
            }

            List<Article> articles = new List<Article>();

            try
            {
                articles = await _context.Article.Where(a => a.Tag.Contains(Tag.ToString()) && a.Status == ArticleStatus.Approved).ToListAsync();

            }
            catch (Exception)
            {
                _toaster.ToastError("读取文章列表失败");
            }

            return View(articles);
        }

        // GET: Article
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var articles = from a in _context.Article
                           select a;

            var isAuthorized = User.IsInRole(Constants.AdministratorsRole);

            var currentUserId = _userManager.GetUserId(User);

            // Only show your own Articles unless you are admin
            if (!isAuthorized)
            {
                articles = articles.Where(a => a.Author == currentUserId);
            }

            foreach (var a in articles)
            {
                var user = await _userManager.FindByIdAsync(a.Author);
                a.Author = _userController.GetNickNameAsync(a.Author).Result.Value;
            }

            var res = await articles.ToListAsync();
            return View(res);
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

            article.Author = _userController.GetNickNameAsync(article.Author).Result.Value;

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
        public async Task<IActionResult> Create(Article article)
        {
            if (ModelState.IsValid)
            {
                article.Author = _userManager.GetUserId(User);
                var isAuthorized = await _aus.AuthorizeAsync(User, article,
                                                ArticleOperations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }

                var canApprove = await _aus.AuthorizeAsync(User,
                                                article,
                                                ArticleOperations.Approve);

                if (canApprove.Succeeded)
                {
                    //Only Mod can change Article Author & Does not need Approving
                    var uid = _userManager.GetUserId(User);
                    var user = await _userManager.FindByIdAsync(uid);

                    article.Author = article.Author ?? (user.Name ?? user.UserName);
                    article.Status = ArticleStatus.Approved;
                }
                else
                {
                    //Use current user as author
                    article.Author = _userManager.GetUserId(User);
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

                    //Only admin can change author
                    if (User.IsInRole(Constants.AdministratorsRole))
                    {
                        await TryUpdateModelAsync(articleToUpdate, "",
                         a => a.Title, a => a.Content, a => a.Author, a => a.MediaSerialized
                        );
                    }
                    else
                    {
                        await TryUpdateModelAsync(articleToUpdate, "",
                         a => a.Title, a => a.Content, a => a.MediaSerialized
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

                    await _context.SaveChangesAsync();
                    _toaster.ToastSuccess("文章更新成功");
                }
                catch (DbUpdateConcurrencyException)
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

            //Delete the related comments
            _commentController.DeleteAllCommentsAsync((int)id);

            _context.Article.Remove(article);

            await _context.SaveChangesAsync();
            _toaster.ToastSuccess("文章删除成功");

            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ArticleID == id);
        }

        #region Helper

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

        #endregion
    }
}
