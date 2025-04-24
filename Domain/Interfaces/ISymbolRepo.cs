using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISymbolRepo
    {
        Task<ICollection<Symbol>> GetSymbols();
        Task<Symbol> GetSymbolByTicker(string ticker);
        Task<bool> CreateSymbol(Symbol symbol);
        Task<bool> TooggleSymbolActivation(Symbol symbol);
        Task<bool> DeleteSymbol(Guid id);
    }
}