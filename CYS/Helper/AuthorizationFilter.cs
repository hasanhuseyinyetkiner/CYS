using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CYS.Helper
{
	public class AuthorizationFilter : IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			bool isAuthorized = CheckUserPermission(context.HttpContext);
			if (!isAuthorized)
			{
				context.Result = new UnauthorizedResult();
			}
		}
		private bool CheckUserPermission(HttpContext user)
		{
			try
			{
				string userName = user.Session.GetString("userName");
				if(userName != "")
					return true;
				return false;

			}
			catch
			{
				return false;

			}
		}
	}
}
