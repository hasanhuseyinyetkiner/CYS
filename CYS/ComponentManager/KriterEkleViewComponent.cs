using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class KriterEkleViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
