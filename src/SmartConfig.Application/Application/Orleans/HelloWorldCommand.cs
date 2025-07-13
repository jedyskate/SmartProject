using MediatR;
using SmartConfig.Data;
using SmartConfig.Orleans.Silo.Grains.Tests;

namespace SmartConfig.Application.Application.Orleans;

public class HelloWorldCommand : IRequest<string>
{
    #region Attributes

    public string Name { get; set; }

    #endregion

    public class Handler : IRequestHandler<HelloWorldCommand, string>
    {
        private readonly SmartConfigContext _context;
        private readonly IClusterClient _clusterClient;
            
        public Handler(SmartConfigContext context, IClusterClient clusterClient)
        {
            _context = context;
            _clusterClient = clusterClient;
        }

        public async Task<string> Handle(HelloWorldCommand request, CancellationToken cancellationToken)
        {
            var grain = _clusterClient.GetGrain<IHelloWorldUserGrain>($"my-first-grain-identifier-{request.Name}");
            var result = await grain.SayHelloWorld(request.Name);
                
            return result;
        }
    }
}