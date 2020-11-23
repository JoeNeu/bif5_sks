@echo off
@echo Deleting bin and obj folders...
REM for /d /r . %%d in (bin, obj) do @if exist “%%d” rd /s/q “%%d”
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"
@echo folders successfully deleted. Close the window.
pause > nul