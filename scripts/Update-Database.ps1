Push-Location -Path $PSScriptRoot\..\backend\LoanComparer.Application

dotnet ef database update --startup-project ..\LoanComparer.Api

Pop-Location