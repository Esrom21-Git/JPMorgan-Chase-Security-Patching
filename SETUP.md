# Setup Guide

## Prerequisites

- **.NET 8.0 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **GitHub Account** with organization access
- **GitHub Personal Access Token** (PAT)

## Step 1: Create GitHub Personal Access Token

1. Go to GitHub Settings → Developer settings → Personal access tokens → [Tokens (classic)](https://github.com/settings/tokens)

2. Click "Generate new token (classic)"

3. Give your token a name: `Security Patching Automation`

4. Select the following scopes:
   - ✅ `repo` (Full control of private repositories)
   - ✅ `read:org` (Read org and team membership, read org projects)
   - ✅ `security_events` (Read and write security events)

5. Click "Generate token"

6. **Important:** Copy the token immediately - you won't see it again!

## Step 2: Set Environment Variables

### Windows (PowerShell)

```powershell
# Required
$env:GITHUB_TOKEN = "ghp_your_token_here"
$env:GITHUB_ORG = "your-organization-name"

# Optional
$env:WEBHOOK_URL = "https://hooks.slack.com/services/YOUR/WEBHOOK/URL"
$env:ERROR_LOG_PATH = "./error_logs"
```

To make permanent (add to PowerShell profile):
```powershell
notepad $PROFILE
# Add the export commands above
```

### Linux/Mac (Bash)

```bash
# Required
export GITHUB_TOKEN="ghp_your_token_here"
export GITHUB_ORG="your-organization-name"

# Optional
export WEBHOOK_URL="https://hooks.slack.com/services/YOUR/WEBHOOK/URL"
export ERROR_LOG_PATH="./error_logs"
```

To make permanent (add to ~/.bashrc or ~/.zshrc):
```bash
echo 'export GITHUB_TOKEN="ghp_your_token_here"' >> ~/.bashrc
echo 'export GITHUB_ORG="your-organization-name"' >> ~/.bashrc
source ~/.bashrc
```

### Using .env file (Alternative)

Create a `.env` file in the project root:

```env
GITHUB_TOKEN=ghp_your_token_here
GITHUB_ORG=your-organization-name
WEBHOOK_URL=https://hooks.slack.com/services/YOUR/WEBHOOK/URL
ERROR_LOG_PATH=./error_logs
```

**Important:** Add `.env` to `.gitignore` to prevent committing secrets!

## Step 3: Configure App Severity Mapping

Edit `Program.cs` and customize the `GetAppSeverityMap()` method:

```csharp
static Dictionary<string, AppSeverity> GetAppSeverityMap()
{
    return new Dictionary<string, AppSeverity>
    {
        // Replace with your actual repository names
        { "your-payment-api", AppSeverity.Critical },
        { "your-core-service", AppSeverity.High },
        { "your-analytics", AppSeverity.Medium },
        { "your-dev-tools", AppSeverity.Low }
    };
}
```

## Step 4: Test in Dry-Run Mode

```bash
dotnet build
dotnet run -- --dry-run
```

Expected output:
```
╔════════════════════════════════════════════════════════╗
║     Automated .NET Security Patching System           ║
║            GitHub Dependabot Integration              ║
╚════════════════════════════════════════════════════════╝

[DRY-RUN MODE] No actual PRs will be created

[INFO] Configuration loaded for organization: your-org-name
[INFO] Step 1: Querying GitHub Dependabot API...
[SUCCESS] Found 5 CVEs from Dependabot API
...
```

## Step 5: Run in Production

Once satisfied with dry-run results:

```bash
dotnet run
```

## Optional: Set Up Webhooks

### Slack Webhook

1. Go to [Slack API Apps](https://api.slack.com/apps)
2. Create new app → From scratch
3. Add "Incoming Webhooks" feature
4. Create webhook for your channel
5. Copy webhook URL to `WEBHOOK_URL` env variable

### Microsoft Teams Webhook

1. Open Teams → Your channel → More options (...)
2. Connectors → Incoming Webhook
3. Configure → Provide name and upload icon
4. Copy webhook URL to `WEBHOOK_URL` env variable

### Discord Webhook

1. Open Discord → Server Settings → Integrations
2. Webhooks → New Webhook
3. Copy webhook URL to `WEBHOOK_URL` env variable

## Troubleshooting

### Error: "GITHUB_TOKEN environment variable is required"

**Solution:** Make sure you've set the environment variable and restarted your terminal.

### Error: "HTTP 401 Unauthorized"

**Solution:** 
- Check that your PAT is valid
- Verify it has the correct scopes (`repo`, `read:org`, `security_events`)
- Ensure the token hasn't expired

### Error: "HTTP 404 Not Found"

**Solution:**
- Verify `GITHUB_ORG` matches your organization name exactly
- Ensure you have access to the organization
- Check that your PAT has `read:org` scope

### No CVEs detected

**Solution:**
- This is normal if you don't have open Dependabot alerts
- System will use mock data for demonstration
- Enable Dependabot in your repositories:
  - Settings → Security & analysis → Dependabot alerts → Enable

## Next Steps

- Review [README.md](README.md) for full feature documentation
- Set up scheduled execution (cron job or Task Scheduler)
- Configure CI/CD pipeline for automated runs
- Customize severity mappings for your organization
- Set up monitoring and alerting

## Need Help?

- Open an issue on GitHub
- Check existing issues for solutions
- Review the code comments in `Program.cs`
