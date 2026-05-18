using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.Services
{
    public static class TournamentMetadata
    {
        private static readonly IReadOnlyDictionary<int, TournamentDetailsDto> DetailsByYear =
            new Dictionary<int, TournamentDetailsDto>
            {
                [2011] = new TournamentDetailsDto
                {
                    Year = 2011, Discipline = "Dota 2", Period = "2011", Frequency = "Ежегодный",
                    Location = "Кёльн, Германия", LocationFlag = "DE", Venue = "Gamescom",
                    Dates = "17-21 августа 2011", PrizePoolTotal = "$1,600,000", Participants = 16,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Natus Vincere",
                    ChampionFlag = "UA", RunnerUp = "EHOME", MVP = "-", Version = "6.72",
                    Matches = 57, Heroes = 96, AverageGPM = 425, AverageXPM = 485
                },
                [2012] = new TournamentDetailsDto
                {
                    Year = 2012, Discipline = "Dota 2", Period = "2012", Frequency = "Ежегодный",
                    Location = "Сиэтл, США", LocationFlag = "US", Venue = "Benaroya Hall",
                    Dates = "31 августа - 2 сентября 2012", PrizePoolTotal = "$1,600,000", Participants = 16,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Invictus Gaming",
                    ChampionFlag = "CN", RunnerUp = "Natus Vincere", MVP = "Ferrari_430", Version = "6.74",
                    Matches = 62, Heroes = 101, AverageGPM = 445, AverageXPM = 502
                },
                [2013] = new TournamentDetailsDto
                {
                    Year = 2013, Discipline = "Dota 2", Period = "2013", Frequency = "Ежегодный",
                    Location = "Сиэтл, США", LocationFlag = "US", Venue = "Benaroya Hall",
                    Dates = "7-11 августа 2013", PrizePoolTotal = "$2,874,380", Participants = 16,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "The Alliance",
                    ChampionFlag = "SE", RunnerUp = "Natus Vincere", MVP = "s4", Version = "6.78",
                    Matches = 71, Heroes = 105, AverageGPM = 462, AverageXPM = 518
                },
                [2014] = new TournamentDetailsDto
                {
                    Year = 2014, Discipline = "Dota 2", Period = "2014", Frequency = "Ежегодный",
                    Location = "Сиэтл, США", LocationFlag = "US", Venue = "KeyArena",
                    Dates = "18-21 июля 2014", PrizePoolTotal = "$10,923,977", Participants = 16,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Newbee",
                    ChampionFlag = "CN", RunnerUp = "Vici Gaming", MVP = "Hao", Version = "6.81",
                    Matches = 73, Heroes = 107, AverageGPM = 478, AverageXPM = 535
                },
                [2015] = new TournamentDetailsDto
                {
                    Year = 2015, Discipline = "Dota 2", Period = "2015", Frequency = "Ежегодный",
                    Location = "Сиэтл, США", LocationFlag = "US", Venue = "KeyArena",
                    Dates = "3-8 августа 2015", PrizePoolTotal = "$18,429,613", Participants = 16,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Evil Geniuses",
                    ChampionFlag = "US", RunnerUp = "CDEC Gaming", MVP = "SumaiL", Version = "6.84",
                    Matches = 81, Heroes = 111, AverageGPM = 495, AverageXPM = 552
                },
                [2016] = new TournamentDetailsDto
                {
                    Year = 2016, Discipline = "Dota 2", Period = "2016", Frequency = "Ежегодный",
                    Location = "Сиэтл, США", LocationFlag = "US", Venue = "KeyArena",
                    Dates = "3-13 августа 2016", PrizePoolTotal = "$20,770,460", Participants = 16,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Wings Gaming",
                    ChampionFlag = "CN", RunnerUp = "Digital Chaos", MVP = "shadow", Version = "6.88",
                    Matches = 94, Heroes = 113, AverageGPM = 512, AverageXPM = 568
                },
                [2017] = new TournamentDetailsDto
                {
                    Year = 2017, Discipline = "Dota 2", Period = "2017", Frequency = "Ежегодный",
                    Location = "Сиэтл, США", LocationFlag = "US", Venue = "KeyArena",
                    Dates = "7-12 августа 2017", PrizePoolTotal = "$24,787,916", Participants = 18,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Team Liquid",
                    ChampionFlag = "EU", RunnerUp = "Newbee", MVP = "Miracle-", Version = "7.06",
                    Matches = 105, Heroes = 114, AverageGPM = 528, AverageXPM = 585
                },
                [2018] = new TournamentDetailsDto
                {
                    Year = 2018, Discipline = "Dota 2", Period = "2018", Frequency = "Ежегодный",
                    Location = "Ванкувер, Канада", LocationFlag = "CA", Venue = "Rogers Arena",
                    Dates = "20-25 августа 2018", PrizePoolTotal = "$25,532,177", Participants = 18,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "OG",
                    ChampionFlag = "EU", RunnerUp = "PSG.LGD", MVP = "ana", Version = "7.19",
                    Matches = 114, Heroes = 115, AverageGPM = 545, AverageXPM = 602
                },
                [2019] = new TournamentDetailsDto
                {
                    Year = 2019, Discipline = "Dota 2", Period = "2019", Frequency = "Ежегодный",
                    Location = "Шанхай, Китай", LocationFlag = "CN", Venue = "Mercedes-Benz Arena",
                    Dates = "20-25 августа 2019", PrizePoolTotal = "$34,330,068", Participants = 18,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "OG",
                    ChampionFlag = "EU", RunnerUp = "Team Liquid", MVP = "N0tail", Version = "7.22",
                    Matches = 124, Heroes = 117, AverageGPM = 562, AverageXPM = 618
                },
                [2021] = new TournamentDetailsDto
                {
                    Year = 2021, Discipline = "Dota 2", Period = "2021", Frequency = "Ежегодный",
                    Location = "Бухарест, Румыния", LocationFlag = "RO", Venue = "Arena Nationala",
                    Dates = "7-17 октября 2021", PrizePoolTotal = "$40,018,195", Participants = 18,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Team Spirit",
                    ChampionFlag = "RU", RunnerUp = "PSG.LGD", MVP = "Yatoro", Version = "7.30",
                    Matches = 107, Heroes = 122, AverageGPM = 578, AverageXPM = 635
                },
                [2022] = new TournamentDetailsDto
                {
                    Year = 2022, Discipline = "Dota 2", Period = "2022", Frequency = "Ежегодный",
                    Location = "Сингапур", LocationFlag = "SG", Venue = "Suntec Centre",
                    Dates = "15-30 октября 2022", PrizePoolTotal = "$18,930,775", Participants = 20,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Tundra Esports",
                    ChampionFlag = "EU", RunnerUp = "Team Secret", MVP = "33", Version = "7.32",
                    Matches = 109, Heroes = 124, AverageGPM = 595, AverageXPM = 652
                },
                [2023] = new TournamentDetailsDto
                {
                    Year = 2023, Discipline = "Dota 2", Period = "2023", Frequency = "Ежегодный",
                    Location = "Сиэтл, США", LocationFlag = "US", Venue = "Climate Pledge Arena",
                    Dates = "12-29 октября 2023", PrizePoolTotal = "$3,380,455", Participants = 20,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Team Spirit",
                    ChampionFlag = "RU", RunnerUp = "Gaimin Gladiators", MVP = "Collapse", Version = "7.34",
                    Matches = 104, Heroes = 125, AverageGPM = 612, AverageXPM = 668
                },
                [2024] = new TournamentDetailsDto
                {
                    Year = 2024, Discipline = "Dota 2", Period = "2024", Frequency = "Ежегодный",
                    Location = "Копенгаген, Дания", LocationFlag = "DK", Venue = "Royal Arena",
                    Dates = "4-15 сентября 2024", PrizePoolTotal = "$2,776,566", Participants = 16,
                    Format = "Group Stage + Playoffs", Organizer = "Valve", Champion = "Team Liquid",
                    ChampionFlag = "EU", RunnerUp = "Gaimin Gladiators", MVP = "miCKe", Version = "7.37",
                    Matches = 89, Heroes = 126, AverageGPM = 628, AverageXPM = 685
                },
                [2025] = new TournamentDetailsDto
                {
                    Year = 2025, Discipline = "Dota 2", Period = "2025", Frequency = "Ежегодный",
                    Location = "Гамбург, Германия", LocationFlag = "DE", Venue = "Barclays Arena",
                    Dates = "4-14 сентября 2025", PrizePoolTotal = "$2,881,791", Participants = 16,
                    Format = "Swiss System + Playoffs", Organizer = "Valve", Champion = "Team Falcons",
                    ChampionFlag = "SA", RunnerUp = "Xtreme Gaming", MVP = "ATF", Version = "7.39d",
                    Matches = 95, Heroes = 127, AverageGPM = 645, AverageXPM = 702
                }
            };

        public static TournamentDetailsDto GetDetails(Tournament tournament)
        {
            if (DetailsByYear.TryGetValue(tournament.Year, out var details))
            {
                return details;
            }

            return new TournamentDetailsDto
            {
                Year = tournament.Year,
                Period = tournament.Year.ToString(),
                Location = tournament.Location ?? "-",
                LocationFlag = "-",
                Venue = "-",
                Dates = "-",
                PrizePoolTotal = tournament.PrizePool.HasValue ? $"${tournament.PrizePool.Value:n0}" : "-",
                Participants = 0,
                Format = "-",
                Organizer = "Valve",
                Champion = tournament.ChampionTeam ?? "-",
                ChampionFlag = "-",
                RunnerUp = "-",
                MVP = "-",
                Version = "-"
            };
        }
    }
}
