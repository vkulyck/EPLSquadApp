namespace EPLSquadBackend.Mappers
{
    public static class TeamNicknameMapper
    {
        public static readonly Dictionary<string, string> NicknameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "The Hammers", "West Ham United FC" },
            { "The Gunners", "Arsenal FC" },
            { "The Red Devils", "Manchester United FC" },
            { "The Blues", "Chelsea FC" },
            { "The Citizens", "Manchester City FC" },
            { "The Spurs", "Tottenham Hotspur FC" },
            { "The Toffees", "Everton FC" },
            { "The Villans", "Aston Villa FC" },
            { "The Magpies", "Newcastle United FC" },
            { "The Seagulls", "Brighton & Hove Albion FC" },
            { "The Clarets", "Burnley FC" },
            { "The Cherries", "AFC Bournemouth" },
            { "The Blades", "Sheffield United FC" },
            { "The Eagles", "Crystal Palace FC" },
            { "The Reds", "Liverpool FC" },
            { "The Cottagers", "Fulham FC" },
            { "The Hatters", "Luton Town FC" },
            { "The Wolves", "Wolverhampton Wanderers FC" },
            { "The Bees", "Brentford FC" },
            { "The Foxes", "Leicester City FC" }
        };
    }
}
