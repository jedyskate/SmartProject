using Microsoft.Extensions.DependencyInjection;
using NBomber.Contracts;
using NBomber.CSharp;
using SmartConfig.BE.Sdk;
using SmartConfig.BE.Sdk.Extensions;

namespace SmartConfig.LoadTests.Tests
{
    [TestFixture]
    public class UserConfigLoadTest : IDisposable
    {
        private IServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new SmartConfigApiSettings());
            services.AddSmartConfigApiClient();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Test, Order(2)]
        public void UserConfig_Crud_LoadTest()
        {
            var scenario = Scenario.Create("user_config_crud_scenario", async context =>
            {
                var smartConfigClient = _serviceProvider.GetRequiredService<ISmartConfigApiClient>();
                var identifier = $"test-user-{Guid.NewGuid()}";
                var name = $"Test User {context.ScenarioInfo.InstanceId}";

                // Create
                var createResponse = await smartConfigClient.CreateUserConfigAsync(new CreateUserConfigCommand
                {
                    Identifier = identifier,
                    Name = name,
                    UserPreferences = new UserPreferences
                    {
                        Language = "en",
                        NotificationType = new NotificationType { Email = true, Sms = false },
                        UserNotifications = new UserNotifications { Billings = true, NewsLetter = false }
                    },
                    UserSettings = new List<UserSetting>
                    {
                        new UserSetting { Key = "theme", Value = "dark" }
                    },
                    Options = new CreateUserConfigOptions { CreatePreferences = true, CreateSettings = true }
                });

                if (createResponse?.Response?.Identifier != identifier)
                {
                    return Response.Fail();
                }

                context.Logger.Information("Created user config with identifier: {Identifier}", identifier);

                // Get
                var getResponse = await smartConfigClient.GetUserConfigAsync(new GetUserConfigQuery
                {
                    Identifier = identifier,
                    Options = new GetUserConfigOptions { ReturnPreferences = true, ReturnSettings = true }
                });

                if (getResponse?.Response?.Identifier != identifier)
                {
                    return Response.Fail();
                }
                
                context.Logger.Information("Got user config with identifier: {Identifier}", identifier);

                // Update
                var updateResponse = await smartConfigClient.UpsertUserConfigAsync(new UpsertUserConfigCommand
                {
                    Identifier = identifier,
                    Name = $"{name} Updated",
                    UserPreferences = getResponse.Response.UserPreferences,
                    UserSettings = getResponse.Response.UserSettings,
                    Status = UserConfigStatus.Active,
                    Options = new UpsertUserConfigOptions { UpsertPreferences = true, UpsertSettings = true, ReturnPreferences = true, ReturnSettings = true }
                });

                if (updateResponse?.Response?.Name != $"{name} Updated")
                {
                    return Response.Fail();
                }
                
                context.Logger.Information("Updated user config with identifier: {Identifier}", identifier);

                return Response.Ok();
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(5)),
                Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)),
                Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15))
            )
            .WithThresholds(
                Threshold.Create(scenarioStats => scenarioStats.Fail.Request.Percent < 1.0),
                Threshold.Create(scenarioStats => scenarioStats.Ok.Latency.Percent95 < 1000)
            );

            NBomberRunner.RegisterScenarios(scenario).Run();
        }

        public void Dispose()
        {
            (_serviceProvider as IDisposable)?.Dispose();
        }
    }
}
