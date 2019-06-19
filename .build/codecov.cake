#load "./modules.cake"

#if !CUSTOM_CODECOV
Setup<CodecovSettings>((context) => {
    var settings = new CodecovSettings {
        Required = true
    };

    if (BuildParameters.Version != null && !string.IsNullOrEmpty(BuildParameters.Version.FullSemVersion) && BuildParameters.IsRunningOnAppVeyor) {
        // Required to work correctly with appveyor because environment changes isn't detected until cake is done running.
        var buildVersion = string.Format("{0}.build.{1}",
            BuildParameters.Version.FullSemVersion,
            BuildSystem.AppVeyor.Environment.Build.Number);
        settings.EnvironmentVariables = new Dictionary<string, string> {{"APPVEYOR_BUILD_VERSION", buildVersion }};
    }

    return settings;
});
#endif

public static void SetToolPath(ICakeContext context, CodecovSettings settings)
{
    if (context.IsRunningOnUnix())
    {
        // Special case, as the addin version used by Cake.Recipe do not support
        // the Correct unix paths.
        settings.ToolPath = context.Tools.Resolve("codecov");
    }
}

((CakeTask)BuildParameters.Tasks.UploadCodecovReportTask.Task).Actions.Clear();
((CakeTask)BuildParameters.Tasks.UploadCodecovReportTask.Task).Criterias.Clear();
BuildParameters.Tasks.UploadCodecovReportTask
    .WithCriteria(() => BuildParameters.IsMainRepository, "This is not the main repository")
    .WithCriteria(() => BuildParameters.CanPublishToCodecov)
    .Does<CodecovSettings>((settings) => {
    var script = "#tool dotnet:?package=Codecov.Tool&version=1.5.0";
    RequireTool(script, () => {
        SetToolPath(Context, settings);

        var files = GetFiles(BuildParameters.Paths.Directories.TestCoverage + "/coverlet/*");
        if (FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath)) {
            files += BuildParameters.Paths.Files.TestCoverageOutputFilePath;
        }

        if (files.Any()) {
            settings.Files = files.Select(f => f.FullPath);
            Codecov(settings);
        }
    });
});
