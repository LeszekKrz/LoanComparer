using LoanComparer.Application.Model;

namespace LoanComparer.Application.Services.Inquiries;

public interface IInquirySender
{
    /// <summary>
    ///     Sends <paramref name="inquiry" /> to all registered banks.
    /// </summary>
    /// <param name="inquiry">Inquiry to send</param>
    /// <returns>An enumerable that yields statuses obtained from bank responses</returns>
    IAsyncEnumerable<SentInquiryStatus> SendInquiryToAllBanks(Inquiry inquiry);
}