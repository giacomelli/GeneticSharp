SET PACKAGE_VERSION=1.1.0

cd .\src\Templates

nuget pack -Version %PACKAGE_VERSION%
dotnet new -i GeneticSharp.Templates.%PACKAGE_VERSION%.nupkg

echo GeneticSharpTspBlazor
dotnet new GeneticSharpTspBlazor -n TspBlazor -o TspBlazor

echo GeneticSharpConsoleApp
dotnet new GeneticSharpConsoleApp -n ConsoleApp -o ConsoleApp
dotnet build ConsoleApp/ConsoleApp.csproj

echo GeneticSharpTspConsoleApp
dotnet new GeneticSharpTspConsoleApp -n TspConsoleApp -o TspConsoleApp
dotnet build TspConsoleApp/TspConsoleApp.csproj

echo GeneticSharpTspUnity3d
dotnet new GeneticSharpTspUnity3d -n TspUnity3d -o TspUnity3d

cd ..\..