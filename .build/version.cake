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
        milestone = version;
      }

      GitVersion assertedVersions = context.GitVersion(new GitVersionSettings
      {
        OutputType = GitVersionOutput.Json,
      });

      version = assertedVersions.MajorMinorPatch;
      semVersion = assertedVersions.LegacySemVerPadded;
      milestone = version;

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
    var projects = context.GetFiles("./src/*/project.json");
    foreach(var project in projects)
    {
      context.Information("Patching project at path: '" + project + "'");
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
        var newReleaseNotes = string.Join("\r\n", RemoveMarkdown(releaseNotesArray));
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
          jw.IndentChar = ' ';
          jw.Indentation = 2;
          JsonSerializer serializer = new JsonSerializer();
          serializer.Formatting = Formatting.Indented;
          serializer.Serialize(jw, node);
        }
      }

      return true;
    };
    return false;
  }

  private static string[] RemoveMarkdown(string[] releaseNotes)
  {
    var newNotes = new List<string>();

    try
    {
      for (int i = 0; i < releaseNotes.Length; i++)
      {
        var note = releaseNotes[i];
        if (note.StartsWith("__") && note.EndsWith("__"))
        {
          if (i > 1)
          {
            newNotes.Add("");
          }
          newNotes.Add("");
          newNotes.Add(note.Trim('_').Trim());
          newNotes.Add("");
        }
        else if (note.StartsWith("-"))
        {
          int index = note.IndexOf(')');
          if (index > 0)
          {
            string issue = note.Substring(index + 1).Trim();
            newNotes.Add(note.Substring(0, 2) + issue);
          }
          else
          {
            newNotes.Add(note);
          }
        }
        else if (note.IndexOf("part of this release") > 0)
        {
          continue;
        }
        else
        {
          newNotes.Add(note);
        }
      }
    } catch {}

    return newNotes.ToArray();
  }
}
