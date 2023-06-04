using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Transfer;
using AutoMapper;
using FuzzySharp;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
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
        private readonly IChatServices _chatServices;
        private readonly IRateService _rateService;
        public LawyerServices(
            IAppUnitOfWork appUnitOfWork, 
            IMapper mapper, 
            IUserServices userServices, 
            IAwsFileService fileService, 
            IChatServices chatServices, 
            IRateService rateService)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
            _userServices = userServices;
            _fileService = fileService;
            _chatServices = chatServices;
            _rateService = rateService;
        }
        public async Task<IPagedList<Lawyer>> GetLawyers(PagedParams pagedParams, FilterParams filterParams)
        {
            var lawyers = await _appUnitOfWork.LawyerRepo.AsQueryableNoTracking()
            .Include(l => l.User)
            .Where(l => string.IsNullOrEmpty(filterParams.Q) || Fuzz.PartialRatio(l.User.Name, filterParams.Q) > 75 || l.ParvandeNo.Contains(filterParams.Q))
            .OrderBy((string.IsNullOrEmpty(filterParams.Sort) ? "Id" : filterParams.Sort), filterParams.IsAscending)
            .ToPagedListAsync(pagedParams.PageNumber, pagedParams.PageSize);

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

            if (lawyerDto.NationalCardImage is { Length: > 0 })
            {
                var nationalCardImageKey = await _fileService.UploadAsync(lawyerDto.NationalCardImage);
                if (nationalCardImageKey != null)
                    foundLawyer.NationalCardImageUrl = nationalCardImageKey;
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

            return await GetLawyerDtoFormLawyer(foundLawyer);
        }

        public async Task<List<LawyerDto>> GetAllLawyers()
        {
            var lawyers = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .ToListAsync();

            var lawyerDtos = new List<LawyerDto>();
            foreach (var lawyer in lawyers)
                lawyerDtos.Add(await GetLawyerDtoFormLawyer(lawyer));

            return lawyerDtos;
        }
            

        public async Task<LawyerDto> GetLawyerById(int lawyerId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == lawyerId);

            if (lawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            return await GetLawyerDtoFormLawyer(lawyer);
        }

        public async Task<LawyerDto> GetLawyerByUserId(int userId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (lawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            return await GetLawyerDtoFormLawyer(lawyer);
        }

        public async Task<bool> IsLawyer(int userId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            return lawyer != null;
        }

        public async Task<bool> VerifyLawyer(int lawyerId)
        {
            var foundLawyer = await _appUnitOfWork.LawyerRepo.FindAsync(lawyerId);
            if (foundLawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            foundLawyer.IsVerified = true;

            _appUnitOfWork.LawyerRepo.Update(foundLawyer);
            var updateResult = await _appUnitOfWork.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();

            return true;
        }

        private async Task<LawyerDto> GetLawyerDtoFormLawyer(Lawyer lawyer)
        {
            var lawyerDto = _mapper.Map<LawyerDto>(lawyer);

            lawyerDto.NumberOfVerifies = await _appUnitOfWork.ThreadCommentRepo
                .AsQueryable()
                .Where(x => x.IsSetAsAnswer == true && x.UserId == lawyer.UserId)
                .CountAsync();

            lawyerDto.NumberOfAnswers = await _appUnitOfWork.ThreadCommentRepo
                .AsQueryable()
                .Where(x => x.UserId == lawyer.UserId)
                .CountAsync();

            var commentLikes = await _appUnitOfWork.ThreadCommentRepo
                .AsQueryable()
                .Where(x => x.UserId == lawyer.UserId)
                .Select(x => x.LikeCount)
                .SumAsync();

            var threadLikes = await _appUnitOfWork.ThreadCommentRepo
                .AsQueryable()
                .Where(x => x.UserId == lawyer.UserId)
                .Include(x => x.Thread)
                .Select(x => x.Thread)
                .Distinct()
                .Select(t => t.LikeCount)
                .SumAsync();

            lawyerDto.NumberOfLikes = commentLikes + threadLikes;


            var chats = await _chatServices.GetChatsOfUser(lawyer.UserId);
            lawyerDto.NumberOfConsultations = chats.Count;

            lawyerDto.Rating = await _rateService.CalculateRatingAsync(lawyer.Id);

            return lawyerDto;
        }

    }
}