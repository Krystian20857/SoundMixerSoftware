#addin "nuget:?package=Cake.VersionReader&version=5.1.0"
#addin "Cake.Squirrel&version=0.15.1"

#tool "nuget:?package=nuget.commandline&version=5.3.0"
#tool "Squirrel.Windows&version=2.0.1"

var releaseFolder = "../output/release";
var packageFolder = "../output/packages";
var squirrelFolder = $"{packageFolder}/release";
var version = GetVersionNumber($"{releaseFolder}/SoundMixerSoftware.exe");
var packFile = $"{packageFolder}/SoundMixerSoftware.{version}.nupkg";
var iconPath = $"../src/SoundMixerSoftware/Resources/App.ico";
var gifPath = $"../src/SoundMixerSoftware/Resources/Loading.gif";

var clean = Argument("target", "Clean");
var nugetTarget = Argument("target", "BuildNuget");
var squirrelTarget = Argument("target", "BuildSquirrel");
var moveReleaseTarget = Argument("target", "MoveRelease");

Task("Clean")
    .Does(() => {
        CleanDirectory(packageFolder); 
    });

Task("BuildNuget")
    .IsDependentOn("Clean")
    .Does(() => {
        var nugetSettings = new NuGetPackSettings{
        
            Id = "SoundMixerSoftware",
            Version = version,
            Title = "SoundMixerSoftware",
            Authors = new List<string>(){"I Love u too"},
            BasePath = releaseFolder,
            OutputDirectory = packageFolder
        };
        
        NuGetPack($"./SoundMixerSoftware.nuspec", nugetSettings);
    });

Task("BuildSquirrel")
    .IsDependentOn("BuildNuget")
	.Does(() => {
		var settings = new SquirrelSettings{
		    NoMsi = true,
		    Silent = false,
		    ReleaseDirectory = squirrelFolder,
		    SetupIcon = iconPath,
		    Icon = iconPath,
		    FrameworkVersion = "net472",
		    LoadingGif = gifPath
		};
		
		Squirrel(File(packFile), settings);
	});
	
Task("MoveRelease")
    .IsDependentOn("BuildSquirrel")
    .Does(() => {
        CopyDirectory(squirrelFolder, packageFolder);
        DeleteDirectory($"{squirrelFolder}",
        new DeleteDirectorySettings {
            Recursive = true,
            Force = true
        });
    });
	
RunTarget(moveReleaseTarget);