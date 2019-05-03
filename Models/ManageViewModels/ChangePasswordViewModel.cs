using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lonefire.Models.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "请填写旧密码")]
        [DataType(DataType.Password)]
        [Display(Name = "旧密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "请填写新密码")]
        [StringLength(64, ErrorMessage = "{0} 长度必须在 {2} 到 {1} 之间", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请再次填写新密码")]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "两次输入密码不相符。")]
        public string ConfirmPassword { get; set; }

    }
}
