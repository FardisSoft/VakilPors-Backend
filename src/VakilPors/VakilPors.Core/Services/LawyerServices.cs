using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Transfer;
using AutoMapper;
using FuzzySharp;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.User;
using X.PagedList;
using VakilPors.Shared.Extensions;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Exceptions;
using VakilPors.Core.Domain.Dtos.Search;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VakilPors.Core.Services
{
    public class LawyerServices : ILawyerServices
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IUserServices _userServices;
        private readonly IMapper _mapper;
        private readonly IAwsFileService _fileService;
        private readonly IChatServices _chatServices;
        private readonly IWalletServices _walletServices;
        public LawyerServices(
            IAppUnitOfWork appUnitOfWork, 
            IMapper mapper, 
            IUserServices userServices, 
            IAwsFileService fileService, 
            IChatServices chatServices,
            IWalletServices walletServices)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
            _userServices = userServices;
            _fileService = fileService;
            _chatServices = chatServices;
            _walletServices = walletServices;
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

            foundLawyer.Title = lawyerDto.Title;
            foundLawyer.City = lawyerDto.City;
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

        public async Task<Lawyer> sample (int userId)
        {
            var lawyer = await _appUnitOfWork.LawyerRepo
                .AsQueryable()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (lawyer == null)
                throw new BadArgumentException("Lawyer Not Found");
            return lawyer;
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

        public async Task AddToken(int lawyerId, int tokens)
        {
            var foundLawyer = await _appUnitOfWork.LawyerRepo.FindAsync(lawyerId);
            if (foundLawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            foundLawyer.Tokens += tokens;

            _appUnitOfWork.LawyerRepo.Update(foundLawyer);
            var updateResult = await _appUnitOfWork.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();
        }

        public async Task SetTokens(int lawyerId, int tokens)
        {
            var foundLawyer = await _appUnitOfWork.LawyerRepo.FindAsync(lawyerId);
            if (foundLawyer == null)
                throw new BadArgumentException("Lawyer Not Found");

            foundLawyer.Tokens = tokens;

            _appUnitOfWork.LawyerRepo.Update(foundLawyer);
            var updateResult = await _appUnitOfWork.SaveChangesAsync();
            if (updateResult <= 0)
                throw new Exception();
        }

        public async Task<bool> TransferToken(int userId)
        {
            var lawyer = await GetLawyerByUserId(userId);
            var tokenValue = 10;
            var remainingTokens = lawyer.Tokens % tokenValue;
            var tokensToTransfer = lawyer.Tokens - remainingTokens;
            var transferAmount = tokensToTransfer * 1000;

            await _walletServices.AddBalance(lawyer.User.Id, transferAmount);
            await SetTokens(lawyer.Id, remainingTokens);

            return true;
        }

        public async Task<LawyerDto> GetLawyerDtoFormLawyer(Lawyer lawyer)
        {
            var lawyerDto = _mapper.Map<LawyerDto>(lawyer);

            var query = _appUnitOfWork.ThreadCommentRepo.AsQueryable().Where(x => x.UserId == lawyer.UserId);
            var query2 = _appUnitOfWork.ThreadCommentRepo.AsQueryable().Where(x => x.IsSetAsAnswer == true && x.UserId == lawyer.UserId);
            var query3 = _appUnitOfWork.ThreadCommentRepo.AsQueryable().Where(x => x.UserId == lawyer.UserId).
            Include(x=>x.Thread).Select(x => x.Thread).Distinct().Select(t => t.LikeCount);

            //try
            //{
            //    lawyerDto.NumberOfVerifies = await query2.CountAsync();
            //}
            //catch (Exception ex)
            //{
            //    await Console.Out.WriteLineAsync();
            //}
            var commentLikes = 0;
            var threadLikes = 0;
            lawyerDto.NumberOfVerifies = await query2.CountAsync();
            lawyerDto.NumberOfAnswers = await query.CountAsync();
            commentLikes = await query.Select(x => x.LikeCount).SumAsync();

            //try
            //{
            //    lawyerDto.NumberOfAnswers = await query.CountAsync();
            //}
            //catch (Exception ex) 
            //{
            //    await Console.Out.WriteLineAsync();
            //}

            
            //try
            //{
            //    commentLikes = await query.Select(x => x.LikeCount).SumAsync();
            //}
            //catch (Exception ex)
            //{
            //    await Console.Out.WriteLineAsync();
            //}

            try
            {
                threadLikes = await query3.SumAsync();
            }

            catch (Exception ex) 
            {
                await Console.Out.WriteLineAsync();
            }

            //lawyerDto.NumberOfVerifies = await _appUnitOfWork.ThreadCommentRepo
            //    .AsQueryable()
            //    .Where(x => x.IsSetAsAnswer == true && x.UserId == lawyer.UserId)
            //    .CountAsync();

            //lawyerDto.NumberOfAnswers = await _appUnitOfWork.ThreadCommentRepo
            //    .AsQueryable()
            //    .Where(x => x.UserId == lawyer.UserId)
            //    .CountAsync();

            //var commentLikes = await _appUnitOfWork.ThreadCommentRepo
            //    .AsQueryable()
            //    .Where(x => x.UserId == lawyer.UserId)
            //    .Select(x => x.LikeCount)
            //    .SumAsync();


            //var threadLikes = 0;
            //var query = _appUnitOfWork.ThreadCommentRepo.AsQueryable().Where(x => x.UserId == lawyer.UserId).
            //    Include(x=>x.Thread).Select(x => x.Thread).Distinct().Select(t => t.LikeCount);
            //try
            //{
            //    threadLikes = await query.SumAsync();
            //}
            //catch (System.Reflection.TargetInvocationException ex)
            //{
            //    await Console.Out.WriteLineAsync();
            //}




            lawyerDto.NumberOfLikes = commentLikes + threadLikes;


            var chats = await _chatServices.GetChatsOfUser(lawyer.UserId);
            if (chats != null)
            {
                lawyerDto.NumberOfConsultations = chats.Count;
            }

            return lawyerDto;
        }

        public async Task<Pagination<Lawyer>> GetLawyers(PagedParams pagedParams, SortParams sortParams , LawyerFilterParams filterParams)
        {
            var filteredLawyers = _appUnitOfWork.LawyerRepo.AsQueryableNoTracking()
            .Include(l => l.User)
            .Where(l => string.IsNullOrEmpty(filterParams.Name) || Fuzz.PartialRatio(l.User.Name, filterParams.Name) > 75 );

            if (filterParams.Rating != null)
            {
                filteredLawyers = filteredLawyers.Where(x => x.Rating >= filterParams.Rating);
            }
            if (!string.IsNullOrEmpty(filterParams.Title))
            {
                filteredLawyers = filteredLawyers.Where(x => x.Title == filterParams.Title);
            }
            if (!string.IsNullOrEmpty(filterParams.City))
            {
                filteredLawyers = filteredLawyers.Where(x => x.City == filterParams.City);
            }
            if (!string.IsNullOrEmpty(filterParams.MemberOf))
            {
                filteredLawyers = filteredLawyers.Where(x => x.MemberOf == filterParams.MemberOf);
            }
            if (!string.IsNullOrEmpty(filterParams.LicenseNumber))
            {
                filteredLawyers = filteredLawyers.Where(x => x.LicenseNumber == filterParams.LicenseNumber);
            }
            if (!string.IsNullOrEmpty(filterParams.Gender))
            {
                filteredLawyers = filteredLawyers.Where(x => x.Gender == filterParams.Gender);
            }

            return await filteredLawyers.AsPaginationAsync(pagedParams.PageNumber, pagedParams.PageSize, (string.IsNullOrEmpty(sortParams.Sort) ? "Id" : sortParams.Sort), !sortParams.IsAscending);
        }

        public async Task<List<LawyerCityCountDto>> GetLawyerCityCounts()
        {
            return await _appUnitOfWork.LawyerRepo.AsQueryableNoTracking().GroupBy(l => l.City).Select(g => new LawyerCityCountDto()
            {
                City = g.Key,
                Count = g.Count()
            }).ToListAsync();
        }
        
    }
}