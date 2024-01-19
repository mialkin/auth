
using Microsoft.AspNetCore.Authorization;

namespace Auth.MinimalApi.Authorization;

public class MyRequirementHandler : AuthorizationHandler<MyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyRequirement requirement)
    {
        var user = context.User;
        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}