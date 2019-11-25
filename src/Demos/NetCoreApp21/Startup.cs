namespace NetCoreApp21
{
  using System.Globalization;

  using Localization.AspNetCore.TagHelpers;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Localization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Razor;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
#pragma warning disable CA1822 // Mark members as static

    public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
    {
      services.AddLocalization(options => options.ResourcesPath = "Localization");

      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services
        .AddMvc()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      // The following configuration is optional, this just sets the default values
      services.Configure<LocalizeTagHelperOptions>(options =>
      {
        options.HtmlEncodeByDefault = true;
        options.NewLineHandling = NewLineHandling.Auto;
        options.TrimWhitespace = true;
      });

      // We configure it here, so we can use DI to get the options in the view
      services.Configure<RequestLocalizationOptions>(options =>
      {
        var supportedCultures = new[]
        {
          "en",
          "nb"
        };

        options.AddSupportedCultures(supportedCultures)
               .AddSupportedUICultures(supportedCultures)
               .SetDefaultCulture(supportedCultures[0]);
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CA1822 // Mark members as static

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
#pragma warning restore CA1822 // Mark members as static
    {
      app.UseRequestLocalization();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();
      app.UseCookiePolicy();

#pragma warning disable IDE0053 // Use expression body for lambda expressions
      app.UseMvc(routes =>
#pragma warning restore IDE0053 // Use expression body for lambda expressions
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
