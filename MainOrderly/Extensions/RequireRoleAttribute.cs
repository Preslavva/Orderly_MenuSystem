using System;
using System.Linq;
using MainOrderly.WebApp.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MainOrderly.WebApp.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _requiredRoles;

        public RequireRoleAttribute(params string[] roles)
        {
            _requiredRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.Session.GetAuthenticatedUser();

            if (user == null)
            {
                context.Result = new RedirectToActionResult("Login", "BusinessAccount", null);
                return;
            }
            bool hasRequiredRole = false;
            
            foreach (var role in _requiredRoles)
            {
                if (user.Roles.Contains(role))
                {
                    hasRequiredRole = true;
                    break;
                }
            }

            if (!hasRequiredRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}