using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartConfig.Application.Application.UserConfig.Models;
using SmartConfig.Common.Models;
using SmartConfig.Core.Enums;
using SmartConfig.Data;
using SmartConfig.Data.Extensions;

namespace SmartConfig.Application.Application.UserConfig.Queries;

public class SearchUserConfigsQuery : IRequest<ResultSet<Core.Models.UserConfig>>
{
    #region Attributes

    public string[]? Identifiers { get; set; }
    public string? Name { get; set; }
    public UserConfigStatus[]? Status { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public ConfigPagination? ConfigPagination { get; set; }
    public ConfigOrder? ConfigOrder { get; set; }

    public GetUserConfigOptions? Options { get; set; }

    #endregion

    public class Handler : IRequestHandler<SearchUserConfigsQuery, ResultSet<Core.Models.UserConfig>>
    {
        private readonly SmartConfigContext _context;
        private readonly IServiceProvider _serviceProvider;

        public Handler(SmartConfigContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public Task<ResultSet<Core.Models.UserConfig>> Handle(SearchUserConfigsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.UserConfigs.AsNoTracking();

            if (request.Identifiers != null && request.Identifiers.Any())
                query = query.Where(r => request.Identifiers.Contains(r.Identifier));

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(r => r.Name == request.Name);

            if (request.Status != null && request.Status.Any())
                query = query.Where(r => request.Status.Contains(r.Status));

            if (request.CreatedFrom.HasValue)
                query = query.Where(r => r.CreatedUtc >= request.CreatedFrom.Value);

            if (request.CreatedTo.HasValue)
                query = query.Where(r => r.CreatedUtc <= request.CreatedTo.Value);

            var result = query.SetOrder(request.ConfigOrder).GetResultSet(request.ConfigPagination);

            if (request.Options?.ReturnPreferences == false)
                foreach (var userConfig in result.Results)
                {
                    userConfig.UserPreferences = null;
                }

            if (request.Options?.ReturnSettings == false)
                foreach (var userConfig in result.Results)
                {
                    userConfig.UserSettings = null;
                }

            return Task.FromResult(result);
        }
    }
}