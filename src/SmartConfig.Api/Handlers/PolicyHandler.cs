using Microsoft.AspNetCore.Authorization;

namespace SmartConfig.Api.Handlers;

public class PolicyRequirement : IAuthorizationRequirement
{
    public string Policy { get; }

    public PolicyRequirement(string policy)
    {
        Policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }
}

public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PolicyHandler(IHttpContextAccessor contextAccessor, IServiceScopeFactory serviceScopeFactory)
    {
        _contextAccessor = contextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
    {
        var authorization = _contextAccessor?.HttpContext?.Request?.Headers["Authorization"] ?? string.Empty;
        var appId = _contextAccessor?.HttpContext?.Request?.Headers["AppId"] ?? string.Empty;

        return Task.CompletedTask;
    }
}