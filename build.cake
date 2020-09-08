#addin "Cake.Git"
#tool "nuget:?package=GitVersion.CommandLine&version=5.1.3"

var args = new
{
    Target = Argument("target", "Default"),
    OutputDirectory = Argument("output", "build"),
    RepositoryPath = Argument("repositoryPath", "."),
    Configuration = Argument("configuration", "Release"),
	AddCommitToDescription = Argument("addCommitToDescription", true),
    Nuget = new
	{
		Source = Argument("nugetSource", "https://www.nuget.org/api/v2/package"),
		ApiKey = Argument("nugetApiKey", ""),
	},
};

var buildDirectory = MakeAbsolute(Directory(args.OutputDirectory));

Task("Clean").Does(() =>
{
	CleanDirectories(args.OutputDirectory);
	CleanDirectories("./**/bin");
	CleanDirectories("./**/obj");
});


GitVersion versionInfo = null;
Task("Version")
	.Description("Retrieves the current version from the git repository")
	.Does(() => {
		
		versionInfo = GitVersion(new GitVersionSettings {
			UpdateAssemblyInfo = false
		});
			
		Information("Major:\t\t\t\t\t" + versionInfo.Major);
		Information("Minor:\t\t\t\t\t" + versionInfo.Minor);
		Information("Patch:\t\t\t\t\t" + versionInfo.Patch);
		Information("MajorMinorPatch:\t\t\t" + versionInfo.MajorMinorPatch);
		Information("SemVer:\t\t\t\t\t" + versionInfo.SemVer);
		Information("LegacySemVer:\t\t\t\t" + versionInfo.LegacySemVer);
		Information("LegacySemVerPadded:\t\t\t" + versionInfo.LegacySemVerPadded);
		Information("AssemblySemVer:\t\t\t\t" + versionInfo.AssemblySemVer);
		Information("FullSemVer:\t\t\t\t" + versionInfo.FullSemVer);
		Information("InformationalVersion:\t\t\t" + versionInfo.InformationalVersion);
		Information("BranchName:\t\t\t\t" + versionInfo.BranchName);
		Information("Sha:\t\t\t\t\t" + versionInfo.Sha);
		Information("NuGetVersionV2:\t\t\t\t" + versionInfo.NuGetVersionV2);
		Information("NuGetVersion:\t\t\t\t" + versionInfo.NuGetVersion);
		Information("CommitsSinceVersionSource:\t\t" + versionInfo.CommitsSinceVersionSource);
		Information("CommitsSinceVersionSourcePadded:\t" + versionInfo.CommitsSinceVersionSourcePadded);
		Information("CommitDate:\t\t\t\t" + versionInfo.CommitDate);
  });	

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
    .Does(() =>
{
    	var sln = "FluentDragDrop.sln";

    // NuGetRestore(sln);
    // MSBuild(sln, c =>
    // {
    //     c.Configuration = args.Configuration;
    // });

		var settings = new DotNetCoreBuildSettings {
			Configuration = args.Configuration,
			OutputDirectory = args.OutputDirectory		 
		};
	 
		settings.MSBuildSettings = new DotNetCoreMSBuildSettings()
		 .WithProperty("PackageVersion", versionInfo.NuGetVersionV2)
		 .WithProperty("Version", versionInfo.AssemblySemVer)
		 .WithProperty("InformationalVersion", versionInfo.InformationalVersion)
		 .WithProperty("PackageOutputPath", args.OutputDirectory);
	 
		// creates also the NuGet packages
		DotNetCoreBuild("FluentDragDrop\\FluentDragDrop.csproj", settings);
});

Task("Pack")
	.IsDependentOn("Build")
	.Does(() =>
{
    if(!DirectoryExists(buildDirectory.FullPath))
        CreateDirectory(buildDirectory.FullPath);

    var nuspecFiles = GetFiles("**/*.nuspec");
    foreach(var nuspec in nuspecFiles)
    {
        var wd = MakeAbsolute(nuspec).GetDirectory();
        var settings = new NuGetPackSettings
        {
            Version = versionInfo.NuGetVersionV2,
            OutputDirectory = buildDirectory.FullPath,
            BasePath = wd,
        };

		if(args.AddCommitToDescription)
		{
			// Extract description from nuspec and concat current commit hash and datetime
			var description = XmlPeek(nuspec, $"/package/metadata/description");
			var lastCommit = GitLogTip(args.RepositoryPath);
			description = $"{description}\n\nCommit : {lastCommit.Sha}, {lastCommit.Author?.When}";
			Information($"Updated package description : {description}");
			settings.Description = description;
		}

        NuGetPack(nuspec, settings);
    }
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(args.Target);