using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace lonefire.Models
{
    //用户类
    public class Image
    {
        [Display(Name = "编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }

        [Display(Name = "文件名")]
        [StringLength(256, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Name { get; set; }

        [NotMapped]
        [Display(Name = "宽度")]
        public int Width { get; set; }

        [NotMapped]
        [Display(Name = "高度")]
        public int Height { get; set; }

        [NotMapped]
        [Display(Name = "颜色")]
        public Color Color { get; set; }
    }
}
