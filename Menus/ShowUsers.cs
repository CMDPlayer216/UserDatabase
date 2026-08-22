using userdb.Commands;
using userdb.Services;

namespace userdb.Menus;

public static class ShowUsers
{
    public static void Show(bool clearConsole = true)
    {
        Logs.Log("ShowUsers", "Mostrando menú/pantalla de usuarios", Logs.logType.Info, 2);
        if (clearConsole) Console.Clear();
        ListUsers.Run(true, false);
    }
}