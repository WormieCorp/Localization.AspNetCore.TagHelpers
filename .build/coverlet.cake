// Do note that this script is dependent on
// Cake.Recipe, make sure that this package
// have already been loaded before loading this
// script.
// If you also wishes to upload the coverage reports
// Then see the associated codecov.cake.
#load "./modules.cake"
#addin nuget:?package=Cake.Coverlet&version=2.2.1

public static CoverletOutputFormat CoverageOutputFormat { get; set; } = CoverletOutputFormat.opencover;

#if !CUSTOM_COVERLET
Setup<CoverletSettings>((context) => {
    var coverletSettings = new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverageOutputFormat,
        ExcludeByFile = ToolSettings.TestCoverageExcludeByFile.Split(';').ToList(),
        ExcludeByAttribute = ToolSettings.TestCoverageExcludeByAttribute.Split(';').ToList()
    };

    foreach (var filter in ToolSettings.TestCoverageFilter.Split(' '))
    {
        if (filter[0] == '+') {
            coverletSettings.WithInclusion(filter.TrimStart('+'));
        }
        else if (filter[0] == '-') {
            coverletSettings.WithFilter(filter.TrimStart('-'));
        }
    }

    return coverletSettings;
});
#endif

public static OpenCoverSettings CreateOpenCoverSettings()
{
    return new OpenCoverSettings
    {
        ReturnTargetCodeOffset = 0,
        OldStyle = true,
        Register = "user"
    }.WithFilter(ToolSettings.TestCoverageFilter)
     .ExcludeByAttribute(ToolSettings.TestCoverageExcludeByAttribute)
     .ExcludeByFile(ToolSettings.TestCoverageExcludeByFile);
}

((CakeTask)BuildParameters.Tasks.DotNetCoreTestTask.Task).Actions.Clear();

BuildParameters.Tasks.DotNetCoreTestTask
.Does((context) => {
    var projects = GetFiles(BuildParameters.TestDirectoryPath + (BuildParameters.TestFilePattern ?? "/**/*Tests.csproj"));

    var coverletSettings = context.Data.Get<CoverletSettings>();
    // We need to ensure the correct coverage directory is used, if we wish to later upload coverage reports
    coverletSettings.CoverletOutputDirectory = BuildParameters.Paths.Directories.TestCoverage.Combine("coverlet");
    var openCoverSettings = CreateOpenCoverSettings();
    EnsureDirectoryExists(coverletSettings.CoverletOutputDirectory);
    Information("Path to coverlet coverage report: {0}", coverletSettings.CoverletOutputDirectory);

    foreach (var project in projects)
    {
        // Because Cake.Coverlet adds parameters to the existing
        // Test settings class, we need to re-create it every iteration.
        var testSettings = new DotNetCoreTestSettings {
            Configuration = BuildParameters.Configuration,
            NoBuild = true
        };
        var parsedProject = ParseProject(project, BuildParameters.Configuration);

        if (parsedProject.IsNetCore && parsedProject.HasPackage("coverlet.msbuild"))
        {
            // A . in the file name causes codecov to not output the correct format
            coverletSettings.CoverletOutputName = parsedProject.RootNameSpace.Replace('.', '-');
            DotNetCoreTest(project.FullPath, testSettings, coverletSettings);
        }
        else if (BuildParameters.IsRunningOnWindows)
        {
            openCoverSettings.MergeOutput = FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath);
            OpenCover((tool) => tool.DotNetCoreTest(project.FullPath, testSettings),
                BuildParameters.Paths.Files.TestCoverageOutputFilePath,
                openCoverSettings);
        }
        else
        {
            DotNetCoreTest(project.FullPath, testSettings);
        }
    }
}).Does(() => {
    var files = GetFiles(BuildParameters.Paths.Directories.TestCoverage + "/coverlet/*");
    if (FileExists(BuildParameters.Paths.Files.TestCoverageOutputFilePath)) {
        files += BuildParameters.Paths.Files.TestCoverageOutputFilePath;
    }

    if (files.Any()) {
        ReportGenerator(files, BuildParameters.Paths.Directories.TestCoverage);
    }
});
