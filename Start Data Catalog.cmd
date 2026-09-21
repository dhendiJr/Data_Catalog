@echo off
title Data Catalog
cd /d "%~dp0"

rem The catalog file lives next to this start file, where it can be seen and copied.
rem Copying that one file is how the catalog gets backed up.
set "DATACATALOG_FILE=%~dp0datacatalog.db"

echo Starting Data Catalog. This can take a moment the first time.
echo.

dotnet run --project "src\DataCatalog.Web" --configuration Release

if errorlevel 1 (
    echo.
    echo This window is staying open so you can read what happened above.
    pause
)
