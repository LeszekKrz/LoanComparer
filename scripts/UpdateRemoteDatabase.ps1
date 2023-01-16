Push-Location -Path $PSScriptRoot\..\backend\LoanComparer.Application

dotnet ef database update --startup-project ..\LoanComparer.Api --connection 'Server=tcp:loancomparerserverpw.database.windows.net,1433;Initial Catalog=LoanComparerDB;Persist Security Info=False;User ID=LoanComparerAdmin;Password=LoanComparerRules!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
dotnet ef database update --startup-project ..\LoanComparer.Api

Pop-Locationcd