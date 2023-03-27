using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Mapper
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<SignUpDto, User>().ReverseMap();

        }
    }
}