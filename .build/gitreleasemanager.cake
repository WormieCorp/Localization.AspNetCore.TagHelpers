#load "./tasks.cake"

BeforeDeploymentTask
  .Does<BuildData>((data) =>
{
  if (data.Ci.IsCiProvider("AppVeyor") && !IsRunningOnWindows())
  {
    Warning("Running on AppVeyor, but not on windows, skipping drafting release notes");
    return;
  }
  if (data.Ci.Branch != "master" && !HasArgument("force"))
  {
    Warning("Not running on the master branch, skipping drafting release notes");
    return;
  }
  if (data.Ci.IsTag)
  {
    Warning("Currently on tagged release, skipping drafting release notes");
    return;
  }
  if (!HasEnvironmentVariable("GITHUB_TOKEN"))
  {
    Warning("Environment variable 'GITHUB_TOKEN' is not set, skipping drafting release notes");
    return;
  }

  var settings = new GitReleaseManagerCreateSettings
  {
    Milestone = data.Version.MajorMinorPatch,
    Name = data.Version.SemVer,
    TargetCommitish = data.Ci.Branch,
    Prerelease = data.Version.SemVer != data.Version.MajorMinorPatch,
    ArgumentCustomization = (args) => args.Append("--no-logo"),
    LogFilePath = "./artifacts/grm.log",
  };

  GitReleaseManagerCreate(EnvironmentVariable("GITHUB_TOKEN"), "WormieCorp", "Localization.AspNetCore.TagHelpers", settings);
});

DeploymentTask
  .Does<BuildData>((data) =>
{
  if (!data.Ci.IsTag && !HasArgument("force-assets"))
  {
    Warning("Not a tagged build, skipping closing milestone");
    return;
  }
  if (data.Ci.IsCiProvider("AppVeyor") && !IsRunningOnWindows())
  {
    Warning("Running on AppVeyor, but not on windows, skipping closing milestone");
    return;
  }
  if (!HasEnvironmentVariable("GITHUB_TOKEN"))
  {
    Warning("Environment variable 'GITHUB_TOKEN' is not set, skipping closing milestone");
    return;
  }

  var assets = GetFiles(data.Dirs.NugetPackages + "/*.nupkg").Select(n => n.ToString());

  if (!assets.Any())
  {
    Error("No assets was found to upload");
    throw new Exception();
  }

  var settings = new GitReleaseManagerAddAssetsSettings
  {
    ArgumentCustomization = (args) => args.Append("--no-logo"),
    LogFilePath = "./artifacts/grm.log",
  };

  GitReleaseManagerAddAssets(EnvironmentVariable("GITHUB_TOKEN"), "WormieCorp", "Localization.AspNetCore.TagHelpers", data.Version.SemVer, string.Join(',', assets), settings);
});

AfterDeploymentTask
  .Does<BuildData>((data) =>
{
  if (!data.Ci.IsTag)
  {
    Warning("Not a tagged build, skipping publishing release");
    Warning("Not a tagged build, skipping closing milestone");
    return;
  }

  if (data.Ci.IsCiProvider("AppVeyor") && !IsRunningOnWindows())
  {
    Warning("Running on AppVeyor, but not on windows, skipping publishing release");
    Warning("Running on AppVeyor, but not on windows, skipping closing milestone");
    return;
  }
  if (!HasEnvironmentVariable("GITHUB_TOKEN"))
  {
    Warning("Environment variable 'GITHUB_TOKEN' is not set, publishing release");
    Warning("Environment variable 'GITHUB_TOKEN' is not set, skipping closing milestone");
    return;
  }

  var publishSettings = new GitReleaseManagerPublishSettings
  {
    ArgumentCustomization = (args) => args.Append("--no-logo"),
    LogFilePath = "./artifacts/grm.log",
  };

  GitReleaseManagerPublish(EnvironmentVariable("GITHUB_TOKEN"), "WormieCorp", "Localization.AspNetCore.TagHelpers", data.Version.SemVer, publishSettings);

  if (data.Version.SemVer != data.Version.MajorMinorPatch)
  {
    Warning("Pre-Release build, skipping closing milestone");
    return;
  }

  var closeSettings = new GitReleaseManagerCloseMilestoneSettings
  {
    ArgumentCustomization = publishSettings.ArgumentCustomization,
    LogFilePath = publishSettings.LogFilePath,
  };

  GitReleaseManagerClose(EnvironmentVariable("GITHUB_TOKEN"), "WormieCorp", "Localization.AspNetCore.TagHelpers", data.Version.MajorMinorPatch, closeSettings);
});
