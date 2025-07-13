using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class BuyukBasViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
