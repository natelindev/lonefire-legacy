using System;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace lonefire.Controllers
{
    public class AjaxController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileIOHelper _io_helper;
        private readonly IConfiguration _config;
        private readonly CommentController _commentController;

        public AjaxController(
            ILogger<AccountController> logger,
            ApplicationDbContext context,
            IFileIOHelper ioHelper,
            IConfiguration config,
            CommentController commentController
           )
        {
            _io_helper = ioHelper;
            _logger = logger;
            _context = context;
            _config = config;
            _commentController = commentController;
        }

        public string ImageUploadPath => _config.GetValue<string>("img_upload_path");

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
            }catch(Exception e)
            {
                _logger.LogInformation("User Like Article Failed For " + articleID);
                _logger.LogInformation(e.Message);
            }

            return new OkResult();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AjaxImgUpload(IFormFile upload)
        {

            string jsonString ="";

            string img_name = await _io_helper.SaveImgAsync(upload, upload.FileName, 256);

            if (string.IsNullOrEmpty(img_name))
            {
                jsonString = JsonConvert.SerializeObject(new { uploaded = false, error = new { message = "图片上传失败" } });
            }
            else
            {
                jsonString = JsonConvert.SerializeObject(new { uploaded = true, url = ImageUploadPath + img_name });
            }

            return Content(jsonString, "application/json");

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AjaxGetComments(int id)
        {
            var comments = await _commentController.GetAllCommentsAsync(id);
            var jsonString = JsonConvert.SerializeObject(comments);
            return Content(jsonString, "application/json");
        }
    }
}
