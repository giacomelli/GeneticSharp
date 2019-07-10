cd ..
dotnet publish -c release
cp  -R bin/Release/netstandard2.0/publish/GeneticSharp.Runner.BlazorApp/dist/  ../../../giacomelli.github.io-jekyll/src/apps/geneticsharp-runner-blazorapp
rm -rf bin/

cd tools
cp -R jekyll-files/ ../../../../giacomelli.github.io-jekyll/src/apps/geneticsharp-runner-blazorapp
