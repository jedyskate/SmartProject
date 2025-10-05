using MediatR;
using SmartConfig.Core.Enums;
using SmartConfig.Core.Models;
using SmartConfig.Data;

namespace SmartConfig.Application.Application.UserConfig.Commands;

public class CreateUserConfigCommand : IRequest<Core.Models.UserConfig>
{
    #region Attributes

    public string Identifier { get; set; }
    public string Name { get; set; }
    public UserPreferences? UserPreferences { get; set; }
    public List<UserSetting>? UserSettings { get; set; }

    public CreateUserConfigOptions? Options { get; set; }

    #endregion

    public class Handler : IRequestHandler<CreateUserConfigCommand, Core.Models.UserConfig>
    {
        private readonly SmartConfigContext _context;
        private readonly IServiceProvider _serviceProvider;
            
        public Handler(SmartConfigContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public async Task<Core.Models.UserConfig> Handle(CreateUserConfigCommand request, CancellationToken cancellationToken)
        {
            ////TODO::DELETE LATER. EXCEPTION HANDLING TEST
            //throw new SmartConfigException(HttpStatusCode.NotFound, "Testing error filter");
            //throw new ApplicationException("Random error");

            var entity = new Core.Models.UserConfig
            {
                Identifier = request.Identifier,
                Name = request.Name,
                UserPreferences = request.Options?.CreatePreferences == true
                    ? request.UserPreferences
                    : null,
                UserSettings = request.Options?.CreateSettings == true
                    ? request.UserSettings
                    : null,
                Status = UserConfigStatus.Active,
                CreatedUtc = DateTimeOffset.Now,
                UpdatedUtc = DateTimeOffset.Now
            };
            _context.UserConfigs.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }
    }
}

public class CreateUserConfigOptions
{
    public bool? CreatePreferences { get; set; }
    public bool? CreateSettings { get; set; }
}