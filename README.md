# UserDB v3.0

**UserDB** es una herramienta de línea de comandos (CLI) desarrollada en C# / .NET para la gestión ligera, rápida y estructurada de perfiles de usuario, roles, fandoms, pronombres y seguimiento de rachas (*streaks*) diarias.

Funciona tanto de forma interactiva por menús como a través de comandos CLI directos. La información se almacena de forma local mediante archivos JSON individuales por usuario y un índice central `.dat`, ofreciendo una interfaz visual en consola con colores y tablas multilínea.

---

## 📸 Screenshots

| Menú Principal | Vista de Tabla de Usuarios |
| :---: | :---: |
| ![Menú principal](screenshots/main-menu.png) | ![Tabla de usuarios](screenshots/users-table.png) |

| Registro de Usuario | Verificación de Racha |
| :---: | :---: |
| ![Agregando un usuario](screenshots/add-an-user.png) | ![Verificación de racha](screenshots/streak-verification.png) |

---

## 🚀 Características Principales

- **Modo CLI Directo y Modo Interactivo**: Puedes ejecutar el programa sin argumentos para abrir el menú interactivo o usar subcomandos (`list`, `add`, `modify`, `remove`, `show`) para automatización o scripts.
- **Visualización en Tabla o Texto**: Renderizado dinámico en consola formateado con columnas adaptativas para arreglos multilínea (roles, pronombres, etc.) o volcado en formato raw.
- **Gestión Completa de Perfiles**:
  - Campos soportados: Nombre, ID, Fandom, Edad, Pronombres, Roles Buscados, Roles Adicionales, Status, Fecha de Registro y Racha (*Streak*).
  - Manejo inteligente de arreglos (añadir/eliminar elementos de forma individual o mediante valores separados por comas).
- **Sistema de Rachas (Streaks)**: Módulo interactivo para verificar la actividad diaria de un usuario e incrementar o reiniciar su contador.
- **Persistencia JSON Local**: Cada perfil se guarda de forma independiente en un archivo `.json` formateado dentro de `~/.userdb/`, con generación de IDs vía NanoID y control para evitar duplicados.
- **Indexación Rápida**: Mantenimiento automático de un archivo `users.dat` que actúa como índice central.
- **Multiplataforma**: Compatible con Linux (`linux-x64`) y Windows (`win-x64`).

---

## 🛠️ Tecnologías Utilizadas

- **Lenguaje**: C# (.NET 8.0+)
- **Librerías**:
  - `System.CommandLine` para la interfaz de comandos CLI.
  - `NanoidDotNet` para la generación de identificadores únicos (NanoID).
  - `System.Text.Json` para serialización y desearialización de datos.
- **Compilación**: Ejecutables nativos aislados (*self-contained*) para `linux-x64` y `win-x64`.

---

## 📂 Estructura de Datos y Almacenamiento

