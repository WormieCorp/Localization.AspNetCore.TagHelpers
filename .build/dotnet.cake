#load "./tasks.cake"

RestoreTask
  .Does<BuildDirectories>((dirs) =>
{
  var restoreSettings = new DotNetCoreRestoreSettings
  {
    PackagesDirectory = dirs.PackageCache
  };

  DotNetCoreRestore("./Localization.AspNetCore.TagHelpers.sln", restoreSettings);
});

BuildTask
  .Does<BuildData>((data) =>
{
  var settings = new DotNetCoreBuildSettings
  {
    Configuration = data.Configuration,
    NoRestore = true,
    VersionSuffix = data.Version.PreReleaseTag,
    MSBuildSettings = new DotNetCoreMSBuildSettings()
      .SetVersionPrefix(data.Version.MajorMinorPatch)
      .SetInformationalVersion(data.Version.InformationalVersion)
      .WithProperty("PackageOutputPath", data.Dirs.NugetPackages)
      .WithProperty("Description", data.Description)
      .ValidateProjectFile()
  };

  DotNetCoreBuild("./Localization.AspNetCore.TagHelpers.sln", settings);
});

TestsTask
  .Does<BuildData>((data) =>
{
  var outputDirectory = data.Dirs.TestReports;

  DeleteDirectories(GetDirectories(outputDirectory + "/*"), new DeleteDirectorySettings {
    Recursive = true,
    Force = true
  });

  EnsureDirectoryExists(outputDirectory);

  var settings = new DotNetCoreTestSettings
  {
    Configuration = data.Configuration,
    NoBuild = true,
    ArgumentCustomization = args =>
      args.Append("--settings:\"test/coverletArgs.runSettings\""),
    ResultsDirectory = outputDirectory
  };

  DotNetCoreTest("test/Localization.AspNetCore.TagHelpers.Tests", settings);
});

DeploymentTask
  .Does<BuildData>((data) =>
{
  if (!data.Ci.IsTag)
  {
    Warning("Not a tagged build, skipping publishing to nuget.org");
    return;
  }
  if (!data.Ci.IsCiProvider("AppVeyor"))
  {
    Warning("Not building on appveyor, skipping publishing to nuget.org");
    return;
  }
  if (!IsRunningOnWindows())
  {
    Warning("Not running on windows, skipping publishing to nuget.org");
    return;
  }
  if (!HasEnvironmentVariable("NUGET_API_KEY") && !HasEnvironmentVariable("NUGET_SOURCE"))
  {
    Warning("Nuget credentials/url not available, skipping publishing to nuget.org");
    return;
  }

  var files = GetFiles(data.Dirs.NugetPackages + "/*.nupkg");
  var settings = new DotNetCoreNuGetPushSettings
  {
    Source = EnvironmentVariable("NUGET_SOURCE"),
    ApiKey = EnvironmentVariable("NUGET_API_KEY"),
  };

  if (HasEnvironmentVariable("NUGET_SYMBOL_SOURCE"))
  {
    settings.SymbolSource = EnvironmentVariable("NUGET_SYMBOL_SOURCE");
  }


  foreach (var file in files.Select(f => f.ToString()))
  {
    DotNetCoreNuGetPush(file, settings);
  }
})
  .Does<BuildData>((data) =>
{
  if (data.Ci.Branch != "develop" && !data.Ci.IsTag)
  {
    Warning("Not on develop branch, and not a tag, skipping publishing to feedz.io");
    return;
  }

  if (!data.Ci.IsCiProvider("AppVeyor"))
  {
    Warning("Not building on appveyor, skipping publishing to feedz.io");
    return;
  }
  if (!IsRunningOnWindows())
  {
    Warning("Not running on windows, skipping publishing to feedz.io");
    return;
  }
  if (!HasEnvironmentVariable("FEEDZ_API_KEY") && !HasEnvironmentVariable("FEEDZ_SOURCE"))
  {
    Warning("Feedz credentials/url not available, skipping publishing to feedz.io");
    return;
  }

  var files = GetFiles(data.Dirs.NugetPackages + "/*.nupkg");
  var settings = new DotNetCoreNuGetPushSettings
  {
    Source = EnvironmentVariable("FEEDZ_SOURCE"),
    ApiKey = EnvironmentVariable("FEEDZ_API_KEY"),
  };

  if (HasEnvironmentVariable("FEEDZ_SYMBOL_SOURCE"))
  {
    settings.SymbolSource = EnvironmentVariable("FEEDZ_SYMBOL_SOURCE");
  }


  foreach (var file in files.Select(f => f.ToString()))
  {
    DotNetCoreNuGetPush(file, settings);
  }
});
