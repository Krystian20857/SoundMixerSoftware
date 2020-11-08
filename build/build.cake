var configuration = Argument("Configuration", "Release");
var target = Argument("Target", "Build");

var outputDir = Directory($"../output/{configuration.ToLower()}");
var solution = "../SoundMixerSoftware.sln";

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(outputDir);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore(solution);
    });

    
Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new MSBuildSettings{
        Configuration = configuration,
    };
    
    MSBuild(solution, settings);
});

RunTarget(target);