using MediatR;
using SmartConfig.Core.Enums;
using SmartConfig.Core.Models;
using SmartConfig.Data;

namespace SmartConfig.Application.Application.UserConfig.Commands;

public class UpsertUserConfigCommand : IRequest<Core.Models.UserConfig>
{
    #region Attributes

    public string Identifier { get; set; }
    public string Name { get; set; }
    public UserPreferences? UserPreferences { get; set; }
    public List<UserSetting>? UserSettings { get; set; }
    public UserConfigStatus Status { get; set; }

    public UpsertUserConfigOptions? Options { get; set; }

    #endregion

    public class Handler : IRequestHandler<UpsertUserConfigCommand, Core.Models.UserConfig>
    {
        private readonly SmartConfigContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IClusterClient _clusterClient;

        public Handler(SmartConfigContext context, IServiceProvider serviceProvider,
            IClusterClient clusterClient)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _clusterClient = clusterClient;
        }

        public async Task<Core.Models.UserConfig> Handle(UpsertUserConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = _context.UserConfigs.FirstOrDefault(r => r.Identifier == request.Identifier);
            if (entity != null)
            {
                entity.Name = request.Name;
                entity.UserPreferences = request.Options?.UpsertPreferences == true ?
                    request.UserPreferences :
                    entity.UserPreferences;
                entity.UserSettings = request.Options?.UpsertSettings == true ?
                    request.UserSettings :
                    entity.UserSettings;
                entity.Status = request.Status;
                entity.UpdatedUtc = DateTimeOffset.Now;
            }
            else
            {
                entity = new Core.Models.UserConfig
                {
                    Identifier = request.Identifier,
                    Name = request.Name,
                    UserPreferences = request.Options?.UpsertPreferences == true
                        ? request.UserPreferences
                        : null,
                    UserSettings = request.Options?.UpsertSettings == true
                        ? request.UserSettings
                        : null,
                    Status = UserConfigStatus.Active,
                    CreatedUtc = DateTimeOffset.Now,
                    UpdatedUtc = DateTimeOffset.Now
                };
                _context.UserConfigs.Add(entity);
            }
            await _context.SaveChangesAsync(cancellationToken);

            if (request.Options?.ReturnPreferences == false)
                entity.UserPreferences = null;

            if (request.Options?.ReturnSettings == false)
                entity.UserSettings = null;

            return entity;
        }
    }
}

public class UpsertUserConfigOptions
{
    public bool? UpsertPreferences { get; set; }
    public bool? UpsertSettings { get; set; }

    public bool? ReturnPreferences { get; set; }
    public bool? ReturnSettings { get; set; }
}