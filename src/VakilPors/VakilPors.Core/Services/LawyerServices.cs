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
        private readonly IAwsFileService _fileService;
        public LawyerServices(IAppUnitOfWork appUnitOfWork, IMapper mapper, IUserServices userServices, IAwsFileService fileService)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
            _userServices = userServices;
            _fileService = fileService;
        }
        public async Task<IPagedList<Lawyer>> GetLawyers(PagedParams pagedParams, FilterParams filterParams)
        {
            var lawyers = await _appUnitOfWork.LawyerRepo.AsQueryableNoTracking()
            .Include(l => l.User)
            .Where(l => string.IsNullOrEmpty(filterParams.Q) || Fuzz.PartialRatio(l.User.Name, filterParams.Q) > 75 || l.ParvandeNo.Contains(filterParams.Q))
            .OrderBy((string.IsNullOrEmpty(filterParams.Sort) ? "Id" : filterParams.Sort), filterParams.IsAscending)
            .ToPagedListAsync(pagedParams.PageNumber, pagedParams.PageSize);

            foreach (var lawyer in lawyers)
            {
                ReplaceFileCodeWithUrl(lawyer);
            }

            return lawyers;
        }

        public async Task<LawyerDto> UpdateLawyer(LawyerDto lawyerDto)
        {
            var foundLawyer = await _appUnitOfWork.LawyerRepo.FindAsync(lawyerDto.Id);
            if (foundLawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            if (lawyerDto.CallingCardImage is { Length: > 0 })
            {
                var callingCardKey = await _fileService.UploadAsync(lawyerDto.CallingCardImage);
                if(callingCardKey != null)
                    foundLawyer.CallingCardImageUrl = callingCardKey;
            }

            if (lawyerDto.ProfileBackgroundPicture is { Length: > 0 })
            {
                var profileBackgroundKey = await _fileService.UploadAsync(lawyerDto.ProfileBackgroundPicture);
                if (profileBackgroundKey != null)
                    foundLawyer.ProfileBackgroundPictureUrl = profileBackgroundKey;
            }

            if (lawyerDto.Resume is { Length: > 0 })
            {
                var resumeKey = await _fileService.UploadAsync(lawyerDto.Resume);
                if (resumeKey != null)
                    foundLawyer.ResumeLink = resumeKey;
            }

            foundLawyer.ParvandeNo = lawyerDto.ParvandeNo;
            foundLawyer.Title = lawyerDto.Title;
            foundLawyer.City = lawyerDto.City;
            foundLawyer.Grade = lawyerDto.Grade;
            foundLawyer.LicenseNumber = lawyerDto.LicenseNumber;
            foundLawyer.MemberOf = lawyerDto.MemberOf;
            foundLawyer.YearsOfExperience = lawyerDto.YearsOfExperience;
            foundLawyer.OfficeAddress = lawyerDto.OfficeAddress;
            foundLawyer.Education = lawyerDto.Education;
            foundLawyer.AboutMe = lawyerDto.AboutMe;
            foundLawyer.Specialties = lawyerDto.Specialties;
            foundLawyer.NumberOfRates = lawyerDto.NumberOfRates;
            foundLawyer.Gender = lawyerDto.Gender;


            _appUnitOfWork.LawyerRepo.Update(foundLawyer);
            var updateResult = await _appUnitOfWork.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();

            if(lawyerDto.User != null && lawyerDto.User.Id > 0)
                await _userServices.UpdateUser(lawyerDto.User);

            return ReplaceFileCodeWithUrl(_mapper.Map<LawyerDto>(foundLawyer));
        }

        public async Task<List<LawyerDto>> GetAllLawyers()
        {
            var lawyers = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .Select(x => _mapper.Map<LawyerDto>(x))
                .ToListAsync();

            foreach (var lawyer in lawyers)
            {
                ReplaceFileCodeWithUrl(lawyer);
            }

            return lawyers;
        }
            

        public async Task<LawyerDto> GetLawyerById(int lawyerId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == lawyerId);

            if (lawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            return ReplaceFileCodeWithUrl(_mapper.Map<LawyerDto>(lawyer));
        }

        public async Task<LawyerDto> GetLawyerByUserId(int userId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (lawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            return ReplaceFileCodeWithUrl(_mapper.Map<LawyerDto>(lawyer));
        }

        public async Task<bool> IsLawyer(int userId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return lawyer != null;
        }

        private LawyerDto ReplaceFileCodeWithUrl(LawyerDto lawyerDto)
        {
            if (lawyerDto.CallingCardImageUrl != null)
                lawyerDto.CallingCardImageUrl = _fileService.GetFileUrl(lawyerDto.CallingCardImageUrl);

            if (lawyerDto.ProfileBackgroundPictureUrl != null)
                lawyerDto.ProfileBackgroundPictureUrl = _fileService.GetFileUrl(lawyerDto.ProfileBackgroundPictureUrl);

            if (lawyerDto.ResumeLink != null)
                lawyerDto.ResumeLink = _fileService.GetFileUrl(lawyerDto.ResumeLink);

            return lawyerDto;
        }

        private Lawyer ReplaceFileCodeWithUrl(Lawyer lawyer)
        {
            if (lawyer.CallingCardImageUrl != null)
                lawyer.CallingCardImageUrl = _fileService.GetFileUrl(lawyer.CallingCardImageUrl);

            if (lawyer.ProfileBackgroundPictureUrl != null)
                lawyer.ProfileBackgroundPictureUrl = _fileService.GetFileUrl(lawyer.ProfileBackgroundPictureUrl);

            if (lawyer.ResumeLink != null)
                lawyer.ResumeLink = _fileService.GetFileUrl(lawyer.ResumeLink);

            return lawyer;
        }

    }
}