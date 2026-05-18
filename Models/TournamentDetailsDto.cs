namespace KURS_ASP.NET.Models
{
    public class TournamentDetailsDto
    {
        public int Year { get; set; }
        public string Discipline { get; set; } = "Dota 2";
        public string Period { get; set; } = string.Empty;
        public string Frequency { get; set; } = "Ежегодный";
        public string Location { get; set; } = string.Empty;
        public string LocationFlag { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string Dates { get; set; } = string.Empty;
        public string PrizePoolTotal { get; set; } = string.Empty;
        public int Participants { get; set; }
        public string Format { get; set; } = string.Empty;
        public string Organizer { get; set; } = string.Empty;
        public string Champion { get; set; } = string.Empty;
        public string ChampionFlag { get; set; } = string.Empty;
        public string RunnerUp { get; set; } = string.Empty;
        public string MVP { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public int Matches { get; set; }
        public int Heroes { get; set; }
        public int AverageGPM { get; set; }
        public int AverageXPM { get; set; }
    }
}
