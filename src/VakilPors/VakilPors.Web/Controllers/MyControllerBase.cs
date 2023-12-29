using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Exceptions;

namespace VakilPors.Api.Controllers
{
    public class MyControllerBase : ControllerBase
    {
        protected string GetPhoneNumber()
        {
            var phoneNumber = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (phoneNumber == null)
            {
                throw new NotFoundException("user not found");
            }
            return phoneNumber;
        }
        protected int GetUserId()
        {
            var userId = User.FindFirstValue("uid");
            if (userId == null)
            {
                throw new NotFoundException("user not found");
            }
            return Convert.ToInt32(userId);
        }
        protected string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }
        protected string GetBaseRoute(){
            var baseRoute = $"{Request.Scheme}://{Request.Host.Value}";
            return baseRoute;
        }
    }
}