namespace LoanComparer.Application.Model
{
    public record Email(IReadOnlyCollection<string> Recipients, string Subject, string PlainTextContent, string HtmlContent);
}
