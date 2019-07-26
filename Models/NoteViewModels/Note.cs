using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lonefire.Models.NoteViewModels
{
    public class Note
    {
        [Display(Name ="编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoteID { get; set; }

        [Display(Name = "标题")]
        [StringLength(64, ErrorMessage = " {0} 长度必须在 {2} 到 {1} 之间。", MinimumLength = 0)]
        public string Title { get; set; }

        [Display(Name = "内容")]
        [Required]
        public string Content { get; set; }

        [Display(Name = "访问权限")]
        public NoteStatus Status { get; set; }

        [Display(Name = "媒体文件")]
        public string MediaSerialized { get; set; }

        [Display(Name = "添加时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd HH:mm}")]
        public DateTime AddTime { get; set; }

    }

    public enum NoteStatus
    {
        [Display(Name = "公开")]
        Public,
        [Display(Name = "私密")]
        Private
    }
}