namespace SmartConfig.Silo.Grains.Tools;

public interface ISuspiciousEmailDomainsGrain : IGrainWithStringKey
{
    Task<List<string>> GetSuspiciousEmailDomains();
    Task<bool> IsSuspiciousEmail(string email);
}

[Serializable]
public class SuspiciousEmailDomainsState
{
    public List<string>? EmailDomains { get; set; }
}

public class SuspiciousEmailDomainsGrain : Grain, ISuspiciousEmailDomainsGrain
{
    private readonly IPersistentState<SuspiciousEmailDomainsState> _suspiciousEmailDomains;
    private readonly IHttpClientFactory _httpClientFactory;

    private const string ProviderUrl = "https://gist.githubusercontent.com/ammarshah/f5c2624d767f91a7cbdc4e54db8dd0bf/raw/660fd949eba09c0b86574d9d3aa0f2137161fc7c/all_email_provider_domains.txt";

    public SuspiciousEmailDomainsGrain(
        [PersistentState("suspiciousEmailDomains")] IPersistentState<SuspiciousEmailDomainsState> suspiciousSuspiciousEmailDomains,
        IHttpClientFactory httpClientFactory)
    {
        _suspiciousEmailDomains = suspiciousSuspiciousEmailDomains;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> IsSuspiciousEmail(string email)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            return false;
            
        var domains = await GetSuspiciousEmailDomains();

        var emailDomain = email.ToLower().Split('@')[1];
        return domains.Contains(emailDomain);
    }

    public async Task<List<string>> GetSuspiciousEmailDomains()
    {
        try
        {

            if (_suspiciousEmailDomains.State.EmailDomains?.Any() ?? false)
                return _suspiciousEmailDomains.State.EmailDomains;
            else
                _suspiciousEmailDomains.State.EmailDomains = new List<string>();

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(ProviderUrl);

            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var content = await response.Content.ReadAsStringAsync();
            var lines = content.Split('\n');

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                    _suspiciousEmailDomains.State.EmailDomains.Add(trimmed);
            }

            await _suspiciousEmailDomains.WriteStateAsync();
            return _suspiciousEmailDomains.State.EmailDomains;
        }
        catch (Exception ex)
        {
            // Optional: log the error with Orleans logging if needed
            return new List<string>();
        }
    }
}