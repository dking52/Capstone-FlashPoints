using Microsoft.AspNetCore.Authorization;

// This class simply gets initialized in Startup.cs with a group name passed as an argument.
// IsInGroupHandler.cs evaluates the policy.
// Read more about this custom policy-based authorization here.
// https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies

namespace FlashPoints.Authorization
{
    public class IsAdminRequirement : IAuthorizationRequirement

    {
        public IsAdminRequirement()
        {
            
        }
    }


}