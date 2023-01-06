using System.ComponentModel.DataAnnotations;

namespace LoanComparer.Application.Configuration
{
    public sealed class FromEmailConfiguration
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string EmailAddress { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = null!;
    }
}
