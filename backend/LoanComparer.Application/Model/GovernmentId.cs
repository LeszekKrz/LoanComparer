using LoanComparer.Application.DTO;

namespace LoanComparer.Application.Model
{
    public record GovernmentId(string Type, string Value)
    {
        public static GovernmentId FromDto(GovernmentIdDTO dto)
        {
            return new(dto.Type, dto.Value);
        }

        public GovernmentIdDTO ToDto()
        {
            return new(Type, Value);
        }
    }
    
    public class GovernmentIdEntity
    {
        public string Id { get; init; }
        public string Type { get; private set; }
        public string Value { get; private set; }
        public User User { get; private set; }

        public GovernmentIdEntity(GovernmentIdDTO governmentIdDTO)
        {
            Type = governmentIdDTO.Type;
            Value = governmentIdDTO.Value;
        }

        public GovernmentIdEntity(string type, string value)
        {
            Type = type;
            Value = value;
        }

        private GovernmentIdEntity() { }
    }
}
