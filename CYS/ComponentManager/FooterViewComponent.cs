using CYS.Models;
using CYS.Models.HelperModel;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CYS.ComponentManager
{
	public class FooterViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			

			return View();
		}
	}
}
