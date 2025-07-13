using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class UstTurEkleViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
