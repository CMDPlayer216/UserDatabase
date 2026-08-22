using System.CommandLine;
using userdb.Commands;
using userdb.Models;
using userdb.Services;

namespace userdb.Commands.Builders;

public static class ModifyCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("modify", "Permite modificar un usuario");

        var name = new Option<string>("--name", "-n") { Description = "Nuevo nombre de usuario", DefaultValueFactory = _ => "" };
        var additionalRolesToAdd = new Option<string>("--add-additional-roles", "-a") { Description = "Añadir roles adicionales", DefaultValueFactory = _ => "" };
        var additionalRolesToRemove = new Option<string>("--remove-additional-roles", "-r") { Description = "Eliminar roles adicionales", DefaultValueFactory = _ => "" };
        var fandom = new Option<string>("--fandom", "-f") { Description = "Nuevo fandom que se le atribuye", DefaultValueFactory = _ => "" };
        var wantedRolesToAdd = new Option<string>("--add-wanted-roles", "-w") { Description = "Añadir personajes que el usuario busca", DefaultValueFactory = _ => "" };
        var wantedRolesToRemove = new Option<string>("--remove-wanted-roles", "-W") { Description = "Eliminar personajes que el usuario busca", DefaultValueFactory = _ => "" };
        var age = new Option<int>("--age", "-A") { Description = "Nueva edad del usuario", DefaultValueFactory = _ => 0 };
        var pronounsToAdd = new Option<string>("--add-pronouns", "-p") { Description = "Añadir pronombres al usuario", DefaultValueFactory = _ => "" };
        var pronounsToRemove = new Option<string>("--remove-pronouns", "-P") { Description = "Eliminar pronombres del usuario", DefaultValueFactory = _ => "" };
        var streak = new Option<int>("--streak", "-s") { Description = "Nueva racha del usuario", DefaultValueFactory = _ => -1 };
        var userId = new Option<string>("--user-id", "-U") { Description = "Nuevo ID del usuario", DefaultValueFactory = _ => "" };
        var sourceUser = new Option<string>("--source-user", "-u") { Description = "Usuario que se va a modificar", Required = true };
        var status = new Option<string>("--status", "-S") { Description = "Estado del usuario", DefaultValueFactory = _ => "" };

        command.Add(name);
        command.Add(additionalRolesToAdd);
        command.Add(additionalRolesToRemove);
        command.Add(fandom);
        command.Add(wantedRolesToAdd);
        command.Add(wantedRolesToRemove);
        command.Add(age);
        command.Add(pronounsToAdd);
        command.Add(pronounsToRemove);
        command.Add(streak);
        command.Add(userId);
        command.Add(sourceUser);
        command.Add(status);

        command.SetAction((ParseResult parseResult) =>
        {
            ModifyingUser user = new ModifyingUser();

            string sourceVal = parseResult.GetValue(sourceUser) ?? "";
            Logs.Log("ModifyCommandBuilder", $"Ejecutando subcomando CLI 'modify' para '{sourceVal}'", Logs.logType.Info, 2);

            string newRolesToAdd = parseResult.GetValue(additionalRolesToAdd) ?? "";
            string oldRolesToRemove = parseResult.GetValue(additionalRolesToRemove) ?? "";
            string wantedCharactersToAdd = parseResult.GetValue(wantedRolesToAdd) ?? "";
            string wantedCharactersToRemove = parseResult.GetValue(wantedRolesToRemove) ?? "";
            string newPronounsToAdd = parseResult.GetValue(pronounsToAdd) ?? "";
            string oldPronounsToRemove = parseResult.GetValue(pronounsToRemove) ?? "";

            user.source = sourceVal;
            user.name = parseResult.GetValue(name) ?? "";
            user.additionalRolesToAdd = newRolesToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            user.additionalRolesToRemove = oldRolesToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            user.age = parseResult.GetValue(age);
            user.fandom = parseResult.GetValue(fandom) ?? "";
            user.pronounsToAdd = newPronounsToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            user.pronounsToRemove = oldPronounsToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            user.status = parseResult.GetValue(status) ?? "";
            user.streak = parseResult.GetValue(streak);
            user.userId = parseResult.GetValue(userId) ?? "";
            user.wantedRolesToAdd = wantedCharactersToAdd.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            user.wantedRolesToRemove = wantedCharactersToRemove.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

            ModifyUser.Run(user);
        });

        return command;
    }
}