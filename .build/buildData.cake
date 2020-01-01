Setup<ICiProvider>(context =>
{
  var buildSystem = context.BuildSystem();
  if (buildSystem.IsRunningOnAppVeyor)
  {
    context.Information("CI Provider Found: AppVeyor...");
    return new AppVeyorCiProvider(buildSystem);
  }

  context.Information("No supported CI provider was found...");
  context.Information("Falling back to local build!");
  return new GitCiProvider(context);
});

Setup<GitVersion>(context =>
{
  var artifactsDir = "artifacts";
  var filePath = System.IO.Path.Join(artifactsDir, "version.xml");
  GitVersion version = null;

  context.EnsureDirectoryExists(artifactsDir);

  if (context.FileExists(filePath))
  {
    using (var file = System.IO.File.OpenRead(filePath))
    {
      var serializer = new System.Xml.Serialization.XmlSerializer(typeof(GitVersion));
      version = (GitVersion)serializer.Deserialize(file);
    }

    var commit = GitLogTip("./");
    if (version.Sha != commit.Sha)
    {
      version = null;
    }
  }

  if (version == null)
  {
    var provider = context.Data.Get<ICiProvider>();
    if (!provider.IsCiProvider("LocalBuild"))
    {
      context.GitVersion(new GitVersionSettings
      {
        UpdateAssemblyInfo = false,
        OutputType = GitVersionOutput.BuildServer,
      });
    }

    version = context.GitVersion(new GitVersionSettings
    {
      UpdateAssemblyInfo = false,
      OutputType = GitVersionOutput.Json,
    });

    using (var file = System.IO.File.Create(filePath))
    {
      var serializer = new System.Xml.Serialization.XmlSerializer(typeof(GitVersion));
      serializer.Serialize(file, version);
    }
  }

  context.Information($"Building version: {version.SemVer} of Localization.AspNetCore.TagHelpers");

  return version;
});

Setup<BuildDirectories>(context =>
{
  context.Information("Setting up directories to use");
  var artifactsBaseDir = MakeAbsolute((DirectoryPath)"./artifacts");

  var directories = new BuildDirectories(artifactsBaseDir.ToString());
  if (context.HasEnvironmentVariable("PKG_CACHE_DIR"))
  {
    directories.PackageCache = context.EnvironmentVariable("PKG_CACHE_DIR");
  }

  return directories;
});

Setup<BuildData>(context =>
{
  context.Information("Creating build data class to hold configuration values");
  return new BuildData
  {
    Configuration = context.Argument("configuration", "Release"),
    Target = context.Argument("target", "Default"),
    Ci = context.Data.Get<ICiProvider>(),
    Dirs = context.Data.Get<BuildDirectories>(),
    SonarCloud = new SonarCloudAuthentication(context),
    Version = context.Data.Get<GitVersion>(),
  };
});

public sealed class BuildData
{
  public string Configuration { get; set; }
  public string Target { get; set; }
  public string TestReportsFilter => Dirs.TestReports + "/**/*.opencover.xml";

  public ICiProvider Ci { get; set; }
  public BuildDirectories Dirs { get; set; }
  public SonarCloudAuthentication SonarCloud { get; set; }
  public GitVersion Version { get; set; }
}

public sealed class BuildDirectories
{
  public BuildDirectories(string artifactsBaseDir)
  {
    ArtifactsBaseDir = artifactsBaseDir;
  }

  public string ArtifactsBaseDir { get; }
  public string Documentation => ArtifactsBaseDir + "/Documentation";
  public string PackageCache { get; set; }
  public string NugetPackages => ArtifactsBaseDir + "/packages/nuget";
  public string TestReports => ArtifactsBaseDir + "/tests/TestReports";
}
