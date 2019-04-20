using System;
using System.Threading.Tasks;
using lonefire.Data;
using lonefire.Models.CommentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace lonefire.Authorization
{
    public class CommentAdministratorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, CommentViewModel>
    {
        protected override Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement,
                                     CommentViewModel resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(Constants.AdministratorsRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
