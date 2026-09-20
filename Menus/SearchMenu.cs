using userdb.Models;

namespace userdb.Menus;

public static class SearchMenu
{
    public static void Show()
    {
        SearchParameters searchOption = new();

        Console.Clear();

        bool loop = true;

        while (loop)
        {
            DrawText("Por qué atributo deseas buscar?");
            DrawText("[1]  ID");
            DrawText("[2]  Nombre");
            DrawText("[3]  Edad");
            DrawText("[4]  Racha");
            DrawText("[5]  Fandom");
            DrawText("[6]  Status");
            DrawText("[7]  Fecha de registro");
            DrawText("[8]  Pronombre");
            DrawText("[9]  Rol buscado");
            DrawText("[10] Rol adicional");
            DrawText("[0]  Salir");
            DrawText("");
            DrawText("Nota: Mientras más arriba está el elemento, más rápido es buscarlo, si no tienes muchos usuarios, ignora este mensaje");

            string input = TakeInput();

            switch (input)
            {
                case "1":
                    {
                        searchOption.Id = TakeInput("Ingresa ID para buscar: ");
                        searchOption.FastSearch = true;
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "2":
                    {
                        searchOption.Name = TakeInput("Ingresa nombre para buscar: ");
                        searchOption.FastSearch = true;
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        Commands.SearchUser.Run(searchOption);
                        DrawText("");
                        break;
                    }
                case "3":
                    {
                        if (!int.TryParse(TakeInput("Ingresa edad para buscar: "), out int age))
                        {
                            Console.Clear();
                            DrawText("Edad inválida!", Color.Red);
                            DrawText("");
                            break;
                        }
                        searchOption.Age = age;
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "4":
                    {
                        if (!int.TryParse(TakeInput("Ingresa racha para buscar: "), out int streak))
                        {
                            Console.Clear();
                            DrawText("Racha inválida!", Color.Red);
                            DrawText("");
                            break;
                        }
                        searchOption.Age = streak;
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "5":
                    {
                        searchOption.Fandom = TakeInput("Ingresa fandom para buscar: ");
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "6":
                    {
                        searchOption.Status = TakeInput("Ingresa status para buscar: ");
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "7":
                    {
                        searchOption.Date = TakeInput("Ingresa fecha de registro (formato aaaa-MM-dd) para buscar: ");
                        try
                        {
                            DateOnly.Parse(searchOption.Date);
                            Console.Clear();
                            DrawText("============ RESULTADOS ============");
                            DrawText("ID             NOMBRE");
                            DrawText("");
                            Commands.SearchUser.Run(searchOption);
                        }
                        catch
                        {
                            Console.Clear();
                            DrawText("Fecha inválida!", Color.Red);
                            DrawText("");
                        }
                        break;
                    }
                case "8":
                    {
                        searchOption.Pronoun = TakeInput("Ingresa pronombre para buscar: ");
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "9":
                    {
                        searchOption.WantedRole = TakeInput("Ingresa rol para buscar: ");
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "10":
                    {
                        searchOption.AdditionalRole = TakeInput("Ingresa rol para buscar: ");
                        Console.Clear();
                        DrawText("============ RESULTADOS ============");
                        DrawText("ID             NOMBRE");
                        DrawText("");
                        Commands.SearchUser.Run(searchOption);
                        break;
                    }
                case "0":
                    {
                        loop = false;
                        Console.Clear();
                        break;
                    }
                default:
                    {
                        Console.Clear();
                        DrawText("Esa opción no es válida!", Color.Red);
                        DrawText("");
                        break;
                    }
            }
        }
    }
}
