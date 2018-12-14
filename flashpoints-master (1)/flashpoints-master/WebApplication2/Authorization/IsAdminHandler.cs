
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FlashPoints.Data;
using System.Linq;

// Part of custom policy-based authorization. This handler is registered in Startup.cs.
// Authorizes user if they are a member of the specified group.
// Read more here: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies

namespace FlashPoints.Authorization
{
    public class IsAdminHandler : AuthorizationHandler<IsAdminRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        // Inject the app's configuration and current HttpContext so we can access information about them.
        public IsAdminHandler(ApplicationDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   IsAdminRequirement requirement)
        {
            var email = _httpContext.HttpContext.User.Identity.Name;
            var query = _context.User.Where(e => e.Email == email);
            bool isAdmin = false;

            if (query.Count() > 0)
            {
                if (query.First().IsAdmin == true)
                {
                    isAdmin = true;
                }
            }

            // If no one is logged in, return without succeeding.
            if (string.IsNullOrEmpty(email)) return Task.CompletedTask;

            else if (isAdmin == true)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }


    }
}