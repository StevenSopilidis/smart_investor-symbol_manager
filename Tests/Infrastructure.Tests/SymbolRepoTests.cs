using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests
{
    public class SymbolRepoTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly SymbolRepo _repo;

        public SymbolRepoTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);
            _repo    = new SymbolRepo(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateSymbol_Should_Add_New_Symbol()
        {
            var id = Guid.NewGuid();
            var symbol = new Symbol { 
                Id=id, 
                Exchange="TestExchange",
                Ticker = "ABC", 
                Active = true
            };

            var result = await _repo.CreateSymbol(symbol);

            Assert.True(result);
            Assert.Equal(1, _context.Symbols.Count());
            var added = await _context.Symbols.SingleAsync();

            Assert.Equal("ABC", added.Ticker);
            Assert.Equal(id, added.Id);
            Assert.Equal("TestExchange", added.Exchange);
            Assert.True(added.Active);
        }

        [Fact]
        public async Task GetActiveSymbols_Should_Return_Only_Active()
        {
            _context.Symbols.AddRange(
                new Symbol { 
                    Id=Guid.NewGuid(), 
                    Exchange="TestExchange",
                    Ticker = "A", 
                    Active = true 
                },
                new Symbol { 
                    Id=Guid.NewGuid(), 
                    Exchange="TestExchange",
                    Ticker = "B", 
                    Active = false 
                },
                new Symbol { 
                    Id=Guid.NewGuid(), 
                    Exchange="TestExchange",
                    Ticker = "C", 
                    Active = true 
                }
            );
            await _context.SaveChangesAsync();

            var active = await _repo.GetActiveSymbols();

            Assert.Equal(2, active.Count());
            Assert.All(active, s => Assert.True(s.Active));
        }

        [Fact]
        public async Task GetSymbols_Should_Return_All_Symbols()
        {
            _context.Symbols.AddRange(
                new Symbol { 
                    Id=Guid.NewGuid(), 
                    Exchange="TestExchange",
                    Ticker = "X", 
                    Active = true 
                },
                new Symbol {
                    Id=Guid.NewGuid(), 
                    Exchange="TestExchange", 
                    Ticker = "Y", 
                    Active = false 
                }
            );
            await _context.SaveChangesAsync();

            var all = await _repo.GetSymbols();

            Assert.Equal(2, all.Count());
            Assert.Contains(all, s => s.Ticker == "X");
            Assert.Contains(all, s => s.Ticker == "Y");
        }

        [Fact]
        public async Task GetSymbol_Should_Return_Correct_Symbol_Or_Null()
        {
            var id = Guid.NewGuid();
            _context.Symbols.Add(new Symbol{ 
                Id=id, 
                Exchange="TestExchange",
                Ticker = "FOO", 
                Active = true 
            });
            await _context.SaveChangesAsync();

            var found = await _repo.GetSymbol("FOO");
            var missing = await _repo.GetSymbol("BAR");

            Assert.NotNull(found);
            Assert.Equal(id, found.Id);
            Assert.Equal("FOO", found.Ticker);
            Assert.Equal("TestExchange", found.Exchange);
            Assert.Null(missing);
        }

        [Fact]
        public async Task TooggleSymbolActivation_Should_Flip_Active_Flag_And_Save()
        {
            var symbol = new Symbol
            {
                Id       = Guid.NewGuid(),
                Exchange = "TestExchange",
                Ticker   = "TOG",
                Active   = true
            };
            _context.Symbols.Add(symbol);
            await _context.SaveChangesAsync();

            var result1 = await _repo.ToggleSymbolActivation(symbol);
            var updated1 = await _context
                .Symbols
                .AsNoTracking()
                .SingleAsync(s => s.Ticker == "TOG");

            var result2 = await _repo.ToggleSymbolActivation(symbol);
            var updated2 = await _context
                .Symbols
                .AsNoTracking()
                .SingleAsync(s => s.Ticker == "TOG");

            Assert.True(result1, "First toggle should succeed");
            Assert.False(updated1.Active, "After first toggle, Active should be false");

            Assert.True(result2, "Second toggle should succeed");
            Assert.True(updated2.Active, "After second toggle, Active should be true");
        }
    }
}