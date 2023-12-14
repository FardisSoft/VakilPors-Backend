using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Report;
using VakilPors.Contracts.UnitOfWork;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MimeKit.Encodings;

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
                .Include(x=>x.ThreadComment)//.ThenInclude(u=>u.User)
                .ToListAsync();

            var ReportDtos =new List<ReportDto>();
            foreach (var report in reports)
                ReportDtos.Add(_mapper.Map<ReportDto>(report));
                // ReportDtos.Add(await GetReportDtoFormReport(report));
            

            return ReportDtos;
        }

    }

}