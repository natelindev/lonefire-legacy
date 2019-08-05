
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Extensions;
using lonefire.Models;
using lonefire.Models.ArticleViewModels;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    [Authorize(Roles = Constants.AdministratorsRole)]
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IToaster _toaster;

        public UserController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
            IToaster toaster
            )
        {
            _toaster = toaster;
            _userManager = userManager;
            _context = context;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = await _context.Users.Where(u => u.Id != _userManager.GetUserId(User)).ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetUserInfo(string data)
        {
            ApplicationUser targetUser = null;
            if (!string.IsNullOrWhiteSpace(data))
            {
                return NotFound();
            }
            if (Guid.TryParse(data, out Guid dump))
            {
                //Is GUID
                targetUser = await _userManager.FindByIdAsync(data);
            }
            else
            {
                targetUser = await _userManager.FindByNameAsync(data);
            }
            return JsonConvert.SerializeObject(targetUser);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _toaster.ToastError("获取用户失败");
                return NotFound();
            }

            return View(user);
        }

        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _toaster.ToastError("获取用户失败");
                return NotFound();
            }

            if (ModelState.IsValid) { 
                try
                {
                    await TryUpdateModelAsync(user, "",
                             u => u.Name,u => u.Name,u=> u.Email
                        );
                    await _context.SaveChangesAsync();

                    _toaster.ToastSuccess("编辑用户成功");
                }
                catch (DbUpdateException)
                {
                    _toaster.ToastError("编辑用户失败");
                }
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _toaster.ToastError("获取用户失败");
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public async Task<ActionResult<TimeSpan>> GetLastAdminLoginTime()
        {

            var user = await _userManager.FindByNameAsync(Constants.AdminName);
            if (user == null)
            {
                _toaster.ToastError("管理员不存在");
                return NotFound();
            }
            return (DateTimeOffset.UtcNow - (user.LastLoginDate??DateTimeOffset.UtcNow));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                _toaster.ToastError("获取用户失败");
                return NotFound();
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _toaster.ToastSuccess("删除用户成功");
            }
            catch (DbUpdateException)
            {
                _toaster.ToastError("删除用户失败");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id,DateTime LockoutEnd)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _toaster.ToastError("获取用户失败");
                return NotFound();
            }

            try
            {
                await TryUpdateModelAsync(user, "",
                         u => u.LockoutEnd
                    );
                await _context.SaveChangesAsync();
                _toaster.ToastSuccess("锁定用户成功: 至" + LockoutEnd.ToLocalTime()+"结束");
            }
            catch (DbUpdateException)
            {
                _toaster.ToastError("锁定用户失败");
            }

            return RedirectToAction(nameof(Index));
        }

        #region Helper

        public async Task<string> GetNickNameAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            string res;
            if (user != null)
            {
                res = (user.Name ?? user.UserName);
                if (await _userManager.IsInRoleAsync(user, Constants.AdministratorsRole))
                {
                    res += Constants.AdminTag;
                }
            }
            else
            {
                res = id ?? Constants.EmptyUserTag;
            }

            return res;
        }

        public async Task<string> GetAvatarAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return (user == null? Constants.DefaultAvatar : user.Avatar);
        }

        #endregion

    }
}
