using CYS.Repos;
using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class OzellikEkleViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{

			return View();
		}
	}
}
