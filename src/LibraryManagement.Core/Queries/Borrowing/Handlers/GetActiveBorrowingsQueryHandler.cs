using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Borrowing;

namespace LibraryManagement.Core.Queries.Borrowing.Handlers;

public class GetActiveBorrowingsQueryHandler : IRequestHandler<GetActiveBorrowingsQuery, IEnumerable<BorrowingTransactionDto>>
{
    private readonly IBorrowingRepository _repository;
    private readonly IMapper _mapper;
    public GetActiveBorrowingsQueryHandler(IBorrowingRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<BorrowingTransactionDto>> Handle(GetActiveBorrowingsQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<BorrowingTransactionDto>>(await _repository.GetActiveBorrowingsAsync());
}
