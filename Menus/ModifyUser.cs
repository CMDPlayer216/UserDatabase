using userdb.Commands;
using userdb.Menus.ModifyMenus;
using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;

namespace userdb.Menus;

/// <summary>
/// Menú interactivo principal para modificar usuarios.
/// Permite editar cada campo individualmente ([1] al [9]), gestionar adiciones/eliminaciones
/// de elementos en listas y guardar explícitamente los cambios con confirmación.
/// </summary>
public static class ModifyUserMenu
{
    public static void Show()
    {
        string logTitle = "ModifyUserMenu";
        Logs.Log(logTitle, "Mostrando menú interactivo para modificar usuario", Logs.logType.Info, 2);
        Console.Clear();

        while (true)
        {
            List<string> users = UserService.GetUserIndexLines();

            // Si no existen usuarios registrados, notificar y regresar al menú principal
            if (users.Count == 0)
            {
                Logs.Log(logTitle, "No hay usuarios registrados en el sistema para modificar", Logs.logType.Info, 1);
                DrawText("No hay usuarios registrados para modificar.", Color.Red);
                DrawText("Presiona [Enter] o cualquier tecla para volver...", Color.Gray);
                Pause();
                return;
            }

            Console.Clear();
            DrawText("=== SELECCIONAR USUARIO PARA MODIFICAR ===", Color.Yellow);
            DrawText("Selecciona un usuario (o escribe \"back\" para volver):", Color.White);
            DrawText("");

            int i = 1;
            foreach (string cuser in users)
            {
                string userName = cuser.Split(',')[0];
                DrawText($"  [{i}] {userName}", Color.Yellow);
                i++;
            }

            DrawText("");
            string input = TakeInput("Ingresa el número del usuario: ", Color.Green);

            if (string.Equals(input, "back", StringComparison.OrdinalIgnoreCase))
            {
                Logs.Log(logTitle, "Volviendo al menú principal desde selección de usuario", Logs.logType.Info, 1);
                break;
            }

            if (!int.TryParse(input, out int selectionIndex) || selectionIndex < 1 || selectionIndex > users.Count)
            {
                Logs.Log(logTitle, $"Selección de usuario no válida: '{input}'", Logs.logType.Warning, 2);
                DrawText("Esa no es una opción válida!", Color.Red);
                DrawText("Presiona [Enter] o cualquier tecla para reintentar...", Color.Gray);
                Pause();
                continue;
            }

            string userFilePath = users[selectionIndex - 1].Split(',')[1];
            User? user = UserService.LoadUserFromJson(userFilePath);

            if (user == null)
            {
                Logs.Log(logTitle, $"Usuario corrupto detectado en: {userFilePath}", Logs.logType.Error, 3);
                DrawText("Usuario corrupto detectado.", Color.Red);
                DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                Pause();
                continue;
            }

            Logs.Log(logTitle, $"Usuario seleccionado: '{user.userId}' ({user.name})", Logs.logType.Info, 2);

            // currentUser acumula los cambios de esta sesión de edición.
            // Se inicializa source con el ID actual para que Commands.ModifyUser ubique el JSON correspondiente.
            ModifyingUser currentUser = new()
            {
                source = user.userId
            };

            // referenceEmptyUser proporciona los valores neutrales de ModifyingUser para comparar cambios
            ModifyingUser referenceEmptyUser = new();

            bool shouldExitUserEdit = false;

            while (!shouldExitUserEdit)
            {
                Console.Clear();

                // Cabecera estilizada
                DrawText("================================================================================", Color.DarkYellow);
                DrawText($"          MODIFICAR USUARIO: {user.name} (ID: {user.userId})", Color.Yellow);
                DrawText("================================================================================", Color.DarkYellow);
                DrawText(string.Format("  {0,-24} | {1,-26} | {2}", "CAMPO", "VALOR ACTUAL", "CAMBIOS PENDIENTES"), Color.Cyan);
                DrawText("---------------------------+----------------------------+-----------------------", Color.DarkGray);

                // [1] Nombre
                string namePending = currentUser.name != referenceEmptyUser.name ? $"-> {currentUser.name}" : "";
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[1] Nombre", Truncate(user.name, 26)), Color.White, insertNewLine: false);
                DrawText(namePending, Color.Green);

                // [2] Fandom
                string fandomPending = currentUser.fandom != referenceEmptyUser.fandom ? $"-> {currentUser.fandom}" : "";
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[2] Fandom", Truncate(user.fandom, 26)), Color.White, insertNewLine: false);
                DrawText(fandomPending, Color.Green);

                // [3] Edad
                string agePending = currentUser.age != referenceEmptyUser.age ? $"-> {currentUser.age}" : "";
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[3] Edad", user.age.ToString()), Color.White, insertNewLine: false);
                DrawText(agePending, Color.Green);

                // [4] Roles buscados
                string wantedActual = user.wantedRoles.Count > 0 ? string.Join(", ", user.wantedRoles) : "(ninguno)";
                List<string> wantedDiff = [];
                if (currentUser.wantedRolesToAdd.Count > 0) wantedDiff.Add("+" + string.Join(", +", currentUser.wantedRolesToAdd));
                if (currentUser.wantedRolesToRemove.Count > 0) wantedDiff.Add("-" + string.Join(", -", currentUser.wantedRolesToRemove));
                string wantedPending = string.Join(" ", wantedDiff);
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[4] Roles buscados", Truncate(wantedActual, 26)), Color.White, insertNewLine: false);
                DrawText(wantedPending, wantedDiff.Count > 0 ? Color.Cyan : Color.Default);

                // [5] Roles adicionales
                string addActual = user.additionalRoles.Count > 0 ? string.Join(", ", user.additionalRoles) : "(ninguno)";
                List<string> addDiff = [];
                if (currentUser.additionalRolesToAdd.Count > 0) addDiff.Add("+" + string.Join(", +", currentUser.additionalRolesToAdd));
                if (currentUser.additionalRolesToRemove.Count > 0) addDiff.Add("-" + string.Join(", -", currentUser.additionalRolesToRemove));
                string addPending = string.Join(" ", addDiff);
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[5] Roles adicionales", Truncate(addActual, 26)), Color.White, insertNewLine: false);
                DrawText(addPending, addDiff.Count > 0 ? Color.Cyan : Color.Default);

                // [6] Pronombres
                string proActual = user.pronouns.Count > 0 ? string.Join(", ", user.pronouns) : "(ninguno)";
                List<string> proDiff = [];
                if (currentUser.pronounsToAdd.Count > 0) proDiff.Add("+" + string.Join(", +", currentUser.pronounsToAdd));
                if (currentUser.pronounsToRemove.Count > 0) proDiff.Add("-" + string.Join(", -", currentUser.pronounsToRemove));
                string proPending = string.Join(" ", proDiff);
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[6] Pronombres", Truncate(proActual, 26)), Color.White, insertNewLine: false);
                DrawText(proPending, proDiff.Count > 0 ? Color.Cyan : Color.Default);

                // [7] Racha
                string streakPending = currentUser.streak != referenceEmptyUser.streak ? $"-> {currentUser.streak}" : "";
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[7] Racha", user.streak.ToString()), Color.White, insertNewLine: false);
                DrawText(streakPending, Color.Green);

                // [8] Status
                string statusPending = currentUser.status != referenceEmptyUser.status ? $"-> {currentUser.status}" : "";
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[8] Status", Truncate(user.status, 26)), Color.White, insertNewLine: false);
                DrawText(statusPending, Color.Green);

                // [9] ID
                string idPending = currentUser.userId != referenceEmptyUser.userId ? $"-> {currentUser.userId}" : "";
                DrawText(string.Format("  {0,-24} | {1,-26} | ", "[9] ID", Truncate(user.userId, 26)), Color.White, insertNewLine: false);
                DrawText(idPending, Color.Green);

                DrawText("---------------------------+----------------------------+-----------------------", Color.DarkGray);
                DrawText("  [G] Guardar cambios", Color.Green);
                DrawText("  [back] Volver al listado de usuarios", Color.Gray);
                DrawText("================================================================================", Color.DarkYellow);

                // Evaluar si existen cambios pendientes comparando contra valores neutrales
                bool hasChanges = currentUser.name != referenceEmptyUser.name ||
                                  currentUser.fandom != referenceEmptyUser.fandom ||
                                  currentUser.age != referenceEmptyUser.age ||
                                  currentUser.wantedRolesToAdd.Count > 0 ||
                                  currentUser.wantedRolesToRemove.Count > 0 ||
                                  currentUser.additionalRolesToAdd.Count > 0 ||
                                  currentUser.additionalRolesToRemove.Count > 0 ||
                                  currentUser.pronounsToAdd.Count > 0 ||
                                  currentUser.pronounsToRemove.Count > 0 ||
                                  currentUser.streak != referenceEmptyUser.streak ||
                                  currentUser.status != referenceEmptyUser.status ||
                                  currentUser.userId != referenceEmptyUser.userId;

                if (hasChanges)
                {
                    DrawText(">> Hay cambios pendientes de guardar. Escribe [G] para aplicar o selecciona otro campo.", Color.Yellow);
                }
                else
                {
                    DrawText(">> Selecciona un campo del [1] al [9] para modificar, o escribe 'back' para volver.", Color.DarkGray);
                }

                input = TakeInput("Opción: ", Color.Green);

                // Salir o volver
                if (string.Equals(input, "back", StringComparison.OrdinalIgnoreCase))
                {
                    if (hasChanges)
                    {
                        DrawText("Tienes cambios sin guardar. ¿Deseas descartarlos y salir? (s/n): ", Color.Yellow, insertNewLine: false);
                        string confirmExit = TakeInput("", Color.Yellow).Trim().ToLower();
                        if (confirmExit == "s" || confirmExit == "si" || confirmExit == "y" || confirmExit == "yes")
                        {
                            Logs.Log(logTitle, $"Descartando cambios sin guardar para '{user.userId}'", Logs.logType.Info, 1);
                            break;
                        }
                        continue;
                    }

                    Logs.Log(logTitle, $"Saliendo de modificación de usuario '{user.userId}' sin cambios", Logs.logType.Info, 1);
                    break;
                }

                switch (input.Trim())
                {
                    // --- NOMBRE ---
                    case "1":
                        {
                            string currentVal = !string.IsNullOrEmpty(currentUser.name) ? currentUser.name : user.name;
                            string? newName = ModifyName.Run(currentVal);
                            if (newName != null)
                            {
                                // Si el usuario reingresa el valor original, vuelve al valor neutral (sin cambio)
                                currentUser.name = (newName == user.name) ? referenceEmptyUser.name : newName;
                                Logs.Log(logTitle, $"Nombre en cola de modificación: '{currentUser.name}'", Logs.logType.Info, 1);
                            }
                            break;
                        }

                    // --- FANDOM ---
                    case "2":
                        {
                            string currentVal = !string.IsNullOrEmpty(currentUser.fandom) ? currentUser.fandom : user.fandom;
                            string? newFandom = ModifyFandom.Run(currentVal);
                            if (newFandom != null)
                            {
                                currentUser.fandom = (newFandom == user.fandom) ? referenceEmptyUser.fandom : newFandom;
                                Logs.Log(logTitle, $"Fandom en cola de modificación: '{currentUser.fandom}'", Logs.logType.Info, 1);
                            }
                            break;
                        }

                    // --- EDAD ---
                    case "3":
                        {
                            int currentVal = currentUser.age != referenceEmptyUser.age ? currentUser.age : user.age;
                            int? newAge = ModifyAge.Run(currentVal);
                            if (newAge.HasValue)
                            {
                                currentUser.age = (newAge.Value == user.age) ? referenceEmptyUser.age : newAge.Value;
                                Logs.Log(logTitle, $"Edad en cola de modificación: {currentUser.age}", Logs.logType.Info, 1);
                            }
                            break;
                        }

                    // --- ROLES BUSCADOS ---
                    case "4":
                        {
                            while (true)
                            {
                                Console.Clear();
                                DrawText("=== MODIFICAR: ROLES BUSCADOS ===", Color.Yellow);
                                DrawText("");
                                DrawText("Roles actuales: " + (user.wantedRoles.Count > 0 ? string.Join(", ", user.wantedRoles) : "(ninguno)"), Color.White);
                                if (currentUser.wantedRolesToAdd.Count > 0)
                                    DrawText("Por añadir:     " + string.Join(", ", currentUser.wantedRolesToAdd), Color.Green);
                                if (currentUser.wantedRolesToRemove.Count > 0)
                                    DrawText("Por eliminar:   " + string.Join(", ", currentUser.wantedRolesToRemove), Color.Red);
                                DrawText("");
                                DrawText("  [1] Añadir roles individualmente");
                                DrawText("  [2] Eliminar roles individualmente");
                                DrawText("  [Enter] Volver");
                                DrawText("");

                                string subInput = TakeInput("Opción: ", Color.Green);
                                if (string.IsNullOrEmpty(subInput) || string.IsNullOrWhiteSpace(subInput)) break;

                                switch (subInput.Trim())
                                {
                                    case "1":
                                        currentUser.wantedRolesToAdd = ModifyWantedRolesToAdd.Run(user.wantedRoles, currentUser.wantedRolesToAdd);
                                        // Sincronización: si se añade un rol que estaba marcado para eliminar, se desmarca de eliminación
                                        currentUser.wantedRolesToRemove.RemoveAll(r => currentUser.wantedRolesToAdd.Contains(r, StringComparer.OrdinalIgnoreCase));
                                        break;
                                    case "2":
                                        currentUser.wantedRolesToRemove = ModifyWantedRolesToRemove.Run(user.wantedRoles, currentUser.wantedRolesToRemove);
                                        // Sincronización: si se marca para eliminar un rol recién añadido, se quita de la lista de añadir
                                        currentUser.wantedRolesToAdd.RemoveAll(r => currentUser.wantedRolesToRemove.Contains(r, StringComparer.OrdinalIgnoreCase));
                                        break;
                                }
                            }
                            break;
                        }

                    // --- ROLES ADICIONALES ---
                    case "5":
                        {
                            while (true)
                            {
                                Console.Clear();
                                DrawText("=== MODIFICAR: ROLES ADICIONALES ===", Color.Yellow);
                                DrawText("");
                                DrawText("Roles actuales: " + (user.additionalRoles.Count > 0 ? string.Join(", ", user.additionalRoles) : "(ninguno)"), Color.White);
                                if (currentUser.additionalRolesToAdd.Count > 0)
                                    DrawText("Por añadir:     " + string.Join(", ", currentUser.additionalRolesToAdd), Color.Green);
                                if (currentUser.additionalRolesToRemove.Count > 0)
                                    DrawText("Por eliminar:   " + string.Join(", ", currentUser.additionalRolesToRemove), Color.Red);
                                DrawText("");
                                DrawText("  [1] Añadir roles adicionales individualmente");
                                DrawText("  [2] Eliminar roles adicionales individualmente");
                                DrawText("  [Enter] Volver");
                                DrawText("");

                                string subInput = TakeInput("Opción: ", Color.Green);
                                if (string.IsNullOrEmpty(subInput) || string.IsNullOrWhiteSpace(subInput)) break;

                                switch (subInput.Trim())
                                {
                                    case "1":
                                        currentUser.additionalRolesToAdd = ModifyAdditionalRolesToAdd.Run(user.additionalRoles, currentUser.additionalRolesToAdd);
                                        currentUser.additionalRolesToRemove.RemoveAll(r => currentUser.additionalRolesToAdd.Contains(r, StringComparer.OrdinalIgnoreCase));
                                        break;
                                    case "2":
                                        currentUser.additionalRolesToRemove = ModifyAdditionalRolesToRemove.Run(user.additionalRoles, currentUser.additionalRolesToRemove);
                                        currentUser.additionalRolesToAdd.RemoveAll(r => currentUser.additionalRolesToRemove.Contains(r, StringComparer.OrdinalIgnoreCase));
                                        break;
                                }
                            }
                            break;
                        }

                    // --- PRONOMBRES ---
                    case "6":
                        {
                            while (true)
                            {
                                Console.Clear();
                                DrawText("=== MODIFICAR: PRONOMBRES ===", Color.Yellow);
                                DrawText("");
                                DrawText("Pronombres actuales: " + (user.pronouns.Count > 0 ? string.Join(", ", user.pronouns) : "(ninguno)"), Color.White);
                                if (currentUser.pronounsToAdd.Count > 0)
                                    DrawText("Por añadir:          " + string.Join(", ", currentUser.pronounsToAdd), Color.Green);
                                if (currentUser.pronounsToRemove.Count > 0)
                                    DrawText("Por eliminar:        " + string.Join(", ", currentUser.pronounsToRemove), Color.Red);
                                DrawText("");
                                DrawText("  [1] Añadir pronombres individualmente");
                                DrawText("  [2] Eliminar pronombres individualmente");
                                DrawText("  [Enter] Volver");
                                DrawText("");

                                string subInput = TakeInput("Opción: ", Color.Green);
                                if (string.IsNullOrEmpty(subInput) || string.IsNullOrWhiteSpace(subInput)) break;

                                switch (subInput.Trim())
                                {
                                    case "1":
                                        currentUser.pronounsToAdd = ModifyPronounsToAdd.Run(user.pronouns, currentUser.pronounsToAdd);
                                        currentUser.pronounsToRemove.RemoveAll(r => currentUser.pronounsToAdd.Contains(r, StringComparer.OrdinalIgnoreCase));
                                        break;
                                    case "2":
                                        currentUser.pronounsToRemove = ModifyPronounsToRemove.Run(user.pronouns, currentUser.pronounsToRemove, currentUser.pronounsToAdd);
                                        currentUser.pronounsToAdd.RemoveAll(r => currentUser.pronounsToRemove.Contains(r, StringComparer.OrdinalIgnoreCase));
                                        break;
                                }
                            }
                            break;
                        }

                    // --- RACHA ---
                    case "7":
                        {
                            int currentVal = currentUser.streak != referenceEmptyUser.streak ? currentUser.streak : user.streak;
                            int? newStreak = ModifyStreak.Run(currentVal);
                            if (newStreak.HasValue)
                            {
                                currentUser.streak = (newStreak.Value == user.streak) ? referenceEmptyUser.streak : newStreak.Value;
                                Logs.Log(logTitle, $"Racha en cola de modificación: {currentUser.streak}", Logs.logType.Info, 1);
                            }
                            break;
                        }

                    // --- STATUS ---
                    case "8":
                        {
                            string currentVal = !string.IsNullOrEmpty(currentUser.status) ? currentUser.status : user.status;
                            string? newStatus = ModifyStatus.Run(currentVal);
                            if (newStatus != null)
                            {
                                currentUser.status = (newStatus == user.status) ? referenceEmptyUser.status : newStatus;
                                Logs.Log(logTitle, $"Status en cola de modificación: '{currentUser.status}'", Logs.logType.Info, 1);
                            }
                            break;
                        }

                    // --- ID ---
                    case "9":
                        {
                            string currentVal = !string.IsNullOrEmpty(currentUser.userId) ? currentUser.userId : user.userId;
                            string? newId = ModifyUserId.Run(currentVal);
                            if (newId != null)
                            {
                                currentUser.userId = (newId == user.userId) ? referenceEmptyUser.userId : newId;
                                Logs.Log(logTitle, $"ID en cola de modificación: '{currentUser.userId}'", Logs.logType.Info, 1);
                            }
                            break;
                        }

                    // --- GUARDAR CAMBIOS ---
                    case "g":
                    case "G":
                    case "guardar":
                    case "save":
                    case "10":
                        {
                            if (!hasChanges)
                            {
                                Logs.Log(logTitle, "Intento de guardar sin cambios pendientes", Logs.logType.Info, 1);
                                DrawText("");
                                DrawText("No hay cambios pendientes para guardar.", Color.Yellow);
                                DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                                Pause();
                                break;
                            }

                            // Validación de campos obligatorios que no pueden quedar vacíos
                            // 1. Nombre
                            string effectiveName = !string.IsNullOrWhiteSpace(currentUser.name) ? currentUser.name : user.name;
                            if (string.IsNullOrWhiteSpace(effectiveName))
                            {
                                Logs.Log(logTitle, "Intento de guardar usuario con nombre vacío", Logs.logType.Warning, 2);
                                DrawText("");
                                DrawText("Error: El campo de nombre no puede estar vacío.", Color.Red);
                                DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                                Pause();
                                break;
                            }

                            // 2. ID
                            string effectiveId = !string.IsNullOrWhiteSpace(currentUser.userId) ? currentUser.userId : user.userId;
                            if (string.IsNullOrWhiteSpace(effectiveId))
                            {
                                Logs.Log(logTitle, "Intento de guardar usuario con ID vacío", Logs.logType.Warning, 2);
                                DrawText("");
                                DrawText("Error: El campo de ID no puede estar vacío.", Color.Red);
                                DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                                Pause();
                                break;
                            }

                            // 3. Status
                            string effectiveStatus = !string.IsNullOrWhiteSpace(currentUser.status) ? currentUser.status : user.status;
                            if (string.IsNullOrWhiteSpace(effectiveStatus))
                            {
                                Logs.Log(logTitle, "Intento de guardar usuario con status vacío", Logs.logType.Warning, 2);
                                DrawText("");
                                DrawText("Error: El campo de status no puede estar vacío.", Color.Red);
                                DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                                Pause();
                                break;
                            }

                            // 4. Pronombres: calcular cuántos pronombres quedan efectivos
                            int effectivePronounsCount = user.pronouns
                                .Count(p => !currentUser.pronounsToRemove.Contains(p, StringComparer.OrdinalIgnoreCase)) 
                                + currentUser.pronounsToAdd.Count;

                            if (effectivePronounsCount <= 0)
                            {
                                Logs.Log(logTitle, $"Intento de guardar usuario '{user.userId}' sin pronombres rechazado", Logs.logType.Warning, 2);
                                DrawText("");
                                DrawText("Error: El campo de pronombres no puede quedar vacío. Debe existir al menos un pronombre.", Color.Red);
                                DrawText("Añade al menos un pronombre antes de guardar los cambios.", Color.Yellow);
                                DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                                Pause();
                                break;
                            }

                            DrawText("");
                            DrawText("¿Confirmas que deseas guardar los cambios realizados en el usuario? (s/n): ", Color.Yellow, insertNewLine: false);
                            string confirm = TakeInput("", Color.Yellow).Trim().ToLower();

                            if (confirm == "s" || confirm == "si" || confirm == "y" || confirm == "yes")
                            {
                                // Asignar source para que Commands.ModifyUser encuentre el archivo JSON original
                                currentUser.source = user.userId;
                                Logs.Log(logTitle, $"Guardando cambios para el usuario '{user.userId}' ({user.name})", Logs.logType.Info, 2);

                                Commands.ModifyUser.Run(currentUser);

                                DrawText("");
                                DrawText("Presiona [Enter] o cualquier tecla para volver al listado de usuarios...", Color.Gray);
                                Pause();
                                shouldExitUserEdit = true;
                            }
                            else
                            {
                                Logs.Log(logTitle, "Guardado cancelado por el usuario", Logs.logType.Info, 1);
                                DrawText("Guardado cancelado. Los cambios siguen pendientes en memoria.", Color.Yellow);
                                DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                                Pause();
                            }
                            break;
                        }

                    default:
                        {
                            DrawText("Opción no reconocida!", Color.Red);
                            DrawText("Presiona [Enter] o cualquier tecla para continuar...", Color.Gray);
                            Pause();
                            break;
                        }
                }
            }
        }
    }

    /// <summary>
    /// Pausa la ejecución esperando una tecla, soportando de forma segura entradas estándar redirigidas.
    /// </summary>
    private static void Pause()
    {
        if (Console.IsInputRedirected)
            Console.ReadLine();
        else
            Console.ReadKey(true);
    }
}