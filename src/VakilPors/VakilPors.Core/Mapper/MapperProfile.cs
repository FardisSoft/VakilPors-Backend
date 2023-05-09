using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Payment;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Dtos.Premium;
using VakilPors.Core.Domain.Entities;
using ForumThread = VakilPors.Core.Domain.Entities.ForumThread;
using X.PagedList;
using VakilPors.Core.Domain.Dtos.Case;
using VakilPors.Core.Domain.Dtos.Rate;

namespace VakilPors.Core.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<SignUpDto, User>().ReverseMap();
            CreateMap<Lawyer, LawyerDto>();
            CreateMap<User, UserDto>();
            CreateMap<Tranaction, TranactionDto>();
            CreateMap<ForumThread, ThreadDto>();
            CreateMap<ThreadComment, ThreadCommentDto>();
            CreateMap<Premium, PremiumDto>().ReverseMap();
            CreateMap<Subscribed, SubscribedDto>();
            CreateMap<SubscribedDto, Subscribed>();
            CreateMap<LegalDocumentDto, LegalDocument>().ReverseMap();
            CreateMap<RateDto , Rate>().ReverseMap();

        }
    }
    public static class MapperExtensions
    {
        public static IPagedList<TDestination> ToMappedPagedList<TSource, TDestination>(this IPagedList<TSource> list, IMapper mapper)
        {
            IEnumerable<TDestination> sourceList = mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list);
            IPagedList<TDestination> pagedResult = new StaticPagedList<TDestination>(sourceList, list.GetMetaData());
            return pagedResult;

        }
    }
}