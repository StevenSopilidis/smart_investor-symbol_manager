using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class SymbolRepo(AppDbContext context) : ISymbolRepo
    {
        private readonly AppDbContext _context = context;

        public async Task<bool> CreateSymbol(Symbol symbol)
        {
            _context.Symbols.Add(symbol);
            var changed = await _context.SaveChangesAsync();
            return changed > 0;
        }

        public async Task<ICollection<Symbol>> GetActiveSymbols()
        {
            return await _context.Symbols.Where(s => s.Active).ToListAsync();
        }

        public async Task<Symbol?> GetSymbol(string ticker)
        {
            return await _context.Symbols.SingleOrDefaultAsync(s => s.Ticker == ticker);
        }

        public async Task<ICollection<Symbol>> GetSymbols()
        {
            return await _context.Symbols.ToListAsync();
        }

        public async Task<bool> ToggleSymbolActivation(Symbol symbol)
        {
            symbol.Active = !symbol.Active;
            _context.Symbols.Update(symbol);
            return await _context.SaveChangesAsync() > 0; 
        }
    }
}