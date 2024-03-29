using AutoMapper;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Payment;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Dtos.Premium;
using VakilPors.Core.Domain.Entities;
using ForumThread = VakilPors.Core.Domain.Entities.ForumThread;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Dtos.Event;
using VakilPors.Core.Domain.Dtos.Rate;
using VakilPors.Core.Domain.Dtos.Report;

namespace VakilPors.Core.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<SignUpDto, User>().ReverseMap();
            CreateMap<Lawyer, LawyerDto>();
            CreateMap<User, UserDto>();
            CreateMap<Transaction, TransactionDto>();
            CreateMap<ForumThread, ThreadDto>();
            CreateMap<ThreadComment, ThreadCommentDto>().ReverseMap();
            CreateMap<Premium, PremiumDto>().ReverseMap();
            CreateMap<Subscribed, SubscribedDto>();
            CreateMap<SubscribedDto, Subscribed>();
            CreateMap<LegalDocumentDto, LegalDocument>().ReverseMap();
            CreateMap<RateDto, Rate>().ReverseMap();
            CreateMap<RateUserDto, Rate>().ReverseMap();
            CreateMap<Event, CreateEventDto>().ReverseMap();
            CreateMap<ForumUserDto,User>().ReverseMap();
            CreateMap<ReportDto , Report>().ReverseMap();
            CreateMap<PostReportDto,Report>().ReverseMap();
        }
    }

    public static class MapperExtensions
    {
        public static Pagination<TDestination> ToMappedPagination<TSource, TDestination>(this Pagination<TSource> list,
            IMapper mapper, int limit)
        {
            IEnumerable<TDestination> sourceList =
                mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list.Results);
            Pagination<TDestination> pagedResult =
                new Pagination<TDestination>(sourceList, list.TotalItems, list.CurrentPage, limit);
            return pagedResult;
        }
    }
}