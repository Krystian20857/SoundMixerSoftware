@echo off

rem copied from https://github.com/Belphemur/SoundSwitch/blob/dev/Installer/Make-Installer.bat

setlocal
cd /d "%~dp0"

if "%PROCESSOR_ARCHITECTURE%"=="x86" (
    set innoSetupRegistryNode=HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Inno Setup 6_is1
) else (
    set innoSetupRegistryNode=HKLM\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Inno Setup 6_is1
)

rem Retrieve registry key of Inno Setup installation
set innoSetupExe=
for /f "eol=; tokens=1,2*" %%a in ('REG QUERY "%innoSetupRegistryNode%" /v InstallLocation') do (
	set innoSetupExe=%%c
)
set innoSetupExe="%innoSetupExe%ISCC.exe"
if not exist %innoSetupExe% (set errorMessage=Inno Setup not found in %innoSetupExe% & goto ERROR_QUIT)

echo Building installer...
%innoSetupExe% installer.iss /DReleaseState=%1
if not %ERRORLEVEL%==0 (set errorMessage=Installer script setup.iss failed & goto ERROR_QUIT)

echo Installer created successfully.
exit /b 0

:ERROR_QUIT
echo.
if defined errorMessage (
    echo Error: %errorMessage%!
) else (
    echo An unknown error occurred.
)
exit /b 1