using API;
using API.Services;
using Application.Common;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;

namespace Api.Tests
{
    public class ApiServiceTests
    {
        private readonly Mock<ISymbolService> _serviceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly ApiService _apiService;

        public ApiServiceTests()
        {
            _apiService = new ApiService(_serviceMock.Object, _mapperMock.Object);
        }

        // test context used for testing
        private readonly ServerCallContext _context = TestServerCallContext.Create(
            method: "method",
            host: "localhost",
            deadline: DateTime.Now.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "peer",
            authContext: null,
            contextPropagationToken: null!,
            writeHeadersFunc: _ => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: _ => { }
        );


        [Fact]
        public async Task PostSymbol_ThrowsFailedPrecondition_OnBadRequest()
        {
            var request = new PostSymbolRequest
            {
                Ticker = "TEST",
                Exchange = "TEST"
            };
            var dto = new CreateSymbolDto
            {
                Ticker = "TEST",
                Exchange = "TEST"
            };
            _mapperMock.Setup(m => m.Map<CreateSymbolDto>(request)).Returns(dto);

            var error = Error.Create("BadRequest", "Failed precondition");

            _serviceMock.Setup(s => s.CreateSymbol(dto))
                .ReturnsAsync(Result<SymbolDto>.Failure(error, ErrorType.BadRequest));

            var act = async () => await _apiService.PostSymbol(request, _context);
            var ex = await Assert.ThrowsAsync<RpcException>(act);
            ex.StatusCode.Should().Be(StatusCode.FailedPrecondition);
        }

        [Fact]
        public async Task PostSymbol_ReturnsResponse_OnSuccess()
        {
            var request = new PostSymbolRequest { Ticker = "ABC", Exchange = "NYSE" };
            var dto = new CreateSymbolDto { Ticker = "ABC", Exchange = "NYSE" };

            var id = Guid.NewGuid();

            _mapperMock.Setup(m => m.Map<CreateSymbolDto>(request)).Returns(dto);
            _serviceMock.Setup(s => s.CreateSymbol(dto))
                .ReturnsAsync(Result<SymbolDto>.Success(new SymbolDto
                {
                    Active = true,
                    Id = id,
                    Ticker = "ABC",
                    Exchange = "NYSE"
                }));

            var response = await _apiService.PostSymbol(request, _context);

            response.Should().BeOfType<PostSymbolResponse>();
        }


        [Fact]
        public async Task PostSymbol_ThrowsInternal_OnServerError()
        {
            var request = new PostSymbolRequest { Ticker = "ERR", Exchange = "NYSE" };
            var dto = new CreateSymbolDto { Ticker = "ERR", Exchange = "NYSE" };

            _mapperMock.Setup(m => m.Map<CreateSymbolDto>(request)).Returns(dto);
            var error = Error.Create("ServerError", "DB error");
            _serviceMock.Setup(s => s.CreateSymbol(dto))
                .ReturnsAsync(Result<SymbolDto>.Failure(error, ErrorType.SomethingWentWrong));

            Func<Task> act = async () => await _apiService.PostSymbol(request, _context);
            var ex = await Assert.ThrowsAsync<RpcException>(act);
            ex.StatusCode.Should().Be(StatusCode.Internal);
            ex.Status.Detail.Should().Be("DB error");
        }

        [Fact]
        public async Task GetSymbols_ReturnsResponse_OnSuccess()
        {
            var id = Guid.NewGuid();
            var dtoList = new List<SymbolDto> {
                new SymbolDto {
                    Id = id,
                    Ticker = "A",
                    Exchange = "NYSE",
                    Active = true
                }
            };
            _serviceMock.Setup(s => s.GetSymbols())
                .ReturnsAsync(Result<ICollection<SymbolDto>>.Success(dtoList));
            _mapperMock.Setup(m => m.Map<IEnumerable<Symbol>>(dtoList))
                .Returns(new List<Symbol> {
                    new Symbol {
                        Ticker = "A",
                        Exchange = "NYSE",
                        Active = true
                    }
                });

            var response = await _apiService.GetSymbols(new GetSymbolsRequest(), _context);
            response.Should().BeOfType<GetSymbolsResponse>();
            response.Symbols.Should().HaveCount(1);
            response.Symbols[0].Ticker.Should().Be("A");
            response.Symbols[0].Exchange.Should().Be("NYSE");
            response.Symbols[0].Active.Should().Be(true);

        }

