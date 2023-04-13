using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Exceptions;

namespace VakilPors.Web.Controllers
{
    public class MyControllerBase : ControllerBase
    {
        protected string getPhoneNumber()
        {
            var phoneNumber = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (phoneNumber == null)
            {
                throw new NotFoundException("user not found");
            }
            return phoneNumber;
        }
        protected int getUserId()
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null)
            {
                throw new NotFoundException("user not found");
            }
            return Convert.ToInt32(userId);
        }
        protected string getQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
        protected string getBaseRoute(){
            var baseRoute = $"{Request.Scheme}://{Request.Host.Value}";
            return baseRoute;
        }
    }
}