using KURS_ASP.NET.Models;
using KURS_ASP.NET.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            await using var scope = services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

            await context.Database.EnsureCreatedAsync();
            await EnsureSecurityTablesAsync(context);
            await EnsureProjectSchemaAsync(context);

            if (!await context.Tournaments.AnyAsync())
            {
                await SeedTournamentsAsync(context);
            }

            if (!await context.Teams.AnyAsync())
            {
                await SeedTeamsAsync(context);
            }

            if (!await context.Players.AnyAsync())
            {
                await SeedPlayersAsync(context);
            }

            if (!await context.Matches.AnyAsync())
            {
                await SeedMatchesAsync(context);
            }

            if (!await context.Heroes.AnyAsync())
            {
                await SeedHeroesAsync(context);
            }

            if (!await context.Items.AnyAsync())
            {
                await SeedItemsAsync(context);
            }

            await RepairSeedDataAsync(context);
            await SeedUsersAsync(context, configuration, passwordHasher);
        }

        private static async Task SeedTournamentsAsync(ApplicationDbContext context)
        {
            context.Tournaments.AddRange(
                new Tournament { TournamentName = "The International 2011", Year = 2011, PrizePool = 1.600000m, Location = "Кёльн, Германия", ChampionTeam = "Natus Vincere" },
                new Tournament { TournamentName = "The International 2012", Year = 2012, PrizePool = 1.600000m, Location = "Сиэтл, США", ChampionTeam = "Invictus Gaming" },
                new Tournament { TournamentName = "The International 2013", Year = 2013, PrizePool = 2.874380m, Location = "Сиэтл, США", ChampionTeam = "The Alliance" },
                new Tournament { TournamentName = "The International 2014", Year = 2014, PrizePool = 10.923977m, Location = "Сиэтл, США", ChampionTeam = "Newbee" },
                new Tournament { TournamentName = "The International 2015", Year = 2015, PrizePool = 18.429613m, Location = "Сиэтл, США", ChampionTeam = "Evil Geniuses" },
                new Tournament { TournamentName = "The International 2016", Year = 2016, PrizePool = 20.770460m, Location = "Сиэтл, США", ChampionTeam = "Wings Gaming" },
                new Tournament { TournamentName = "The International 2017", Year = 2017, PrizePool = 24.787916m, Location = "Сиэтл, США", ChampionTeam = "Team Liquid" },
                new Tournament { TournamentName = "The International 2018", Year = 2018, PrizePool = 25.532177m, Location = "Ванкувер, Канада", ChampionTeam = "OG" },
                new Tournament { TournamentName = "The International 2019", Year = 2019, PrizePool = 34.330068m, Location = "Шанхай, Китай", ChampionTeam = "OG" },
                new Tournament { TournamentName = "The International 2021", Year = 2021, PrizePool = 40.018195m, Location = "Бухарест, Румыния", ChampionTeam = "Team Spirit" },
                new Tournament { TournamentName = "The International 2022", Year = 2022, PrizePool = 18.930775m, Location = "Сингапур", ChampionTeam = "Tundra Esports" },
                new Tournament { TournamentName = "The International 2023", Year = 2023, PrizePool = 3.380455m, Location = "Сиэтл, США", ChampionTeam = "Team Spirit" },
                new Tournament { TournamentName = "The International 2024", Year = 2024, PrizePool = 2.776566m, Location = "Копенгаген, Дания", ChampionTeam = "Team Liquid" },
                new Tournament { TournamentName = "The International 2025", Year = 2025, PrizePool = 2.881791m, Location = "Гамбург, Германия", ChampionTeam = "Team Falcons" }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedTeamsAsync(ApplicationDbContext context)
        {
            context.Teams.AddRange(
                new Team { TeamName = "Natus Vincere", Country = "Украина", team_url = "https://liquipedia.net/dota2/Natus_Vincere" },
                new Team { TeamName = "EHOME", Country = "Китай", team_url = "https://liquipedia.net/dota2/EHOME" },
                new Team { TeamName = "Invictus Gaming", Country = "Китай", team_url = "https://liquipedia.net/dota2/Invictus_Gaming" },
                new Team { TeamName = "The Alliance", Country = "Швеция", team_url = "https://liquipedia.net/dota2/The_Alliance" },
                new Team { TeamName = "Newbee", Country = "Китай", team_url = "https://liquipedia.net/dota2/Newbee" },
                new Team { TeamName = "Vici Gaming", Country = "Китай", team_url = "https://liquipedia.net/dota2/Vici_Gaming" },
                new Team { TeamName = "Evil Geniuses", Country = "США", team_url = "https://liquipedia.net/dota2/Evil_Geniuses" },
                new Team { TeamName = "CDEC Gaming", Country = "Китай", team_url = "https://liquipedia.net/dota2/CDEC_Gaming" },
                new Team { TeamName = "Wings Gaming", Country = "Китай", team_url = "https://liquipedia.net/dota2/Wings_Gaming" },
                new Team { TeamName = "Digital Chaos", Country = "США", team_url = "https://liquipedia.net/dota2/Digital_Chaos" },
                new Team { TeamName = "Team Liquid", Country = "Европа", team_url = "https://liquipedia.net/dota2/Team_Liquid" },
                new Team { TeamName = "OG", Country = "Европа", team_url = "https://liquipedia.net/dota2/OG" },
                new Team { TeamName = "PSG.LGD", Country = "Китай", team_url = "https://liquipedia.net/dota2/PSG.LGD" },
                new Team { TeamName = "Team Spirit", Country = "Сербия", team_url = "https://liquipedia.net/dota2/Team_Spirit" },
                new Team { TeamName = "Tundra Esports", Country = "Великобритания", team_url = "https://liquipedia.net/dota2/Tundra_Esports" },
                new Team { TeamName = "Team Secret", Country = "Европа", team_url = "https://liquipedia.net/dota2/Team_Secret" },
                new Team { TeamName = "Gaimin Gladiators", Country = "Канада", team_url = "https://liquipedia.net/dota2/Gaimin_Gladiators" },
                new Team { TeamName = "Team Falcons", Country = "Саудовская Аравия", team_url = "https://liquipedia.net/dota2/Team_Falcons" },
                new Team { TeamName = "Xtreme Gaming", Country = "Китай", team_url = "https://liquipedia.net/dota2/Xtreme_Gaming" }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedPlayersAsync(ApplicationDbContext context)
        {
            var teams = await context.Teams.ToDictionaryAsync(t => t.TeamName ?? string.Empty);

            context.Players.AddRange(
                CreatePlayer("Dendi", "Danil Ishutin", "Украина", "Mid", "Natus Vincere", "https://liquipedia.net/dota2/Dendi", teams),
                CreatePlayer("Puppey", "Clement Ivanov", "Эстония", "Support", "Natus Vincere", "https://liquipedia.net/dota2/Puppey", teams),
                CreatePlayer("Ferrari_430", "Luo Feichi", "Китай", "Mid", "Invictus Gaming", "https://liquipedia.net/dota2/Ferrari_430", teams),
                CreatePlayer("Faith", "Zeng Hongda", "Китай", "Support", "Invictus Gaming", "https://liquipedia.net/dota2/Faith", teams),
                CreatePlayer("s4", "Gustav Magnusson", "Швеция", "Mid", "The Alliance", "https://liquipedia.net/dota2/S4", teams),
                CreatePlayer("Loda", "Jonathan Berg", "Швеция", "Carry", "The Alliance", "https://liquipedia.net/dota2/Loda", teams),
                CreatePlayer("Hao", "Chen Zhihao", "Китай", "Carry", "Newbee", "https://liquipedia.net/dota2/Hao", teams),
                CreatePlayer("xiao8", "Zhang Ning", "Китай", "Offlane", "Newbee", "https://liquipedia.net/dota2/Xiao8", teams),
                CreatePlayer("Fear", "Clinton Loomis", "США", "Carry", "Evil Geniuses", "https://liquipedia.net/dota2/Fear", teams),
                CreatePlayer("SumaiL", "Sumail Hassan", "Пакистан", "Mid", "Evil Geniuses", "https://liquipedia.net/dota2/SumaiL", teams),
                CreatePlayer("shadow", "Chu Zeyu", "Китай", "Carry", "Wings Gaming", "https://liquipedia.net/dota2/Shadow", teams),
                CreatePlayer("Faith_bian", "Zhang Ruida", "Китай", "Offlane", "Wings Gaming", "https://liquipedia.net/dota2/Faith_bian", teams),
                CreatePlayer("MATUMBAMAN", "Lasse Urpalainen", "Финляндия", "Carry", "Team Liquid", "https://liquipedia.net/dota2/MATUMBAMAN", teams),
                CreatePlayer("Miracle-", "Amer Al-Barkawi", "Иордания", "Mid", "Team Liquid", "https://liquipedia.net/dota2/Miracle-", teams),
                CreatePlayer("ana", "Anathan Pham", "Австралия", "Carry", "OG", "https://liquipedia.net/dota2/Ana", teams),
                CreatePlayer("N0tail", "Johan Sundstein", "Дания", "Support", "OG", "https://liquipedia.net/dota2/N0tail", teams),
                CreatePlayer("Yatoro", "Illya Mulyarchuk", "Украина", "Carry", "Team Spirit", "https://liquipedia.net/dota2/Yatoro", teams),
                CreatePlayer("Collapse", "Magomed Khalilov", "Россия", "Offlane", "Team Spirit", "https://liquipedia.net/dota2/Collapse", teams),
                CreatePlayer("33", "Neta Shapira", "Израиль", "Offlane", "Tundra Esports", "https://liquipedia.net/dota2/33", teams),
                CreatePlayer("miCKe", "Michael Vu", "Швеция", "Carry", "Team Liquid", "https://liquipedia.net/dota2/MiCKe", teams),
                CreatePlayer("ATF", "Ammar Al-Assaf", "Иордания", "Offlane", "Team Falcons", "https://liquipedia.net/dota2/ATF", teams),
                CreatePlayer("skiter", "Oliver Lepko", "Словакия", "Carry", "Team Falcons", "https://liquipedia.net/dota2/Skiter", teams)
            );

            await context.SaveChangesAsync();
            await EnsureRosterMembershipsAsync(context);
        }

        private static async Task SeedMatchesAsync(ApplicationDbContext context)
        {
            var tournaments = await context.Tournaments.ToDictionaryAsync(t => t.Year);
            var teams = await context.Teams.ToDictionaryAsync(t => t.TeamName ?? string.Empty);
            var players = await context.Players.ToDictionaryAsync(p => p.Nickname ?? string.Empty);

            context.Matches.AddRange(
                CreateMatch(2011, "Natus Vincere", "EHOME", "Dendi", "2011-08-21", tournaments, teams, players),
                CreateMatch(2012, "Invictus Gaming", "Natus Vincere", "Ferrari_430", "2012-09-02", tournaments, teams, players),
                CreateMatch(2013, "The Alliance", "Natus Vincere", "s4", "2013-08-11", tournaments, teams, players),
                CreateMatch(2014, "Newbee", "Vici Gaming", "Hao", "2014-07-21", tournaments, teams, players),
                CreateMatch(2015, "Evil Geniuses", "CDEC Gaming", "SumaiL", "2015-08-08", tournaments, teams, players),
                CreateMatch(2016, "Wings Gaming", "Digital Chaos", "Faith_bian", "2016-08-13", tournaments, teams, players),
                CreateMatch(2017, "Team Liquid", "Newbee", "Miracle-", "2017-08-12", tournaments, teams, players),
                CreateMatch(2018, "OG", "PSG.LGD", "ana", "2018-08-25", tournaments, teams, players),
                CreateMatch(2019, "OG", "Team Liquid", "N0tail", "2019-08-25", tournaments, teams, players),
                CreateMatch(2021, "Team Spirit", "PSG.LGD", "Yatoro", "2021-10-17", tournaments, teams, players),
                CreateMatch(2022, "Tundra Esports", "Team Secret", "33", "2022-10-30", tournaments, teams, players),
                CreateMatch(2023, "Team Spirit", "Gaimin Gladiators", "Collapse", "2023-10-29", tournaments, teams, players),
                CreateMatch(2024, "Team Liquid", "Gaimin Gladiators", "miCKe", "2024-09-15", tournaments, teams, players),
                CreateMatch(2025, "Team Falcons", "Xtreme Gaming", "ATF", "2025-09-14", tournaments, teams, players)
            );

            await context.SaveChangesAsync();
        }

        private static Match CreateMatch(
            int year,
            string winner,
            string runnerUp,
            string mvp,
            string date,
            IReadOnlyDictionary<int, Tournament> tournaments,
            IReadOnlyDictionary<string, Team> teams,
            IReadOnlyDictionary<string, Player> players)
        {
            return new Match
            {
                Stage = "Grand Final",
                MatchDate = DateTime.Parse(date),
                TournamentID = tournaments[year].TournamentID,
                Team1ID = teams[winner].TeamID,
                Team2ID = teams[runnerUp].TeamID,
                WinnerTeamID = teams[winner].TeamID,
                MVPPlayerID = players[mvp].PlayerID,
                match_url = $"https://liquipedia.net/dota2/The_International/{year}"
            };
        }

        private static async Task SeedHeroesAsync(ApplicationDbContext context)
        {
            context.Heroes.AddRange(
                new Hero { HeroName = "Axe", PrimaryAttribute = "Strength", AttackType = "Melee", StrengthGain = 2.8m, AgilityGain = 1.7m, IntelligenceGain = 1.6m },
                new Hero { HeroName = "Juggernaut", PrimaryAttribute = "Agility", AttackType = "Melee", StrengthGain = 2.0m, AgilityGain = 2.8m, IntelligenceGain = 1.4m },
                new Hero { HeroName = "Invoker", PrimaryAttribute = "Intelligence", AttackType = "Ranged", StrengthGain = 2.4m, AgilityGain = 1.9m, IntelligenceGain = 4.6m },
                new Hero { HeroName = "Rubick", PrimaryAttribute = "Intelligence", AttackType = "Ranged", StrengthGain = 2.0m, AgilityGain = 2.5m, IntelligenceGain = 3.7m },
                new Hero { HeroName = "Marci", PrimaryAttribute = "Universal", AttackType = "Melee", StrengthGain = 3.3m, AgilityGain = 2.4m, IntelligenceGain = 1.9m }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedItemsAsync(ApplicationDbContext context)
        {
            context.Items.AddRange(
                new Item { ItemName = "Tango", Cost = 90, Category = "Consumables", Subcategory = "Regeneration", Description = "Восстанавливает здоровье.", IsNeutral = false, IsRemoved = false },
                new Item { ItemName = "Boots of Speed", Cost = 500, Category = "Boots", Subcategory = "Movement", Description = "Увеличивает скорость передвижения.", IsNeutral = false, IsRemoved = false },
                new Item { ItemName = "Blink Dagger", Cost = 2250, Category = "Utility", Subcategory = "Mobility", Description = "Мгновенное перемещение на короткую дистанцию.", IsNeutral = false, IsRemoved = false },
                new Item { ItemName = "Black King Bar", Cost = 4050, Category = "Defense", Subcategory = "Core", Description = "Даёт защиту от магии.", IsNeutral = false, IsRemoved = false },
                new Item { ItemName = "Aghanim's Scepter", Cost = 4200, Category = "Magic", Subcategory = "Upgrade", Description = "Улучшает способности героя.", IsNeutral = false, IsRemoved = false },
                new Item { ItemName = "Arcane Ring", Cost = 0, Category = "Neutral", Subcategory = "Tier 1", Description = "Нейтральный предмет для маны.", IsNeutral = true, NeutralTier = 1, IsRemoved = false },
                new Item { ItemName = "Vambrace", Cost = 0, Category = "Neutral", Subcategory = "Tier 2", Description = "Нейтральный предмет на атрибуты.", IsNeutral = true, NeutralTier = 2, IsRemoved = false }
            );

            await context.SaveChangesAsync();
        }

        private static async Task RepairSeedDataAsync(ApplicationDbContext context)
        {
            var tournamentLocations = new Dictionary<int, string>
            {
                [2011] = "Кёльн, Германия",
                [2012] = "Сиэтл, США",
                [2013] = "Сиэтл, США",
                [2014] = "Сиэтл, США",
                [2015] = "Сиэтл, США",
                [2016] = "Сиэтл, США",
                [2017] = "Сиэтл, США",
                [2018] = "Ванкувер, Канада",
                [2019] = "Шанхай, Китай",
                [2021] = "Бухарест, Румыния",
                [2022] = "Сингапур",
                [2023] = "Сиэтл, США",
                [2024] = "Копенгаген, Дания",
                [2025] = "Гамбург, Германия"
            };

            var teamCountries = new Dictionary<string, string>
            {
                ["Natus Vincere"] = "Украина",
                ["EHOME"] = "Китай",
                ["Invictus Gaming"] = "Китай",
                ["The Alliance"] = "Швеция",
                ["Newbee"] = "Китай",
                ["Vici Gaming"] = "Китай",
                ["Evil Geniuses"] = "США",
                ["CDEC Gaming"] = "Китай",
                ["Wings Gaming"] = "Китай",
                ["Digital Chaos"] = "США",
                ["Team Liquid"] = "Европа",
                ["OG"] = "Европа",
                ["PSG.LGD"] = "Китай",
                ["Team Spirit"] = "Сербия",
                ["Tundra Esports"] = "Великобритания",
                ["Team Secret"] = "Европа",
                ["Gaimin Gladiators"] = "Канада",
                ["Team Falcons"] = "Саудовская Аравия",
                ["Xtreme Gaming"] = "Китай"
            };

            var playerCountries = new Dictionary<string, string>
            {
                ["Dendi"] = "Украина",
                ["Puppey"] = "Эстония",
                ["Ferrari_430"] = "Китай",
                ["Faith"] = "Китай",
                ["s4"] = "Швеция",
                ["Loda"] = "Швеция",
                ["Hao"] = "Китай",
                ["xiao8"] = "Китай",
                ["Fear"] = "США",
                ["SumaiL"] = "Пакистан",
                ["shadow"] = "Китай",
                ["Faith_bian"] = "Китай",
                ["MATUMBAMAN"] = "Финляндия",
                ["Miracle-"] = "Иордания",
                ["ana"] = "Австралия",
                ["N0tail"] = "Дания",
                ["Yatoro"] = "Украина",
                ["Collapse"] = "Россия",
                ["33"] = "Израиль",
                ["miCKe"] = "Швеция",
                ["ATF"] = "Иордания",
                ["skiter"] = "Словакия"
            };

            var playerLinks = new Dictionary<string, string>
            {
                ["Dendi"] = "https://liquipedia.net/dota2/Dendi",
                ["Puppey"] = "https://liquipedia.net/dota2/Puppey",
                ["Ferrari_430"] = "https://liquipedia.net/dota2/Ferrari_430",
                ["Faith"] = "https://liquipedia.net/dota2/Faith",
                ["s4"] = "https://liquipedia.net/dota2/S4",
                ["Loda"] = "https://liquipedia.net/dota2/Loda",
                ["Hao"] = "https://liquipedia.net/dota2/Hao",
                ["xiao8"] = "https://liquipedia.net/dota2/Xiao8",
                ["Fear"] = "https://liquipedia.net/dota2/Fear",
                ["SumaiL"] = "https://liquipedia.net/dota2/SumaiL",
                ["shadow"] = "https://liquipedia.net/dota2/Shadow",
                ["Faith_bian"] = "https://liquipedia.net/dota2/Faith_bian",
                ["MATUMBAMAN"] = "https://liquipedia.net/dota2/MATUMBAMAN",
                ["Miracle-"] = "https://liquipedia.net/dota2/Miracle-",
                ["ana"] = "https://liquipedia.net/dota2/Ana",
                ["N0tail"] = "https://liquipedia.net/dota2/N0tail",
                ["Yatoro"] = "https://liquipedia.net/dota2/Yatoro",
                ["Collapse"] = "https://liquipedia.net/dota2/Collapse",
                ["33"] = "https://liquipedia.net/dota2/33",
                ["miCKe"] = "https://liquipedia.net/dota2/MiCKe",
                ["ATF"] = "https://liquipedia.net/dota2/ATF",
                ["skiter"] = "https://liquipedia.net/dota2/Skiter"
            };

            var itemDescriptions = new Dictionary<string, string>
            {
                ["Tango"] = "Восстанавливает здоровье.",
                ["Boots of Speed"] = "Увеличивает скорость передвижения.",
                ["Blink Dagger"] = "Мгновенное перемещение на короткую дистанцию.",
                ["Black King Bar"] = "Даёт защиту от магии.",
                ["Aghanim's Scepter"] = "Улучшает способности героя.",
                ["Arcane Ring"] = "Нейтральный предмет для маны.",
                ["Vambrace"] = "Нейтральный предмет на атрибуты."
            };

            var hasChanges = false;

            var tournaments = await context.Tournaments.ToListAsync();
            foreach (var tournament in tournaments)
            {
                if (tournament.Year != 0 && tournamentLocations.TryGetValue(tournament.Year, out var location) && tournament.Location != location)
                {
                    tournament.Location = location;
                    hasChanges = true;
                }
            }

            var teams = await context.Teams.ToListAsync();
            foreach (var team in teams)
            {
                if (!string.IsNullOrWhiteSpace(team.TeamName) && teamCountries.TryGetValue(team.TeamName, out var country) && team.Country != country)
                {
                    team.Country = country;
                    hasChanges = true;
                }
            }

            var players = await context.Players.ToListAsync();
            foreach (var player in players)
            {
                if (!string.IsNullOrWhiteSpace(player.Nickname) && playerCountries.TryGetValue(player.Nickname, out var country) && player.Country != country)
                {
                    player.Country = country;
                    hasChanges = true;
                }

                if (!string.IsNullOrWhiteSpace(player.Nickname) && playerLinks.TryGetValue(player.Nickname, out var link) && player.player_url != link)
                {
                    player.player_url = link;
                    hasChanges = true;
                }
            }

            var items = await context.Items.ToListAsync();
            foreach (var item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.ItemName) && itemDescriptions.TryGetValue(item.ItemName, out var description) && item.Description != description)
                {
                    item.Description = description;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await context.SaveChangesAsync();
            }

            await RepairPlayerTeamLinksAsync(context);
            await EnsureRosterMembershipsAsync(context);
        }

        private static async Task EnsureSecurityTablesAsync(ApplicationDbContext context)
        {
            const string sql = @"
IF OBJECT_ID(N'dbo.AppUsers', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AppUsers]
    (
        [AppUserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserName] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(120) NOT NULL,
        [PasswordHash] NVARCHAR(512) NOT NULL,
        [Role] NVARCHAR(20) NOT NULL,
        [AvatarUrl] NVARCHAR(260) NULL,
        [CreatedAtUtc] DATETIME2 NOT NULL,
        [IsActive] BIT NOT NULL
    );
END;

IF COL_LENGTH(N'dbo.AppUsers', N'AvatarUrl') IS NULL
BEGIN
    ALTER TABLE [dbo].[AppUsers] ADD [AvatarUrl] NVARCHAR(260) NULL;
END;

IF COL_LENGTH(N'dbo.AppUsers', N'ExternalProvider') IS NULL
BEGIN
    ALTER TABLE [dbo].[AppUsers] ADD [ExternalProvider] NVARCHAR(50) NULL;
END;

IF COL_LENGTH(N'dbo.AppUsers', N'ExternalSubject') IS NULL
BEGIN
    ALTER TABLE [dbo].[AppUsers] ADD [ExternalSubject] NVARCHAR(200) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AppUsers_UserName' AND object_id = OBJECT_ID(N'dbo.AppUsers'))
BEGIN
    CREATE UNIQUE INDEX [IX_AppUsers_UserName] ON [dbo].[AppUsers]([UserName]);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AppUsers_Email' AND object_id = OBJECT_ID(N'dbo.AppUsers'))
BEGIN
    CREATE UNIQUE INDEX [IX_AppUsers_Email] ON [dbo].[AppUsers]([Email]);
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AppUsers_ExternalProvider_ExternalSubject' AND object_id = OBJECT_ID(N'dbo.AppUsers'))
BEGIN
    CREATE UNIQUE INDEX [IX_AppUsers_ExternalProvider_ExternalSubject]
    ON [dbo].[AppUsers]([ExternalProvider], [ExternalSubject])
    WHERE [ExternalProvider] IS NOT NULL AND [ExternalSubject] IS NOT NULL;
END;";

            await context.Database.ExecuteSqlRawAsync(sql);
        }

        private static async Task EnsureProjectSchemaAsync(ApplicationDbContext context)
        {
            const string sql = @"
IF COL_LENGTH(N'dbo.Players', N'TeamId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Players] ADD [TeamId] INT NULL;
END;

IF COL_LENGTH(N'dbo.Players', N'AvatarUrl') IS NULL
BEGIN
    ALTER TABLE [dbo].[Players] ADD [AvatarUrl] NVARCHAR(260) NULL;
END;

IF COL_LENGTH(N'dbo.Heroes', N'AvatarUrl') IS NULL
BEGIN
    ALTER TABLE [dbo].[Heroes] ADD [AvatarUrl] NVARCHAR(260) NULL;
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Players_TeamId' AND object_id = OBJECT_ID(N'dbo.Players'))
BEGIN
    CREATE INDEX [IX_Players_TeamId] ON [dbo].[Players]([TeamId]);
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Players_Teams_TeamId')
BEGIN
    ALTER TABLE [dbo].[Players]
    ADD CONSTRAINT [FK_Players_Teams_TeamId]
    FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams]([TeamID]);
END;

IF OBJECT_ID(N'dbo.RosterMemberships', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RosterMemberships]
    (
        [RosterMembershipId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [PlayerId] INT NOT NULL,
        [TeamId] INT NOT NULL,
        [StartYear] INT NOT NULL,
        [EndYear] INT NULL,
        [IsCurrent] BIT NOT NULL CONSTRAINT [DF_RosterMemberships_IsCurrent] DEFAULT(1),
        [Label] NVARCHAR(120) NULL,
        CONSTRAINT [FK_RosterMemberships_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerID]) ON DELETE CASCADE,
        CONSTRAINT [FK_RosterMemberships_Teams_TeamId] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams]([TeamID]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RosterMemberships_PlayerId_TeamId_StartYear' AND object_id = OBJECT_ID(N'dbo.RosterMemberships'))
BEGIN
    CREATE INDEX [IX_RosterMemberships_PlayerId_TeamId_StartYear] ON [dbo].[RosterMemberships]([PlayerId], [TeamId], [StartYear]);
END;

IF OBJECT_ID(N'dbo.TournamentComments', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[TournamentComments]
    (
        [TournamentCommentId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [TournamentId] INT NOT NULL,
        [AppUserId] INT NOT NULL,
        [Content] NVARCHAR(1000) NOT NULL,
        [CreatedAtUtc] DATETIME2 NOT NULL,
        CONSTRAINT [FK_TournamentComments_Tournaments_TournamentId] FOREIGN KEY ([TournamentId]) REFERENCES [dbo].[Tournaments]([TournamentID]) ON DELETE CASCADE,
        CONSTRAINT [FK_TournamentComments_AppUsers_AppUserId] FOREIGN KEY ([AppUserId]) REFERENCES [dbo].[AppUsers]([AppUserId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TournamentComments_TournamentId_CreatedAtUtc' AND object_id = OBJECT_ID(N'dbo.TournamentComments'))
BEGIN
    CREATE INDEX [IX_TournamentComments_TournamentId_CreatedAtUtc] ON [dbo].[TournamentComments]([TournamentId], [CreatedAtUtc]);
END;

IF OBJECT_ID(N'dbo.AdminActionLogs', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AdminActionLogs]
    (
        [AdminActionLogId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AdminUserId] INT NOT NULL,
        [ActionType] NVARCHAR(60) NOT NULL,
        [EntityType] NVARCHAR(60) NOT NULL,
        [EntityId] INT NULL,
        [Description] NVARCHAR(400) NULL,
        [CreatedAtUtc] DATETIME2 NOT NULL,
        CONSTRAINT [FK_AdminActionLogs_AppUsers_AdminUserId] FOREIGN KEY ([AdminUserId]) REFERENCES [dbo].[AppUsers]([AppUserId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AdminActionLogs_CreatedAtUtc' AND object_id = OBJECT_ID(N'dbo.AdminActionLogs'))
BEGIN
    CREATE INDEX [IX_AdminActionLogs_CreatedAtUtc] ON [dbo].[AdminActionLogs]([CreatedAtUtc]);
END;";

            await context.Database.ExecuteSqlRawAsync(sql);
        }

        private static async Task SeedUsersAsync(
            ApplicationDbContext context,
            IConfiguration configuration,
            IPasswordHasherService passwordHasher)
        {
            var adminLogin = configuration["SeedAdmin:UserName"] ?? "admin";
            var adminEmail = configuration["SeedAdmin:Email"] ?? "admin@dota-archive.local";
            var adminPassword = configuration["SeedAdmin:Password"] ?? "Admin123!";

            var hasAdmin = await context.AppUsers.AnyAsync(u => u.UserName == adminLogin);
            if (hasAdmin)
            {
                return;
            }

            context.AppUsers.Add(new AppUser
            {
                UserName = adminLogin,
                Email = adminEmail,
                PasswordHash = passwordHasher.HashPassword(adminPassword),
                Role = SecurityRoles.Admin,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        private static Player CreatePlayer(
            string nickname,
            string realName,
            string country,
            string role,
            string teamName,
            string? playerUrl,
            IReadOnlyDictionary<string, Team> teams)
        {
            teams.TryGetValue(teamName, out var team);

            return new Player
            {
                Nickname = nickname,
                RealName = realName,
                Country = country,
                Role = role,
                Team = teamName,
                TeamId = team?.TeamID,
                player_url = playerUrl
            };
        }

        private static async Task RepairPlayerTeamLinksAsync(ApplicationDbContext context)
        {
            var teamsByName = await context.Teams
                .Where(t => !string.IsNullOrWhiteSpace(t.TeamName))
                .ToDictionaryAsync(t => t.TeamName!, StringComparer.OrdinalIgnoreCase);

            var players = await context.Players.ToListAsync();
            var hasChanges = false;

            foreach (var player in players)
            {
                if (!string.IsNullOrWhiteSpace(player.Team) &&
                    teamsByName.TryGetValue(player.Team, out var team))
                {
                    if (player.TeamId != team.TeamID)
                    {
                        player.TeamId = team.TeamID;
                        hasChanges = true;
                    }
                }
                else if (player.TeamId.HasValue && teamsByName.Values.All(t => t.TeamID != player.TeamId.Value))
                {
                    player.TeamId = null;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await context.SaveChangesAsync();
            }
        }

        private static async Task EnsureRosterMembershipsAsync(ApplicationDbContext context)
        {
            var players = await context.Players
                .Where(p => p.TeamId != null)
                .ToListAsync();

            var memberships = await context.RosterMemberships.ToListAsync();
            var hasChanges = false;

            foreach (var player in players)
            {
                var existing = memberships.FirstOrDefault(m =>
                    m.PlayerId == player.PlayerID &&
                    m.TeamId == player.TeamId);

                if (existing == null)
                {
                    context.RosterMemberships.Add(new RosterMembership
                    {
                        PlayerId = player.PlayerID,
                        TeamId = player.TeamId!.Value,
                        StartYear = Math.Max(DateTime.UtcNow.Year - 1, 2011),
                        EndYear = null,
                        IsCurrent = true,
                        Label = "Основной состав"
                    });
                    hasChanges = true;
                }
                else
                {
                    if (!existing.IsCurrent || existing.EndYear != null)
                    {
                        existing.IsCurrent = true;
                        existing.EndYear = null;
                        hasChanges = true;
                    }
                }
            }

            if (hasChanges)
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
