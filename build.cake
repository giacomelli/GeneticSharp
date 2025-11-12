// Tools & Addins
#tool dotnet:?package=dotnet-sonarscanner&version=11.0.0
#addin nuget:?package=Cake.Sonar&version=5.0.0
#addin nuget:?package=Cake.Git&version=5.0.1

var target      = Argument("target", "Default");
var solutionDir = "./src";
var sonarLogin  = EnvironmentVariable("GeneticSharp_SonarQube_login");

// Detect branch name from CI or local git
string DetectBranch()
{
    var fromEnv =
        EnvironmentVariable("GITHUB_REF_NAME") ??
        EnvironmentVariable("BUILD_SOURCEBRANCHNAME") ??
        EnvironmentVariable("APPVEYOR_REPO_BRANCH");

    if (!string.IsNullOrWhiteSpace(fromEnv))
        return fromEnv;

    try 
    { 
        return GitBranchCurrent(".").FriendlyName; 
    }
    catch 
    { 
        return "main"; 
    }
}
var branch = DetectBranch();

// Validation
if (string.IsNullOrWhiteSpace(sonarLogin))
{
    throw new Exception("Set 'GeneticSharp_SonarQube_login' env var with a SonarCloud token from https://sonarcloud.io/account/security/.");
}

// Tasks
Task("Restore")
    .Does(() =>
{
    DotNetRestore(solutionDir);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetBuild(solutionDir, new DotNetBuildSettings
    {
        Configuration = "Release",
        NoRestore = true
    });
});

Task("Test")
    .Does(() =>
{
    DotNetTest(solutionDir, new DotNetTestSettings
    {
        Configuration = "Release",
        NoBuild = true,
        ArgumentCustomization = args =>
            args.Append("/p:CollectCoverage=true")
                .Append("/p:CoverletOutput=./coverage")
                .Append("/p:CoverletOutputFormat=opencover")
    });
});

Task("SonarBegin")
    .Does(() =>
{
    SonarBegin(new SonarBeginSettings
    {
        Key          = "GeneticSharp",
        Organization = "giacomelli-github",
        Url          = "https://sonarcloud.io",
        Token        = sonarLogin,
        Branch       = branch,
        Verbose      = true,
        OpenCoverReportsPath = "**/coverage.opencover.xml",
        Exclusions = string.Join(",", new[]{
            "GeneticSharp.Benchmarks/**/*.cs",
            "**/*Test.cs",
            "**/Samples/**/*.cs",
            "GeneticSharp.Runner.BlazorApp/**/*.*",
            "src/GeneticSharp.Runner.BlazorApp/wwwroot/js/canvas-helper.js",
            "GeneticSharp.Runner.GtkApp/*.*",
            "src/GeneticSharp.Runner.GtkApp/MainWindow.cs",
            "src/GeneticSharp.Runner.GtkApp/PropertyEditor.cs",
            "Templates/content/**/*.*",
            "src/Templates/content/TspBlazorApp/wwwroot/js/canvas-helper.js",
            "**/*.xml",
            "**/Program.cs",
            "**/AssemblyInfo.cs"
        })
    });
});

Task("SonarEnd")
    .Does(() =>
{
    SonarEnd(new SonarEndSettings
    {
        Token   = sonarLogin
    });
});

Task("Default")
    .IsDependentOn("SonarBegin")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("SonarEnd");

RunTarget(target);
