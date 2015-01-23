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
DefaultDirName={pf}\WetNet

[Files]
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetSvc\bin\x64\Release\WetEngine.config.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetSvc\bin\x64\Release\WetLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\WetSvc\bin\x64\Release\WetSvc.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Database\wetdb.sql"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\apache-tomcat-8.0.15.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\mysql-5.6.22-winx64.msi"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: DB
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\mysql-connector-odbc-5.3.2-winx64.msi"; DestDir: "{tmp}"; Flags: deleteafterinstall; Components: DB
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\WebApp\ROOT.war"; DestDir: "{app}"
Source: "D:\Sincronia\SkyDrive\Files\Lavoro\Progetti\Visual Studio\WetNet\Setup\Packages\vcredist_x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Types]
Name: "Full"; Description: "All components"
Name: "Custom"; Description: "Custom installation"; Flags: iscustom

[Components]
Name: "Svc"; Description: "WetNet Service"; Types: Full Custom
Name: "DB"; Description: "MySQL Server"; Types: Custom Full
Name: "Tomcat"; Description: "Tomcat Web-Server"; Types: Custom Full

[Tasks]
Name: "InstallService"; Description: "Install .NET Framework"; Components: Svc
Name: "InstallDB"; Description: "Install MySQL database"; Components: DB
Name: "InstallWebApp"; Description: "Install Tomcat and web-application"; Components: Tomcat

[Run]
Filename: "{tmp}\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"; Parameters: "/passive /norestart"; WorkingDir: "{tmp}"; Flags: waituntilterminated; StatusMsg: "Installing Microsoft .NET Framework 4.5.2 ..."; Tasks: InstallService
Filename: "{tmp}\mysql-5.6.22-winx64.msi"; Parameters: "/passive /norestart"; WorkingDir: "{tmp}"; Flags: waituntilterminated; StatusMsg: "Installing MySQL database service..."; Tasks: InstallDB
Filename: "{pf}\MySQL\MySQL Server 5.6\bin\MySQLInstanceConfig.exe"; Parameters: "-i -q ServiceName=MySQL RootPassword=root ServerType=SERVER DatabaseType=INNODB Port=3306 Charset=utf8"; Flags: waituntilterminated; StatusMsg: "Configuring MySQL instance..."; Tasks: InstallDB
Filename: "{tmp}\vcredist_x64.exe"; Parameters: "/passive /norestart"; WorkingDir: "{tmp}"; Flags: waituntilterminated; StatusMsg: "Installing Visual C++ 2010 SP1 x64 redistributable package..."; Tasks: InstallDB
Filename: "{tmp}\mysql-connector-odbc-5.3.2-winx64.msi"; Parameters: "/passive /norestart"; WorkingDir: "{tmp}"; Flags: waituntilterminated; StatusMsg: "Installing MySQL ODBC Connector x64..."; Tasks: InstallDB
Filename: "{pf}\MySQL\MySQL Server 5.6\bin\mysql.exe"; Parameters: "< wetdb.sql"; WorkingDir: "{tmp}"; Flags: waituntilterminated; StatusMsg: "Creating DB from model..."; Tasks: InstallDB
Filename: "{tmp}\apache-tomcat-8.0.15.exe"; Parameters: "/s"; Flags: waituntilterminated

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "greek"; MessagesFile: "compiler:Languages\Greek.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "nepali"; MessagesFile: "compiler:Languages\Nepali.islu"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "scottishgaelic"; MessagesFile: "compiler:Languages\ScottishGaelic.isl"
Name: "serbiancyrillic"; MessagesFile: "compiler:Languages\SerbianCyrillic.isl"
Name: "serbianlatin"; MessagesFile: "compiler:Languages\SerbianLatin.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"
