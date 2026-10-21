; ! ==========================================================
; ! Ball Scanner - x86 installer | UTF-8 with BOM REQUIRED !!!
; ! ==========================================================
; ! --- 0. Генерация установщика в Unicode и High DPI support
Unicode true
ManifestDPIAware true

; ! --- 1. Задаём алгоритм и степень сжатия
SetCompressor /SOLID lzma
SetCompressorDictSize 64

; ! --- 2. Директивы препроцессора NSIS (аналог #define cpp)
!define DOTNET_EXE "ndp48-web.exe"
!define DOTNET_SRC ".\external\ndp48-web.exe"

!define APP_NAME      "Ball Scanner (32-bit)"
!define APP_FOLDER    "Ball Scanner"
!define APP_VERSION   "1.1.0"
!define APP_PUBLISHER "aLeeTheY"
!define APP_URL       "https://github.com/aLeeTheY/BallScanner-CSharp"
!define APP_EXE       "Ball Scanner.exe"
!define APP_SOURCE    "..\BallScanner\bin\x86\Release"

; ! --- 3. Место генерации установщика и путь установки программы
OutFile "output\BallScanner_${APP_VERSION}_windows_x86_setup.exe"
InstallDir "$PROGRAMFILES\${APP_FOLDER}"

; ! --- 4. Заголовок окна установщика и запрашиваемые права на доступа при установке (UAC)
Name "${APP_NAME} ${APP_VERSION}"
RequestExecutionLevel admin

; ! --- 5. Настройки UI установщика (Modern UI)
; ! --- 5.1. ВЫбор иконки
!define MUI_ICON   "..\BallScanner\favicon.ico"
!define MUI_UNICON "..\BallScanner\favicon.ico"

; ! --- 5.2. Подключение заголовочных файлов
!include "MUI2.nsh"

; ! --- 5.3. Порядок страниц установщика и их настройки
!insertmacro MUI_PAGE_WELCOME

!define MUI_LICENSEPAGE_CHECKBOX
!insertmacro MUI_PAGE_LICENSE "../LICENSE"

!define MUI_COMPONENTSPAGE_NODESC
!insertmacro MUI_PAGE_COMPONENTS

!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${APP_EXE}"
!define MUI_FINISHPAGE_RUN_TEXT "$(FINISH_PAGE_RUN)"
!define MUI_FINISHPAGE_SHOWREADME_TEXT "$(FINISH_PAGE_SHOWREADME)"
!define MUI_FINISHPAGE_SHOWREADME "https://github.com/aLeeTheY/BallScanner-CSharp"
!insertmacro MUI_PAGE_FINISH

; ! --- 5.4. Порядок страниц uninstaller и их настройки
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; ! --- 6. Настройки переводов (i18n)
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "Russian"

; ! --- 6.1. Локализация кнопочек на странице MUI_PAGE_COMPONENTS
LangString SEC_DESKTOP_NAME   ${LANG_ENGLISH} "Create a desktop shortcut"
LangString SEC_DESKTOP_NAME   ${LANG_RUSSIAN} "Создать ярлык на рабочем столе"

LangString SEC_STARTMENU_NAME ${LANG_ENGLISH} "Create a Start Menu shortcut"
LangString SEC_STARTMENU_NAME ${LANG_RUSSIAN} "Создать ярлык в меню Пуск"

; ! --- 6.2. Локализация кнопочек на странице MUI_PAGE_FINISH
LangString FINISH_PAGE_RUN ${LANG_ENGLISH} "Run Ball Scanner"
LangString FINISH_PAGE_RUN ${LANG_RUSSIAN} "Запустить Ball Scanner"
LangString FINISH_PAGE_SHOWREADME ${LANG_ENGLISH} "Open project on GitHub"
LangString FINISH_PAGE_SHOWREADME ${LANG_RUSSIAN} "Открыть проект на GitHub"

; ! --- 6.3. Текст на страничке MUI_LANGDLL_DISPLAY
!define MUI_LANGDLL_WINDOWTITLE "${APP_NAME} ${APP_VERSION} Setup"
!define MUI_LANGDLL_INFO "Please select the installer language"
!define MUI_LANGDLL_ALWAYSSHOW

; ! --- 7. Метаданные файла установщика в файловой системе
VIProductVersion "1.1.0.0"
VIAddVersionKey "ProductName"     "Ball Scanner (32-bit)"
VIAddVersionKey "FileDescription" "Ball Scanner (32-bit) Installer"
VIAddVersionKey "FileVersion"     "1.1.0.0"
VIAddVersionKey "ProductVersion"  "1.1.0.0"
VIAddVersionKey "CompanyName"     "aLeeTheY"
VIAddVersionKey "LegalCopyright"  "Copyright (C) 2021-2026 aLeeTheY"
VIAddVersionKey "OriginalFilename" "BallScanner_${APP_VERSION}_windows_x86_setup.exe"

; ! --- 8. Cекция со скриптом установки | Исполняется когда винда страничка MUI_PAGE_INSTFILES | REQUIRED
Section "-Install"
    ; * --- 8.1. Текущая секция READ-ONLY | От неё нельзя отказаться в MUI_PAGE_COMPONENTS
    SectionIn RO

    ; * --- 8.2. Переходим в папку, куда ставим прогу
    SetOutPath "$INSTDIR"

    ; * --- 8.3. Кладём туда все файлы программы
    File /r "${APP_SOURCE}\*.*"

    ; * --- 8.4. Выдаём права на Data через icacls
    nsExec::ExecToLog 'icacls "$INSTDIR\Data" /grant *S-1-5-32-545:(OI)(CI)F /T /C /Q'
    Pop $0
    ${If} $0 != 0
        DetailPrint "Failed to grant permissions on $INSTDIR\Data. Exit code: $0"
    ${EndIf}

    ; * --- 8.5. Создаём папку Logs и выдаём на неё права
    CreateDirectory "$INSTDIR\Logs"
    nsExec::ExecToLog 'icacls "$INSTDIR\Logs" /grant *S-1-5-32-545:(OI)(CI)F /T /C /Q'
    Pop $0
    ${If} $0 != 0
        DetailPrint "Failed to grant permissions on $INSTDIR\Logs. Exit code: $0"
    ${EndIf}

    ; * --- 8.6. Создаем uninstaller.exe файл
    WriteUninstaller "$INSTDIR\Uninstall ${APP_NAME}.exe"

    ; * --- 8.7. Создаем папку в Start Menu folder и создаем ярлык для Start Menu для uninstaller
    CreateDirectory "$SMPROGRAMS\${APP_FOLDER}"
    CreateShortcut "$SMPROGRAMS\${APP_FOLDER}\Uninstall ${APP_NAME}.lnk" "$INSTDIR\Uninstall ${APP_NAME}.exe"

    ; * --- 8.8. Запись данных о программе в реестр Windows
    WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}" "DisplayName"     "${APP_NAME}"
    WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}" "DisplayVersion"  "${APP_VERSION}"
    WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}" "Publisher"       "${APP_PUBLISHER}"
    WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}" "URLInfoAbout"    "${APP_URL}"
    WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}" "UninstallString" "$\"$INSTDIR\Uninstall ${APP_NAME}.exe$\""
    WriteRegStr   HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}" "DisplayIcon"     "$INSTDIR\${APP_EXE}"
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}" "EstimatedSize"   20324
SectionEnd

