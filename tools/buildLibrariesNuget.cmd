SET PACKAGE_VERSION="2.5.1"

mkdir .\src\nuget
dotnet pack src/GeneticSharp.Domain/GeneticSharp.Domain.csproj -c release --no-build --output ../nuget /p:PackageVersion=%PACKAGE_VERSION%
dotnet pack src/GeneticSharp.Extensions/GeneticSharp.Extensions.csproj -c release --no-build --output ../nuget /p:PackageVersion=%PACKAGE_VERSION%