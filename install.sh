#!/usr/bin/env bash

# Script de instalación automática para UserDatabase (Linux)
# Descarga el binario ejecutable directamente desde las Releases de GitHub

set -e

REPO="CMDPlayer216/UserDatabase"
INSTALL_DIR="$HOME/.local/bin"
BINARY_NAME="userdb"

# Colores para la terminal
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # Sin color

echo -e "${CYAN}=== Instalador de UserDatabase (Linux) ===${NC}\n"

# 1. Verificar que sea Linux
OS="$(uname -s | tr '[:upper:]' '[:lower:]')"

if [ "$OS" != "linux" ]; then
    echo -e "${RED}Error: De momento este script solo soporta Linux.${NC}"
    echo -e "Para Windows, utiliza el instalador install.ps1 en PowerShell."
    exit 1
fi

ASSET_NAME="userdb-linux-x64"

# 2. URL de descarga directa desde la última Release
DOWNLOAD_URL="https://github.com/${REPO}/releases/latest/download/${ASSET_NAME}"

# 3. Crear directorio de instalación si no existe
if [ ! -d "$INSTALL_DIR" ]; then
    echo -e "${YELLOW}Creando directorio de instalación: $INSTALL_DIR${NC}"
    mkdir -p "$INSTALL_DIR"
fi

TARGET_PATH="$INSTALL_DIR/$BINARY_NAME"

# 4. Descargar el binario directamente usando curl o wget
echo -e "${CYAN}Descargando ejecutable desde GitHub Releases...${NC}"
if command -v curl >/dev/null 2>&1; then
    curl -sSL -o "$TARGET_PATH" "$DOWNLOAD_URL"
elif command -v wget >/dev/null 2>&1; then
    wget -q -O "$TARGET_PATH" "$DOWNLOAD_URL"
else
    echo -e "${RED}Error: Se requiere 'curl' o 'wget' para descargar.${NC}"
    exit 1
fi

# 5. Otorgar permisos de ejecución al ejecutable
chmod +x "$TARGET_PATH"
echo -e "${GREEN}✓ Binario descargado y configurado en: $TARGET_PATH${NC}"

# 6. Verificar y agregar ~/.local/bin al PATH si es necesario
if [[ ":$PATH:" != *":$INSTALL_DIR:"* ]]; then
    SHELL_CONFIG=""
    if [ -f "$HOME/.bashrc" ]; then
        SHELL_CONFIG="$HOME/.bashrc"
    elif [ -f "$HOME/.zshrc" ]; then
        SHELL_CONFIG="$HOME/.zshrc"
    fi

    if [ -n "$SHELL_CONFIG" ]; then
        if ! grep -q "$INSTALL_DIR" "$SHELL_CONFIG"; then
            echo "export PATH=\"\$HOME/.local/bin:\$PATH\"" >> "$SHELL_CONFIG"
            echo -e "${GREEN}✓ Se agregó $INSTALL_DIR al PATH en $SHELL_CONFIG${NC}"
            echo -e "Ejecuta ${CYAN}source $SHELL_CONFIG${NC} o reinicia la terminal."
        fi
    fi
else
    echo -e "${GREEN}✓ '$INSTALL_DIR' ya está en tu PATH.${NC}"
fi

echo -e "\n${GREEN}¡Instalación completada!${NC}"
echo -e "Escribe '${CYAN}userdb${NC}' para ejecutar la aplicación.\n"