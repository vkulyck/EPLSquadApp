namespace EPLSquadBackend.Models
{
    public class Team
    {
        public required string Crest { get; set; }
        public required string Name { get; set; }
        public required List<Player> Squad { get; set; }
    }
}
