using CYS.Models.HelperModel;
using CYS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CYS.ComponentManager
{
	public class KategoriEkleViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			
			return View();
		}
	}
}
