SET PACKAGE_VERSION=3.1.2

cd .\src\Templates

nuget pack -Version %PACKAGE_VERSION%
dotnet new -u GeneticSharp.Templates
dotnet new -i GeneticSharp.Templates.%PACKAGE_VERSION%.nupkg

echo GeneticSharpTspBlazorApp
dotnet new GeneticSharpTspBlazorApp -n TspBlazorApp -o TspBlazorApp

echo GeneticSharpConsoleApp
dotnet new GeneticSharpConsoleApp -n ConsoleApp -o ConsoleApp
dotnet build ConsoleApp/ConsoleApp.csproj

echo GeneticSharpTspConsoleApp
dotnet new GeneticSharpTspConsoleApp -n TspConsoleApp -o TspConsoleApp
dotnet build TspConsoleApp/TspConsoleApp.csproj

echo GeneticSharpTspUnity3d
dotnet new GeneticSharpTspUnity3d -n TspUnity3d -o TspUnity3d

cd ..\..