namespace Application.Dtos
{
    public class SymbolDto
    {
        public required Guid Id { get; set; }
        public required string  Ticker { get; set; }
        public required string Exchange { get; set; }       
        public required bool Active { get; set; } = true;
    }
}