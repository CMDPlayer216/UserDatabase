# Resumen de Cambios - UserDB v3.1.1

Este documento detalla exclusivamente las modificaciones, correcciones de errores, refactorizaciones arquitectónicas y nuevas funcionalidades incorporadas al proyecto.

---

## 1. Corrección de Bugs

### 🐛 Bug Crítico al Modificar Usuario
- **Causa**: En `Commands/ModifyUser.cs`, al modificar un usuario sin alterar su ID, `inputUser.userId` era una cadena vacía. Al guardar con `Path.Combine(GPath, $"{inputUser.userId}.json")`, el archivo se guardaba literalmente como `.json` en el directorio de datos en lugar de usar el ID actual del usuario (`user.userId.json`).
- **Solución**: Se corrigió la ruta de guardado a `Path.Combine(GPath, $"{user.userId}.json")`, asegurando que los cambios se guarden siempre en el archivo del perfil correspondiente.
- **Validación de Racha**: En `Commands/ModifyUser.cs` la condición de racha evaluaba erróneamente `inputUser.age >= 0` en lugar de `inputUser.streak >= 0`. Se corrigió la condición y se estableció `-1` como valor por defecto/neutro para preservar la racha existente sin sobreescribirla con `0`.

### 🐛 Correcciones en Command Builders y CLI
- **`ModifyCommandBuilder.cs`**:
  - Se corrigió el mapeo donde `--add-additional-roles` asignaba los valores a `wantedRolesToAdd`.
  - Se corrigió el mapeo de `--fandom` que leía de `name` y `--status` que leía de `userId`.
  - Se ajustó el valor por defecto de `--streak` a `-1` y `--status` a `""` para no sobreescribir datos si no se pasan en CLI.
- **`ImportCommandBuilder.cs`**:
  - Se corrigió el error tipográfico en el arreglo de modos válidos (`combine-keepeng-original` -> `combine-keeping-original`).
- **`Show.cs`**:
  - Se corrigió la impresión de pronombres en consola (estaba imprimiendo `additionalRoles` en su lugar).

### 🐛 Correcciones en Servicios y Menús
- **`RegenerateIndex.cs`**:
  - Se corrigieron los bloques `catch` para ejecutar `continue` en lugar de `return`, evitando que un archivo corrupto o inválido cancele la reconstrucción del índice para los demás usuarios válidos.
  - Se corrigió la lectura de propiedades de JSON para soportar tanto `userId` como `UserId`.
- **`UserService.cs`**:
  - En `SaveUser`, al resolver colisiones de nombres añadiendo un sufijo numérico (ej. `usuario1.json`), se sincroniza ahora la propiedad `newUser.userId` antes de serializar el archivo.
- **`Menus/ModifyUser.cs`**:
  - Se añadió la eliminación en memoria (`RemoveAt`) en las listas interactivas de roles y pronombres para que la vista en pantalla refleje los cambios en tiempo real.

---

## 2. Refactorización Arquitectónica y Namespaces

Se eliminó el archivo monolítico `SubMenus.cs` y se organizó el proyecto en responsabilidades independientes bajo namespaces limpios:

- **`userdb.Models`**:
  - `User.cs`: Modelo de datos del perfil de usuario.
  - `ModifyingUser.cs`: DTO para transferir modificaciones (con racha por defecto en `-1`).
- **`userdb.Services`**:
  - `UserService.cs`: Persistencia, lectura/escritura de JSON y resolución de rutas en el sistema de archivos.
  - `RegenerateIndex.cs`: Sincronización y reconstrucción de `users.dat`.
  - `Logs.cs`: Sistema de registro y auditoría.
- **`userdb.InterfaceServices`**:
  - `ListUsers.cs`: Renderizado visual adaptativo de tablas multilínea con formato ASCII y colores.
- **`userdb.Commands`**:
  - Separación de cada comando en su propio archivo: `AddUser.cs`, `ModifyUser.cs`, `DeleteUser.cs`, `Show.cs`, `ListUsers.cs`, `ExportUser.cs`, `ImportUser.cs`, `ExportDataBase.cs`, `ImportDataBase.cs`.
- **`userdb.Commands.Builders`**:
  - Constructores de comandos CLI desacoplados para `System.CommandLine`: `AddCommandBuilder.cs`, `ModifyCommandBuilder.cs`, `DeleteCommandBuilder.cs`, `ShowCommandBuilder.cs`, `ListCommandBuilder.cs`, `ExportCommandBuilder.cs`, `ImportCommandBuilder.cs`.
- **`userdb.Menus`**:
  - Pantallas del modo interactivo separadas: `AddUser.cs`, `ModifyUser.cs`, `ShowUsers.cs`, `VerifyUserStreak.cs`, `RemoveUser.cs`, `UserImportOrExport.cs`, `DataBaseImportOrExport.cs`.
- **`userdb`**:
  - `Program.cs`: Punto de entrada y orquestador del menú principal.
  - `ConsoleHelper.cs`: Utilidades de consola y formato de texto.

---

## 3. Sistema de Logs Diario

- **Rotación diaria sin hora en el nombre de archivo**:
  - Los archivos de log se guardan en el directorio de configuración de la aplicación (`~/.config/userdb/` en Linux / `%APPDATA%\userdb\` en Windows) con el formato:
    `LOG-dd-MM-yyyy.log` (ej. `LOG-22-08-2026.log`).
  - Las acciones realizadas hoy y mañana quedan automáticamente divididas en archivos independientes según la fecha del sistema.
- **Formato estructurado**:
  - `[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] [{Info|Warning|Error}] <{Prioridad} {Título}> {Mensaje}`
- **Trazabilidad completa**:
  - Se integró `Logs.Log` en **todos los métodos y clases** del sistema: arranque de CLI, opciones seleccionadas en menús interactivos, operaciones de guardado/modificación/eliminación de usuarios, cambios individuales de atributos, lectura de índices y excepciones capturadas.

---

## 4. Importación y Exportación de Base de Datos y Usuarios

- **Exportación**:
  - Individual: Guarda el perfil seleccionado en formato `.json`.
  - Base de Datos: Comprime todos los archivos del directorio de datos en un paquete `.userdb` (formato ZIP).
- **Importación con 4 Modos de Resolución**:
  - `keep`: Conserva el usuario local existente en caso de colisión de ID.
  - `overwrite`: Sobrescribe los datos del usuario local existente.
  - `combine-keeping-original`: Combina listas agregando elementos nuevos, preservando los datos individuales del perfil original.
  - `combine-keeping-new`: Combina listas y actualiza los campos individuales con la información del nuevo perfil.
