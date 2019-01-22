@echo ------------------------------------------
@echo Running StyleCop...
@echo ------------------------------------------

StyleCopCmd\Net.SF.StyleCopCmd.Console\StyleCopCmd.exe -sf ..\src\GeneticSharp.sln -of stylecop-report.xml

@echo stylecop-report.xml file created.

@echo Done!
@pause
@exit