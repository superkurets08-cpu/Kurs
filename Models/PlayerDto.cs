namespace KURS_ASP.NET.Models
{
    // Класс для игроков - только основная информация
    public class PlayerDto
    {
        public int PlayerID { get; set; }
        public string? Nickname { get; set; }
        public string? RealName { get; set; }
        public string? Country { get; set; }
        public string? Role { get; set; }
        public string? Team { get; set; }
        public string? player_url { get; set; }
    }
}