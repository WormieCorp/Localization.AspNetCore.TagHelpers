namespace NetCoreApp21
{
  using Microsoft.AspNetCore;
  using Microsoft.AspNetCore.Hosting;

  public static class Program
  {
    public static void Main(string[] args)
    {
#pragma warning disable DF0001 // Marks undisposed anonymous objects from method invocations.
      CreateWebHostBuilder(args).Build().Run();
#pragma warning restore DF0001 // Marks undisposed anonymous objects from method invocations.
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
      return WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>();
    }
  }
}
