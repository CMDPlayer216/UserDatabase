using userdb.InterfaceServices;
using userdb.Services;
using static userdb.ConsoleHelper;
using static userdb.Services.UserService;

namespace userdb.Commands;

public static class ListUsers
{
    public static void Run(bool isTable, bool isRaw, bool isTheIndexRegenerated = false)
    {
        string logTitle = "ListUsers";
        Logs.Log(logTitle, $"Listando usuarios (isTable: {isTable}, isRaw: {isRaw}, isTheIndexRegenerated: {isTheIndexRegenerated})", Logs.logType.Info, 1);

        List<string> userLines = GetUserIndexLines(!isTheIndexRegenerated);
        Logs.Log(logTitle, $"Líneas del índice obtenidas: {userLines.Count}", Logs.logType.Info, 1);

        if (userLines.Count == 0 && !isRaw)
        {
            Logs.Log(logTitle, "No hay usuarios registrados para listar", Logs.logType.Info, 1);
            DrawText("No hay usuarios registrados.", Color.Red);
            return;
        }

        if (isRaw)
        {
            Logs.Log(logTitle, "Mostrando lista en formato crudo (raw)", Logs.logType.Info, 1);
            string output = string.Join(Environment.NewLine, userLines);
            DrawText(output);
            return;
        }

        if (isTable)
        {
            Logs.Log(logTitle, "Mostrando lista en formato tabla", Logs.logType.Info, 1);
            List.InTable(userLines);
        }
        else
        {
            Logs.Log(logTitle, "Mostrando lista en formato simple de nombres", Logs.logType.Info, 1);
            foreach (string userline in userLines)
            {
                List<string> user = userline.Split(',').ToList();
                if (user.Count > 1) DrawText(user[0]);
            }
        }
    }
}
