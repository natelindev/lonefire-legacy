using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lonefire.Models.ManageViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "用户名")]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "昵称")]
        [StringLength(32, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Name { get; set; }

        [Display(Name = "个人描述")]
        [StringLength(256, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Description { get; set; }

        [Display(Name = "头像")]
        [StringLength(256, ErrorMessage = " {0} 名称长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Avatar { get; set; }
    }
}
