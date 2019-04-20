using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lonefire.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(64, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "两次输入密码不相符。")]
        public string ConfirmPassword { get; set; }
    }
}
