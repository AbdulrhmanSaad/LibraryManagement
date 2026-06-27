using AutoMapper;
using MediatR;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Core.Queries.Borrowing;

namespace LibraryManagement.Core.Queries.Borrowing.Handlers;

public class GetAllBorrowingsQueryHandler : IRequestHandler<GetAllBorrowingsQuery, IEnumerable<BorrowingTransactionDto>>
{
    private readonly IBorrowingRepository _repository;
    private readonly IMapper _mapper;
    public GetAllBorrowingsQueryHandler(IBorrowingRepository repository, IMapper mapper) { _repository = repository; _mapper = mapper; }

    public async Task<IEnumerable<BorrowingTransactionDto>> Handle(GetAllBorrowingsQuery request, CancellationToken ct)
    {
        var all = await _repository.GetAllAsync();
        var enriched = new List<BorrowingTransactionDto>();
        foreach (var t in all)
        {
            var detail = await _repository.GetTransactionWithDetailsAsync(t.Id);
            if (detail != null) enriched.Add(_mapper.Map<BorrowingTransactionDto>(detail));
        }
        return enriched.OrderByDescending(t => t.BorrowDate);
    }
}
