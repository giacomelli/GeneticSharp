cd .\src\Templates

nuget pack -Version 1.0.0
dotnet new -i GeneticSharp.Templates.1.0.0.nupkg

echo GeneticSharpConsoleApp
dotnet new GeneticSharpConsoleApp -n ConsoleApp -o ConsoleApp
dotnet build ConsoleApp/ConsoleApp.csproj

echo GeneticSharpTspConsoleApp
dotnet new GeneticSharpTspConsoleApp -n TspConsoleApp -o TspConsoleApp
dotnet build TspConsoleApp/TspConsoleApp.csproj

echo GeneticSharpTspUnity3d
dotnet new GeneticSharpTspUnity3d -n TspUnity3d -o TspUnity3d

cd ..\..