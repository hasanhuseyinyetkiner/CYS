using CYS.Models.HelperModel;
using Microsoft.AspNetCore.Mvc;

namespace CYS.ComponentManager
{
	public class PageHeaderViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync(string pageHeader, string pageTitle, string current)
		{
			PageHeaderModel phm = new PageHeaderModel()
			{
				parentPageTitle = pageHeader,
				pageTitle = pageTitle,
				currentPage = current,
			};
			return View(phm);
		}
		
	}
}
