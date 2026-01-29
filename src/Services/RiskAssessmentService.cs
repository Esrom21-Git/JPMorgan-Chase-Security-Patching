using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityPatchingAutomation.Services
{
    /// <summary>
    /// Risk assessment service for evaluating CVE severity and financial impact
    /// </summary>
    public class RiskAssessmentService
    {
        private readonly Dictionary<string, RiskLevel> _criticalRepos = new()
        {
            // Trading Platform (TEAM 1)
            { "hft-trading-engine", RiskLevel.Critical },
            { "order-management-system", RiskLevel.Critical },
            { "market-data-feed", RiskLevel.Critical },
            { "risk-calculation-engine", RiskLevel.Critical },
            
            // Core Banking (TEAM 2)
            { "account-management-api", RiskLevel.Critical },
            { "transaction-processing", RiskLevel.Critical },
            { "deposit-withdrawal-service", RiskLevel.Critical },
            
            // Payments (TEAM 3)
            { "wire-transfer-service", RiskLevel.Critical },
            { "ach-processing", RiskLevel.Critical },
            { "card-authorization", RiskLevel.Critical },
            { "fraud-detection-engine", RiskLevel.Critical },
            
            // Risk & Compliance (TEAM 4)
            { "aml-transaction-monitoring", RiskLevel.Critical },
            { "kyc-verification-service", RiskLevel.Critical },
            { "ofac-sanctions-screening", RiskLevel.Critical }
        };

        public enum RiskLevel
        {
            Low,
            Medium,
            High,
            Critical
        }

        public class RiskAssessment
        {
            public string RepositoryName { get; set; } = "";
            public RiskLevel Level { get; set; }
            public string CVEId { get; set; } = "";
            public double CVSSScore { get; set; }
            public bool RequiresManualReview { get; set; }
            public int PatchWindowHours { get; set; }
            public string RegulatoryImpact { get; set; } = "";
            public decimal FinancialImpact { get; set; }
        }

        /// <summary>
        /// Assess risk for a CVE in a given repository
        /// </summary>
        public async Task<RiskAssessment> AssessRiskAsync(string repoName, string cveId, double cvssScore)
        {
            await Task.Delay(10); // Simulate API call

            var assessment = new RiskAssessment
            {
                RepositoryName = repoName,
                CVEId = cveId,
                CVSSScore = cvssScore,
                Level = DetermineRiskLevel(repoName, cvssScore),
            };

            // Determine regulatory impact
            assessment.RegulatoryImpact = GetRegulatoryImpact(repoName);

            // Determine if manual review required
            assessment.RequiresManualReview = RequiresManualReview(assessment);

            // Set patch window based on risk
            assessment.PatchWindowHours = GetPatchWindow(assessment.Level, assessment.RequiresManualReview);

            // Calculate financial impact
            assessment.FinancialImpact = CalculateFinancialImpact(assessment);

            return assessment;
        }

        private RiskLevel DetermineRiskLevel(string repoName, double cvssScore)
        {
            // Check if repo is critical infrastructure
            if (_criticalRepos.TryGetValue(repoName, out var repoRisk) && repoRisk == RiskLevel.Critical)
            {
                if (cvssScore >= 7.0) return RiskLevel.Critical;
                if (cvssScore >= 4.0) return RiskLevel.High;
                return RiskLevel.Medium;
            }

            // Standard CVSS-based risk
            if (cvssScore >= 9.0) return RiskLevel.Critical;
            if (cvssScore >= 7.0) return RiskLevel.High;
            if (cvssScore >= 4.0) return RiskLevel.Medium;
            return RiskLevel.Low;
        }

        private string GetRegulatoryImpact(string repoName)
        {
            // Trading platforms
            if (repoName.Contains("trading") || repoName.Contains("hft") || repoName.Contains("order"))
                return "SEC Reg SCI - Manual review required during market hours";

            // Payment systems
            if (repoName.Contains("payment") || repoName.Contains("card") || repoName.Contains("wire") || repoName.Contains("ach"))
                return "PCI-DSS Level 1 - 24-hour patch SLA";

            // Banking systems
            if (repoName.Contains("account") || repoName.Contains("deposit") || repoName.Contains("lending"))
                return "OCC Guidelines - 48-hour patch SLA";

            // AML/Compliance
            if (repoName.Contains("aml") || repoName.Contains("kyc") || repoName.Contains("ofac") || repoName.Contains("sanctions"))
                return "BSA/AML - Zero tolerance for violations";

            return "SOX Section 404 - Documented controls required";
        }

        private bool RequiresManualReview(RiskAssessment assessment)
        {
            // Trading platforms always require manual review
            if (assessment.RepositoryName.Contains("trading") || 
                assessment.RepositoryName.Contains("hft") || 
                assessment.RepositoryName.Contains("order-management"))
                return true;

            // Critical CVEs in critical repos
            if (assessment.Level == RiskLevel.Critical && _criticalRepos.ContainsKey(assessment.RepositoryName))
                return true;

            // CVSS 9.0+ always requires review
            if (assessment.CVSSScore >= 9.0)
                return true;

            return false;
        }

        private int GetPatchWindow(RiskLevel level, bool manualReview)
        {
            if (manualReview)
            {
                return level switch
                {
                    RiskLevel.Critical => 12, // 12 hours for manual critical
                    RiskLevel.High => 24,
                    _ => 48
                };
            }

            return level switch
            {
                RiskLevel.Critical => 24,
                RiskLevel.High => 48,
                RiskLevel.Medium => 168, // 1 week
                _ => 720 // 30 days
            };
        }

        private decimal CalculateFinancialImpact(RiskAssessment assessment)
        {
            decimal baseImpact = assessment.Level switch
            {
                RiskLevel.Critical => 500000m, // $500K
                RiskLevel.High => 150000m,
                RiskLevel.Medium => 50000m,
                _ => 10000m
            };

            // Trading platform multiplier (downtime cost)
            if (assessment.RepositoryName.Contains("trading") || assessment.RepositoryName.Contains("hft"))
                baseImpact *= 24; // $12M/hour downtime cost

            // Payment system multiplier
            if (assessment.RepositoryName.Contains("payment") || assessment.RepositoryName.Contains("card"))
                baseImpact *= 8;

            return baseImpact;
        }

        /// <summary>
        /// Get all repositories classified as critical
        /// </summary>
        public List<string> GetCriticalRepositories()
        {
            return _criticalRepos.Where(x => x.Value == RiskLevel.Critical)
                                 .Select(x => x.Key)
                                 .ToList();
        }
    }
}
