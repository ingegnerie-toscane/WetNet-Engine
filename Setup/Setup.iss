[Files]
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetLib\bin\Release\WetLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetLib\bin\Release\WetEngine.config.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetSvc\bin\Release\WetSvc.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\PreReqs\NDP451-KB2859818-Web.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: CheckDotNet451

[Run]
Filename: "{tmp}\NDP451-KB2859818-Web.exe"; Parameters: "/norestart /passive /showrmui"; WorkingDir: "{tmp}"; Flags: waituntilterminated; Check: CheckDotNet451
Filename: "{dotnet40}\installutil.exe"; Parameters: "-i WetSvc.exe"; WorkingDir: "{app}"

[UninstallRun]
Filename: "{dotnet40}\installutil.exe"; Parameters: "-u WetSvc.exe"; WorkingDir: "{app}"

[Setup]
AppName=WetNet
AppVersion=1.0.0.0
AppCopyright=Copyright (C) Ingegnerie Toscane 2014-2015
AppId={{67167378-D6A9-4324-88E2-41A0B0D09CBA}
UninstallDisplayName=WetNet
UninstallDisplayIcon={uninstallexe}
VersionInfoVersion=1.0.0.0
VersionInfoCompany=Ingegnerie Toscane S.r.l.
VersionInfoDescription=WetNet setup application
VersionInfoCopyright=Copyright (C) Ingegnerie Toscane 2014-2015
VersionInfoProductName=Setup
VersionInfoProductVersion=1.0.0.0
VersionInfoProductTextVersion=1.0.0.0
SolidCompression=True
InternalCompressLevel=ultra
DefaultDirName={pf}\WetNet
ArchitecturesInstallIn64BitMode=x64

[Code]
function CheckDotNet451() : boolean;

var
  key : string;
  success : boolean;
  release : cardinal;

begin
  key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';
  success := RegQueryDWordValue(HKLM, key, 'Release', release);
  success := success and (release >= 378758);

  result := success;
end;
