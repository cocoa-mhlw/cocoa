git clean -xdf --exclude=packages

for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"
pause