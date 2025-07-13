using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class HeaderBrandViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
