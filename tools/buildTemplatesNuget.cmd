cd .\src\Templates

nuget pack -Version 1.0.0
dotnet new -i GeneticSharp.Templates.1.0.0.nupkg

# GeneticSharpConsoleApp
dotnet new GeneticSharpConsoleApp -n ConsoleApp -o ConsoleApp
dotnet build ConsoleApp/ConsoleApp.csproj # We don't run it, because this templates throw an exception asking to programmer to implement the Chromomosome representation.

# GeneticSharpTspConsoleApp
dotnet new GeneticSharpTspConsoleApp -n TspConsoleApp -o TspConsoleApp
dotnet run TspConsoleApp/TspConsoleApp.csproj

# GeneticSharpTspUnity3d
dotnet new GeneticSharpTspUnity3d -n TspUnity3d -o TspUnity3d
# We don't build this template, because it needs Unity3d installed.