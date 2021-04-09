#tool "nuget:?package=nuget.commandline&version=5.3.0"

var configuration = Argument("Configuration", "Release");
var target = Argument("Target", "AfterBuildClean");

var outputDir = Directory($"../output/{configuration.ToLower()}");
var solution = "../SoundMixerSoftware.sln";

var foldersToRemove = new string[]{"de", "ru", "Ru", "ru-ru", "uz-Latn-UZ", "cs", "cs-CZ", "pt", "pt-BR", "fr", "fr-FR", "ar-DZ"};
var removeFilesWithExtension = new string[]{".pdb"};

Task("Clean")
    .Does(() =>
    {
        Information($"Cleaning output directory {outputDir}...");
        CleanDirectory(outputDir);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        Information($"Restoring nuget packages...");
        NuGetRestore(solution);
    });

    
Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
    Information("Building from sources...");
    
    var settings = new MSBuildSettings{
        Configuration = configuration,
    };
    
    MSBuild(solution, settings);
    });

Task("AfterBuildClean")
    .IsDependentOn("Build")
    .Does(() => {

        Information("Cleaning directories...");        
        foreach(var folder in foldersToRemove)
        {
            var dirToRemove = $"{outputDir}/{folder}";
            if(!DirectoryExists(dirToRemove))
                continue;
            Information($"\t{folder}");
            
            DeleteDirectory(dirToRemove, 
            new DeleteDirectorySettings {
                Recursive = true,
                Force = true
            });
        }
        
        Information("Cleaning files..."); 
        foreach(var extension in removeFilesWithExtension){
            var files = $"{outputDir}/*{extension}";
            Information($"\t{files}");
            DeleteFiles(GlobPattern.FromString(files));        
        }
        
    });

RunTarget(target);