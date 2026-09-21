# UserDB v3.3.1

**UserDB** es una herramienta de gestión de perfiles de usuario ligera, rápida y estructurada para la terminal, desarrollada en C# y .NET. Permite administrar información personal, roles, fandoms, pronombres, seguimiento de rachas (*streaks*) diarias, búsquedas avanzadas, auditoría continua y operaciones de importación/exportación de bases de datos completas.

Soporta dos modalidades de uso: **modo interactivo** con menús guiados y **modo CLI** con flags para integración y automatización en scripts.

| Menú Principal | Vista de Tabla de Usuarios |
| :---: | :---: |
| ![Menú principal](screenshots/main-menu.png) | ![Tabla de usuarios](screenshots/users-table.png) |

| Registro de Usuario | Verificación de Racha |
| :---: | :---: |
| ![Agregando un usuario](screenshots/add-an-user.png) | ![Verificación de racha](screenshots/streak-verification.png) |

---

## 1. Descripción del Proyecto

UserDB está diseñado para resolver la administración ágil de perfiles comunitarios o de rol, manteniendo persistencia local, seguridad de concurrencia y portabilidad total:

- **Almacenamiento Local y Desacoplado**: Cada usuario se almacena en un archivo JSON independiente (`<userId>.json`) dentro de `~/.userdb/` sincronizado con un archivo índice rápido (`users.dat`).
- **Control de Concurrencia**: Sistema de bloqueo de base de datos (`userdb.lock`) que previene inconsistencias y condiciones de carrera cuando múltiples instancias o comandos intentan acceder simultáneamente.
- **Sistema de Auditoría y Logs**: Genera registros diarios automáticos (`LOG-dd-MM-yyyy.log`) en `~/.config/userdb/`, con capacidad de consulta histórica e inspección reactiva en tiempo real (*live streaming*).
- **Rachas (*Streaks*)**: Seguimiento y actualización de actividad periódica por usuario.
- **Búsqueda Avanzada**: Búsquedas por múltiples criterios (edad, racha, fandom, pronombres, roles, fechas) con soporte para modo rápido (*fast search*).
- **Importación/Exportación Flexible**: Exporta perfiles individuales a JSON o la base de datos completa a archivos comprimidos `.userdb` (ZIP), con 4 modos de resolución de conflictos.

---

## 2. Método de Instalación Sencillo

No requiere compilar manualmente. Puedes instalar UserDB directamente usando los scripts incluidos en el repositorio:

### En Linux
```bash
chmod +x install.sh
./install.sh
```
*Descarga la última versión de GitHub Release en `~/.local/bin/userdb` y lo añade automáticamente a tu `PATH` en `.bashrc` o `.zshrc`.*

### En Windows (PowerShell)
```powershell
.\install.ps1
```
*Descarga el ejecutable en `%LOCALAPPDATA%\UserDB\userdb.exe` y lo añade al `PATH` del usuario.*

Una vez instalado, abre una nueva terminal y ejecuta:
```bash
userdb
```

---

## 3. Método de Uso (Interactivo)

Para iniciar la interfaz interactiva con menús guiados por consola, ejecuta el comando sin argumentos:

```bash
userdb
```

### Opciones del Menú:
1. **Mostrar usuarios**: Imprime la tabla con todos los perfiles registrados y sus atributos.
2. **Agregar un usuario**: Asistente guiado paso a paso para dar de alta a un usuario con validaciones de datos.
3. **Verificar un usuario**: Módulo para registrar actividad y sumar o reiniciar la racha (*streak*) diaria.
4. **Modificar un usuario**: Submenús específicos con tabla comparativa en vivo para actualizar cualquier campo (nombre, edad, roles, pronombres, etc.).
5. **Eliminar usuario**: Selección y baja de un perfil previa confirmación.
6. **Importar/Exportar usuario**: Exporta un perfil específico a `.json` o importa uno resolviendo conflictos.
7. **Importar/Exportar base de datos**: Genera un archivo `.userdb` comprimido con toda la base de datos o restaura uno existente.
8. **Buscar usuario**: Menú interactivo con filtros combinables (por nombre, fandom, rangos de edad/racha, etc.).
9. **Salir**: Cierra la aplicación liberando los recursos de la base de datos.

---

## 4. Método de Uso (CLI) y Conexión con Otros Programas

UserDB incluye una interfaz CLI completa (`System.CommandLine`) ideal para automatizaciones, pipelines y scripts.

### Comandos Principales

