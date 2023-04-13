using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Payment;
using VakilPors.Core.Domain.Dtos.User;
using VakilPors.Core.Domain.Entities;
using Thread = VakilPors.Core.Domain.Entities.Thread;

namespace VakilPors.Core.Mapper
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<SignUpDto, User>().ReverseMap();
            CreateMap<Lawyer, LawyerDto>();
            CreateMap<User, UserDto>();
            CreateMap<Tranaction, TranactionDto>();
            CreateMap<Thread, ThreadDto>();
            CreateMap<ThreadComment, ThreadCommentDto>();
        }
    }
}