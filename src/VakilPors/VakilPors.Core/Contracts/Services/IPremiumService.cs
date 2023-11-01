using Pagination.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Dtos.Premium;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface IPremiumService :IScopedDependency
    {
        Task<SubscribedDto> GetPremiumStatus(int user_id);
        Task<Subscribed> ActivatePremium(string premium, int user_id);
        Task DeactivatePremium(int user_id);
        Task UpdatePlan(SubscribedDto subscribedDto);

        Task<bool> DoseUserHaveAnyActiveSubscription(int userId);
        Task<Pagination<Subscribed>> GetAllSubscriptionStatus(PagedParams pagedParams, SortParams sortParams);

    }
}
