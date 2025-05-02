using Application.Common;
using Application.Dtos;

namespace Application.Interfaces
{
    public interface ISymbolService
    {
        Task<Result<SymbolDto>> CreateSymbol(CreateSymbolDto dto);
        Task<Result<ICollection<SymbolDto>>> GetSymbols();
        Task<Result<ICollection<SymbolDto>>> GetActiveSymbols();
        Task<Result<bool>> ToggleSymbolActivation(string ticker);
    }
}