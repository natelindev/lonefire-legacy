using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace lonefire.Models
{
    //用户类
    public class ApplicationUser : IdentityUser
    {

        [Display(Name = "昵称")]
        [StringLength(32, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Name { get; set; }

        [Display(Name = "个人描述")]
        [StringLength(256, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Description { get; set; }

        [Display(Name = "头像")]
        [StringLength(256, ErrorMessage = " {0} 名称长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Avatar { get; set; }

        [Display(Name = "上次访问时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTimeOffset? LastLoginDate { get; set; }
    }
}
