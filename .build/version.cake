using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class BuildVersion
{
  public string Version { get; private set; }
  public string SemVersion { get; private set; }
  public string FullSemVersion { get; private set; }
  public string DotNetAsterix { get; private set; }
  public string Milestone { get; private set; }
  public string CakeVersion { get; private set; }

  public static BuildVersion Calculate(ICakeContext context, BuildParameters parameters)
  {
    if (context == null)
    {
      throw new ArgumentNullException("context");
    }

    string version = null;
    string semVersion = null;
    string milestone = null;
    string fullSemVersion = null;

    if (context.IsRunningOnWindows() && !parameters.SkipGitVersion)
    {
      context.Information("Calculating Semantic Version");
      if (!parameters.IsLocalBuild || parameters.IsPublishBuild || parameters.IsReleaseBuild)
      {
        context.GitVersion(new GitVersionSettings{
          UpdateAssemblyInfoFilePath = "./src/SolutionInfo.cs",
          UpdateAssemblyInfo = true,
          OutputType = GitVersionOutput.BuildServer
        });

        version = context.EnvironmentVariable("GitVersion_MajorMinorPatch");
        semVersion = context.EnvironmentVariable("GitVersion_LegacySemVerPadded");
        fullSemVersion = context.EnvironmentVariable("GitVersion_FullSemVer");
        milestone = version;
      }
    }

    try {
      GitVersion assertedVersions = context.GitVersion(new GitVersionSettings
      {
        OutputType = GitVersionOutput.Json,
      });

      version = assertedVersions.MajorMinorPatch;
      semVersion = assertedVersions.LegacySemVerPadded;
      fullSemVersion = assertedVersions.FullSemVer;
      milestone = version;

      context.Information("Calculated Semantic Version: {0}", semVersion);
    } catch {
      context.Warning("Unable to calculate Semantic Version, Setting version to '0.0.0-unknown'");
      version = "0.0.0";
      semVersion = "0.0.0-unknown";
      fullSemVersion = semVersion;
      milestone = string.Concat("v", version);
    }

    var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

    return new BuildVersion
    {
      Version = version,
      SemVersion = semVersion,
      FullSemVer = fullSemVersion,
      DotNetAsterix = semVersion.Substring(version.Length).TrimStart('-'),
      Milestone = milestone,
      CakeVersion = cakeVersion
    };
  }
}
