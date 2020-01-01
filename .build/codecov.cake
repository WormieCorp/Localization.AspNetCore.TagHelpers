#load "./tasks.cake"

AfterTestsTask
  .Does<BuildData>((data) =>
{
  if (!data.Ci.IsCiProvider("AppVeyor") && !HasEnvironmentVariable("CODECOV_TOKEN"))
  {
    Warning("Not running on appveyor, and environment variable CODECOV_TOKEN is not available.");
  }
  else
  {
    Codecov(GetFiles(data.TestReportsFilter).Select(r => r.ToString()));
  }
});
