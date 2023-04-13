using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Params;
using X.PagedList;
using VakilPors.Shared.Extensions;
using VakilPors.Core.Domain.Entities;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using VakilPors.Core.Domain.Dtos.Lawyer;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VakilPors.Core.Services
{
    public class LawyerServices : ILawyerServices
    {
        private readonly IMapper mapper;

        private readonly IAppUnitOfWork appUnitOfWork;
        public LawyerServices(IAppUnitOfWork appUnitOfWork)
        {
            this.appUnitOfWork = appUnitOfWork;

        }
        public async Task<IPagedList<Lawyer>> GetLawyers(PagedParams pagedParams, FilterParams filterParams)
        {
            return await appUnitOfWork.LawyerRepo.AsQueryableNoTracking()
            .Include(l => l.User)
            .Where(l => string.IsNullOrEmpty(filterParams.Q) || Fuzz.PartialRatio(l.User.Name, filterParams.Q) > 75 || l.ParvandeNo.Contains(filterParams.Q))
            .OrderBy((string.IsNullOrEmpty(filterParams.Sort) ? "Id" : filterParams.Sort), filterParams.IsAscending)
            .ToPagedListAsync(pagedParams.PageNumber, pagedParams.PageSize);
        }
        public async Task<Lawyer> GetLawyerByID(int id)
        {
            //return appUnitOfWork.LawyerRepo.AsQueryableNoTracking().Where(l => l.User.Id == id).FirstOrDefault();
            return await appUnitOfWork.LawyerRepo.FindAsync(id);

        }
        public async Task EditLawyer(int id, LawyerDto lawyerDto)
        {
            var lawyer = await appUnitOfWork.LawyerRepo.FindAsync(id);
            lawyer.Rating = lawyerDto.Rating;
            lawyer.ParvandeNo = lawyerDto.ParvandeNo;
            lawyer.IsAuthorized = lawyerDto.IsAuthorized;
            lawyer.ProfilePicture = lawyerDto.ProfilePicture;
            lawyer.ProfileBackgroundPicture = lawyerDto.ProfileBackgroundPicture;
            lawyer.IsOnline = lawyerDto.IsOnline;
            lawyer.Name = lawyerDto.Name;
            lawyer.Field = lawyerDto.Field;
            lawyer.NumberOfRates = lawyerDto.NumberOfRates;
            lawyer.city = lawyerDto.city;
            lawyer.Grade = lawyerDto.Grade;
            lawyer.LicenseNumber = lawyerDto.LicenseNumber;
            lawyer.Specialists = lawyerDto.Specialists;
            lawyer.YearsOfExperience = lawyerDto.YearsOfExperience;
            lawyer.Gender = lawyerDto.Gender;
            lawyer.EducationField = lawyerDto.EducationField;
            lawyer.OfficeAddress = lawyerDto.OfficeAddress;
            lawyer.NumberOfConsultations = lawyerDto.NumberOfConsultations;
            lawyer.NumberOfAnswers = lawyerDto.NumberOfAnswers;
            lawyer.NumberOfLikes = lawyerDto.NumberOfLikes;
            lawyer.NumberOfVerifies = lawyerDto.NumberOfVerifies;
            lawyer.AboutMe = lawyerDto.AboutMe;
            lawyer.CallingCard = lawyerDto.CallingCard;
            lawyer.ResumeLink = lawyerDto.ResumeLink;
            await appUnitOfWork.SaveChangesAsync();
        }
    }
}