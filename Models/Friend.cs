using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lonefire.Models
{
    public class Friend
    {
        [Display(Name = "编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FriendID { get; set; }

        [Display(Name = "网址")]
        [StringLength(256, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string URL { get; set; }

        [Display(Name = "描述")]
        [StringLength(256, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Description { get; set; }

        [Display(Name = "图标")]
        [StringLength(256, ErrorMessage = " {0} 名称长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Icon { get; set; }
    }
}
