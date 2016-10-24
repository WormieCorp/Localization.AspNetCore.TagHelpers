using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class BuildVersion
{
  public string Version { get; private set; }
  public string SemVersion { get; private set; }
  public string DotNetAsterix { get; private set; }
  public string Milestone { get; private set; }
  public string CakeVersion { get; private set; }

  public static BuildVersion Calculate(ICakeContext context, BuildParameters parameters)
  {
    if (context == null)
    {
      throw new ArgumentNullException("context");
    }

    string version = null;
    string semVersion = null;
    string milestone = null;

    if (context.IsRunningOnWindows() && !parameters.SkipGitVersion)
    {
      context.Information("Calculating Semantic Version");
      if (!parameters.IsLocalBuild || parameters.IsPublishBuild || parameters.IsReleaseBuild)
      {
        context.GitVersion(new GitVersionSettings{
          UpdateAssemblyInfoFilePath = "./src/SolutionInfo.cs",
          UpdateAssemblyInfo = true,
          OutputType = GitVersionOutput.BuildServer
        });

        version = context.EnvironmentVariable("GitVersion_MajorMinorPatch");
        semVersion = context.EnvironmentVariable("GitVersion_LegacySemVerPadded");
        milestone = string.Concat("v", version);
      }

      GitVersion assertedVersions = context.GitVersion(new GitVersionSettings
      {
        OutputType = GitVersionOutput.Json,
      });

      version = assertedVersions.MajorMinorPatch;
      semVersion = assertedVersions.LegacySemVerPadded;
      milestone = string.Concat("v", version);

      context.Information("Calculated Semantic Version: {0}", semVersion);
    }

    if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(semVersion))
    {
      context.Information("Fetching verson from first project.json...");
      version = ReadProjectJsonVersion(context);
      semVersion = version;
      milestone = string.Concat("v", version);
    }

    var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

    return new BuildVersion
    {
      Version = version,
      SemVersion = semVersion,
      DotNetAsterix = semVersion.Substring(version.Length).TrimStart('-'),
      Milestone = milestone,
      CakeVersion = cakeVersion
    };
  }

  public static string ReadProjectJsonVersion(ICakeContext context)
  {
    var projects = context.GetFiles("./**/project.json");
    foreach(var project in projects)
    {
      var content = System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);
      var node = JObject.Parse(content);
      if(node["version"] != null)
      {
        var version = node["version"].ToString();
        return version.Replace("-*", "");
      }
    }
    throw new CakeException("Could not parse version.");
  }

  public bool PatchProjectJson(FilePath project, string[] releaseNotesArray)
  {
    var content = System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);
    var node = JObject.Parse(content);

    var packOptions = node["packOptions"];
    bool releaseNotesChanged = false;
    if (packOptions != null)
    {
      var releaseNotes = packOptions["releaseNotes"];
      if (releaseNotes != null)
      {
        var oldReleaseNotes = releaseNotes.ToString();
        var newReleaseNotes = string.Join("\r\n", releaseNotesArray);
        if (newReleaseNotes != oldReleaseNotes)
        {
          releaseNotes.Replace(newReleaseNotes);
          releaseNotesChanged = true;
        }
      }
    }

    if(node["version"] != null)
    {
      var oldVersion = node["version"].ToString();
      var newVersion = string.Concat(Version, "-*");
      if (oldVersion != newVersion || releaseNotesChanged)
      {
        node["version"].Replace(string.Concat(Version, "-*"));
        using (var fs = System.IO.File.OpenWrite(project.FullPath))
        using (var sw = new StreamWriter(fs))
        using (var jw = new JsonTextWriter(sw))
        {
          jw.IndentChar = '\t';
          jw.Indentation = 1;
          JsonSerializer serializer = new JsonSerializer();
          serializer.Formatting = Formatting.Indented;
          serializer.Serialize(jw, node);
        }
      }

      return true;
    };
    return false;
  }
}
