namespace NetCoreApp30
{
  using Localization.AspNetCore.TagHelpers;

  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Razor;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddLocalization(options => options.ResourcesPath = "Localization");

      services.AddControllersWithViews()
        // AddViewLocalization registers the interfaces and classes used by the tag helper
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization();

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
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseRequestLocalization();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
