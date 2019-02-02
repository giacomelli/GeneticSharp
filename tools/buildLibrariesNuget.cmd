mkdir .\src\nuget
dotnet pack src/GeneticSharp.Domain/GeneticSharp.Domain.csproj -c release --no-build --output ../nuget /p:PackageVersion=2.5.0
dotnet pack src/GeneticSharp.Extensions/GeneticSharp.Extensions.csproj -c release --no-build --output ../nuget /p:PackageVersion=2.5.0