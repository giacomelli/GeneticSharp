SET PACKAGE_VERSION="3.0.0"

mkdir .\src\nuget

echo Packing GeneticSharp
dotnet pack src/GeneticSharp.Domain/GeneticSharp.Domain.csproj -c release --output ./src/nuget /p:PackageVersion=%PACKAGE_VERSION%

echo Packing GeneticSharp.Extensions
dotnet pack src/GeneticSharp.Extensions/GeneticSharp.Extensions.csproj -c release --output ./src/nuget /p:PackageVersion=%PACKAGE_VERSION%