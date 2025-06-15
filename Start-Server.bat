@echo off
REM Get current path (directory where this .bat is located)
set "BASE_PATH=%~dp0"

REM Remove trailing backslash (optional but clean)
set "BASE_PATH=%BASE_PATH:~0,-1%"

REM Append relative path to instance binary
set "TARGET_PATH=%BASE_PATH%\Instance-Service\bin\Debug\net7.0"

REM Change to that directory
cd /d "%TARGET_PATH%"

REM Start the executable
start "" Instance-Service.exe