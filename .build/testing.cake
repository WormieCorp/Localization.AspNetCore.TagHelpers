public static class UnitTesting
{
	public static void RunUnitTest(ICakeContext context, FilePath project, bool netCore, BuildParameters parameters)
	{
		FilePath testResultOutput;
		if (!netCore && parameters.IsRunningOnUnix)
		{
			var name = project.GetFilenameWithoutExtension();
			var dirPath = project.GetDirectory().FullPath;
			var config = parameters.Configuration;
			var nunit = context.GetFiles(dirPath + "/bin/" + config + "/net451/*/dotnet-test-nunit.exe").First().FullPath;
			var testFile = context.GetFiles(dirPath + "/bin/" + config + "/net451/*/" + name + ".dll").First().FullPath;

			using (var process = context.StartAndReturnProcess("mono", new ProcessSettings { Arguments = nunit + " " + testFile }))
			{
				process.WaitForExit();
				if (process.GetExitCode() != 0)
				{
					throw new Exception("Mono tests failed!");
				}
			}
			testResultOutput = parameters.Paths.Directories.TestResults + "/TestResult-net451.xml";
			if (context.FileExists(testResultOutput))
			{
				context.DeleteFile(testResultOutput);
			}

			context.MoveFile("./TestResult.xml", testResultOutput);
			return;
		}
		else if (parameters.IsRunningOnUnix)
			return; // Fails somehow on unix with a dotnet-test-nunit not found

		Action<ICakeContext> testAction = tool =>
		{
			tool.DotNetCoreTest(project.GetDirectory().FullPath, new DotNetCoreTestSettings
			{
				Configuration = parameters.Configuration,
				NoBuild = true,
				Verbose = false,
				Framework = netCore ? "netcoreapp1.0" : "net451"
			});
		};

		if (parameters.SkipOpenCover)
		{
			testAction(context);
		}
		else
		{
			context.OpenCover(testAction,
				parameters.Paths.Files.TestCoverageOutputFilePath,
				new OpenCoverSettings
				{
					ReturnTargetCodeOffset = 0,
					MergeOutput = true,
					OldStyle = true
				}
				.WithFilter("+[*]* -[*Tests]*")
			);
		}

		testResultOutput = parameters.Paths.Directories.TestResults.CombineWithFilePath("TestResult-" + (netCore ? "netcore" : "net451") + ".xml");

		if (context.FileExists(testResultOutput))
		{
			context.DeleteFile(testResultOutput);
		}

		context.MoveFile("./TestResult.xml", testResultOutput);
	}
}
