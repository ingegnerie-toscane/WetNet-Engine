@echo off
start "" /b /w %WINDIR%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe WetSvc.exe
start "" /b /w net start WetSvc

