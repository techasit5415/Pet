@echo off
cd /d "%~dp0"
dotnet run --project DesktopPet.csproj
if errorlevel 1 pause
