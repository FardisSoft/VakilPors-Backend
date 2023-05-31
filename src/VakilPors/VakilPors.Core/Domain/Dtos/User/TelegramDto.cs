using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakilPors.Core.Domain.Dtos.User
{
    public record TelegramDto
    {
        public string phone_number { get; set; }
        public string chat_id { get; set; }
    }
}
