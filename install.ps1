# 1. Definir la ruta de instalación local (ej. C:\Users\Nombre\AppData\Local\UserDB)
$installDir = "$env:LOCALAPPDATA\UserDB"
$exePath = "$installDir\userdb.exe"

# 2. Crear el directorio si no existe
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir | Out-Null
}

# 3. Descargar el ejecutable desde la Release de GitHub
$downloadUrl = "https://github.com/tu_usuario/tu_repo/releases/latest/download/userdb-win-x64.exe"
Write-Host "Descargando UserDB..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $downloadUrl -OutFile $exePath

# 4. Agregar la carpeta al PATH del usuario (para poder usar 'userdb' desde cualquier terminal)
$userPath = [Environment]::GetEnvironmentVariable("Path", "User")
if ($userPath -notlike "*$installDir*") {
    [Environment]::SetEnvironmentVariable("Path", "$userPath;$installDir", "User")
    Write-Host "Se agregó UserDB al PATH del sistema." -ForegroundColor Green
}

Write-Host "¡Instalación completada! Abre una nueva terminal y escribe 'userdb'." -ForegroundColor Green