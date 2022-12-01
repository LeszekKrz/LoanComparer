param(
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$migrationName = $(throw "Please specify migration name")
)

Push-Location -Path $PSScriptRoot\..\backend\LoanComparer.Application

dotnet ef migrations add $migrationName --startup-project ..\LoanComparer.Api

Pop-Location