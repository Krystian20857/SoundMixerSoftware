//Dependency installer: https://github.com/domgho/InnoDependencyInstaller/blob/master/setup.iss

// comment out product defines to disable installing them
#define use_msi31
#define use_msi45
#define use_wic

#define use_dotnetfx47

// supported languages
#include "scripts\lang\english.iss"
#include "scripts\lang\german.iss"
#include "scripts\lang\french.iss"
#include "scripts\lang\italian.iss"
#include "scripts\lang\dutch.iss"

#ifdef UNICODE
#include "scripts\lang\chinese.iss"
#include "scripts\lang\polish.iss"
#include "scripts\lang\russian.iss"
#include "scripts\lang\japanese.iss"
#endif

// shared code for installing the products
#include "scripts\products.iss"

// helper functions
#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\dotnetfxversion.iss"

// actual products
#ifdef use_msi31
#include "scripts\products\msi31.iss"
#endif
#ifdef use_msi45
#include "scripts\products\msi45.iss"
#endif
#ifdef use_wic
#include "scripts\products\wic.iss"
#endif

#ifdef use_dotnetfx11
#include "scripts\products\dotnetfx11.iss"
#include "scripts\products\dotnetfx11sp1.iss"
#ifdef use_dotnetfx11lp
#include "scripts\products\dotnetfx11lp.iss"
#endif
#endif

#ifdef use_dotnetfx20
#include "scripts\products\dotnetfx20.iss"
#include "scripts\products\dotnetfx20sp1.iss"
#include "scripts\products\dotnetfx20sp2.iss"
#ifdef use_dotnetfx20lp
#include "scripts\products\dotnetfx20lp.iss"
#include "scripts\products\dotnetfx20sp1lp.iss"
#include "scripts\products\dotnetfx20sp2lp.iss"
#endif
#endif

#ifdef use_dotnetfx35
//#include "scripts\products\dotnetfx35.iss"
#include "scripts\products\dotnetfx35sp1.iss"
#ifdef use_dotnetfx35lp
//#include "scripts\products\dotnetfx35lp.iss"
#include "scripts\products\dotnetfx35sp1lp.iss"
#endif
#endif

#ifdef use_dotnetfx40
#include "scripts\products\dotnetfx40client.iss"
#include "scripts\products\dotnetfx40full.iss"
#endif

#ifdef use_dotnetfx45
#include "scripts\products\dotnetfx45.iss"
#endif
#ifdef use_dotnetfx46
#include "scripts\products\dotnetfx46.iss"
#endif
#ifdef use_dotnetfx47
#include "scripts\products\dotnetfx47.iss"
#endif
#ifdef use_dotnetfx48
#include "scripts\products\dotnetfx48.iss"
#endif

#ifdef use_netcorecheck
#include "scripts\products\netcorecheck.iss"
#endif
#ifdef use_netcore31
#include "scripts\products\netcore31.iss"
#endif
#ifdef use_netcore31asp
#include "scripts\products\netcore31asp.iss"
#endif
#ifdef use_netcore31desktop
#include "scripts\products\netcore31desktop.iss"
#endif
#ifdef use_dotnet50
#include "scripts\products\dotnet50.iss"
#endif
#ifdef use_dotnet50asp
#include "scripts\products\dotnet50asp.iss"
#endif
#ifdef use_dotnet50desktop
#include "scripts\products\dotnet50desktop.iss"
#endif

#ifdef use_msiproduct
#include "scripts\products\msiproduct.iss"
#endif
#ifdef use_vc2005
#include "scripts\products\vcredist2005.iss"
#endif
#ifdef use_vc2008
#include "scripts\products\vcredist2008.iss"
#endif
#ifdef use_vc2010
#include "scripts\products\vcredist2010.iss"
#endif
#ifdef use_vc2012
#include "scripts\products\vcredist2012.iss"
#endif
#ifdef use_vc2013
#include "scripts\products\vcredist2013.iss"
#endif
#ifdef use_vc2015
#include "scripts\products\vcredist2015.iss"
#endif
#ifdef use_vc2017
#include "scripts\products\vcredist2017.iss"
#endif
#ifdef use_vc2019
#include "scripts\products\vcredist2019.iss"
#endif

#ifdef use_directxruntime
#include "scripts\products\directxruntime.iss"
#endif

#ifdef use_sqlcompact35sp2
#include "scripts\products\sqlcompact35sp2.iss"
#endif
#ifdef use_sql2008express
#include "scripts\products\sql2008express.iss"
#endif

#define MyAppName "Sound Mixer Software"
#define MyAppVersion GetFileVersion("release/SoundMixerSoftware.exe")
#define MyAppPublisher "I Love u too"
#define MyAppURL "https://github.com/krystian20857/SoundMixerSoftware"
#define MyAppExeName "SoundMixerSoftware.exe"

