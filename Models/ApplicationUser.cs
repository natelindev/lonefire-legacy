using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace lonefire.Models
{
    //用户类
    public class ApplicationUser : IdentityUser
    {
        //昵称
        [StringLength(32, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Name { get; set; }
    }
}
