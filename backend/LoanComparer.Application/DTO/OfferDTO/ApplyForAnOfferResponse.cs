using LoanComparer.Application.Model;

namespace LoanComparer.Application.DTO.OfferDTO
{
    public sealed class ApplyForAnOfferResponse
    {
        public string Status { get; }

        private ApplyForAnOfferResponse(string status)
        {
            Status = status;
        }

        public static ApplyForAnOfferResponse FromInquiryStatus(InquiryStatus inquiryStatus)
        {
            string status = inquiryStatus switch
            {
                InquiryStatus.Pending => "PENDING",
                InquiryStatus.Accepted => "OFFERRECEIVED",
                InquiryStatus.Rejected => "REJECTED",
                InquiryStatus.WaitingForAcceptance => "WAITINGFORACCEPTANCE",
                InquiryStatus.Timeout => "TIMEOUT",
                InquiryStatus.Error => "ERROR",
                _ => throw new ArgumentOutOfRangeException()
            };

            return new ApplyForAnOfferResponse(status);
        }
    }
}
