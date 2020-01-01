#load "./tasks.cake"

public interface ICiProvider
{
  bool IsCiProvider(string name);

  string Branch { get; }
  bool IsTag { get; }
  bool SupportsArtifactUploads { get; }

  void UploadArtifacts(IEnumerable<FilePath> artifacts);
}

public abstract class CiProviderBase : ICiProvider
{
  private readonly string _ciName;

  protected CiProviderBase(string ciName)
  {
    this._ciName = ciName;
  }

  public abstract string Branch { get; }
  public virtual bool IsTag => false;
  public virtual bool SupportsArtifactUploads => false;

  public virtual bool IsCiProvider(string name)
  {
    return string.Equals(name, this._ciName, StringComparison.OrdinalIgnoreCase);
  }

  public virtual void UploadArtifacts(IEnumerable<FilePath> artifacts)
  {
    throw new NotSupportedException();
  }
}

public sealed class GitCiProvider : CiProviderBase
{
  public GitCiProvider(ICakeContext context)
    : base("LocalBuild")
  {
    Branch = context.GitBranchCurrent(".")?.CanonicalName;
  }

  public override string Branch { get; }
}

public sealed class AppVeyorCiProvider : CiProviderBase
{
  private readonly BuildSystem _buildSystem;

  public AppVeyorCiProvider(BuildSystem buildSystem)
    : base("AppVeyor")
  {
    this._buildSystem = buildSystem;
  }

  public override string Branch => this._buildSystem.AppVeyor.Environment.Repository.Branch;
  public override bool IsTag => this._buildSystem.AppVeyor.Environment.Repository.Tag.IsTag;

  public override bool SupportsArtifactUploads => true;

  public override void UploadArtifacts(IEnumerable<FilePath> artifacts)
  {
    var provider = this._buildSystem.AppVeyor;

    foreach(var artifact in artifacts)
    {
      provider.UploadArtifact(artifact);
    }
  }
}

AfterTestsTask
  .Does<BuildData>((context, data) =>
{
  if (!data.Ci.SupportsArtifactUploads)
  {
    Warning("Uploading artifacts is not supported");
    return;
  }

  Information("Uploading artifacts to CI provider");
  var artifacts = new[]
  {
    "/*.nupkg",
    "/*.snupkg"
  }.SelectMany(a => context.GetFiles(data.Dirs.NugetPackages + a));

  data.Ci.UploadArtifacts(artifacts);
});
