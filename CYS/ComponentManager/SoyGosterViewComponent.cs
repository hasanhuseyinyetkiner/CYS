using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class SoyGosterViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{

			return View();
		}
	}
}
