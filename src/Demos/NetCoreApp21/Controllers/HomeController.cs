namespace NetCoreApp21.Controllers
{
  using System;
  using System.Diagnostics;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Localization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Localization;
  using NetCoreApp21.Models;

  public class HomeController : Controller
  {
    private readonly IStringLocalizer _localizer;

    public HomeController(IStringLocalizer<HomeController> localizer)
    {
      _localizer = localizer;
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult About()
    {
      ViewData["Message"] = _localizer["Your application description page."];

      return View();
    }

    public IActionResult Contact()
    {
      ViewData["Message"] = _localizer["Your contact page."];

      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
      Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions
        {
          Expires = DateTimeOffset.UtcNow.AddYears(1),
          IsEssential = true
        });

      return LocalRedirect(returnUrl);
    }
  }
}
