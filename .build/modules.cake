public static List<string> Modules = new List<string>{
    "#module nuget:?package=Cake.DotNetTool.Module&version=0.1.0"
};

public static void InstallModules(ICakeContext context)
{
    var script = context.MakeAbsolute(context.File($"./{Guid.NewGuid()}.cake"));
    try
    {
        var arguments = new Dictionary<string, string>();
        arguments.Add("bootstrap", "");

        if(BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient") != null) {
            arguments.Add("nuget_useinprocessclient", BuildParameters.CakeConfiguration.GetValue("NuGet_UseInProcessClient"));
        }

        if(BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification") != null) {
            arguments.Add("settings_skipverification", BuildParameters.CakeConfiguration.GetValue("Settings_SkipVerification"));
        }

        System.IO.File.WriteAllText(script.FullPath, string.Join("\n", Modules));
        context.CakeExecuteScript(script,
            new CakeSettings
            {
                Arguments = arguments
            });
    }
    finally
    {
        if (context.FileExists(script))
        {
            context.DeleteFile(script);
        }
    }
}

Setup((context) => InstallModules(context));
