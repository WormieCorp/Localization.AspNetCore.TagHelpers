#load "./credentials.cake"
#load "./paths.cake"
#load "./version.cake"

public class BuildParameters
{
  public const string MainRepoUser = "WormieCorp";
  public const string MainRepoName = "Localization.AspNetCore.TagHelpers";
  public const string MainRepo = MainRepoUser + "/" + MainRepoName;
  public const string MainBranch = "master";

  public string Target { get; private set; }
  public string Configuration { get; private set; }
  public bool IsLocalBuild { get; private set; }
  public bool IsRunningOnUnix { get; private set; }
  public bool IsRunningOnWindows { get; private set; }
  public bool IsRunningOnAppVeyor { get; private set; }
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
      IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
      IsMainRepo = StringComparer.OrdinalIgnoreCase.Equals(MainRepo, buildSystem.AppVeyor.Environment.Repository.Name),
      IsMainBranch = StringComparer.OrdinalIgnoreCase.Equals(MainBranch, buildSystem.AppVeyor.Environment.Repository.Branch),
      IsTagged = IsBuildTagged(buildSystem),
      GitHub = BuildCredentials.GetGitHubCredentials(context),
      IsPublishBuild = IsPublishing(target),
      IsReleaseBuild = IsReleasing(target),
      SkipGitVersion = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("_SKIP_GITVERSION")),
      SkipOpenCover  = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("_SKIP_OPENCOVER"))
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
}
