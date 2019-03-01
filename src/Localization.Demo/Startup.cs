//-----------------------------------------------------------------------
// <copyright file="Startup.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.Demo
{
  using System.Globalization;
  using AspNetCore.TagHelpers;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Localization;
  using Microsoft.AspNetCore.Mvc.Razor;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = Configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
      app.UseRequestLocalization(locOptions.Value);

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddLocalization(options => options.ResourcesPath = "Localization");

      // Add framework services.
      services.AddMvc()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization();

      services.Configure<RequestLocalizationOptions>(options =>
      {
        var supportedCultures = new[]
        {
          new CultureInfo("en"),
          new CultureInfo("nb-NO")
        };

        options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");

        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
      });

      // The following configuration is optional, this just sets the default values
      services.Configure<LocalizeTagHelperOptions>(options =>
      {
        options.NewLineHandling = NewLineHandling.Auto;
        options.TrimWhitespace = true;
      });
    }
  }
}
