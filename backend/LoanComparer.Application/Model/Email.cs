namespace LoanComparer.Application.Model;

public record Email(IReadOnlyCollection<string> Recipients, string Subject, string PlainTextContent,
    string HtmlContent);

public sealed record ConfirmEmailAddressEmail : Email
{
    public ConfirmEmailAddressEmail(string recipientEmail, string? recipientName, string confirmationLink) : base(
        new[] { recipientEmail }, "[Loan Comparer] Confirm email", string.Empty,
        $@"Dear {recipientName ?? "user"},<br>Please confirm your email: <a href=""{confirmationLink}"">Confirm email</a><br>
        If you have not created account on Loan Comparer website please do nothing.")
    {
    }
}

public sealed record ResetPasswordEmail : Email
{
    public ResetPasswordEmail(string recipientEmail, string? recipientName, string passwordResetLink) : base(
        new[] { recipientEmail }, "[Loan Comparer] Password reset", string.Empty,
        $@"Dear {recipientName ?? "user"},<br>Click the link to reset your password: <a href=""{passwordResetLink}"">Reset password</a>")
    {
    }
}

public sealed record StatusChangedEmail : Email
{
    public StatusChangedEmail(string recipientEmail, string? recipientName, string checkInquiryLink) : base(
        new[] { recipientEmail }, "[Loan Comparer] Inquiry status changed", string.Empty,
        $@"Dear {recipientName ?? "user"},<br>Click the link to check status of your inquiry: <a href=""{checkInquiryLink}"">Check</a>")
    {
    }
}

public sealed record NewInquiryEmail : Email
{
    public NewInquiryEmail(string recipientEmail, string recipientName, string checkInquiryLink) : base(
        new[] { recipientEmail },
        "[Loan Comparer] New inquiry has been submitted",
        string.Empty,
        $@"Dear {recipientName},<br>Click the link to check status of your inquiry: <a href=""{checkInquiryLink}"">Check</a>")
    {
    }
}