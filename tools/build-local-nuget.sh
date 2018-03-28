nuget locals all -clear
rm -Rf ../build/local-nuget

dotnet pack ../src/GeneticSharp.Domain/GeneticSharp.Domain.csproj /p:PackageVersion=2.0.0-rc1 
dotnet pack ../src/GeneticSharp.Extensions/GeneticSharp.Extensions.csproj /p:PackageVersion=2.0.0-rc1 

nuget add ../src/GeneticSharp.Domain/bin/Debug/GeneticSharp.2.0.0-rc1.nupkg -source ../build/local-nuget
nuget add ../src/GeneticSharp.Extensions/bin/Debug/GeneticSharp.Extensions.2.0.0-rc1.nupkg -source ../build/local-nuget
