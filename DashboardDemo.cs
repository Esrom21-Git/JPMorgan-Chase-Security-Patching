using System;
using System.Threading.Tasks;
using SecurityPatchingAutomation.Services;

namespace SecurityPatchingAutomation
{
    /// <summary>
    /// Demo program showcasing the dashboard
    /// </summary>
    class DashboardDemo
    {
        static async Task Main(string[] args)
        {
            var dashboard = new DashboardService();
            
            // Show scanning animation
            await dashboard.DisplayScanningAnimation();
            
            // Display main dashboard
            var metrics = new DashboardService.DashboardMetrics
            {
                TotalRepositories = 250,
                TotalTeams = 10,
                CVEsDetected = 15234,
                CVEsPatched = 15234,
                CriticalCVEs = 1247,
                AutoApproved = 73,
                AnnualSavings = 8500000m,
                AverageMTTP = 18.0,
                SystemUptime = 99.999,
                SecurityIncidents = 0
            };
            
            dashboard.DisplayDashboard(metrics);
            
            Console.WriteLine();
            Console.WriteLine("Press any key to see patching progress simulation...");
            Console.ReadKey();
            Console.WriteLine();
            
            // Simulate patching progress
            Console.WriteLine("\n🔧 Automated Patching in Progress...\n");
            for (int i = 1; i <= 100; i++)
            {
                var repos = new[] { "trading-engine", "payment-gateway", "banking-api", "fraud-detection" };
                dashboard.DisplayPatchingProgress(i, 100, repos[i % repos.Length]);
                await Task.Delay(50);
            }
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ All patches applied successfully!");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
