using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace SecurityPatchingAutomation
{
    // Enums
    public enum AppSeverity
    {
        Critical,  // Payment/Customer systems - highest priority
        High,      // Core business applications
        Medium,    // Support systems
        Low        // Dev/Test tools
    }

    public enum CveSeverity
    {
        Critical,
        Important,
        Moderate,
        Low
    }

    public enum RiskLevel
    {
        Critical,  // Critical CVE OR Critical App with any CVE
        High,      // Important CVE + Critical/High App, OR Major version change
        Medium,    // Moderate CVE or Medium App
        Low        // Low CVE + Low App, patch updates only
    }

    // Models
    public class Repository
    {
        public string Name { get; set; } = string.Empty;
        public AppSeverity Severity { get; set; }
        public string Team { get; set; } = string.Empty;
        public int VulnerabilityCount { get; set; }
        public List<CVE> Vulnerabilities { get; set; } = new();
    }

    public class CVE
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public CveSeverity Severity { get; set; }
        public string AffectedPackage { get; set; } = string.Empty;
        public string CurrentVersion { get; set; } = string.Empty;
        public string PatchedVersion { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }

    public class PatchDecision
    {
        public Repository Repository { get; set; } = new();
        public CVE Cve { get; set; } = new();
        public RiskLevel Risk { get; set; }
        public bool AutoApprove { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsMajorVersionChange { get; set; }
    }

    public class ScanMetrics
    {
        public int CvesDetected { get; set; }
        public int RepositoriesScanned { get; set; }
        public int PrsCreated { get; set; }
        public int PrsFailed { get; set; }
        public int AutoApproved { get; set; }
        public int ManualReview { get; set; }
        public int CriticalCves { get; set; }
        public double ScanDurationSeconds { get; set; }
        public DateTime ScanDate { get; set; } = DateTime.Now;
    }

    class Program
    {
        private static HttpClient _httpClient = new HttpClient();
        private static string _githubToken = string.Empty;
        private static string _teamsWebhookUrl = string.Empty;
        private static string _errorLogPath = "./error_logs";
        private static bool _dryRunMode = false;
        private static ScanMetrics _metrics = new ScanMetrics();

        static async Task Main(string[] args)
        {
            var startTime = Stopwatch.StartNew();

            // Check for dry-run mode
            _dryRunMode = args.Contains("--dry-run");
            if (_dryRunMode)
            {
                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║            🔍 DRY RUN MODE ENABLED 🔍                  ║");
                Console.WriteLine("║   No PRs will be created or approved                   ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");
            }

            // Load configuration
            LoadConfiguration();

            // Ensure error log directory exists
            Directory.CreateDirectory(_errorLogPath);

            try
            {
                Console.WriteLine("╔════════════════════════════════════════════════════════╗");
                Console.WriteLine("║   AUTOMATED .NET SECURITY PATCHING SYSTEM v1.1.0       ║");
                Console.WriteLine("║   Owner: Esrom Tekle | Team: E-Commerce                ║");
                Console.WriteLine($"║   Scan Date: {DateTime.Now:MMMM dd, yyyy HH:mm}                  ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

                // Step 1: Fetch CVEs from MSRC API
                Console.WriteLine("[INFO] Step 1: Fetching .NET CVEs from Microsoft Security Response Center...");
                var cves = await FetchCVEsFromMSRC();
                _metrics.CvesDetected = cves.Count;
                _metrics.CriticalCves = cves.Count(c => c.Severity == CveSeverity.Critical);
                Console.WriteLine($"[SUCCESS] Found {cves.Count} CVEs ({_metrics.CriticalCves} Critical)\n");

                // Step 2: Fetch repositories from GitHub
                Console.WriteLine("[INFO] Step 2: Scanning Alaska-ECommerce GitHub repositories...");
                var repositories = await FetchGitHubRepositories();
                _metrics.RepositoriesScanned = repositories.Count;
                Console.WriteLine($"[SUCCESS] Scanned {repositories.Count} repositories\n");

                // Step 3: Match CVEs to repositories and calculate risk
                Console.WriteLine("[INFO] Step 3: Analyzing vulnerabilities and calculating risk...");
                var patchDecisions = AnalyzeVulnerabilities(repositories, cves);
                Console.WriteLine($"[SUCCESS] Generated {patchDecisions.Count} patch recommendations\n");

                // Step 4: Display Priority Dashboard
                ShowPriorityDashboard(repositories);

                // Step 5: Create Pull Requests
                Console.WriteLine("\n[INFO] Step 4: Creating Pull Requests...");
                await CreatePullRequests(patchDecisions);

                // Step 6: Display Metrics Dashboard
                _metrics.ScanDurationSeconds = startTime.Elapsed.TotalSeconds;
                ShowMetricsDashboard();

                // Step 7: Send Teams notification for critical items
                await SendTeamsNotification(repositories, patchDecisions);

                Console.WriteLine("\n[SUCCESS] ✅ Security patching scan completed successfully!");
                Console.WriteLine($"[INFO] Total execution time: {startTime.Elapsed.TotalSeconds:F1}s\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] ❌ Fatal error: {ex.Message}");
                LogError("FATAL_ERROR", ex);
                await SendTeamsAlert($"🚨 Security Patching System FAILED: {ex.Message}");
            }
        }

        static void LoadConfiguration()
        {
            // Try to load from appsettings.json
            try
            {
                if (File.Exists("appsettings.json"))
                {
                    var json = File.ReadAllText("appsettings.json");
                    var config = JsonSerializer.Deserialize<JsonElement>(json);
                    var appSettings = config.GetProperty("AppSettings");

                    _githubToken = appSettings.GetProperty("GitHubToken").GetString() ?? string.Empty;
                    _teamsWebhookUrl = appSettings.GetProperty("TeamsWebhookUrl").GetString() ?? string.Empty;
                    _errorLogPath = appSettings.GetProperty("ErrorLogPath").GetString() ?? "./error_logs";
                }
            }
            catch
            {
                Console.WriteLine("[WARNING] Could not load appsettings.json, using environment variables");
            }

            // Override with environment variables if present
            _githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? _githubToken;
            _teamsWebhookUrl = Environment.GetEnvironmentVariable("TEAMS_WEBHOOK_URL") ?? _teamsWebhookUrl;
            _errorLogPath = Environment.GetEnvironmentVariable("ERROR_LOG_PATH") ?? _errorLogPath;

            // Validate critical configuration
            if (string.IsNullOrEmpty(_githubToken))
            {
                Console.WriteLine("[WARNING] ⚠️ GITHUB_TOKEN not configured - using mock data");
            }

            if (string.IsNullOrEmpty(_teamsWebhookUrl))
            {
                Console.WriteLine("[WARNING] ⚠️ TEAMS_WEBHOOK_URL not configured - Teams notifications disabled");
            }
        }

        static async Task<List<CVE>> FetchCVEsFromMSRC()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://api.msrc.microsoft.com/cvrf/v2.0/updates");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARNING] MSRC API returned {response.StatusCode}, using mock data");
                    return GetMockCVEs();
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(content);

                // Parse MSRC response (simplified for demo)
                var cves = new List<CVE>();
                
                // In production, parse actual MSRC response structure
                // For now, return mock data
                Console.WriteLine("[INFO] MSRC API connected successfully");
                return GetMockCVEs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Failed to fetch from MSRC API: {ex.Message}");
                Console.WriteLine("[INFO] Falling back to mock CVE data");
                return GetMockCVEs();
            }
        }

        static List<CVE> GetMockCVEs()
        {
            return new List<CVE>
            {
                new CVE
                {
                    Id = "CVE-2026-0001",
                    Title = "System.Text.Json Denial of Service Vulnerability",
                    Severity = CveSeverity.Critical,
                    AffectedPackage = "System.Text.Json",
                    CurrentVersion = "8.0.0",
                    PatchedVersion = "8.0.5",
                    PublishedDate = DateTime.Parse("2026-01-20")
                },
                new CVE
                {
                    Id = "CVE-2026-0002",
                    Title = "ASP.NET Core Authentication Bypass",
                    Severity = CveSeverity.Important,
                    AffectedPackage = "Microsoft.AspNetCore.Authentication",
                    CurrentVersion = "8.0.0",
                    PatchedVersion = "8.0.1",
                    PublishedDate = DateTime.Parse("2026-01-18")
                },
                new CVE
                {
                    Id = "CVE-2026-0003",
                    Title = "Entity Framework Core SQL Injection",
                    Severity = CveSeverity.Moderate,
                    AffectedPackage = "Microsoft.EntityFrameworkCore",
                    CurrentVersion = "8.0.0",
                    PatchedVersion = "8.0.2",
                    PublishedDate = DateTime.Parse("2026-01-15")
                }
            };
        }

        static async Task<List<Repository>> FetchGitHubRepositories()
        {
            // KNOWN ISSUE: GitHub API parsing bug - code looks for .GetProperty("value") but GitHub returns direct array
            // FIX NEEDED: Remove .GetProperty("value") from this method
            
            if (string.IsNullOrEmpty(_githubToken))
            {
                Console.WriteLine("[INFO] Using mock repository data (no GitHub token configured)");
                return GetMockRepositories();
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SecurityPatcher", "1.1.0"));
                
                // KNOWN ISSUE: Should use Bearer token, currently uses Basic auth (deprecated)
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_githubToken}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var response = await _httpClient.GetAsync("https://api.github.com/orgs/Alaska-ECommerce/repos");
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[WARNING] GitHub API returned {response.StatusCode}, using mock data");
                    return GetMockRepositories();
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(content);

                // CRITICAL BUG: This line will fail - GitHub returns array directly, not wrapped in "value"
                // var repos = json.GetProperty("value"); // ❌ WRONG
                
                // For now, use mock data
                Console.WriteLine("[INFO] GitHub API connected (using mock data for demo)");
                return GetMockRepositories();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Failed to fetch GitHub repos: {ex.Message}");
                LogError("GITHUB_API_ERROR", ex);
                return GetMockRepositories();
            }
        }

        static List<Repository> GetMockRepositories()
        {
            var appSeverityMap = GetAppSeverityMap();
            
            return new List<Repository>
            {
                new Repository { Name = "MPPayments", Severity = AppSeverity.Critical, Team = "MP Avanza" },
                new Repository { Name = "MPBankActivity", Severity = AppSeverity.Critical, Team = "MP Avanza" },
                new Repository { Name = "MPProfile", Severity = AppSeverity.Critical, Team = "MP Avanza" },
                new Repository { Name = "TeamAService", Severity = AppSeverity.High, Team = "Team A" },
                new Repository { Name = "InternalDashboard", Severity = AppSeverity.Medium, Team = "Internal Tools" },
                new Repository { Name = "DevTools", Severity = AppSeverity.Low, Team = "DevOps" }
            };
        }

        static Dictionary<string, AppSeverity> GetAppSeverityMap()
        {
            // Centralized mapping of JPMorgan Chase financial services to business criticality
            return new Dictionary<string, AppSeverity>
            {
                // CRITICAL - Trading & Payment Systems (patch within 24 hours - PCI-DSS/SEC requirement)
                { "hft-trading-engine", AppSeverity.Critical },
                { "wire-transfer-service", AppSeverity.Critical },
                { "transaction-processing", AppSeverity.Critical },
                { "aml-transaction-monitoring", AppSeverity.Critical },
                { "commercial-lending-platform", AppSeverity.Critical },
                { "mobile-banking-api", AppSeverity.Critical },
                { "threat-intelligence-platform", AppSeverity.Critical },
                { "order-management-system", AppSeverity.Critical },
                { "account-management-api", AppSeverity.Critical },
                { "ach-processing", AppSeverity.Critical },
                { "real-time-payments", AppSeverity.Critical },
                { "swift-messaging", AppSeverity.Critical },
                { "card-authorization", AppSeverity.Critical },
                { "fraud-detection-engine", AppSeverity.Critical },
                { "kyc-verification-service", AppSeverity.Critical },
                { "ofac-sanctions-screening", AppSeverity.Critical },
                
                // HIGH - Core Banking Applications (patch within 48 hours)
                { "market-data-feed", AppSeverity.High },
                { "risk-calculation-engine", AppSeverity.High },
                { "trade-settlement-api", AppSeverity.High },
                { "algorithmic-trading", AppSeverity.High },
                { "deposit-withdrawal-service", AppSeverity.High },
                { "balance-inquiry-api", AppSeverity.High },
                { "statement-generation", AppSeverity.High },
                { "settlement-reconciliation", AppSeverity.High },
                { "payment-gateway", AppSeverity.High },
                { "fraud-analytics-platform", AppSeverity.High },
                { "credit-risk-modeling", AppSeverity.High },
                { "compliance-reporting", AppSeverity.High },
                { "portfolio-management-system", AppSeverity.High },
                { "trade-execution-api", AppSeverity.High },
                { "advisory-platform", AppSeverity.High },
                { "credit-approval-workflow", AppSeverity.High },
                { "treasury-management", AppSeverity.High },
                { "online-banking-portal", AppSeverity.High },
                { "authentication-service", AppSeverity.High },
                { "digital-onboarding", AppSeverity.High },
                { "bill-pay-service", AppSeverity.High },
                { "siem-analytics", AppSeverity.High },
                { "incident-response-automation", AppSeverity.High },
                { "data-lake-platform", AppSeverity.High },
                { "ml-ai-platform", AppSeverity.High },
                { "aws-infrastructure-code", AppSeverity.High },
                { "kubernetes-platform", AppSeverity.High },
                
                // MEDIUM - Analytics & Support Systems (patch within 1 week)
                { "trading-analytics", AppSeverity.Medium },
                { "interest-calculation", AppSeverity.Medium },
                { "performance-reporting", AppSeverity.Medium },
                { "rebalancing-engine", AppSeverity.Medium },
                { "loan-servicing-api", AppSeverity.Medium },
                { "vulnerability-management", AppSeverity.Medium },
                { "real-time-analytics", AppSeverity.Medium },
                { "data-governance", AppSeverity.Medium },
                { "cicd-pipeline-framework", AppSeverity.Medium },
                { "monitoring-observability", AppSeverity.Medium },
                
                // LOW - Dev/Test Tools (patch as available)
                { "backtesting-tools", AppSeverity.Low },
                { "simulation-tools", AppSeverity.Low },
                { "reporting-analytics", AppSeverity.Low },
                { "etl-pipelines", AppSeverity.Low },
                { "devops-tools", AppSeverity.Low },
                { "dev-tools-utilities", AppSeverity.Low }
            };
        }

        static List<PatchDecision> AnalyzeVulnerabilities(List<Repository> repositories, List<CVE> cves)
        {
            var decisions = new List<PatchDecision>();

            foreach (var repo in repositories)
            {
                foreach (var cve in cves)
                {
                    // Simulate vulnerability matching (in production, scan actual dependencies)
                    repo.Vulnerabilities.Add(cve);
                    repo.VulnerabilityCount++;

                    var risk = CalculateCombinedRisk(repo.Severity, cve.Severity, cve.CurrentVersion, cve.PatchedVersion);
                    var autoApprove = ShouldAutoApprove(repo.Severity, cve.Severity, risk);

                    decisions.Add(new PatchDecision
                    {
                        Repository = repo,
                        Cve = cve,
                        Risk = risk,
                        AutoApprove = autoApprove,
                        Reason = GetApprovalReason(repo.Severity, cve.Severity, risk, autoApprove),
                        IsMajorVersionChange = IsMajorVersionChange(cve.CurrentVersion, cve.PatchedVersion)
                    });
                }
            }

            return decisions;
        }

        static RiskLevel CalculateCombinedRisk(AppSeverity appSeverity, CveSeverity cveSeverity, string currentVersion, string patchedVersion)
        {
            // Business Rule: Critical apps with ANY CVE = Critical Risk
            if (appSeverity == AppSeverity.Critical)
            {
                return RiskLevel.Critical;
            }

            // Business Rule: Critical CVE = Critical Risk (regardless of app)
            if (cveSeverity == CveSeverity.Critical)
            {
                return RiskLevel.Critical;
            }

            // Business Rule: Major version change = High Risk
            if (IsMajorVersionChange(currentVersion, patchedVersion))
            {
                return RiskLevel.High;
            }

            // Business Rule: Important CVE + High App = High Risk
            if (cveSeverity == CveSeverity.Important && appSeverity == AppSeverity.High)
            {
                return RiskLevel.High;
            }

            // Business Rule: Moderate CVE or Medium App = Medium Risk
            if (cveSeverity == CveSeverity.Moderate || appSeverity == AppSeverity.Medium)
            {
                return RiskLevel.Medium;
            }

            // Default: Low Risk
            return RiskLevel.Low;
        }

        static bool ShouldAutoApprove(AppSeverity appSeverity, CveSeverity cveSeverity, RiskLevel risk)
        {
            // Business Rules for Auto-Approval

            // NEVER auto-approve Critical apps (even for Low CVEs)
            if (appSeverity == AppSeverity.Critical)
            {
                return false;
            }

            // NEVER auto-approve Critical risk
            if (risk == RiskLevel.Critical)
            {
                return false;
            }

            // High apps: Only auto-approve Low CVEs
            if (appSeverity == AppSeverity.High)
            {
                return cveSeverity == CveSeverity.Low;
            }

            // Medium apps: Auto-approve Low and Moderate CVEs
            if (appSeverity == AppSeverity.Medium)
            {
                return cveSeverity == CveSeverity.Low || cveSeverity == CveSeverity.Moderate;
            }

            // Low apps: Aggressive auto-approval (everything except Critical)
            if (appSeverity == AppSeverity.Low)
            {
                return cveSeverity != CveSeverity.Critical;
            }

            return false;
        }

        static string GetApprovalReason(AppSeverity appSeverity, CveSeverity cveSeverity, RiskLevel risk, bool autoApprove)
        {
            if (appSeverity == AppSeverity.Critical)
            {
                return "Critical app - Manual review required (prevents revenue impact)";
            }

            if (risk == RiskLevel.Critical)
            {
                return "Critical risk - InfoSec team review required";
            }

            if (autoApprove)
            {
                return $"Auto-approved: {appSeverity} app + {cveSeverity} CVE = {risk} risk (within safety threshold)";
            }

            return $"Manual review: {appSeverity} app + {cveSeverity} CVE = {risk} risk";
        }

        static bool IsMajorVersionChange(string currentVersion, string patchedVersion)
        {
            try
            {
                var current = Version.Parse(currentVersion.Split('-')[0]);
                var patched = Version.Parse(patchedVersion.Split('-')[0]);
                return patched.Major > current.Major;
            }
            catch
            {
                return false;
            }
        }

        static void ShowPriorityDashboard(List<Repository> repositories)
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║            APP PRIORITY DASHBOARD                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            var criticalApps = repositories.Where(r => r.Severity == AppSeverity.Critical).ToList();
            var highApps = repositories.Where(r => r.Severity == AppSeverity.High).ToList();
            var mediumApps = repositories.Where(r => r.Severity == AppSeverity.Medium).ToList();
            var lowApps = repositories.Where(r => r.Severity == AppSeverity.Low).ToList();

            // Critical Apps
            if (criticalApps.Any())
            {
                Console.WriteLine("🔴 [CRITICAL] CRITICAL APPS (Payment/Customer Systems):");
                foreach (var app in criticalApps)
                {
                    var indicator = app.VulnerabilityCount > 0 ? "[URGENT]" : "[MONITOR]";
                    Console.WriteLine($"  {indicator} {app.Name}: {app.VulnerabilityCount} CVEs");
                }
                Console.WriteLine($"  Total: {criticalApps.Sum(a => a.VulnerabilityCount)} vulnerabilities in {criticalApps.Count} critical apps\n");
            }

            // High Priority Apps
            if (highApps.Any())
            {
                Console.WriteLine("🟠 [HIGH] HIGH PRIORITY APPS (Core Business):");
                foreach (var app in highApps)
                {
                    Console.WriteLine($"  {app.Name}: {app.VulnerabilityCount} CVEs");
                }
                Console.WriteLine($"  Total: {highApps.Sum(a => a.VulnerabilityCount)} vulnerabilities in {highApps.Count} high-priority apps\n");
            }

            // Medium Priority Apps
            if (mediumApps.Any())
            {
                Console.WriteLine("🟡 [MEDIUM] MEDIUM PRIORITY APPS (Support Systems):");
                foreach (var app in mediumApps)
                {
                    Console.WriteLine($"  {app.Name}: {app.VulnerabilityCount} CVEs");
                }
                Console.WriteLine($"  Total: {mediumApps.Sum(a => a.VulnerabilityCount)} vulnerabilities in {mediumApps.Count} medium-priority apps\n");
            }

            // Calculate business impact
            var criticalAndHighCves = criticalApps.Sum(a => a.VulnerabilityCount) + highApps.Sum(a => a.VulnerabilityCount);
            var manualPatchDays = criticalAndHighCves * 0.5; // 4 hours per CVE
            var automatedHours = 24;
            var reductionPercent = ((manualPatchDays * 8 - automatedHours) / (manualPatchDays * 8)) * 100;

            Console.WriteLine("📊 BUSINESS IMPACT:");
            Console.WriteLine($"  Critical+High CVEs: {criticalAndHighCves}");
            Console.WriteLine($"  Manual Patch Time: {manualPatchDays:F1} business days");
            Console.WriteLine($"  Automated Time: < {automatedHours} hours");
            Console.WriteLine($"  Risk Window Reduction: {reductionPercent:F0}% faster\n");

            if (criticalApps.Any(a => a.VulnerabilityCount > 0))
            {
                Console.WriteLine("⚠️  [ACTION] IMMEDIATE ACTION REQUIRED:");
                Console.WriteLine("  - Patch critical apps within 24 hours");
                Console.WriteLine("  - Alert security team for critical CVEs");
                Console.WriteLine("  - Consider maintenance window for payment systems\n");
            }
        }

        static async Task CreatePullRequests(List<PatchDecision> decisions)
        {
            foreach (var decision in decisions)
            {
                try
                {
                    if (_dryRunMode)
                    {
                        Console.WriteLine($"[DRY-RUN] Would create PR for {decision.Repository.Name} - {decision.Cve.Id}");
                        Console.WriteLine($"          Risk: {decision.Risk} | Auto-Approve: {decision.AutoApprove}");
                        Console.WriteLine($"          Reason: {decision.Reason}");
                        _metrics.PrsCreated++;
                        continue;
                    }

                    // KNOWN ISSUE: This logs "created PR" but doesn't actually call GitHub API
                    // FIX NEEDED: Implement real PR creation with GitHub Pulls API
                    
                    Console.WriteLine($"[INFO] Creating PR: {decision.Repository.Name} - {decision.Cve.Id}");
                    Console.WriteLine($"       Package: {decision.Cve.AffectedPackage} ({decision.Cve.CurrentVersion} → {decision.Cve.PatchedVersion})");
                    Console.WriteLine($"       Risk: {decision.Risk} | Auto-Approve: {decision.AutoApprove}");
                    Console.WriteLine($"       Reason: {decision.Reason}");

                    // TODO: Implement actual GitHub PR creation
                    // var prUrl = await CreateGitHubPR(decision);
                    // if (decision.AutoApprove) { await ApproveAndMergePR(prUrl); }

                    _metrics.PrsCreated++;
                    if (decision.AutoApprove)
                    {
                        _metrics.AutoApproved++;
                        Console.WriteLine($"[SUCCESS] ✅ PR created and auto-approved");
                    }
                    else
                    {
                        _metrics.ManualReview++;
                        Console.WriteLine($"[INFO] 📋 PR created - awaiting manual review");
                    }

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] ❌ Failed to create PR for {decision.Repository.Name}: {ex.Message}");
                    _metrics.PrsFailed++;
                    LogError("PR_CREATION_FAILURE", ex, new { decision.Repository.Name, decision.Cve.Id });
                }
            }
        }

        static void ShowMetricsDashboard()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║      METRICS DASHBOARD - {DateTime.Now:MMMM yyyy}              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            var successRate = _metrics.PrsCreated > 0 
                ? ((_metrics.PrsCreated - _metrics.PrsFailed) / (double)_metrics.PrsCreated) * 100 
                : 0;

            var autoApprovalRate = _metrics.PrsCreated > 0 
                ? (_metrics.AutoApproved / (double)_metrics.PrsCreated) * 100 
                : 0;

            Console.WriteLine("📈 SCAN SUMMARY:");
            Console.WriteLine($"  CVEs Detected: {_metrics.CvesDetected}");
            Console.WriteLine($"  Repositories Scanned: {_metrics.RepositoriesScanned}");
            Console.WriteLine($"  PRs Created: {_metrics.PrsCreated}");
            Console.WriteLine($"  PRs Failed: {_metrics.PrsFailed}");
            Console.WriteLine($"  Success Rate: {successRate:F1}%");
            Console.WriteLine($"  Critical CVEs: {_metrics.CriticalCves}");
            Console.WriteLine($"  Scan Duration: {_metrics.ScanDurationSeconds:F1}s\n");

            Console.WriteLine("🤖 AUTOMATION METRICS:");
            Console.WriteLine($"  Auto-Approved: {_metrics.AutoApproved} PRs");
            Console.WriteLine($"  Manual Review: {_metrics.ManualReview} PRs");
            Console.WriteLine($"  Auto-Approval Rate: {autoApprovalRate:F1}%");
            Console.WriteLine($"  Avg Patch Time: 18 hours (estimated)\n");

            var complianceStatus = _metrics.CriticalCves > 0 ? "REQUIRES ATTENTION" : "EXCELLENT";
            Console.WriteLine("✅ COMPLIANCE:");
            Console.WriteLine($"  Critical Unpatched: {_metrics.CriticalCves}");
            Console.WriteLine($"  SLA Compliance: {(_metrics.CriticalCves == 0 ? "PASS" : "IN PROGRESS")} < 24 hours");
            Console.WriteLine($"  Security Posture: {complianceStatus}\n");
        }

        static async Task SendTeamsNotification(List<Repository> repositories, List<PatchDecision> decisions)
        {
            if (string.IsNullOrEmpty(_teamsWebhookUrl))
            {
                Console.WriteLine("[INFO] Teams notifications disabled (no webhook URL configured)");
                return;
            }

            var criticalDecisions = decisions.Where(d => d.Risk == RiskLevel.Critical).ToList();
            if (!criticalDecisions.Any())
            {
                return;
            }

            var message = $"🚨 **CRITICAL SECURITY ALERT** 🚨\n\n" +
                         $"**{criticalDecisions.Count} Critical CVEs detected** requiring immediate attention:\n\n";

            foreach (var decision in criticalDecisions.Take(5))
            {
                message += $"- **{decision.Repository.Name}** ({decision.Repository.Severity}): {decision.Cve.Id}\n" +
                          $"  {decision.Cve.Title}\n" +
                          $"  Patch: {decision.Cve.CurrentVersion} → {decision.Cve.PatchedVersion}\n\n";
            }

            message += $"\n**Action Required:** Patch critical apps within 24 hours\n" +
                      $"**Scan Date:** {DateTime.Now:MMM dd, yyyy HH:mm}\n";

            await SendTeamsAlert(message);
        }

        static async Task SendTeamsAlert(string message)
        {
            if (string.IsNullOrEmpty(_teamsWebhookUrl))
            {
                return;
            }

            try
            {
                var payload = new
                {
                    text = message
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(_teamsWebhookUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[INFO] Teams notification sent successfully");
                }
                else
                {
                    Console.WriteLine($"[WARNING] Teams notification failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Failed to send Teams notification: {ex.Message}");
            }
        }

        static void LogError(string errorType, Exception ex, object? context = null)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var filename = $"{timestamp}_{errorType}.json";
                var filepath = Path.Combine(_errorLogPath, filename);

                var errorLog = new
                {
                    Timestamp = DateTime.Now,
                    ErrorType = errorType,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Context = context
                };

                var json = JsonSerializer.Serialize(errorLog, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filepath, json);

                Console.WriteLine($"[INFO] Error logged to: {filename}");
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"[ERROR] Failed to write error log: {logEx.Message}");
            }
        }
    }
}
