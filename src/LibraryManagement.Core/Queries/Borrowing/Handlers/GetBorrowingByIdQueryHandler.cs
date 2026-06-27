using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Borrowing;

namespace LibraryManagement.Core.Queries.Borrowing.Handlers;

public class GetBorrowingByIdQueryHandler : IRequestHandler<GetBorrowingByIdQuery, BorrowingTransactionDto?>
{
    private readonly IBorrowingRepository _repository;
    private readonly IMapper _mapper;
    public GetBorrowingByIdQueryHandler(IBorrowingRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<BorrowingTransactionDto?> Handle(GetBorrowingByIdQuery request, CancellationToken ct)
    {
        var t = await _repository.GetTransactionWithDetailsAsync(request.Id);
        return t == null ? null : _mapper.Map<BorrowingTransactionDto>(t);
    }
}
