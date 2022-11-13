using System.ComponentModel.DataAnnotations;

namespace LoanComparer.Application.Configuration
{
    public class FromEmailConfiguration
    {
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
