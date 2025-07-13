using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class AgirlikModalViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{


			return View();
		}
	}
}
