namespace LoanComparer.Application.Exceptions
{
    public class InquiryErrorException : Exception
    {
        public InquiryErrorException() : base() { }
        public InquiryErrorException(string? message) : base(message) { }
        public InquiryErrorException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
