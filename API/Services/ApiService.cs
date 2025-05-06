using Application.Common;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Grpc.Core;

namespace API.Services
{
    public class ApiService : Api.ApiBase
    {
        private readonly ISymbolService _service;
        private readonly IMapper _mapper;

        public ApiService(ISymbolService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public override async Task<PostSymbolResponse> PostSymbol(
            PostSymbolRequest request, 
            ServerCallContext context
        ) 
        {
            var dto = _mapper.Map<CreateSymbolDto>(request);

            var res = await _service.CreateSymbol(dto);
            if (!res.IsSuccess)
                return res.Type == ErrorType.BadRequest?
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, res.Error.Message))
                    : throw new RpcException(new Status(StatusCode.Internal, res.Error.Message));

            return new PostSymbolResponse(); 
        }

        public override async Task<GetSymbolsResponse> GetSymbols(
            GetSymbolsRequest request,
            ServerCallContext context
        ) {
            var res = await _service.GetSymbols();
            if (!res.IsSuccess)
                throw new RpcException(new Status(StatusCode.Internal, res.Error.Message));

            var response = new GetSymbolsResponse
            {
                Symbols = { _mapper.Map<IEnumerable<Symbol>>(res.Value) }
            };

            return response;
        }

        public override async Task<GetActiveSymbolsResponse> GetActiveSymbols(
            GetActiveSymbolsRequest request,
            ServerCallContext context
        ) {
            var res = await _service.GetActiveSymbols();
            if (!res.IsSuccess)
                throw new RpcException(new Status(StatusCode.Internal, res.Error.Message));

            var response = new GetActiveSymbolsResponse
            {
                Symbols = { _mapper.Map<IEnumerable<Symbol>>(res.Value) }
            };

            return response;
        }

        public override async Task<ToggleSymbolActivationResponse> ToggleSymbolActivation(
            ToggleSymbolActivationRequest request,
            ServerCallContext context 
        ) {
            var res = await _service.ToggleSymbolActivation(request.Ticker);
            if (!res.IsSuccess)
                return res.Type == ErrorType.BadRequest?
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, res.Error.Message))
                    : throw new RpcException(new Status(StatusCode.Internal, res.Error.Message));

            return new ToggleSymbolActivationResponse();
        }
    }
}