; ! --- 9. Секция .NET Framework 4.8 | Обязательная | Пропускается, если .NET уже стоит
Section ".NET Framework 4.8" SecDotNet
    SetOutPath "$PLUGINSDIR"
    File /oname=${DOTNET_EXE} "${DOTNET_SRC}"

    ; * Если основной установщик запущен с /S — .NET Framework тоже ставим тихо
    ; * Иначе — показываем GUI-визард установки .NET Framework
    ${If} ${Silent}
        ExecWait '"$PLUGINSDIR\${DOTNET_EXE}" /q /norestart' $0
    ${Else}
        ExecWait '"$PLUGINSDIR\${DOTNET_EXE}" /norestart' $0
    ${EndIf}

    ; * Если установщик вернул 3010 — значит нужен ребут
    ${If} $0 == 3010
        SetRebootFlag true
    ${EndIf}
SectionEnd

; ! --- 10. Секция со скриптом создания ярлыка на Рабочем столе | Отображается как галочка на странице MUI_PAGE_COMPONENTS | OPTIONAL
Section "$(SEC_DESKTOP_NAME)" SecDesktop
    CreateShortcut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
SectionEnd

; ! --- 11. Секция со скриптом создания ярлыка в меню Пуск | Отображается как галочка на странице MUI_PAGE_COMPONENTS | OPTIONAL
Section "$(SEC_STARTMENU_NAME)" SecStartMenu
    CreateShortcut "$SMPROGRAMS\${APP_FOLDER}\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
