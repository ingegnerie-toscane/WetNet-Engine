@echo off
start "" /b /w net stop WetSvc
start "" /b /w %WINDIR%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /u WetSvc.exe
