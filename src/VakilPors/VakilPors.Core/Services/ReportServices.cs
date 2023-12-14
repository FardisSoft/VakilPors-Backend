using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Report;
using VakilPors.Contracts.UnitOfWork;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MimeKit.Encodings;
using Microsoft.AspNetCore.Mvc;

namespace VakilPors.Core.Services
{
    public class ReportServices : IReportServices
    {
        private readonly IAppUnitOfWork _appUnitOfWork;
        private readonly IMapper _mapper;

        public ReportServices(IAppUnitOfWork appUnitOfWork,IMapper mapper)
        {
            _appUnitOfWork = appUnitOfWork;
            _mapper = mapper;
        }
        public async Task<List<ReportDto>> GetAllLawyers()
        {
            var reports = await _appUnitOfWork.ReportRepo
                .AsQueryable()
                .Include(x  => x.User)
                .Include(x=>x.ThreadComment).ThenInclude(u=>u.User)
                .ToListAsync();

            var ReportDtos =new List<ReportDto>();
            foreach (var report in reports)
                ReportDtos.Add(_mapper.Map<ReportDto>(report));
                // ReportDtos.Add(await GetReportDtoFormReport(report));
            

            return ReportDtos;
        }
        public async Task<bool> PostReport(PostReportDto postReportDto)
        {
            var report = _mapper.Map<PostReportDto, Report>(postReportDto);

            // report.User = await _appUnitOfWork.UserRepo.(reportDto.User);
            // report.ThreadComment = await _appUnitOfWork.ThreadCommentRepo.FindAsync(reportDto.CommentId);


            // Save the Report to the database
            await _appUnitOfWork.ReportRepo.AddAsync(report);
            await _appUnitOfWork.SaveChangesAsync();

            // Return a successful response
            return true;
        }
        public async Task<bool> DeleteReport(int reportId)
        {
            try
            {
                // Retrieve the report entity by ID
                // var reportEntity = await _appUnitOfWork.ReportRepo.GetByIdAsync(reportId);
                var reportEntity = await _appUnitOfWork.ReportRepo
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == reportId);

                if (reportEntity == null)
                {
                    return false; // Report not found
                }

                // Delete the report entity from the repository
                _appUnitOfWork.ReportRepo.Remove(reportEntity);

                // Save changes to the database
                await _appUnitOfWork.SaveChangesAsync();

                return true; // Report successfully deleted
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return false; // Report deletion failed
            }
        }

    }

}