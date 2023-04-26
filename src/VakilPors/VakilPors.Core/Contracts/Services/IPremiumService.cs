using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.Premium;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface IPremiumService :IScopedDependency
    {
        Task<SubscribedDto> GetPremiumStatus(int user_id);
        Task ActivatePremium(SubscribedDto premium, int user_id);
        Task DeactivatePremium(int user_id);
        Task UpdatePlan(SubscribedDto subscribedDto);


    }
}
