call ./test-on-jekyll.cmd

cd ../../../../giacomelli.github.io-jekyll/src/apps/geneticsharp-runner-blazorapp
git status
git add .
git commit -m"GeneticSharp Runner BlazorApp updated"
git push

cd ../../../../GeneticSharp/src/GeneticSharp.Runner.BlazorApp/tools