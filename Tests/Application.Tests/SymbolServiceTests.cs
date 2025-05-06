using Application.Common;
using Application.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace Application.Tests
{
    public class SymbolServiceTests
    {
        private readonly Mock<ISymbolRepo> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SymbolService _service;

        public SymbolServiceTests()
        {
            _repoMock = new Mock<ISymbolRepo>();
            _mapperMock = new Mock<IMapper>();
            _service = new SymbolService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateSymbol_Should_Fail_When_Ticker_Exists()
        {
            var dto = new CreateSymbolDto { 
                Ticker = "ABC", 
                Exchange="TEST" 
            };
            _repoMock.Setup(r => r.GetSymbol("ABC"))
                .ReturnsAsync(new Symbol
                {
                    Ticker = "ABC",
                    Id = Guid.NewGuid(),
                    Exchange = "TestExchange",
                    Active = true
                });

            var result = await _service.CreateSymbol(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.Type);
        }

        [Fact]
        public async Task CreateSymbol_Should_Fail_When_Repo_Create_Fails()
        {
            var dto = new CreateSymbolDto { 
                Ticker = "XYZ",
                Exchange="TEST"
            };
            _repoMock.Setup(r => r.GetSymbol("XYZ"))
                .ReturnsAsync((Symbol?)null);
            _mapperMock.Setup(m => m.Map<Symbol>(dto))
                .Returns(new Symbol
                {
                    Ticker = "XYZ",
                    Id = Guid.NewGuid(),
                    Exchange = "TestExchange",
                    Active = true
                });
            _repoMock.Setup(r => r.CreateSymbol(It.IsAny<Symbol>()))
                .ReturnsAsync(false);

            var result = await _service.CreateSymbol(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.SomethingWentWrong, result.Type);
        }

        [Fact]
        public async Task CreateSymbol_Should_Succeed_When_Repo_Creates()
        {
            var id = Guid.NewGuid();
            var dto = new CreateSymbolDto { 
                Ticker = "NEW",
                Exchange="TEST" 
            };

            var entity = new Symbol
            {
                Ticker = "NEW",
                Id = id,
                Exchange = "TestExchange",
                Active = true
            };
            var resultDto = new SymbolDto
            {
                Ticker = "NEW",
                Id = id,
                Exchange = "TestExchange",
                Active = true
            };

            _repoMock.Setup(r => r.GetSymbol("NEW"))
                .ReturnsAsync((Symbol?)null);
            _mapperMock.Setup(m => m.Map<Symbol>(dto))
                .Returns(entity);
            _repoMock.Setup(r => r.CreateSymbol(entity))
                .ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<SymbolDto>(entity))
                .Returns(resultDto);

            var result = await _service.CreateSymbol(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(resultDto, result.Value);
        }

        [Fact]
        public async Task GetActiveSymbols_Should_Return_Mapped_Dtos()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();


            var entities = new List<Symbol>
            {
                new Symbol {
                    Ticker = "A",
                    Active = true,
                    Exchange = "TestExchange",
                    Id = id1,
                },
                new Symbol {
                    Ticker = "B",
                    Active = true,
                    Exchange = "TestExchange",
                    Id = id2,
                }
            };
            var dtos = new List<SymbolDto>
            {
                new SymbolDto {
                    Ticker = "A",
                    Active = true,
                    Exchange = "TestExchange",
                    Id = id1,
                },
                new SymbolDto {
                    Ticker = "B",
                    Active = true,
                    Exchange = "TestExchange",
                    Id = id2,
                }
            };

            _repoMock.Setup(r => r.GetActiveSymbols())
                .ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<ICollection<SymbolDto>>(entities))
                .Returns(dtos);

            var result = await _service.GetActiveSymbols();

            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Value);
        }

        [Fact]
        public async Task GetSymbols_Should_Return_Mapped_Dtos()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var entities = new List<Symbol>
            {
                new Symbol {
                    Id = id1,
                    Exchange = "TestExchange",
                    Ticker = "X",
                    Active = false
                },
                new Symbol {
                    Id = id2,
                    Exchange = "TestExchange",
                    Ticker = "Y",
                    Active = true
                }
            };
            var dtos = new List<SymbolDto>
            {
                new SymbolDto {
                    Id = id1,
                    Exchange = "TestExchange",
                    Ticker = "X",
                    Active = false
                },
                new SymbolDto {
                    Id = id1,
                    Exchange = "TestExchange",
                    Ticker = "Y",
                    Active = true
                }
            };

            _repoMock.Setup(r => r.GetSymbols())
                .ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<ICollection<SymbolDto>>(entities))
                .Returns(dtos);

            var result = await _service.GetSymbols();

            Assert.True(result.IsSuccess);
            Assert.Equal(dtos, result.Value);
        }

        [Fact]
        public async Task ToggleSymbolActivation_Should_Fail_When_Symbol_Not_Found()
        {
            _repoMock.Setup(r => r.GetSymbol("NONE"))
                .ReturnsAsync((Symbol?)null);

            var result = await _service.ToggleSymbolActivation("NONE");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.Type);
        }

        [Fact]
        public async Task ToggleSymbolActivation_Should_Succeed_When_Symbol_Found()
        {
            var symbol = new Symbol
            {
                Id = Guid.NewGuid(),
                Exchange = "TestExchange",
                Ticker = "OK",
                Active = true
            };
            _repoMock.Setup(r => r.GetSymbol("OK"))
                .ReturnsAsync(symbol);
            _repoMock.Setup(r => r.ToggleSymbolActivation(symbol))
                .ReturnsAsync(true);

            var result = await _service.ToggleSymbolActivation("OK");

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task ToggleSymbolActivation_Should_Fail_When_Repo_Toggle_Fails()
        {
            var symbol = new Symbol
            {
                Id = Guid.NewGuid(),
                Exchange = "TestExchange",
                Ticker = "FAIL",
                Active = false
            };
            _repoMock.Setup(r => r.GetSymbol("FAIL"))
                .ReturnsAsync(symbol);
            _repoMock.Setup(r => r.ToggleSymbolActivation(symbol))
                .ReturnsAsync(false);

            var result = await _service.ToggleSymbolActivation("FAIL");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.SomethingWentWrong, result.Type);
        }
    }
}