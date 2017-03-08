#load "./credentials.cake"
#load "./paths.cake"
#load "./version.cake"

public class BuildParameters
{
  public const string MainRepoUser = "WormieCorp";
  public const string MainRepoName = "Localization.AspNetCore.TagHelpers";
  public const string MainRepo = MainRepoUser + "/" + MainRepoName;
  public const string MainBranch = "master";
  private const string ArgumentCustomizationFormat = "/property:VersionPrefix={0};PackageReleaseNotes={1};PackageOutputPath={2}{3}";

  public string Target { get; private set; }
  public string Configuration { get; private set; }
  public bool IsLocalBuild { get; private set; }
  public bool IsRunningOnUnix { get; private set; }
  public bool IsRunningOnWindows { get; private set; }
  public bool IsRunningOnAppVeyor { get; private set; }
  public bool IsRunningOnTravis { get; private set; }
  public bool IsPullRequest { get; private set; }
  public bool IsMainRepo { get; private set; }
  public bool IsMainBranch { get; private set; }
  public bool IsTagged { get; private set; }
  public bool IsPublishBuild { get; private set; }
  public bool IsReleaseBuild { get; private set; }
  public bool SkipGitVersion { get; private set; }
  public bool SkipOpenCover { get; private set; }
  public BuildCredentials GitHub { get; private set; }
  public BuildVersion Version { get; private set; }
  public BuildPaths Paths { get; private set; }
  public string ReleaseNotes { get; private set; }
  public bool CompileNetCoreOnly { get; private set; }

  public bool ShouldPublish
  {
    get
    {
      return !IsLocalBuild && !IsPullRequest && IsMainRepo
        && IsTagged;
    }
  }

  public bool ShouldPublishToMyGet
  {
    get
    {
      return !IsLocalBuild && !IsPullRequest &&
        IsMainRepo && (IsTagged || !IsMainBranch);
    }
  }

  public bool ShouldCreateReleaseNotes
  {
    get
    {
      return IsPublishBuild ||
        (!IsLocalBuild && !IsPullRequest && IsMainRepo
         && IsMainBranch && !IsTagged);
    }
  }

  public void Initialize(ICakeContext context)
  {
    Version = BuildVersion.Calculate(context, this);

    Paths = BuildPaths.GetPaths(context, Configuration, Version.SemVersion);
  }

  public void SetReleaseNotes(ICakeContext context, string[] releaseNotesArray)
  {
    if (ShouldPublish)
    {
      ReleaseNotes = string.Join("\r\n", RemoveMarkdown(releaseNotesArray));
    }
    else
    {
      ReleaseNotes = string.Join("\r\n", releaseNotesArray);
    }
  }

  public Func<ProcessArgumentBuilder, ProcessArgumentBuilder> GetMsBuildArgs(ICakeContext context)
  {
    return (args) =>
    {
      return
        args
          .AppendQuoted(
            ArgumentCustomizationFormat,
            Version.Version,
            ReleaseNotes,
            Paths.Directories.NugetRoot.MakeAbsolute(context.Environment),
            CompileNetCoreOnly ? ";NetCoreOnly=true" : ""
          );
    };
  }

  public static BuildParameters GetParameters(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException("context");
    }

    var target = context.Argument("target", "Default");
    var buildSystem = context.BuildSystem();

    return new BuildParameters
    {
      Target = target,
      Configuration = context.Argument("configuration", "Release"),
      IsLocalBuild = buildSystem.IsLocalBuild,
      IsRunningOnUnix = context.IsRunningOnUnix(),
      IsRunningOnWindows = context.IsRunningOnWindows(),
      IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor,
      IsRunningOnTravis = buildSystem.TravisCI.IsRunningOnTravisCI,
      IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
      IsMainRepo = StringComparer.OrdinalIgnoreCase.Equals(MainRepo, buildSystem.AppVeyor.Environment.Repository.Name),
      IsMainBranch = StringComparer.OrdinalIgnoreCase.Equals(MainBranch, buildSystem.AppVeyor.Environment.Repository.Branch),
      IsTagged = IsBuildTagged(buildSystem),
      GitHub = BuildCredentials.GetGitHubCredentials(context),
      IsPublishBuild = IsPublishing(target),
      IsReleaseBuild = IsReleasing(target),
      SkipGitVersion = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("_SKIP_GITVERSION")),
      SkipOpenCover  = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("_SKIP_OPENCOVER")),
      CompileNetCoreOnly = context.Argument("netcoreonly", false)
    };
  }

  private static bool IsBuildTagged(BuildSystem buildSystem)
  {
    return buildSystem.AppVeyor.Environment.Repository.Tag.IsTag
      && !string.IsNullOrWhiteSpace(buildSystem.AppVeyor.Environment.Repository.Tag.Name);
  }

  private static bool IsReleasing(string target)
  {
    var targets = new[] { "Publish", "Publish-NuGet", "Publish-GitHub-Release" };
    return targets.Any(t => StringComparer.OrdinalIgnoreCase.Equals(t, target));
  }

  private static bool IsPublishing(string target)
  {
    var targets = new[] { "ReleaseNotes", "Create-Release-Notes" };
    return targets.Any(t => StringComparer.OrdinalIgnoreCase.Equals(t, target));
  }

  private static IEnumerable<string> RemoveMarkdown(string[] releaseNotes)
  {
    for (int i = 0; i < releaseNotes.Length; i++)
    {
      var note = releaseNotes[i];
      if (note.StartsWith("__") && note.EndsWith("__"))
      {
        if (i > 1)
        {
          yield return "";
        }
        yield return "";
        yield return note.Trim('_').Trim();
        yield return note;
      }
      else if (note.StartsWith("-"))
      {
        int index = note.IndexOf(')');
        if (index > 0)
        {
          string issue = note.Substring(index + 1).Trim();
          yield return note.Substring(0, 2) + issue;
        }
        else
        {
          yield return note;
        }
      }
      else if (note.IndexOf("part of this release") > 0)
      {
        continue;
      }
      else
      {
        yield return note;
      }
    }
  }
}
