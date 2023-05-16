using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.Rate;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface IRateService :IScopedDependency
    {
        Task<RateDto> GetRateAsync(int user_id , int lawyer_id);
        Task<List<RateUserDto>> GetAllRatesAsync(int lawyer_id);
        Task AddRateAsync(RateDto rate,int user_id , int lawyer_id);
        Task UpdateRateAsync(RateDto rate ,int user_id , int lawyer_id);
        Task DeleteRateAsync(int rate_id);
        Task<double> CalculateRatingAsync(int laywer_id);



    }
}
