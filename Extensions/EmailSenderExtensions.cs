using System.Text.Encodings.Web;
using System.Threading.Tasks;
using lonefire.Services;

namespace lonefire.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "确认您的邮箱",
                $"点击后面的链接以确认邮箱: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
