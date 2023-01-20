namespace LoanComparer.Application.Constants
{
    public static class LoanComparerConstants
    {
        internal const int MaxJobTypeNameLength = 50;
        internal const int MaxFirstNameLength = 50;
        internal const int MaxLastNameLength = 50;
        internal const int MaxGovernmentIdTypeLength = 15;
        internal static readonly IReadOnlySet<string> GovernmentIdTypes = new HashSet<string> { "PESEL", "ID Number", "Passport Number" };
        internal const int MaxGovernmentIdValueLength = 11;
        internal const int MaxEmailLength = 256;
        internal const int PeselLength = 11;
        internal const int IdNumberLength = 9;
        internal const int PassportNumberLength = 9;
        public const string ClientRoleName = "Client";
        public const string BankEmployeeRoleName = "BankEmployee";
        public const string OurBankName = "This Bank";
    }
}
