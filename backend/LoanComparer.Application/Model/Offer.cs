using System.ComponentModel.DataAnnotations;
using LoanComparer.Application.DTO.OfferDTO;

namespace LoanComparer.Application.Model;

public sealed class Offer
{
    public Guid Id { get; init; }
    
    public OfferEntity ToEntity()
    {
        throw new NotImplementedException();
    }

    public static Offer FromEntity(OfferEntity entity)
    {
        throw new NotImplementedException();
    }

    public OfferResponse ToDto()
    {
        return new()
        {
            Id = Id
        };
    }
}

public sealed class OfferEntity
{
    [Key]
    [Required]
    public Guid Id { get; init; }
}