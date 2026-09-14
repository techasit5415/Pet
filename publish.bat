@echo off
cd /d "%~dp0"
dotnet publish DesktopPet.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
if errorlevel 1 (
 pause
 exit /b 1
)
echo Launch publish\DesktopPet.exe
pause
