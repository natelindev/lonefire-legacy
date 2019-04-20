using System;
using System.ComponentModel.DataAnnotations;

namespace lonefire.Models
{
    public class ErrorViewModel
    {
        [Display(Name = "错误代码")]
        public int StatusCode { get; set; }

        [Display(Name = "请求ID")]
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}