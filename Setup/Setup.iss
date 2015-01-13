[Setup]
AppName=WetNet
AppVersion=1.0.0.0
AppCopyright=Copyright (C) Ingegnerie Toscane 2013-2014
AppId={{80924E3D-3ED2-48A5-8DBB-B4A6BA650F8F}
LicenseFile=D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\License\EUPL-1.1.txt
DisableProgramGroupPage=yes
UninstallDisplayName=WetNet
UninstallDisplayIcon={uninstallexe}
VersionInfoVersion=1.0.0.0
VersionInfoCompany=Ingegnerie Toscane S.r.l.
VersionInfoDescription=WetNet
VersionInfoCopyright=Copyright (C) Ingegnerie Toscane 2013-2014
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
SolidCompression=True
Compression=lzma2/ultra64
InternalCompressLevel=ultra
MinVersion=0,6.1

[Files]
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetSvc\bin\x64\Release\WetEngine.config.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetSvc\bin\x64\Release\WetLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetSvc\bin\x64\Release\WetSvc.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Database\wetdb.sql"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\apache-tomcat-8.0.15.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\mysql-5.6.22-winx64.msi"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\mysql-connector-odbc-5.3.2-winx64.msi"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Types]
Name: "Standard"; Description: "Standard installation (only WetNet service)"
Name: "Standard + Database"; Description: "Install WetNet service + MySQL server + ODBC connector"
Name: "Standard + Database + Tomcat + WebApp"; Description: "Install WetNet service + MySQL server + ODBC connector + Tomcat + Web Application"
Name: "Custom"; Description: "Custom installation"; Flags: iscustom

[Components]
Name: "WetNet service"; Description: "WetNet service"; Types: Standard + Database + Tomcat + WebApp Standard + Database Custom; Flags: fixed
Name: "WetNet service + Database"; Description: "WetNet service + Database"; Types: Standard + Database Standard + Database + Tomcat + WebApp Custom
Name: "WetNet service + Database + Tomcat + Web Application"; Description: "WetNet service + Database + Tomcat + Web Application"; Types: Standard + Database + Tomcat + WebApp Custom

[Run]
Filename: "{app}\WetSvc.exe"; WorkingDir: "{pf}"
