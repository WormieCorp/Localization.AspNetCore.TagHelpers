#load "./tasks.cake"

public sealed class SonarCloudAuthentication
{
  public SonarCloudAuthentication(ICakeContext context)
  {
    Login = context.EnvironmentVariable("SONARCLOUD_LOGIN");
    ProjectKey = context.EnvironmentVariable("SONARCLOUD_PROJECT_KEY");
    Organization = context.EnvironmentVariable("SONARCLOUD_ORGANIZATION");
  }

  public bool IsEnabled => !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(ProjectKey) && !string.IsNullOrEmpty(Organization);

  public string Login { get; }
  public string ProjectKey { get; }
  public string Organization { get; }
}

BeforeBuildTask
  .Does<BuildData>((data) =>
{
  if (!IsRunningOnWindows())
  {
    Warning("Skipping SonarCloud as not running on windows.");
    return;
  }

  if (!data.SonarCloud.IsEnabled)
  {
    Warning("Skipping SonarCloud integration, missing credentials.");
    return;
  }

  Information("Starting SonarCloud integration...");
  SonarBegin(new SonarBeginSettings {
    Key = data.SonarCloud.ProjectKey,
    Branch = data.Ci.Branch,
    Organization = data.SonarCloud.Organization,
    Url = "https://sonarcloud.io",
    Exclusions = "**/Demos/**/*.*,**/*.Tests/*.cs",
    OpenCoverReportsPath = "**/*.opencover.xml",
    Login = data.SonarCloud.Login
  });
});

AfterTestsTask
  .Does<BuildData>((data) =>
{
  if (IsRunningOnWindows() && data.SonarCloud.IsEnabled)
  {
    SonarEnd(new SonarEndSettings {
      Login = data.SonarCloud.Login
    });
  }
});
