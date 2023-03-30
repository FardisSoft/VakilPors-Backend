using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VakilPors.Core.Exceptions
{
    public class BadArgumentException :CustomException
    {
        public BadArgumentException(string message)
            : base(message, null, HttpStatusCode.BadRequest)
        {
        }
    }
}