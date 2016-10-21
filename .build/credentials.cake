public class BuildCredentials
{
	public string UserName { get; private set; }
	public string Password { get; private set; }

	public bool HasCredentials
	{
		get
		{
			return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
		}
	}

	public static BuildCredentials GetGitHubCredentials(ICakeContext context)
	{
		return new BuildCredentials
		{
			UserName = context.EnvironmentVariable("GITHUB_USERNAME"),
			Password = context.EnvironmentVariable("GITHUB_PASSWORD")
		};
	}
}
