using CYS.Repos;
using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class UstSoyViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			HayvanCTX usctx = new HayvanCTX();
			var hList = usctx.hayvanList("select * from Hayvan where aktif = 1", null);
			return View(hList);
		}
	}
}
