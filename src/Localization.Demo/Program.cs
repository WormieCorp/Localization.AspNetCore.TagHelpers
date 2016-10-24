//-----------------------------------------------------------------------
// <copyright file="Program.cs">
//   Copyright (c) Kim Nordmo. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <author>Kim Nordmo</author>
//-----------------------------------------------------------------------

namespace Localization.Demo
{
  using System.IO;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;

  public static class Program
  {
    public static void Main(string[] args)
    {
      var config = new ConfigurationBuilder()
          .AddCommandLine(args)
          .AddEnvironmentVariables(prefix: "ASPNETCORE_")
          .Build();

      var host = new WebHostBuilder()
          .UseConfiguration(config)
          .UseKestrel()
          .UseContentRoot(Directory.GetCurrentDirectory())
          .UseIISIntegration()
          .UseStartup<Startup>()
          .Build();

      host.Run();
    }
  }
}
