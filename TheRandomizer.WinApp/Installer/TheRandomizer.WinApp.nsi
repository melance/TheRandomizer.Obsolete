# Constants
!define APP_NAME "The Randomizer"
!define COMP_NAME "Lance Boudreaux"
!define WEB_SITE "https://github.com/melance/TheRandomizer"
!define VERSION "3.00.20.00"
!define COPYRIGHT "Lance Boudreaux © 2015-2017"
!define DESCRIPTION "The infinitely customizable random content generator"
!define INSTALLER_NAME "TheRandomizerWinAppSetup.exe"
!define INSTALL_TYPE "SetShellVarContext current"
!define REG_ROOT "HKCU"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Unintall\${APP_NAME}"
!define MAIN_APP_EXE "TheRandomizer.exe"
!define LICENSE_TXT "License.rtf"
!define INSTALLER_ICON "TheRandomizer.ico"
!define WELCOME_IMAGE "Welcome2.bmp"

!define REG_START_MENU "Start Menu Folder"

var START_MENU_FOLDER

# Version Info
VIProductVersion "${VERSION}"
VIAddVersionKey "ProductName" "${APP_NAME}"
VIAddVersionKey "CompanyName" "${COMP_NAME}"
VIAddVersionKey "LegalCopyright" "${COPYRIGHT}"
VIAddVersionKey "FileDescription" "${DESCRIPTION}"
VIAddVersionKey "FileVersion" "${VERSION}"
VIAddVersionKey "ProductVersion" "${VERSION}"

# Settings
SetCompressor ZLIB
XPStyle on
Name "${APP_NAME}"
Caption "${APP_NAME}"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "$PROGRAMFILES\TheRandomizer"

# Includes
!include "MUI.nsh"

# Modern UI
!ifdef INSTALLER_ICON
	!define MUI_ICON "${INSTALLER_ICON}"
	!define MUI_UNICON "${INSTALLER_ICON}"
!endif
!ifdef WELCOME_IMAGE
	!define MUI_WELCOMEFINISHPAGE_BITMAP "${WELCOME_IMAGE}"
	!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${WELCOME_IMAGE}"
!endif
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING

!insertmacro MUI_PAGE_WELCOME

!ifdef LICENSE_TXT
!insertmacro MUI_PAGE_LICENSE "${LICENSE_TXT}"
!endif

!insertmacro MUI_PAGE_DIRECTORY

!ifdef REG_START_MENU
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "The Randomizer"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${REG_ROOT}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${UNINSTALL_PATH}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${REG_START_MENU}"
!insertmacro MUI_PAGE_STARTMENU Application $START_MENU_FOLDER
!endif

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${MAIN_APP_EXE}"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"

# Main Program Section
Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer
SetOutPath "$INSTDIR"
File "..\bin\release\*.dll"
File "..\bin\release\*.exe"
File "..\bin\release\*.config"
SetOutPath "$INSTDIR\Accents"
File "..\bin\release\Accents\*.xaml"
SectionEnd

# Registry and Start Menu Section
Section -Icons_Reg
SetOutPath "$INSTDIR"
WriteUninstaller "$INSTDIR\Unintall.exe"

!ifdef	 REG_START_MENU
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateDirectory "$SMPROGRAMS\$START_MENU_FOLDER"
CreateShortCut "$SMPROGRAMS\$START_MENU_FOLDER\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\$START_MENU_FOLDER\${APP_NAME} Editor.lnk" "$INSTDIR\${MAIN_APP_EXE} -m Editor"
CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$DESKTOP\${APP_NAME} Editor.lnk" "$INSTDIR\${MAIN_APP_EXE}" "-m Editor"
CreateShortCut "$SMPROGRAMS\$START_MENU_FOLDER\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

!ifdef WEB_SITE
WriteINIStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\$START_MENU_FOLDER\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif	
!insertmacro MUI_STARTMENU_WRITE_END
!endif	

WriteRegStr ${REG_ROOT} "${REG_APP_PATH}" "" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayName" "${APP_NAME}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "UninstallString" "$INSTDIR\uninstall.exe"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayVersion" "${VERSION}"

!ifdef WEB_SITE
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "URLInfoAbout" "${WEB_SITE}"
!endif	
SectionEnd

# Uninstall Section
Section Uninstall
${INSTALL_TYPE}
Delete "$INSTDIR\*.*"
RMDir "$INSTDIR"

!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_GETFOLDER "Application" $START_MENU_FOLDER
Delete "$SMPROGRAMS\$START_MENU_FOLDER\*.*"
RMDir "$SMPROGRAMS\$START_MENU_FOLDER"
!endif

DeleteRegKey ${REG_ROOT} "{${REG_APP_PATH}"
DeleteRegKey ${REG_ROOT} "${UNINSTALL_PATH}"
SectionEnd