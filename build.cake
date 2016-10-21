#load "./.build/tools.cake"
#load "./.build/parameters.cake"
#load "./.build/testing.cake"

var parameters = BuildParameters.GetParameters(Context);
bool publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
	parameters.Initialize(context);

	// Increase verbosity?
	if(parameters.IsMainBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
		Information("Increasing verbosity to diagnostic.");
		context.Log.Verbosity = Verbosity.Diagnostic;
	}

	Information("Building version {0} of Cake ({1}, {2}) using version {3} of Cake. (IsTagged: {4})",
		parameters.Version.SemVersion,
		parameters.Configuration,
		parameters.Target,
		parameters.Version.CakeVersion,
		parameters.IsTagged);
});

Task("Clean")
	.Does(() =>
{
	CleanDirectories(parameters.Paths.Directories.ToClean);
});

Task("Patch-Project-Json")
	.IsDependentOn("Export-Release-Notes")
	.IsDependentOn("Clean")
	.Does(() =>
	{
		var projects = GetFiles("./**/project.json");
		string[] releaseNotes;
		if (!parameters.ShouldPublish)
		{
			releaseNotes = new[] { "No release notes available for this release" };
		}
		else
		{
			releaseNotes = ParseReleaseNotes("./CHANGELOG.md").Notes.ToArray();
		}

		foreach(var project in projects)
		{
			if (!parameters.Version.PatchProjectJson(project, releaseNotes))
			{
				Warning("No version specified in {0}.", project.FullPath);
			}
		}
	});

Task("Restore-NuGet-Packages")
	.IsDependentOn("Clean")
	.Does(() =>
	{
		DotNetCoreRestore("./", new DotNetCoreRestoreSettings
		{
			Verbose = false,
			Verbosity = DotNetCoreRestoreVerbosity.Warning
		});
	});

Task("Build")
	.IsDependentOn("Patch-Project-Json")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() =>
{
	foreach(var project in parameters.Paths.Directories.Projects)
	{
		DotNetCoreBuild(project.FullPath, new DotNetCoreBuildSettings
		{
			VersionSuffix = parameters.Version.DotNetAsterix,
			Configuration = parameters.Configuration,
			NoDependencies = true
		});
	}
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
	var projects = GetFiles("./test/**/*.Tests.xproj");
	foreach(var project in projects)
	{
		UnitTesting.RunUnitTest(Context, project, true, parameters);
		UnitTesting.RunUnitTest(Context, project, false, parameters);
	}

	if (FileExists(parameters.Paths.Files.TestCoverageOutputFilePath))
	{
		ReportGenerator(parameters.Paths.Files.TestCoverageOutputFilePath, parameters.Paths.Directories.TestResults);
	}
});

Task("Copy-Files")
	.IsDependentOn("Run-Unit-Tests")
	.IsDependentOn("Export-Release-Notes")
	.Does(() =>
{
	DotNetCorePublish("./src/" + BuildParameters.MainRepoName, new DotNetCorePublishSettings
	{
		Framework = "net451",
		VersionSuffix = parameters.Version.DotNetAsterix,
		Configuration = parameters.Configuration,
		OutputDirectory = parameters.Paths.Directories.ArtifactsBinNet451,
		NoBuild = true,
		Verbose = false
	});
});

Task("Zip-Files")
	.IsDependentOn("Copy-Files")
	.Does(() =>
{
	var netFiles = GetFiles(parameters.Paths.Directories.ArtifactsBinNet451.FullPath + "/**/*");
	Zip(parameters.Paths.Directories.ArtifactsBinNet451, parameters.Paths.Files.ZipArtifactsPathDesktop, netFiles);
});

Task("Create-NuGet-Packages")
	.IsDependentOn("Copy-Files")
	.Does(() =>
{
	var projects = GetFiles("./src/**/*.xproj");
	foreach (var project in projects)
	{
		var name = project.GetDirectory().FullPath;
		if (name.EndsWith("Demo"))
		{
			continue;
		}

		DotNetCorePack(project.GetDirectory().FullPath, new DotNetCorePackSettings
		{
			VersionSuffix = parameters.Version.DotNetAsterix,
			Configuration = parameters.Configuration,
			OutputDirectory = parameters.Paths.Directories.NugetRoot,
			NoBuild = true,
			Verbose = false
		});
	}
});

