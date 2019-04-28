using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AjaxController(
            ILogger<AccountController> logger,
            ApplicationDbContext context,
            IFileIOHelper ioHelper,
            IConfiguration config
           )
        {
            _io_helper = ioHelper;
            _logger = logger;
            _context = context;
            _config = config;
        }

        public string ImageUploadPath => _config.GetValue<string>("img_upload_path");

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
    }
}
