using lonefire.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace lonefire.Controllers
{
    public class AjaxController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public AjaxController(
            ILogger<AccountController> logger,
            ApplicationDbContext context
           )
        {
            _logger = logger;
            _context = context;
        }

    }
}
