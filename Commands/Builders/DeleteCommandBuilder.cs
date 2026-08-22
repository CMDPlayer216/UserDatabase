using System.CommandLine;
using userdb.Commands;
using userdb.Services;

namespace userdb.Commands.Builders;

public static class DeleteCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("delete", "Permite eliminar un usuario");
        var userId = new Option<string>("--user-id", "-u") { Description = "ID del usuario a eliminar", Required = true };
        var noConfirm = new Option<bool>("--no-confirm") { Description = "Salta la confirmación (usar con cuidado)", DefaultValueFactory = _ => false };

        command.Add(userId);
        command.Add(noConfirm);

        command.SetAction((ParseResult parseResult) =>
        {
            Logs.Log("DeleteCommandBuilder", "Ejecutando subcomando CLI 'delete'", Logs.logType.Info, 2);
            DeleteUser.Run(parseResult.GetValue(userId) ?? "0", parseResult.GetValue(noConfirm));
        });

        return command;
    }
}