SectionEnd

; ! --- 12. Cекция со скриптом удаления | Исполняется когда винда страничка MUI_UNPAGE_INSTFILES | REQUIRED
Section "Uninstall"
    ; * --- 12.1. Удаляем все файлы программы и саму папку программы
    RMDir /r "$INSTDIR"

    ; * --- 12.2. Чистим за собой ярлыки из Start Menu
    Delete "$SMPROGRAMS\${APP_FOLDER}\${APP_NAME}.lnk"
    Delete "$SMPROGRAMS\${APP_FOLDER}\Uninstall ${APP_NAME}.lnk"
    RMDir "$SMPROGRAMS\${APP_FOLDER}"

    ; * --- 12.3. Чистим за собой ярлык из Рабочего стола
    Delete "$DESKTOP\${APP_NAME}.lnk"

    ; * --- 12.4. Чистим за собой реестр Windows
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_FOLDER}"
SectionEnd

; ! --- 13. Проверка что приложение не запущено
!macro CheckAppNotRunning
    check_loop:
        System::Call 'kernel32::OpenMutex(i 0x100000, b 0, t "Global\BallScannerApp") p .R0'
        ${If} $R0 != 0
            System::Call 'kernel32::CloseHandle(p R0)'
            ${If} ${Silent}
                Abort
            ${Else}
                MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION|MB_TOPMOST \
                    "Ball Scanner is currently running. Please close the application and click Retry." \
                    IDRETRY check_loop
                Abort
            ${EndIf}
        ${EndIf}
!macroend

!macro un.CheckAppNotRunning
    un_check_loop:
        System::Call 'kernel32::OpenMutex(i 0x100000, b 0, t "Global\BallScannerApp") p .R0'
        ${If} $R0 != 0
            System::Call 'kernel32::CloseHandle(p R0)'
            ${If} ${Silent}
                Abort
            ${Else}
                MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION|MB_TOPMOST \
                    "Ball Scanner is currently running. Please close the application and click Retry." \
                    IDRETRY un_check_loop
                Abort
            ${EndIf}
        ${EndIf}
!macroend

; ! --- 14. Вызов окна выбора языка при состоянии .onInit | Callback Function до вызова основного окна installer | REQUIRED
Function .onInit
    ; * --- 14.1. Проверка: Ball Scanner не запущен
    !insertmacro CheckAppNotRunning

    ; * --- 14.2. Режим работы установщиека в 32-bit режиме | Для всего installer
    SetRegView 32

    ; * --- 14.3. Вызов странички выбора языка
    !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

; ! --- 15. Вызов окна выбора языка при состоянии un.onInit | Callback Function до вызова основного окна uninstaller | REQUIRED
Function un.onInit
    ; * --- 15.1. Проверка: Ball Scanner не запущен
    !insertmacro un.CheckAppNotRunning

    ; * --- 15.2. Режим работы установщиека в 32-bit режиме | Для всего uninstaller
    SetRegView 32

    ; * --- 15.3. Вызов странички выбора языка
    !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd
