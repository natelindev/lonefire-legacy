using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Models;
using lonefire.Models.ArticleViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace lonefire.Authorization
{
    public class ArticleIsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, Article>
    {
        UserManager<ApplicationUser> _userManager;

        public ArticleIsOwnerAuthorizationHandler(UserManager<ApplicationUser> 
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Article resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If we're not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName   &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName )
            {
                return Task.CompletedTask;
            }

            if (resource.Author == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
