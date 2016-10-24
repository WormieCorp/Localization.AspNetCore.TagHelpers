//-----------------------------------------------------------------------
// <copyright file="HomeController.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.Demo.Controllers
{
  using System;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Localization;
  using Microsoft.AspNetCore.Mvc;

  public class HomeController : Controller
  {
    public IActionResult About()
    {
      ViewData["Message"] = "Your application description page.";

      return View();
    }

    public IActionResult Contact()
    {
      ViewData["Message"] = "Your contact page.";

      return View();
    }

    public IActionResult Error()
    {
      return View();
    }

    public IActionResult Index()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
      Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

      return LocalRedirect(returnUrl);
    }
  }
}
