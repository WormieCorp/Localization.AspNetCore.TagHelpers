namespace NetCoreApp30
{
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Hosting;

  public static class Program
  {
    public static void Main(string[] args)
    {
      using (var host = CreateHostBuilder(args).Build())
      {
        host.Run();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
  }
}
