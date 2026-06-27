using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Members;

namespace LibraryManagement.Core.Queries.Members.Handlers;

public class GetMemberBorrowingsQueryHandler : IRequestHandler<GetMemberBorrowingsQuery, IEnumerable<BorrowingTransactionDto>>
{
    private readonly IBorrowingRepository _repository;
    private readonly IMapper _mapper;
    public GetMemberBorrowingsQueryHandler(IBorrowingRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }
    public async Task<IEnumerable<BorrowingTransactionDto>> Handle(GetMemberBorrowingsQuery request, CancellationToken ct)
    {
        var transactions = await _repository.GetByMemberIdAsync(request.MemberId);
        return _mapper.Map<IEnumerable<BorrowingTransactionDto>>(transactions);
    }
}
