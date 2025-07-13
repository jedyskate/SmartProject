using Microsoft.Playwright;

namespace SmartConfig.E2ETests.Tests.Blazor;

public class BlazorTests : PageTest
{
    [Test]
    public async Task BlazorCounter_test()
    {
        await using var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 150
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        
        // await Page.PauseAsync();
        await page.GotoAsync("https://localhost:7052", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        }); //Deploy Blazor before running tests

        await page.ReloadAsync();

        //Counter menu button
        var counterMenuLink = page.GetByRole(AriaRole.Link, new() { Name = "Counter" });
        
        // Verify menu button link
        await Expect(counterMenuLink).ToHaveAttributeAsync("href", "counter");
        
        //Go to Counter page
        await counterMenuLink.ClickAsync();
        await Expect(page.GetByRole(AriaRole.Status)).ToContainTextAsync("Current count: 0");

        //Increase counter by 1
        await page.GetByRole(AriaRole.Button, new() { Name = "Click me" }).ClickAsync();
        await Expect(page.GetByRole(AriaRole.Status)).ToContainTextAsync("Current count: 1");

        //Increase counter some more
        await page.GetByRole(AriaRole.Button, new() { Name = "Click me" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Click me" }).ClickAsync();
        await Expect(page.GetByRole(AriaRole.Status)).ToContainTextAsync("Current count: 3");
    }
}