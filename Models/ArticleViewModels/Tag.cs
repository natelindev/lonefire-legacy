using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lonefire.Models.ArticleViewModels
{
    public class Tag
    {
        [Display(Name = "编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagID { get; set; }

        [Display(Name = "标签名称")]
        [Required]
        public string TagName { get; set; }

        [Display(Name = "标签数量")]
        [DefaultValue(0)]
        public int TagCount { get; set;}

    }
}
