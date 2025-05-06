using System.Collections.Concurrent;
using System.Net;
using Application.Common;
using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class SymbolService : ISymbolService
    {
        private readonly ISymbolRepo _repo;
        private readonly IMapper _mapper;

        public SymbolService(ISymbolRepo repo, IMapper mapper)
        {
            _repo = repo;    
            _mapper = mapper;
        }

        public async Task<Result<SymbolDto>> CreateSymbol(CreateSymbolDto dto)
        {
            var symbol = await _repo.GetSymbol(dto.Ticker);
            if (symbol is not null)
                return Result<SymbolDto>.Failure(
                    "DuplicateTicker", 
                    "Provided Ticker already exists",
                    ErrorType.BadRequest
                );

            var newSymbol = _mapper.Map<Symbol>(dto);
            var created = await _repo.CreateSymbol(newSymbol);
            if (!created)
                return Result<SymbolDto>.Failure(
                    "InternalError", 
                    "Could not create Symbol",
                    ErrorType.SomethingWentWrong
                );

            var resultDto = _mapper.Map<SymbolDto>(newSymbol);
            return Result<SymbolDto>.Success(resultDto);
        }

        public async Task<Result<ICollection<SymbolDto>>> GetActiveSymbols()
        {
            var symbols = await _repo.GetActiveSymbols();
            var result = _mapper.Map<ICollection<SymbolDto>>(symbols);
            return Result<ICollection<SymbolDto>>.Success(result);
        }

        public async Task<Result<ICollection<SymbolDto>>> GetSymbols()
        {
            var symbols = await _repo.GetSymbols();
            var result = _mapper.Map<ICollection<SymbolDto>>(symbols);
            return Result<ICollection<SymbolDto>>.Success(result);
        }

        public async Task<Result<bool>> ToggleSymbolActivation(string ticker)
        {
            var symbol = await _repo.GetSymbol(ticker);
            if (symbol is null)
                return Result<bool>.Failure(
                    "InvalidTicker", 
                    "Provided Ticker does not exist",
                    ErrorType.BadRequest
                );

            symbol!.Active = !symbol.Active;
            var changed = await _repo.ToggleSymbolActivation(symbol);
            if (!changed)
                return Result<bool>.Failure(
                    "InternalError", 
                    "Could not save changes made to symbol",
                    ErrorType.SomethingWentWrong
                );

            return Result<bool>.Success(true);
        }
    }
}