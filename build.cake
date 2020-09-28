#addin "Cake.FileHelpers"
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


Task("BuildFullFramework")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
    .Does(() =>
{
	
		ReplaceRegexInFiles("./FluentDragDrop-net45/**/AssemblyInfo.*", "(?<=AssemblyBuildDate\\(\")([0-9\\-\\:T]+)(?=\"\\))", DateTime.Now.ToString("s"));
		ReplaceRegexInFiles("./FluentDragDrop-net45/**/*.csproj", "(?<=<ReleaseVersion>).*?(?=</ReleaseVersion>)", versionInfo.AssemblySemVer);
		ReplaceRegexInFiles("./FluentDragDrop-net45/**/*.csproj", "(?<=<Version>).*?(?=</Version>)", versionInfo.AssemblySemVer);

	 	// Use MSBuild

		MSBuild("FluentDragDrop-net45\\FluentDragDrop-net45.csproj", settings =>
			settings.SetConfiguration(args.Configuration)
					.SetPlatformTarget(PlatformTarget.MSIL)
					.UseToolVersion(MSBuildToolVersion.VS2019));
});

Task("Pack")
	.IsDependentOn("Build")
	.IsDependentOn("BuildFullFramework")
	.Does(() =>
{
    if(!DirectoryExists(buildDirectory.FullPath))
        CreateDirectory(buildDirectory.FullPath);

    var nuspecFiles = GetFiles("**/FluentDragDrop*.nuspec");
    foreach(var nuspec in nuspecFiles)
    {
        var wd = MakeAbsolute(nuspec).GetDirectory();
        var settings = new NuGetPackSettings
        {
            Version = versionInfo.NuGetVersionV2,
            OutputDirectory = buildDirectory.FullPath,
            BasePath = wd,
        };

		// TODO: does not work: XmlPeek says 'Failed to find node matching the XPath'
		//		 and replaces the description text (see Vibrancy.Forms as well)
		// if(args.AddCommitToDescription)
		// {
		// 	// Extract description from nuspec and concat current commit hash and datetime
		// 	var description = XmlPeek(nuspec, $"/package/metadata/description");
		// 	var lastCommit = GitLogTip(args.RepositoryPath);
		// 	description = $"{description}\n\nCommit : {lastCommit.Sha}, {lastCommit.Author?.When}";
		// 	Information($"Updated package description : {description}");
		// 	settings.Description = description;
		// }

        NuGetPack(nuspec, settings);
    }
});

Task("Default")
    .IsDependentOn("Pack");

RunTarget(args.Target);