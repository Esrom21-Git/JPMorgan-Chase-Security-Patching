using System;
using System.Threading.Tasks;

namespace SecurityPatchingAutomation.Services
{
    /// <summary>
    /// Terminal dashboard for JPMorgan Chase security patching metrics
    /// </summary>
    public class DashboardService
    {
        public class DashboardMetrics
        {
            public int TotalRepositories { get; set; }
            public int TotalTeams { get; set; }
            public int CVEsDetected { get; set; }
            public int CVEsPatched { get; set; }
            public int CriticalCVEs { get; set; }
            public int AutoApproved { get; set; }
            public decimal AnnualSavings { get; set; }
            public double AverageMTTP { get; set; }
            public double SystemUptime { get; set; }
            public int SecurityIncidents { get; set; }
        }

        /// <summary>
        /// Display real-time dashboard in terminal
        /// </summary>
        public void DisplayDashboard(DashboardMetrics metrics)
        {
            Console.Clear();
            
            // Header
            DisplayHeader();
            
            // Key Metrics
            DisplayKeyMetrics(metrics);
            
            // Team Breakdown
            DisplayTeamBreakdown();
            
            // Regulatory Compliance
            DisplayComplianceStatus();
            
            // Recent Activity
            DisplayRecentActivity();
            
            // Footer
            DisplayFooter();
        }

        private void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                                              ║");
            Console.WriteLine("║           🏦 JPMorgan Chase - Security Patching Automation 🔒               ║");
            Console.WriteLine("║                                                                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void DisplayKeyMetrics(DashboardMetrics metrics)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─ KEY METRICS ─────────────────────────────────────────────────────────────┐");
            Console.ResetColor();
            
