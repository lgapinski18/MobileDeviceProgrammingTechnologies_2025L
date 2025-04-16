@echo off
setlocal enabledelayedexpansion

REM === CONFIGURATION ===
set "SRC_DIR=ComunicationApiXmlDto"
set "DLL_DIR=ComunicationApiXmlDto/bin"
set "SCHEMA_DIR=ComunicationApiXmlDto/xmlSchema"
set "NAMESPACE=ComunicationApiXmlDto"

REM === CLASS LIST ===
set CLASSES=AccountOwnerCreationData AccountOwnerDto BankAccountDto BankAccountReportDto CreationAccountOwnerResponse Credentials TransferData TransferResultCodes

REM === Ensure output folders exist ===
if not exist "%DLL_DIR%" mkdir "%DLL_DIR%"
if not exist "%SCHEMA_DIR%" mkdir "%SCHEMA_DIR%"

REM === Compile all .cs files into one DLL ===
echo Compiling all source files...
csc.exe /t:library /out:%DLL_DIR%\AllTypes.dll %SRC_DIR%\*.cs
if errorlevel 1 (
    echo ❌ Compilation failed!
    exit /b 1
)
echo ✅ Compilation successful!

REM === Generate schema for each class ===
for %%C in (%CLASSES%) do (
    echo Processing %%C...

    xsd.exe %DLL_DIR%\AllTypes.dll /type:%NAMESPACE%.%%C /outputdir:%SCHEMA_DIR% >nul
    if not exist "%SCHEMA_DIR%\schema0.xsd" (
        echo ⚠ xsd.exe did not generate schema0.xsd for %%C
    ) else (
        move /Y "%SCHEMA_DIR%\schema0.xsd" "%SCHEMA_DIR%\%%C.xsd" >nul
        echo ✓ Generated schema: %SCHEMA_DIR%\%%C.xsd
    )
)

echo 🏁 Done!
endlocal
