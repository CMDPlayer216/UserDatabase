using System.CommandLine;
using userdb.Services;
using userdb.Validators;

namespace userdb.Commands.Builders;

public static class AddCommandBuilder
{
    public static Command Create()
    {
        var addCommand = new Command("add", "Permite añadir un usuario");

        var nameOption = new Option<string>("--name", "-n")
        {
            Description = "Nombre del usuario a registrar",
            Required = true
        };
        var additionalRolesOption = new Option<string>("--additional-roles", "-a")
        {
            Description = "Roles adicionales que se le atribuyen",
            DefaultValueFactory = _ => ""
        };
        var fandomOption = new Option<string>("--fandom", "-f")
        {
            Description = "Fandom que se le atribuye",
            Required = true
        };
        var lookedCharactersOption = new Option<string>("--looked-characters", "-l")
        {
            Description = "Personajes que el usuario busca",
            DefaultValueFactory = _ => ""
        };
        var ageOption = new Option<int>("--age", "-A")
        {
            Description = "Edad del usuario",
            Required = true
        };
        var pronounsOption = new Option<string>("--pronouns", "-p")
        {
            Description = "Pronombres del usuario",
            Required = true
        };
        var streakOption = new Option<int>("--streak", "-s")
        {
            Description = "Racha del usuario",
            DefaultValueFactory = _ => 0
        };
        Option<string>? userIdOption = new("--user-id", "-u")
        {
            Description = "ID del usuario",
            DefaultValueFactory = _ => Nanoid.Generate(size: 12)
        };
        var statusOption = new Option<string>("--status", "-S")
        {
            Description = "Estado del usuario",
            DefaultValueFactory = _ => "Activo"
        };

        addCommand.Add(nameOption);
        addCommand.Add(additionalRolesOption);
        addCommand.Add(fandomOption);
        addCommand.Add(lookedCharactersOption);
        addCommand.Add(ageOption);
        addCommand.Add(pronounsOption);
        addCommand.Add(streakOption);
        addCommand.Add(userIdOption);
        addCommand.Add(statusOption);

        addCommand.SetAction(parseResult =>
        {
            Logs.Log("AddCommandBuilder", "Ejecutando subcomando CLI 'add'", Logs.logType.Info, 2);
            string name = parseResult.GetValue(nameOption) ?? "Desconocido";                 // Limpiamos la entrada de la opción -n / --name de forma inicial
            string additionalRoles = parseResult.GetValue(additionalRolesOption) ?? "";      // Limpiamos la entrada de la opción -a / --additional-roles de forma inicial
            string fandom = parseResult.GetValue(fandomOption) ?? "Desconocido";             // Limpiamos la entrada de la opción -f / --fandom de forma inicial
            string lookedCharacters = parseResult.GetValue(lookedCharactersOption) ?? "";    // Limpiamos la entrada de la opción -l / --looked-characters de forma inicial
            string pronouns = parseResult.GetValue(pronounsOption) ?? "";                    // Limpiamos la entrada de la opción -p / --pronouns de forma inicial
            string userid = parseResult.GetValue(userIdOption) ?? Nanoid.Generate(size: 12); // Limpiamos la entrada de la opcion -u / --user-id de forma inicial
            int age = parseResult.GetValue(ageOption);
            int streak = parseResult.GetValue(streakOption);
            string status = parseResult.GetValue(statusOption) ?? "Activo";

            if (!UserValidators.ValidateId(userid))
            {
                DrawText("El ID no es válido");
                return;
            }
            if (!UserValidators.ValidateName(name))
            {
                DrawText("El nombre no es válido");
                return;
            }
            foreach (string rol in additionalRoles.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (!UserValidators.ValidateRole(rol))
                {
                    DrawText($"El rol \"{rol}\" es inválido");
                    return;
                }
            }
            if (!UserValidators.ValidateFandom(fandom))
            {
                DrawText("El fandom no es válido");
                return;
            }
            foreach (string rol in lookedCharacters.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (!UserValidators.ValidateRole(rol))
                {
                    DrawText($"El rol \"{rol}\" es inválido");
                    return;
                }
            }
            if (UserValidators.ValidateAge(age) is not true)
            {
                DrawText("La edad no es válida");
                return;
            }
            foreach (string pronoun in pronouns.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (!UserValidators.ValidateRole(pronoun))
                {
                    DrawText($"El pronombre \"{pronoun}\" es inválido");
                    return;
                }
            }
            if (UserValidators.ValidateStreak(streak) is not true)
            {
                DrawText("La racha no es válida");
                return;
            }
            if (!UserValidators.ValidateStatus(status))
            {
                DrawText("El ID no es válido");
                return;
            }

            AddUser.Run(                                                                                        // Le pasamos el control a Commands.AddUser() que añadirá la información según se haya especificado
                string.Concat(name.Split(Path.GetInvalidFileNameChars())).Replace(",", ""),                          // Terminamos de limpiar la entrada de -n / --name
                [.. additionalRoles.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)],  // Terminamos de limpiar la entrada de -a / --aditional-roles
                fandom,                                                                                              // Pasamos la opcion -f / --fandom
                [.. lookedCharacters.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)], // Terminamos de limpiar la entrada de -l / --looked-characters
                age,                                                                     // Pasamos la opcion -A / --age
                [.. pronouns.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)],         // Terminamos de limpiar la entrada de -p / --pronouns
                streak,                                                                  // Pasamos la opcion -s / --streak
                userid,                                                                                              // Pasamos la opcion -u / --user-id
                status                                                            // Pasamos la opcion -S / --status
                );
        });

        return addCommand;
    }
}