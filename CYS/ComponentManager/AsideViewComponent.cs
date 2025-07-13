using CYS.Models.HelperModel;
using CYS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CYS.ComponentManager
{
	public class AsideViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var user = HttpContext.Session.GetString("user");
			var profile = HttpContext.Session.GetString("profile");
			NameLogo nl = new NameLogo();
			var userObj = JsonConvert.DeserializeObject<User>(user);
			var profileObj = JsonConvert.DeserializeObject<Profile>(profile);

			nl.name = userObj.userName;
			nl.logo = profileObj.logo;
			return View(nl);
		}
	}
}
