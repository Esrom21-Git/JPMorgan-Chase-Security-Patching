# SEC Regulations Compliance

## Securities and Exchange Commission

**Registrant:** JPMorgan Chase & Co.  
**CIK:** 0000019617  
**Last Filing:** Form 10-K (FY 2025)  
**Next Filing:** Q1 2026 (10-Q)

## Regulation SCI (Systems Compliance and Integrity)

### Trading Platform Security Requirements

**Scope:** High-frequency trading, order management, market data systems

**SCI Entity Obligations:**

1. **Vulnerability Management (Rule 1001)**
   - Continuous monitoring for security vulnerabilities
   - Risk-based patch prioritization
   - Automated patching for non-critical systems
   - Manual review for trading platform patches

2. **Business Continuity (Rule 1002)**
   - Zero downtime from unpatched CVEs (18 months)
   - Trading platform uptime: 99.999%
   - Disaster recovery tested quarterly

3. **Change Management (Rule 1003)**
   - All system changes documented
   - Automated testing before production
   - Staged rollout to minimize risk

## Trading Platform Systems

### In-Scope SCI Systems

| System | Type | Criticality | Patch Window |
|--------|------|-------------|--------------|
| HFT Trading Engine | Direct Market Access | Critical | Manual review required |
| Order Management | Routing System | Critical | Manual review required |
| Market Data Feed | Market Data | Critical | Manual review required |
| Risk Calculation | Risk Management | Critical | Manual review required |

### Patch Management for SCI Systems

**Critical Trading Systems:**
- **Never auto-approved** - manual InfoSec review required
- **Trading hours:** No patches during market hours (9:30 AM - 4:00 PM ET)
- **Testing:** Extensive pre-production testing required
- **Rollback:** Immediate rollback capability

**Non-SCI Trading Systems:**
- Auto-approval eligible for low-risk patches
- 73% auto-approval rate (2025)
- Faster deployment cycle

## Form 10-K Cybersecurity Disclosure

### 2025 Annual Report Excerpt

**Item 1C. Cybersecurity:**

"The Company has implemented an automated security patching system that monitors 250+ repositories across 10 engineering teams. In 2025, the system automatically patched 15,234 CVEs, reducing mean time to patch from 4.5 days to 18 hours. The system has achieved zero security incidents from unpatched CVEs over an 18-month period while maintaining 99.999% uptime for trading platforms."

**Material Impact:**
- $8.5M annual cost savings
- $144M trading downtime prevented
- Zero regulatory violations

## SEC Rule 15c3-5 (Market Access Rule)

### Risk Management Controls

**Pre-Trade Risk Controls:**
- Automated vulnerability scanning
- Timely patching of trading systems
- Security testing before deployment

**Post-Trade Surveillance:**
- Incident detection and response
- Audit trail for all system changes
- Quarterly compliance reporting

## Annual Compliance Certification

**Certified by:** Chief Compliance Officer  
**Date:** December 31, 2025  
**Period:** FY 2025  

**Certifications:**
✅ Regulation SCI compliance maintained  
✅ Form 10-K cybersecurity disclosure accurate  
✅ Market Access Rule controls operational  
✅ Trading platform integrity preserved  

## SEC Examination Readiness

**Last SEC Examination:** June 2025  
**Findings:** No deficiencies noted  
**Next Examination:** Expected Q2 2026

**Examination Focus Areas:**
- Cybersecurity risk management
- Trading system resilience
- Vulnerability management processes
- Business continuity planning

---

**SEC EDGAR Filings:** www.sec.gov/cgi-bin/browse-edgar?CIK=19617  
**Compliance Contact:** sec.compliance@jpmorgan.example.com  
**Trading Systems Contact:** trading.systems@jpmorgan.example.com
