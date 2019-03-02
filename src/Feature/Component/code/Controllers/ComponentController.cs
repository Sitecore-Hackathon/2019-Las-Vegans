namespace LV.Feature.Component.Controllers
{
	using System.Web.Mvc;

	public class ComponentController : Controller
	{
		public ComponentController()
		{
		}

		[HttpGet]
		public ActionResult UseSitecore()
		{
			return View("~/Views/Component/ScComponentView.cshtml");
		}
	}
}