| Comando | Descripción | Ejemplo |
| :--- | :--- | :--- |
| `list` | Lista usuarios (formato normal, `--table` o `--raw`) | `userdb list --table` |
| `show` | Muestra los detalles de un usuario (`--raw` para JSON puro) | `userdb show -u "mob100"` |
| `add` | Registra un nuevo usuario con validación de campos | `userdb add -n "Reigen" -f "Mob Psycho" -A 28 -p "Él,He"` |
| `modify` | Modifica atributos de un usuario existente | `userdb modify -u "reigen" -A 29 -S "Ocupado"` |
| `delete` | Elimina un usuario (`--no-confirm` para scripts) | `userdb delete -u "reigen" --no-confirm` |
| `search` | Filtra usuarios por criterios (`--fast`, `--raw`) | `userdb search -f "Mob Psycho" --min-age 18` |
| `log` | Inspecciona o monitorea logs en tiempo real (`--live`, `--cat`) | `userdb log --live` |
| `export` | Exporta individual (`-u`) o todo (`-a`) | `userdb export -a -t "copia.userdb"` |
| `import` | Importa usuarios con resolución de conflictos (`-m`) | `userdb import -a -t "copia.userdb" -m keep` |

> Modos de resolución de conflictos en `import` (`-m`): `keep`, `overwrite`, `combine-keeping-original`, `combine-keeping-new`.

---

### Detalles de Comandos Específicos

#### 🔹 Auditoría y Logs (`log`)
```bash
# Monitorear logs en vivo a medida que ocurren eventos
userdb log --live

# Ver las últimas 20 entradas del registro actual
userdb log --cat 20

# Ver todo el historial del log de hoy
userdb log --cat -1
```
*Parámetros:*
- `-l, --live`: Transmite las nuevas entradas de log en tiempo real por consola (detener con `ENTER`).
- `-c, --cat <n>`: Muestra tantas entradas del registro como se especifique en orden cronológico inverso (`-1` para todas).

#### 🔹 Búsqueda Avanzada (`search`)
```bash
# Búsqueda rápida por nombre en memoria
userdb search -n "Reigen" --fast

# Filtrar por fandom y rango de edad
userdb search -f "Mob Psycho 100" --min-age 18 --max-age 30

# Búsqueda con salida cruda
userdb search --status "Activo" --raw
```

---

### Conexión e Integración con Otros Programas

Gracias al modificador `--raw` y al subcomando `log`, UserDB emite datos limpios (JSON estructurado o texto separado por comas) para encadenarse mediante tuberías (*pipes*):

#### 1. Procesar datos con `jq`
Consultar un perfil en formato JSON crudo y extraer o transformar propiedades con `jq`:

```bash
# Obtener solo el estado y la racha de un usuario
userdb show -u "mob100" --raw | jq '{nombre: .name, racha: .streak, estado: .status}'

# Validar en un script de bash si un usuario está activo
ESTADO=$(userdb show -u "mob100" --raw | jq -r '.status')
if [ "$ESTADO" = "Activo" ]; then
    echo "El usuario está habilitado."
fi
```

#### 2. Monitoreo reactivo de logs con `grep`
Supervisar operaciones críticas en segundo plano:

```bash
# Filtrar en tiempo real solo los errores generados en el sistema
userdb log --live | grep --line-buffered "ERROR"

# Consultar advertencias recientes en el registro
userdb log --cat 50 | grep -i "warning"
```

#### 3. Procesar listas con `cut`, `awk` y bucles `while read`
Iterar sobre los usuarios registrados utilizando la salida cruda de `list`:

```bash
# Extraer únicamente los nombres de usuario del índice
userdb list --raw | cut -d',' -f1

# Iterar sobre todos los usuarios para ejecutar acciones externas
userdb list --raw | while IFS=',' read -r nombre ruta; do
    id=$(basename "$ruta" .json)
    echo "Sincronizando: $nombre (ID: $id)"
done
```

#### 4. Búsqueda y filtrado en tuberías (*pipes*)
Filtrar usuarios por criterios específicos y canalizar los resultados:

```bash
# Contar cuántos usuarios pertenecen a un fandom
userdb search -f "Mob Psycho 100" --raw | wc -l

# Obtener los IDs de usuarios mayores de 18 años
userdb search --min-age 18 --raw | cut -d',' -f2 | xargs -n 1 basename -s .json
```

#### 5. Automatización de respaldos diarios con `cron`
Programar una exportación automática empaquetada:

```bash
# Script o tarea en crontab
userdb export -a -t "$HOME/backups/userdb_$(date +%Y%m%d).userdb"
```

#### 6. Integración con Python
Consumir UserDB directamente desde un script en Python:

```python
import subprocess
import json

# Ejecutar UserDB y parsear su salida JSON
salida = subprocess.check_output(["userdb", "show", "-u", "mob100", "--raw"])
usuario = json.loads(salida)

print(f"Usuario: {usuario['name']} | Racha: {usuario['streak']} días")
```

---

## 5. Método de Compilación

Si deseas compilar el código fuente por tu cuenta o generar binarios distribuidos:

### Prerrequisitos
- [.NET SDK](https://dotnet.microsoft.com/download) (versión 10.0 o 8.0+)

### Compilar Proyecto
```bash
dotnet build
```

### Ejecutar en Desarrollo
```bash
dotnet run -- list --table
```

### Compilar y Empaquetar Binarios Autocontenidos (Linux / Windows)
Puedes utilizar el script `export.sh` para compilar binarios autocontenidos (*single-file* comprimidos) listos para distribución:

```bash
chmod +x export.sh
./export.sh
```

O compilar manualmente para cada plataforma:

```bash
# Para Linux (binario único comprimido)
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o ./publish/linux-x64

# Para Windows
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/windows-x64
```

---

## 6. Tecnologías Usadas

- **C# / .NET 10.0**: Lenguaje y runtime principal de la aplicación.
- **System.CommandLine (2.0.10)**: Manejo, parsing y validación de argumentos y subcomandos CLI.
- **Nanoid (3.1.0)**: Generación de identificadores únicos seguros y compactos.
- **System.Text.Json**: Serialización y deserialización estructurada de perfiles.
- **System.IO.Compression**: Empaquetado y descompresión ZIP para archivos de base de datos `.userdb`.
- **FileSystemWatcher & I/O Streams**: Monitoreo y transmisión en tiempo real de eventos de log con detección de cambios y rotación.
- **Mecanismos de Bloqueo Basados en Archivos**: Control de concurrencia y prevención de condiciones de carrera con PID de proceso (`userdb.lock`).
- **Bash & PowerShell**: Scripts de instalación rápida (`install.sh`, `install.ps1`) y empaquetado (`export.sh`).

---

## 7. Estructura de Archivos y Carpetas

### Estructura del Código Fuente
```text
UserDatabase/
├── Commands/                     # Lógica de comandos CLI
│   ├── Builders/                 # Configuración de subcomandos y opciones (System.CommandLine)
│   │   ├── AddCommandBuilder.cs
│   │   ├── DeleteCommandBuilder.cs
│   │   ├── ExportCommandBuilder.cs
│   │   ├── ImportCommandBuilder.cs
│   │   ├── ListCommandBuilder.cs
│   │   ├── LogCommandBuilder.cs
│   │   ├── ModifyCommandBuilder.cs
│   │   ├── SearchCommandBuilder.cs
│   │   └── ShowCommandBuilder.cs
│   ├── AddUser.cs
│   ├── DeleteUser.cs
│   ├── ExportDataBase.cs
│   ├── ExportUser.cs
│   ├── ImportDataBase.cs
│   ├── ImportUser.cs
│   ├── ListUsers.cs
│   ├── LogCommand.cs
│   ├── ModifyUser.cs
│   ├── SearchUser.cs
│   └── Show.cs
├── InterfaceServices/            # Renderizado visual de tablas y formateo
│   └── ListUsers.cs
├── Menus/                        # Flujos y pantallas del modo interactivo
│   ├── ModifyMenus/              # Submenús interactivos para modificación de campos
│   ├── AddUser.cs
│   ├── DataBaseImportOrExport.cs
│   ├── ModifyUser.cs
│   ├── RemoveUser.cs
│   ├── SearchMenu.cs
│   ├── ShowUsers.cs
│   ├── UserImportOrExport.cs
│   └── VerifyUserStreak.cs
├── Models/                       # Modelos de datos y DTOs
│   ├── ModifyingUser.cs
│   ├── SearchParameters.cs
│   └── User.cs
├── Services/                     # Servicios centrales y persistencia
│   ├── DatabaseLock.cs           # Control de concurrencia y bloqueo de BD
│   ├── LiveLogReader.cs          # Lector en tiempo real con FileSystemWatcher
│   ├── Logs.cs                   # Auditoría y logging diario
│   ├── RegenerateIndex.cs        # Reconstrucción del índice users.dat
│   └── UserService.cs            # I/O de perfiles y resolución de rutas
├── Validators/                   # Validadores de entrada de datos
│   └── UserValidators.cs
├── screenshots/                  # Capturas de pantalla
├── ConsoleHelper.cs              # Manejo de colores y utilidades de consola
├── GlobalUsings.cs               # Usings globales del proyecto
├── Program.cs                    # Punto de entrada de la aplicación
├── userdb.csproj                 # Configuración del proyecto .NET
├── install.sh                    # Script de instalación para Linux
├── install.ps1                   # Script de instalación para Windows
├── export.sh                     # Script de compilación y empaquetado
└── LICENSE                       # Licencia MIT
```

### Rutas de Almacenamiento en el Sistema
- **Directorio de Usuarios**: `~/.userdb/` en Linux / `%USERPROFILE%\.userdb\` en Windows
  - `users.dat`: Índice central de usuarios (`Nombre,RutaJSON`).
  - `<userId>.json`: Archivo individual de datos de cada perfil.
  - `userdb.lock`: Archivo de control de concurrencia que registra el PID del proceso activo.
- **Directorio de Logs**: `~/.config/userdb/` en Linux / `%APPDATA%\userdb\` en Windows
  - `LOG-dd-MM-yyyy.log`: Registro diario de operaciones y auditoría.

---

## 8. Créditos y Licencia

- **Autor**: CMDPlayer216 (2026)
- **Licencia**: Distribuido bajo la Licencia MIT. Para más detalles, consulta el archivo [LICENSE](LICENSE).