#define ExeDir "release\"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{F3F46984-70BC-428B-AAC2-F8CFB4499407}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\SoundMixerSoftware\bin
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
;PrivilegesRequiredOverridesAllowed=dialog
OutputDir=out
OutputBaseFilename=SoundMixerSoftware {#MyAppVersion}
SetupIconFile=../SoundMixerSoftware/Resources/SpeakerIcon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
SourceDir=.
AppMutex=f3f46984-70bc-428b-aac2-f8cfb4499407

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files] 
Source: "{#ExeDir}*.exe"; DestDir: "{app}";  Flags: signonce
Source: "{#ExeDir}*.dll"; DestDir: "{app}";
Source: "{#ExeDir}*.config"; DestDir: "{app}";

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall

[CustomMessages]
DependenciesDir=MyProgramDependencies
WindowsServicePack=Windows %1 Service Pack %2

[Code]
function InitializeSetup(): Boolean;
begin
	// initialize windows version
	initwinversion();
#ifdef use_msi31
	msi31('3.1'); // install if version < 3.1
#endif
#ifdef use_msi45
	msi45('4.5'); // install if version < 4.5
#endif
#ifdef use_wic
	wic();
#endif
#ifdef use_dotnetfx11
	dotnetfx11();
#ifdef use_dotnetfx11lp
	dotnetfx11lp();
#endif
	dotnetfx11sp1();
#endif
	// install .netfx 2.0 sp2 if possible; if not sp1 if possible; if not .netfx 2.0
#ifdef use_dotnetfx20
	// check if .netfx 2.0 can be installed on this OS
	if not minwinspversion(5, 0, 3) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [FmtMessage(CustomMessage('WindowsServicePack'), ['2000', '3'])]), mbError, MB_OK);
		exit;
	end;
	if not minwinspversion(5, 1, 2) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [FmtMessage(CustomMessage('WindowsServicePack'), ['XP', '2'])]), mbError, MB_OK);
		exit;
	end;
	if minwinversion(5, 1) then begin
		dotnetfx20sp2();
#ifdef use_dotnetfx20lp
		dotnetfx20sp2lp();
#endif
	end else begin
		if minwinversion(5, 0) and minwinspversion(5, 0, 4) then begin
			dotnetfx20sp1();
#ifdef use_dotnetfx20lp
			dotnetfx20sp1lp();
#endif
		end else begin
			dotnetfx20();
#ifdef use_dotnetfx20lp
			dotnetfx20lp();
#endif
		end;
	end;
#endif
#ifdef use_dotnetfx35
	//dotnetfx35();
	dotnetfx35sp1();
#ifdef use_dotnetfx35lp
	//dotnetfx35lp();
	dotnetfx35sp1lp();
#endif
#endif
	// if no .netfx 4.0 is found, install the client (smallest)
#ifdef use_dotnetfx40
	if (not dotnetfxinstalled(NetFx40Client, '') and not dotnetfxinstalled(NetFx40Full, '')) then
		dotnetfx40client();
#endif
#ifdef use_dotnetfx45
	dotnetfx45(50); // install if version < 4.5.0
#endif
#ifdef use_dotnetfx46
	dotnetfx46(60); // install if version < 4.6.0
#endif
#ifdef use_dotnetfx47
	dotnetfx47(70); // install if version < 4.7.0
#endif
#ifdef use_dotnetfx48
	dotnetfx48(80); // install if version < 4.8.0
#endif
#ifdef use_netcore31
	netcore31();
#endif
#ifdef use_netcore31asp
	netcore31asp();
#endif
#ifdef use_netcore31desktop
	netcore31desktop();
#endif
#ifdef use_dotnet50
	dotnet50();
#endif
#ifdef use_dotnet50asp
	dotnet50asp();
#endif
#ifdef use_dotnet50desktop
	dotnet50desktop();
#endif
#ifdef use_vc2005
	vcredist2005('6'); // install if version < 6.0
#endif
#ifdef use_vc2008
	vcredist2008('9'); // install if version < 9.0
#endif
#ifdef use_vc2010
	vcredist2010('10'); // install if version < 10.0
#endif
#ifdef use_vc2012
	vcredist2012('11'); // install if version < 11.0
#endif
#ifdef use_vc2013
	//SetForceX86(true); // force 32-bit install of next products
	vcredist2013('12'); // install if version < 12.0
	//SetForceX86(false); // disable forced 32-bit install again
#endif
#ifdef use_vc2015
	vcredist2015('14'); // install if version < 14.0
#endif
#ifdef use_vc2017
	vcredist2017('14.10'); // install if version < 14.10
#endif
#ifdef use_vc2019
	vcredist2019('14.20'); // install if version < 14.20
#endif
#ifdef use_directxruntime
	directxruntime();
#endif
#ifdef use_sqlcompact35sp2
	sqlcompact35sp2();
#endif
#ifdef use_sql2008express
	sql2008express();
#endif
	Result := true;
end;