@echo off
setlocal

:: Tìm đường dẫn MSBuild.exe
set "MSBUILD_PATH="
for /f "usebackq tokens=*" %%i in (`"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe 2^>nul`) do (
    set "MSBUILD_PATH=%%i"
)

if not defined MSBUILD_PATH (
    set "MSBUILD_PATH=C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
)

echo Dang build Key-n-Stroke (Release)...
"%MSBUILD_PATH%" KeyNStroke.sln /t:Restore /p:RestorePackagesConfig=true /p:Configuration=Release
if %ERRORLEVEL% neq 0 (
    echo Restore package that bai!
    pause
    exit /b %ERRORLEVEL%
)

"%MSBUILD_PATH%" KeyNStroke.sln /p:Configuration=Release
if %ERRORLEVEL% neq 0 (
    echo Build project that bai!
    pause
    exit /b %ERRORLEVEL%
)

echo Build hoan tat thanh cong!
echo Dang mo folder chua file exe...
explorer "KeyNStroke\bin\Release"

echo Dang chay file exe...
start "" "KeyNStroke\bin\Release\Key-n-Stroke.exe"
