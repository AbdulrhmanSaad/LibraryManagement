using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Borrowing;

namespace LibraryManagement.Core.Queries.Borrowing.Handlers;

public class GetBorrowingsByMemberQueryHandler : IRequestHandler<GetBorrowingsByMemberQuery, IEnumerable<BorrowingTransactionDto>>
{
    private readonly IBorrowingRepository _repository;
    private readonly IMapper _mapper;
    public GetBorrowingsByMemberQueryHandler(IBorrowingRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<BorrowingTransactionDto>> Handle(GetBorrowingsByMemberQuery request, CancellationToken ct)
        => _mapper.Map<IEnumerable<BorrowingTransactionDto>>(await _repository.GetByMemberIdAsync(request.MemberId));
}
