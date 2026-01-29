# Deployment Guide

## Production Deployment

### Prerequisites
- Azure subscription with contributor access
- GitHub organization admin permissions
- .NET 8.0 SDK
- Azure CLI installed

### Deployment Steps

1. **Configure Azure Resources**
   ```bash
   az group create --name jpmc-security-patching --location eastus
   az functionapp create --name jpmc-patching-function --resource-group jpmc-security-patching
   ```

2. **Set Environment Variables**
   ```bash
   GITHUB_TOKEN=your_token_here
   JPMC_ORG=JPMorgan-Chase
   ```

3. **Deploy Application**
   ```bash
   dotnet publish -c Release
   az functionapp deployment source config-zip --resource-group jpmc-security-patching --name jpmc-patching-function --src publish.zip
   ```

4. **Configure Timer Trigger**
   - Schedule: Every Monday at 9:00 AM EST
   - CRON: `0 0 9 * * 1`

5. **Verify Deployment**
   ```bash
   az functionapp function show --name jpmc-patching-function --resource-group jpmc-security-patching
   ```
