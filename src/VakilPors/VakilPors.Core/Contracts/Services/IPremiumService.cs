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
        public Task<PremiumDto> GetPremiumStatus(int user_id);
        public Task ActivatePremium(PremiumDto premium, int user_id);
        public Task DeactivatePremium(int user_id);

        
    }
}
