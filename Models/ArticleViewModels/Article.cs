﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using lonefire.Models.CommentViewModels;

namespace lonefire.Models.ArticleViewModels
{
    public class Article
    {
        [Display(Name ="编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleID { get; set; }

        [Display(Name = "标题")]
        [Required]
        [StringLength(64, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Title { get; set; }

        [Display(Name = "作者")]
        [StringLength(16, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Author { get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "标签")]
        public string Tag { get; set; }

        [Display(Name = "媒体文件")]
        public string MediaSerialized { get; set; }

        [Display(Name = "添加时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AddTime { get; set; }

        [Display(Name = "审核状态")]
        public ArticleStatus Status { get; set; }

        public List<Comment> Comments;
    }
}

public enum ArticleStatus
{
    Submitted,
    Approved,
    Rejected
}