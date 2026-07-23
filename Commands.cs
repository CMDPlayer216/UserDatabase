using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Console;
using static userdb.ConsoleHelper;
using static userdb.Menus;

namespace userdb;

static class Commands
{
    public static void ListUsers()
    {
        string[] userLines = UserService.GetUserIndexLines();

        if (userLines.Length == 0)
        {
            DrawText("No hay usuarios registrados.", Color.Red);
            return;
        }

        foreach (string userline in userLines)
        {
            string[] user = userline.Split(',');
            if (user.Length > 1) DrawText(user[0]);
        }
    }
}