using System;
using System.Threading.Tasks;
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
            //if (foobar)
            //{
                return Task.FromResult(IdentityResult.Success);
            //}


            //return Task.FromResult(
                     //IdentityResult.Failed(new IdentityError
                     //{
                     //    Code = "Invalid foobar",
                     //    Description = "foobar is invalid."
                     //}));
        }
    }
}
