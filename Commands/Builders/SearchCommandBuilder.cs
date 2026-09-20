using System.CommandLine;
using userdb.Models;

namespace userdb.Commands.Builders;

public static class SearchCommandBuilder
{
    public static Command Create()
    {
        var command = new Command("search", "Permite buscar usuarios aplicando filtros");

        var byName = new Option<string>("-n", "--name") { Description = "Buscar por nombre" };
        var byAge = new Option<int>("-a", "--age") { Description = "Buscar por edad", DefaultValueFactory = _ => -1 };
        var byStreak = new Option<int>("-s", "--streak") { Description = "Buscar por racha ", DefaultValueFactory = _ => -1 };
        var byFandom = new Option<string>("-f", "--fandom") { Description = "Buscar por fandom" };
        var byAdditionalRole = new Option<string>("-A", "--aditional-role") { Description = "Buscar por rol adicional" };
        var byWantedRole = new Option<string>("-w", "--wanted-role") { Description = "Buscar por rol buscado" };
        var byId = new Option<string>("-u", "--user-id") { Description = "Buscar por ID de usuario" };
        var byDate = new Option<string>("-d", "--date-registered") { Description = "Buscar por fecha de registro" };
        var byPronoun = new Option<string>("-p", "--pronoun") { Description = "Buscar por pronombres" };
        var byStatus = new Option<string>("-S", "--status") { Description = "Buscar por status" };
        var minAgeFilter = new Option<int>("--min-age") { Description = "Edad minima permitida para mostrar", DefaultValueFactory = _ => -1 };
        var maxAgeFilter = new Option<int>("--max-age") { Description = "Edad máxima permitida para mostrar", DefaultValueFactory = _ => -1 };
        var minStreakFilter = new Option<int>("--min-streak") { Description = "Racha minima permitida para mostrar", DefaultValueFactory = _ => -1 };
        var maxStreakFilter = new Option<int>("--max-streak") { Description = "Racha máxima permitida para mostrar", DefaultValueFactory = _ => -1 };
        var minDateFilter = new Option<string>("--min-date") { Description = "Fecha minima permitida para mostrar" };
        var maxDateFilter = new Option<string>("--max-date") { Description = "Fecha máxima permitida para mostrar" };
        var fastSearchModifier = new Option<bool>("--fast") { Description = "Mejora la velocidad en búsquedas por nombre y por ID" };
        var inverseOrderModifier = new Option<bool>("--inverse-order") { Description = "Invierte el orden" };
        var rawModifier = new Option<bool>("-r", "--raw") { Description = "Imprime las líneas de índice de las coincidencias en formato raw"};

        var options = new Option[] {
            byName, byAge, byStreak, byFandom, byAdditionalRole, byId, byDate, byPronoun,
            byStatus, minAgeFilter, maxAgeFilter, minStreakFilter, maxStreakFilter,
            minDateFilter, maxDateFilter, fastSearchModifier, inverseOrderModifier,
            rawModifier, byWantedRole
        };

        foreach (var option in options)
        {
            command.Add(option);
        }

        command.SetAction((ParseResult parseResult) =>
        {
            var searchParams = new SearchParameters
            {
                Name = parseResult.GetValue(byName),
                Age = parseResult.GetValue(byAge),
                Streak = parseResult.GetValue(byStreak),
                Fandom = parseResult.GetValue(byFandom),
                AdditionalRole = parseResult.GetValue(byAdditionalRole),
                WantedRole = parseResult.GetValue(byWantedRole),
                Id = parseResult.GetValue(byId),
                Date = parseResult.GetValue(byDate),
                Pronoun = parseResult.GetValue(byPronoun),
                Status = parseResult.GetValue(byStatus),
                MinAge = parseResult.GetValue(minAgeFilter),
                MaxAge = parseResult.GetValue(maxAgeFilter),
                MinStreak = parseResult.GetValue(minStreakFilter),
                MaxStreak = parseResult.GetValue(maxStreakFilter),
                MinDate = parseResult.GetValue(minDateFilter),
                MaxDate = parseResult.GetValue(maxDateFilter),
                FastSearch = parseResult.GetValue(fastSearchModifier),
                InverseOrder = parseResult.GetValue(inverseOrderModifier),
                RawModifier = parseResult.GetValue(rawModifier)
            };
            Commands.SearchUser.Run(searchParams);
        });

        return command;
    }
}
