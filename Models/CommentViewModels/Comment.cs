using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using lonefire.Models.ArticleViewModels;

namespace lonefire.Models.CommentViewModels
{
    public class Comment
    {
        [Display(Name = "编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }

        [Display(Name = "所属文章ID")]
        [Required]
        public int ArticleID { get; set; }

        public virtual Article Article { get; set; }

        [Display(Name = "父评论ID")]
        public int? ParentID { get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "作者")]
        [StringLength(64, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Author { get; set; }

        [Display(Name = "发布时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd HH:mm}")]
        public DateTime AddTime { get; set; }

        [ForeignKey("ParentID")]
        public List<Comment> Comments { get; set; }
    }
}
