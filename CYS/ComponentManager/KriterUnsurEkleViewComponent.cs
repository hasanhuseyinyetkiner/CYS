using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class KriterUnsurEkleViewComponent: ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
