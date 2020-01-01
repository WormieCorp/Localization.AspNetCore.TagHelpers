// Concrete tasks

private readonly CakeTaskBuilder CleanTask = Task("Clean");
private readonly CakeTaskBuilder RestoreTask = Task("Restore");
private readonly CakeTaskBuilder BeforeBuildTask = Task("Before-Build").IsDependentOn(RestoreTask);
private readonly CakeTaskBuilder BuildTask = Task("Build").IsDependentOn(BeforeBuildTask);
private readonly CakeTaskBuilder AfterBuildTask = Task("After-Build").IsDependentOn(BuildTask);
private readonly CakeTaskBuilder BeforeTestsTask = Task("Before-Tests");
private readonly CakeTaskBuilder TestsTask = Task("Tests").IsDependentOn(BeforeTestsTask);
private readonly CakeTaskBuilder AfterTestsTask = Task("After-Tests").IsDependentOn(TestsTask);
private readonly CakeTaskBuilder BeforeDeploymentTask = Task("Before-Deployment");
private readonly CakeTaskBuilder DeploymentTask = Task("Deployment").IsDependentOn("Before-Deployment");
private readonly CakeTaskBuilder AfterDeploymentTask = Task("After-Deployment").IsDependentOn("Deployment");

// Meta tasks
Task("Full-Deployment")
  .IsDependentOn(AfterDeploymentTask)
  .IsDependentOn("Publish-Documentation");

Task("Default")
  .IsDependentOn(AfterTestsTask);

if (!HasArgument("only-tests"))
{
    BeforeTestsTask.IsDependentOn(AfterBuildTask);
}

if (HasArgument("clean"))
{
  RestoreTask.IsDependentOn(CleanTask);
  BeforeTestsTask.IsDependentOn(CleanTask);
}

var target = Argument("target", "Default");

if (string.Equals(target, "Build", StringComparison.OrdinalIgnoreCase)
  || string.Equals(target, "Tests", StringComparison.OrdinalIgnoreCase))
{
  target = $"After-{target}";
}
else if (string.Equals(target, "Deployment", StringComparison.OrdinalIgnoreCase))
{
  target = "Full-Deployment";
}
