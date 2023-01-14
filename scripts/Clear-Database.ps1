Push-Location -Path $PSScriptRoot\..\backend\LoanComparer.Application

dotnet ef database update 0 --startup-project ..\LoanComparer.Api --connection 'Server=localhost;Integrated Security=true;Database=LoanComparerTest'
dotnet ef database update --startup-project ..\LoanComparer.Api --connection 'Server=localhost;Integrated Security=true;Database=LoanComparerTest'
dotnet ef database update --startup-project ..\LoanComparer.Api

Pop-Location