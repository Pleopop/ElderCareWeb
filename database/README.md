# ElderCare Database Setup

## Quick Start

### Option 1: SQL Server (Recommended)

```bash
# Run the script in SQL Server Management Studio (SSMS)
# Or use command line:
sqlcmd -S localhost\SQLEXPRESS -i CreateDatabase.sql
```

### Option 2: Command Line

```powershell
# From this directory
Invoke-Sqlcmd -ServerInstance "localhost\SQLEXPRESS" -InputFile "CreateDatabase.sql"
```

## Connection String

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ElderCareDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Tables Created

- **Users** - Authentication & user management
- **Customers** - Customer profiles
- **Caregivers** - Caregiver profiles  
- **Beneficiaries** - Elderly care recipients
- **Bookings** - Care bookings
- **Wallets** - Payment wallets
- **Transactions** - Payment transactions
- **Notifications** - System notifications
- **CareNotes** - AI caregiver observations
- **ActivitySuggestions** - AI activity recommendations
- **DailyReports** - AI daily care reports
- And more...

Total: **20+ tables** with full relationships and indexes.
