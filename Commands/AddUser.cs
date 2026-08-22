using NanoidDotNet;
using userdb.Models;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.Commands;

public static class AddUser
{
    public static void Run(string name, List<string> additionalRoles, string fandom, List<string> wantedRoles, int age, List<string> pronouns, int streak, string id, string status)
    {
        string logTitle = "AddUser";
        Logs.Log(logTitle, $"Ejecutando comando AddUser para '{name}'", Logs.logType.Info, 2);

        User newUser = new();
        newUser.name = name;
        newUser.additionalRoles = additionalRoles;
        newUser.fandom = fandom;
        newUser.wantedRoles = wantedRoles;
        newUser.age = age;
        newUser.pronouns = pronouns;
        newUser.streak = streak;
        newUser.status = status;
        if (string.IsNullOrEmpty(id))
        {
            id = Nanoid.Generate(size: 10);
            Logs.Log(logTitle, $"ID generado automáticamente: {id}", Logs.logType.Info, 1);
        }
        newUser.userId = id;
        newUser.dateRegistered = DateOnly.FromDateTime(DateTime.Now);
        newUser.status = status;

        SaveUser(newUser);
        Logs.Log(logTitle, $"Usuario '{name}' con ID '{newUser.userId}' guardado con éxito", Logs.logType.Info, 2);
        DrawText("Usuario agregado exitosamente!", Color.Green);
    }
}
