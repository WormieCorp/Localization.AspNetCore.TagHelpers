#load "./tasks.cake"

CleanTask
  .Does<BuildDirectories>((dirs) =>
{
  var directories = GetDirectories("src/**/bin")
    + GetDirectories("src/**/obj")
    + GetDirectories("test/**/bin")
    + GetDirectories("test/**/obj")
    + GetDirectories("test/**/TestResults")
    + GetDirectories("**/wwwroot/lib")
    + GetDirectories(dirs.ArtifactsBaseDir + "/*")
    - GetDirectories(dirs.PackageCache ?? (dirs.ArtifactsBaseDir + "/_cache"));

  DeleteDirectories(directories, new DeleteDirectorySettings
  {
    Recursive = true,
    Force = true
  });
});
