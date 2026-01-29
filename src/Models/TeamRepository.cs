namespace SecurityPatchingAutomation.Models;

public class TeamRepository
{
    public string Name { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public int CVEsPerYear { get; set; }
    public string Team { get; set; } = string.Empty;
    public List<string> RegulatoryCompliance { get; set; } = new();
}

public class Team
{
    public int TeamNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RepositoryCount { get; set; }
    public decimal AnnualSavings { get; set; }
    public int CriticalServices { get; set; }
    public List<string> RegulatoryFrameworks { get; set; } = new();
    public List<TeamRepository> Repositories { get; set; } = new();
    
    public decimal CalculateROI()
    {
        int totalCVEs = Repositories.Sum(r => r.CVEsPerYear);
        decimal hoursSaved = totalCVEs * 11.5m; // Financial services premium
        decimal hourlyCost = 150m; // Finance engineer hourly rate
        return hoursSaved * hourlyCost;
    }
}

public static class JPMorganTeams
{
    public static List<Team> GetAll() => new()
    {
        new Team
        {
            TeamNumber = 1,
            Name = "Trading Platform",
            RepositoryCount = 35,
            AnnualSavings = 1_800_000m,
            CriticalServices = 7,
            RegulatoryFrameworks = new() { "SEC", "FINRA", "MiFID II" },
            Repositories = new()
            {
                new() { Name = "hft-trading-engine", Severity = "Critical", Purpose = "High-frequency trading execution", CVEsPerYear = 50 },
                new() { Name = "order-management-system", Severity = "Critical", Purpose = "Multi-asset order routing", CVEsPerYear = 45 },
                new() { Name = "market-data-feed", Severity = "Critical", Purpose = "Real-time market data", CVEsPerYear = 40 }
            }
        },
        new Team
        {
            TeamNumber = 2,
            Name = "Core Banking",
            RepositoryCount = 28,
            AnnualSavings = 1_500_000m,
            CriticalServices = 6,
            RegulatoryFrameworks = new() { "OCC", "FDIC", "FFIEC" }
        },
        new Team
        {
            TeamNumber = 3,
            Name = "Payments & Settlement",
            RepositoryCount = 42,
            AnnualSavings = 1_400_000m,
            CriticalServices = 8,
            RegulatoryFrameworks = new() { "PCI-DSS Level 1", "NACHA", "SWIFT" }
        }
    };
}
