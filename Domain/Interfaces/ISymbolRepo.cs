using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISymbolRepo
    {
        Task<Symbol?> GetSymbol(string ticker);
        Task<ICollection<Symbol>> GetSymbols();
        Task<ICollection<Symbol>> GetActiveSymbols();
        Task<bool> CreateSymbol(Symbol symbol);
        Task<bool> TooggleSymbolActivation(Symbol symbol);
    }
}