# TEAM 2: Core Banking

**Team Size:** 28 repositories  
**Annual Savings:** $1.5M  
**Critical Services:** 6  
**Regulatory:** OCC, FDIC, FFIEC

## Critical Repositories

| Repository | Severity | Purpose | CVEs/Year |
|------------|----------|---------|-----------|
| `account-management-api` | Critical | Customer account lifecycle management | 48 |
| `transaction-processing` | Critical | Real-time transaction processing | 52 |
| `deposit-withdrawal-service` | Critical | ATM and branch deposit/withdrawal | 45 |
| `balance-inquiry-api` | High | Real-time balance calculations | 40 |
| `statement-generation` | High | Account statement generation | 35 |
| `interest-calculation` | Medium | Interest accrual engine | 30 |

## Customer Impact

- **Retail customers:** 60M
- **Daily transactions:** $2.5T
- **1-hour outage cost:** $8M
- **SOX compliance:** 24-hour patch SLA

## Compliance Requirements

### OCC Guidelines
- Patch critical vulnerabilities within 24 hours
- Maintain audit trail for 7 years
- Quarterly compliance reporting

### FFIEC Standards
- Risk-based patch management
- Automated vulnerability scanning
- Incident response procedures
