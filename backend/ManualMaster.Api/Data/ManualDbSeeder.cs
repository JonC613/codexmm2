using ManualMaster.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ManualMaster.Api.Data;

public static class ManualDbSeeder
{
    public static async Task SeedAsync(ManualDbContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("ManualDbSeeder");

        if (await context.Manuals.AnyAsync())
        {
            logger.LogInformation("Manual data already seeded.");
            return;
        }

        var now = DateTime.UtcNow;
        var manuals = new List<Manual>
        {
            new()
            {
                Title = "Acme Smart Kettle 2000",
                Category = "Kitchen Appliances",
                Tags = new List<string> { "kettle", "acme", "smart" },
                Content = "Step 1: Fill the kettle. Step 2: Select desired temperature. Step 3: Press start.",
                Size = 0,
                UploadDate = now.AddDays(-5),
                SourceUrl = "https://example.com/manuals/acme-smart-kettle-2000",
                SearchQuery = "Acme Smart Kettle 2000 manual"
            },
            new()
            {
                Title = "Horizon RoboVac X1",
                Category = "Home Cleaning",
                Tags = new List<string> { "vacuum", "robot", "horizon" },
                Content = "Getting started: charge RoboVac X1 for 6 hours. Use the mobile app to schedule cleaning.",
                Size = 0,
                UploadDate = now.AddDays(-2),
                SourceUrl = "https://example.com/manuals/horizon-robovac-x1",
                SearchQuery = "Horizon RoboVac X1 manual"
            }
        };

        await context.Manuals.AddRangeAsync(manuals);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} manuals", manuals.Count);
    }
}
