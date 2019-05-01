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
        public string Name { get; set; }

        [Display(Name = "个人描述")]
        public string Description { get; set; }
    }
}
