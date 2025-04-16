@echo off
setlocal enabledelayedexpansion

REM === CONFIGURATION ===
set "XSD_DIR=ComunicationApiXmlDto/xmlSchema"
set "OUT_DIR=ComunicationApiXmlDto/GeneratedClasses"
set "NAMESPACE=ComunicationApiXmlDto"

REM === Ensure output folder exists ===
if not exist "%OUT_DIR%" mkdir "%OUT_DIR%"

REM === Loop through all .xsd files ===
for %%F in (%XSD_DIR%\*.xsd) do (
    set "FILENAME=%%~nF"
    echo Generating class from: %%F : %%~nF with namespace %NAMESPACE%

    REM Generate .cs from .xsd using specific root element name (same as file name)
    xsd.exe %%F /c /e:%%~nF /n:ComunicationApiXmlDto /outputdir:%OUT_DIR% >nul

    REM Optional: Rename to match XSD name
    if exist "%OUT_DIR%\%%~nF.cs" (
        echo ✓ Generated: %OUT_DIR%\%%~nF.cs
    ) else (
        echo Output file not named as expected. Check: %OUT_DIR%
    )
)

echo All classes generated!
