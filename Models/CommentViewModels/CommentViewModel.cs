using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace lonefire.Models.CommentViewModels
{
    public class CommentViewModel
    {
        [Display(Name = "编号")]
        public int ID { get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "作者")]
        [StringLength(16, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Author { get; set; }

        [Display(Name = "发布时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd HH:mm}")]
        public DateTime AddTime { get; set; }

        public List<CommentViewModel> Childs { get; set; }
    }
}
