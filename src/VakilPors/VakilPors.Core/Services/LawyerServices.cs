using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using X.PagedList;
using VakilPors.Shared.Extensions;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services
{
    public class LawyerServices : ILawyerServices
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IUserServices _userServices;
        private readonly IMapper _mapper;
        public LawyerServices(IAppUnitOfWork appUnitOfWork, IMapper mapper, IUserServices userServices)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
            _userServices = userServices;
        }
        public async Task<IPagedList<Lawyer>> GetLawyers(PagedParams pagedParams, FilterParams filterParams)
        {
            return await _appUnitOfWork.LawyerRepo.AsQueryableNoTracking()
            .Include(l => l.User)
            .Where(l => string.IsNullOrEmpty(filterParams.Q) || Fuzz.PartialRatio(l.User.Name, filterParams.Q) > 75 || l.ParvandeNo.Contains(filterParams.Q))
            .OrderBy((string.IsNullOrEmpty(filterParams.Sort) ? "Id" : filterParams.Sort), filterParams.IsAscending)
            .ToPagedListAsync(pagedParams.PageNumber, pagedParams.PageSize);
        }

        public async Task<LawyerDto> UpdateLawyer(LawyerDto lawyerDto)
        {
            var foundLawyer = await _appUnitOfWork.LawyerRepo.FindAsync(lawyerDto.Id);
            if (foundLawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            foundLawyer.ParvandeNo = lawyerDto.ParvandeNo;
            foundLawyer.ProfileImageUrl = lawyerDto.ProfileImageUrl;
            foundLawyer.Title = lawyerDto.Title;
            foundLawyer.City = lawyerDto.City;
            foundLawyer.Grade = lawyerDto.Grade;
            foundLawyer.LicenseNumber = lawyerDto.LicenseNumber;
            foundLawyer.MemberOf = lawyerDto.MemberOf;
            foundLawyer.YearsOfExperience = lawyerDto.YearsOfExperience;
            foundLawyer.OfficeAddress = lawyerDto.OfficeAddress;
            foundLawyer.Education = lawyerDto.Education;
            foundLawyer.AboutMe = lawyerDto.AboutMe;
            foundLawyer.CallingCardImageUrl = lawyerDto.CallingCardImageUrl;
            foundLawyer.ResumeLink = lawyerDto.ResumeLink;
            foundLawyer.Specialties = lawyerDto.Specialties;
            foundLawyer.ProfileBackgroundPictureUrl = lawyerDto.ProfileBackgroundPictureUrl;
            foundLawyer.NumberOfRates = lawyerDto.NumberOfRates;
            foundLawyer.Gender = lawyerDto.Gender;


            _appUnitOfWork.LawyerRepo.Update(foundLawyer);
            var updateResult = await _appUnitOfWork.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();

            await _userServices.UpdateUser(lawyerDto.User);

            return lawyerDto;
        }

        public async Task<List<LawyerDto>> GetAllLawyers()
            => await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .Select(x => _mapper.Map<LawyerDto>(x))
                .ToListAsync();

        public async Task<LawyerDto> GetLawyerById(int lawyerId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == lawyerId);

            if (lawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            return _mapper.Map<LawyerDto>(lawyer);
        }

        public async Task<LawyerDto> GetLawyerByUserId(int userId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (lawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            return _mapper.Map<LawyerDto>(lawyer);
        }

        public async Task<bool> IsLawyer(int userId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return lawyer == null;
        }
    }
}