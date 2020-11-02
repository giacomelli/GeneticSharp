cd ../
dotnet publish -c release
xcopy "bin/Release/netstandard2.0/publish/GeneticSharp.Runner.BlazorApp/dist" "../../../giacomelli.github.io-jekyll/src/apps/geneticsharp-runner-blazorapp" /E /Y
rm -rf bin/

cd tools
xcopy "jekyll-files" "../../../../giacomelli.github.io-jekyll/src/apps/geneticsharp-runner-blazorapp" /E /Y 
