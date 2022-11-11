using LoanComparer.Application.DTO;

namespace LoanComparer.Application.Model
{
    public class GovernmentId
    {
        public string Id { get; set; }
        public string Type { get; private set; }
        public string Value { get; private set; }
        public User User { get; private set; }

        public GovernmentId(GovernmentIdDTO governmentIdDTO)
        {
            Type = governmentIdDTO.Type;
            Value = governmentIdDTO.Value;
        }
        private GovernmentId() { }
    }
}
