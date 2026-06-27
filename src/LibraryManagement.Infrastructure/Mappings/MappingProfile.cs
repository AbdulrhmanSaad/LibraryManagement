using AutoMapper;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(d => d.Authors, o => o.MapFrom(s => s.Authors.Select(a => $"{a.FirstName} {a.LastName}")))
            .ForMember(d => d.Categories, o => o.MapFrom(s => s.Categories.Select(c => c.Name)))
            .ForMember(d => d.PublisherName, o => o.MapFrom(s => s.Publisher != null ? s.Publisher.Name : null));

        CreateMap<CreateBookDto, Book>()
            .ForMember(d => d.Authors, o => o.Ignore())
            .ForMember(d => d.Categories, o => o.Ignore());

        CreateMap<Member, MemberDto>();
        CreateMap<CreateMemberDto, Member>()
            .ForMember(d => d.MemberNumber, o => o.Ignore());

        CreateMap<AppUser, UserDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.UserName))
            .ForMember(d => d.Role, o => o.Ignore());
        CreateMap<CreateUserDto, AppUser>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.Username))
            .ForMember(d => d.PasswordHash, o => o.Ignore());

        CreateMap<BorrowingTransaction, BorrowingTransactionDto>()
            .ForMember(d => d.BookTitle, o => o.MapFrom(s => s.Book.Title))
            .ForMember(d => d.BookISBN, o => o.MapFrom(s => s.Book.ISBN))
            .ForMember(d => d.MemberName, o => o.MapFrom(s => $"{s.Member.FirstName} {s.Member.LastName}"))
            .ForMember(d => d.MemberNumber, o => o.MapFrom(s => s.Member.MemberNumber))
            .ForMember(d => d.BorrowedByName, o => o.MapFrom(s => s.BorrowedBy.FullName));

        CreateMap<ActivityLog, ActivityLogDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User.UserName));

        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ParentCategoryName, o => o.MapFrom(s => s.ParentCategory != null ? s.ParentCategory.Name : null));
        CreateMap<CreateCategoryDto, Category>();

        CreateMap<Publisher, PublisherDto>();
        CreateMap<CreatePublisherDto, Publisher>();
        CreateMap<UpdatePublisherDto, Publisher>();

        CreateMap<Author, AuthorDto>();
        CreateMap<CreateAuthorDto, Author>();
        CreateMap<UpdateAuthorDto, Author>();
    }
}
