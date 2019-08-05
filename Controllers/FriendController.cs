using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lonefire.Data;
using lonefire.Models;
using lonefire.Services;

namespace lonefire.Controllers
{
    public class FriendController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToaster _toaster;

        public FriendController(ApplicationDbContext context, IToaster toaster
            )
        {
            _context = context;
            _toaster = toaster;
        }

        // GET: Friend
        public async Task<IActionResult> Index()
        {
            return View(await _context.Friend_1.ToListAsync());
        }

        // GET: Friend/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Friend_1
                .FirstOrDefaultAsync(m => m.FriendID == id);
            if (friend == null)
            {
                return NotFound();
            }

            return View(friend);
        }

        // GET: Friend/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Friend/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FriendID,URL,Description,Icon")] Friend friend)
        {
            if (ModelState.IsValid)
            {
                _context.Add(friend);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(HomeController.Friends), "Home");
            }
            return View(friend);
        }

        // GET: Friend/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Friend_1.FindAsync(id);
            if (friend == null)
            {
                return NotFound();
            }
            return View(friend);
        }

        // POST: Friend/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Friend.Where(c => c.FriendID == id).FirstOrDefaultAsync();
            if (friend == null)
            {
                _toaster.ToastError("获取评论失败");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await TryUpdateModelAsync(friend, "",
                             c => c.URL, c => c.Icon, c => c.Description
                        );
                    await _context.SaveChangesAsync();

                    _toaster.ToastSuccess("编辑友链成功");
                }
                catch (DbUpdateException)
                {
                    _toaster.ToastError("编辑友链失败");
                }
            }

            return View(friend);
        }

        // GET: Friend/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var friend = await _context.Friend_1
                .FirstOrDefaultAsync(m => m.FriendID == id);
            if (friend == null)
            {
                return NotFound();
            }

            return View(friend);
        }

        // POST: Friend/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var friend = await _context.Friend_1.FindAsync(id);
            _context.Friend_1.Remove(friend);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FriendExists(int id)
        {
            return _context.Friend_1.Any(e => e.FriendID == id);
        }
    }
}
