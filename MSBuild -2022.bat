cd /d "C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin"
msbuild -t:Restore "%~dp0\IDMSWebServer.sln"
msbuild "%~dp0\IDMSWebServer.sln"
pause