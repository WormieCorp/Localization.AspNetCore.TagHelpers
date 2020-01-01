#load "./tasks.cake"

AfterTestsTask.Does<BuildData>(data =>
{
  if (data.Ci.IsCiProvider("LocalBuild"))
  {
    Information("Running Report Generator");
    var files = GetFiles(data.TestReportsFilter);

    ReportGenerator(files, data.Dirs.TestReports + "/../CoverageResults");
  }
});
