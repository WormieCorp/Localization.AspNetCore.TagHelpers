private WyamSettings GetWyamSettings(BuildData data)
{
  return new WyamSettings
  {
    Recipe = "Docs",
    Theme = "Samson",
    OutputPath = MakeAbsolute(Directory(data.Dirs.Documentation)),
    RootPath = MakeAbsolute(Directory("./docs")),
    ConfigurationFile = MakeAbsolute(File("config.wyam")),
    PreviewVirtualDirectory = "Localization.AspNetCore.TagHelpers",
    Settings = new Dictionary<string, object>
    {
      { "Host", "wormiecorp.github.io" },
      { "LinkRoot", "Localization.AspNetCore.TagHelpers" },
      { "BaseEditUrl", "https://github.com/WormieCorp/Localization.AspNetCore.TagHelpers/tree/develop/docs/input/" },
      { "SourceFiles", "../../src/**/{!bin,!obj,!packages,!Demos/**,}/**/*.cs" },
      { "Title", "Localization.AspNetCore.TagHelpers" },
      { "IncludeGlobalNamespace", false },
    }
  };
}

Task("Preview-Documentation")
  .Does<BuildData>((data) =>
{
  var settings = GetWyamSettings(data);
  settings.Preview = true;
  settings.Watch = true;
  Wyam(settings);
});

Task("preview")
  .IsDependentOn("Preview-Documentation");

Task("Publish-Documentation")
  .IsDependentOn(AfterDeploymentTask)
  .WithCriteria<ICiProvider>((context, ci) => !ci.IsTag, "Skipping documentation publishing due to being a tagged build")
  .WithCriteria<ICiProvider>((context, ci) => ci.Branch == "master", "Skipping documentation publishing due to not being master branch")
  .WithCriteria(() => HasEnvironmentVariable("WYAM_ACCESS_TOKEN"), "Missing wyam deploy credentials")
  .WithCriteria(() => HasEnvironmentVariable("WYAM_DEPLOY_REMOTE"), "Missing wyam deploy remote repository")
  .WithCriteria(() => HasEnvironmentVariable("WYAM_DEPLOY_BRANCH"), "Missing wyam deploy branch")
  .Does<BuildData>((data) =>
{
  Wyam(GetWyamSettings(data));

  var sourceCommit = GitLogTip("./");

  var publishFolder = ((DirectoryPath)data.Dirs.Documentation).Combine(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
  Information("Publishing Folder: {0}", publishFolder);
  Information("Getting publish branch...");
  GitClone(EnvironmentVariable("WYAM_DEPLOY_REMOTE"), publishFolder, new GitCloneSettings
  {
    BranchName = EnvironmentVariable("WYAM_DEPLOY_BRANCH")
  });

  Information("Sync output files...");
  Kudu.Sync(data.Dirs.Documentation, publishFolder, new KuduSyncSettings
  {
    ArgumentCustomization = args => args.Append("--ignore").AppendQuoted(".git;CNAME")
  });

  if (GitHasUncommitedChanges(publishFolder))
  {
    Information("Stage all changes...");
    GitAddAll(publishFolder);

    if (GitHasStagedChanges(publishFolder))
    {
      Information("Commit all changes...");
      GitCommit(
        publishFolder,
        sourceCommit.Committer.Name,
        sourceCommit.Committer.Email,
        string.Format("Continuous Integration Publish: {0}\n{1}", sourceCommit.Sha, sourceCommit.Message)
      );

      Information("Pushing all changes...");
      GitPush(publishFolder, EnvironmentVariable("WYAM_ACCESS_TOKEN"), "x-oauth-basic", EnvironmentVariable("WYAM_DEPLOY_BRANCH"));
    }
  }
});
