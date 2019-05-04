using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lonefire.Models.ArticleViewModels
{
    [NotMapped]
    public class ArticleIndexVM
    {
        [Display(Name = "编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleID { get; set; }

        [Display(Name = "标题")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "作者")]
        public string Author { get; set; }

        [Display(Name = "标签")]
        public string Tag { get; set; }

        [Display(Name = "添加时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AddTime { get; set; }

        [Display(Name = "审核状态")]
        public ArticleStatus Status { get; set; }
    }
}
