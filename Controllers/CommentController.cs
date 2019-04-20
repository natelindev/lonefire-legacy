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

namespace lonefire.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
        {
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
