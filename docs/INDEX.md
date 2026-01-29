# 📁 JPMorgan Chase Documentation Structure

## Team Documentation

Individual team breakdowns with detailed metrics:

- [Team 1: Trading Platform](teams/TEAM_01_TRADING_PLATFORM.md) - $1.8M savings
- [Team 2: Core Banking](teams/TEAM_02_CORE_BANKING.md) - $1.5M savings
- [Team 3: Payments & Settlement](teams/TEAM_03_PAYMENTS_SETTLEMENT.md) - $1.4M savings
- [Team 4: Risk & Compliance](teams/TEAM_04_RISK_COMPLIANCE.md) - $1.2M savings
- [Team 5: Wealth Management](teams/TEAM_05_WEALTH_MANAGEMENT.md) - $0.9M savings
- [Team 6: Commercial Banking](teams/TEAM_06_COMMERCIAL_BANKING.md) - $0.7M savings
- [Team 7: Digital Banking](teams/TEAM_07_DIGITAL_BANKING.md) - $0.6M savings
- [Team 8: Cybersecurity](teams/TEAM_08_CYBERSECURITY.md) - $0.5M savings
- [Team 9: Data & Analytics](teams/TEAM_09_DATA_ANALYTICS.md) - $0.4M savings
- [Team 10: Infrastructure & Cloud](teams/TEAM_10_INFRASTRUCTURE.md) - $0.3M savings

## Compliance Documentation

Regulatory compliance details:

- [SOX Compliance](compliance/SOX_COMPLIANCE.md) - Section 404 controls
- [PCI-DSS Compliance](compliance/PCI_DSS_COMPLIANCE.md) - Level 1 requirements
- [OCC Guidelines](compliance/OCC_GUIDELINES.md) - Banking regulations
- [SEC Regulations](compliance/SEC_REGULATIONS.md) - Trading platform compliance
- [FFIEC Standards](compliance/FFIEC_STANDARDS.md) - Financial institution requirements

## Source Code Structure

```
src/
├── Models/
│   ├── TeamRepository.cs          # Team and repository models
│   ├── CVE.cs                      # CVE data structures
│   └── PatchDecision.cs            # Patch decision models
├── Services/
│   ├── CVEDetectionService.cs     # CVE scanning service
│   ├── RiskAssessmentService.cs   # Risk calculation service
│   └── PRCreationService.cs       # Pull request automation
└── Program.cs                      # Main entry point
```

## Quick Navigation

- **Getting Started:** [../README.md](../README.md)
- **Setup Guide:** [../SETUP.md](../SETUP.md)
- **Architecture:** [../ARCHITECTURE.md](../ARCHITECTURE.md)
- **Executive Summary:** [../EXECUTIVE_SUMMARY.md](../EXECUTIVE_SUMMARY.md)
