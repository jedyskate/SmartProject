using MediatR;
using SmartConfig.Data;
using SmartConfig.Orleans.Silo.Grains.Tools;

namespace SmartConfig.Application.Application.Orleans.Commands;

public class SuspiciousEmailDomainsCommand : IRequest<List<string>>
{
    #region Attributes

    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }

    #endregion

    public class Handler : IRequestHandler<SuspiciousEmailDomainsCommand, List<string>>
    {
        private readonly SmartConfigContext _context;
        private readonly IClusterClient _clusterClient;
            
        public Handler(SmartConfigContext context, IClusterClient clusterClient)
        {
            _context = context;
            _clusterClient = clusterClient;
        }

        public async Task<List<string>> Handle(SuspiciousEmailDomainsCommand request, CancellationToken cancellationToken)
        {
            var grain = _clusterClient.GetGrain<ISuspiciousEmailDomainsGrain>($"Suspicious-Email-Addresses-Grain-Identifier");
            var domains = await grain.GetSuspiciousEmailDomains();
                
            var pageSize = request.PageSize ?? 10;
            var pageNumber = request.PageNumber ?? 1;
                
            var result = domains
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();

            return result;
        }
    }
}