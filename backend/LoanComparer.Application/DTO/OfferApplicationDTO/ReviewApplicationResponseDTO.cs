using LoanComparer.Application.Model;

namespace LoanComparer.Application.DTO.OfferApplicationDTO
{
    public sealed record ReviewApplicationResponseDTO(string Status)
    {
        public ReviewApplicationResponseDTO(InquiryStatus inquiryStatus) : this(
            inquiryStatus switch
            {
                InquiryStatus.Accepted => "ACCEPTED",
                InquiryStatus.Rejected => "REJECTED",
                _ => throw new ArgumentOutOfRangeException()
            })
        { }
    }
}
