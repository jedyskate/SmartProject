using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartConfig.Application.Application.UserConfig.Models;
using SmartConfig.Common.Exceptions;
using SmartConfig.Data;

namespace SmartConfig.Application.Application.UserConfig.Queries;

public class GetUserConfigQuery : IRequest<Core.Models.UserConfig>
{
    #region Attributes

    public string Identifier { get; set; }

    public GetUserConfigOptions? Options { get; set; }

    #endregion

    public class Handler : IRequestHandler<GetUserConfigQuery, Core.Models.UserConfig>
    {
        private readonly SmartConfigContext _context;
        private readonly IServiceProvider _serviceProvider;

        public Handler(SmartConfigContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public Task<Core.Models.UserConfig> Handle(GetUserConfigQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Identifier))
                throw new SmartConfigException(HttpStatusCode.BadRequest, "UserConfig parameter required.");

            var query = _context.UserConfigs.AsNoTracking();

            if (!string.IsNullOrEmpty(request.Identifier))
                query = query.Where(r => r.Identifier == request.Identifier);

            var result = query.FirstOrDefault();
            if (result == null)
                throw new SmartConfigException(HttpStatusCode.NotFound, "UserConfig doesn't exist");

            if (request.Options?.ReturnPreferences == false)
                result.UserPreferences = null;

            if (request.Options?.ReturnSettings == false)
                result.UserSettings = null;

            return Task.FromResult(result);
        }
    }
}