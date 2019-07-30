using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lonefire.Data;
using lonefire.Models.CommentViewModels;
using Microsoft.AspNetCore.Identity;
using lonefire.Models;
using lonefire.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace lonefire.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserController _userController;

        public CommentController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        UserController userController
            )
        {
            _userController = userController;
            _userManager = userManager;
            _context = context;
        }

        // GET: Comment
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comment.ToListAsync());
        }

        // GET: Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.CommentID == id);

            var user = await _userManager.FindByIdAsync(comment.Author);
            comment.Author = user.Name ?? user.UserName;

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleID,ParentID,Content")] Comment comment,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //自动设置作者
                comment.Author = _userManager.GetUserId(User);

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToLocal(returnUrl);
            }
            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> VisitorCreate([Bind("ArticleID,ParentID,Content,Author,Email,Blog")] Comment comment, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToLocal(returnUrl);
            }
            return RedirectToLocal(returnUrl);
        }

        // GET: Comment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ArticleID,ParentID,Content,Author,AddTime")] Comment comment)
        {
            if (id != comment.CommentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentID))
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
            return View(comment);
        }

        // GET: Comment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.CommentID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,string returnUrl)
        {
            var comment = await _context.Comment.FindAsync(id);
            var comments = await _context.Comment.Where(c => c.ArticleID == comment.ArticleID).ToListAsync();
            DeleteChildComments(comments, id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToLocal(returnUrl);
        }

        //Recursive child comment deleter
        public void DeleteChildComments(List<Comment> comments, int cid)
        {
            var cs = comments.Where(c => c.ParentID == cid);
            foreach(var c in cs)
            {
                DeleteChildComments(comments, c.CommentID);
                _context.Comment.Remove(c);
            }
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.CommentID == id);
        }

        #region Helpers

        public async Task<List<CommentViewModel>> GetAllCommentsAsync(int id)
        {
            //Get Comments
            List<Comment> comments = await _context.Comment.Where(c => c.ArticleID == id).ToListAsync();
            List<CommentViewModel> cvms = new List<CommentViewModel>();

            //Build up structure of comments
            foreach (var c in comments)
            {
                if (c.ParentID == null)
                {
                    var cvm = new CommentViewModel
                    {
                        CommentID = c.CommentID,
                        Content = LF_MarkdownParser.ParseWithoutStyle(c.Content),
                        Author = await _userController.GetNickNameAsync(c.Author),
                        AddTime = c.AddTime.ToLocalTime(),
                        Childs = await GetChildCommentsAsync(comments, c.CommentID),
                        Avatar = await _userController.GetAvatarAsync(c.Author)
                    };
                    cvms.Add(cvm);
                }
            }

            return cvms;
        }

        public async void DeleteAllCommentsAsync(int id)
        {
            //Delete the related comments 
            var comments = await _context.Comment.Where(c => c.ArticleID == id).ToListAsync();
            foreach (var c in comments)
            {
                DeleteChildComments(comments, c.CommentID);
                _context.Comment.Remove(c);
            }
        }

        //Recursive child comment fetcher
        public async Task<List<CommentViewModel>> GetChildCommentsAsync(List<Comment> comments, int cid)
        {
            List<Comment> child_comments = comments.Where(c => c.ParentID == cid).ToList();
            List<CommentViewModel> cvms = new List<CommentViewModel>();
            foreach (var c in child_comments)
            {
                var cvm = new CommentViewModel
                {
                    CommentID = c.CommentID,
                    Content = LF_MarkdownParser.ParseWithoutStyle(c.Content),
                    Author = await _userController.GetNickNameAsync(c.Author),
                    AddTime = c.AddTime.ToLocalTime(),
                    Childs = await GetChildCommentsAsync(comments, c.CommentID),
                    Avatar = await _userController.GetAvatarAsync(c.Author)
                };
                cvms.Add(cvm);
            }

            return cvms;
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #endregion
    }
}
