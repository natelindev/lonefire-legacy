using System;
using System.Threading.Tasks;
using lonefire.Data;
using Microsoft.AspNetCore.Identity;

namespace lonefire.Extensions
{
    //Override default Username Validator
    public class LF_UsernameValidator<TUser> : IUserValidator<TUser>
       where TUser : IdentityUser
    {
       
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager,
                                                  TUser user)
        {
            if (!user.UserName.Contains(Constants.AdminTag) && !user.UserName.Contains(Constants.EmptyUserTag))
            {
                return Task.FromResult(IdentityResult.Success);
            }


            return Task.FromResult(
                     IdentityResult.Failed(new IdentityError
                     {
                         Code = "非法用户名",
                         Description = "用户名包含系统预留字段"
                     }));
        }
    }
}
