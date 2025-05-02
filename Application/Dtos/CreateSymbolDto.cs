using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class CreateSymbolDto
    {
        [Required]
        [StringLength(12, MinimumLength = 1)]
        public required string Ticker { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 1)]
        public int Exchange { get; set; }
    }
}