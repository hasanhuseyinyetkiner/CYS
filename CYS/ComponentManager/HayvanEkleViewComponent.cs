using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class HayvanEkleViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			
			return View();
		}
	}
}
