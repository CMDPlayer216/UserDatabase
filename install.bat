@echo off
setlocal

if not exist ".\bin\release\userdb.exe" (
    echo [ERROR] No se encontro el archivo userdb.exe en .\bin\release\
    echo Por favor asegúrate de compilar el proyecto antes de instalar.
    echo.
    pause
    exit /b 1
)

set "INSTALL_DIR=%LOCALAPPDATA%\userdb"

echo [1/3] Creando directorio de instalacion en: %INSTALL_DIR%
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

echo [2/3] Copiando userdb.exe...
copy /Y ".\bin\release\userdb.exe" "%INSTALL_DIR%\userdb.exe" >nul
if errorlevel 1 (
    echo [ERROR] No se pudo copiar el ejecutable.
    pause
    exit /b 1
)

echo [3/3] Agregando la ruta al PATH del usuario...
echo %PATH% | findstr /I /C:"%INSTALL_DIR%" >nul
if errorlevel 1 (
    setx PATH "%PATH%;%INSTALL_DIR%" >nul
    echo Ruta agregada al PATH correctamente.
) else (
    echo La ruta ya estaba presente en el PATH.
)

echo.
echo ===================================================
echo ¡Instalacion completada con exito!
echo Puedes ejecutar 'userdb' desde cualquier terminal.
echo (Nota: Reinicia la consola para que tome el nuevo PATH).
echo ===================================================
echo.
pause