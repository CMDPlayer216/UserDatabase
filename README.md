# UserDB v2.0 (STABLE)

**UserDB** es una herramienta de línea de comandos (CLI) desarrollada en C# / .NET para la gestión ligera y estructurada de perfiles de usuario, roles, fandoms y seguimiento de rachas (streaks) diarias. 

El sistema almacena la información localmente mediante ficheros JSON individuales para cada usuario y un índice general `.dat`, ofreciendo una interfaz visual en consola formateada con colores y tablas multilínea.

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

- **Modificación Interactiva de Usuarios (NUEVO v2.0)**: Módulo dinámico para editar perfiles existentes, renombrar archivos JSON en tiempo real actualizando el índice, y gestionar elementos de arreglos de forma interactiva (añadir/eliminar roles o pronombres).
- **Estructura de Datos Limpia**: Arreglos opcionales vacíos por defecto (`Array.Empty<string>()`) para un manejo de datos consistente sin etiquetas innecesarias.
- **Tabla Multilínea en Consola**: Renderizado dinámico con ajuste de columnas para arrays de roles buscados, roles adicionales y pronombres.
- **Sistema de Rachas (Streaks)**: Módulo interactivo para verificar la actividad diaria de un usuario e incrementar o reiniciar su contador.
- **Persistencia JSON Local**: Cada perfil se guarda de forma independiente en un archivo `.json` formateado, evitando colisiones de nombres mediante numeración automática.
- **Indexación Rápida**: Mantenimiento de un archivo `users.dat` que actúa como índice central de rutas.
- **Soporte Cross-Platform**: Scripts de compilación unificados y scripts de instalación automática para Linux y Windows (`install.sh` e `install.bat`).

---

## 🛠️ Tecnologías Utilizadas

- **Lenguaje**: C# (.NET 8.0+)
- **Librerías**: `System.Text.Json` para serialización
- **Compilación**: Target nativo `linux-x64` y `win-x64` con `PublishSingleFile`

---

## 📂 Estructura de Datos y Almacenamiento

El programa crea automáticamente el directorio de datos en la carpeta personal del usuario (`~/.userdb/` en Linux / `%USERPROFILE%\.userdb\` en Windows).

```text
~/.userdb/
├── users.dat              # Índice general (formato: Nombre,RutaArchivoJSON)
├── Gabriel.json           # Perfil individual en formato JSON
├── Gabriel1.json          # Autonumeración en caso de duplicidad de nombre
└── ...
```

### Ejemplo de esquema JSON (`User.json`)

```json
{
  "name": "Gabriel",
  "userId": "d7a4b12c-9e23-4567-89ab-cdef01234567",
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
  "dateRegistered": "2026-07-21",
  "streak": 5
}
```

---

## 🔨 Compilación e Instalación

### Prerrequisitos

Tener instalado el SDK de .NET:

```bash
dotnet --version
```

### Exportar Binarios (Linux & Windows)

Puedes ejecutar el script unificado de exportación para compilar ambas plataformas de forma aislada:

```bash
chmod +x export.sh
./export.sh
```

Esto generará los ejecutables comprimidos (*self-contained*) en `./publish/linux-x64/` y `./publish/windows-x64/`.

### Instalación Rápida

- **En Linux:**
  ```bash
  chmod +x install.sh
  ./install.sh
  ```
  *(Copia el binario a `~/.local/bin/userdb`)*

- **En Windows:**
  Ejecuta el script `install.bat` en tu terminal o con doble clic.
  *(Instala el ejecutable en `%LOCALAPPDATA%\userdb` y lo añade automáticamente al `PATH` del usuario)*

---

## 📖 Modo de Uso

Una vez instalado, ejecuta la herramienta en tu terminal:

```bash
userdb
```

### Opciones del Menú

1. **Mostrar usuarios**: Despliega la tabla ASCII interactiva con todos los usuarios registrados y sus métricas.
2. **Agregar un usuario**: Formulario interactivo en consola con validaciones de tipo y campos obligatorios.
3. **Modificar un usuario**: Menú interactivo para editar datos personales, renombrar perfiles y gestionar arreglos (roles, pronombres).
4. **Verificar un usuario**: Selecciona un perfil por su índice y marca si cumplió su actividad del día para subir o reiniciar su racha.
5. **Salir**: Cierra la aplicación.

---

## 📜 Licencia y Créditos

Desarrollado por **CMDPlayer216** (2026).