El programa crea automáticamente el directorio de datos en la carpeta personal del usuario (`~/.userdb/` en Linux / `%USERPROFILE%\.userdb\` en Windows).

```text
~/.userdb/
├── users.dat              # Índice general (formato: Nombre,RutaArchivoJSON)
├── V1StG2pL9qRt.json      # Perfil individual guardado por su userId (NanoID)
└── ...
```

### Ejemplo de esquema JSON (`<userId>.json`)

```json
{
  "name": "Gabriel",
  "userId": "V1StG2pL9qRt",
  "additionalRoles": [
    "Dev",
    "Admin"
  ],
  "age": 18,
  "fandom": "Cyberpunk",
  "lookedCharacters": [
    "V",
    "Johnny Silverhand"
  ],
  "pronouns": [
    "he/him"
  ],
  "dateRegistered": "2026-07-23",
  "streak": 5,
  "status": "Activo"
}
```

---

## 🔨 Compilación e Instalación

### Prerrequisitos

Tener instalado el SDK de .NET 8.0+:

```bash
dotnet --version
```

### Exportar Binarios (Linux)

Puedes ejecutar el script unificado de exportación para compilar ambas plataformas de forma aislada:

```bash
chmod +x export.sh
./export.sh
```

Esto generará los ejecutables comprimidos en `./publish/linux-x64/` y `./publish/windows-x64/` además de copiarlos al escritorio.

### Instalación Rápida

- **En Linux:**
  ```bash
  chmod +x install.sh
  ./install.sh
  ```
  *(Descarga o mueve el binario a `~/.local/bin/userdb` y lo agrega a tu PATH)*

- **En Windows (PowerShell):**
  ```powershell
  .\install.ps1
  ```
  *(Instala el ejecutable en `%LOCALAPPDATA%\UserDB` y configura las variables de entorno)*

---

## 📖 Modo de Uso

### 1. Modo Interactivo

Si ejecutas el comando sin argumentos, entrarás al menú guiado en consola:

```bash
userdb
```

**Opciones disponibles:**
1. **Mostrar usuarios**: Despliega la tabla ASCII interactiva con todos los usuarios registrados.
2. **Agregar un usuario**: Formulario interactivo paso a paso con validaciones.
3. **Verificar un usuario**: Revisa y actualiza la racha diaria de un usuario seleccionado.
4. **Modificar un usuario**: Menú dinámico para editar campos personales y gestionar listas (añadir o eliminar roles/pronombres).
5. **Eliminar usuario**: Selecciona y borra un perfil previa confirmación.
6. **Salir**: Cierra la aplicación.

---

### 2. Modo de Línea de Comandos (CLI)

UserDB permite realizar todas las operaciones directamente mediante flags y subcomandos.

#### 🔹 Listar Usuarios (`list`)
```bash
# Mostrar usuarios formateados en tabla ASCII
userdb list --table

# Mostrar únicamente el archivo de índice plano
userdb list --raw
```

#### 🔹 Agregar Usuario (`add`)
```bash
userdb add -n "Gabriel" -f "Cyberpunk" -A 18 -p "he/him" -a "Dev,Admin" -l "V,Johnny" -s 3 -S "Activo"
```
*Parámetros:*
- `-n, --name` (Requerido): Nombre del usuario.
- `-f, --fandom` (Requerido): Fandom al que pertenece.
- `-A, --age` (Requerido): Edad del usuario.
- `-p, --pronouns` (Requerido): Pronombres (separados por comas si son varios).
- `-a, --additional-roles`: Roles adicionales (separados por comas).
- `-l, --looked-characters`: Personajes buscados (separados por comas).
- `-s, --streak`: Racha inicial (por defecto `0`).
- `-u, --user-id`: ID personalizado (por defecto se genera un NanoID de 12 caracteres).
- `-S, --status`: Estado del usuario (por defecto `"Activo"`).

#### 🔹 Modificar Usuario (`modify`)
```bash
# Cambiar nombre y sumar un rol adicional a un usuario existente
userdb modify -u "V1StG2pL9qRt" -n "Gabriel (Editado)" -a "Moderador"

# Eliminar un personaje buscado y actualizar el status
userdb modify -u "V1StG2pL9qRt" -L "V" -S "Inactivo"
```
*Parámetros principales:*
- `-u, --source-user` (Requerido): ID del usuario a modificar.
- `-n, --name`: Nuevo nombre.
- `-f, --fandom`: Nuevo fandom.
- `-A, --age`: Nueva edad.
- `-s, --streak`: Nueva racha.
- `-U, --user-id`: Nuevo ID para reasignar al usuario.
- `-S, --status`: Nuevo status.
- `-a, --add-additional-roles` / `-r, --remove-additional-roles`: Añadir / eliminar roles adicionales.
- `-l, --add-looked-characters` / `-L, --remove-looked-characters`: Añadir / eliminar personajes buscados.
- `-p, --add-pronouns` / `-P, --remove-pronouns`: Añadir / eliminar pronombres.

#### 🔹 Ver Detalle de Usuario (`show`)
```bash
# Ver información detallada en consola
userdb show -u "V1StG2pL9qRt"

# Ver el contenido directamente en formato JSON
userdb show -u "V1StG2pL9qRt" --raw
```

#### 🔹 Eliminar Usuario (`remove`)
```bash
# Eliminar pidiendo confirmación
userdb remove -u "V1StG2pL9qRt"

# Eliminar sin pedir confirmación
userdb remove -u "V1StG2pL9qRt" --noconfirm
```

---

## 📜 Licencia y Créditos

Desarrollado por **CMDPlayer216** (2026).
