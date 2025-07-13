using Microsoft.Playwright;

namespace SmartConfig.E2ETests.Tests.Swagger;

public class SwaggerTests : PageTest
{
    [Test]
    public async Task SwaggerApi_OrleansApi_HelloWorld_Tests()
    {
        await using var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 150
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // await page.PauseAsync();
        await page.GotoAsync("https://localhost:5111/swagger/index.html"); //Deploy API before running tests

        await Expect(page.Locator("#operations-Orleans-post_api_Orleans_HelloWorld")).ToContainTextAsync("/api/Orleans/HelloWorld");
        
        //User hello world
        await page.GetByRole(AriaRole.Button, new() { Name = "post /api/Orleans/HelloWorld", Exact = true }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Try it out" }).ClickAsync();
        await page.GetByText("{ \"name\": \"string\" }").ClickAsync();
        await page.GetByText("{ \"name\": \"string\" }").PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"string\" }").PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"string\" }").PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"string\" }").FillAsync("{\n  \"name\": \"User\"\n}");
        await page.GetByRole(AriaRole.Button, new() { Name = "Execute" }).ClickAsync();
        await Expect(page.GetByRole(AriaRole.Table)).ToContainTextAsync("\"Hello world number 1 from User. Total hello world count: 1\"");
        
        //Tester hello world
        await page.GetByText("{ \"name\": \"User\" }", new() { Exact = true }).ClickAsync();
        await page.GetByText("{ \"name\": \"User\" }", new() { Exact = true }).PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"User\" }", new() { Exact = true }).PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"User\" }", new() { Exact = true }).PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"User\" }", new() { Exact = true }).FillAsync("{\n  \"name\": \"Tester\"\n}");
        await page.GetByRole(AriaRole.Button, new() { Name = "Execute" }).ClickAsync();
        await Expect(page.GetByRole(AriaRole.Table)).ToContainTextAsync("\"Hello world number 1 from Tester. Total hello world count: 2\"");

        //User hello world
        await page.GetByText("{ \"name\": \"Tester\" }", new() { Exact = true }).ClickAsync();
        await page.GetByText("{ \"name\": \"Tester\" }", new() { Exact = true }).PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"Tester\" }", new() { Exact = true }).PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"Tester\" }", new() { Exact = true }).PressAsync("ArrowLeft");
        await page.GetByText("{ \"name\": \"Tester\" }", new() { Exact = true }).FillAsync("{\n  \"name\": \"User\"\n}");
        await page.GetByRole(AriaRole.Button, new() { Name = "Execute" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Execute" }).ClickAsync();
        await Expect(page.GetByRole(AriaRole.Table)).ToContainTextAsync("\"Hello world number 3 from User. Total hello world count: 4\"");
    }
}