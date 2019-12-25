namespace NetCoreApp30.Controllers
{
  using System;
  using System.Diagnostics;

  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Localization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Logging;

  using NetCoreApp30.Models;

  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }

    public IActionResult Index()
    {
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
    public IActionResult SetLanguage(string culture, Uri returnUrl)
    {
      Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions
        {
          Expires = DateTimeOffset.UtcNow.AddYears(1),
          IsEssential = true
        });

      return LocalRedirect(returnUrl.ToString());
    }
  }
}