        [Fact]
        public async Task GetSymbols_ThrowsInternal_OnFailure()
        {
            var error = Error.Create("FetchError", "Fetch failed");
            _serviceMock.Setup(s => s.GetSymbols())
                .ReturnsAsync(Result<ICollection<SymbolDto>>.Failure(
                    error, ErrorType.SomethingWentWrong
                ));

            Func<Task> act = async () => await _apiService.GetSymbols(new GetSymbolsRequest(), _context);
            var ex = await Assert.ThrowsAsync<RpcException>(act);
            ex.StatusCode.Should().Be(StatusCode.Internal);
            ex.Status.Detail.Should().Be("Fetch failed");
        }

        [Fact]
        public async Task GetActiveSymbols_ReturnsResponse_OnSuccess()
        {
            var id = Guid.NewGuid();
            var dtoList = new List<SymbolDto> {
                new SymbolDto {
                    Id = id,
                    Ticker = "A",
                    Exchange = "NYSE",
                    Active = true
                }
            };

            _serviceMock.Setup(s => s.GetActiveSymbols())
                .ReturnsAsync(Result<ICollection<SymbolDto>>.Success(dtoList));
            _mapperMock.Setup(m => m.Map<IEnumerable<Symbol>>(dtoList))
                .Returns(new List<Symbol> {
                    new Symbol {
                        Ticker = "A",
                        Exchange = "NYSE",
                        Active = true
                    }
                });

            var response = await _apiService.GetActiveSymbols(new GetActiveSymbolsRequest(), _context);
            response.Should().BeOfType<GetActiveSymbolsResponse>();
            response.Symbols.Should().HaveCount(1);
            response.Symbols[0].Ticker.Should().Be("A");
            response.Symbols[0].Exchange.Should().Be("NYSE");
            response.Symbols[0].Active.Should().Be(true);
        }

        [Fact]
        public async Task GetActiveSymbols_ThrowsInternal_OnFailure()
        {
            var error = Error.Create("FetchError", "Active fetch failed");
            _serviceMock.Setup(s => s.GetActiveSymbols())
                .ReturnsAsync(Result<ICollection<SymbolDto>>.Failure(
                    error, ErrorType.SomethingWentWrong
                ));

            Func<Task> act = async () => await _apiService.GetActiveSymbols(
                new GetActiveSymbolsRequest(), _context
            );

            var ex = await Assert.ThrowsAsync<RpcException>(act);
            ex.StatusCode.Should().Be(StatusCode.Internal);
            ex.Status.Detail.Should().Be("Active fetch failed");
        }

        [Fact]
        public async Task ToggleSymbolActivation_ReturnsResponse_OnSuccess()
        {
            var ticker = "TGL";
            _serviceMock.Setup(s => s.ToggleSymbolActivation(ticker))
                .ReturnsAsync(Result<bool>.Success(true));

            var response = await _apiService.ToggleSymbolActivation(
                new ToggleSymbolActivationRequest { Ticker = ticker }, _context
            );
            response.Should().BeOfType<ToggleSymbolActivationResponse>();
        }

        [Fact]
        public async Task ToggleSymbolActivation_ThrowsFailedPrecondition_OnBadRequest()
        {
            var ticker = "BAD";
            var error = Error.Create("BadToggle", "Cannot toggle");
            _serviceMock.Setup(s => s.ToggleSymbolActivation(ticker))
                .ReturnsAsync(Result<bool>.Failure(error, ErrorType.BadRequest));

            Func<Task> act = async () => await _apiService.ToggleSymbolActivation(
                new ToggleSymbolActivationRequest { Ticker = ticker }, _context
            );
            
            var ex = await Assert.ThrowsAsync<RpcException>(act);
            ex.StatusCode.Should().Be(StatusCode.FailedPrecondition);
            ex.Status.Detail.Should().Be("Cannot toggle");
        }

        [Fact]
        public async Task ToggleSymbolActivation_ThrowsInternal_OnServerError()
        {
            var ticker = "ERR";
            var error = Error.Create("ToggleError", "Toggle failed");
            _serviceMock.Setup(s => s.ToggleSymbolActivation(ticker))
                .ReturnsAsync(Result<bool>.Failure(error, ErrorType.SomethingWentWrong));

            Func<Task> act = async () => await _apiService.ToggleSymbolActivation(
                new ToggleSymbolActivationRequest { Ticker = ticker }, _context
            );

            var ex = await Assert.ThrowsAsync<RpcException>(act);
            ex.StatusCode.Should().Be(StatusCode.Internal);
            ex.Status.Detail.Should().Be("Toggle failed");
        }

    }
}