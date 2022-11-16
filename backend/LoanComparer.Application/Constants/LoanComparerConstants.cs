using System.Net.NetworkInformation;

namespace LoanComparer.Application.Constants
{
    internal static class LoanComparerConstants
    {
        internal const int MaxJobTypeNameLength = 50;
        internal const int MaxFirstNameLength = 50;
        internal const int MaxLastNameLength = 50;
        internal const int MaxGovernmentIdTypeLength = 15;
        internal readonly static HashSet<string> GovernmentIdTypes = new HashSet<string>() { "PESEL", "ID Number", "Passport Number" };
        internal const int MaxGovernmentIdValueLength = 11;
        internal const int MaxEmailLength = 256;
        internal const int PeselLength = 11;
        internal const int IDNumberLength = 9;
        internal const int PassportNumberLength = 9;
        internal const string ClientRoleName = "Client";
        internal const string BankEmployeeRoleName = "BankEmployee";
        internal const string PasswordResetEmailSubject = "Loan Comparer password reset";
        internal const string PasswordResetHtmlContent = @"Dear {0},<br>Click the link to reset your password: <a href=""{1}"">Reset password</a>";
        internal const string EmailConfirmHtmlContent = @"Dear {0},<br>Please confirm your email: <a href=""{1}"">Confirm email</a><br>If you have not created account or Loan Comparer website please do nothing.";
    }
}
