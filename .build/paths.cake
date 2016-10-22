public class BuildPaths
{
	public BuildFiles Files { get; private set; }
	public BuildDirectories Directories { get; private set; }

	public static BuildPaths GetPaths(
		ICakeContext context,
		string configuration,
		string semVersion
	)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}

		if (string.IsNullOrEmpty(configuration))
		{
			throw new ArgumentNullException("configuration");
		}

		if (string.IsNullOrEmpty(semVersion))
		{
			throw new ArgumentNullException("semVersion");
		}

    const string AppName = "Localization.AspNetCore.TagHelpers";
		var buildDir = context.Directory("./src/" + AppName + "/bin/" + configuration);
		var artifactsDir = (DirectoryPath)context.Directory("./artifacts/v" + semVersion);
		var artifactsBinDir = artifactsDir.Combine("bin");
		var artifactsBinNet451 = artifactsBinDir.Combine("net451");
		var testResultsDir = artifactsDir.Combine("test-results");
		var nugetRoot = artifactsDir.Combine("nuget");
		var testingDir = context.Directory("./test/" + AppName + ".Tests/bin/" + configuration);

		var locFiles = new FilePath[]
		{
			context.File(AppName + ".dll"),
			context.File(AppName + ".pdb"),
			context.File(AppName + ".xml")
		};

		var locAssemblyPaths = locFiles.Select(file => buildDir.Path.CombineWithFilePath(file)).ToArray();

		var testingAssemblyPaths = new FilePath[]
		{
			testingDir + context.File(AppName + ".Tests.dll")
		};

		var repoFilPaths = new FilePath[]
		{
			"LICENSE",
			"README.md",
			"CHANGELOG.md"
		};

		var buildProjects = new List<DirectoryPath>
		{
			context.Directory("./src/" + BuildParameters.MainRepoName),
			context.Directory("./test/" + BuildParameters.MainRepoName + ".Tests")
		};

		if (context.BuildSystem().IsLocalBuild)
		{
			buildProjects.Add(context.Directory("./src/Localization.Demo"));
		}

		var artifactSourcePaths = locAssemblyPaths.Concat(testingAssemblyPaths.Concat(repoFilPaths)).ToArray();

		var zipArtifactsPathDesktop = artifactsDir.CombineWithFilePath(AppName + "-bin-net451-v" + semVersion + ".zip");

		var testCoverageOutputFilePath = testResultsDir.CombineWithFilePath("OpenCover.xml");

		var BuildDirectories = new BuildDirectories(
			artifactsDir,
			testResultsDir,
			nugetRoot,
			artifactsBinDir,
			artifactsBinNet451,
			buildProjects.ToArray()
		);

		var buildFiles = new BuildFiles(
			context,
			locAssemblyPaths,
			testingAssemblyPaths,
			repoFilPaths,
			artifactSourcePaths,
			zipArtifactsPathDesktop,
			testCoverageOutputFilePath
		);

		return new BuildPaths
		{
			Files = buildFiles,
			Directories = BuildDirectories
		};
	}
}

public class BuildFiles
{
	public ICollection<FilePath> Assemblypaths { get; private set; }
	public ICollection<FilePath> TestingAssemblyPaths { get; private set; }
	public ICollection<FilePath> RepoFilePaths { get; private set; }
	public ICollection<FilePath> ArtifactsSourcePaths { get; private set; }
	public FilePath ZipArtifactsPathDesktop { get; private set; }
	public FilePath TestCoverageOutputFilePath { get; private set; }

	public BuildFiles(
		ICakeContext context,
		FilePath[] assemblypaths,
		FilePath[] testingAssemblyPaths,
		FilePath[] repoFilPaths,
		FilePath[] artifactsSourcePaths,
		FilePath zipArtifactsPathDesktop,
		FilePath testCoverageOutputFilePath
	)
	{
		Assemblypaths = Filter(context, assemblypaths);
		TestingAssemblyPaths = Filter(context, testingAssemblyPaths);
		RepoFilePaths = Filter(context, repoFilPaths);
		ArtifactsSourcePaths = Filter(context, artifactsSourcePaths);
		ZipArtifactsPathDesktop = zipArtifactsPathDesktop;
		TestCoverageOutputFilePath = testCoverageOutputFilePath;
	}

	private static FilePath[] Filter(ICakeContext context, FilePath[] files)
	{
		if (!context.IsRunningOnWindows())
		{
			return files.Where(f => !f.FullPath.EndsWith("pdb")).ToArray();
		}
		return files;
	}
}

public class BuildDirectories
{
	public DirectoryPath Artifacts { get; private set; }
	public DirectoryPath TestResults { get; private set; }
	public DirectoryPath NugetRoot { get; private set; }
	public DirectoryPath ArtifactsBin { get; private set; }
	public DirectoryPath ArtifactsBinNet451 { get; private set; }
	public ICollection<DirectoryPath> Projects { get; private set; }
	public ICollection<DirectoryPath> ToClean { get; private set; }

	public BuildDirectories(
		DirectoryPath artifactsDir,
		DirectoryPath testResultsDir,
		DirectoryPath nugetRoot,
		DirectoryPath artifactsBinDir,
		DirectoryPath artifactsBinNet451,
		DirectoryPath[] projects
	)
	{
		Artifacts = artifactsDir;
		TestResults = testResultsDir;
		NugetRoot = nugetRoot;
		ArtifactsBin = artifactsBinDir;
		ArtifactsBinNet451 = artifactsBinNet451;
		Projects = projects;
		ToClean = new[]
		{
			Artifacts,
			TestResults,
			NugetRoot,
			ArtifactsBin,
			ArtifactsBinNet451
		}.Concat(projects.Select(p => p.Combine("bin")))
		 .Concat(projects.Select(p => p.Combine("obj")))
		 .ToArray();
	}
}