Task("Publish-MyGet")
	.IsDependentOn("Create-NuGet-Packages")
	.WithCriteria(() => parameters.ShouldPublishToMyGet)
	.Does(() =>
{
	var apiKey = EnvironmentVariable("MYGET_API_KEY");
	if (string.IsNullOrEmpty(apiKey))
	{
		throw new InvalidOperationException("Could not resolve MyGet API key.");
	}
	var apiUrl = EnvironmentVariable("MYGET_API_URL");
	if (string.IsNullOrEmpty(apiUrl))
	{
		throw new InvalidOperationException("Could not resolve MyGet API url.");
	}
	var symbolsUrl = EnvironmentVariable("MYGET_SYMBOLS_URL");
	if (string.IsNullOrEmpty(symbolsUrl))
	{
		throw new InvalidOperationException("Could not resolve MyGet Symbols url.");
	}

	var packages = GetFiles(parameters.Paths.Directories.NugetRoot + "/**/*.nupkg");

	foreach (var package in packages)
	{
		var settings = new NuGetPushSettings
		{
			ApiKey = apiKey
		};
		if (package.FullPath.EndsWith(".symbols.nupkg"))
		{
			settings.Source = symbolsUrl;
		}
		else
		{
			settings.Source = apiUrl;
		}

		NuGetPush(package, settings);
	}
})
.OnError(exception =>
{
	Information("Publish-MyGet Task failed, but continuing with next Task...");
	publishingError = true;
});

Task("Publish-NuGet")
	.IsDependentOn("Create-NuGet-Packages")
	.WithCriteria(() => parameters.ShouldPublish)
	.Does(() =>
{
	var apiKey = EnvironmentVariable("NUGET_API_KEY");
	if (string.IsNullOrEmpty(apiKey))
	{
		throw new InvalidOperationException("Could not resolve NuGet API key.");
	}

	var apiUrl = EnvironmentVariable("NUGET_API_URL");
	if (string.IsNullOrEmpty(apiUrl))
	{
		throw new InvalidOperationException("Could not resolve NuGet API url.");
	}

	var packages = GetFiles(parameters.Paths.Directories.NugetRoot + "/**/*.nupkg");
	foreach (var package in packages)
	{
		if (!package.FullPath.EndsWith(".symbols.nupkg"))
		{
			NuGetPush(package, new NuGetPushSettings
			{
				ApiKey = apiKey,
				Source = apiUrl
			});
		}
	}
})
.OnError(exception =>
{
	Information("Publish-NuGet Task failed, but continuing with next Task...");
	publishingError = true;
});

Task("Publish-GitHub-Release")
	.IsDependentOn("Zip-Files")
	.WithCriteria(() => parameters.GitHub.HasCredentials)
	.WithCriteria(() => parameters.ShouldPublish)
	.Does(() =>
{
	GitReleaseManagerAddAssets(
		parameters.GitHub.UserName,
		parameters.GitHub.Password,
		BuildParameters.MainRepoUser,
		BuildParameters.MainRepoName,
		parameters.Version.Milestone,
		parameters.Paths.Files.ZipArtifactsPathDesktop.ToString()
	);
	GitReleaseManagerClose(
		parameters.GitHub.UserName,
		parameters.GitHub.Password,
		BuildParameters.MainRepoUser,
		BuildParameters.MainRepoName,
		parameters.Version.Milestone
	);
});

Task("Create-Release-Notes")
	.WithCriteria(() => parameters.GitHub.HasCredentials)
	.Does(() =>
	{
		GitReleaseManagerCreate(
			parameters.GitHub.UserName,
			parameters.GitHub.Password,
			BuildParameters.MainRepoUser,
			BuildParameters.MainRepoName,
			new GitReleaseManagerCreateSettings
			{
				Milestone = parameters.Version.Milestone,
				Name      = "New Release: " + parameters.Version.Milestone,
				Prerelease = !parameters.IsMainBranch,
				TargetCommitish = BuildParameters.MainBranch
			}
		);
	});

Task("Export-Release-Notes")
	.WithCriteria(() => parameters.GitHub.HasCredentials)
	.Does(() =>
{
	GitReleaseManagerExport(
		parameters.GitHub.UserName,
		parameters.GitHub.Password,
		BuildParameters.MainRepoUser,
		BuildParameters.MainRepoName,
		File("./CHANGELOG.md")
	);
});

Task("Package")
	.IsDependentOn("Zip-Files")
	.IsDependentOn("Create-NuGet-Packages");

Task("Default")
	.IsDependentOn("Package");

Task("AppVeyor")
	.IsDependentOn("Publish-MyGet")
	.IsDependentOn("Publish-NuGet")
	.IsDependentOn("Publish-GitHub-Release")
	.Finally(() =>
{
	if (publishingError)
	{
		throw new Exception("An error occurred during the publishing of " + BuildParameters.MainRepoName + ". All publishing tasks have been attempted.");
	}
});

Task("Unix-CI")
	.IsDependentOn("Run-Unit-Tests");

Task("ReleaseNotes")
	.IsDependentOn("Create-Release-Notes");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(parameters.Target);