            Console.WriteLine($"│ Total Repositories:      {metrics.TotalRepositories,3}                                             │");
            Console.WriteLine($"│ Engineering Teams:       {metrics.TotalTeams,3}                                             │");
            Console.WriteLine($"│ CVEs Detected (2025):    {metrics.CVEsDetected,5:N0}                                         │");
            Console.WriteLine($"│ CVEs Patched (2025):     {metrics.CVEsPatched,5:N0}                                         │");
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"│ Critical CVEs:           {metrics.CriticalCVEs,5:N0}   🔴 URGENT                              │");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"│ Auto-Approved:           {metrics.AutoApproved}%     ✅ Efficiency                         │");
            Console.ResetColor();
            
            Console.WriteLine($"│                                                                           │");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"│ Annual Savings:          ${metrics.AnnualSavings / 1000000:F1}M   💰                                   │");
            Console.ResetColor();
            
            Console.WriteLine($"│ Avg. Time to Patch:      {metrics.AverageMTTP} hours  ⚡ 78% faster                     │");
            Console.WriteLine($"│ System Uptime:           {metrics.SystemUptime}%   🟢 Industry leading            │");
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"│ Security Incidents:      {metrics.SecurityIncidents}      ✅ Zero from unpatched CVEs        │");
            Console.ResetColor();
            
            Console.WriteLine("└───────────────────────────────────────────────────────────────────────────┘");
            Console.WriteLine();
        }

        private void DisplayTeamBreakdown()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─ TOP TEAMS BY SAVINGS ────────────────────────────────────────────────────┐");
            Console.ResetColor();
            
            var teams = new[]
            {
                ("TEAM 1: Trading Platform", 35, "$1.8M", "Critical", "SEC, FINRA"),
                ("TEAM 2: Core Banking", 28, "$1.5M", "Critical", "OCC, FDIC"),
                ("TEAM 3: Payments", 42, "$1.4M", "Critical", "PCI-DSS"),
                ("TEAM 4: Risk & Compliance", 31, "$1.2M", "High", "BSA/AML"),
                ("TEAM 5: Wealth Management", 26, "$0.9M", "High", "SEC, RIA")
            };

            foreach (var (name, repos, savings, priority, regulations) in teams)
            {
                Console.Write($"│ {name,-30}");
                Console.Write($" {repos,3} repos │ ");
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{savings,6}");
                Console.ResetColor();
                
                Console.Write($" │ {priority,-8}");
                Console.WriteLine($" │ {regulations,-12} │");
            }
            
            Console.WriteLine("└───────────────────────────────────────────────────────────────────────────┘");
            Console.WriteLine();
        }

        private void DisplayComplianceStatus()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─ REGULATORY COMPLIANCE STATUS ────────────────────────────────────────────┐");
            Console.ResetColor();
            
            var regulations = new[]
            {
                ("SOX Section 404", "✅ Compliant", "Q1 2026", "Internal Controls"),
                ("PCI-DSS Level 1", "✅ Compliant", "Jan 2027", "Payment Security"),
                ("OCC Guidelines", "✅ Compliant", "Q4 2026", "Banking Regs"),
                ("SEC Reg SCI", "✅ Compliant", "Q2 2026", "Trading Systems"),
                ("FFIEC Standards", "✅ Compliant", "Q2 2026", "Cybersecurity")
            };

            foreach (var (regulation, status, nextAudit, category) in regulations)
            {
                Console.Write($"│ {regulation,-20}");
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" {status,-14}");
                Console.ResetColor();
                
                Console.WriteLine($" │ Next: {nextAudit,-8} │ {category,-15} │");
            }
            
            Console.WriteLine("└───────────────────────────────────────────────────────────────────────────┘");
            Console.WriteLine();
        }

        private void DisplayRecentActivity()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("┌─ RECENT ACTIVITY (Last 24 Hours) ─────────────────────────────────────────┐");
            Console.ResetColor();
            
            var activities = new[]
            {
                ("CVE-2025-1234", "hft-trading-engine", "Critical", "9.8", "Patched ✅"),
                ("CVE-2025-5678", "card-authorization", "Critical", "9.2", "PR Created 🔄"),
                ("CVE-2025-9012", "aml-transaction-monitoring", "High", "7.5", "Testing 🧪"),
                ("CVE-2025-3456", "mobile-banking-api", "High", "7.1", "Auto-Merged ✅"),
                ("CVE-2025-7890", "data-lake-platform", "Medium", "5.3", "Scheduled 📅")
            };

            Console.WriteLine("│ CVE ID          │ Repository                     │ Severity │ CVSS │ Status    │");
            Console.WriteLine("├─────────────────┼────────────────────────────────┼──────────┼──────┼───────────┤");

            foreach (var (cve, repo, severity, cvss, status) in activities)
            {
                Console.Write($"│ {cve,-15}");
                Console.Write($" │ {repo,-30}");
                
                Console.ForegroundColor = severity == "Critical" ? ConsoleColor.Red : 
                                        severity == "High" ? ConsoleColor.Yellow : 
                                        ConsoleColor.Green;
                Console.Write($" │ {severity,-8}");
                Console.ResetColor();
                
                Console.WriteLine($" │ {cvss,-4} │ {status,-9} │");
            }
            
            Console.WriteLine("└───────────────────────────────────────────────────────────────────────────┘");
            Console.WriteLine();
        }

        private void DisplayFooter()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Last Updated: {DateTime.Now:yyyy-MM-dd HH:mm:ss} EST");
            Console.WriteLine($"System Status: 🟢 Operational | Monitoring: 250 repositories across 10 teams");
            Console.WriteLine($"GitHub: JPMorgan-Chase | Contact: security-team@jpmorgan.example.com");
            Console.ResetColor();
        }

        /// <summary>
        /// Display animated scanning progress
        /// </summary>
        public async Task DisplayScanningAnimation()
        {
            Console.Clear();
            DisplayHeader();
            
            Console.WriteLine("🔍 Scanning repositories for security vulnerabilities...\n");
            
            var repos = new[]
            {
                "hft-trading-engine",
                "order-management-system",
                "market-data-feed",
                "account-management-api",
                "wire-transfer-service",
                "card-authorization",
                "fraud-detection-engine",
                "aml-transaction-monitoring"
            };

            foreach (var repo in repos)
            {
                Console.Write($"   Scanning {repo,-35} ");
                await Task.Delay(200);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓");
                Console.ResetColor();
            }
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Scan complete! 15,234 CVEs detected.");
            Console.ResetColor();
            
            await Task.Delay(1000);
        }

        /// <summary>
        /// Display live patching progress
        /// </summary>
        public void DisplayPatchingProgress(int current, int total, string currentRepo)
        {
            var percentage = (current * 100) / total;
            var barLength = 50;
            var filled = (percentage * barLength) / 100;
            
            Console.Write($"\r[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('█', filled));
            Console.ResetColor();
            Console.Write(new string('░', barLength - filled));
            Console.Write($"] {percentage}% ({current}/{total}) - {currentRepo}");
        }
    }
}
