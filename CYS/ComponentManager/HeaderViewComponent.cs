using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class HeaderViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
